namespace RFID_Explorer
{
	partial class OptionsLoggingControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsLoggingControl));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.directoryButton = new System.Windows.Forms.Button();
            this.savePathTextBox = new System.Windows.Forms.TextBox();
            this.enableLoggingCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.noTempFileCheckBox = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.directoryButton);
            this.groupBox2.Controls.Add(this.savePathTextBox);
            this.groupBox2.Controls.Add(this.enableLoggingCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(67, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(416, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log file directory";
            // 
            // directoryButton
            // 
            this.directoryButton.AutoSize = true;
            this.directoryButton.Enabled = false;
            this.directoryButton.Location = new System.Drawing.Point(375, 62);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(26, 22);
            this.directoryButton.TabIndex = 2;
            this.directoryButton.Text = "...";
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // savePathTextBox
            // 
            this.savePathTextBox.Enabled = false;
            this.savePathTextBox.Location = new System.Drawing.Point(16, 65);
            this.savePathTextBox.Name = "savePathTextBox";
            this.savePathTextBox.Size = new System.Drawing.Size(353, 22);
            this.savePathTextBox.TabIndex = 2;
            // 
            // enableLoggingCheckBox
            // 
            this.enableLoggingCheckBox.AutoSize = true;
            this.enableLoggingCheckBox.Location = new System.Drawing.Point(16, 18);
            this.enableLoggingCheckBox.Name = "enableLoggingCheckBox";
            this.enableLoggingCheckBox.Size = new System.Drawing.Size(102, 16);
            this.enableLoggingCheckBox.TabIndex = 0;
            this.enableLoggingCheckBox.Text = "Enable Logging.";
            this.enableLoggingCheckBox.UseVisualStyleBackColor = true;
            this.enableLoggingCheckBox.CheckedChanged += new System.EventHandler(this.enableLoggingCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.noTempFileCheckBox);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(67, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 119);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // noTempFileCheckBox
            // 
            this.noTempFileCheckBox.AutoSize = true;
            this.noTempFileCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.noTempFileCheckBox.Location = new System.Drawing.Point(16, 15);
            this.noTempFileCheckBox.Name = "noTempFileCheckBox";
            this.noTempFileCheckBox.Size = new System.Drawing.Size(389, 17);
            this.noTempFileCheckBox.TabIndex = 0;
            this.noTempFileCheckBox.Text = "Do not save data to temporary file. (application restart required to take effect)" +
                "";
            this.noTempFileCheckBox.UseVisualStyleBackColor = true;
            this.noTempFileCheckBox.CheckedChanged += new System.EventHandler(this.noTempFileCheckBox_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(16, 41);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(367, 72);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // OptionsLoggingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "OptionsLoggingControl";
            this.Size = new System.Drawing.Size(550, 289);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button directoryButton;
		private System.Windows.Forms.TextBox savePathTextBox;
		private System.Windows.Forms.CheckBox enableLoggingCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox noTempFileCheckBox;
		private System.Windows.Forms.TextBox textBox1;
	}
}
