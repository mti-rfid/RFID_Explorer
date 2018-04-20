namespace RFID_Explorer
{
    partial class CommandTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandTest));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSendCommand = new System.Windows.Forms.TextBox();
            this.textBoxResponse = new System.Windows.Forms.TextBox();
            this.btnCmdSend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxPacketHeader = new System.Windows.Forms.TextBox();
            this.textBoxDeviceID = new System.Windows.Forms.TextBox();
            this.textBoxCommandID = new System.Windows.Forms.TextBox();
            this.textBoxParameters = new System.Windows.Forms.TextBox();
            this.lineShape7 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape6 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape5 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape4 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape3 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.LabelRemainingCount = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.LabelCommandCount = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label20 = new System.Windows.Forms.Label();
            this.TextBoxCommandScriptDes = new System.Windows.Forms.TextBox();
            this.ButtonExecute = new System.Windows.Forms.Button();
            this.ButtonLoadScript = new System.Windows.Forms.Button();
            this.TextBoxScriptResponse = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.TextBoxCommandScript = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.label24 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Command";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Response";
            // 
            // textBoxSendCommand
            // 
            this.textBoxSendCommand.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxSendCommand.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxSendCommand.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSendCommand.Location = new System.Drawing.Point(76, 71);
            this.textBoxSendCommand.Name = "textBoxSendCommand";
            this.textBoxSendCommand.Size = new System.Drawing.Size(451, 22);
            this.textBoxSendCommand.TabIndex = 5;
            this.textBoxSendCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSendCommand_KeyPress);
            // 
            // textBoxResponse
            // 
            this.textBoxResponse.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResponse.Location = new System.Drawing.Point(76, 111);
            this.textBoxResponse.Name = "textBoxResponse";
            this.textBoxResponse.Size = new System.Drawing.Size(451, 22);
            this.textBoxResponse.TabIndex = 6;
            this.textBoxResponse.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxResponse_KeyPress);
            // 
            // btnCmdSend
            // 
            this.btnCmdSend.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCmdSend.Location = new System.Drawing.Point(533, 69);
            this.btnCmdSend.Name = "btnCmdSend";
            this.btnCmdSend.Size = new System.Drawing.Size(75, 23);
            this.btnCmdSend.TabIndex = 0;
            this.btnCmdSend.Text = "Execute";
            this.btnCmdSend.UseVisualStyleBackColor = true;
            this.btnCmdSend.Click += new System.EventHandler(this.btnExecuteCommand_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(533, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Gen CMD";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnGenCommand_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(73, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Packet Header";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(188, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "DeviceID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(265, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "CommandID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(349, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 1;
            this.label6.Text = "Parameters";
            // 
            // textBoxPacketHeader
            // 
            this.textBoxPacketHeader.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxPacketHeader.Enabled = false;
            this.textBoxPacketHeader.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPacketHeader.Location = new System.Drawing.Point(76, 30);
            this.textBoxPacketHeader.Name = "textBoxPacketHeader";
            this.textBoxPacketHeader.Size = new System.Drawing.Size(95, 22);
            this.textBoxPacketHeader.TabIndex = 0;
            this.textBoxPacketHeader.Text = "43 49 54 4D";
            // 
            // textBoxDeviceID
            // 
            this.textBoxDeviceID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxDeviceID.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDeviceID.Location = new System.Drawing.Point(191, 30);
            this.textBoxDeviceID.MaxLength = 2;
            this.textBoxDeviceID.Name = "textBoxDeviceID";
            this.textBoxDeviceID.Size = new System.Drawing.Size(45, 22);
            this.textBoxDeviceID.TabIndex = 2;
            this.textBoxDeviceID.Text = "FF";
            this.textBoxDeviceID.Leave += new System.EventHandler(this.textBoxDeviceID_Leave);
            this.textBoxDeviceID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxDeviceID_KeyPress);
            // 
            // textBoxCommandID
            // 
            this.textBoxCommandID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxCommandID.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCommandID.Location = new System.Drawing.Point(268, 30);
            this.textBoxCommandID.MaxLength = 2;
            this.textBoxCommandID.Name = "textBoxCommandID";
            this.textBoxCommandID.Size = new System.Drawing.Size(45, 22);
            this.textBoxCommandID.TabIndex = 3;
            this.textBoxCommandID.Text = "00";
            this.textBoxCommandID.Leave += new System.EventHandler(this.textBoxCommandID_Leave);
            this.textBoxCommandID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCommandID_KeyPress);
            // 
            // textBoxParameters
            // 
            this.textBoxParameters.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxParameters.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxParameters.Location = new System.Drawing.Point(352, 30);
            this.textBoxParameters.MaxLength = 23;
            this.textBoxParameters.Name = "textBoxParameters";
            this.textBoxParameters.Size = new System.Drawing.Size(175, 22);
            this.textBoxParameters.TabIndex = 4;
            this.textBoxParameters.Text = "00 00 00 00 00 00 00 00";
            this.textBoxParameters.Leave += new System.EventHandler(this.textBoxParameters_Leave);
            this.textBoxParameters.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxParameters_KeyPress);
            // 
            // lineShape7
            // 
            this.lineShape7.Name = "lineShape7";
            this.lineShape7.X1 = 200;
            this.lineShape7.X2 = 200;
            this.lineShape7.Y1 = 134;
            this.lineShape7.Y2 = 154;
            // 
            // lineShape6
            // 
            this.lineShape6.Name = "lineShape6";
            this.lineShape6.X1 = 408;
            this.lineShape6.X2 = 408;
            this.lineShape6.Y1 = 135;
            this.lineShape6.Y2 = 155;
            // 
            // lineShape5
            // 
            this.lineShape5.Name = "lineShape5";
            this.lineShape5.X1 = 368;
            this.lineShape5.X2 = 368;
            this.lineShape5.Y1 = 134;
            this.lineShape5.Y2 = 154;
            // 
            // lineShape4
            // 
            this.lineShape4.Name = "lineShape4";
            this.lineShape4.X1 = 179;
            this.lineShape4.X2 = 179;
            this.lineShape4.Y1 = 134;
            this.lineShape4.Y2 = 154;
            // 
            // lineShape3
            // 
            this.lineShape3.Name = "lineShape3";
            this.lineShape3.X1 = 158;
            this.lineShape3.X2 = 158;
            this.lineShape3.Y1 = 134;
            this.lineShape3.Y2 = 154;
            // 
            // lineShape2
            // 
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 76;
            this.lineShape2.X2 = 76;
            this.lineShape2.Y1 = 134;
            this.lineShape2.Y2 = 154;
            // 
            // lineShape1
            // 
            this.lineShape1.BorderColor = System.Drawing.SystemColors.Desktop;
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 77;
            this.lineShape1.X2 = 408;
            this.lineShape1.Y1 = 144;
            this.lineShape1.Y2 = 144;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(77, 161);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 14);
            this.label7.TabIndex = 8;
            this.label7.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(159, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 14);
            this.label8.TabIndex = 9;
            this.label8.Text = "4";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(179, 161);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 14);
            this.label9.TabIndex = 10;
            this.label9.Text = "5";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(200, 161);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 14);
            this.label10.TabIndex = 11;
            this.label10.Text = "6";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(367, 161);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 14);
            this.label11.TabIndex = 12;
            this.label11.Text = "14";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(391, 161);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(21, 14);
            this.label12.TabIndex = 13;
            this.label12.Text = "15";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.LimeGreen;
            this.label13.Location = new System.Drawing.Point(87, 175);
            this.label13.MaximumSize = new System.Drawing.Size(50, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 28);
            this.label13.TabIndex = 14;
            this.label13.Text = "Header [4]";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.LimeGreen;
            this.label14.Location = new System.Drawing.Point(142, 175);
            this.label14.MaximumSize = new System.Drawing.Size(50, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 28);
            this.label14.TabIndex = 15;
            this.label14.Text = "Device ID";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.LimeGreen;
            this.label15.Location = new System.Drawing.Point(192, 175);
            this.label15.MaximumSize = new System.Drawing.Size(60, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 28);
            this.label15.TabIndex = 16;
            this.label15.Text = "Command ID";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.LimeGreen;
            this.label16.Location = new System.Drawing.Point(256, 175);
            this.label16.MaximumSize = new System.Drawing.Size(100, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 28);
            this.label16.TabIndex = 17;
            this.label16.Text = "ReturnedData [8]";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.LimeGreen;
            this.label17.Location = new System.Drawing.Point(362, 175);
            this.label17.MaximumSize = new System.Drawing.Size(70, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 28);
            this.label17.TabIndex = 18;
            this.label17.Text = "Checksum [2]";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Red;
            this.labelMessage.Location = new System.Drawing.Point(73, 94);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 14);
            this.labelMessage.TabIndex = 19;
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.InitialImage = null;
            this.pictureBox.Location = new System.Drawing.Point(533, 109);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(24, 24);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 20;
            this.pictureBox.TabStop = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(625, 681);
            this.tabControl1.TabIndex = 21;
            this.tabControl1.Click += new System.EventHandler(this.TabControl1_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.pictureBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.labelMessage);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.textBoxSendCommand);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.textBoxResponse);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.btnCmdSend);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.textBoxPacketHeader);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBoxDeviceID);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.textBoxCommandID);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.textBoxParameters);
            this.tabPage1.Controls.Add(this.shapeContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(617, 655);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Single Test";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(3, 3);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape7,
            this.lineShape6,
            this.lineShape5,
            this.lineShape4,
            this.lineShape3,
            this.lineShape2,
            this.lineShape1});
            this.shapeContainer2.Size = new System.Drawing.Size(611, 649);
            this.shapeContainer2.TabIndex = 21;
            this.shapeContainer2.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.LabelRemainingCount);
            this.tabPage2.Controls.Add(this.label21);
            this.tabPage2.Controls.Add(this.LabelCommandCount);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.TextBoxCommandScriptDes);
            this.tabPage2.Controls.Add(this.ButtonExecute);
            this.tabPage2.Controls.Add(this.ButtonLoadScript);
            this.tabPage2.Controls.Add(this.TextBoxScriptResponse);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.TextBoxCommandScript);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(617, 655);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Script Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(439, 337);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(70, 14);
            this.label23.TabIndex = 26;
            this.label23.Text = "Remaining";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(542, 337);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(63, 14);
            this.label22.TabIndex = 25;
            this.label22.Text = "Commands";
            // 
            // LabelRemainingCount
            // 
            this.LabelRemainingCount.AutoSize = true;
            this.LabelRemainingCount.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRemainingCount.Location = new System.Drawing.Point(520, 337);
            this.LabelRemainingCount.Name = "LabelRemainingCount";
            this.LabelRemainingCount.Size = new System.Drawing.Size(14, 14);
            this.LabelRemainingCount.TabIndex = 24;
            this.LabelRemainingCount.Text = "0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(452, 115);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(63, 14);
            this.label21.TabIndex = 23;
            this.label21.Text = "Commands";
            // 
            // LabelCommandCount
            // 
            this.LabelCommandCount.AutoSize = true;
            this.LabelCommandCount.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCommandCount.Location = new System.Drawing.Point(432, 114);
            this.LabelCommandCount.Name = "LabelCommandCount";
            this.LabelCommandCount.Size = new System.Drawing.Size(14, 14);
            this.LabelCommandCount.TabIndex = 22;
            this.LabelCommandCount.Text = "0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(400, 334);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(17, 22);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(133, 14);
            this.label20.TabIndex = 7;
            this.label20.Text = "Script Description";
            // 
            // TextBoxCommandScriptDes
            // 
            this.TextBoxCommandScriptDes.Location = new System.Drawing.Point(19, 43);
            this.TextBoxCommandScriptDes.Multiline = true;
            this.TextBoxCommandScriptDes.Name = "TextBoxCommandScriptDes";
            this.TextBoxCommandScriptDes.ReadOnly = true;
            this.TextBoxCommandScriptDes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxCommandScriptDes.Size = new System.Drawing.Size(586, 62);
            this.TextBoxCommandScriptDes.TabIndex = 6;
            this.TextBoxCommandScriptDes.WordWrap = false;
            // 
            // ButtonExecute
            // 
            this.ButtonExecute.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonExecute.Location = new System.Drawing.Point(530, 111);
            this.ButtonExecute.Name = "ButtonExecute";
            this.ButtonExecute.Size = new System.Drawing.Size(75, 23);
            this.ButtonExecute.TabIndex = 5;
            this.ButtonExecute.Text = "Execute";
            this.ButtonExecute.UseVisualStyleBackColor = true;
            this.ButtonExecute.Click += new System.EventHandler(this.ButtonExecute_Click);
            // 
            // ButtonLoadScript
            // 
            this.ButtonLoadScript.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonLoadScript.Location = new System.Drawing.Point(505, 18);
            this.ButtonLoadScript.Name = "ButtonLoadScript";
            this.ButtonLoadScript.Size = new System.Drawing.Size(100, 23);
            this.ButtonLoadScript.TabIndex = 4;
            this.ButtonLoadScript.Text = "Load Script";
            this.ButtonLoadScript.UseVisualStyleBackColor = true;
            this.ButtonLoadScript.Click += new System.EventHandler(this.ButtonLoadScript_Click);
            // 
            // TextBoxScriptResponse
            // 
            this.TextBoxScriptResponse.Location = new System.Drawing.Point(19, 361);
            this.TextBoxScriptResponse.Multiline = true;
            this.TextBoxScriptResponse.Name = "TextBoxScriptResponse";
            this.TextBoxScriptResponse.ReadOnly = true;
            this.TextBoxScriptResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxScriptResponse.Size = new System.Drawing.Size(586, 275);
            this.TextBoxScriptResponse.TabIndex = 3;
            this.TextBoxScriptResponse.WordWrap = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(17, 337);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(63, 14);
            this.label19.TabIndex = 2;
            this.label19.Text = "Response";
            // 
            // TextBoxCommandScript
            // 
            this.TextBoxCommandScript.Location = new System.Drawing.Point(19, 137);
            this.TextBoxCommandScript.Multiline = true;
            this.TextBoxCommandScript.Name = "TextBoxCommandScript";
            this.TextBoxCommandScript.ReadOnly = true;
            this.TextBoxCommandScript.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxCommandScript.Size = new System.Drawing.Size(586, 191);
            this.TextBoxCommandScript.TabIndex = 1;
            this.TextBoxCommandScript.WordWrap = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(17, 115);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(105, 14);
            this.label18.TabIndex = 0;
            this.label18.Text = "Script Content";
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(384, 115);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(42, 14);
            this.label24.TabIndex = 27;
            this.label24.Text = "Total";
            // 
            // CommandTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 705);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommandTest";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Command Test";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSendCommand;
        private System.Windows.Forms.TextBox textBoxResponse;
        private System.Windows.Forms.Button btnCmdSend;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxPacketHeader;
        private System.Windows.Forms.TextBox textBoxDeviceID;
        private System.Windows.Forms.TextBox textBoxCommandID;
        private System.Windows.Forms.TextBox textBoxParameters;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape6;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape5;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape4;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape3;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox TextBoxCommandScript;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox TextBoxScriptResponse;
        private System.Windows.Forms.Button ButtonLoadScript;
        private System.Windows.Forms.Button ButtonExecute;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.TextBox TextBoxCommandScriptDes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label LabelRemainingCount;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label LabelCommandCount;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label24;
    }
}