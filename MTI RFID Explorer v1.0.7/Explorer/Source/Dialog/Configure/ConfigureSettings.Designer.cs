namespace RFID_Explorer
{
	partial class ConfigureSettingsControl
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
            this.profileComboBox = new System.Windows.Forms.ComboBox();
            this.labelLinkProfile = new System.Windows.Forms.Label();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.labelAlgorithm = new System.Windows.Forms.Label();
            this.algorithmComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Update = new System.Windows.Forms.Button();
            this.rBtn_UART = new System.Windows.Forms.RadioButton();
            this.rBtn_USB = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RegionMatchLabel = new System.Windows.Forms.Label();
            this.btn_SetRegion = new System.Windows.Forms.Button();
            this.regionComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // profileComboBox
            // 
            this.profileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.profileComboBox.Enabled = false;
            this.profileComboBox.FormattingEnabled = true;
            this.profileComboBox.Location = new System.Drawing.Point(277, 194);
            this.profileComboBox.Name = "profileComboBox";
            this.profileComboBox.Size = new System.Drawing.Size(154, 20);
            this.profileComboBox.TabIndex = 7;
            // 
            // labelLinkProfile
            // 
            this.labelLinkProfile.AutoSize = true;
            this.labelLinkProfile.Location = new System.Drawing.Point(173, 197);
            this.labelLinkProfile.Name = "labelLinkProfile";
            this.labelLinkProfile.Size = new System.Drawing.Size(61, 12);
            this.labelLinkProfile.TabIndex = 5;
            this.labelLinkProfile.Text = "Link Profile";
            // 
            // statusTextBox
            // 
            this.statusTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.statusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusTextBox.Location = new System.Drawing.Point(22, 280);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(525, 15);
            this.statusTextBox.TabIndex = 20;
            this.statusTextBox.TabStop = false;
            this.statusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusTextBox.Visible = false;
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.AutoSize = true;
            this.labelAlgorithm.Location = new System.Drawing.Point(135, 232);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(102, 12);
            this.labelAlgorithm.TabIndex = 14;
            this.labelAlgorithm.Text = "Inventory Algorithm";
            // 
            // algorithmComboBox
            // 
            this.algorithmComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmComboBox.FormattingEnabled = true;
            this.algorithmComboBox.Location = new System.Drawing.Point(277, 229);
            this.algorithmComboBox.Name = "algorithmComboBox";
            this.algorithmComboBox.Size = new System.Drawing.Size(154, 20);
            this.algorithmComboBox.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Update);
            this.groupBox1.Controls.Add(this.rBtn_UART);
            this.groupBox1.Controls.Add(this.rBtn_USB);
            this.groupBox1.Location = new System.Drawing.Point(137, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 56);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communication Port";
            // 
            // btn_Update
            // 
            this.btn_Update.Location = new System.Drawing.Point(171, 21);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(75, 23);
            this.btn_Update.TabIndex = 4;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // rBtn_UART
            // 
            this.rBtn_UART.AutoSize = true;
            this.rBtn_UART.Location = new System.Drawing.Point(86, 24);
            this.rBtn_UART.Name = "rBtn_UART";
            this.rBtn_UART.Size = new System.Drawing.Size(54, 16);
            this.rBtn_UART.TabIndex = 1;
            this.rBtn_UART.TabStop = true;
            this.rBtn_UART.Text = "UART";
            this.rBtn_UART.UseVisualStyleBackColor = true;
            // 
            // rBtn_USB
            // 
            this.rBtn_USB.AutoSize = true;
            this.rBtn_USB.Location = new System.Drawing.Point(19, 24);
            this.rBtn_USB.Name = "rBtn_USB";
            this.rBtn_USB.Size = new System.Drawing.Size(45, 16);
            this.rBtn_USB.TabIndex = 0;
            this.rBtn_USB.TabStop = true;
            this.rBtn_USB.Text = "USB";
            this.rBtn_USB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RegionMatchLabel);
            this.groupBox2.Controls.Add(this.btn_SetRegion);
            this.groupBox2.Controls.Add(this.regionComboBox);
            this.groupBox2.Location = new System.Drawing.Point(137, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 62);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Region";
            // 
            // RegionMatchLabel
            // 
            this.RegionMatchLabel.AutoSize = true;
            this.RegionMatchLabel.ForeColor = System.Drawing.Color.Red;
            this.RegionMatchLabel.Location = new System.Drawing.Point(17, 44);
            this.RegionMatchLabel.Name = "RegionMatchLabel";
            this.RegionMatchLabel.Size = new System.Drawing.Size(81, 12);
            this.RegionMatchLabel.TabIndex = 23;
            this.RegionMatchLabel.Text = "Region Message";
            this.RegionMatchLabel.Visible = false;
            // 
            // btn_SetRegion
            // 
            this.btn_SetRegion.Location = new System.Drawing.Point(171, 19);
            this.btn_SetRegion.Name = "btn_SetRegion";
            this.btn_SetRegion.Size = new System.Drawing.Size(75, 23);
            this.btn_SetRegion.TabIndex = 5;
            this.btn_SetRegion.Text = "Set";
            this.btn_SetRegion.UseVisualStyleBackColor = true;
            this.btn_SetRegion.Click += new System.EventHandler(this.btn_SetRegion_Click);
            // 
            // regionComboBox
            // 
            this.regionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.regionComboBox.FormattingEnabled = true;
            this.regionComboBox.Location = new System.Drawing.Point(19, 19);
            this.regionComboBox.Name = "regionComboBox";
            this.regionComboBox.Size = new System.Drawing.Size(121, 20);
            this.regionComboBox.TabIndex = 4;
            // 
            // ConfigureSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.profileComboBox);
            this.Controls.Add(this.algorithmComboBox);
            this.Controls.Add(this.labelAlgorithm);
            this.Controls.Add(this.labelLinkProfile);
            this.Name = "ConfigureSettingsControl";
            this.Size = new System.Drawing.Size(550, 308);
            this.Load += new System.EventHandler(this.ConfigureSettingsControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.ComboBox profileComboBox;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Label labelLinkProfile;
		private System.Windows.Forms.Label labelAlgorithm;
        private System.Windows.Forms.ComboBox algorithmComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rBtn_UART;
        private System.Windows.Forms.RadioButton rBtn_USB;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_SetRegion;
        private System.Windows.Forms.ComboBox regionComboBox;
        private System.Windows.Forms.Label RegionMatchLabel;

	}
}
