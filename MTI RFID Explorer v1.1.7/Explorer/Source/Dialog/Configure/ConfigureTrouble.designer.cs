namespace RFID_Explorer
{
	partial class ConfigureTroubleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureTroubleControl));
            this.resetButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.errorCodeTextBox = new System.Windows.Forms.TextBox();
            this.infoPictureBox = new System.Windows.Forms.PictureBox();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.errorClearButton = new System.Windows.Forms.Button();
            this.lastInfoPictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lastErrorCodeTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lastInfoPictureBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(17, 22);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(99, 21);
            this.resetButton.TabIndex = 1;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Current Error Code";
            // 
            // errorCodeTextBox
            // 
            this.errorCodeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.errorCodeTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.errorCodeTextBox.Location = new System.Drawing.Point(115, 19);
            this.errorCodeTextBox.Name = "errorCodeTextBox";
            this.errorCodeTextBox.ReadOnly = true;
            this.errorCodeTextBox.Size = new System.Drawing.Size(145, 22);
            this.errorCodeTextBox.TabIndex = 3;
            this.errorCodeTextBox.TabStop = false;
            // 
            // infoPictureBox
            // 
            this.infoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("infoPictureBox.Image")));
            this.infoPictureBox.Location = new System.Drawing.Point(261, 22);
            this.infoPictureBox.Name = "infoPictureBox";
            this.infoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.infoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.infoPictureBox.TabIndex = 14;
            this.infoPictureBox.TabStop = false;
            this.infoPictureBox.Visible = false;
            this.infoPictureBox.MouseLeave += new System.EventHandler(this.infoPictureBox_MouseLeave);
            this.infoPictureBox.MouseHover += new System.EventHandler(this.infoPictureBox_MouseHover);
            // 
            // statusTextBox
            // 
            this.statusTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.statusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusTextBox.Location = new System.Drawing.Point(18, 258);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(525, 15);
            this.statusTextBox.TabIndex = 3;
            this.statusTextBox.TabStop = false;
            this.statusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusTextBox.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.errorClearButton);
            this.groupBox1.Controls.Add(this.lastInfoPictureBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lastErrorCodeTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.errorCodeTextBox);
            this.groupBox1.Controls.Add(this.infoPictureBox);
            this.groupBox1.Location = new System.Drawing.Point(44, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 78);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MAC Errors";
            // 
            // errorClearButton
            // 
            this.errorClearButton.Location = new System.Drawing.Point(295, 19);
            this.errorClearButton.Name = "errorClearButton";
            this.errorClearButton.Size = new System.Drawing.Size(75, 21);
            this.errorClearButton.TabIndex = 18;
            this.errorClearButton.Text = "Clear Error";
            this.errorClearButton.UseVisualStyleBackColor = true;
            this.errorClearButton.Click += new System.EventHandler(this.errorClearButton_Click);
            // 
            // lastInfoPictureBox
            // 
            this.lastInfoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("lastInfoPictureBox.Image")));
            this.lastInfoPictureBox.Location = new System.Drawing.Point(261, 45);
            this.lastInfoPictureBox.Name = "lastInfoPictureBox";
            this.lastInfoPictureBox.Size = new System.Drawing.Size(16, 16);
            this.lastInfoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.lastInfoPictureBox.TabIndex = 17;
            this.lastInfoPictureBox.TabStop = false;
            this.lastInfoPictureBox.Visible = false;
            this.lastInfoPictureBox.MouseLeave += new System.EventHandler(this.lastInfoPictureBox_MouseLeave);
            this.lastInfoPictureBox.MouseHover += new System.EventHandler(this.lastInfoPictureBox_MouseHover);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "Last Error Code";
            // 
            // lastErrorCodeTextBox
            // 
            this.lastErrorCodeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lastErrorCodeTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lastErrorCodeTextBox.Location = new System.Drawing.Point(115, 43);
            this.lastErrorCodeTextBox.Name = "lastErrorCodeTextBox";
            this.lastErrorCodeTextBox.ReadOnly = true;
            this.lastErrorCodeTextBox.Size = new System.Drawing.Size(145, 22);
            this.lastErrorCodeTextBox.TabIndex = 16;
            this.lastErrorCodeTextBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.resetButton);
            this.groupBox2.Location = new System.Drawing.Point(44, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(135, 62);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Reset Firmware";
            // 
            // ConfigureTroubleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusTextBox);
            this.Name = "ConfigureTroubleControl";
            this.Size = new System.Drawing.Size(550, 277);
            ((System.ComponentModel.ISupportInitialize)(this.infoPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lastInfoPictureBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox errorCodeTextBox;
		private System.Windows.Forms.PictureBox infoPictureBox;
		private System.Windows.Forms.TextBox statusTextBox;
		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox lastErrorCodeTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox lastInfoPictureBox;
        private System.Windows.Forms.Button errorClearButton;

	}
}
