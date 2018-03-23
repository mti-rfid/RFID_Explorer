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
 * $Id: ConfigureGPIO.cs,v 1.4 2009/12/08 08:07:00 dciampi Exp $
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

using RFID.RFIDInterface;
using my_DataGridViewButtonColumn;

namespace RFID_Explorer
{
	public partial class ConfigureGPIO : UserControl
	{
		private LakeChabotReader _reader = null;
		private Timer            _timer  = null;

        private DataGridViewDisableButtonColumn   buttonColumn  = new DataGridViewDisableButtonColumn();
        private DataGridViewTextBoxColumn         nameColumn    = new DataGridViewTextBoxColumn( ); 
        private DataGridViewComboBoxColumn        accessColumn  = new DataGridViewComboBoxColumn( );
        private DataGridViewComboBoxColumn        stateColumn   = new DataGridViewComboBoxColumn( );
        private DataGridViewTextBoxColumn         statusColumn  = new DataGridViewTextBoxColumn( );

        private BindingSource              bindingSource = new BindingSource( );
        private Source_GPIOList            gpioList      = new Source_GPIOList();


        private enum enumColumn
        {
            ENUM_APPLY_BTN,
            ENUM_PIN_INDEX,
            ENUM_ACCESS,
            ENUM_PIN_STATE,
            ENUM_ACCESS_RESULT,
        }


        public ConfigureGPIO( LakeChabotReader reader )
        {
            if ( null == reader )
            {
                throw new ArgumentNullException( "reader", "Null reader passed to ConfigureGeneral CTOR()" );
            }

            if ( reader.Mode != rfidReader.OperationMode.BoundToReader )
            {
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureGeneral()" );
            }

            InitializeComponent( );

			_reader = reader;

			_timer           = new Timer();
			_timer.Interval  = 5000;
			_timer.Tick     += new EventHandler(timer_Tick);

			errorTextBox.Visible = false;
		}


		void timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			errorTextBox.Visible = false;
		}


        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

			ConfigureForm parent = this.TopLevelControl as ConfigureForm;

            if ( null == parent )
            {
                System.Diagnostics.Debug.Assert( false, String.Format( "Unknown parent form: {0}", this.TopLevelControl.GetType( ) ) );
            }
            else
            {
                //@@ parent.DialogClose += new DialogCloseDelegate( OnDialogClose );
            }

            //Button
            this.buttonColumn.HeaderText = "";
            this.buttonColumn.MinimumWidth = 90;
            this.buttonColumn.UseColumnTextForButtonValue = true;
            this.buttonColumn.Text = "APPLY";
            this.buttonColumn.Name = "Buttons";
            this.buttonColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            this.view.Columns.Add( buttonColumn );

            
            //Pin Number
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.MinimumWidth = 70;
            this.nameColumn.DataPropertyName = "Pin";
            this.nameColumn.DefaultCellStyle.Padding = new Padding( 5, 0, 0, 0 );
            this.nameColumn.ReadOnly = true;
            this.nameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            
            this.view.Columns.Add( nameColumn );

            
            //Access Type (Set or Get)
            this.accessColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.accessColumn.HeaderText = "Access";
            this.accessColumn.MinimumWidth = 98;
            this.accessColumn.DataPropertyName = "Access";
            this.accessColumn.Name = "Access";
            foreach (Source_GPIO.OpAccess item in Enum.GetValues(typeof(Source_GPIO.OpAccess)))
            {
                this.accessColumn.Items.Add(item);
            }
            this.accessColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            this.view.Columns.Add( accessColumn );

            
            //Pin State(High or Low)
            this.stateColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.stateColumn.HeaderText = "State";
            this.stateColumn.MinimumWidth = 98;
            this.stateColumn.DataPropertyName = "State";
            this.stateColumn.Name = "State";           
            foreach (Source_GPIO.OpState item in Enum.GetValues(typeof(Source_GPIO.OpState)))
            {
                this.stateColumn.Items.Add(item);
            }
            //Clark 2011.2.22  If pin was disable or got state failure,it will return UNKNOWN. Need to supply this item.               
            //this.stateColumn.Items.Remove(Source_GPIO.OpState.FAILURE);            
            this.stateColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            this.view.Columns.Add( stateColumn );


            //Access Pin result(Success or Failure)
            this.statusColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.statusColumn.HeaderText = "Status";
            this.statusColumn.MinimumWidth = 98;
            this.statusColumn.DataPropertyName = "Status";
            this.statusColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.statusColumn.ReadOnly = true;
            this.stateColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.view.Columns.Add(statusColumn);      
            
 

            gpioList.load( LakeChabotReader.MANAGED_ACCESS, _reader.ReaderHandle );

            this.bindingSource.DataSource = gpioList;

            this.view.DataSource = this.bindingSource;

            // The following so the combo boxes activate immediately
            // instead of requiring a 2x click operation...

            this.view.EditMode = DataGridViewEditMode.EditOnEnter;
           
		}

        private void view_CellContentClick( object sender, DataGridViewCellEventArgs e )
        {
			if ( e.ColumnIndex == this.buttonColumn.Index )
			{
                Source_GPIO pin =
                    ( ( BindingSource ) this.view.DataSource )
                    [ 
                        e.RowIndex
                    ]
                    as Source_GPIO;

                //Clark 2011.2.22   If can't get pin status, doesn't support this function.
                if ( pin.Status == Source_GPIO.OpResult.UNSUPPORTED )
                {
                    return;
                }

               
                switch ( pin.Access )
                {
                    case Source_GPIO.OpAccess.GET:
                        {
                            pin.load( LakeChabotReader.MANAGED_ACCESS, this._reader.ReaderHandle );
                        }
                        break;

                    case Source_GPIO.OpAccess.SET:
                        {
                            pin.store( LakeChabotReader.MANAGED_ACCESS, this._reader.ReaderHandle );
                        }
                        break;

                    default:
                        {
                            // NOP
                        }
                        break;
                }

                this.view.Refresh( );                   
            }
        }


        private void readAll_Click( object sender, EventArgs e )
        {   
            for( int row = 0; row < view.RowCount; ++ row )
            {
                Source_GPIO pin =
                    ( ( BindingSource ) this.view.DataSource )
                    [
                        row
                    ]
                    as Source_GPIO;

                pin.Access = Source_GPIO.OpAccess.GET;

                pin.load( LakeChabotReader.MANAGED_ACCESS, this._reader.ReaderHandle );
            }
            
            this.view.Refresh( );
        }


        //Clark 2011.2.22
        private void view_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Source_GPIO.OpResult gpioResult =  gpioList[e.RowIndex].Status;

            if ( gpioResult == Source_GPIO.OpResult.UNSUPPORTED )
            {
                e.CellStyle.ForeColor = SystemColors.GrayText;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.CellStyle.BackColor = SystemColors.ButtonFace;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;

                switch (e.ColumnIndex)
                {

                    case (int)enumColumn.ENUM_APPLY_BTN:
                        
                        //Set the text
                        e.Value = gpioResult.ToString();

                        try
                        {
                            //Disable the button
                            ((DataGridViewDisableButtonCell)view.Rows[e.RowIndex].Cells[e.ColumnIndex]).Enabled = false;
                        }
                        catch (Exception ex)
                        { 
                           
                        }

                        //2011.12.30 disable the Row which doesn't support the pin.
                        view.Rows[e.RowIndex].ReadOnly = true;
                        break;

                    case (int)enumColumn.ENUM_PIN_INDEX:
                    case (int)enumColumn.ENUM_ACCESS:
                        break;


                    case (int)enumColumn.ENUM_PIN_STATE:
                    case (int)enumColumn.ENUM_ACCESS_RESULT:
                        e.Value = gpioResult;
                        break;


                    default:
                        break;
                }
            }
            else
            {
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;                 
            }
        }

        private void view_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {

        }



	} //public partial class ConfigureGPIO : UserControl


}
