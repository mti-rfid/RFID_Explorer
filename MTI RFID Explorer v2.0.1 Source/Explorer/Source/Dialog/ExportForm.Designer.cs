namespace RFID_Explorer
{
	partial class ExportForm
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
            this.ExportViewCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ExportTargetComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            //this.selectAllButton = new System.Windows.Forms.Button(); //Del by FJ for hiding Summary View Function, 2018-04-11
            //this.ClearAllButton = new System.Windows.Forms.Button(); //Del by FJ for hiding Summary View Function, 2018-04-11
            this.textPath = new System.Windows.Forms.TextBox();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDefaultPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExportViewCheckedListBox
            // 
            this.ExportViewCheckedListBox.CheckOnClick = true;
            this.ExportViewCheckedListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportViewCheckedListBox.FormattingEnabled = true;
            this.ExportViewCheckedListBox.Location = new System.Drawing.Point(38, 145);
            this.ExportViewCheckedListBox.Name = "ExportViewCheckedListBox";
            this.ExportViewCheckedListBox.Size = new System.Drawing.Size(339, 139);
            this.ExportViewCheckedListBox.TabIndex = 3;
            this.ExportViewCheckedListBox.ThreeDCheckBoxes = true;
            this.ExportViewCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ExportViewCheckedListBox_ItemCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select the export target";
            // 
            // ExportTargetComboBox
            // 
            this.ExportTargetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ExportTargetComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportTargetComboBox.FormattingEnabled = true;
            this.ExportTargetComboBox.Location = new System.Drawing.Point(38, 45);
            this.ExportTargetComboBox.Name = "ExportTargetComboBox";
            this.ExportTargetComboBox.Size = new System.Drawing.Size(339, 21);
            this.ExportTargetComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select the views to export.";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(405, 320);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(324, 320);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 21);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "Export";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // selectAllButton
            // 
            //Del by FJ for hiding Summary View Function, 2018-04-11
            //this.selectAllButton.Location = new System.Drawing.Point(78, 294);
            //this.selectAllButton.Name = "selectAllButton";
            //this.selectAllButton.Size = new System.Drawing.Size(75, 21);
            //this.selectAllButton.TabIndex = 6;
            //this.selectAllButton.Text = "Select &All";
            //this.selectAllButton.UseVisualStyleBackColor = true;
            //this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            //End by FJ for hiding Summary View Function, 2018-04-11
            // 
            // ClearAllButton
            //
            //Del by FJ for hiding Summary View Function, 2018-04-11
            //this.ClearAllButton.Location = new System.Drawing.Point(203, 294);
            //this.ClearAllButton.Name = "ClearAllButton";
            //this.ClearAllButton.Size = new System.Drawing.Size(75, 21);
            //this.ClearAllButton.TabIndex = 7;
            //this.ClearAllButton.Text = "&Clear All";
            //this.ClearAllButton.UseVisualStyleBackColor = true;
            //this.ClearAllButton.Click += new System.EventHandler(this.ClearAllButton_Click);
            //End by FJ for hiding Summary View Function, 2018-04-11
            // 
            // textPath
            // 
            this.textPath.Location = new System.Drawing.Point(38, 91);
            this.textPath.Name = "textPath";
            this.textPath.ReadOnly = true;
            this.textPath.Size = new System.Drawing.Size(339, 22);
            this.textPath.TabIndex = 8;
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(405, 76);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 22);
            this.btnBrowser.TabIndex = 9;
            this.btnBrowser.Text = "Browser";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Path";
            // 
            // btnDefaultPath
            // 
            this.btnDefaultPath.Location = new System.Drawing.Point(405, 104);
            this.btnDefaultPath.Name = "btnDefaultPath";
            this.btnDefaultPath.Size = new System.Drawing.Size(75, 22);
            this.btnDefaultPath.TabIndex = 11;
            this.btnDefaultPath.Text = "Default  Path";
            this.btnDefaultPath.UseVisualStyleBackColor = true;
            this.btnDefaultPath.Click += new System.EventHandler(this.btnDefaultPath_Click);
            // 
            // ExportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(495, 353);
            this.Controls.Add(this.btnDefaultPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.textPath);
            //this.Controls.Add(this.ClearAllButton); //Del by FJ for hiding Summary View Function, 2018-04-11
            //this.Controls.Add(this.selectAllButton); //Del by FJ for hiding Summary View Function, 2018-04-11
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ExportTargetComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExportViewCheckedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Captured Data";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.CheckedListBox ExportViewCheckedListBox;
		private System.Windows.Forms.Label label1;
		public  System.Windows.Forms.ComboBox ExportTargetComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
        //private System.Windows.Forms.Button selectAllButton; //Del by FJ for hiding Summary View Function, 2018-04-11
        //private System.Windows.Forms.Button ClearAllButton; //Del by FJ for hiding Summary View Function, 2018-04-11
        private System.Windows.Forms.TextBox textPath;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDefaultPath;
	}
}