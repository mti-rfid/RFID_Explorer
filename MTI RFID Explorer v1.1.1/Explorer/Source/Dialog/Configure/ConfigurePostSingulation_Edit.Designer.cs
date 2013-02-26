namespace RFID_Explorer
{
    partial class ConfigurePostSingulation_Edit
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
            this.configurePostSingulation_Display = new RFID_Explorer.ConfigurePostSingulation_Display( );
            this.SuspendLayout( );
            // 
            // configurePostSingulation_Display
            // 
            this.configurePostSingulation_Display.Location = new System.Drawing.Point( 2, 1 );
            this.configurePostSingulation_Display.Mode = 0;
            this.configurePostSingulation_Display.Name = "configurePostSingulation_Display";
            this.configurePostSingulation_Display.Size = new System.Drawing.Size( 550, 312 );
            this.configurePostSingulation_Display.TabIndex = 0;
            // 
            // ConfigurePostSingulation_Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 561, 328 );
            this.Controls.Add( this.configurePostSingulation_Display );
            this.Name = "ConfigurePostSingulation_Edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConfigurePostSingulation_Edit";
            this.ResumeLayout( false );

        }

        #endregion

        private ConfigurePostSingulation_Display configurePostSingulation_Display;
    }
}