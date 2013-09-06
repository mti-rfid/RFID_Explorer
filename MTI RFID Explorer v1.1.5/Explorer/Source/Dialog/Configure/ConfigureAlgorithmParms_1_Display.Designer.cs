namespace RFID_Explorer
{
    partial class ConfigureAlgorithmParms_1_Display
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.startQValue = new System.Windows.Forms.NumericUpDown();
            this.minQValue = new System.Windows.Forms.NumericUpDown();
            this.toggleTarget = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.maxQValue = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.retryCount = new System.Windows.Forms.NumericUpDown();
            this.thresholdMultiplier = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.startQValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minQValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxQValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.retryCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdMultiplier)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Q Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Min Q Value";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(289, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Toggle Target ( A<>B )";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(289, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Threshold Multiplier";
            // 
            // startQValue
            // 
            this.startQValue.Location = new System.Drawing.Point(97, 19);
            this.startQValue.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.startQValue.Name = "startQValue";
            this.startQValue.Size = new System.Drawing.Size(134, 20);
            this.startQValue.TabIndex = 8;
            this.startQValue.ValueChanged += new System.EventHandler(this.startQValue_ValueChanged);
            // 
            // minQValue
            // 
            this.minQValue.Location = new System.Drawing.Point(97, 57);
            this.minQValue.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.minQValue.Name = "minQValue";
            this.minQValue.Size = new System.Drawing.Size(134, 20);
            this.minQValue.TabIndex = 9;
            this.minQValue.ValueChanged += new System.EventHandler(this.minQValue_ValueChanged);
            // 
            // toggleTarget
            // 
            this.toggleTarget.FormattingEnabled = true;
            this.toggleTarget.Location = new System.Drawing.Point(410, 94);
            this.toggleTarget.Name = "toggleTarget";
            this.toggleTarget.Size = new System.Drawing.Size(134, 21);
            this.toggleTarget.TabIndex = 10;
            this.toggleTarget.SelectedIndexChanged += new System.EventHandler(this.toggleTarget_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Max Q value";
            // 
            // maxQValue
            // 
            this.maxQValue.Location = new System.Drawing.Point(97, 95);
            this.maxQValue.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.maxQValue.Name = "maxQValue";
            this.maxQValue.Size = new System.Drawing.Size(134, 20);
            this.maxQValue.TabIndex = 13;
            this.maxQValue.ValueChanged += new System.EventHandler(this.maxQValue_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(289, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Retry Count";
            // 
            // retryCount
            // 
            this.retryCount.Location = new System.Drawing.Point(410, 19);
            this.retryCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.retryCount.Name = "retryCount";
            this.retryCount.Size = new System.Drawing.Size(134, 20);
            this.retryCount.TabIndex = 15;
            this.retryCount.ValueChanged += new System.EventHandler(this.retryCount_ValueChanged);
            // 
            // thresholdMultiplier
            // 
            this.thresholdMultiplier.Location = new System.Drawing.Point(410, 57);
            this.thresholdMultiplier.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.thresholdMultiplier.Name = "thresholdMultiplier";
            this.thresholdMultiplier.Size = new System.Drawing.Size(134, 20);
            this.thresholdMultiplier.TabIndex = 16;
            this.thresholdMultiplier.ValueChanged += new System.EventHandler(this.maxQueryReps_ValueChanged);
            // 
            // ConfigureAlgorithmParms_1_Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.thresholdMultiplier);
            this.Controls.Add(this.retryCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.maxQValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.toggleTarget);
            this.Controls.Add(this.minQValue);
            this.Controls.Add(this.startQValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ConfigureAlgorithmParms_1_Display";
            this.Size = new System.Drawing.Size(560, 133);
            this.Load += new System.EventHandler(this.AlgorithmParms_1_Display_Load);
            ((System.ComponentModel.ISupportInitialize)(this.startQValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minQValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxQValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.retryCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdMultiplier)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown startQValue;
        private System.Windows.Forms.NumericUpDown minQValue;
        private System.Windows.Forms.ComboBox toggleTarget;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown maxQValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown retryCount;
        private System.Windows.Forms.NumericUpDown thresholdMultiplier;
    }
}
