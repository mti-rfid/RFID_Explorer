namespace RFID_Explorer
{
	partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.optionsToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.optionsToolStrip = new System.Windows.Forms.ToolStrip();
            this.generalToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.loggingToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.optionsToolStripContainer.LeftToolStripPanel.SuspendLayout();
            this.optionsToolStripContainer.SuspendLayout();
            this.optionsToolStrip.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsToolStripContainer
            // 
            this.optionsToolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // optionsToolStripContainer.ContentPanel
            // 
            this.optionsToolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.optionsToolStripContainer.ContentPanel.Size = new System.Drawing.Size(577, 275);
            this.optionsToolStripContainer.Dock = System.Windows.Forms.DockStyle.Top;
            // 
            // optionsToolStripContainer.LeftToolStripPanel
            // 
            this.optionsToolStripContainer.LeftToolStripPanel.Controls.Add(this.optionsToolStrip);
            this.optionsToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.optionsToolStripContainer.Name = "optionsToolStripContainer";
            this.optionsToolStripContainer.RightToolStripPanelVisible = false;
            this.optionsToolStripContainer.Size = new System.Drawing.Size(700, 300);
            this.optionsToolStripContainer.TabIndex = 0;
            this.optionsToolStripContainer.Text = "toolStripContainer1";
            // 
            // optionsToolStrip
            // 
            this.optionsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.optionsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.optionsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripButton,
            this.loggingToolStripButton});
            this.optionsToolStrip.Location = new System.Drawing.Point(0, 3);
            this.optionsToolStrip.Name = "optionsToolStrip";
            this.optionsToolStrip.Size = new System.Drawing.Size(123, 81);
            this.optionsToolStrip.TabIndex = 0;
            // 
            // generalToolStripButton
            // 
            this.generalToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.generalToolStripButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.generalToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("generalToolStripButton.Image")));
            this.generalToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.generalToolStripButton.Name = "generalToolStripButton";
            this.generalToolStripButton.Padding = new System.Windows.Forms.Padding(4);
            this.generalToolStripButton.Size = new System.Drawing.Size(121, 28);
            this.generalToolStripButton.Text = "General Options";
            this.generalToolStripButton.Click += new System.EventHandler(this.generalToolStripButton_Click);
            // 
            // loggingToolStripButton
            // 
            this.loggingToolStripButton.CheckOnClick = true;
            this.loggingToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loggingToolStripButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loggingToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loggingToolStripButton.Name = "loggingToolStripButton";
            this.loggingToolStripButton.Padding = new System.Windows.Forms.Padding(3);
            this.loggingToolStripButton.Size = new System.Drawing.Size(121, 26);
            this.loggingToolStripButton.Text = "Data Logging";
            this.loggingToolStripButton.Click += new System.EventHandler(this.loggingToolStripButton_Click);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 302);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(700, 47);
            this.bottomPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(485, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(404, 14);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(700, 349);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.optionsToolStripContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.optionsToolStripContainer.LeftToolStripPanel.ResumeLayout(false);
            this.optionsToolStripContainer.LeftToolStripPanel.PerformLayout();
            this.optionsToolStripContainer.ResumeLayout(false);
            this.optionsToolStripContainer.PerformLayout();
            this.optionsToolStrip.ResumeLayout(false);
            this.optionsToolStrip.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer optionsToolStripContainer;
		private System.Windows.Forms.ToolStrip optionsToolStrip;
		private System.Windows.Forms.ToolStripButton generalToolStripButton;
		private System.Windows.Forms.ToolStripButton loggingToolStripButton;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
	}
}