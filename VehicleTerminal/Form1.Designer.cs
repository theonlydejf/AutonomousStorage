
namespace VehicleTerminal
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
            Team.HobbyRobot.TDN.Core.TDNRoot tdnRoot1 = new Team.HobbyRobot.TDN.Core.TDNRoot();
            Team.HobbyRobot.TDN.Core.TDNRoot tdnRoot2 = new Team.HobbyRobot.TDN.Core.TDNRoot();
            this.RecievedRootViewer = new VehicleTerminal.TDNRootViewer();
            this.vehicleConnectionGroup = new System.Windows.Forms.GroupBox();
            this.ConnectionStatusLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.IPLbl = new System.Windows.Forms.Label();
            this.IPTxt = new System.Windows.Forms.TextBox();
            this.LocalLoggerTxt = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RemoteLoggerTxt = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.CurrRootViewer = new VehicleTerminal.TDNRootViewer();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ClearRootBtn = new System.Windows.Forms.Button();
            this.DeleteValueBtn = new System.Windows.Forms.Button();
            this.PathTxt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ParserCombo = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TDNFactoryConsoleOutTxt = new System.Windows.Forms.RichTextBox();
            this.TDNFactoryConsoleInTxt = new System.Windows.Forms.TextBox();
            this.CreateValueBtn = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SendBtn = new System.Windows.Forms.Button();
            this.vehicleConnectionGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // RecievedRootViewer
            // 
            this.RecievedRootViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecievedRootViewer.Location = new System.Drawing.Point(3, 21);
            this.RecievedRootViewer.Name = "RecievedRootViewer";
            this.RecievedRootViewer.Root = tdnRoot1;
            this.RecievedRootViewer.Size = new System.Drawing.Size(225, 346);
            this.RecievedRootViewer.TabIndex = 0;
            // 
            // vehicleConnectionGroup
            // 
            this.vehicleConnectionGroup.Controls.Add(this.ConnectionStatusLbl);
            this.vehicleConnectionGroup.Controls.Add(this.label1);
            this.vehicleConnectionGroup.Controls.Add(this.ConnectBtn);
            this.vehicleConnectionGroup.Controls.Add(this.IPLbl);
            this.vehicleConnectionGroup.Controls.Add(this.IPTxt);
            this.vehicleConnectionGroup.Location = new System.Drawing.Point(12, 12);
            this.vehicleConnectionGroup.Name = "vehicleConnectionGroup";
            this.vehicleConnectionGroup.Size = new System.Drawing.Size(138, 97);
            this.vehicleConnectionGroup.TabIndex = 1;
            this.vehicleConnectionGroup.TabStop = false;
            this.vehicleConnectionGroup.Text = "Vehicle Connection";
            // 
            // ConnectionStatusLbl
            // 
            this.ConnectionStatusLbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ConnectionStatusLbl.AutoSize = true;
            this.ConnectionStatusLbl.ForeColor = System.Drawing.Color.Red;
            this.ConnectionStatusLbl.Location = new System.Drawing.Point(45, 74);
            this.ConnectionStatusLbl.Name = "ConnectionStatusLbl";
            this.ConnectionStatusLbl.Size = new System.Drawing.Size(86, 15);
            this.ConnectionStatusLbl.TabIndex = 4;
            this.ConnectionStatusLbl.Text = "Not connected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Status:";
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(6, 48);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(125, 23);
            this.ConnectBtn.TabIndex = 2;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // IPLbl
            // 
            this.IPLbl.AutoSize = true;
            this.IPLbl.Location = new System.Drawing.Point(6, 22);
            this.IPLbl.Name = "IPLbl";
            this.IPLbl.Size = new System.Drawing.Size(20, 15);
            this.IPLbl.TabIndex = 1;
            this.IPLbl.Text = "IP:";
            // 
            // IPTxt
            // 
            this.IPTxt.Location = new System.Drawing.Point(32, 19);
            this.IPTxt.Name = "IPTxt";
            this.IPTxt.Size = new System.Drawing.Size(99, 23);
            this.IPTxt.TabIndex = 0;
            this.IPTxt.Text = "192.168.1.109";
            // 
            // LocalLoggerTxt
            // 
            this.LocalLoggerTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalLoggerTxt.BackColor = System.Drawing.Color.Black;
            this.LocalLoggerTxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LocalLoggerTxt.ForeColor = System.Drawing.Color.White;
            this.LocalLoggerTxt.Location = new System.Drawing.Point(3, 23);
            this.LocalLoggerTxt.Name = "LocalLoggerTxt";
            this.LocalLoggerTxt.ReadOnly = true;
            this.LocalLoggerTxt.Size = new System.Drawing.Size(383, 202);
            this.LocalLoggerTxt.TabIndex = 2;
            this.LocalLoggerTxt.Text = "";
            this.LocalLoggerTxt.TextChanged += new System.EventHandler(this.AutoScrollRichTextbox_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Local logger:";
            // 
            // RemoteLoggerTxt
            // 
            this.RemoteLoggerTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoteLoggerTxt.BackColor = System.Drawing.Color.Black;
            this.RemoteLoggerTxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RemoteLoggerTxt.ForeColor = System.Drawing.Color.White;
            this.RemoteLoggerTxt.Location = new System.Drawing.Point(3, 23);
            this.RemoteLoggerTxt.Name = "RemoteLoggerTxt";
            this.RemoteLoggerTxt.ReadOnly = true;
            this.RemoteLoggerTxt.Size = new System.Drawing.Size(378, 202);
            this.RemoteLoggerTxt.TabIndex = 4;
            this.RemoteLoggerTxt.Text = "";
            this.RemoteLoggerTxt.TextChanged += new System.EventHandler(this.AutoScrollRichTextbox_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Remote logger:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer1.Location = new System.Drawing.Point(12, 388);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LocalLoggerTxt);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.RemoteLoggerTxt);
            this.splitContainer1.Size = new System.Drawing.Size(776, 228);
            this.splitContainer1.SplitterDistance = 388;
            this.splitContainer1.TabIndex = 6;
            // 
            // CurrRootViewer
            // 
            this.CurrRootViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrRootViewer.Location = new System.Drawing.Point(3, 21);
            this.CurrRootViewer.Name = "CurrRootViewer";
            this.CurrRootViewer.Root = tdnRoot2;
            this.CurrRootViewer.Size = new System.Drawing.Size(221, 346);
            this.CurrRootViewer.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Current root:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Last recieved root:";
            // 
            // ClearRootBtn
            // 
            this.ClearRootBtn.Location = new System.Drawing.Point(156, 62);
            this.ClearRootBtn.Name = "ClearRootBtn";
            this.ClearRootBtn.Size = new System.Drawing.Size(83, 23);
            this.ClearRootBtn.TabIndex = 7;
            this.ClearRootBtn.Text = "Clear root";
            this.ClearRootBtn.UseVisualStyleBackColor = true;
            this.ClearRootBtn.Click += new System.EventHandler(this.ClearRootBtn_Click);
            // 
            // DeleteValueBtn
            // 
            this.DeleteValueBtn.Location = new System.Drawing.Point(245, 62);
            this.DeleteValueBtn.Name = "DeleteValueBtn";
            this.DeleteValueBtn.Size = new System.Drawing.Size(75, 23);
            this.DeleteValueBtn.TabIndex = 7;
            this.DeleteValueBtn.Text = "Delete";
            this.DeleteValueBtn.UseVisualStyleBackColor = true;
            this.DeleteValueBtn.Click += new System.EventHandler(this.DeleteValueBtn_Click);
            // 
            // PathTxt
            // 
            this.PathTxt.Location = new System.Drawing.Point(156, 33);
            this.PathTxt.Name = "PathTxt";
            this.PathTxt.Size = new System.Drawing.Size(164, 23);
            this.PathTxt.TabIndex = 8;
            this.PathTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PathTxt_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(156, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "Path:";
            // 
            // ParserCombo
            // 
            this.ParserCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParserCombo.FormattingEnabled = true;
            this.ParserCombo.Location = new System.Drawing.Point(196, 91);
            this.ParserCombo.Name = "ParserCombo";
            this.ParserCombo.Size = new System.Drawing.Size(124, 23);
            this.ParserCombo.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(156, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "Type:";
            // 
            // TDNFactoryConsoleOutTxt
            // 
            this.TDNFactoryConsoleOutTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TDNFactoryConsoleOutTxt.BackColor = System.Drawing.Color.Black;
            this.TDNFactoryConsoleOutTxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TDNFactoryConsoleOutTxt.ForeColor = System.Drawing.Color.White;
            this.TDNFactoryConsoleOutTxt.Location = new System.Drawing.Point(12, 120);
            this.TDNFactoryConsoleOutTxt.Name = "TDNFactoryConsoleOutTxt";
            this.TDNFactoryConsoleOutTxt.ReadOnly = true;
            this.TDNFactoryConsoleOutTxt.Size = new System.Drawing.Size(308, 202);
            this.TDNFactoryConsoleOutTxt.TabIndex = 2;
            this.TDNFactoryConsoleOutTxt.Text = "";
            this.TDNFactoryConsoleOutTxt.TextChanged += new System.EventHandler(this.AutoScrollRichTextbox_TextChanged);
            // 
            // TDNFactoryConsoleInTxt
            // 
            this.TDNFactoryConsoleInTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TDNFactoryConsoleInTxt.BackColor = System.Drawing.Color.Black;
            this.TDNFactoryConsoleInTxt.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TDNFactoryConsoleInTxt.ForeColor = System.Drawing.Color.White;
            this.TDNFactoryConsoleInTxt.Location = new System.Drawing.Point(12, 328);
            this.TDNFactoryConsoleInTxt.Name = "TDNFactoryConsoleInTxt";
            this.TDNFactoryConsoleInTxt.Size = new System.Drawing.Size(308, 22);
            this.TDNFactoryConsoleInTxt.TabIndex = 12;
            // 
            // CreateValueBtn
            // 
            this.CreateValueBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CreateValueBtn.Location = new System.Drawing.Point(220, 356);
            this.CreateValueBtn.Name = "CreateValueBtn";
            this.CreateValueBtn.Size = new System.Drawing.Size(100, 23);
            this.CreateValueBtn.TabIndex = 13;
            this.CreateValueBtn.Text = "Create Value";
            this.CreateValueBtn.UseVisualStyleBackColor = true;
            this.CreateValueBtn.Click += new System.EventHandler(this.CreateValueBtn_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer2.Location = new System.Drawing.Point(326, 12);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.CurrRootViewer);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.RecievedRootViewer);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Size = new System.Drawing.Size(462, 370);
            this.splitContainer2.SplitterDistance = 227;
            this.splitContainer2.TabIndex = 14;
            // 
            // SendBtn
            // 
            this.SendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SendBtn.Location = new System.Drawing.Point(12, 356);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.Size = new System.Drawing.Size(110, 23);
            this.SendBtn.TabIndex = 16;
            this.SendBtn.Text = "Send to vehicle";
            this.SendBtn.UseVisualStyleBackColor = true;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 628);
            this.Controls.Add(this.SendBtn);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.CreateValueBtn);
            this.Controls.Add(this.TDNFactoryConsoleInTxt);
            this.Controls.Add(this.TDNFactoryConsoleOutTxt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ParserCombo);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PathTxt);
            this.Controls.Add(this.DeleteValueBtn);
            this.Controls.Add(this.ClearRootBtn);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.vehicleConnectionGroup);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.vehicleConnectionGroup.ResumeLayout(false);
            this.vehicleConnectionGroup.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TDNRootViewer RecievedRootViewer;
        private System.Windows.Forms.GroupBox vehicleConnectionGroup;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Label IPLbl;
        private System.Windows.Forms.TextBox IPTxt;
        private System.Windows.Forms.Label ConnectionStatusLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox LocalLoggerTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox RemoteLoggerTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TDNRootViewer CurrRootViewer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ClearRootBtn;
        private System.Windows.Forms.Button DeleteValueBtn;
        private System.Windows.Forms.TextBox PathTxt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ParserCombo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox TDNFactoryConsoleOutTxt;
        private System.Windows.Forms.TextBox TDNFactoryConsoleInTxt;
        private System.Windows.Forms.Button CreateValueBtn;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button SendBtn;
    }
}

