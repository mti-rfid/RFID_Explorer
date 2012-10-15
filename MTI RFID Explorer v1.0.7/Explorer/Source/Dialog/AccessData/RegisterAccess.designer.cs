namespace RFID_Explorer
{
	partial class FORM_RegisterAccess
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
            this.COMBOBOX_RegisterAccessType = new System.Windows.Forms.ComboBox();
            this.BUTTON_RegisterAccessReadRegister = new System.Windows.Forms.Button();
            this.BUTTON_RegisterAccessWriteRegister = new System.Windows.Forms.Button();
            this.BUTTON_RegisterAccessBatch = new System.Windows.Forms.Button();
            this.BUTTON_RegisterAccessBatchHelp = new System.Windows.Forms.Button();
            this.CHECKBOX_RegisterAccesssBatchStopOnError = new System.Windows.Forms.CheckBox();
            this.TEXTBOX_RegisterAccessReadAddress = new System.Windows.Forms.TextBox();
            this.TEXTBOX_RegisterAccessWriteRegisterAddress = new System.Windows.Forms.TextBox();
            this.TEXTBOX_RegisterAccessWriteRegisterData = new System.Windows.Forms.TextBox();
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength = new System.Windows.Forms.NumericUpDown();
            this.NUMERICUPDOWN_RegisterAccessProfile = new System.Windows.Forms.NumericUpDown();
            this.TEXTBOX_RegisterAccessWriteRegisterBank = new System.Windows.Forms.TextBox();
            this.BUTTON_ClearStatus = new System.Windows.Forms.Button();
            this.BUTTON_SaveStatus = new System.Windows.Forms.Button();
            this.BUTTON_Close = new System.Windows.Forms.Button();
            this.CHECKBOX_AppendHistory = new System.Windows.Forms.CheckBox();
            this.LABEL_RegisterAccessReadRegisterAddress = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessWriteRegisterAddress = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessReadRegisterCount = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessWriteRegisterData = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessWriteRegisterBank = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessProfile = new System.Windows.Forms.Label();
            this.LABEL_RegisterAccessRegisterType = new System.Windows.Forms.Label();
            this.TEXTBOX_MainStatus = new System.Windows.Forms.RichTextBox();
            this.BUTTON_DumpMac = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_RegisterAccessReadRegisterLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_RegisterAccessProfile)).BeginInit();
            this.SuspendLayout();
            // 
            // COMBOBOX_RegisterAccessType
            // 
            this.COMBOBOX_RegisterAccessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_RegisterAccessType.FormattingEnabled = true;
            this.COMBOBOX_RegisterAccessType.Location = new System.Drawing.Point(93, 12);
            this.COMBOBOX_RegisterAccessType.Name = "COMBOBOX_RegisterAccessType";
            this.COMBOBOX_RegisterAccessType.Size = new System.Drawing.Size(96, 21);
            this.COMBOBOX_RegisterAccessType.TabIndex = 0;
            this.COMBOBOX_RegisterAccessType.SelectedIndexChanged += new System.EventHandler(this.COMBOBOX_RegisterAccess_SelectedIndexChanged);
            // 
            // BUTTON_RegisterAccessReadRegister
            // 
            this.BUTTON_RegisterAccessReadRegister.Location = new System.Drawing.Point(12, 62);
            this.BUTTON_RegisterAccessReadRegister.Name = "BUTTON_RegisterAccessReadRegister";
            this.BUTTON_RegisterAccessReadRegister.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_RegisterAccessReadRegister.TabIndex = 1;
            this.BUTTON_RegisterAccessReadRegister.Text = "Read";
            this.BUTTON_RegisterAccessReadRegister.UseVisualStyleBackColor = true;
            this.BUTTON_RegisterAccessReadRegister.Click += new System.EventHandler(this.BUTTON_ReadRegister_Click);
            // 
            // BUTTON_RegisterAccessWriteRegister
            // 
            this.BUTTON_RegisterAccessWriteRegister.Location = new System.Drawing.Point(12, 91);
            this.BUTTON_RegisterAccessWriteRegister.Name = "BUTTON_RegisterAccessWriteRegister";
            this.BUTTON_RegisterAccessWriteRegister.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_RegisterAccessWriteRegister.TabIndex = 2;
            this.BUTTON_RegisterAccessWriteRegister.Text = "Write";
            this.BUTTON_RegisterAccessWriteRegister.UseVisualStyleBackColor = true;
            this.BUTTON_RegisterAccessWriteRegister.Click += new System.EventHandler(this.BUTTON_WriteRegister_Click);
            // 
            // BUTTON_RegisterAccessBatch
            // 
            this.BUTTON_RegisterAccessBatch.Location = new System.Drawing.Point(12, 120);
            this.BUTTON_RegisterAccessBatch.Name = "BUTTON_RegisterAccessBatch";
            this.BUTTON_RegisterAccessBatch.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_RegisterAccessBatch.TabIndex = 3;
            this.BUTTON_RegisterAccessBatch.Text = "Batch";
            this.BUTTON_RegisterAccessBatch.UseVisualStyleBackColor = true;
            this.BUTTON_RegisterAccessBatch.Click += new System.EventHandler(this.BUTTON_RegisterAccessBatch_Click);
            // 
            // BUTTON_RegisterAccessBatchHelp
            // 
            this.BUTTON_RegisterAccessBatchHelp.Location = new System.Drawing.Point(87, 120);
            this.BUTTON_RegisterAccessBatchHelp.Name = "BUTTON_RegisterAccessBatchHelp";
            this.BUTTON_RegisterAccessBatchHelp.Size = new System.Drawing.Size(19, 23);
            this.BUTTON_RegisterAccessBatchHelp.TabIndex = 4;
            this.BUTTON_RegisterAccessBatchHelp.Text = "?";
            this.BUTTON_RegisterAccessBatchHelp.UseVisualStyleBackColor = true;
            this.BUTTON_RegisterAccessBatchHelp.Click += new System.EventHandler(this.BUTTON_RegisterAccessBatchHelp_Click);
            // 
            // CHECKBOX_RegisterAccesssBatchStopOnError
            // 
            this.CHECKBOX_RegisterAccesssBatchStopOnError.AutoSize = true;
            this.CHECKBOX_RegisterAccesssBatchStopOnError.Location = new System.Drawing.Point(112, 124);
            this.CHECKBOX_RegisterAccesssBatchStopOnError.Name = "CHECKBOX_RegisterAccesssBatchStopOnError";
            this.CHECKBOX_RegisterAccesssBatchStopOnError.Size = new System.Drawing.Size(121, 17);
            this.CHECKBOX_RegisterAccesssBatchStopOnError.TabIndex = 5;
            this.CHECKBOX_RegisterAccesssBatchStopOnError.Text = "Batch Stop On Error";
            this.CHECKBOX_RegisterAccesssBatchStopOnError.UseVisualStyleBackColor = true;
            // 
            // TEXTBOX_RegisterAccessReadAddress
            // 
            this.TEXTBOX_RegisterAccessReadAddress.Location = new System.Drawing.Point(150, 64);
            this.TEXTBOX_RegisterAccessReadAddress.MaxLength = 4;
            this.TEXTBOX_RegisterAccessReadAddress.Name = "TEXTBOX_RegisterAccessReadAddress";
            this.TEXTBOX_RegisterAccessReadAddress.Size = new System.Drawing.Size(39, 20);
            this.TEXTBOX_RegisterAccessReadAddress.TabIndex = 6;
            this.TEXTBOX_RegisterAccessReadAddress.Text = "0";
            this.TEXTBOX_RegisterAccessReadAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_RegisterAccessReadAddress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // TEXTBOX_RegisterAccessWriteRegisterAddress
            // 
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.Location = new System.Drawing.Point(150, 93);
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.MaxLength = 4;
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.Name = "TEXTBOX_RegisterAccessWriteRegisterAddress";
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.Size = new System.Drawing.Size(39, 20);
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.TabIndex = 7;
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.Text = "0";
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_RegisterAccessWriteRegisterAddress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // TEXTBOX_RegisterAccessWriteRegisterData
            // 
            this.TEXTBOX_RegisterAccessWriteRegisterData.Location = new System.Drawing.Point(246, 93);
            this.TEXTBOX_RegisterAccessWriteRegisterData.MaxLength = 8;
            this.TEXTBOX_RegisterAccessWriteRegisterData.Name = "TEXTBOX_RegisterAccessWriteRegisterData";
            this.TEXTBOX_RegisterAccessWriteRegisterData.Size = new System.Drawing.Size(72, 20);
            this.TEXTBOX_RegisterAccessWriteRegisterData.TabIndex = 8;
            this.TEXTBOX_RegisterAccessWriteRegisterData.Text = "0";
            this.TEXTBOX_RegisterAccessWriteRegisterData.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_RegisterAccessWriteRegisterData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // NUMERICUPDOWN_RegisterAccessReadRegisterLength
            // 
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.Location = new System.Drawing.Point(246, 64);
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.Name = "NUMERICUPDOWN_RegisterAccessReadRegisterLength";
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.Size = new System.Drawing.Size(49, 20);
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.TabIndex = 9;
            this.NUMERICUPDOWN_RegisterAccessReadRegisterLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NUMERICUPDOWN_RegisterAccessProfile
            // 
            this.NUMERICUPDOWN_RegisterAccessProfile.Location = new System.Drawing.Point(93, 36);
            this.NUMERICUPDOWN_RegisterAccessProfile.Name = "NUMERICUPDOWN_RegisterAccessProfile";
            this.NUMERICUPDOWN_RegisterAccessProfile.Size = new System.Drawing.Size(39, 20);
            this.NUMERICUPDOWN_RegisterAccessProfile.TabIndex = 10;
            // 
            // TEXTBOX_RegisterAccessWriteRegisterBank
            // 
            this.TEXTBOX_RegisterAccessWriteRegisterBank.Location = new System.Drawing.Point(377, 93);
            this.TEXTBOX_RegisterAccessWriteRegisterBank.MaxLength = 2;
            this.TEXTBOX_RegisterAccessWriteRegisterBank.Name = "TEXTBOX_RegisterAccessWriteRegisterBank";
            this.TEXTBOX_RegisterAccessWriteRegisterBank.Size = new System.Drawing.Size(22, 20);
            this.TEXTBOX_RegisterAccessWriteRegisterBank.TabIndex = 11;
            this.TEXTBOX_RegisterAccessWriteRegisterBank.Text = "0";
            this.TEXTBOX_RegisterAccessWriteRegisterBank.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_RegisterAccessWriteRegisterBank.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // BUTTON_ClearStatus
            // 
            this.BUTTON_ClearStatus.Location = new System.Drawing.Point(12, 151);
            this.BUTTON_ClearStatus.Name = "BUTTON_ClearStatus";
            this.BUTTON_ClearStatus.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_ClearStatus.TabIndex = 12;
            this.BUTTON_ClearStatus.Text = "Clear Status";
            this.BUTTON_ClearStatus.UseVisualStyleBackColor = true;
            this.BUTTON_ClearStatus.Click += new System.EventHandler(this.BUTTON_ClearStatus_Click);
            // 
            // BUTTON_SaveStatus
            // 
            this.BUTTON_SaveStatus.Location = new System.Drawing.Point(93, 151);
            this.BUTTON_SaveStatus.Name = "BUTTON_SaveStatus";
            this.BUTTON_SaveStatus.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_SaveStatus.TabIndex = 13;
            this.BUTTON_SaveStatus.Text = "Save Status";
            this.BUTTON_SaveStatus.UseVisualStyleBackColor = true;
            this.BUTTON_SaveStatus.Click += new System.EventHandler(this.BUTTON_SaveStatus_Click);
            // 
            // BUTTON_Close
            // 
            this.BUTTON_Close.Location = new System.Drawing.Point(459, 12);
            this.BUTTON_Close.Name = "BUTTON_Close";
            this.BUTTON_Close.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_Close.TabIndex = 14;
            this.BUTTON_Close.Text = "Close";
            this.BUTTON_Close.UseVisualStyleBackColor = true;
            this.BUTTON_Close.Click += new System.EventHandler(this.BUTTON_Close_Click);
            // 
            // CHECKBOX_AppendHistory
            // 
            this.CHECKBOX_AppendHistory.AutoSize = true;
            this.CHECKBOX_AppendHistory.Checked = true;
            this.CHECKBOX_AppendHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHECKBOX_AppendHistory.Location = new System.Drawing.Point(456, 157);
            this.CHECKBOX_AppendHistory.Name = "CHECKBOX_AppendHistory";
            this.CHECKBOX_AppendHistory.Size = new System.Drawing.Size(86, 17);
            this.CHECKBOX_AppendHistory.TabIndex = 16;
            this.CHECKBOX_AppendHistory.Text = "Keep History";
            this.CHECKBOX_AppendHistory.UseVisualStyleBackColor = true;
            // 
            // LABEL_RegisterAccessReadRegisterAddress
            // 
            this.LABEL_RegisterAccessReadRegisterAddress.AutoSize = true;
            this.LABEL_RegisterAccessReadRegisterAddress.Location = new System.Drawing.Point(90, 67);
            this.LABEL_RegisterAccessReadRegisterAddress.Name = "LABEL_RegisterAccessReadRegisterAddress";
            this.LABEL_RegisterAccessReadRegisterAddress.Size = new System.Drawing.Size(60, 13);
            this.LABEL_RegisterAccessReadRegisterAddress.TabIndex = 42;
            this.LABEL_RegisterAccessReadRegisterAddress.Text = "Address (h)";
            // 
            // LABEL_RegisterAccessWriteRegisterAddress
            // 
            this.LABEL_RegisterAccessWriteRegisterAddress.AutoSize = true;
            this.LABEL_RegisterAccessWriteRegisterAddress.Location = new System.Drawing.Point(88, 96);
            this.LABEL_RegisterAccessWriteRegisterAddress.Name = "LABEL_RegisterAccessWriteRegisterAddress";
            this.LABEL_RegisterAccessWriteRegisterAddress.Size = new System.Drawing.Size(60, 13);
            this.LABEL_RegisterAccessWriteRegisterAddress.TabIndex = 43;
            this.LABEL_RegisterAccessWriteRegisterAddress.Text = "Address (h)";
            // 
            // LABEL_RegisterAccessReadRegisterCount
            // 
            this.LABEL_RegisterAccessReadRegisterCount.AutoSize = true;
            this.LABEL_RegisterAccessReadRegisterCount.Location = new System.Drawing.Point(195, 67);
            this.LABEL_RegisterAccessReadRegisterCount.Name = "LABEL_RegisterAccessReadRegisterCount";
            this.LABEL_RegisterAccessReadRegisterCount.Size = new System.Drawing.Size(50, 13);
            this.LABEL_RegisterAccessReadRegisterCount.TabIndex = 44;
            this.LABEL_RegisterAccessReadRegisterCount.Text = "Count (d)";
            // 
            // LABEL_RegisterAccessWriteRegisterData
            // 
            this.LABEL_RegisterAccessWriteRegisterData.AutoSize = true;
            this.LABEL_RegisterAccessWriteRegisterData.Location = new System.Drawing.Point(195, 96);
            this.LABEL_RegisterAccessWriteRegisterData.Name = "LABEL_RegisterAccessWriteRegisterData";
            this.LABEL_RegisterAccessWriteRegisterData.Size = new System.Drawing.Size(45, 13);
            this.LABEL_RegisterAccessWriteRegisterData.TabIndex = 46;
            this.LABEL_RegisterAccessWriteRegisterData.Text = "Data (h)";
            // 
            // LABEL_RegisterAccessWriteRegisterBank
            // 
            this.LABEL_RegisterAccessWriteRegisterBank.AutoSize = true;
            this.LABEL_RegisterAccessWriteRegisterBank.Location = new System.Drawing.Point(324, 96);
            this.LABEL_RegisterAccessWriteRegisterBank.Name = "LABEL_RegisterAccessWriteRegisterBank";
            this.LABEL_RegisterAccessWriteRegisterBank.Size = new System.Drawing.Size(47, 13);
            this.LABEL_RegisterAccessWriteRegisterBank.TabIndex = 97;
            this.LABEL_RegisterAccessWriteRegisterBank.Text = "Bank (h)";
            // 
            // LABEL_RegisterAccessProfile
            // 
            this.LABEL_RegisterAccessProfile.AutoSize = true;
            this.LABEL_RegisterAccessProfile.Location = new System.Drawing.Point(12, 38);
            this.LABEL_RegisterAccessProfile.Name = "LABEL_RegisterAccessProfile";
            this.LABEL_RegisterAccessProfile.Size = new System.Drawing.Size(63, 13);
            this.LABEL_RegisterAccessProfile.TabIndex = 98;
            this.LABEL_RegisterAccessProfile.Text = "Profile Id (d)";
            // 
            // LABEL_RegisterAccessRegisterType
            // 
            this.LABEL_RegisterAccessRegisterType.AutoSize = true;
            this.LABEL_RegisterAccessRegisterType.Location = new System.Drawing.Point(12, 17);
            this.LABEL_RegisterAccessRegisterType.Name = "LABEL_RegisterAccessRegisterType";
            this.LABEL_RegisterAccessRegisterType.Size = new System.Drawing.Size(73, 13);
            this.LABEL_RegisterAccessRegisterType.TabIndex = 99;
            this.LABEL_RegisterAccessRegisterType.Text = "Register Type";
            // 
            // TEXTBOX_MainStatus
            // 
            this.TEXTBOX_MainStatus.Location = new System.Drawing.Point(12, 180);
            this.TEXTBOX_MainStatus.Name = "TEXTBOX_MainStatus";
            this.TEXTBOX_MainStatus.ReadOnly = true;
            this.TEXTBOX_MainStatus.Size = new System.Drawing.Size(530, 446);
            this.TEXTBOX_MainStatus.TabIndex = 100;
            this.TEXTBOX_MainStatus.Text = "";
            // 
            // BUTTON_DumpMac
            // 
            this.BUTTON_DumpMac.Location = new System.Drawing.Point(459, 62);
            this.BUTTON_DumpMac.Name = "BUTTON_DumpMac";
            this.BUTTON_DumpMac.Size = new System.Drawing.Size(75, 23);
            this.BUTTON_DumpMac.TabIndex = 101;
            this.BUTTON_DumpMac.Text = "Dump MAC";
            this.BUTTON_DumpMac.UseVisualStyleBackColor = true;
            this.BUTTON_DumpMac.Click += new System.EventHandler(this.BUTTON_DumpMac_Click);
            // 
            // FORM_RegisterAccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 638);
            this.Controls.Add(this.BUTTON_DumpMac);
            this.Controls.Add(this.TEXTBOX_MainStatus);
            this.Controls.Add(this.LABEL_RegisterAccessRegisterType);
            this.Controls.Add(this.LABEL_RegisterAccessProfile);
            this.Controls.Add(this.LABEL_RegisterAccessWriteRegisterBank);
            this.Controls.Add(this.LABEL_RegisterAccessWriteRegisterData);
            this.Controls.Add(this.LABEL_RegisterAccessReadRegisterCount);
            this.Controls.Add(this.LABEL_RegisterAccessWriteRegisterAddress);
            this.Controls.Add(this.LABEL_RegisterAccessReadRegisterAddress);
            this.Controls.Add(this.CHECKBOX_AppendHistory);
            this.Controls.Add(this.BUTTON_Close);
            this.Controls.Add(this.BUTTON_SaveStatus);
            this.Controls.Add(this.BUTTON_ClearStatus);
            this.Controls.Add(this.TEXTBOX_RegisterAccessWriteRegisterBank);
            this.Controls.Add(this.NUMERICUPDOWN_RegisterAccessProfile);
            this.Controls.Add(this.NUMERICUPDOWN_RegisterAccessReadRegisterLength);
            this.Controls.Add(this.TEXTBOX_RegisterAccessWriteRegisterData);
            this.Controls.Add(this.TEXTBOX_RegisterAccessWriteRegisterAddress);
            this.Controls.Add(this.TEXTBOX_RegisterAccessReadAddress);
            this.Controls.Add(this.CHECKBOX_RegisterAccesssBatchStopOnError);
            this.Controls.Add(this.BUTTON_RegisterAccessBatchHelp);
            this.Controls.Add(this.BUTTON_RegisterAccessBatch);
            this.Controls.Add(this.BUTTON_RegisterAccessWriteRegister);
            this.Controls.Add(this.BUTTON_RegisterAccessReadRegister);
            this.Controls.Add(this.COMBOBOX_RegisterAccessType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FORM_RegisterAccess";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Register Access";
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_RegisterAccessReadRegisterLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_RegisterAccessProfile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.ComboBox COMBOBOX_RegisterAccessType;
        private System.Windows.Forms.Button BUTTON_RegisterAccessReadRegister;
        private System.Windows.Forms.Button BUTTON_RegisterAccessWriteRegister;
        private System.Windows.Forms.Button BUTTON_RegisterAccessBatch;
        private System.Windows.Forms.Button BUTTON_RegisterAccessBatchHelp;
        private System.Windows.Forms.CheckBox CHECKBOX_RegisterAccesssBatchStopOnError;
        private System.Windows.Forms.TextBox TEXTBOX_RegisterAccessReadAddress;
        private System.Windows.Forms.TextBox TEXTBOX_RegisterAccessWriteRegisterAddress;
        private System.Windows.Forms.TextBox TEXTBOX_RegisterAccessWriteRegisterData;
        private System.Windows.Forms.NumericUpDown NUMERICUPDOWN_RegisterAccessReadRegisterLength;
        private System.Windows.Forms.NumericUpDown NUMERICUPDOWN_RegisterAccessProfile;
        private System.Windows.Forms.TextBox TEXTBOX_RegisterAccessWriteRegisterBank;
        private System.Windows.Forms.Button BUTTON_ClearStatus;
        private System.Windows.Forms.Button BUTTON_SaveStatus;
        private System.Windows.Forms.Button BUTTON_Close;
        private System.Windows.Forms.CheckBox CHECKBOX_AppendHistory;
        private System.Windows.Forms.Label LABEL_RegisterAccessReadRegisterAddress;
        private System.Windows.Forms.Label LABEL_RegisterAccessWriteRegisterAddress;
        private System.Windows.Forms.Label LABEL_RegisterAccessReadRegisterCount;
        private System.Windows.Forms.Label LABEL_RegisterAccessWriteRegisterData;
        private System.Windows.Forms.Label LABEL_RegisterAccessWriteRegisterBank;
        private System.Windows.Forms.Label LABEL_RegisterAccessProfile;
        private System.Windows.Forms.Label LABEL_RegisterAccessRegisterType;
        private System.Windows.Forms.RichTextBox TEXTBOX_MainStatus;
        private System.Windows.Forms.Button BUTTON_DumpMac;


    }
}