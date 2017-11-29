namespace UpdateFWTool
{
    partial class SetComport
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
            this.btn_Set = new System.Windows.Forms.Button();
            this.labelPort = new System.Windows.Forms.Label();
            this.numComNum = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numComNum)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Set
            // 
            this.btn_Set.Location = new System.Drawing.Point(172, 13);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(63, 22);
            this.btn_Set.TabIndex = 10;
            this.btn_Set.Text = "Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(14, 17);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(36, 12);
            this.labelPort.TabIndex = 8;
            this.labelPort.Text = "Port #:";
            // 
            // numComNum
            // 
            this.numComNum.Location = new System.Drawing.Point(56, 13);
            this.numComNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numComNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numComNum.Name = "numComNum";
            this.numComNum.Size = new System.Drawing.Size(98, 22);
            this.numComNum.TabIndex = 11;
            this.numComNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SetComport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(247, 48);
            this.Controls.Add(this.numComNum);
            this.Controls.Add(this.btn_Set);
            this.Controls.Add(this.labelPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetComport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Set COM port";
            ((System.ComponentModel.ISupportInitialize)(this.numComNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Set;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.NumericUpDown numComNum;
    }
}