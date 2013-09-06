namespace RFID_Explorer
{
	partial class AboutReaderControl
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
            this.label5 = new System.Windows.Forms.Label();
            this.ModelTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.updatePackTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.OemTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BootLoaderTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.fwVersionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.serialNumberTextBox = new System.Windows.Forms.TextBox();
            this.productTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.manufactureTextBox = new System.Windows.Forms.TextBox();
            this.regionTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Model Name";
            // 
            // ModelTextBox
            // 
            this.ModelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModelTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ModelTextBox.Location = new System.Drawing.Point(92, 58);
            this.ModelTextBox.Name = "ModelTextBox";
            this.ModelTextBox.ReadOnly = true;
            this.ModelTextBox.Size = new System.Drawing.Size(190, 22);
            this.ModelTextBox.TabIndex = 1;
            this.ModelTextBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.updatePackTextBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.OemTextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.BootLoaderTextBox);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.fwVersionTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(26, 148);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 149);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Version Information";
            // 
            // updatePackTextBox
            // 
            this.updatePackTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updatePackTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.updatePackTextBox.Location = new System.Drawing.Point(384, 76);
            this.updatePackTextBox.Name = "updatePackTextBox";
            this.updatePackTextBox.ReadOnly = true;
            this.updatePackTextBox.Size = new System.Drawing.Size(97, 22);
            this.updatePackTextBox.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(263, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 39);
            this.label8.TabIndex = 14;
            this.label8.Text = "OEMcfg Update Pack Information Code";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // OemTextBox
            // 
            this.OemTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OemTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.OemTextBox.Location = new System.Drawing.Point(384, 38);
            this.OemTextBox.Name = "OemTextBox";
            this.OemTextBox.ReadOnly = true;
            this.OemTextBox.Size = new System.Drawing.Size(97, 22);
            this.OemTextBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(261, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 36);
            this.label2.TabIndex = 12;
            this.label2.Text = "OEMcfg Manufacture Version Number";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BootLoaderTextBox
            // 
            this.BootLoaderTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BootLoaderTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.BootLoaderTextBox.Location = new System.Drawing.Point(94, 76);
            this.BootLoaderTextBox.Name = "BootLoaderTextBox";
            this.BootLoaderTextBox.ReadOnly = true;
            this.BootLoaderTextBox.Size = new System.Drawing.Size(143, 22);
            this.BootLoaderTextBox.TabIndex = 11;
            this.BootLoaderTextBox.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "Bootloader";
            // 
            // fwVersionTextBox
            // 
            this.fwVersionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fwVersionTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.fwVersionTextBox.Location = new System.Drawing.Point(92, 38);
            this.fwVersionTextBox.Name = "fwVersionTextBox";
            this.fwVersionTextBox.ReadOnly = true;
            this.fwVersionTextBox.Size = new System.Drawing.Size(145, 22);
            this.fwVersionTextBox.TabIndex = 5;
            this.fwVersionTextBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Firmware";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.serialNumberTextBox);
            this.groupBox1.Controls.Add(this.productTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ModelTextBox);
            this.groupBox1.Controls.Add(this.manufactureTextBox);
            this.groupBox1.Controls.Add(this.regionTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(26, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "About Module";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "Manufacturer";
            // 
            // serialNumberTextBox
            // 
            this.serialNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serialNumberTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.serialNumberTextBox.Location = new System.Drawing.Point(384, 25);
            this.serialNumberTextBox.MaxLength = 79;
            this.serialNumberTextBox.Name = "serialNumberTextBox";
            this.serialNumberTextBox.ReadOnly = true;
            this.serialNumberTextBox.Size = new System.Drawing.Size(97, 22);
            this.serialNumberTextBox.TabIndex = 3;
            // 
            // productTextBox
            // 
            this.productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.productTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.productTextBox.Location = new System.Drawing.Point(92, 25);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.ReadOnly = true;
            this.productTextBox.Size = new System.Drawing.Size(190, 22);
            this.productTextBox.TabIndex = 9;
            this.productTextBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Product Name";
            // 
            // manufactureTextBox
            // 
            this.manufactureTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.manufactureTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.manufactureTextBox.Location = new System.Drawing.Point(94, 92);
            this.manufactureTextBox.Name = "manufactureTextBox";
            this.manufactureTextBox.ReadOnly = true;
            this.manufactureTextBox.Size = new System.Drawing.Size(387, 22);
            this.manufactureTextBox.TabIndex = 5;
            this.manufactureTextBox.TabStop = false;
            // 
            // regionTextBox
            // 
            this.regionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.regionTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.regionTextBox.Location = new System.Drawing.Point(384, 58);
            this.regionTextBox.Name = "regionTextBox";
            this.regionTextBox.ReadOnly = true;
            this.regionTextBox.Size = new System.Drawing.Size(97, 22);
            this.regionTextBox.TabIndex = 7;
            this.regionTextBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(301, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Serial Number";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(334, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "Region";
            // 
            // AboutReaderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "AboutReaderControl";
            this.Size = new System.Drawing.Size(550, 305);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox ModelTextBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox fwVersionTextBox;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox productTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox manufactureTextBox;
		private System.Windows.Forms.TextBox regionTextBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox serialNumberTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox BootLoaderTextBox;
        private System.Windows.Forms.TextBox updatePackTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox OemTextBox;
        private System.Windows.Forms.Label label2;

	}
}
