namespace RFID_Explorer
{
	partial class ProtocolControl
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
            this.topPanel = new System.Windows.Forms.Panel();
            this.liveUpdateLabel = new System.Windows.Forms.Label();
            this.captionsCheckBox = new System.Windows.Forms.CheckBox();
            this.freezeCheckBox = new System.Windows.Forms.CheckBox();
            this.topLabel = new System.Windows.Forms.Label();
            this.dataPanel = new System.Windows.Forms.Panel();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.tracerScrollBar = new System.Windows.Forms.VScrollBar();
            this.tracer = new RFID_Explorer.TracerControl();
            this.topPanel.SuspendLayout();
            this.dataPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.topPanel.Controls.Add(this.liveUpdateLabel);
            this.topPanel.Controls.Add(this.captionsCheckBox);
            this.topPanel.Controls.Add(this.freezeCheckBox);
            this.topPanel.Controls.Add(this.topLabel);
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(710, 30);
            this.topPanel.TabIndex = 0;
            // 
            // liveUpdateLabel
            // 
            this.liveUpdateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.liveUpdateLabel.AutoSize = true;
            this.liveUpdateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liveUpdateLabel.Location = new System.Drawing.Point(251, 7);
            this.liveUpdateLabel.Name = "liveUpdateLabel";
            this.liveUpdateLabel.Size = new System.Drawing.Size(209, 18);
            this.liveUpdateLabel.TabIndex = 5;
            this.liveUpdateLabel.Text = "*** Data Capture Active ***";
            this.liveUpdateLabel.Visible = false;
            // 
            // captionsCheckBox
            // 
            this.captionsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.captionsCheckBox.Checked = true;
            this.captionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captionsCheckBox.Location = new System.Drawing.Point(516, 4);
            this.captionsCheckBox.Name = "captionsCheckBox";
            this.captionsCheckBox.Size = new System.Drawing.Size(86, 21);
            this.captionsCheckBox.TabIndex = 3;
            this.captionsCheckBox.Text = "Captions";
            this.captionsCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.captionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // freezeCheckBox
            // 
            this.freezeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.freezeCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.freezeCheckBox.AutoSize = true;
            this.freezeCheckBox.Location = new System.Drawing.Point(621, 4);
            this.freezeCheckBox.Name = "freezeCheckBox";
            this.freezeCheckBox.Size = new System.Drawing.Size(83, 22);
            this.freezeCheckBox.TabIndex = 3;
            this.freezeCheckBox.Text = "Freeze Display";
            this.freezeCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.freezeCheckBox.UseVisualStyleBackColor = true;
            // 
            // topLabel
            // 
            this.topLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topLabel.Location = new System.Drawing.Point(0, 6);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(233, 18);
            this.topLabel.TabIndex = 2;
            this.topLabel.Text = "Protocol Trace";
            // 
            // dataPanel
            // 
            this.dataPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataPanel.Controls.Add(this.hScrollBar);
            this.dataPanel.Controls.Add(this.tracerScrollBar);
            this.dataPanel.Controls.Add(this.tracer);
            this.dataPanel.Location = new System.Drawing.Point(0, 30);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(713, 404);
            this.dataPanel.TabIndex = 1;
			
			//Add by Chingsheng for display extention data, 2015-01-14
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 385);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(694, 17);
            this.hScrollBar.TabIndex = 1;
			//Add by Chingsheng for display extention data, 2015-01-14
			
            // 
            // tracerScrollBar
            // 
            this.tracerScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.tracerScrollBar.Location = new System.Drawing.Point(694, 0);
            this.tracerScrollBar.Name = "tracerScrollBar";
            this.tracerScrollBar.Size = new System.Drawing.Size(17, 402);
            this.tracerScrollBar.TabIndex = 1;
            // 
            // tracer
            // 
            this.tracer.ActiveLabel = null;
            this.tracer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tracer.BackColor = System.Drawing.SystemColors.Control;
            this.tracer.CaptionsCheckbox = null;
            this.tracer.FreezeCheckbox = null;
            this.tracer.HScrollBar = null;
            this.tracer.Location = new System.Drawing.Point(0, 0);
            this.tracer.Margin = new System.Windows.Forms.Padding(0);
            this.tracer.Name = "tracer";
            this.tracer.ScrollBar = null;
            this.tracer.ShowCaption = true;
            this.tracer.Size = new System.Drawing.Size(693, 385);
            this.tracer.TabIndex = 0;
            // 
            // ProtocolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "ProtocolControl";
            this.Size = new System.Drawing.Size(713, 439);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.dataPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.Label topLabel;
		private System.Windows.Forms.CheckBox freezeCheckBox;
		private System.Windows.Forms.Panel dataPanel;
		private TracerControl tracer;
		private System.Windows.Forms.VScrollBar tracerScrollBar;
		private System.Windows.Forms.CheckBox captionsCheckBox;
		private System.Windows.Forms.Label liveUpdateLabel;
        private System.Windows.Forms.HScrollBar hScrollBar; //Add by Chingsheng for display extention data, 2015-01-14




	}
}
