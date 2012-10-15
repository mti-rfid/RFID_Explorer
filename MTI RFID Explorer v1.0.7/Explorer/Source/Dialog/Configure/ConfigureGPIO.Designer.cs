namespace RFID_Explorer
{
	partial class ConfigureGPIO
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
            this.view = new System.Windows.Forms.DataGridView();
            this.errorTextBox = new System.Windows.Forms.TextBox();
            this.viewTitleLabel = new System.Windows.Forms.Label();
            this.readAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            this.SuspendLayout();
            // 
            // view
            // 
            this.view.AllowUserToAddRows = false;
            this.view.AllowUserToDeleteRows = false;
            this.view.Location = new System.Drawing.Point(37, 32);
            this.view.MultiSelect = false;
            this.view.Name = "view";
            this.view.RowHeadersVisible = false;
            this.view.RowTemplate.Height = 24;
            this.view.ShowEditingIcon = false;
            this.view.Size = new System.Drawing.Size(477, 201);
            this.view.TabIndex = 1;
            this.view.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.view_CellFormatting);
            this.view.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.view_RowStateChanged);
            this.view.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.view_CellContentClick);
            // 
            // errorTextBox
            // 
            this.errorTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.errorTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorTextBox.Location = new System.Drawing.Point(37, 233);
            this.errorTextBox.Name = "errorTextBox";
            this.errorTextBox.Size = new System.Drawing.Size(477, 15);
            this.errorTextBox.TabIndex = 2;
            this.errorTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // viewTitleLabel
            // 
            this.viewTitleLabel.AutoSize = true;
            this.viewTitleLabel.ForeColor = System.Drawing.Color.Blue;
            this.viewTitleLabel.Location = new System.Drawing.Point(4, 4);
            this.viewTitleLabel.Name = "viewTitleLabel";
            this.viewTitleLabel.Size = new System.Drawing.Size(118, 12);
            this.viewTitleLabel.TabIndex = 3;
            this.viewTitleLabel.Text = "GPIO Pin Configuration";
            // 
            // readAll
            // 
            this.readAll.Location = new System.Drawing.Point(230, 282);
            this.readAll.Name = "readAll";
            this.readAll.Size = new System.Drawing.Size(91, 23);
            this.readAll.TabIndex = 4;
            this.readAll.Text = "Read All";
            this.readAll.UseVisualStyleBackColor = true;
            this.readAll.Click += new System.EventHandler(this.readAll_Click);
            // 
            // ConfigureGPIO
            // 
            this.Controls.Add(this.readAll);
            this.Controls.Add(this.viewTitleLabel);
            this.Controls.Add(this.view);
            this.Controls.Add(this.errorTextBox);
            this.Name = "ConfigureGPIO";
            this.Size = new System.Drawing.Size(550, 313);
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label viewTitleLabel;
		private System.Windows.Forms.DataGridView view;
        private System.Windows.Forms.TextBox errorTextBox;
        private System.Windows.Forms.Button readAll;

	}
}
