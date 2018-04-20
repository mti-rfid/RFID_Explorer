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
 * $Id: ExportForm.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
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

namespace RFID_Explorer
{
	public partial class ExportForm : Form
	{
        const string DEF_FILE_NAME = "RFID Exported Data.xml";
        const string DEF_FILE_PATH = "C:\\";

        private string m_strExportPath
        {
            get 
            {
                return RFID_Explorer.Properties.Settings.Default.strExportPath;  
            }
            
            set 
            { 
                RFID_Explorer.Properties.Settings.Default.strExportPath = value; 
                RFID_Explorer.Properties.Settings.Default.Save();
            }
        }


		public ExportForm()
		{
			InitializeComponent();

            this.textPath.Text = m_strExportPath;

		}

		private void selectAllButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < ExportViewCheckedListBox.Items.Count; i++)
				ExportViewCheckedListBox.SetItemChecked(i, true);
		}

		private void ClearAllButton_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < ExportViewCheckedListBox.Items.Count; i++)
				ExportViewCheckedListBox.SetItemChecked(i, false);

		}

		private void ExportViewCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.NewValue == CheckState.Checked)
			{
				okButton.Enabled = true;
				return;
			}

			okButton.Enabled  = ExportViewCheckedListBox.CheckedItems.Count > 1;
		}



        private void btnBrowser_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter   = "XML (*.xml)|*.xml|ALL Files (*.*)|*.*";
            saveFileDialog1.Title    = "Save a File";
            saveFileDialog1.FileName = DEF_FILE_NAME;

            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.textPath.Text = m_strExportPath = saveFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show( "Please select a valid path. Restore the default path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.textPath.Text = m_strExportPath = System.IO.Path.Combine(DEF_FILE_PATH, DEF_FILE_NAME );
            }           
        }



        private void btnDefaultPath_Click(object sender, EventArgs e)
        {
            this.textPath.Text = m_strExportPath = System.IO.Path.Combine(DEF_FILE_PATH, DEF_FILE_NAME );
        }

		
	}
}