namespace RFID_Explorer
{
	partial class ConfigureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureForm));
            this.configureToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.configureToolStrip = new System.Windows.Forms.ToolStrip();
            this.settingsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.antennaStripButton = new System.Windows.Forms.ToolStripButton();
            this.selectCriteriaStripButton = new System.Windows.Forms.ToolStripButton();
            this.algorithmStripButton = new System.Windows.Forms.ToolStripButton();
            this.postSingulationStripButton = new System.Windows.Forms.ToolStripButton();
            this.gpioStripButton = new System.Windows.Forms.ToolStripButton();
            this.RFBandsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.versionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.troubleshootToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.configureToolStripContainer.LeftToolStripPanel.SuspendLayout();
            this.configureToolStripContainer.SuspendLayout();
            this.configureToolStrip.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // configureToolStripContainer
            // 
            this.configureToolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // configureToolStripContainer.ContentPanel
            // 
            this.configureToolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.configureToolStripContainer.ContentPanel.Size = new System.Drawing.Size(580, 298);
            this.configureToolStripContainer.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // configureToolStripContainer.LeftToolStripPanel
            // 
            this.configureToolStripContainer.LeftToolStripPanel.Controls.Add(this.configureToolStrip);
            this.configureToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.configureToolStripContainer.Name = "configureToolStripContainer";
            this.configureToolStripContainer.RightToolStripPanelVisible = false;
            this.configureToolStripContainer.Size = new System.Drawing.Size(700, 323);
            this.configureToolStripContainer.TabIndex = 0;
            this.configureToolStripContainer.Text = "toolStripContainer1";
            // 
            // configureToolStrip
            // 
            this.configureToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.configureToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.configureToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripButton,
            this.antennaStripButton,
            this.selectCriteriaStripButton,
            this.algorithmStripButton,
            this.postSingulationStripButton,
            this.gpioStripButton,
            this.RFBandsToolStripButton,
            this.versionToolStripButton,
            this.troubleshootToolStripButton});
            this.configureToolStrip.Location = new System.Drawing.Point(0, 3);
            this.configureToolStrip.Name = "configureToolStrip";
            this.configureToolStrip.Size = new System.Drawing.Size(120, 247);
            this.configureToolStrip.TabIndex = 0;
            // 
            // settingsToolStripButton
            // 
            this.settingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.settingsToolStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripButton.Image")));
            this.settingsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsToolStripButton.Name = "settingsToolStripButton";
            this.settingsToolStripButton.Padding = new System.Windows.Forms.Padding(4);
            this.settingsToolStripButton.Size = new System.Drawing.Size(118, 26);
            this.settingsToolStripButton.Text = "Settings";
            this.settingsToolStripButton.Click += new System.EventHandler(this.generalToolStripButton_Click);
            // 
            // antennaStripButton
            // 
            this.antennaStripButton.CheckOnClick = true;
            this.antennaStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.antennaStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.antennaStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.antennaStripButton.Name = "antennaStripButton";
            this.antennaStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.antennaStripButton.Size = new System.Drawing.Size(118, 24);
            this.antennaStripButton.Text = "Antenna Ports";
            this.antennaStripButton.Click += new System.EventHandler(this.antennaStripButton_Click);
            // 
            // selectCriteriaStripButton
            // 
            this.selectCriteriaStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.selectCriteriaStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectCriteriaStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectCriteriaStripButton.Name = "selectCriteriaStripButton";
            this.selectCriteriaStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.selectCriteriaStripButton.Size = new System.Drawing.Size(118, 24);
            this.selectCriteriaStripButton.Text = "Select Criteria";
            this.selectCriteriaStripButton.Click += new System.EventHandler(this.selectCriteriaStripButton_Click);
            // 
            // algorithmStripButton
            // 
            this.algorithmStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.algorithmStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.algorithmStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.algorithmStripButton.Name = "algorithmStripButton";
            this.algorithmStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.algorithmStripButton.Size = new System.Drawing.Size(118, 24);
            this.algorithmStripButton.Text = "Algorithm";
            this.algorithmStripButton.Click += new System.EventHandler(this.algorithmStripButton_Click);
            // 
            // postSingulationStripButton
            // 
            this.postSingulationStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.postSingulationStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.postSingulationStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.postSingulationStripButton.Name = "postSingulationStripButton";
            this.postSingulationStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.postSingulationStripButton.Size = new System.Drawing.Size(118, 24);
            this.postSingulationStripButton.Text = "Post Singulation";
            this.postSingulationStripButton.Click += new System.EventHandler(this.postSingulationStripButton_Click);
            // 
            // gpioStripButton
            // 
            this.gpioStripButton.CheckOnClick = true;
            this.gpioStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.gpioStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpioStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gpioStripButton.Name = "gpioStripButton";
            this.gpioStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.gpioStripButton.Size = new System.Drawing.Size(118, 24);
            this.gpioStripButton.Text = "G P I O";
            this.gpioStripButton.Click += new System.EventHandler(this.gpioStripButton_Click);
            // 
            // RFBandsToolStripButton
            // 
            this.RFBandsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RFBandsToolStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RFBandsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("RFBandsToolStripButton.Image")));
            this.RFBandsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RFBandsToolStripButton.Name = "RFBandsToolStripButton";
            this.RFBandsToolStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.RFBandsToolStripButton.Size = new System.Drawing.Size(118, 24);
            this.RFBandsToolStripButton.Text = "RF Channels";
            this.RFBandsToolStripButton.Click += new System.EventHandler(this.RFBandsToolStripButton_Click);
            // 
            // versionToolStripButton
            // 
            this.versionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.versionToolStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.versionToolStripButton.Name = "versionToolStripButton";
            this.versionToolStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.versionToolStripButton.Size = new System.Drawing.Size(118, 24);
            this.versionToolStripButton.Text = "About Module";
            this.versionToolStripButton.ToolTipText = "About Module";
            this.versionToolStripButton.Click += new System.EventHandler(this.aboutReaderToolStripButton_Click);
            // 
            // troubleshootToolStripButton
            // 
            this.troubleshootToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.troubleshootToolStripButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.troubleshootToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.troubleshootToolStripButton.Name = "troubleshootToolStripButton";
            this.troubleshootToolStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.troubleshootToolStripButton.Size = new System.Drawing.Size(118, 24);
            this.troubleshootToolStripButton.Text = "Troubleshooting";
            this.troubleshootToolStripButton.Click += new System.EventHandler(this.troubleshootToolStripButton_Click);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 317);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(700, 42);
            this.bottomPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(485, 10);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(404, 10);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ConfigureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(700, 359);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.configureToolStripContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Module";
            this.configureToolStripContainer.LeftToolStripPanel.ResumeLayout(false);
            this.configureToolStripContainer.LeftToolStripPanel.PerformLayout();
            this.configureToolStripContainer.ResumeLayout(false);
            this.configureToolStripContainer.PerformLayout();
            this.configureToolStrip.ResumeLayout(false);
            this.configureToolStrip.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer configureToolStripContainer;
		private System.Windows.Forms.ToolStrip configureToolStrip;
        private System.Windows.Forms.ToolStripButton settingsToolStripButton;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.ToolStripButton gpioStripButton;
		private System.Windows.Forms.ToolStripButton antennaStripButton;
		private System.Windows.Forms.ToolStripButton versionToolStripButton;
		private System.Windows.Forms.ToolStripButton troubleshootToolStripButton;
		private System.Windows.Forms.ToolStripButton RFBandsToolStripButton;
        private System.Windows.Forms.ToolStripButton selectCriteriaStripButton;
		private System.Windows.Forms.ToolStripButton algorithmStripButton;
        private System.Windows.Forms.ToolStripButton postSingulationStripButton;
	}
}