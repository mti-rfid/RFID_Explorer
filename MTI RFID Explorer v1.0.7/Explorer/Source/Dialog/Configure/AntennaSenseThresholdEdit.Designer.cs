namespace RFID_Explorer
{
    partial class AntennaSenseThresholdEdit
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.label1 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.activeThreshold = new System.Windows.Forms.TextBox( );
            this.newThreshold = new System.Windows.Forms.NumericUpDown( );
            this.okButton = new System.Windows.Forms.Button( );
            this.cancelButton = new System.Windows.Forms.Button( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.newThreshold ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 42, 37 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 160, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Active Global Threshold ( ohms )";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 42, 113 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 185, 13 );
            this.label2.TabIndex = 1;
            this.label2.Text = "New Global Sense Threshold ( ohms )";
            // 
            // activeThreshold
            // 
            this.activeThreshold.Location = new System.Drawing.Point( 45, 64 );
            this.activeThreshold.Name = "activeThreshold";
            this.activeThreshold.Size = new System.Drawing.Size( 182, 20 );
            this.activeThreshold.TabIndex = 2;
            // 
            // newThreshold
            // 
            this.newThreshold.Location = new System.Drawing.Point( 45, 138 );
            this.newThreshold.Name = "newThreshold";
            this.newThreshold.Size = new System.Drawing.Size( 182, 20 );
            this.newThreshold.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point( 45, 205 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point( 172, 205 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler( this.cancelButton_Click );
            // 
            // AntennaSenseThresholdEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 292, 247 );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.newThreshold );
            this.Controls.Add( this.activeThreshold );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Name = "AntennaSenseThresholdEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Antenna Sense Threshold";
            ( ( System.ComponentModel.ISupportInitialize ) ( this.newThreshold ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox activeThreshold;
        private System.Windows.Forms.NumericUpDown newThreshold;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}