namespace RFID_Explorer
{
	partial class FORM_TagAccess
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
            this.COMBOBOX_TagAccessMemoryBank = new System.Windows.Forms.ComboBox();
            this.LABEL_TagAccessMemoryBank = new System.Windows.Forms.Label();
            this.TEXTBOX_TagAccessOffset = new System.Windows.Forms.TextBox();
            this.LABEL_TagAccessOffset = new System.Windows.Forms.Label();
            this.LABEL_TagAccessCount = new System.Windows.Forms.Label();
            this.BUTTON_TagAccessCancel = new System.Windows.Forms.Button();
            this.BUTTON_TagAccessOk = new System.Windows.Forms.Button();
            this.TEXTBOX_TagAccessAccessPassword = new System.Windows.Forms.TextBox();
            this.LABEL_TagAccessAccessPassword = new System.Windows.Forms.Label();
            this.TEXTBOX_TagAccessValue1 = new System.Windows.Forms.TextBox();
            this.LABEL_TagAccessValue1 = new System.Windows.Forms.Label();
            this.COMBOBOX_TagAccess = new System.Windows.Forms.ComboBox();
            this.LABEL_AccessType = new System.Windows.Forms.Label();
            this.LABEL_TagAccessKillPassword = new System.Windows.Forms.Label();
            this.TEXTBOX_TagAccessKillPassword = new System.Windows.Forms.TextBox();
            this.GROUPBOX_TagAccessQTControl = new System.Windows.Forms.GroupBox();
            this.LABEL_QTMemory = new System.Windows.Forms.Label();
            this.LABEL_QTRange = new System.Windows.Forms.Label();
            this.LABEL_QTPersistence = new System.Windows.Forms.Label();
            this.LABEL_QTAccess = new System.Windows.Forms.Label();
            this.COMBOBOX_QTMemMap = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_QTShortRange = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_QTPersistence = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_QTCtrlType = new System.Windows.Forms.ComboBox();
            this.GROUPBOX_TagAccessPermissions = new System.Windows.Forms.GroupBox();
            this.COMBOBOX_UserBankPermissions = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_TIDBankPermissions = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_EPCBankPermissions = new System.Windows.Forms.ComboBox();
            this.COMBOBOX_AccessPasswordPermissions = new System.Windows.Forms.ComboBox();
            this.LABEL_PermissionsUserBank = new System.Windows.Forms.Label();
            this.LABEL_PermissionsTIDBank = new System.Windows.Forms.Label();
            this.LABEL_PermissionsEPCBank = new System.Windows.Forms.Label();
            this.LABEL_PermissionsAccessPassword = new System.Windows.Forms.Label();
            this.LABEL_PermissionsKillPassword = new System.Windows.Forms.Label();
            this.COMBOBOX_KillPasswordPermissions = new System.Windows.Forms.ComboBox();
            this.NUMERICUPDOWN_TagAccessCount = new System.Windows.Forms.NumericUpDown();
            this.LABEL_TagAccessValue2 = new System.Windows.Forms.Label();
            this.TEXTBOX_TagAccessValue2 = new System.Windows.Forms.TextBox();
            this.chkPerformSelectOps = new System.Windows.Forms.CheckBox();
            this.chkPerformPostMatch = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LABEL_TagAccessReadWords = new System.Windows.Forms.Label();
            this.LABEL_TagAccessTotalReadWords = new System.Windows.Forms.Label();
            this.COMBOBOX_TagAccessReadWords = new System.Windows.Forms.ComboBox();
            this.TEXTBOX_TagAccessTotalReadWords = new System.Windows.Forms.TextBox();
            this.GROUPBOX_TagAccessQTControl.SuspendLayout();
            this.GROUPBOX_TagAccessPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_TagAccessCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // COMBOBOX_TagAccessMemoryBank
            // 
            this.COMBOBOX_TagAccessMemoryBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_TagAccessMemoryBank.FormattingEnabled = true;
            this.COMBOBOX_TagAccessMemoryBank.Location = new System.Drawing.Point(106, 37);
            this.COMBOBOX_TagAccessMemoryBank.MaxDropDownItems = 4;
            this.COMBOBOX_TagAccessMemoryBank.Name = "COMBOBOX_TagAccessMemoryBank";
            this.COMBOBOX_TagAccessMemoryBank.Size = new System.Drawing.Size(121, 20);
            this.COMBOBOX_TagAccessMemoryBank.TabIndex = 1;
            // 
            // LABEL_TagAccessMemoryBank
            // 
            this.LABEL_TagAccessMemoryBank.AutoSize = true;
            this.LABEL_TagAccessMemoryBank.Location = new System.Drawing.Point(18, 45);
            this.LABEL_TagAccessMemoryBank.Name = "LABEL_TagAccessMemoryBank";
            this.LABEL_TagAccessMemoryBank.Size = new System.Drawing.Size(73, 12);
            this.LABEL_TagAccessMemoryBank.TabIndex = 0;
            this.LABEL_TagAccessMemoryBank.Text = "Memory Bank";
            // 
            // TEXTBOX_TagAccessOffset
            // 
            this.TEXTBOX_TagAccessOffset.Location = new System.Drawing.Point(119, 63);
            this.TEXTBOX_TagAccessOffset.MaxLength = 4;
            this.TEXTBOX_TagAccessOffset.Name = "TEXTBOX_TagAccessOffset";
            this.TEXTBOX_TagAccessOffset.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessOffset.TabIndex = 3;
            this.TEXTBOX_TagAccessOffset.Text = "0";
            this.TEXTBOX_TagAccessOffset.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessOffset.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // LABEL_TagAccessOffset
            // 
            this.LABEL_TagAccessOffset.AutoSize = true;
            this.LABEL_TagAccessOffset.Location = new System.Drawing.Point(18, 73);
            this.LABEL_TagAccessOffset.Name = "LABEL_TagAccessOffset";
            this.LABEL_TagAccessOffset.Size = new System.Drawing.Size(50, 12);
            this.LABEL_TagAccessOffset.TabIndex = 2;
            this.LABEL_TagAccessOffset.Text = "Offset (h)";
            // 
            // LABEL_TagAccessCount
            // 
            this.LABEL_TagAccessCount.AutoSize = true;
            this.LABEL_TagAccessCount.Location = new System.Drawing.Point(18, 101);
            this.LABEL_TagAccessCount.Name = "LABEL_TagAccessCount";
            this.LABEL_TagAccessCount.Size = new System.Drawing.Size(34, 12);
            this.LABEL_TagAccessCount.TabIndex = 8;
            this.LABEL_TagAccessCount.Text = "Count";
            // 
            // BUTTON_TagAccessCancel
            // 
            this.BUTTON_TagAccessCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BUTTON_TagAccessCancel.Location = new System.Drawing.Point(106, 287);
            this.BUTTON_TagAccessCancel.Name = "BUTTON_TagAccessCancel";
            this.BUTTON_TagAccessCancel.Size = new System.Drawing.Size(75, 21);
            this.BUTTON_TagAccessCancel.TabIndex = 11;
            this.BUTTON_TagAccessCancel.Text = "Cancel";
            this.BUTTON_TagAccessCancel.UseVisualStyleBackColor = true;
            // 
            // BUTTON_TagAccessOk
            // 
            this.BUTTON_TagAccessOk.Location = new System.Drawing.Point(16, 287);
            this.BUTTON_TagAccessOk.Name = "BUTTON_TagAccessOk";
            this.BUTTON_TagAccessOk.Size = new System.Drawing.Size(75, 21);
            this.BUTTON_TagAccessOk.TabIndex = 10;
            this.BUTTON_TagAccessOk.Text = "OK";
            this.BUTTON_TagAccessOk.UseVisualStyleBackColor = true;
            this.BUTTON_TagAccessOk.Click += new System.EventHandler(this.okButton_Click);
            // 
            // TEXTBOX_TagAccessAccessPassword
            // 
            this.TEXTBOX_TagAccessAccessPassword.Location = new System.Drawing.Point(119, 176);
            this.TEXTBOX_TagAccessAccessPassword.MaxLength = 8;
            this.TEXTBOX_TagAccessAccessPassword.Name = "TEXTBOX_TagAccessAccessPassword";
            this.TEXTBOX_TagAccessAccessPassword.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessAccessPassword.TabIndex = 7;
            this.TEXTBOX_TagAccessAccessPassword.Text = "0";
            this.TEXTBOX_TagAccessAccessPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessAccessPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // LABEL_TagAccessAccessPassword
            // 
            this.LABEL_TagAccessAccessPassword.AutoSize = true;
            this.LABEL_TagAccessAccessPassword.Location = new System.Drawing.Point(17, 185);
            this.LABEL_TagAccessAccessPassword.Name = "LABEL_TagAccessAccessPassword";
            this.LABEL_TagAccessAccessPassword.Size = new System.Drawing.Size(80, 12);
            this.LABEL_TagAccessAccessPassword.TabIndex = 6;
            this.LABEL_TagAccessAccessPassword.Text = "Accesss Pwd (h)";
            // 
            // TEXTBOX_TagAccessValue1
            // 
            this.TEXTBOX_TagAccessValue1.Location = new System.Drawing.Point(119, 120);
            this.TEXTBOX_TagAccessValue1.MaxLength = 4;
            this.TEXTBOX_TagAccessValue1.Name = "TEXTBOX_TagAccessValue1";
            this.TEXTBOX_TagAccessValue1.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessValue1.TabIndex = 5;
            this.TEXTBOX_TagAccessValue1.Text = "0";
            this.TEXTBOX_TagAccessValue1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessValue1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // LABEL_TagAccessValue1
            // 
            this.LABEL_TagAccessValue1.AutoSize = true;
            this.LABEL_TagAccessValue1.Location = new System.Drawing.Point(18, 130);
            this.LABEL_TagAccessValue1.Name = "LABEL_TagAccessValue1";
            this.LABEL_TagAccessValue1.Size = new System.Drawing.Size(58, 12);
            this.LABEL_TagAccessValue1.TabIndex = 4;
            this.LABEL_TagAccessValue1.Text = "Value 1 (h)";
            // 
            // COMBOBOX_TagAccess
            // 
            this.COMBOBOX_TagAccess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_TagAccess.FormattingEnabled = true;
            this.COMBOBOX_TagAccess.Location = new System.Drawing.Point(106, 12);
            this.COMBOBOX_TagAccess.Name = "COMBOBOX_TagAccess";
            this.COMBOBOX_TagAccess.Size = new System.Drawing.Size(121, 20);
            this.COMBOBOX_TagAccess.TabIndex = 13;
            this.COMBOBOX_TagAccess.SelectedIndexChanged += new System.EventHandler(this.COMBOBOX_TagAccess_SelectedIndexChanged);
            // 
            // LABEL_AccessType
            // 
            this.LABEL_AccessType.AutoSize = true;
            this.LABEL_AccessType.Location = new System.Drawing.Point(18, 20);
            this.LABEL_AccessType.Name = "LABEL_AccessType";
            this.LABEL_AccessType.Size = new System.Drawing.Size(63, 12);
            this.LABEL_AccessType.TabIndex = 14;
            this.LABEL_AccessType.Text = "Access Type";
            // 
            // LABEL_TagAccessKillPassword
            // 
            this.LABEL_TagAccessKillPassword.AutoSize = true;
            this.LABEL_TagAccessKillPassword.Location = new System.Drawing.Point(18, 213);
            this.LABEL_TagAccessKillPassword.Name = "LABEL_TagAccessKillPassword";
            this.LABEL_TagAccessKillPassword.Size = new System.Drawing.Size(62, 12);
            this.LABEL_TagAccessKillPassword.TabIndex = 15;
            this.LABEL_TagAccessKillPassword.Text = "Kill Pwd (h)";
            // 
            // TEXTBOX_TagAccessKillPassword
            // 
            this.TEXTBOX_TagAccessKillPassword.Location = new System.Drawing.Point(119, 204);
            this.TEXTBOX_TagAccessKillPassword.MaxLength = 8;
            this.TEXTBOX_TagAccessKillPassword.Name = "TEXTBOX_TagAccessKillPassword";
            this.TEXTBOX_TagAccessKillPassword.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessKillPassword.TabIndex = 16;
            this.TEXTBOX_TagAccessKillPassword.Text = "0";
            this.TEXTBOX_TagAccessKillPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessKillPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // GROUPBOX_TagAccessQTControl
            // 
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.LABEL_QTMemory);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.LABEL_QTRange);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.LABEL_QTPersistence);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.LABEL_QTAccess);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.COMBOBOX_QTMemMap);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.COMBOBOX_QTShortRange);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.COMBOBOX_QTPersistence);
            this.GROUPBOX_TagAccessQTControl.Controls.Add(this.COMBOBOX_QTCtrlType);
            this.GROUPBOX_TagAccessQTControl.Location = new System.Drawing.Point(243, 17);
            this.GROUPBOX_TagAccessQTControl.Name = "GROUPBOX_TagAccessQTControl";
            this.GROUPBOX_TagAccessQTControl.Size = new System.Drawing.Size(294, 105);
            this.GROUPBOX_TagAccessQTControl.TabIndex = 17;
            this.GROUPBOX_TagAccessQTControl.TabStop = false;
            this.GROUPBOX_TagAccessQTControl.Text = "QT Control";
            // 
            // LABEL_QTMemory
            // 
            this.LABEL_QTMemory.AutoSize = true;
            this.LABEL_QTMemory.Location = new System.Drawing.Point(12, 81);
            this.LABEL_QTMemory.Name = "LABEL_QTMemory";
            this.LABEL_QTMemory.Size = new System.Drawing.Size(45, 12);
            this.LABEL_QTMemory.TabIndex = 7;
            this.LABEL_QTMemory.Text = "Memory";
            // 
            // LABEL_QTRange
            // 
            this.LABEL_QTRange.AutoSize = true;
            this.LABEL_QTRange.Location = new System.Drawing.Point(12, 61);
            this.LABEL_QTRange.Name = "LABEL_QTRange";
            this.LABEL_QTRange.Size = new System.Drawing.Size(63, 12);
            this.LABEL_QTRange.TabIndex = 6;
            this.LABEL_QTRange.Text = "Short Range";
            // 
            // LABEL_QTPersistence
            // 
            this.LABEL_QTPersistence.AutoSize = true;
            this.LABEL_QTPersistence.Location = new System.Drawing.Point(12, 41);
            this.LABEL_QTPersistence.Name = "LABEL_QTPersistence";
            this.LABEL_QTPersistence.Size = new System.Drawing.Size(55, 12);
            this.LABEL_QTPersistence.TabIndex = 5;
            this.LABEL_QTPersistence.Text = "Persistence";
            // 
            // LABEL_QTAccess
            // 
            this.LABEL_QTAccess.AutoSize = true;
            this.LABEL_QTAccess.Location = new System.Drawing.Point(12, 20);
            this.LABEL_QTAccess.Name = "LABEL_QTAccess";
            this.LABEL_QTAccess.Size = new System.Drawing.Size(36, 12);
            this.LABEL_QTAccess.TabIndex = 4;
            this.LABEL_QTAccess.Text = "Access";
            // 
            // COMBOBOX_QTMemMap
            // 
            this.COMBOBOX_QTMemMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_QTMemMap.FormattingEnabled = true;
            this.COMBOBOX_QTMemMap.Location = new System.Drawing.Point(91, 78);
            this.COMBOBOX_QTMemMap.Name = "COMBOBOX_QTMemMap";
            this.COMBOBOX_QTMemMap.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_QTMemMap.TabIndex = 3;
            // 
            // COMBOBOX_QTShortRange
            // 
            this.COMBOBOX_QTShortRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_QTShortRange.FormattingEnabled = true;
            this.COMBOBOX_QTShortRange.Location = new System.Drawing.Point(91, 58);
            this.COMBOBOX_QTShortRange.Name = "COMBOBOX_QTShortRange";
            this.COMBOBOX_QTShortRange.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_QTShortRange.TabIndex = 2;
            // 
            // COMBOBOX_QTPersistence
            // 
            this.COMBOBOX_QTPersistence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_QTPersistence.FormattingEnabled = true;
            this.COMBOBOX_QTPersistence.Location = new System.Drawing.Point(91, 38);
            this.COMBOBOX_QTPersistence.Name = "COMBOBOX_QTPersistence";
            this.COMBOBOX_QTPersistence.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_QTPersistence.TabIndex = 1;
            // 
            // COMBOBOX_QTCtrlType
            // 
            this.COMBOBOX_QTCtrlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_QTCtrlType.FormattingEnabled = true;
            this.COMBOBOX_QTCtrlType.Location = new System.Drawing.Point(91, 18);
            this.COMBOBOX_QTCtrlType.Name = "COMBOBOX_QTCtrlType";
            this.COMBOBOX_QTCtrlType.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_QTCtrlType.TabIndex = 0;
            this.COMBOBOX_QTCtrlType.SelectedIndexChanged += new System.EventHandler(this.COMBOBOX_QTCtrlType_SelectedIndexChanged);
            // 
            // GROUPBOX_TagAccessPermissions
            // 
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.COMBOBOX_UserBankPermissions);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.COMBOBOX_TIDBankPermissions);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.COMBOBOX_EPCBankPermissions);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.COMBOBOX_AccessPasswordPermissions);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.LABEL_PermissionsUserBank);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.LABEL_PermissionsTIDBank);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.LABEL_PermissionsEPCBank);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.LABEL_PermissionsAccessPassword);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.LABEL_PermissionsKillPassword);
            this.GROUPBOX_TagAccessPermissions.Controls.Add(this.COMBOBOX_KillPasswordPermissions);
            this.GROUPBOX_TagAccessPermissions.Location = new System.Drawing.Point(243, 17);
            this.GROUPBOX_TagAccessPermissions.Name = "GROUPBOX_TagAccessPermissions";
            this.GROUPBOX_TagAccessPermissions.Size = new System.Drawing.Size(294, 152);
            this.GROUPBOX_TagAccessPermissions.TabIndex = 18;
            this.GROUPBOX_TagAccessPermissions.TabStop = false;
            this.GROUPBOX_TagAccessPermissions.Text = "Permissions";
            // 
            // COMBOBOX_UserBankPermissions
            // 
            this.COMBOBOX_UserBankPermissions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_UserBankPermissions.FormattingEnabled = true;
            this.COMBOBOX_UserBankPermissions.Location = new System.Drawing.Point(91, 121);
            this.COMBOBOX_UserBankPermissions.Name = "COMBOBOX_UserBankPermissions";
            this.COMBOBOX_UserBankPermissions.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_UserBankPermissions.TabIndex = 14;
            // 
            // COMBOBOX_TIDBankPermissions
            // 
            this.COMBOBOX_TIDBankPermissions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_TIDBankPermissions.FormattingEnabled = true;
            this.COMBOBOX_TIDBankPermissions.Location = new System.Drawing.Point(91, 95);
            this.COMBOBOX_TIDBankPermissions.Name = "COMBOBOX_TIDBankPermissions";
            this.COMBOBOX_TIDBankPermissions.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_TIDBankPermissions.TabIndex = 13;
            // 
            // COMBOBOX_EPCBankPermissions
            // 
            this.COMBOBOX_EPCBankPermissions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_EPCBankPermissions.FormattingEnabled = true;
            this.COMBOBOX_EPCBankPermissions.Location = new System.Drawing.Point(91, 69);
            this.COMBOBOX_EPCBankPermissions.Name = "COMBOBOX_EPCBankPermissions";
            this.COMBOBOX_EPCBankPermissions.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_EPCBankPermissions.TabIndex = 12;
            // 
            // COMBOBOX_AccessPasswordPermissions
            // 
            this.COMBOBOX_AccessPasswordPermissions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_AccessPasswordPermissions.FormattingEnabled = true;
            this.COMBOBOX_AccessPasswordPermissions.Location = new System.Drawing.Point(91, 43);
            this.COMBOBOX_AccessPasswordPermissions.Name = "COMBOBOX_AccessPasswordPermissions";
            this.COMBOBOX_AccessPasswordPermissions.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_AccessPasswordPermissions.TabIndex = 11;
            // 
            // LABEL_PermissionsUserBank
            // 
            this.LABEL_PermissionsUserBank.AutoSize = true;
            this.LABEL_PermissionsUserBank.Location = new System.Drawing.Point(12, 100);
            this.LABEL_PermissionsUserBank.Name = "LABEL_PermissionsUserBank";
            this.LABEL_PermissionsUserBank.Size = new System.Drawing.Size(54, 12);
            this.LABEL_PermissionsUserBank.TabIndex = 10;
            this.LABEL_PermissionsUserBank.Text = "User Bank";
            // 
            // LABEL_PermissionsTIDBank
            // 
            this.LABEL_PermissionsTIDBank.AutoSize = true;
            this.LABEL_PermissionsTIDBank.Location = new System.Drawing.Point(12, 124);
            this.LABEL_PermissionsTIDBank.Name = "LABEL_PermissionsTIDBank";
            this.LABEL_PermissionsTIDBank.Size = new System.Drawing.Size(52, 12);
            this.LABEL_PermissionsTIDBank.TabIndex = 9;
            this.LABEL_PermissionsTIDBank.Text = "TID Bank";
            // 
            // LABEL_PermissionsEPCBank
            // 
            this.LABEL_PermissionsEPCBank.AutoSize = true;
            this.LABEL_PermissionsEPCBank.Location = new System.Drawing.Point(12, 72);
            this.LABEL_PermissionsEPCBank.Name = "LABEL_PermissionsEPCBank";
            this.LABEL_PermissionsEPCBank.Size = new System.Drawing.Size(54, 12);
            this.LABEL_PermissionsEPCBank.TabIndex = 8;
            this.LABEL_PermissionsEPCBank.Text = "EPC Bank";
            // 
            // LABEL_PermissionsAccessPassword
            // 
            this.LABEL_PermissionsAccessPassword.AutoSize = true;
            this.LABEL_PermissionsAccessPassword.Location = new System.Drawing.Point(12, 46);
            this.LABEL_PermissionsAccessPassword.Name = "LABEL_PermissionsAccessPassword";
            this.LABEL_PermissionsAccessPassword.Size = new System.Drawing.Size(59, 12);
            this.LABEL_PermissionsAccessPassword.TabIndex = 7;
            this.LABEL_PermissionsAccessPassword.Text = "Access Pwd";
            // 
            // LABEL_PermissionsKillPassword
            // 
            this.LABEL_PermissionsKillPassword.AutoSize = true;
            this.LABEL_PermissionsKillPassword.Location = new System.Drawing.Point(12, 23);
            this.LABEL_PermissionsKillPassword.Name = "LABEL_PermissionsKillPassword";
            this.LABEL_PermissionsKillPassword.Size = new System.Drawing.Size(45, 12);
            this.LABEL_PermissionsKillPassword.TabIndex = 6;
            this.LABEL_PermissionsKillPassword.Text = "Kill Pwd";
            // 
            // COMBOBOX_KillPasswordPermissions
            // 
            this.COMBOBOX_KillPasswordPermissions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_KillPasswordPermissions.FormattingEnabled = true;
            this.COMBOBOX_KillPasswordPermissions.Location = new System.Drawing.Point(91, 18);
            this.COMBOBOX_KillPasswordPermissions.Name = "COMBOBOX_KillPasswordPermissions";
            this.COMBOBOX_KillPasswordPermissions.Size = new System.Drawing.Size(196, 20);
            this.COMBOBOX_KillPasswordPermissions.TabIndex = 5;
            // 
            // NUMERICUPDOWN_TagAccessCount
            // 
            this.NUMERICUPDOWN_TagAccessCount.Location = new System.Drawing.Point(119, 92);
            this.NUMERICUPDOWN_TagAccessCount.Name = "NUMERICUPDOWN_TagAccessCount";
            this.NUMERICUPDOWN_TagAccessCount.Size = new System.Drawing.Size(74, 22);
            this.NUMERICUPDOWN_TagAccessCount.TabIndex = 19;
            this.NUMERICUPDOWN_TagAccessCount.ValueChanged += new System.EventHandler(this.NUMERICUPDOWN_TagAccessCount_ValueChanged);
            // 
            // LABEL_TagAccessValue2
            // 
            this.LABEL_TagAccessValue2.AutoSize = true;
            this.LABEL_TagAccessValue2.Location = new System.Drawing.Point(18, 157);
            this.LABEL_TagAccessValue2.Name = "LABEL_TagAccessValue2";
            this.LABEL_TagAccessValue2.Size = new System.Drawing.Size(58, 12);
            this.LABEL_TagAccessValue2.TabIndex = 20;
            this.LABEL_TagAccessValue2.Text = "Value 2 (h)";
            // 
            // TEXTBOX_TagAccessValue2
            // 
            this.TEXTBOX_TagAccessValue2.Location = new System.Drawing.Point(119, 148);
            this.TEXTBOX_TagAccessValue2.MaxLength = 4;
            this.TEXTBOX_TagAccessValue2.Name = "TEXTBOX_TagAccessValue2";
            this.TEXTBOX_TagAccessValue2.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessValue2.TabIndex = 21;
            this.TEXTBOX_TagAccessValue2.Text = "0";
            this.TEXTBOX_TagAccessValue2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessValue2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // chkPerformSelectOps
            // 
            this.chkPerformSelectOps.AutoSize = true;
            this.chkPerformSelectOps.Location = new System.Drawing.Point(14, 27);
            this.chkPerformSelectOps.Name = "chkPerformSelectOps";
            this.chkPerformSelectOps.Size = new System.Drawing.Size(121, 16);
            this.chkPerformSelectOps.TabIndex = 22;
            this.chkPerformSelectOps.Text = "Activate Select Rules";
            this.chkPerformSelectOps.UseVisualStyleBackColor = true;
            // 
            // chkPerformPostMatch
            // 
            this.chkPerformPostMatch.AutoSize = true;
            this.chkPerformPostMatch.Location = new System.Drawing.Point(14, 50);
            this.chkPerformPostMatch.Name = "chkPerformPostMatch";
            this.chkPerformPostMatch.Size = new System.Drawing.Size(170, 16);
            this.chkPerformPostMatch.TabIndex = 23;
            this.chkPerformPostMatch.Text = "Activate Post-Singulation Rules";
            this.chkPerformPostMatch.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkPerformSelectOps);
            this.groupBox1.Controls.Add(this.chkPerformPostMatch);
            this.groupBox1.Location = new System.Drawing.Point(243, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 87);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tag Access Rules";
            // 
            // LABEL_TagAccessReadWords
            // 
            this.LABEL_TagAccessReadWords.AutoSize = true;
            this.LABEL_TagAccessReadWords.Location = new System.Drawing.Point(18, 238);
            this.LABEL_TagAccessReadWords.Name = "LABEL_TagAccessReadWords";
            this.LABEL_TagAccessReadWords.Size = new System.Drawing.Size(63, 12);
            this.LABEL_TagAccessReadWords.TabIndex = 26;
            this.LABEL_TagAccessReadWords.Text = "Read Words";
            // 
            // LABEL_TagAccessTotalReadWords
            // 
            this.LABEL_TagAccessTotalReadWords.AutoSize = true;
            this.LABEL_TagAccessTotalReadWords.Location = new System.Drawing.Point(18, 260);
            this.LABEL_TagAccessTotalReadWords.Name = "LABEL_TagAccessTotalReadWords";
            this.LABEL_TagAccessTotalReadWords.Size = new System.Drawing.Size(90, 12);
            this.LABEL_TagAccessTotalReadWords.TabIndex = 27;
            this.LABEL_TagAccessTotalReadWords.Text = "Total Read Words";
            // 
            // COMBOBOX_TagAccessReadWords
            // 
            this.COMBOBOX_TagAccessReadWords.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMBOBOX_TagAccessReadWords.FormattingEnabled = true;
            this.COMBOBOX_TagAccessReadWords.Items.AddRange(new object[] {
            "8",
            "16",
            "24",
            "32",
            "40",
            "48",
            "56",
            "64",
            "128"});
            this.COMBOBOX_TagAccessReadWords.Location = new System.Drawing.Point(119, 232);
            this.COMBOBOX_TagAccessReadWords.Name = "COMBOBOX_TagAccessReadWords";
            this.COMBOBOX_TagAccessReadWords.Size = new System.Drawing.Size(74, 20);
            this.COMBOBOX_TagAccessReadWords.TabIndex = 30;
            // 
            // TEXTBOX_TagAccessTotalReadWords
            // 
            this.TEXTBOX_TagAccessTotalReadWords.Location = new System.Drawing.Point(119, 257);
            this.TEXTBOX_TagAccessTotalReadWords.MaxLength = 4;
            this.TEXTBOX_TagAccessTotalReadWords.Name = "TEXTBOX_TagAccessTotalReadWords";
            this.TEXTBOX_TagAccessTotalReadWords.Size = new System.Drawing.Size(74, 22);
            this.TEXTBOX_TagAccessTotalReadWords.TabIndex = 31;
            this.TEXTBOX_TagAccessTotalReadWords.Text = "3840";
            // 
            // FORM_TagAccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BUTTON_TagAccessCancel;
            this.ClientSize = new System.Drawing.Size(556, 329);
            this.Controls.Add(this.TEXTBOX_TagAccessTotalReadWords);
            this.Controls.Add(this.COMBOBOX_TagAccessReadWords);
            this.Controls.Add(this.LABEL_TagAccessTotalReadWords);
            this.Controls.Add(this.LABEL_TagAccessReadWords);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LABEL_TagAccessValue2);
            this.Controls.Add(this.TEXTBOX_TagAccessValue2);
            this.Controls.Add(this.NUMERICUPDOWN_TagAccessCount);
            this.Controls.Add(this.GROUPBOX_TagAccessPermissions);
            this.Controls.Add(this.GROUPBOX_TagAccessQTControl);
            this.Controls.Add(this.LABEL_TagAccessKillPassword);
            this.Controls.Add(this.TEXTBOX_TagAccessKillPassword);
            this.Controls.Add(this.LABEL_AccessType);
            this.Controls.Add(this.COMBOBOX_TagAccess);
            this.Controls.Add(this.BUTTON_TagAccessCancel);
            this.Controls.Add(this.BUTTON_TagAccessOk);
            this.Controls.Add(this.LABEL_TagAccessAccessPassword);
            this.Controls.Add(this.LABEL_TagAccessValue1);
            this.Controls.Add(this.LABEL_TagAccessCount);
            this.Controls.Add(this.TEXTBOX_TagAccessAccessPassword);
            this.Controls.Add(this.TEXTBOX_TagAccessValue1);
            this.Controls.Add(this.LABEL_TagAccessOffset);
            this.Controls.Add(this.TEXTBOX_TagAccessOffset);
            this.Controls.Add(this.LABEL_TagAccessMemoryBank);
            this.Controls.Add(this.COMBOBOX_TagAccessMemoryBank);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FORM_TagAccess";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tag Access";
            this.GROUPBOX_TagAccessQTControl.ResumeLayout(false);
            this.GROUPBOX_TagAccessQTControl.PerformLayout();
            this.GROUPBOX_TagAccessPermissions.ResumeLayout(false);
            this.GROUPBOX_TagAccessPermissions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUMERICUPDOWN_TagAccessCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox COMBOBOX_TagAccessMemoryBank;
		private System.Windows.Forms.Label LABEL_TagAccessMemoryBank;
		private System.Windows.Forms.TextBox TEXTBOX_TagAccessOffset;
        private System.Windows.Forms.Label LABEL_TagAccessOffset;
		private System.Windows.Forms.Label LABEL_TagAccessCount;
		private System.Windows.Forms.Button BUTTON_TagAccessCancel;
		private System.Windows.Forms.Button BUTTON_TagAccessOk;
		private System.Windows.Forms.TextBox TEXTBOX_TagAccessAccessPassword;
		private System.Windows.Forms.Label LABEL_TagAccessAccessPassword;
		private System.Windows.Forms.TextBox TEXTBOX_TagAccessValue1;
        private System.Windows.Forms.Label LABEL_TagAccessValue1;
        private System.Windows.Forms.ComboBox COMBOBOX_TagAccess;
        private System.Windows.Forms.Label LABEL_AccessType;
        private System.Windows.Forms.Label LABEL_TagAccessKillPassword;
        private System.Windows.Forms.TextBox TEXTBOX_TagAccessKillPassword;
        private System.Windows.Forms.GroupBox GROUPBOX_TagAccessQTControl;
        private System.Windows.Forms.Label LABEL_QTMemory;
        private System.Windows.Forms.Label LABEL_QTRange;
        private System.Windows.Forms.Label LABEL_QTPersistence;
        private System.Windows.Forms.Label LABEL_QTAccess;
        private System.Windows.Forms.ComboBox COMBOBOX_QTMemMap;
        private System.Windows.Forms.ComboBox COMBOBOX_QTShortRange;
        private System.Windows.Forms.ComboBox COMBOBOX_QTPersistence;
        private System.Windows.Forms.ComboBox COMBOBOX_QTCtrlType;
        private System.Windows.Forms.GroupBox GROUPBOX_TagAccessPermissions;
        private System.Windows.Forms.ComboBox COMBOBOX_AccessPasswordPermissions;
        private System.Windows.Forms.Label LABEL_PermissionsUserBank;
        private System.Windows.Forms.Label LABEL_PermissionsTIDBank;
        private System.Windows.Forms.Label LABEL_PermissionsEPCBank;
        private System.Windows.Forms.Label LABEL_PermissionsAccessPassword;
        private System.Windows.Forms.Label LABEL_PermissionsKillPassword;
        private System.Windows.Forms.ComboBox COMBOBOX_KillPasswordPermissions;
        private System.Windows.Forms.ComboBox COMBOBOX_UserBankPermissions;
        private System.Windows.Forms.ComboBox COMBOBOX_TIDBankPermissions;
        private System.Windows.Forms.ComboBox COMBOBOX_EPCBankPermissions;
        private System.Windows.Forms.NumericUpDown NUMERICUPDOWN_TagAccessCount;
        private System.Windows.Forms.Label LABEL_TagAccessValue2;
        private System.Windows.Forms.TextBox TEXTBOX_TagAccessValue2;
        private System.Windows.Forms.CheckBox chkPerformSelectOps;
        private System.Windows.Forms.CheckBox chkPerformPostMatch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LABEL_TagAccessReadWords;
        private System.Windows.Forms.Label LABEL_TagAccessTotalReadWords;
        private System.Windows.Forms.ComboBox COMBOBOX_TagAccessReadWords;
        private System.Windows.Forms.TextBox TEXTBOX_TagAccessTotalReadWords;
	}
}