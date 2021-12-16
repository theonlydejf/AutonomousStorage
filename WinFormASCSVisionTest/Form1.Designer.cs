
namespace WinFormASCSVisionTest
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startVideoBtn = new System.Windows.Forms.Button();
            this.videoDeviceCombo = new System.Windows.Forms.ComboBox();
            this.videoSettingsGroup = new System.Windows.Forms.GroupBox();
            this.videoDeviceSettingsList = new System.Windows.Forms.ListBox();
            this.refreshVideDevicesBtn = new System.Windows.Forms.Button();
            this.videoBox = new System.Windows.Forms.PictureBox();
            this.videoSettingsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // startVideoBtn
            // 
            this.startVideoBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startVideoBtn.Location = new System.Drawing.Point(109, 580);
            this.startVideoBtn.Name = "startVideoBtn";
            this.startVideoBtn.Size = new System.Drawing.Size(75, 23);
            this.startVideoBtn.TabIndex = 0;
            this.startVideoBtn.Text = "Start Video";
            this.startVideoBtn.UseVisualStyleBackColor = true;
            this.startVideoBtn.Click += new System.EventHandler(this.startVideoBtn_Click);
            // 
            // videoDeviceCombo
            // 
            this.videoDeviceCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoDeviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.videoDeviceCombo.FormattingEnabled = true;
            this.videoDeviceCombo.Location = new System.Drawing.Point(6, 22);
            this.videoDeviceCombo.Name = "videoDeviceCombo";
            this.videoDeviceCombo.Size = new System.Drawing.Size(148, 23);
            this.videoDeviceCombo.TabIndex = 1;
            this.videoDeviceCombo.SelectedIndexChanged += new System.EventHandler(this.videoDeviceCombo_SelectedIndexChanged);
            // 
            // videoSettingsGroup
            // 
            this.videoSettingsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.videoSettingsGroup.Controls.Add(this.videoDeviceSettingsList);
            this.videoSettingsGroup.Controls.Add(this.refreshVideDevicesBtn);
            this.videoSettingsGroup.Controls.Add(this.videoDeviceCombo);
            this.videoSettingsGroup.Controls.Add(this.startVideoBtn);
            this.videoSettingsGroup.Location = new System.Drawing.Point(12, 12);
            this.videoSettingsGroup.Name = "videoSettingsGroup";
            this.videoSettingsGroup.Size = new System.Drawing.Size(190, 609);
            this.videoSettingsGroup.TabIndex = 2;
            this.videoSettingsGroup.TabStop = false;
            this.videoSettingsGroup.Text = "Video device settings";
            // 
            // videoDeviceSettingsList
            // 
            this.videoDeviceSettingsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoDeviceSettingsList.FormattingEnabled = true;
            this.videoDeviceSettingsList.ItemHeight = 15;
            this.videoDeviceSettingsList.Location = new System.Drawing.Point(6, 51);
            this.videoDeviceSettingsList.Name = "videoDeviceSettingsList";
            this.videoDeviceSettingsList.Size = new System.Drawing.Size(177, 514);
            this.videoDeviceSettingsList.TabIndex = 4;
            this.videoDeviceSettingsList.DoubleClick += new System.EventHandler(this.startVideoBtn_Click);
            // 
            // refreshVideDevicesBtn
            // 
            this.refreshVideDevicesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshVideDevicesBtn.BackgroundImage = global::WinFormASCSVisionTest.Properties.Resources.refresh60;
            this.refreshVideDevicesBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.refreshVideDevicesBtn.Location = new System.Drawing.Point(160, 22);
            this.refreshVideDevicesBtn.Name = "refreshVideDevicesBtn";
            this.refreshVideDevicesBtn.Size = new System.Drawing.Size(23, 23);
            this.refreshVideDevicesBtn.TabIndex = 3;
            this.refreshVideDevicesBtn.UseVisualStyleBackColor = true;
            this.refreshVideDevicesBtn.Click += new System.EventHandler(this.refreshVideDevicesBtn_Click);
            // 
            // videoBox
            // 
            this.videoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoBox.Location = new System.Drawing.Point(208, 12);
            this.videoBox.Name = "videoBox";
            this.videoBox.Size = new System.Drawing.Size(580, 609);
            this.videoBox.TabIndex = 3;
            this.videoBox.TabStop = false;
            this.videoBox.Paint += new System.Windows.Forms.PaintEventHandler(this.videoBox_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 633);
            this.Controls.Add(this.videoBox);
            this.Controls.Add(this.videoSettingsGroup);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.videoSettingsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startVideoBtn;
        private System.Windows.Forms.ComboBox videoDeviceCombo;
        private System.Windows.Forms.GroupBox videoSettingsGroup;
        private System.Windows.Forms.Button refreshVideDevicesBtn;
        private System.Windows.Forms.ListBox videoDeviceSettingsList;
        private System.Windows.Forms.PictureBox videoBox;
    }
}

