namespace RFID_Explorer
{
	partial class RFChannelEditForm
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
            this.cancelButton = new System.Windows.Forms.Button( );
            this.okButton = new System.Windows.Forms.Button( );
            this.state = new System.Windows.Forms.ComboBox( );
            this.channelSlotNumber = new System.Windows.Forms.Label( );
            this.label5 = new System.Windows.Forms.Label( );
            this.label6 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.frequency = new System.Windows.Forms.TextBox( );
            this.label4 = new System.Windows.Forms.Label( );
            this.multiplyRatio = new System.Windows.Forms.NumericUpDown( );
            this.divideRatio = new System.Windows.Forms.NumericUpDown( );
            this.minimumDAC = new System.Windows.Forms.NumericUpDown( );
            this.maximumDAC = new System.Windows.Forms.NumericUpDown( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label7 = new System.Windows.Forms.Label( );
            this.affinityBand = new System.Windows.Forms.NumericUpDown( );
            this.guardBand = new System.Windows.Forms.NumericUpDown( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.multiplyRatio ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.divideRatio ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.minimumDAC ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.maximumDAC ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.affinityBand ) ).BeginInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.guardBand ) ).BeginInit( );
            this.SuspendLayout( );
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 287, 238 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point( 186, 238 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 12;
            this.okButton.Text = "Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler( this.okButton_Click );
            // 
            // state
            // 
            this.state.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.state.FormattingEnabled = true;
            this.state.Location = new System.Drawing.Point( 91, 49 );
            this.state.Name = "state";
            this.state.Size = new System.Drawing.Size( 120, 21 );
            this.state.TabIndex = 1;
            // 
            // channelSlotNumber
            // 
            this.channelSlotNumber.AutoSize = true;
            this.channelSlotNumber.Location = new System.Drawing.Point( 91, 31 );
            this.channelSlotNumber.Name = "channelSlotNumber";
            this.channelSlotNumber.Size = new System.Drawing.Size( 124, 13 );
            this.channelSlotNumber.TabIndex = 0;
            this.channelSlotNumber.Text = "RF Channel Slot Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point( 337, 31 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 101, 13 );
            this.label5.TabIndex = 10;
            this.label5.Text = "Minimum DAC Band";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point( 337, 121 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 107, 13 );
            this.label6.TabIndex = 8;
            this.label6.Text = "Maximum DAC Band ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 91, 121 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 70, 13 );
            this.label2.TabIndex = 4;
            this.label2.Text = "Multiply Ratio";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 91, 166 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 65, 13 );
            this.label1.TabIndex = 6;
            this.label1.Text = "Divide Ratio";
            // 
            // frequency
            // 
            this.frequency.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frequency.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.frequency.Location = new System.Drawing.Point( 91, 95 );
            this.frequency.MaxLength = 10;
            this.frequency.Name = "frequency";
            this.frequency.ReadOnly = true;
            this.frequency.Size = new System.Drawing.Size( 120, 20 );
            this.frequency.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 91, 76 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 57, 13 );
            this.label4.TabIndex = 2;
            this.label4.Text = "Frequency";
            // 
            // multiplyRatio
            // 
            this.multiplyRatio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.multiplyRatio.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.multiplyRatio.Location = new System.Drawing.Point( 91, 140 );
            this.multiplyRatio.Maximum = new decimal( new int[ ] {
            65535,
            0,
            0,
            0} );
            this.multiplyRatio.Minimum = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.multiplyRatio.Name = "multiplyRatio";
            this.multiplyRatio.Size = new System.Drawing.Size( 120, 20 );
            this.multiplyRatio.TabIndex = 5;
            this.multiplyRatio.Value = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.multiplyRatio.ValueChanged += new System.EventHandler( this.mulitplyFactor_ValueChanged );
            // 
            // divideRatio
            // 
            this.divideRatio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divideRatio.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.divideRatio.Location = new System.Drawing.Point( 91, 185 );
            this.divideRatio.Maximum = new decimal( new int[ ] {
            255,
            0,
            0,
            0} );
            this.divideRatio.Minimum = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.divideRatio.Name = "divideRatio";
            this.divideRatio.Size = new System.Drawing.Size( 120, 20 );
            this.divideRatio.TabIndex = 7;
            this.divideRatio.Value = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.divideRatio.ValueChanged += new System.EventHandler( this.divideRatio_ValueChanged );
            // 
            // minimumDAC
            // 
            this.minimumDAC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.minimumDAC.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.minimumDAC.Location = new System.Drawing.Point( 337, 50 );
            this.minimumDAC.Maximum = new decimal( new int[ ] {
            7,
            0,
            0,
            0} );
            this.minimumDAC.Name = "minimumDAC";
            this.minimumDAC.Size = new System.Drawing.Size( 120, 20 );
            this.minimumDAC.TabIndex = 11;
            this.minimumDAC.Value = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.minimumDAC.ValueChanged += new System.EventHandler( this.minimumDAC_ValueChanged );
            // 
            // maximumDAC
            // 
            this.maximumDAC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maximumDAC.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.maximumDAC.Location = new System.Drawing.Point( 337, 140 );
            this.maximumDAC.Maximum = new decimal( new int[ ] {
            7,
            0,
            0,
            0} );
            this.maximumDAC.Name = "maximumDAC";
            this.maximumDAC.Size = new System.Drawing.Size( 120, 20 );
            this.maximumDAC.TabIndex = 9;
            this.maximumDAC.Value = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.maximumDAC.ValueChanged += new System.EventHandler( this.maximumDACBand_ValueChanged );
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 337, 76 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 66, 13 );
            this.label3.TabIndex = 14;
            this.label3.Text = "Affinity Band";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point( 337, 166 );
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size( 101, 13 );
            this.label7.TabIndex = 15;
            this.label7.Text = "Guard Band ( MHz )";
            // 
            // affinityBand
            // 
            this.affinityBand.Location = new System.Drawing.Point( 337, 95 );
            this.affinityBand.Maximum = new decimal( new int[ ] {
            7,
            0,
            0,
            0} );
            this.affinityBand.Name = "affinityBand";
            this.affinityBand.Size = new System.Drawing.Size( 120, 20 );
            this.affinityBand.TabIndex = 16;
            this.affinityBand.Value = new decimal( new int[ ] {
            1,
            0,
            0,
            0} );
            this.affinityBand.ValueChanged += new System.EventHandler( this.affinityBand_ValueChanged );
            // 
            // guardBand
            // 
            this.guardBand.Location = new System.Drawing.Point( 337, 185 );
            this.guardBand.Maximum = new decimal( new int[ ] {
            255,
            0,
            0,
            0} );
            this.guardBand.Name = "guardBand";
            this.guardBand.Size = new System.Drawing.Size( 120, 20 );
            this.guardBand.TabIndex = 17;
            this.guardBand.ValueChanged += new System.EventHandler( this.guardBand_ValueChanged );
            // 
            // RFChannelEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 548, 275 );
            this.Controls.Add( this.guardBand );
            this.Controls.Add( this.affinityBand );
            this.Controls.Add( this.label7 );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label5 );
            this.Controls.Add( this.label6 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.maximumDAC );
            this.Controls.Add( this.minimumDAC );
            this.Controls.Add( this.divideRatio );
            this.Controls.Add( this.multiplyRatio );
            this.Controls.Add( this.frequency );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.state );
            this.Controls.Add( this.channelSlotNumber );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RFChannelEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RF Channel Settings";
            ( ( System.ComponentModel.ISupportInitialize ) ( this.multiplyRatio ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.divideRatio ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.minimumDAC ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.maximumDAC ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.affinityBand ) ).EndInit( );
            ( ( System.ComponentModel.ISupportInitialize ) ( this.guardBand ) ).EndInit( );
            this.ResumeLayout( false );
            this.PerformLayout( );

		}

		#endregion

		private System.Windows.Forms.Label channelSlotNumber;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox frequency;
		private System.Windows.Forms.NumericUpDown multiplyRatio;
        private System.Windows.Forms.NumericUpDown divideRatio;
        private System.Windows.Forms.NumericUpDown minimumDAC;
        private System.Windows.Forms.NumericUpDown maximumDAC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown affinityBand;
        private System.Windows.Forms.NumericUpDown guardBand;
        private System.Windows.Forms.ComboBox state;
		
	}
}