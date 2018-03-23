namespace RFID_Explorer
{
    partial class NXPAuthentication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelRandomChallenge = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxVerificationKey1 = new System.Windows.Forms.TextBox();
            this.btnDefaultKey1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxMemBank = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxOffset = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxBlockCount = new System.Windows.Forms.ComboBox();
            this.btnTAM2Authenticate = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnTAM1Authenticate = new System.Windows.Forms.Button();
            this.btnDefaultKey0 = new System.Windows.Forms.Button();
            this.textBoxVerificationKey0 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelTagMemoryData = new System.Windows.Forms.Label();
            this.labelDecryptedChallenge = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnActivateKey = new System.Windows.Forms.Button();
            this.btnInsertKey = new System.Windows.Forms.Button();
            this.btnGetKey = new System.Windows.Forms.Button();
            this.textBoxInserKey1 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxInserKey0 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.chkPerformSelectOps = new System.Windows.Forms.CheckBox();
            this.chkPerformPostMatch = new System.Windows.Forms.CheckBox();
            this.textBoxEPC = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LABEL_TagAccessAccessPassword = new System.Windows.Forms.Label();
            this.TEXTBOX_TagAccessAccessPassword = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelRandomChallenge);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.labelTagMemoryData);
            this.groupBox1.Controls.Add(this.labelDecryptedChallenge);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Location = new System.Drawing.Point(12, 232);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 354);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Key Authentication";
            // 
            // labelRandomChallenge
            // 
            this.labelRandomChallenge.AutoSize = true;
            this.labelRandomChallenge.Location = new System.Drawing.Point(182, 269);
            this.labelRandomChallenge.Name = "labelRandomChallenge";
            this.labelRandomChallenge.Size = new System.Drawing.Size(0, 12);
            this.labelRandomChallenge.TabIndex = 71;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(42, 267);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 14);
            this.label9.TabIndex = 70;
            this.label9.Text = "Random Challenge =>";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBoxVerificationKey1);
            this.groupBox3.Controls.Add(this.btnDefaultKey1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.comboBoxMemBank);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBoxOffset);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.comboBoxBlockCount);
            this.groupBox3.Controls.Add(this.btnTAM2Authenticate);
            this.groupBox3.Location = new System.Drawing.Point(12, 127);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(524, 126);
            this.groupBox3.TabIndex = 69;
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 14);
            this.label4.TabIndex = 27;
            this.label4.Text = "Verification Key1";
            // 
            // textBoxVerificationKey1
            // 
            this.textBoxVerificationKey1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxVerificationKey1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVerificationKey1.Location = new System.Drawing.Point(136, 20);
            this.textBoxVerificationKey1.MaxLength = 32;
            this.textBoxVerificationKey1.Name = "textBoxVerificationKey1";
            this.textBoxVerificationKey1.Size = new System.Drawing.Size(293, 22);
            this.textBoxVerificationKey1.TabIndex = 22;
            // 
            // btnDefaultKey1
            // 
            this.btnDefaultKey1.Location = new System.Drawing.Point(435, 20);
            this.btnDefaultKey1.Name = "btnDefaultKey1";
            this.btnDefaultKey1.Size = new System.Drawing.Size(83, 23);
            this.btnDefaultKey1.TabIndex = 52;
            this.btnDefaultKey1.Text = "Default Key1";
            this.btnDefaultKey1.UseVisualStyleBackColor = true;
            this.btnDefaultKey1.Click += new System.EventHandler(this.btnDefaultKey1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 14);
            this.label1.TabIndex = 53;
            this.label1.Text = "Memory Profile";
            // 
            // comboBoxMemBank
            // 
            this.comboBoxMemBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMemBank.DropDownWidth = 80;
            this.comboBoxMemBank.FormattingEnabled = true;
            this.comboBoxMemBank.Location = new System.Drawing.Point(116, 57);
            this.comboBoxMemBank.Name = "comboBoxMemBank";
            this.comboBoxMemBank.Size = new System.Drawing.Size(68, 20);
            this.comboBoxMemBank.TabIndex = 54;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(215, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 55;
            this.label2.Text = "Offset";
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Location = new System.Drawing.Point(265, 55);
            this.textBoxOffset.MaxLength = 4;
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(38, 22);
            this.textBoxOffset.TabIndex = 56;
            this.textBoxOffset.Text = "0";
            this.toolTip1.SetToolTip(this.textBoxOffset, "The offset of the first 64-bit blocks, where zero is the first 64-bit block in th" +
                    "e memory bank.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(324, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 14);
            this.label5.TabIndex = 57;
            this.label5.Text = "Block Count";
            // 
            // comboBoxBlockCount
            // 
            this.comboBoxBlockCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBlockCount.DropDownWidth = 80;
            this.comboBoxBlockCount.FormattingEnabled = true;
            this.comboBoxBlockCount.Location = new System.Drawing.Point(411, 58);
            this.comboBoxBlockCount.Name = "comboBoxBlockCount";
            this.comboBoxBlockCount.Size = new System.Drawing.Size(107, 20);
            this.comboBoxBlockCount.TabIndex = 58;
            // 
            // btnTAM2Authenticate
            // 
            this.btnTAM2Authenticate.Location = new System.Drawing.Point(360, 89);
            this.btnTAM2Authenticate.Name = "btnTAM2Authenticate";
            this.btnTAM2Authenticate.Size = new System.Drawing.Size(158, 23);
            this.btnTAM2Authenticate.TabIndex = 30;
            this.btnTAM2Authenticate.Text = "TAM2 Authenticate";
            this.btnTAM2Authenticate.UseVisualStyleBackColor = true;
            this.btnTAM2Authenticate.Click += new System.EventHandler(this.btnTAM2Authenticate_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnTAM1Authenticate);
            this.groupBox5.Controls.Add(this.btnDefaultKey0);
            this.groupBox5.Controls.Add(this.textBoxVerificationKey0);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Location = new System.Drawing.Point(12, 18);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(524, 94);
            this.groupBox5.TabIndex = 68;
            this.groupBox5.TabStop = false;
            // 
            // btnTAM1Authenticate
            // 
            this.btnTAM1Authenticate.Location = new System.Drawing.Point(360, 59);
            this.btnTAM1Authenticate.Name = "btnTAM1Authenticate";
            this.btnTAM1Authenticate.Size = new System.Drawing.Size(158, 23);
            this.btnTAM1Authenticate.TabIndex = 54;
            this.btnTAM1Authenticate.Text = "TAM1 Authenticate";
            this.btnTAM1Authenticate.UseVisualStyleBackColor = true;
            this.btnTAM1Authenticate.Click += new System.EventHandler(this.btnTAM1Authenticate_Click);
            // 
            // btnDefaultKey0
            // 
            this.btnDefaultKey0.Location = new System.Drawing.Point(435, 21);
            this.btnDefaultKey0.Name = "btnDefaultKey0";
            this.btnDefaultKey0.Size = new System.Drawing.Size(83, 23);
            this.btnDefaultKey0.TabIndex = 55;
            this.btnDefaultKey0.Text = "Default Key0";
            this.btnDefaultKey0.UseVisualStyleBackColor = true;
            this.btnDefaultKey0.Click += new System.EventHandler(this.btnDefaultKey0_Click);
            // 
            // textBoxVerificationKey0
            // 
            this.textBoxVerificationKey0.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxVerificationKey0.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxVerificationKey0.Location = new System.Drawing.Point(136, 22);
            this.textBoxVerificationKey0.MaxLength = 32;
            this.textBoxVerificationKey0.Name = "textBoxVerificationKey0";
            this.textBoxVerificationKey0.Size = new System.Drawing.Size(293, 22);
            this.textBoxVerificationKey0.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 14);
            this.label3.TabIndex = 52;
            this.label3.Text = "Verification Key0";
            // 
            // labelTagMemoryData
            // 
            this.labelTagMemoryData.AutoSize = true;
            this.labelTagMemoryData.Location = new System.Drawing.Point(182, 326);
            this.labelTagMemoryData.Name = "labelTagMemoryData";
            this.labelTagMemoryData.Size = new System.Drawing.Size(0, 12);
            this.labelTagMemoryData.TabIndex = 62;
            // 
            // labelDecryptedChallenge
            // 
            this.labelDecryptedChallenge.AutoSize = true;
            this.labelDecryptedChallenge.Location = new System.Drawing.Point(182, 297);
            this.labelDecryptedChallenge.Name = "labelDecryptedChallenge";
            this.labelDecryptedChallenge.Size = new System.Drawing.Size(0, 12);
            this.labelDecryptedChallenge.TabIndex = 60;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 14);
            this.label6.TabIndex = 59;
            this.label6.Text = "Decrypted Challenge =>";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(49, 324);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(133, 14);
            this.label25.TabIndex = 61;
            this.label25.Text = "Tag Memory Data =>";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnActivateKey);
            this.groupBox2.Controls.Add(this.btnInsertKey);
            this.groupBox2.Controls.Add(this.btnGetKey);
            this.groupBox2.Controls.Add(this.textBoxInserKey1);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.textBoxInserKey0);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Location = new System.Drawing.Point(12, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(548, 130);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Key Insertion and Activation";
            // 
            // btnActivateKey
            // 
            this.btnActivateKey.Location = new System.Drawing.Point(459, 96);
            this.btnActivateKey.Name = "btnActivateKey";
            this.btnActivateKey.Size = new System.Drawing.Size(83, 23);
            this.btnActivateKey.TabIndex = 55;
            this.btnActivateKey.Text = "Activate Keys";
            this.btnActivateKey.UseVisualStyleBackColor = true;
            this.btnActivateKey.Click += new System.EventHandler(this.btnActivateKey_Click);
            // 
            // btnInsertKey
            // 
            this.btnInsertKey.Location = new System.Drawing.Point(346, 96);
            this.btnInsertKey.Name = "btnInsertKey";
            this.btnInsertKey.Size = new System.Drawing.Size(83, 23);
            this.btnInsertKey.TabIndex = 54;
            this.btnInsertKey.Text = "Insert Keys";
            this.btnInsertKey.UseVisualStyleBackColor = true;
            this.btnInsertKey.Click += new System.EventHandler(this.btnInsertKey_Click);
            // 
            // btnGetKey
            // 
            this.btnGetKey.Location = new System.Drawing.Point(232, 96);
            this.btnGetKey.Name = "btnGetKey";
            this.btnGetKey.Size = new System.Drawing.Size(83, 23);
            this.btnGetKey.TabIndex = 53;
            this.btnGetKey.Text = "Get Keys";
            this.btnGetKey.UseVisualStyleBackColor = true;
            this.btnGetKey.Click += new System.EventHandler(this.btnGetKey_Click);
            // 
            // textBoxInserKey1
            // 
            this.textBoxInserKey1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxInserKey1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInserKey1.Location = new System.Drawing.Point(116, 62);
            this.textBoxInserKey1.MaxLength = 32;
            this.textBoxInserKey1.Name = "textBoxInserKey1";
            this.textBoxInserKey1.Size = new System.Drawing.Size(426, 22);
            this.textBoxInserKey1.TabIndex = 38;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(12, 65);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(98, 14);
            this.label24.TabIndex = 37;
            this.label24.Text = "Key1 To Write";
            // 
            // textBoxInserKey0
            // 
            this.textBoxInserKey0.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxInserKey0.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInserKey0.Location = new System.Drawing.Point(116, 23);
            this.textBoxInserKey0.MaxLength = 32;
            this.textBoxInserKey0.Name = "textBoxInserKey0";
            this.textBoxInserKey0.Size = new System.Drawing.Size(426, 22);
            this.textBoxInserKey0.TabIndex = 36;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(12, 26);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(98, 14);
            this.label23.TabIndex = 35;
            this.label23.Text = "Key0 To Write";
            // 
            // chkPerformSelectOps
            // 
            this.chkPerformSelectOps.AutoSize = true;
            this.chkPerformSelectOps.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPerformSelectOps.Location = new System.Drawing.Point(12, 63);
            this.chkPerformSelectOps.Name = "chkPerformSelectOps";
            this.chkPerformSelectOps.Size = new System.Drawing.Size(152, 17);
            this.chkPerformSelectOps.TabIndex = 63;
            this.chkPerformSelectOps.Text = "Activate Select Rules";
            this.chkPerformSelectOps.UseVisualStyleBackColor = true;
            // 
            // chkPerformPostMatch
            // 
            this.chkPerformPostMatch.AutoSize = true;
            this.chkPerformPostMatch.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPerformPostMatch.Location = new System.Drawing.Point(179, 63);
            this.chkPerformPostMatch.Name = "chkPerformPostMatch";
            this.chkPerformPostMatch.Size = new System.Drawing.Size(212, 17);
            this.chkPerformPostMatch.TabIndex = 64;
            this.chkPerformPostMatch.Text = "Activate Post-Singulation Rules";
            this.chkPerformPostMatch.UseVisualStyleBackColor = true;
            // 
            // textBoxEPC
            // 
            this.textBoxEPC.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxEPC.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxEPC.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEPC.Location = new System.Drawing.Point(43, 21);
            this.textBoxEPC.MaxLength = 23;
            this.textBoxEPC.Name = "textBoxEPC";
            this.textBoxEPC.ReadOnly = true;
            this.textBoxEPC.Size = new System.Drawing.Size(426, 22);
            this.textBoxEPC.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(9, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 14);
            this.label7.TabIndex = 27;
            this.label7.Text = "EPC";
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // LABEL_TagAccessAccessPassword
            // 
            this.LABEL_TagAccessAccessPassword.AutoSize = true;
            this.LABEL_TagAccessAccessPassword.Location = new System.Drawing.Point(403, 64);
            this.LABEL_TagAccessAccessPassword.Name = "LABEL_TagAccessAccessPassword";
            this.LABEL_TagAccessAccessPassword.Size = new System.Drawing.Size(80, 12);
            this.LABEL_TagAccessAccessPassword.TabIndex = 65;
            this.LABEL_TagAccessAccessPassword.Text = "Accesss Pwd (h)";
            // 
            // TEXTBOX_TagAccessAccessPassword
            // 
            this.TEXTBOX_TagAccessAccessPassword.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TEXTBOX_TagAccessAccessPassword.Location = new System.Drawing.Point(489, 59);
            this.TEXTBOX_TagAccessAccessPassword.MaxLength = 8;
            this.TEXTBOX_TagAccessAccessPassword.Name = "TEXTBOX_TagAccessAccessPassword";
            this.TEXTBOX_TagAccessAccessPassword.Size = new System.Drawing.Size(68, 22);
            this.TEXTBOX_TagAccessAccessPassword.TabIndex = 66;
            this.TEXTBOX_TagAccessAccessPassword.Text = "0";
            this.TEXTBOX_TagAccessAccessPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValidateHexInput);
            this.TEXTBOX_TagAccessAccessPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ValidateHexInput);
            // 
            // NXPAuthentication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 600);
            this.Controls.Add(this.TEXTBOX_TagAccessAccessPassword);
            this.Controls.Add(this.LABEL_TagAccessAccessPassword);
            this.Controls.Add(this.chkPerformPostMatch);
            this.Controls.Add(this.chkPerformSelectOps);
            this.Controls.Add(this.textBoxEPC);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NXPAuthentication";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NXP Authentication";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxVerificationKey1;
        private System.Windows.Forms.Button btnTAM2Authenticate;
        private System.Windows.Forms.TextBox textBoxEPC;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxInserKey0;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBoxInserKey1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button btnDefaultKey1;
        private System.Windows.Forms.Button btnInsertKey;
        private System.Windows.Forms.Button btnGetKey;
        private System.Windows.Forms.Button btnActivateKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxMemBank;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxBlockCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxOffset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelDecryptedChallenge;
        private System.Windows.Forms.Label labelTagMemoryData;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkPerformSelectOps;
        private System.Windows.Forms.CheckBox chkPerformPostMatch;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnTAM1Authenticate;
        private System.Windows.Forms.Button btnDefaultKey0;
        private System.Windows.Forms.TextBox textBoxVerificationKey0;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelRandomChallenge;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label LABEL_TagAccessAccessPassword;
        private System.Windows.Forms.TextBox TEXTBOX_TagAccessAccessPassword;

    }
}
