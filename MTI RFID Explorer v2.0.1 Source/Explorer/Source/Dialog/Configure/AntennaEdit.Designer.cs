namespace RFID_Explorer
{
	partial class AntennaEditForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.state = new System.Windows.Forms.ComboBox();
            this.antennaNumberLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.PhysicalPort = new System.Windows.Forms.NumericUpDown();
            this.dwellTime = new System.Windows.Forms.NumericUpDown();
            this.inventoryCycles = new System.Windows.Forms.NumericUpDown();
            this.powerLevel = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PhysicalPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dwellTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryCycles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(287, 224);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(186, 224);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 14;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // state
            // 
            this.state.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.state.FormattingEnabled = true;
            this.state.Location = new System.Drawing.Point(26, 45);
            this.state.Name = "state";
            this.state.Size = new System.Drawing.Size(121, 20);
            this.state.TabIndex = 1;
            // 
            // antennaNumberLabel
            // 
            this.antennaNumberLabel.AutoSize = true;
            this.antennaNumberLabel.Location = new System.Drawing.Point(26, 30);
            this.antennaNumberLabel.Name = "antennaNumberLabel";
            this.antennaNumberLabel.Size = new System.Drawing.Size(69, 12);
            this.antennaNumberLabel.TabIndex = 0;
            this.antennaNumberLabel.Text = "Antenna Port ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(377, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Inventory Cycles";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(205, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Physical Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(376, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Milliseconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(376, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Power ( 1/10 dBm )";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(376, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 12);
            this.label8.TabIndex = 8;
            this.label8.Text = "Maximum Dwell Time";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(377, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "Maximum";
            // 
            // PhysicalPort
            // 
            this.PhysicalPort.Location = new System.Drawing.Point(205, 45);
            this.PhysicalPort.Name = "PhysicalPort";
            this.PhysicalPort.Size = new System.Drawing.Size(120, 22);
            this.PhysicalPort.TabIndex = 16;
            this.PhysicalPort.ValueChanged += new System.EventHandler(this.PhysicalPort_ValueChanged);
            // 
            // dwellTime
            // 
            this.dwellTime.Location = new System.Drawing.Point(376, 46);
            this.dwellTime.Name = "dwellTime";
            this.dwellTime.Size = new System.Drawing.Size(120, 22);
            this.dwellTime.TabIndex = 18;
            // 
            // inventoryCycles
            // 
            this.inventoryCycles.Location = new System.Drawing.Point(377, 110);
            this.inventoryCycles.Name = "inventoryCycles";
            this.inventoryCycles.Size = new System.Drawing.Size(120, 22);
            this.inventoryCycles.TabIndex = 19;
            // 
            // powerLevel
            // 
            this.powerLevel.Location = new System.Drawing.Point(376, 171);
            this.powerLevel.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.powerLevel.Name = "powerLevel";
            this.powerLevel.Size = new System.Drawing.Size(120, 22);
            this.powerLevel.TabIndex = 20;
            // 
            // AntennaEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(548, 254);
            this.Controls.Add(this.powerLevel);
            this.Controls.Add(this.inventoryCycles);
            this.Controls.Add(this.dwellTime);
            this.Controls.Add(this.PhysicalPort);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.state);
            this.Controls.Add(this.antennaNumberLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AntennaEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Antenna Settings";
            ((System.ComponentModel.ISupportInitialize)(this.PhysicalPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dwellTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inventoryCycles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label antennaNumberLabel;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox state;
		private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown PhysicalPort;
        private System.Windows.Forms.NumericUpDown dwellTime;
        private System.Windows.Forms.NumericUpDown inventoryCycles;
        private System.Windows.Forms.NumericUpDown powerLevel;
		
	}
}