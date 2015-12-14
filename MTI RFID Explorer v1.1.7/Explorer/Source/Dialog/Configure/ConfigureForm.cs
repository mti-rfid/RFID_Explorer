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
 * $Id: ConfigureForm.cs,v 1.4 2009/12/03 03:48:49 dciampi Exp $
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
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using RFID_Explorer.Properties;
using RFID.RFIDInterface;


namespace RFID_Explorer
{
	public partial class ConfigureForm : Form
	{
		public event DialogCloseDelegate DialogClose;

		private LakeChabotReader			        _reader					= null;
		private ConfigureSettingsControl	        _generalControl			= null;
		private ConfigureAntenna			        _antennaControl			= null;
        private ConfigureSelect_Display             _selectCriteriaControl  = null;
		private ConfigureAlgorithm_Display	        _algorithmControl		= null;
        private ConfigurePostSingulation_Display    _postSingulationControl = null;
		private ConfigureGPIO				        _gpioControl			= null;
		private ConfigureRFBandForm			        _rfControl				= null;
		private AboutReaderControl			        _aboutReaderControl		= null;
		private ConfigureTroubleControl		        _troubleshootControl	= null;
		
		
		public ConfigureForm(LakeChabotReader reader)
		{            
			_reader = reader;

			InitializeComponent();
            
            //Clark 2011.7.25.   Clear Memory data. Avoid many invenory or 
            //other to full in memory and can't catch corrent packet.
            LakeChabotReader.MANAGED_ACCESS.API_RadioEnterConfigMenu();
    

            this.Text = String.Format("{0}  Module Configuration", AssemblyTitle);
           
			_generalControl = new ConfigureSettingsControl(Reader);
            _generalControl.Visible = true;
            _generalControl.Name = "_generalControl";
            configureToolStripContainer.ContentPanel.Controls.Add(_generalControl);
           
            _antennaControl	= new ConfigureAntenna(Reader);
            _antennaControl.Visible = false;
            _antennaControl.Name = "_antennaControl";
            configureToolStripContainer.ContentPanel.Controls.Add(_antennaControl);
            
            _selectCriteriaControl = new ConfigureSelect_Display( Reader );
            _selectCriteriaControl.Visible = false;
            _selectCriteriaControl.Name = "_selectCriteriaControl";
            configureToolStripContainer.ContentPanel.Controls.Add(_selectCriteriaControl);
            
			_algorithmControl = new ConfigureAlgorithm_Display(Reader);
			_algorithmControl.Visible = false;
			_algorithmControl.Name = "_algorithmControl";
			configureToolStripContainer.ContentPanel.Controls.Add(_algorithmControl);
            
            _postSingulationControl = new ConfigurePostSingulation_Display(Reader);
            _postSingulationControl.Visible = false;
            _postSingulationControl.Name = "_postSingulationControl";
            configureToolStripContainer.ContentPanel.Controls.Add(_postSingulationControl);
            
            _gpioControl = new ConfigureGPIO(Reader);
			_gpioControl.Visible = false;
			_gpioControl.Name = "_gpioControl"; 
			configureToolStripContainer.ContentPanel.Controls.Add(_gpioControl);            

            
			_rfControl = new ConfigureRFBandForm(Reader);
			_rfControl.Visible = false;
			_rfControl.Name = "_rfControl";
			configureToolStripContainer.ContentPanel.Controls.Add(_rfControl);
            
			_aboutReaderControl = new AboutReaderControl(Reader);
            _aboutReaderControl.Visible = false;
			_aboutReaderControl.Name = "_aboutReaderControl";
			configureToolStripContainer.ContentPanel.Controls.Add(_aboutReaderControl);
            
			_troubleshootControl = new ConfigureTroubleControl(Reader);
			_troubleshootControl.Visible = false;
			_troubleshootControl.Name = "_troubleshootControl";
			configureToolStripContainer.ContentPanel.Controls.Add(_troubleshootControl);


            //Clark 2011.2.10 Cpoied from R1000 Tracer.
            RFBandsToolStripButton.Visible     = false;

		}



		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
            
			settingsToolStripButton.PerformClick();
		}

		public LakeChabotReader Reader
		{
			get { return _reader; }
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
			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}
            
			_generalControl.Visible = true;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
            _gpioControl.Visible = false;
			_rfControl.Visible = false;
            _aboutReaderControl.Visible = false;
            _troubleshootControl.Visible = false;
			settingsToolStripButton.Checked = true;
            
		}

		private void antennaStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = true;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			antennaStripButton.Checked = true;
			Cursor.Current = oldCursor;
             
		}


        private void selectCriteriaStripButton_Click(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            foreach (ToolStripItem item in configureToolStrip.Items)
            {
                ToolStripButton button = item as ToolStripButton;
                if (button != null)
                    button.Checked = false;

            }
            
            _generalControl.Visible = false;
            _antennaControl.Visible = false;
            _selectCriteriaControl.Visible = true;
            _algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
            _rfControl.Visible = false;
            _aboutReaderControl.Visible = false;
            _troubleshootControl.Visible = false;

            selectCriteriaStripButton.Checked = true;
            Cursor.Current = oldCursor;
        }

		private void algorithmStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = true;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			algorithmStripButton.Checked = true;
			Cursor.Current = oldCursor;
		}


        private void postSingulationStripButton_Click(object sender, EventArgs e)
        {
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            foreach (ToolStripItem item in configureToolStrip.Items)
            {
                ToolStripButton button = item as ToolStripButton;
                if (button != null)
                    button.Checked = false;

            }
            
            _generalControl.Visible = false;
            _antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
            _algorithmControl.Visible = false;
            _postSingulationControl.Visible = true;
			_gpioControl.Visible = false;
            _rfControl.Visible = false;
            _aboutReaderControl.Visible = false;
            _troubleshootControl.Visible = false;

            postSingulationStripButton.Checked = true;
            Cursor.Current = oldCursor;
        }


		private void gpioStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = true;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			gpioStripButton.Checked = true;
			Cursor.Current = oldCursor;
		}


		private void RFBandsToolStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;


			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;

			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = true;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			RFBandsToolStripButton.Checked = true;
			Cursor.Current = oldCursor;
		}

		private void macRegistersToolStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;

				if (button != null)
					button.Checked = false;
			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			Cursor.Current = oldCursor;
		}


		private void hardwareToolStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;
			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = false;

			Cursor.Current = oldCursor;
		}

		private void aboutReaderToolStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;
			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
			_gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = true;
			_troubleshootControl.Visible = false;

			versionToolStripButton.Checked = true;
			Cursor.Current = oldCursor;
		}


		private void troubleshootToolStripButton_Click(object sender, EventArgs e)
		{
			Cursor oldCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			foreach (ToolStripItem item in configureToolStrip.Items)
			{
				ToolStripButton button = item as ToolStripButton;
				if (button != null)
					button.Checked = false;
			}
            
			_generalControl.Visible = false;
			_antennaControl.Visible = false;
            _selectCriteriaControl.Visible = false;
			_algorithmControl.Visible = false;
            _postSingulationControl.Visible = false;
            _gpioControl.Visible = false;
			_rfControl.Visible = false;
			_aboutReaderControl.Visible = false;
			_troubleshootControl.Visible = true;
			
			troubleshootToolStripButton.Checked = true;
			Cursor.Current = oldCursor;
		}


		private void okButton_Click(object sender, EventArgs e)
		{
			DialogCloseEventArgs closeNotice =  new DialogCloseEventArgs(DialogResult.OK);
			
			if (DialogClose != null)
			{
				DialogClose(this, closeNotice);
				if (closeNotice.Error)
				{
					{
						if (closeNotice.ErrorPage == _generalControl)
						{
							settingsToolStripButton.PerformClick();
						}
						else if (closeNotice.ErrorPage == _antennaControl)
						{
							antennaStripButton.PerformClick();
						}
                        else if (closeNotice.ErrorPage == _selectCriteriaControl)
                        {
                         // TODO:   selectCriteriaButton.PerformClick();
                        }
                        else if (closeNotice.ErrorPage == _selectCriteriaControl)
                        {
                            selectCriteriaStripButton.PerformClick();
                        } 
						else if (closeNotice.ErrorPage == _algorithmControl)
						{
							algorithmStripButton.PerformClick();
						}                        
						else if (closeNotice.ErrorPage == _postSingulationControl)
						{
							postSingulationStripButton.PerformClick();
						}
						else if (closeNotice.ErrorPage == _gpioControl)
						{
							gpioStripButton.PerformClick();
						}

						/*else if (closeNotice.ErrorPage == _rfControl)
						{
							RFBandsToolStripButton.PerformClick();
						}*/
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

			}
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Settings.Default.Reload();
			DialogResult = DialogResult.Cancel;

			if (DialogClose != null)
			{
				DialogClose(this, new DialogCloseEventArgs(DialogResult.Cancel));
			}
			this.Close();
		}

	}
}