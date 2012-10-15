/*
 *****************************************************************************
 *                                                                           *
 *                   MTI CONFIDENTIAL AND PROPRIETARY                        *
 *                                                                           *
 * This source code is the sole property of MTI, Inc.  Reproduction or       *
 * utilization of this source code in whole or in part is forbidden without  *
 * the prior written consent of MTI, Inc.                                    *
 *                                                                           *
 * (c) Copyright MTI, Inc. 2011. All rights reserved.                        *
 *                                                                           *
 *****************************************************************************
 */

/*
 *****************************************************************************
 *
 * $Id: OptionsLogging.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID_Explorer.Properties;

namespace RFID_Explorer
{
	public partial class OptionsLoggingControl : UserControl
	{
		private bool _noTempFileStartingValue;

		public OptionsLoggingControl()
		{
			InitializeComponent();

			if ((enableLoggingCheckBox.Checked = Settings.Default.enableLogging) == true)
			{
				savePathTextBox.Enabled = true;
				directoryButton.Enabled = true;
			}
			else
			{
				savePathTextBox.Enabled = false;
				directoryButton.Enabled = false;
			}


			if (Settings.Default.logPath == null || Settings.Default.logPath == "")
			{
				Settings.Default.logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			savePathTextBox.Text = Settings.Default.logPath;

			noTempFileCheckBox.Checked = _noTempFileStartingValue = Settings.Default.noTempFile;
		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			OptionsForm parent = this.TopLevelControl as OptionsForm;
			if (parent == null)
			{
				System.Diagnostics.Debug.Assert(false, String.Format("Unknown parent form: {0}", this.TopLevelControl.GetType()));
			}
			else
			{
				parent.DialogClose +=new DialogCloseDelegate(DialogClose);
			}

		}



		void DialogClose(object sender, DialogCloseEventArgs e)
		{
			if (e.Result == DialogResult.OK)
			{
				if (enableLoggingCheckBox.Checked)
				{
					if (!System.IO.Directory.Exists(savePathTextBox.Text))
					{
						e.PageError("Invalid directory.\n\nThe log file directory is invalid. Please provide a valid directory for the log files or disable logging.", this);
						e.ErrorControl = savePathTextBox;
						e.SelectAll = true;
						return;
					}
				}

				e.SaveRequired = true;
				e.RestartRequired = _noTempFileStartingValue != Settings.Default.noTempFile;
			}
		}


		private void enableLoggingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Settings.Default.enableLogging = enableLoggingCheckBox.Checked;
			if (enableLoggingCheckBox.Checked)
			{
				savePathTextBox.Enabled = true;
				directoryButton.Enabled = true;
			}
			else
			{
				savePathTextBox.Enabled = false;
				directoryButton.Enabled = false;
			}
		}


		private void noTempFileCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Settings.Default.noTempFile = noTempFileCheckBox.Checked;
		}



		private void directoryButton_Click(object sender, EventArgs e)
		{
			RFID_Explorer.mainForm.CommonDialogSupport dlg = new RFID_Explorer.mainForm.CommonDialogSupport(mainForm.CommonDialogSupport.DialogType.Folder);

			dlg.ShowDialog();

			if (dlg.Result == DialogResult.OK)
			{
				savePathTextBox.Text = dlg.FileName;
				Settings.Default.logPath = dlg.FileName;
			}
		}

	}
}
