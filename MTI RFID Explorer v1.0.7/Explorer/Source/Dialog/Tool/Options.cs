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
 * $Id: Options.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RFID_Explorer.Properties;
using System.Reflection;

namespace RFID_Explorer
{
	public partial class OptionsForm : Form
	{
		public event DialogCloseDelegate DialogClose;

		OptionsGeneralControl generalOptions;
		OptionsLoggingControl loggingOptions;

		private bool _restartRequired			= false;
		

		public bool RestartRequired
		{
			get { return _restartRequired; }
			set { _restartRequired = value; }
		}

		public OptionsForm(string [] viewArray)
		{
			InitializeComponent();

			this.Text = String.Format("{0} Options", AssemblyTitle);

			generalOptions = new OptionsGeneralControl(viewArray);
			generalOptions.Dock = DockStyle.Fill;
			generalOptions.Visible = true;


			loggingOptions = new OptionsLoggingControl();
			loggingOptions.Dock = DockStyle.Fill;
			loggingOptions.Visible = false;

			this.optionsToolStripContainer.ContentPanel.Controls.AddRange( new Control[] {generalOptions, loggingOptions});
			// select the general options tab
			generalToolStripButton.PerformClick();

		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}


		public string AssemblyTitle
		{
			get
			{
				// Get all Title attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				// If there is at least one Title attribute
				if (attributes.Length > 0)
				{
					// Select the first one
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					// If it is not an empty string, return it
					if (titleAttribute.Title != "")
						return titleAttribute.Title;
				}
				// If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}



		private void generalToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ToolStripItem item in optionsToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}

			generalToolStripButton.Checked = true;
			
			generalOptions.Visible = true;
			
			loggingOptions.Visible = false;
			
		}


		private void inventoryToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ToolStripItem item in optionsToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;

				if (button != null)
					button.Checked = false;
			}

			loggingToolStripButton.Checked = false;
			generalToolStripButton.Checked = false;

			loggingOptions.Visible = false;
			generalOptions.Visible = false;
			
		}


		private void loggingToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (ToolStripItem item in optionsToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}

			loggingToolStripButton.Checked = true;
			generalToolStripButton.Checked = false;

			loggingOptions.Visible = true;
			generalOptions.Visible = false;
		}





		private void okButton_Click(object sender, EventArgs e)
		{
						
			DialogCloseEventArgs closeNotice = new DialogCloseEventArgs(DialogResult.OK);
			if (DialogClose != null)
			{
				DialogClose(this, closeNotice);
			}
			
            if (closeNotice.SaveRequired)
			{
				RFID_Explorer.Properties.Settings.Default.Save();
			}

			if (closeNotice.Error)
			{
				if (closeNotice.ErrorPage is OptionsGeneralControl)
				{
					generalToolStripButton.PerformClick();
				}

				if (closeNotice.ErrorPage is OptionsLoggingControl)
				{
					loggingToolStripButton.PerformClick();
				}

				MessageBox.Show(closeNotice.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				if (closeNotice.ErrorControl != null)
				{
					closeNotice.ErrorControl.Focus();
					if (closeNotice.SelectAll && closeNotice.ErrorControl is TextBox)
					{
						((TextBox)closeNotice.ErrorControl).SelectAll();
					}
				}
				return;
			}

			_restartRequired = closeNotice.RestartRequired;
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Settings.Default.Reload();
			DialogResult = DialogResult.Cancel;
		}

	}
}