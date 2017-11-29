namespace RFID_Explorer
{
	partial class ConfigureAntenna
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
            this.view = new System.Windows.Forms.DataGridView();
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.rwLabel = new System.Windows.Forms.Label();
            this.editGlobalSenseThreshold = new System.Windows.Forms.Button();
            this.globalSenseThreshold = new System.Windows.Forms.TextBox();
            this.label_GlobalThreshold = new System.Windows.Forms.Label();
            this.Btn_RestoreDefault = new System.Windows.Forms.Button();
            this.label_PowerThreshold = new System.Windows.Forms.Label();
            this.numericUpDownRfPowerThreshold = new System.Windows.Forms.NumericUpDown();
            this.Btn_SetRfPowerThreshold = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRfPowerThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // view
            // 
            this.view.AllowUserToAddRows = false;
            this.view.AllowUserToDeleteRows = false;
            this.view.BackgroundColor = System.Drawing.Color.White;
            this.view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.view.Location = new System.Drawing.Point(11, 23);
            this.view.Name = "view";
            this.view.RowTemplate.Height = 24;
            this.view.Size = new System.Drawing.Size(528, 195);
            this.view.TabIndex = 1;
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(434, 260);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(105, 20);
            this.importButton.TabIndex = 6;
            this.importButton.Text = "Import from Excel";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(434, 229);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(105, 20);
            this.exportButton.TabIndex = 5;
            this.exportButton.Text = "Export to Excel";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // rwLabel
            // 
            this.rwLabel.AutoSize = true;
            this.rwLabel.ForeColor = System.Drawing.Color.Blue;
            this.rwLabel.Location = new System.Drawing.Point(4, 4);
            this.rwLabel.Name = "rwLabel";
            this.rwLabel.Size = new System.Drawing.Size(113, 12);
            this.rwLabel.TabIndex = 0;
            this.rwLabel.Text = "Antenna Configuration";
            // 
            // editGlobalSenseThreshold
            // 
            this.editGlobalSenseThreshold.Location = new System.Drawing.Point(142, 244);
            this.editGlobalSenseThreshold.Name = "editGlobalSenseThreshold";
            this.editGlobalSenseThreshold.Size = new System.Drawing.Size(84, 22);
            this.editGlobalSenseThreshold.TabIndex = 7;
            this.editGlobalSenseThreshold.Text = "Edit Threshold";
            this.editGlobalSenseThreshold.UseVisualStyleBackColor = true;
            this.editGlobalSenseThreshold.Click += new System.EventHandler(this.editGlobalSenseThreshold_Click);
            // 
            // globalSenseThreshold
            // 
            this.globalSenseThreshold.Enabled = false;
            this.globalSenseThreshold.Location = new System.Drawing.Point(11, 244);
            this.globalSenseThreshold.Name = "globalSenseThreshold";
            this.globalSenseThreshold.Size = new System.Drawing.Size(125, 22);
            this.globalSenseThreshold.TabIndex = 8;
            this.globalSenseThreshold.Text = "UNKNOWN";
            // 
            // label_GlobalThreshold
            // 
            this.label_GlobalThreshold.AutoSize = true;
            this.label_GlobalThreshold.Location = new System.Drawing.Point(9, 229);
            this.label_GlobalThreshold.Name = "label_GlobalThreshold";
            this.label_GlobalThreshold.Size = new System.Drawing.Size(199, 12);
            this.label_GlobalThreshold.TabIndex = 9;
            this.label_GlobalThreshold.Text = "Global Antenna Sense Threshold ( ohms )";
            // 
            // Btn_RestoreDefault
            // 
            this.Btn_RestoreDefault.Location = new System.Drawing.Point(434, 294);
            this.Btn_RestoreDefault.Name = "Btn_RestoreDefault";
            this.Btn_RestoreDefault.Size = new System.Drawing.Size(105, 20);
            this.Btn_RestoreDefault.TabIndex = 10;
            this.Btn_RestoreDefault.Text = "Restore Default";
            this.Btn_RestoreDefault.UseVisualStyleBackColor = true;
            this.Btn_RestoreDefault.Click += new System.EventHandler(this.RestoreDefaultButton_Click);
            // 
            // label_PowerThreshold
            // 
            this.label_PowerThreshold.AutoSize = true;
            this.label_PowerThreshold.Location = new System.Drawing.Point(11, 276);
            this.label_PowerThreshold.Name = "label_PowerThreshold";
            this.label_PowerThreshold.Size = new System.Drawing.Size(190, 12);
            this.label_PowerThreshold.TabIndex = 12;
            this.label_PowerThreshold.Text = "RF Reverse Power Threshold (1/10 dB)";
            // 
            // numericUpDownRfPowerThreshold
            // 
            this.numericUpDownRfPowerThreshold.Location = new System.Drawing.Point(11, 291);
            this.numericUpDownRfPowerThreshold.Name = "numericUpDownRfPowerThreshold";
            this.numericUpDownRfPowerThreshold.Size = new System.Drawing.Size(125, 22);
            this.numericUpDownRfPowerThreshold.TabIndex = 13;
            // 
            // Btn_SetRfPowerThreshold
            // 
            this.Btn_SetRfPowerThreshold.Location = new System.Drawing.Point(142, 291);
            this.Btn_SetRfPowerThreshold.Name = "Btn_SetRfPowerThreshold";
            this.Btn_SetRfPowerThreshold.Size = new System.Drawing.Size(84, 23);
            this.Btn_SetRfPowerThreshold.TabIndex = 14;
            this.Btn_SetRfPowerThreshold.Text = "Set Threshold";
            this.Btn_SetRfPowerThreshold.UseVisualStyleBackColor = true;
            this.Btn_SetRfPowerThreshold.Click += new System.EventHandler(this.Btn_SetRfPowerThreshold_Click);
            // 
            // ConfigureAntenna
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Btn_SetRfPowerThreshold);
            this.Controls.Add(this.numericUpDownRfPowerThreshold);
            this.Controls.Add(this.label_PowerThreshold);
            this.Controls.Add(this.Btn_RestoreDefault);
            this.Controls.Add(this.label_GlobalThreshold);
            this.Controls.Add(this.globalSenseThreshold);
            this.Controls.Add(this.editGlobalSenseThreshold);
            this.Controls.Add(this.view);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.rwLabel);
            this.Name = "ConfigureAntenna";
            this.Size = new System.Drawing.Size(550, 359);
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRfPowerThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.DataGridView view;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label rwLabel;
        private System.Windows.Forms.Button editGlobalSenseThreshold;
        private System.Windows.Forms.TextBox globalSenseThreshold;
        private System.Windows.Forms.Label label_GlobalThreshold;
        private System.Windows.Forms.Button Btn_RestoreDefault;
        private System.Windows.Forms.Label label_PowerThreshold;
        private System.Windows.Forms.NumericUpDown numericUpDownRfPowerThreshold;
        private System.Windows.Forms.Button Btn_SetRfPowerThreshold;

	}
}
