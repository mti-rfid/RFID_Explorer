namespace RFID_Explorer
{
    partial class ConfigureSelect_Edit
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
            this.configureSelect_Display = new RFID_Explorer.ConfigureSelect_Display( );
            this.SuspendLayout( );
            // 
            // configureSelect_Display
            // 
            this.configureSelect_Display.Location = new System.Drawing.Point( 1, 0 );
            this.configureSelect_Display.Mode = 0;
            this.configureSelect_Display.Name = "configureSelect_Display";
            this.configureSelect_Display.Size = new System.Drawing.Size( 550, 312 );
            this.configureSelect_Display.TabIndex = 0;
            // 
            // ConfigureSelect_Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 555, 316 );
            this.Controls.Add( this.configureSelect_Display );
            this.Name = "ConfigureSelect_Edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Select Criteria Settings";
            this.ResumeLayout( false );

        }

        #endregion

        private ConfigureSelect_Display configureSelect_Display;

    }
}