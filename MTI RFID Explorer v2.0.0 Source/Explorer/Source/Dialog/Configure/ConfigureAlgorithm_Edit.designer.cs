namespace RFID_Explorer
{
    partial class ConfigureAlgorithm_Edit
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
            this.algorithmDisplay = new RFID_Explorer.ConfigureAlgorithm_Display( );
            this.SuspendLayout( );
            // 
            // algorithmDisplay
            // 
            this.algorithmDisplay.Location = new System.Drawing.Point( 1, 0 );
            this.algorithmDisplay.MasterEnabled = false;
            this.algorithmDisplay.Name = "algorithmDisplay";
            this.algorithmDisplay.Size = new System.Drawing.Size( 580, 345 );
            this.algorithmDisplay.TabIndex = 0;
            // 
            // Algorithm_Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 584, 347 );
            this.Controls.Add( this.algorithmDisplay );
            this.Name = "Algorithm_Edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Algorithm Settings";
            this.ResumeLayout( false );

        }

        #endregion

        private ConfigureAlgorithm_Display algorithmDisplay;
    }
}
