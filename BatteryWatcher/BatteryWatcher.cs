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
        private readonly NotifyIcon _batteryWatcherIcon;
        private readonly Thread _batteryWatcherBackgroundWorker;
        private readonly Icon _iconPrimary;
        private readonly Icon _iconDanger;
        private readonly Icon _iconWarning;
        private bool _disableUpperAlert;
        private bool _disableLowerAlert;
        private DateTime _upperAlertReEnableTime;
        private DateTime _lowerAlertReEnableTime;
        private bool _forceClose;
        #endregion

        #region Constructor
        public BatteryWatcher()
        {
            this.Closing += BatteryWatcher_Closing;
            InitializeComponent();

            //create Icons
            _iconPrimary = new Icon(@".\Resources\Images\IconPrimary.ico");
            _iconDanger = new Icon(@".\Resources\Images\IconDanger.ico");
            _iconWarning = new Icon(@".\Resources\Images\IconWarning.ico");

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

            ////hide the window to minimized state
            ////this is notification tray application
            //this.WindowState = FormWindowState.Minimized;
            ////remove from task bar 
            //this.ShowInTaskbar = false;

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
        /// Hide window back to notification tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideButton_Click(object sender, EventArgs e)
        {
            //hide the window
            this.WindowState = FormWindowState.Minimized;
            //remove from task bar 
            this.ShowInTaskbar = false;
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


                        if (batteryLevel >= upperLevel && !_disableUpperAlert)
                        {
                            _batteryWatcherIcon.Icon = _iconWarning;
                            soundPlayer.Play();
                            DisableUpperAlert();
                        }
                        else if (batteryLevel <= lowerLevel && !_disableLowerAlert)
                        {
                            _batteryWatcherIcon.Icon = _iconDanger;
                            soundPlayer.Play();
                            DisableLowerAlert();
                        }
                        else if (batteryLevel > lowerLevel && batteryLevel < upperLevel)
                        {
                            _batteryWatcherIcon.Icon = _iconPrimary;
                        }
                    }

                    CheckUpperAlertForReEnable();
                    CheckLowerAlertForReEnable();

                    Thread.Sleep(5000);
                }
            }
            catch (ThreadAbortException)
            {

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

        private void DisableUpperAlert()
        {
            _disableUpperAlert = true;
            _upperAlertReEnableTime = DateTime.Now.AddMinutes(5);
        }

        private void DisableLowerAlert()
        {
            _disableLowerAlert = true;
            _lowerAlertReEnableTime = DateTime.Now.AddMinutes(5);
        }

        private void CheckUpperAlertForReEnable()
        {
            _disableUpperAlert = !(_disableUpperAlert && DateTime.Now >= _upperAlertReEnableTime);
        }

        private void CheckLowerAlertForReEnable()
        {
            _disableLowerAlert = !(_disableLowerAlert && DateTime.Now >= _lowerAlertReEnableTime);
        }

        /// <summary>
        /// re-enables alert tone 
        /// </summary>
        private void ReEnableAlert()
        {
            _disableUpperAlert = false;
            _disableLowerAlert = false;
        }
        #endregion
    }
}
