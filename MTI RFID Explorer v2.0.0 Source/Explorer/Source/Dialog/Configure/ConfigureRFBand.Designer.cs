namespace RFID_Explorer
{
	partial class ConfigureRFBandForm
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
            this.importButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.rwLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            this.SuspendLayout();
            // 
            // view
            // 
            this.view.AllowUserToAddRows = false;
            this.view.AllowUserToDeleteRows = false;
            this.view.BackgroundColor = System.Drawing.Color.White;
            this.view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.view.Location = new System.Drawing.Point(11, 23);
            this.view.Name = "view";
            this.view.RowTemplate.Height = 24;
            this.view.Size = new System.Drawing.Size(528, 229);
            this.view.TabIndex = 1;
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(323, 259);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(105, 20);
            this.importButton.TabIndex = 6;
            this.importButton.Text = "Import from Excel";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(434, 259);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(105, 20);
            this.exportButton.TabIndex = 5;
            this.exportButton.Text = "Export to Excel";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // rwLabel
            // 
            this.rwLabel.AutoSize = true;
            this.rwLabel.ForeColor = System.Drawing.Color.Blue;
            this.rwLabel.Location = new System.Drawing.Point(4, 4);
            this.rwLabel.Name = "rwLabel";
            this.rwLabel.Size = new System.Drawing.Size(115, 12);
            this.rwLabel.TabIndex = 0;
            this.rwLabel.Text = "RF Channel Definitions";
            // 
            // ConfigureRFBandForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.view);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.rwLabel);
            this.Name = "ConfigureRFBandForm";
            this.Size = new System.Drawing.Size(550, 289);
            this.Load += new System.EventHandler(this.ConfigureRFBandForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label rwLabel;
        private System.Windows.Forms.DataGridView view;

	}
}
