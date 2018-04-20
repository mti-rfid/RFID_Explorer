namespace RFID_Explorer
{
	partial class OptionsGeneralControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.generalOptionsPanel = new System.Windows.Forms.Panel();
            this.defaultViewLabel = new System.Windows.Forms.Label();
            this.defaultViewComboBox = new System.Windows.Forms.ComboBox();
            this.confirmExitCheckBox = new System.Windows.Forms.CheckBox();
            //this.automaticallyIndexCheckBox = new System.Windows.Forms.CheckBox(); //Mod by FJ for hiding Summary View Function, 2018-04-11
            this.maximizeCheckBox = new System.Windows.Forms.CheckBox();
            this.generalOptionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalOptionsPanel
            // 
            this.generalOptionsPanel.Controls.Add(this.defaultViewLabel);
            this.generalOptionsPanel.Controls.Add(this.defaultViewComboBox);
            this.generalOptionsPanel.Controls.Add(this.confirmExitCheckBox);
            //this.generalOptionsPanel.Controls.Add(this.automaticallyIndexCheckBox); //Mod by FJ for hiding Summary View Function, 2018-04-11
            this.generalOptionsPanel.Controls.Add(this.maximizeCheckBox);
            this.generalOptionsPanel.Location = new System.Drawing.Point(41, 18);
            this.generalOptionsPanel.Name = "generalOptionsPanel";
            this.generalOptionsPanel.Size = new System.Drawing.Size(468, 253);
            this.generalOptionsPanel.TabIndex = 1;
            // 
            // defaultViewLabel
            // 
            this.defaultViewLabel.AutoSize = true;
            this.defaultViewLabel.Location = new System.Drawing.Point(38, 161);
            this.defaultViewLabel.Name = "defaultViewLabel";
            this.defaultViewLabel.Size = new System.Drawing.Size(64, 12);
            this.defaultViewLabel.TabIndex = 0;
            this.defaultViewLabel.Text = "Default view";
            // 
            // defaultViewComboBox
            // 
            this.defaultViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defaultViewComboBox.FormattingEnabled = true;
            this.defaultViewComboBox.Location = new System.Drawing.Point(150, 158);
            this.defaultViewComboBox.Name = "defaultViewComboBox";
            this.defaultViewComboBox.Size = new System.Drawing.Size(150, 20);
            this.defaultViewComboBox.TabIndex = 1;
            this.defaultViewComboBox.SelectedIndexChanged += new System.EventHandler(this.defaultViewComboBox_SelectedIndexChanged);
            // 
            // confirmExitCheckBox
            // 
            this.confirmExitCheckBox.AutoSize = true;
            this.confirmExitCheckBox.Location = new System.Drawing.Point(40, 46);
            this.confirmExitCheckBox.Name = "confirmExitCheckBox";
            this.confirmExitCheckBox.Size = new System.Drawing.Size(190, 16);
            this.confirmExitCheckBox.TabIndex = 1;
            this.confirmExitCheckBox.Text = "Confirm before closing application.";
            this.confirmExitCheckBox.UseVisualStyleBackColor = true;
            this.confirmExitCheckBox.CheckedChanged += new System.EventHandler(this.confirmExitCheckBox_CheckedChanged);
            // 
            // automaticallyIndexCheckBox
            // 
            //Mod by FJ for hiding Summary View Function, 2018-04-11
            //this.automaticallyIndexCheckBox.AutoSize = true;
            //this.automaticallyIndexCheckBox.Checked = true;
            //this.automaticallyIndexCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.automaticallyIndexCheckBox.Location = new System.Drawing.Point(40, 69);
            //this.automaticallyIndexCheckBox.Name = "automaticallyIndexCheckBox";
            //this.automaticallyIndexCheckBox.Size = new System.Drawing.Size(245, 16);
            //this.automaticallyIndexCheckBox.TabIndex = 2;
            //this.automaticallyIndexCheckBox.Text = "Automatically perform post-capture processing.";
            //this.automaticallyIndexCheckBox.UseVisualStyleBackColor = true;
            //this.automaticallyIndexCheckBox.CheckedChanged += new System.EventHandler(this.automaticallyIndexCheckBox_CheckedChanged);
            //End by FJ for hiding Summary View Function, 2018-04-11
            // 
            // maximizeCheckBox
            // 
            this.maximizeCheckBox.AutoSize = true;
            this.maximizeCheckBox.Location = new System.Drawing.Point(40, 23);
            this.maximizeCheckBox.Name = "maximizeCheckBox";
            this.maximizeCheckBox.Size = new System.Drawing.Size(188, 16);
            this.maximizeCheckBox.TabIndex = 0;
            this.maximizeCheckBox.Text = "Maximize main window on startup.";
            this.maximizeCheckBox.UseVisualStyleBackColor = true;
            this.maximizeCheckBox.CheckedChanged += new System.EventHandler(this.maximizeCheckBox_CheckedChanged);
            // 
            // OptionsGeneralControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generalOptionsPanel);
            this.Name = "OptionsGeneralControl";
            this.Size = new System.Drawing.Size(550, 289);
            this.generalOptionsPanel.ResumeLayout(false);
            this.generalOptionsPanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel generalOptionsPanel;
        private System.Windows.Forms.Label defaultViewLabel;
		private System.Windows.Forms.ComboBox defaultViewComboBox;
		private System.Windows.Forms.CheckBox confirmExitCheckBox;
        //private System.Windows.Forms.CheckBox automaticallyIndexCheckBox; //Mod by FJ for hiding Summary View Function, 2018-04-11
		private System.Windows.Forms.CheckBox maximizeCheckBox;
	}
}
