using System;
using System.Drawing;
using System.Management;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace BatteryWatcher
{
    public partial class BatteryWatcher : Form
    {

        #region Globals

        /// <summary>
        /// notify icon for system tray
        /// </summary>
        private readonly NotifyIcon _batteryWatcherIcon;

        /// <summary>
        /// background worker that monitors the battery
        /// </summary>
        private readonly Thread _batteryWatcherBackgroundWorker;

        /// <summary>
        /// enables default window close action if set to true.
        /// </summary>
        private bool _forceClose;
        #endregion

        #region Properties and Fields
        /// <summary>
        /// primary icon for normal state
        /// </summary>
        private Icon _iconPrimary => new Icon(@".\Resources\Images\IconPrimary.ico");

        /// <summary>
        /// danger icon for low battery state
        /// </summary>
        private Icon _iconDanger => new Icon(@".\Resources\Images\IconDanger.ico");

        /// <summary>
        /// warning icon for high battery state
        /// </summary>
        private Icon _iconWarning => new Icon(@".\Resources\Images\IconWarning.ico");

        /// <summary>
        /// flag to disable upper alert
        /// </summary>
        private bool _disableUpperAlert;

        public bool DisableUpperAlert
        {
            get { return _disableUpperAlert; }
            set
            {
                _disableUpperAlert = value;

                //if upper alert is disabler set re-enable timer
                if (value)
                {
                    _upperAlertReEnableTime = DateTime.Now.AddMinutes(5);
                }
            }
        }

        /// <summary>
        /// upper alert re-enable time
        /// </summary>
        private DateTime _upperAlertReEnableTime;


        /// <summary>
        /// flag to disable lower alert
        /// </summary>
        private bool _disableLowerAlert;

        public bool DisableLowerAlert
        {
            get { return _disableLowerAlert; }
            set
            {
                _disableLowerAlert = value;

                //if lower alert is disabled set re-enable timer
                if (value)
                {
                    _lowerAlertReEnableTime = DateTime.Now.AddMinutes(5);
                }
            }
        }

        /// <summary>
        /// lower alert re-enable time
        /// </summary>
        private DateTime _lowerAlertReEnableTime;

        #endregion

        #region Constructor
        public BatteryWatcher()
        {
            this.Closing += BatteryWatcher_Closing;
            InitializeComponent();

            //create notifyIcon
            //assign assign Image and show it
            _batteryWatcherIcon = new NotifyIcon
            {
                Icon = _iconPrimary,
                Visible = true
            };

            //create context menu
            ContextMenu contextMenu = new ContextMenu();
            //create Battery Watcher menu
            MenuItem batteryWatcherMenuItem = new MenuItem("Battery Watcher");
            //create quite menu to quit the application
            MenuItem quitMenuItem = new MenuItem("Quit");
            //create about menu to show about details, this will do nothing
            MenuItem aboutMenuItem = new MenuItem("Battery Watcher Beta 0.1 -Sunil Shahi.");

            //wire quit menu to close application
            quitMenuItem.Click += QuitMenuItem_Click;

            //wire battery watcher menu item to show main window
            batteryWatcherMenuItem.Click += BatteryWatcherMenuItem_Click;

            //add menu items to context menu
            contextMenu.MenuItems.Add(batteryWatcherMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            contextMenu.MenuItems.Add(aboutMenuItem);

            //add context menu to notify icon
            _batteryWatcherIcon.ContextMenu = contextMenu;

            LowerLevelTrackBar.Maximum = 100;
            LowerLevelTrackBar.TickFrequency = 5;
            LowerLevelTrackBar.LargeChange = 5;
            LowerLevelTrackBar.SmallChange = 1;

            UpperLevelTrackBar.Maximum = 100;
            UpperLevelTrackBar.TickFrequency = 5;
            UpperLevelTrackBar.LargeChange = 5;
            UpperLevelTrackBar.SmallChange = 1;

            SetUpperLevelTrackBar();
            SetLowerLevelTrackBar();

            //start battery watcher worker thread 
            _batteryWatcherBackgroundWorker = new Thread(BatteryWatcherBackgroundWorkerTherad);
            _batteryWatcherBackgroundWorker.Start();


            //disable resizing of the window
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// show the battery watcher application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatteryWatcherMenuItem_Click(object sender, EventArgs e)
        {
            //show the main window in normal mode
            this.WindowState = FormWindowState.Normal;
            //show the icon in task bar as well
            this.ShowInTaskbar = true;
        }

        /// <summary>
        /// changes default closing action depending on force close value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatteryWatcher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //hide the window
            this.WindowState = FormWindowState.Minimized;
            //remove from task bar 
            this.ShowInTaskbar = false;
            //disable default close behaviour when force close is true
            if (!_forceClose)
            {
                _forceClose = false;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Close the application when user clicks close on context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            _batteryWatcherBackgroundWorker.Abort();
            _batteryWatcherIcon.Dispose();
            _forceClose = true;
            this.Close();
        }

        /// <summary>
        /// set upper tracker bar and manages lower tracker bar accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpperLevelTrackBar_ValueChanged(object sender, EventArgs e)
        {
            //set value to corresponding textbox
            UpperLevelTextBox.Text = UpperLevelTrackBar.Value.ToString();

            //change lower tracker bar if upper tracker bar is smaller
            if (UpperLevelTrackBar.Value <= LowerLevelTrackBar.Value)
            {
                SetLowerLevelTrackBar(UpperLevelTrackBar.Value);
            }

            ReEnableAlert();
        }

        /// <summary>
        /// set lower tracker bar and manages upper tracker bar accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LowerLevelTrackBar_ValueChanged(object sender, EventArgs e)
        {
            //set value to corresponding textbox
            LowerLevelTextBox.Text = LowerLevelTrackBar.Value.ToString();

            //change upper tracker bar if upper tracker bar is smaller
            if (UpperLevelTrackBar.Value < LowerLevelTrackBar.Value)
            {
                SetUpperLevelTrackBar(LowerLevelTrackBar.Value);
            }

            ReEnableAlert();
        }
        /// <summary>
        /// set upper tracker when corresponding textbox value is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpperLevelTextBox_TextChanged(object sender, EventArgs e)
        {
            int upperLevel;
            //try to convert the string to integer
            bool converted = int.TryParse(UpperLevelTextBox.Text, out upperLevel);
            //change if conversion is successful set back to old value if not
            SetUpperLevelTrackBar(converted ? upperLevel : UpperLevelTrackBar.Value);
        }

        /// <summary>
        /// set lower tracker when corresponding textbox value is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LowerLevelTextBox_TextChanged(object sender, EventArgs e)
        {
            int lowerLevel;
            //try to convert the string to integer
            bool converted = int.TryParse(LowerLevelTextBox.Text, out lowerLevel);
            //change if conversion is successful set back to old value if not
            SetLowerLevelTrackBar(converted ? lowerLevel : LowerLevelTrackBar.Value);
        }
        #endregion

        #region BackgroundWorkerThread
        /// <summary>
        /// main thread that monitors battery
        /// </summary>
        public void BatteryWatcherBackgroundWorkerTherad()
        {
            try
            {
                //get hold of wmi battery info object
                var batteryManagementClass = new ManagementClass("Win32_Battery");
                //get colletion of the instances
                var batteryManagementObjectCollection = batteryManagementClass.GetInstances();
                //load sound alert file
                var soundPlayer = new SoundPlayer(@".\Resources\Sounds\BatteryAlert.wav");



                //main loop where battery is monitored
                while (true)
                {
                    foreach (var managementObject in batteryManagementObjectCollection)
                    {
                        //read value from wmi object abd keep value between 0 and 100 
                        //value can actually go upto 105
                        var batteryLevel = Math.Max(0, Math.Min(Convert.ToInt32(managementObject["EstimatedChargeRemaining"]), 100));

                        //read upper and lower level in thread safe way
                        int upperLevel = (int)this.Invoke(new Func<int>(() => UpperLevelTrackBar.Value));
                        int lowerLevel = (int)this.Invoke(new Func<int>(() => LowerLevelTrackBar.Value));

                        if (batteryLevel >= upperLevel && !DisableUpperAlert)
                        {
                            //change notification tray icon to warning icon
                            _batteryWatcherIcon.Icon = _iconWarning;
                            //play sound
                            soundPlayer.Play();
                            //disable upper alert for default time
                            DisableUpperAlert = true;
                        }
                        else if (batteryLevel <= lowerLevel && !DisableLowerAlert)
                        {
                            //change notification tray icon to danger icon
                            _batteryWatcherIcon.Icon = _iconDanger;
                            //play sound
                            soundPlayer.Play();
                            //disable lower alert for default time
                            DisableLowerAlert = true;
                        }
                        else if (batteryLevel > lowerLevel && batteryLevel < upperLevel)
                        {
                            //change notification tray icon to primary icon
                            _batteryWatcherIcon.Icon = _iconPrimary;
                        }
                    }

                    //check if alerts disable time has passed
                    CheckUpperAlertForReEnable();
                    CheckLowerAlertForReEnable();

                    Thread.Sleep(5000);
                }
            }
            catch (ThreadAbortException)
            {
                //do nothing
            }
        }

        #endregion

        #region Helper methods
        /// <summary>
        /// set upperbar when initialized or when corresponding textbox value is changed
        /// </summary>
        /// <param name="upperLevel"></param>
        private void SetUpperLevelTrackBar(int upperLevel = 95)
        {
            //keep values between 0 and 100
            UpperLevelTrackBar.Value = Math.Max(0, Math.Min(upperLevel, 100));
            //reflect changes in corresponding textbox
            UpperLevelTextBox.Text = UpperLevelTrackBar.Value.ToString();
        }

        /// <summary>
        /// set lowerbar when initialized or when corresponding textbox value is changed
        /// </summary>
        /// <param name="lowerLevel"></param>
        private void SetLowerLevelTrackBar(int lowerLevel = 20)
        {
            //keep values between 0 and 100
            LowerLevelTrackBar.Value = Math.Max(0, Math.Min(lowerLevel, 100));
            //reflect changes in corresponding textbox
            LowerLevelTextBox.Text = LowerLevelTrackBar.Value.ToString();
        }

        /// <summary>
        /// check if upper alert re-enable time as passed. Enable if so.
        /// </summary>
        private void CheckUpperAlertForReEnable()
        {
            DisableUpperAlert = !(DisableUpperAlert && DateTime.Now >= _upperAlertReEnableTime);
        }

        /// <summary>
        /// check if lower alert re-enable time as passed. Enable if so.
        /// </summary>
        private void CheckLowerAlertForReEnable()
        {
            DisableLowerAlert = !(DisableLowerAlert && DateTime.Now >= _lowerAlertReEnableTime);
        }

        /// <summary>
        /// re-enables alert tone when track bar value is changed
        /// </summary>
        private void ReEnableAlert()
        {
            DisableUpperAlert = false;
            DisableLowerAlert = false;
        }
        #endregion
    }
}
