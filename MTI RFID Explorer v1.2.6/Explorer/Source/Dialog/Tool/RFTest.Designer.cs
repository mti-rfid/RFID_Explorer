namespace RFID_Explorer
{
    partial class RFTest
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
            this.cmbBoxRegion = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBoxFreq = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbAntPort = new System.Windows.Forms.ComboBox();
            this.numPowerLevel = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoBtnSingleChannel = new System.Windows.Forms.RadioButton();
            this.rdoBtnMultiChannel = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.FixedTypeRdoBtn = new System.Windows.Forms.RadioButton();
            this.InternalTypeRdoBtn = new System.Windows.Forms.RadioButton();
            this.PulsingRdoBtn = new System.Windows.Forms.RadioButton();
            this.ContinuousRdoBtn = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.numPulseOffTime = new System.Windows.Forms.NumericUpDown();
            this.numPulseOnTime = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnRfOn = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnPulse = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnRF = new System.Windows.Forms.Button();
            this.btnRfOff = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.textBoxTemperature = new System.Windows.Forms.TextBox();
            this.TemperatureBTN = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.rdoBtnNonContinuous = new System.Windows.Forms.RadioButton();
            this.rdoBtnContinuous = new System.Windows.Forms.RadioButton();
            this.btnClear = new System.Windows.Forms.Button();
            this.ckboxErrorKeepRunning = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerLevel)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseOffTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseOnTime)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbBoxRegion
            // 
            this.cmbBoxRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxRegion.FormattingEnabled = true;
            this.cmbBoxRegion.Location = new System.Drawing.Point(71, 22);
            this.cmbBoxRegion.Name = "cmbBoxRegion";
            this.cmbBoxRegion.Size = new System.Drawing.Size(118, 20);
            this.cmbBoxRegion.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Region";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Frequency";
            // 
            // cmbBoxFreq
            // 
            this.cmbBoxFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxFreq.FormattingEnabled = true;
            this.cmbBoxFreq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbBoxFreq.Location = new System.Drawing.Point(71, 59);
            this.cmbBoxFreq.Name = "cmbBoxFreq";
            this.cmbBoxFreq.Size = new System.Drawing.Size(118, 20);
            this.cmbBoxFreq.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbAntPort);
            this.groupBox1.Controls.Add(this.numPowerLevel);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Location = new System.Drawing.Point(224, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 107);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Antenna";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "Power Level";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "Physical Port";
            // 
            // cmbAntPort
            // 
            this.cmbAntPort.FormattingEnabled = true;
            this.cmbAntPort.Location = new System.Drawing.Point(80, 23);
            this.cmbAntPort.Name = "cmbAntPort";
            this.cmbAntPort.Size = new System.Drawing.Size(112, 20);
            this.cmbAntPort.TabIndex = 15;
            // 
            // numPowerLevel
            // 
            this.numPowerLevel.Location = new System.Drawing.Point(80, 64);
            this.numPowerLevel.Maximum = new decimal(new int[] {
            330,
            0,
            0,
            0});
            this.numPowerLevel.Name = "numPowerLevel";
            this.numPowerLevel.Size = new System.Drawing.Size(112, 22);
            this.numPowerLevel.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(99, 92);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 12);
            this.label10.TabIndex = 13;
            this.label10.Text = "Tx Port  1/10 dBm";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoBtnSingleChannel);
            this.groupBox2.Controls.Add(this.rdoBtnMultiChannel);
            this.groupBox2.Location = new System.Drawing.Point(224, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 95);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Channel";
            // 
            // rdoBtnSingleChannel
            // 
            this.rdoBtnSingleChannel.AutoSize = true;
            this.rdoBtnSingleChannel.Checked = true;
            this.rdoBtnSingleChannel.Location = new System.Drawing.Point(34, 60);
            this.rdoBtnSingleChannel.Name = "rdoBtnSingleChannel";
            this.rdoBtnSingleChannel.Size = new System.Drawing.Size(94, 16);
            this.rdoBtnSingleChannel.TabIndex = 1;
            this.rdoBtnSingleChannel.TabStop = true;
            this.rdoBtnSingleChannel.Text = "Single Channel";
            this.rdoBtnSingleChannel.UseVisualStyleBackColor = true;
            this.rdoBtnSingleChannel.CheckedChanged += new System.EventHandler(this.SingleChannelRadioButton_CheckedChanged);
            // 
            // rdoBtnMultiChannel
            // 
            this.rdoBtnMultiChannel.AutoSize = true;
            this.rdoBtnMultiChannel.Location = new System.Drawing.Point(34, 23);
            this.rdoBtnMultiChannel.Name = "rdoBtnMultiChannel";
            this.rdoBtnMultiChannel.Size = new System.Drawing.Size(64, 16);
            this.rdoBtnMultiChannel.TabIndex = 0;
            this.rdoBtnMultiChannel.Text = "Hopping";
            this.rdoBtnMultiChannel.UseVisualStyleBackColor = true;
            this.rdoBtnMultiChannel.CheckedChanged += new System.EventHandler(this.MultiChannelRadioButton_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox9);
            this.groupBox3.Controls.Add(this.PulsingRdoBtn);
            this.groupBox3.Controls.Add(this.ContinuousRdoBtn);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.numPulseOffTime);
            this.groupBox3.Controls.Add(this.numPulseOnTime);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(11, 120);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(198, 196);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Modulation for ETSI";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.FixedTypeRdoBtn);
            this.groupBox9.Controls.Add(this.InternalTypeRdoBtn);
            this.groupBox9.Location = new System.Drawing.Point(13, 134);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(176, 52);
            this.groupBox9.TabIndex = 10;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Random Data Type";
            // 
            // FixedTypeRdoBtn
            // 
            this.FixedTypeRdoBtn.AutoSize = true;
            this.FixedTypeRdoBtn.Location = new System.Drawing.Point(79, 22);
            this.FixedTypeRdoBtn.Name = "FixedTypeRdoBtn";
            this.FixedTypeRdoBtn.Size = new System.Drawing.Size(86, 16);
            this.FixedTypeRdoBtn.TabIndex = 1;
            this.FixedTypeRdoBtn.Text = "Fixed 511-bit";
            this.FixedTypeRdoBtn.UseVisualStyleBackColor = true;
            // 
            // InternalTypeRdoBtn
            // 
            this.InternalTypeRdoBtn.AutoSize = true;
            this.InternalTypeRdoBtn.Checked = true;
            this.InternalTypeRdoBtn.Location = new System.Drawing.Point(13, 22);
            this.InternalTypeRdoBtn.Name = "InternalTypeRdoBtn";
            this.InternalTypeRdoBtn.Size = new System.Drawing.Size(59, 16);
            this.InternalTypeRdoBtn.TabIndex = 0;
            this.InternalTypeRdoBtn.TabStop = true;
            this.InternalTypeRdoBtn.Text = "Internal";
            this.InternalTypeRdoBtn.UseVisualStyleBackColor = true;
            // 
            // PulsingRdoBtn
            // 
            this.PulsingRdoBtn.AutoSize = true;
            this.PulsingRdoBtn.Checked = true;
            this.PulsingRdoBtn.Location = new System.Drawing.Point(107, 26);
            this.PulsingRdoBtn.Name = "PulsingRdoBtn";
            this.PulsingRdoBtn.Size = new System.Drawing.Size(57, 16);
            this.PulsingRdoBtn.TabIndex = 9;
            this.PulsingRdoBtn.TabStop = true;
            this.PulsingRdoBtn.Text = "Pulsing";
            this.PulsingRdoBtn.UseVisualStyleBackColor = true;
            this.PulsingRdoBtn.CheckedChanged += new System.EventHandler(this.PulsingRdoBtn_CheckedChanged);
            // 
            // ContinuousRdoBtn
            // 
            this.ContinuousRdoBtn.AutoSize = true;
            this.ContinuousRdoBtn.Location = new System.Drawing.Point(19, 26);
            this.ContinuousRdoBtn.Name = "ContinuousRdoBtn";
            this.ContinuousRdoBtn.Size = new System.Drawing.Size(77, 16);
            this.ContinuousRdoBtn.TabIndex = 8;
            this.ContinuousRdoBtn.Text = "Continuous";
            this.ContinuousRdoBtn.UseVisualStyleBackColor = true;
            this.ContinuousRdoBtn.CheckedChanged += new System.EventHandler(this.ContinuousRdoBtn_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(171, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "ms";
            // 
            // numPulseOffTime
            // 
            this.numPulseOffTime.Location = new System.Drawing.Point(71, 98);
            this.numPulseOffTime.Name = "numPulseOffTime";
            this.numPulseOffTime.Size = new System.Drawing.Size(96, 22);
            this.numPulseOffTime.TabIndex = 6;
            // 
            // numPulseOnTime
            // 
            this.numPulseOnTime.Location = new System.Drawing.Point(71, 57);
            this.numPulseOnTime.Name = "numPulseOnTime";
            this.numPulseOnTime.Size = new System.Drawing.Size(96, 22);
            this.numPulseOnTime.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(171, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Off Time";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "On Time";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cmbBoxRegion);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.cmbBoxFreq);
            this.groupBox4.Location = new System.Drawing.Point(11, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(198, 97);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Frequency";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(161, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "MHz";
            // 
            // btnRfOn
            // 
            this.btnRfOn.Location = new System.Drawing.Point(281, 366);
            this.btnRfOn.Name = "btnRfOn";
            this.btnRfOn.Size = new System.Drawing.Size(83, 23);
            this.btnRfOn.TabIndex = 17;
            this.btnRfOn.Text = "RF On";
            this.btnRfOn.UseVisualStyleBackColor = true;
            this.btnRfOn.Visible = false;
            this.btnRfOn.Click += new System.EventHandler(this.RfOnButton_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(118, 21);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(83, 23);
            this.btnInventory.TabIndex = 18;
            this.btnInventory.Text = "Inventory On";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.InventoryButton_Click);
            // 
            // btnPulse
            // 
            this.btnPulse.Location = new System.Drawing.Point(224, 22);
            this.btnPulse.Name = "btnPulse";
            this.btnPulse.Size = new System.Drawing.Size(100, 23);
            this.btnPulse.TabIndex = 19;
            this.btnPulse.Text = "Modulation On";
            this.btnPulse.UseVisualStyleBackColor = true;
            this.btnPulse.Click += new System.EventHandler(this.PulseButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnRF);
            this.groupBox5.Controls.Add(this.btnInventory);
            this.groupBox5.Controls.Add(this.btnPulse);
            this.groupBox5.Location = new System.Drawing.Point(12, 395);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(338, 58);
            this.groupBox5.TabIndex = 21;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Function";
            // 
            // btnRF
            // 
            this.btnRF.Location = new System.Drawing.Point(11, 22);
            this.btnRF.Name = "btnRF";
            this.btnRF.Size = new System.Drawing.Size(83, 23);
            this.btnRF.TabIndex = 17;
            this.btnRF.Text = "RF On";
            this.btnRF.UseVisualStyleBackColor = true;
            this.btnRF.Click += new System.EventHandler(this.btnRF_Click);
            // 
            // btnRfOff
            // 
            this.btnRfOff.Location = new System.Drawing.Point(367, 366);
            this.btnRfOff.Name = "btnRfOff";
            this.btnRfOff.Size = new System.Drawing.Size(83, 23);
            this.btnRfOff.TabIndex = 20;
            this.btnRfOff.Text = "RF Off";
            this.btnRfOff.UseVisualStyleBackColor = true;
            this.btnRfOff.Visible = false;
            this.btnRfOff.Click += new System.EventHandler(this.btnRfOff_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.groupBox4);
            this.groupBox6.Controls.Add(this.groupBox1);
            this.groupBox6.Controls.Add(this.groupBox2);
            this.groupBox6.Controls.Add(this.groupBox3);
            this.groupBox6.Location = new System.Drawing.Point(12, 7);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(441, 353);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Configuration";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.textBoxTemperature);
            this.groupBox8.Controls.Add(this.TemperatureBTN);
            this.groupBox8.Location = new System.Drawing.Point(224, 294);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(203, 52);
            this.groupBox8.TabIndex = 18;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Temperature";
            // 
            // textBoxTemperature
            // 
            this.textBoxTemperature.Location = new System.Drawing.Point(19, 21);
            this.textBoxTemperature.Name = "textBoxTemperature";
            this.textBoxTemperature.Size = new System.Drawing.Size(90, 22);
            this.textBoxTemperature.TabIndex = 1;
            // 
            // TemperatureBTN
            // 
            this.TemperatureBTN.Location = new System.Drawing.Point(115, 19);
            this.TemperatureBTN.Name = "TemperatureBTN";
            this.TemperatureBTN.Size = new System.Drawing.Size(77, 23);
            this.TemperatureBTN.TabIndex = 0;
            this.TemperatureBTN.Text = "Get";
            this.TemperatureBTN.UseVisualStyleBackColor = true;
            this.TemperatureBTN.Click += new System.EventHandler(this.TemperatureBTN_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.rdoBtnNonContinuous);
            this.groupBox7.Controls.Add(this.rdoBtnContinuous);
            this.groupBox7.Location = new System.Drawing.Point(224, 233);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(203, 52);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Operation Mode";
            // 
            // rdoBtnNonContinuous
            // 
            this.rdoBtnNonContinuous.AutoSize = true;
            this.rdoBtnNonContinuous.Location = new System.Drawing.Point(96, 21);
            this.rdoBtnNonContinuous.Name = "rdoBtnNonContinuous";
            this.rdoBtnNonContinuous.Size = new System.Drawing.Size(101, 16);
            this.rdoBtnNonContinuous.TabIndex = 1;
            this.rdoBtnNonContinuous.TabStop = true;
            this.rdoBtnNonContinuous.Text = "Non-Continuous";
            this.rdoBtnNonContinuous.UseVisualStyleBackColor = true;
            // 
            // rdoBtnContinuous
            // 
            this.rdoBtnContinuous.AutoSize = true;
            this.rdoBtnContinuous.Checked = true;
            this.rdoBtnContinuous.Location = new System.Drawing.Point(8, 21);
            this.rdoBtnContinuous.Name = "rdoBtnContinuous";
            this.rdoBtnContinuous.Size = new System.Drawing.Size(77, 16);
            this.rdoBtnContinuous.TabIndex = 0;
            this.rdoBtnContinuous.TabStop = true;
            this.rdoBtnContinuous.Text = "Continuous";
            this.rdoBtnContinuous.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(356, 416);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(83, 23);
            this.btnClear.TabIndex = 20;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ckboxErrorKeepRunning
            // 
            this.ckboxErrorKeepRunning.AutoSize = true;
            this.ckboxErrorKeepRunning.Location = new System.Drawing.Point(12, 366);
            this.ckboxErrorKeepRunning.Name = "ckboxErrorKeepRunning";
            this.ckboxErrorKeepRunning.Size = new System.Drawing.Size(191, 16);
            this.ckboxErrorKeepRunning.TabIndex = 23;
            this.ckboxErrorKeepRunning.Text = "Keep inventory after received error.";
            this.ckboxErrorKeepRunning.UseVisualStyleBackColor = true;
            this.ckboxErrorKeepRunning.CheckedChanged += new System.EventHandler(this.ckboxErrorKeepRunning_CheckedChanged);
            // 
            // RFTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(462, 459);
            this.Controls.Add(this.btnRfOff);
            this.Controls.Add(this.ckboxErrorKeepRunning);
            this.Controls.Add(this.btnRfOn);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RFTest";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "RF Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RFTest_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPowerLevel)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseOffTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseOnTime)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbBoxRegion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBoxFreq;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdoBtnSingleChannel;
        private System.Windows.Forms.RadioButton rdoBtnMultiChannel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnRfOn;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnPulse;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown numPulseOffTime;
        private System.Windows.Forms.NumericUpDown numPulseOnTime;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.NumericUpDown numPowerLevel;
        private System.Windows.Forms.ComboBox cmbAntPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRfOff;
        private System.Windows.Forms.CheckBox ckboxErrorKeepRunning;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton rdoBtnNonContinuous;
        private System.Windows.Forms.RadioButton rdoBtnContinuous;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button TemperatureBTN;
        private System.Windows.Forms.TextBox textBoxTemperature;
        private System.Windows.Forms.Button btnRF;
        private System.Windows.Forms.RadioButton PulsingRdoBtn;
        private System.Windows.Forms.RadioButton ContinuousRdoBtn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.RadioButton InternalTypeRdoBtn;
        private System.Windows.Forms.RadioButton FixedTypeRdoBtn;
        //Add by Wayne for add Tx random data feature in RFTest page, 2014-09-10
        public static byte RandomType;
        public static byte ControlType;
        //End by Wayne for add Tx random data feature in RFTest page, 2014-09-10
    }
}