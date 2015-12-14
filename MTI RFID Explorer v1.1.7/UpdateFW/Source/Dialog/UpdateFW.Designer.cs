namespace UpdateFWTool
{
    partial class UpdateFW
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateFW));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBootLoader = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceInterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetComPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.textFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.labelPercent = new System.Windows.Forms.Label();
            this.btnRelink = new System.Windows.Forms.Button();
            this.textBoxDevice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.statusBar.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 141);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(445, 22);
            this.statusBar.TabIndex = 4;
            this.statusBar.Text = "statusBar";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version";
            // 
            // textBootLoader
            // 
            this.textBootLoader.Location = new System.Drawing.Point(57, 41);
            this.textBootLoader.Name = "textBootLoader";
            this.textBootLoader.ReadOnly = true;
            this.textBootLoader.Size = new System.Drawing.Size(99, 22);
            this.textBootLoader.TabIndex = 1;
            this.textBootLoader.Text = "Disconnected";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(358, 109);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 22);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(57, 109);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(266, 22);
            this.progressBar.TabIndex = 3;
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testModeToolStripMenuItem,
            this.deviceInterfaceToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // testModeToolStripMenuItem
            // 
            this.testModeToolStripMenuItem.CheckOnClick = true;
            this.testModeToolStripMenuItem.Name = "testModeToolStripMenuItem";
            this.testModeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.testModeToolStripMenuItem.Text = "Test Mode";
            // 
            // deviceInterfaceToolStripMenuItem
            // 
            this.deviceInterfaceToolStripMenuItem.Name = "deviceInterfaceToolStripMenuItem";
            this.deviceInterfaceToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.deviceInterfaceToolStripMenuItem.Text = "Device Interface";
            this.deviceInterfaceToolStripMenuItem.Click += new System.EventHandler(this.deviceInterfaceToolStripMenuItem_Click);
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetComPortToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolToolStripMenuItem.Text = "Tool";
            // 
            // SetComPortToolStripMenuItem
            // 
            this.SetComPortToolStripMenuItem.Name = "SetComPortToolStripMenuItem";
            this.SetComPortToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.SetComPortToolStripMenuItem.Text = "Set COM Port";
            this.SetComPortToolStripMenuItem.Click += new System.EventHandler(this.SetComPortToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.toolToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(445, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // textFilePath
            // 
            this.textFilePath.Location = new System.Drawing.Point(57, 74);
            this.textFilePath.Name = "textFilePath";
            this.textFilePath.ReadOnly = true;
            this.textFilePath.Size = new System.Drawing.Size(266, 22);
            this.textFilePath.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "Progress";
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(358, 74);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 22);
            this.btnBrowser.TabIndex = 8;
            this.btnBrowser.Text = "Browser";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // labelPercent
            // 
            this.labelPercent.AutoSize = true;
            this.labelPercent.Location = new System.Drawing.Point(325, 113);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(20, 12);
            this.labelPercent.TabIndex = 10;
            this.labelPercent.Text = "0%";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRelink
            // 
            this.btnRelink.Location = new System.Drawing.Point(358, 41);
            this.btnRelink.Name = "btnRelink";
            this.btnRelink.Size = new System.Drawing.Size(75, 22);
            this.btnRelink.TabIndex = 11;
            this.btnRelink.Text = "Relink";
            this.btnRelink.UseVisualStyleBackColor = true;
            this.btnRelink.Click += new System.EventHandler(this.btnRelink_Click);
            // 
            // textBoxDevice
            // 
            this.textBoxDevice.Location = new System.Drawing.Point(228, 41);
            this.textBoxDevice.Name = "textBoxDevice";
            this.textBoxDevice.ReadOnly = true;
            this.textBoxDevice.Size = new System.Drawing.Size(95, 22);
            this.textBoxDevice.TabIndex = 13;
            this.textBoxDevice.Text = "No device";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "Device";
            // 
            // UpdateFW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 163);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDevice);
            this.Controls.Add(this.btnRelink);
            this.Controls.Add(this.labelPercent);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textFilePath);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.textBootLoader);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "UpdateFW";
            this.Text = "Update Firmware Tool";
            this.Load += new System.EventHandler(this.UpdateFW_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateFW_FormClosing);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBootLoader;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deviceInterfaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetComPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox textFilePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.Button btnRelink;
        private System.Windows.Forms.TextBox textBoxDevice;
        private System.Windows.Forms.Label label4;
    }
}

