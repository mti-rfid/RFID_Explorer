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
 * $Id: ConfigureRFBand.cs,v 1.4 2009/12/03 03:56:28 dciampi Exp $
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



namespace RFID_Explorer
{
	public partial class ConfigureRFBandForm : UserControl
	{
        private static int EDIT_BUTTON_INDEX = 0;

		private LakeChabotReader	      reader = null;
		private Timer				      timer  = null;

        private DataGridViewButtonColumn  editColumn       = new DataGridViewButtonColumn( );

        private DataGridViewTextBoxColumn channelColumn    = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn stateColumn      = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn frequencyColumn  = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn multiplierColumn = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn dividerColumn    = new DataGridViewTextBoxColumn( );

        private DataGridViewTextBoxColumn minColumn        = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn affinityColumn   = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn maxColumn        = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn guardColumn      = new DataGridViewTextBoxColumn( );

        private BindingSource bindingSource =
            new BindingSource( );

        private Source_FrequencyBandList channelList =
            new Source_FrequencyBandList( );


        public ConfigureRFBandForm( LakeChabotReader reader )
		{
			if ( null == reader)
            {
				throw new ArgumentNullException("reader", "Null reader passed to ConfigureAntenna CTOR()");
            }

			if ( reader.Mode != rfidReader.OperationMode.BoundToReader )
            {
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureAntenna()" );
            }

            InitializeComponent( );

            this.reader = reader;

            this.timer = new Timer( );
            this.timer.Interval = 5000;
            this.timer.Tick += new EventHandler( timer_Tick );

            // edit column
            editColumn.HeaderText = "";
            editColumn.Text = "Edit";
            editColumn.UseColumnTextForButtonValue = true;
            editColumn.MinimumWidth = 50;
            editColumn.Resizable = DataGridViewTriState.False;
            editColumn.Width = 50;
            editColumn.Frozen = true;

            // channel column
            channelColumn.HeaderText = " Slot";
            channelColumn.MinimumWidth = 50;
            channelColumn.Name = "channelColumn";
            channelColumn.ReadOnly = true;
            channelColumn.Resizable = DataGridViewTriState.False;
            channelColumn.Width = 50;
            channelColumn.DataPropertyName = "Band";
            channelColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            channelColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            channelColumn.Frozen = true;

            // state column
            stateColumn.HeaderText = "Enabled";
            stateColumn.MinimumWidth = 100;
            stateColumn.Name = "stateColumn";
            stateColumn.ReadOnly = true;
            stateColumn.Resizable = DataGridViewTriState.False;
            stateColumn.Width = 100;
            stateColumn.DataPropertyName = "State";
            stateColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            stateColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // frequency column
            frequencyColumn.HeaderText = "Frequency (MHz)";
            frequencyColumn.MinimumWidth = 120;
            frequencyColumn.Name = "frequencyColumn";
            frequencyColumn.ReadOnly = true;
            frequencyColumn.Resizable = DataGridViewTriState.False;
            frequencyColumn.Width = 100;
            frequencyColumn.DataPropertyName = "Frequency";
            frequencyColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            frequencyColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // multiplier column
            multiplierColumn.HeaderText = "Multiply Ratio";
            multiplierColumn.MinimumWidth = 100;
            multiplierColumn.Name = "multiplierColumn";
            multiplierColumn.ReadOnly = true;
            multiplierColumn.Resizable = DataGridViewTriState.False;
            multiplierColumn.Width = 100;
            multiplierColumn.DataPropertyName = "MultiplyRatio";
            multiplierColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            multiplierColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // divider column
            dividerColumn.HeaderText = "Divide Ratio";
            dividerColumn.MinimumWidth = 100;
            dividerColumn.Name = "dividerColumn";
            dividerColumn.ReadOnly = true;
            dividerColumn.Resizable = DataGridViewTriState.False;
            dividerColumn.Width = 100;
            dividerColumn.DataPropertyName = "DivideRatio";
            dividerColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dividerColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // min column
            minColumn.HeaderText = "Minimum DAC Band";
            minColumn.MinimumWidth = 100;
            minColumn.Name = "minColumn";
            minColumn.ReadOnly = true;
            minColumn.Resizable = DataGridViewTriState.False;
            minColumn.Width = 100;
            minColumn.DataPropertyName = "MinimumDACBand";
            minColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            minColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // affinity column
            affinityColumn.HeaderText = "Affinity Band";
            affinityColumn.MinimumWidth = 100;
            affinityColumn.Name = "affinityColumn";
            affinityColumn.ReadOnly = true;
            affinityColumn.Resizable = DataGridViewTriState.False;
            affinityColumn.Width = 100;
            affinityColumn.DataPropertyName = "AffinityBand";
            affinityColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            affinityColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // max column
            maxColumn.HeaderText = "Maximum DAC Band";
            maxColumn.MinimumWidth = 100;
            maxColumn.Name = "maxColumn";
            maxColumn.ReadOnly = true;
            maxColumn.Resizable = DataGridViewTriState.False;
            maxColumn.Width = 100;
            maxColumn.DataPropertyName = "MaximumDACBand";
            maxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            maxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // guard column
            guardColumn.HeaderText = "Guard Band";
            guardColumn.MinimumWidth = 100;
            guardColumn.Name = "guardColumn";
            guardColumn.ReadOnly = true;
            guardColumn.Resizable = DataGridViewTriState.False;
            guardColumn.Width = 100;
            guardColumn.DataPropertyName = "GuardBand";
            guardColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            guardColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            view.RowHeadersVisible = false;
        }


        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            ConfigureForm parent = this.TopLevelControl as ConfigureForm;

            view.AutoGenerateColumns = false;
            view.AllowUserToResizeRows = false;
            view.AllowUserToResizeColumns = false;
            view.AllowUserToOrderColumns = false;
            view.AllowUserToAddRows = false;
            view.AllowUserToDeleteRows = false;
            view.MultiSelect = false;
            view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            view.Columns.Clear( );

            view.Columns.AddRange
            (
                editColumn,
                channelColumn,
                stateColumn,
                frequencyColumn,
                multiplierColumn,
                dividerColumn,
                minColumn,
                affinityColumn,
                maxColumn,
                guardColumn
            );

            view.CellContentClick += new DataGridViewCellEventHandler( CellContentClick );
            view.CellFormatting   += new DataGridViewCellFormattingEventHandler( CellFormatting );

            //this.channelList.load( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );

            //this.bindingSource.DataSource = this.channelList;

            //this.view.DataSource = this.bindingSource;
        }


        // This override is REQUIRED due to the 'quick set' "feature"
        // in the configure settings panel...

        protected override void OnVisibleChanged( EventArgs e )
        {
            base.OnVisibleChanged( e );

            // cludge to handle edit window event mis-ordering that
            // seems to occur about 50% or so of the time...

            if ( null != this.reader && this.Visible )
            {
                this.channelList.load( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );

                this.bindingSource.DataSource = this.channelList;

                this.view.DataSource = this.bindingSource;
            }
        }


        void CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {
            if ( Source_FrequencyBand.BandState.DISABLED == this.channelList[ e.RowIndex ].State )
            {
                e.CellStyle.BackColor = SystemColors.ButtonFace;
                e.CellStyle.ForeColor = SystemColors.GrayText;
            }
        }


        void timer_Tick( object sender, EventArgs e )
        {
            this.timer.Stop( );
        }


        void CellContentClick( object sender, DataGridViewCellEventArgs e )
        {
            if ( e.ColumnIndex == EDIT_BUTTON_INDEX )
            {
                using ( RFChannelEditForm dlg = new RFChannelEditForm( this.reader, this.channelList[ e.RowIndex ] ) )
                {
                    if ( DialogResult.OK == dlg.ShowDialog( ) )
                    {
                        this.view.Refresh( );
                    }
                    dlg.Close( );
                }
            }
        }


        private void exportButton_Click( object sender, EventArgs e )
        {
            Cursor priorCursor = null;
            try
            {
                this.Capture = true;
                priorCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                RFID_Explorer.ExcelExport.ExportRFChannelConfig( this.reader );
            }
            finally
            {
                if ( priorCursor != null ) Cursor.Current = priorCursor;
                this.Capture = false;
            }
        }


        private void importButton_Click( object sender, EventArgs e )
        {
            Cursor priorCursor = null;

            RFID_Explorer.mainForm.CommonDialogSupport dlg = new mainForm.CommonDialogSupport( mainForm.CommonDialogSupport.DialogType.OpenChannel );

            if ( dlg.ShowDialog( ) == DialogResult.OK )
            {
                try
                {
                    this.Capture = true;
                    priorCursor = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        this.bindingSource.DataSource =  this.channelList =
                              RFID_Explorer.ExcelExport.ImportRFChannelConfig( this.reader, dlg.FileName );

                        
                        rfid.Constants.Result result =
                            this.channelList.store( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );

                        if ( rfid.Constants.Result.OK != result )
                        {
                            throw new Exception( result.ToString( ) );
                        }

                        this.view.Refresh( );
                    }
                    catch ( Exception e2 )
                    {
                        MessageBox.Show( String.Format( "Error importing RF channel settings.\n\n{0}", e2.Message ), "Invalid import file", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        return;
                    }
                }
                finally
                {
                    if ( priorCursor != null ) Cursor.Current = priorCursor;
                    this.Capture = false;
                }
            }
        }

        private void ConfigureRFBandForm_Load( object sender, EventArgs e )
        {

        }


    } // public partial class ConfigureRFBandForm : UserControl


}
