namespace RFID_Explorer
{
    partial class ConfigureAlgorithm_Display
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
            this.panelTitleLabel = new System.Windows.Forms.Label();
            this.commonParmsPanel = new System.Windows.Forms.Panel();
            this.tagGroupTarget = new System.Windows.Forms.ComboBox();
            this.tagGroupSession = new System.Windows.Forms.ComboBox();
            this.tagGroupSelected = new System.Windows.Forms.ComboBox();
            this.activeAlgorithm = new System.Windows.Forms.ComboBox();
            this.sessionTargetLabel = new System.Windows.Forms.Label();
            this.sessionFlagLabel = new System.Windows.Forms.Label();
            this.selectStateLabel = new System.Windows.Forms.Label();
            this.activeAlgorithmLabel = new System.Windows.Forms.Label();
            this.actionPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.submitButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.algorithmParms_1 = new RFID_Explorer.ConfigureAlgorithmParms_1_Display();
            this.algorithmParms_0 = new RFID_Explorer.ConfigureAlgorithmParms_0_Display();
            this.commonParmsPanel.SuspendLayout();
            this.actionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitleLabel
            // 
            this.panelTitleLabel.AutoSize = true;
            this.panelTitleLabel.ForeColor = System.Drawing.Color.Blue;
            this.panelTitleLabel.Location = new System.Drawing.Point(20, 10);
            this.panelTitleLabel.Name = "panelTitleLabel";
            this.panelTitleLabel.Size = new System.Drawing.Size(92, 12);
            this.panelTitleLabel.TabIndex = 0;
            this.panelTitleLabel.Text = "Algorithm Settings";
            // 
            // commonParmsPanel
            // 
            this.commonParmsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.commonParmsPanel.Controls.Add(this.tagGroupTarget);
            this.commonParmsPanel.Controls.Add(this.tagGroupSession);
            this.commonParmsPanel.Controls.Add(this.tagGroupSelected);
            this.commonParmsPanel.Controls.Add(this.activeAlgorithm);
            this.commonParmsPanel.Controls.Add(this.sessionTargetLabel);
            this.commonParmsPanel.Controls.Add(this.sessionFlagLabel);
            this.commonParmsPanel.Controls.Add(this.selectStateLabel);
            this.commonParmsPanel.Controls.Add(this.activeAlgorithmLabel);
            this.commonParmsPanel.Location = new System.Drawing.Point(10, 33);
            this.commonParmsPanel.Name = "commonParmsPanel";
            this.commonParmsPanel.Size = new System.Drawing.Size(560, 106);
            this.commonParmsPanel.TabIndex = 1;
            // 
            // tagGroupTarget
            // 
            this.tagGroupTarget.FormattingEnabled = true;
            this.tagGroupTarget.Location = new System.Drawing.Point(410, 71);
            this.tagGroupTarget.Name = "tagGroupTarget";
            this.tagGroupTarget.Size = new System.Drawing.Size(134, 20);
            this.tagGroupTarget.TabIndex = 78;
            this.tagGroupTarget.SelectedIndexChanged += new System.EventHandler(this.tagGroupTarget_SelectedIndexChanged);
            // 
            // tagGroupSession
            // 
            this.tagGroupSession.FormattingEnabled = true;
            this.tagGroupSession.Location = new System.Drawing.Point(410, 42);
            this.tagGroupSession.Name = "tagGroupSession";
            this.tagGroupSession.Size = new System.Drawing.Size(134, 20);
            this.tagGroupSession.TabIndex = 77;
            this.tagGroupSession.SelectedIndexChanged += new System.EventHandler(this.tagGroupSession_SelectedIndexChanged);
            // 
            // tagGroupSelected
            // 
            this.tagGroupSelected.FormattingEnabled = true;
            this.tagGroupSelected.Location = new System.Drawing.Point(410, 14);
            this.tagGroupSelected.Name = "tagGroupSelected";
            this.tagGroupSelected.Size = new System.Drawing.Size(134, 20);
            this.tagGroupSelected.TabIndex = 76;
            this.tagGroupSelected.SelectedIndexChanged += new System.EventHandler(this.tagGroupSelected_SelectedIndexChanged);
            // 
            // activeAlgorithm
            // 
            this.activeAlgorithm.FormattingEnabled = true;
            this.activeAlgorithm.Location = new System.Drawing.Point(97, 18);
            this.activeAlgorithm.Name = "activeAlgorithm";
            this.activeAlgorithm.Size = new System.Drawing.Size(134, 20);
            this.activeAlgorithm.TabIndex = 75;
            // 
            // sessionTargetLabel
            // 
            this.sessionTargetLabel.AutoSize = true;
            this.sessionTargetLabel.Location = new System.Drawing.Point(326, 73);
            this.sessionTargetLabel.Name = "sessionTargetLabel";
            this.sessionTargetLabel.Size = new System.Drawing.Size(72, 12);
            this.sessionTargetLabel.TabIndex = 3;
            this.sessionTargetLabel.Text = "Session Target";
            // 
            // sessionFlagLabel
            // 
            this.sessionFlagLabel.AutoSize = true;
            this.sessionFlagLabel.Location = new System.Drawing.Point(326, 45);
            this.sessionFlagLabel.Name = "sessionFlagLabel";
            this.sessionFlagLabel.Size = new System.Drawing.Size(39, 12);
            this.sessionFlagLabel.TabIndex = 2;
            this.sessionFlagLabel.Text = "Session";
            // 
            // selectStateLabel
            // 
            this.selectStateLabel.AutoSize = true;
            this.selectStateLabel.Location = new System.Drawing.Point(326, 18);
            this.selectStateLabel.Name = "selectStateLabel";
            this.selectStateLabel.Size = new System.Drawing.Size(57, 12);
            this.selectStateLabel.TabIndex = 1;
            this.selectStateLabel.Text = "Select State";
            // 
            // activeAlgorithmLabel
            // 
            this.activeAlgorithmLabel.AutoSize = true;
            this.activeAlgorithmLabel.Location = new System.Drawing.Point(8, 21);
            this.activeAlgorithmLabel.Name = "activeAlgorithmLabel";
            this.activeAlgorithmLabel.Size = new System.Drawing.Size(86, 12);
            this.activeAlgorithmLabel.TabIndex = 0;
            this.activeAlgorithmLabel.Text = "Active Algorithm";
            // 
            // actionPanel
            // 
            this.actionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.actionPanel.Controls.Add(this.cancelButton);
            this.actionPanel.Controls.Add(this.submitButton);
            this.actionPanel.Controls.Add(this.editButton);
            this.actionPanel.Controls.Add(this.loadButton);
            this.actionPanel.Location = new System.Drawing.Point(10, 274);
            this.actionPanel.Name = "actionPanel";
            this.actionPanel.Size = new System.Drawing.Size(560, 33);
            this.actionPanel.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(344, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 21);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(137, 3);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 21);
            this.submitButton.TabIndex = 3;
            this.submitButton.Text = "Save";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(137, 4);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 21);
            this.editButton.TabIndex = 67;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(344, 4);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 21);
            this.loadButton.TabIndex = 66;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // algorithmParms_1
            // 
            this.algorithmParms_1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.algorithmParms_1.Location = new System.Drawing.Point(10, 145);
            this.algorithmParms_1.MasterEnabled = false;
            this.algorithmParms_1.Name = "algorithmParms_1";
            this.algorithmParms_1.Size = new System.Drawing.Size(560, 123);
            this.algorithmParms_1.TabIndex = 4;
            // 
            // algorithmParms_0
            // 
            this.algorithmParms_0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.algorithmParms_0.Location = new System.Drawing.Point(10, 146);
            this.algorithmParms_0.MasterEnabled = false;
            this.algorithmParms_0.Name = "algorithmParms_0";
            this.algorithmParms_0.Size = new System.Drawing.Size(560, 123);
            this.algorithmParms_0.TabIndex = 3;
            // 
            // ConfigureAlgorithm_Display
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.algorithmParms_1);
            this.Controls.Add(this.algorithmParms_0);
            this.Controls.Add(this.actionPanel);
            this.Controls.Add(this.commonParmsPanel);
            this.Controls.Add(this.panelTitleLabel);
            this.Name = "ConfigureAlgorithm_Display";
            this.Size = new System.Drawing.Size(580, 318);
            this.commonParmsPanel.ResumeLayout(false);
            this.commonParmsPanel.PerformLayout();
            this.actionPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label panelTitleLabel;
        private System.Windows.Forms.Panel commonParmsPanel;
        private System.Windows.Forms.Panel actionPanel;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label sessionTargetLabel;
        private System.Windows.Forms.Label sessionFlagLabel;
        private System.Windows.Forms.Label selectStateLabel;
        private System.Windows.Forms.Label activeAlgorithmLabel;
        private System.Windows.Forms.ComboBox tagGroupTarget;
        private System.Windows.Forms.ComboBox tagGroupSession;
        private System.Windows.Forms.ComboBox tagGroupSelected;
        private System.Windows.Forms.ComboBox activeAlgorithm;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button cancelButton;
        private ConfigureAlgorithmParms_0_Display algorithmParms_0;
        private ConfigureAlgorithmParms_1_Display algorithmParms_1;

    }
}
