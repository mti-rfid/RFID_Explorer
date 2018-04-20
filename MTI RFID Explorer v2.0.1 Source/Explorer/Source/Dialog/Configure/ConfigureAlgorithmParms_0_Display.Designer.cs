namespace RFID_Explorer
{
    partial class ConfigureAlgorithmParms_0_Display
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
            this.label1 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label4 = new System.Windows.Forms.Label( );
            this.qValue = new System.Windows.Forms.NumericUpDown( );
            this.retryCount = new System.Windows.Forms.NumericUpDown( );
            this.toggleTarget = new System.Windows.Forms.ComboBox( );
            this.repeatUntilNoTags = new System.Windows.Forms.ComboBox( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.qValue ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.retryCount ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 8, 44 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 45, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Q Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 8, 77 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 63, 13 );
            this.label2.TabIndex = 1;
            this.label2.Text = "Retry Count";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 289, 44 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 115, 13 );
            this.label3.TabIndex = 2;
            this.label3.Text = "Toggle Target ( A<>B )";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 289, 77 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 110, 13 );
            this.label4.TabIndex = 3;
            this.label4.Text = "Repeat Until No Tags";
            // 
            // qValue
            // 
            this.qValue.Location = new System.Drawing.Point( 97, 41 );
            this.qValue.Maximum = new decimal( new int[ ] {
            15,
            0,
            0,
            0} );
            this.qValue.Name = "qValue";
            this.qValue.Size = new System.Drawing.Size( 134, 20 );
            this.qValue.TabIndex = 8;
            this.qValue.ValueChanged += new System.EventHandler( this.qValue_ValueChanged );
            // 
            // retryCount
            // 
            this.retryCount.Location = new System.Drawing.Point( 97, 77 );
            this.retryCount.Maximum = new decimal( new int[ ] {
            255,
            0,
            0,
            0} );
            this.retryCount.Name = "retryCount";
            this.retryCount.Size = new System.Drawing.Size( 134, 20 );
            this.retryCount.TabIndex = 9;
            this.retryCount.ValueChanged += new System.EventHandler( this.retryCount_ValueChanged );
            // 
            // toggleTarget
            // 
            this.toggleTarget.FormattingEnabled = true;
            this.toggleTarget.Location = new System.Drawing.Point( 410, 41 );
            this.toggleTarget.Name = "toggleTarget";
            this.toggleTarget.Size = new System.Drawing.Size( 134, 21 );
            this.toggleTarget.TabIndex = 10;
            this.toggleTarget.SelectedIndexChanged += new System.EventHandler( this.toggleTarget_SelectedIndexChanged );
            // 
            // repeatUntilNoTags
            // 
            this.repeatUntilNoTags.FormattingEnabled = true;
            this.repeatUntilNoTags.Location = new System.Drawing.Point( 410, 77 );
            this.repeatUntilNoTags.Name = "repeatUntilNoTags";
            this.repeatUntilNoTags.Size = new System.Drawing.Size( 134, 21 );
            this.repeatUntilNoTags.TabIndex = 11;
            this.repeatUntilNoTags.SelectedIndexChanged += new System.EventHandler( this.repeatUntilNoTags_SelectedIndexChanged );
            // 
            // ConfigureAlgorithmParms_0_Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add( this.repeatUntilNoTags );
            this.Controls.Add( this.toggleTarget );
            this.Controls.Add( this.retryCount );
            this.Controls.Add( this.qValue );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Name = "ConfigureAlgorithmParms_0_Display";
            this.Size = new System.Drawing.Size( 560, 133 );
            this.Load += new System.EventHandler( this.AlgorithmParms_0_Display_Load );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.qValue ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.retryCount ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown qValue;
        private System.Windows.Forms.NumericUpDown retryCount;
        private System.Windows.Forms.ComboBox toggleTarget;
        private System.Windows.Forms.ComboBox repeatUntilNoTags;
    }
}
