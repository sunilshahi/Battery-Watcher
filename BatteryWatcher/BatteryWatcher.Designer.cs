namespace BatteryWatcher
{
    partial class BatteryWatcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LowerLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.UpperLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.UpperLevelTrackBarLabel = new System.Windows.Forms.Label();
            this.LowerLevelTrackerBarLabel = new System.Windows.Forms.Label();
            this.UpperLevelTextBox = new System.Windows.Forms.TextBox();
            this.LowerLevelTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.LowerLevelTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpperLevelTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // LowerLevelTrackBar
            // 
            this.LowerLevelTrackBar.Location = new System.Drawing.Point(18, 122);
            this.LowerLevelTrackBar.Name = "LowerLevelTrackBar";
            this.LowerLevelTrackBar.Size = new System.Drawing.Size(357, 45);
            this.LowerLevelTrackBar.TabIndex = 1;
            this.LowerLevelTrackBar.ValueChanged += new System.EventHandler(this.LowerLevelTrackBar_ValueChanged);
            // 
            // UpperLevelTrackBar
            // 
            this.UpperLevelTrackBar.Location = new System.Drawing.Point(15, 59);
            this.UpperLevelTrackBar.Name = "UpperLevelTrackBar";
            this.UpperLevelTrackBar.Size = new System.Drawing.Size(357, 45);
            this.UpperLevelTrackBar.TabIndex = 2;
            this.UpperLevelTrackBar.ValueChanged += new System.EventHandler(this.UpperLevelTrackBar_ValueChanged);
            // 
            // UpperLevelTrackBarLabel
            // 
            this.UpperLevelTrackBarLabel.AutoSize = true;
            this.UpperLevelTrackBarLabel.Location = new System.Drawing.Point(15, 43);
            this.UpperLevelTrackBarLabel.Name = "UpperLevelTrackBarLabel";
            this.UpperLevelTrackBarLabel.Size = new System.Drawing.Size(65, 13);
            this.UpperLevelTrackBarLabel.TabIndex = 3;
            this.UpperLevelTrackBarLabel.Text = "Upper Level";
            // 
            // LowerLevelTrackerBarLabel
            // 
            this.LowerLevelTrackerBarLabel.AutoSize = true;
            this.LowerLevelTrackerBarLabel.Location = new System.Drawing.Point(18, 106);
            this.LowerLevelTrackerBarLabel.Name = "LowerLevelTrackerBarLabel";
            this.LowerLevelTrackerBarLabel.Size = new System.Drawing.Size(65, 13);
            this.LowerLevelTrackerBarLabel.TabIndex = 4;
            this.LowerLevelTrackerBarLabel.Text = "Lower Level";
            // 
            // UpperLevelTextBox
            // 
            this.UpperLevelTextBox.Location = new System.Drawing.Point(378, 59);
            this.UpperLevelTextBox.Name = "UpperLevelTextBox";
            this.UpperLevelTextBox.Size = new System.Drawing.Size(40, 20);
            this.UpperLevelTextBox.TabIndex = 5;
            this.UpperLevelTextBox.TextChanged += new System.EventHandler(this.UpperLevelTextBox_TextChanged);
            // 
            // LowerLevelTextBox
            // 
            this.LowerLevelTextBox.Location = new System.Drawing.Point(378, 122);
            this.LowerLevelTextBox.Name = "LowerLevelTextBox";
            this.LowerLevelTextBox.Size = new System.Drawing.Size(40, 20);
            this.LowerLevelTextBox.TabIndex = 6;
            this.LowerLevelTextBox.TextChanged += new System.EventHandler(this.LowerLevelTextBox_TextChanged);
            // 
            // BatteryWatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 182);
            this.Controls.Add(this.LowerLevelTextBox);
            this.Controls.Add(this.UpperLevelTextBox);
            this.Controls.Add(this.LowerLevelTrackerBarLabel);
            this.Controls.Add(this.UpperLevelTrackBarLabel);
            this.Controls.Add(this.UpperLevelTrackBar);
            this.Controls.Add(this.LowerLevelTrackBar);
            this.Name = "BatteryWatcher";
            this.Text = "Battery Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.LowerLevelTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpperLevelTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar LowerLevelTrackBar;
        private System.Windows.Forms.TrackBar UpperLevelTrackBar;
        private System.Windows.Forms.Label UpperLevelTrackBarLabel;
        private System.Windows.Forms.Label LowerLevelTrackerBarLabel;
        private System.Windows.Forms.TextBox UpperLevelTextBox;
        private System.Windows.Forms.TextBox LowerLevelTextBox;
    }
}

