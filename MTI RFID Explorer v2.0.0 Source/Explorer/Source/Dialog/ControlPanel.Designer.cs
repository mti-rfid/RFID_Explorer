namespace RFID_Explorer
{
	partial class ControlPanelForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlPanelForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.inventoryButton = new System.Windows.Forms.ToolStripButton();
            this.inventoryOnceButton = new System.Windows.Forms.ToolStripButton();
            this.BUTTON_TagAccess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.BUTTON_ConfigureReader = new System.Windows.Forms.ToolStripButton();
            this.BUTTON_RegisterAccess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.pauseButton = new System.Windows.Forms.ToolStripButton();
            this.abortButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inventoryButton,
            this.inventoryOnceButton,
            this.BUTTON_TagAccess,
            this.toolStripSeparator7,
            this.BUTTON_ConfigureReader,
            this.BUTTON_RegisterAccess,
            this.toolStripSeparator3,
            this.stopButton,
            this.pauseButton,
            this.abortButton,
            this.toolStripSeparator2,
            this.clearButton});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(127, 187);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // inventoryButton
            // 
            this.inventoryButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.inventoryButton.Image = ((System.Drawing.Image)(resources.GetObject("inventoryButton.Image")));
            this.inventoryButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inventoryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inventoryButton.Name = "inventoryButton";
            this.inventoryButton.Size = new System.Drawing.Size(125, 20);
            this.inventoryButton.Text = "Run Inventory";
            this.inventoryButton.ToolTipText = "Continuouly run tag inventory until manually stoped.";
            this.inventoryButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.inventoryButton.Click += new System.EventHandler(this.inventoryButton_Click);
            // 
            // inventoryOnceButton
            // 
            this.inventoryOnceButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.inventoryOnceButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.inventoryOnceButton.Image = ((System.Drawing.Image)(resources.GetObject("inventoryOnceButton.Image")));
            this.inventoryOnceButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inventoryOnceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inventoryOnceButton.Name = "inventoryOnceButton";
            this.inventoryOnceButton.Size = new System.Drawing.Size(125, 20);
            this.inventoryOnceButton.Text = "Inventory Once";
            this.inventoryOnceButton.ToolTipText = "Run one full tag inventory for all enabled ports.";
            this.inventoryOnceButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.inventoryOnceButton.Click += new System.EventHandler(this.inventoryOnceButton_Click);
            // 
            // BUTTON_TagAccess
            // 
            this.BUTTON_TagAccess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.BUTTON_TagAccess.Image = ((System.Drawing.Image)(resources.GetObject("BUTTON_TagAccess.Image")));
            this.BUTTON_TagAccess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BUTTON_TagAccess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BUTTON_TagAccess.Name = "BUTTON_TagAccess";
            this.BUTTON_TagAccess.Size = new System.Drawing.Size(125, 20);
            this.BUTTON_TagAccess.Text = "Tag Access";
            this.BUTTON_TagAccess.ToolTipText = "Tag Access (Read, Write, BlockWrite, BlockErase, Lock, Kill, QT)";
            this.BUTTON_TagAccess.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.BUTTON_TagAccess.Click += new System.EventHandler(this.BUTTON_TagAccess_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(125, 6);
            // 
            // BUTTON_ConfigureReader
            // 
            this.BUTTON_ConfigureReader.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.BUTTON_ConfigureReader.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BUTTON_ConfigureReader.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BUTTON_ConfigureReader.Name = "BUTTON_ConfigureReader";
            this.BUTTON_ConfigureReader.Size = new System.Drawing.Size(125, 18);
            this.BUTTON_ConfigureReader.Text = "Configure Reader";
            this.BUTTON_ConfigureReader.ToolTipText = "Configure Reader ";
            this.BUTTON_ConfigureReader.Click += new System.EventHandler(this.BUTTON_ConfigureReader_Click);
            // 
            // BUTTON_RegisterAccess
            // 
            this.BUTTON_RegisterAccess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.BUTTON_RegisterAccess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BUTTON_RegisterAccess.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BUTTON_RegisterAccess.Name = "BUTTON_RegisterAccess";
            this.BUTTON_RegisterAccess.Size = new System.Drawing.Size(125, 18);
            this.BUTTON_RegisterAccess.Text = "Register Access";
            this.BUTTON_RegisterAccess.ToolTipText = "Register Access (MAC, Bypass, OEM, LinkProfile)";
            this.BUTTON_RegisterAccess.Click += new System.EventHandler(this.BUTTON_RegsiterAccess_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(125, 6);
            // 
            // stopButton
            // 
            this.stopButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stopButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(125, 20);
            this.stopButton.Text = "Stop Operation";
            this.stopButton.ToolTipText = "Cancel or abort the current operation.";
            this.stopButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.CheckOnClick = true;
            this.pauseButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.pauseButton.Image = ((System.Drawing.Image)(resources.GetObject("pauseButton.Image")));
            this.pauseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(125, 20);
            this.pauseButton.Text = "Pause";
            this.pauseButton.ToolTipText = "Request Pause Operation";
            this.pauseButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // abortButton
            // 
            this.abortButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.abortButton.Image = ((System.Drawing.Image)(resources.GetObject("abortButton.Image")));
            this.abortButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.abortButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.abortButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(58, 20);
            this.abortButton.Text = "Abort";
            this.abortButton.ToolTipText = "Cancel or abort the current operation.";
            this.abortButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.abortButton.Click += new System.EventHandler(this.abortToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(125, 6);
            // 
            // clearButton
            // 
            this.clearButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.clearButton.Image = ((System.Drawing.Image)(resources.GetObject("clearButton.Image")));
            this.clearButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(97, 20);
            this.clearButton.Text = "Clear Session";
            this.clearButton.ToolTipText = "Clear the current session.";
            this.clearButton.MouseEnter += new System.EventHandler(this.ControlPanelForm_MouseEnter);
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(127, 187);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(127, 212);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // ControlPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(127, 212);
            this.ControlBox = false;
            this.Controls.Add(this.toolStripContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ControlPanelForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Control Panel";
            this.Load += new System.EventHandler(this.ControlPanelForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton stopButton;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton pauseButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton inventoryButton;
		private System.Windows.Forms.ToolStripButton inventoryOnceButton;
        private System.Windows.Forms.ToolStripButton clearButton;
		private System.Windows.Forms.ToolStripButton BUTTON_TagAccess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton abortButton;
        private System.Windows.Forms.ToolStripButton BUTTON_RegisterAccess;
        private System.Windows.Forms.ToolStripButton BUTTON_ConfigureReader;
	}
}