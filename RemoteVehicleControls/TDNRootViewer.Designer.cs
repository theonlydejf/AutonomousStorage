
namespace Team.HobbyRobot.ASCS.RemoteVehicleControls
{
    partial class TDNRootViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RootView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // RootView
            // 
            this.RootView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RootView.Location = new System.Drawing.Point(0, 0);
            this.RootView.Name = "RootView";
            this.RootView.Size = new System.Drawing.Size(150, 150);
            this.RootView.TabIndex = 0;
            // 
            // TDNRootViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RootView);
            this.Name = "TDNRootViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView RootView;
    }
}
