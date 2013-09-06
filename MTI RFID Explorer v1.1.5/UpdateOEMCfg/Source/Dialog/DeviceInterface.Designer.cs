namespace UpdateOEMCfgTool
{
    partial class DeviceInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceInterface));
            this.btn_Update = new System.Windows.Forms.Button();
            this.rBtn_UART = new System.Windows.Forms.RadioButton();
            this.rBtn_USB = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btn_Update
            // 
            this.btn_Update.Location = new System.Drawing.Point(173, 9);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(75, 23);
            this.btn_Update.TabIndex = 7;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // rBtn_UART
            // 
            this.rBtn_UART.AutoSize = true;
            this.rBtn_UART.Location = new System.Drawing.Point(88, 12);
            this.rBtn_UART.Name = "rBtn_UART";
            this.rBtn_UART.Size = new System.Drawing.Size(54, 16);
            this.rBtn_UART.TabIndex = 6;
            this.rBtn_UART.TabStop = true;
            this.rBtn_UART.Text = "UART";
            this.rBtn_UART.UseVisualStyleBackColor = true;
            // 
            // rBtn_USB
            // 
            this.rBtn_USB.AutoSize = true;
            this.rBtn_USB.Location = new System.Drawing.Point(21, 12);
            this.rBtn_USB.Name = "rBtn_USB";
            this.rBtn_USB.Size = new System.Drawing.Size(45, 16);
            this.rBtn_USB.TabIndex = 5;
            this.rBtn_USB.TabStop = true;
            this.rBtn_USB.Text = "USB";
            this.rBtn_USB.UseVisualStyleBackColor = true;
            // 
            // DeviceInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 41);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.rBtn_UART);
            this.Controls.Add(this.rBtn_USB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeviceInterface";
            this.Text = "Device Interface";
            this.Load += new System.EventHandler(this.DeviceInterface_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.RadioButton rBtn_UART;
        private System.Windows.Forms.RadioButton rBtn_USB;
    }
}