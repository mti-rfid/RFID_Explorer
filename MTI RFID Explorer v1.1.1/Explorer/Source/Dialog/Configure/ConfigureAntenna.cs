using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;
using rfid.Constants;
using rfid.Structures;

using Global;


namespace RFID_Explorer
{

    public partial class ConfigureAntenna : UserControl
    {
        private LakeChabotReader reader       = null;
        private Timer            timer        = null;
        public  string           error        = "";

        private DataGridViewButtonColumn  editColumn              = new DataGridViewButtonColumn( );

        private DataGridViewTextBoxColumn portColumn              = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn PortColumn              = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn powerColumn             = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn dwellColumn             = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn roundsColumn            = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn antennaSenseValueColumn = new DataGridViewTextBoxColumn( );
        private DataGridViewTextBoxColumn antennaCountColumn      = new DataGridViewTextBoxColumn( );

        private BindingSource      bindingSource                  = new BindingSource( );
        private Source_AntennaList antennaList                    = new Source_AntennaList( );


        private enum enumColumn
        {
            ENUM_EDIT_BTN,
            ENUM_PORT_INDEX,
            ENUM_PHYSICAL_PORT,
            ENUM_POWER_LEVEL,
            ENUM_DWELL_TIME,
            ENUM_INVENTORY_CYCLE,
        }


        public ConfigureAntenna( LakeChabotReader reader )
        {
            if ( null == reader )
            {
                throw new ArgumentNullException( "reader", "Null reader passed to ConfigureAntenna CTOR()" );
            }

            if ( reader.Mode != rfidReader.OperationMode.BoundToReader )
            {
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureAntenna()" );
            }

            InitializeComponent( );

            //applyAtStartupCheckBox.Checked = !String.IsNullOrEmpty( RFID_Explorer.Properties.Settings.Default.antennaSettings );

            this.reader = reader;
            this.timer = new Timer( );
            this.timer.Interval = 5000;
            this.timer.Tick += new EventHandler( timer_Tick );


//clark not sure. Wait firmware support this function.
#if (LBT)
            //RfPowerThreshold  Clark 2011.2.10 Cpoied from R1000 Tracer
            UInt32 oem_Data = 0;                       
            reader.MacReadOemData
                ( 
                    (ushort)enumOEM_ADDR.ENUM_OEM_ADDR_RF_REVPWR_THRESHOLD,//0x000000AE,
                    ref oem_Data
                );

            numericUpDownRfPowerThreshold.Value = oem_Data;
#endif
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
                parent.DialogClose += new DialogCloseDelegate( OnDialogClose );
            }


            view.AutoGenerateColumns      = false;
            view.AllowUserToResizeRows    = false;
            view.AllowUserToResizeColumns = false;
            view.AllowUserToOrderColumns  = false;
            view.AllowUserToAddRows       = false;
            view.AllowUserToDeleteRows    = false;
            view.MultiSelect              = false;
            view.RowHeadersVisible        = false;
            view.SelectionMode            = DataGridViewSelectionMode.FullRowSelect;
            view.CellContentClick        += new DataGridViewCellEventHandler( CellContentClick );
            view.CellFormatting          += new DataGridViewCellFormattingEventHandler( CellFormatting );

            view.Columns.Clear( );


            // editColumn
            editColumn.HeaderText = "";
            editColumn.Text = "Edit";
            editColumn.UseColumnTextForButtonValue = true;
            editColumn.MinimumWidth = 50;
            editColumn.Resizable = DataGridViewTriState.False;
            editColumn.Width = 50;
            editColumn.Frozen = true;
            view.Columns.Add(editColumn);


            // portColumn
            portColumn.HeaderText = " #";
            portColumn.MinimumWidth = 50;
            portColumn.Name = "portColumn";
            portColumn.ReadOnly = true;
            portColumn.Resizable = DataGridViewTriState.False;
            portColumn.Width = 50;
            portColumn.DataPropertyName = "Port";
            portColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            portColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            portColumn.Frozen = true;
            view.Columns.Add(portColumn);


            // txPortColumn
            //Clark 2011.2.10 Cpoied from R1000 Tracer
            //txPortColumn.HeaderText = "Physical Tx Port"; 
            PortColumn.HeaderText = "Physical Port";
            PortColumn.MinimumWidth = 100;
            PortColumn.Name = "PortColumn";
            PortColumn.ReadOnly = true;
            PortColumn.Resizable = DataGridViewTriState.False;
            PortColumn.Width = 100;
            PortColumn.DataPropertyName = "PhysicalPort";
            PortColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PortColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            view.Columns.Add(PortColumn);                     


            // powerColumn
            //Clark 2011.2.10 Cpoied from R1000 Tracer            
            //powerColumn.HeaderText = "Power Level (dBm)";
            powerColumn.HeaderText = "Power Level 1/10 dBm";
            powerColumn.MinimumWidth = 100;
            powerColumn.Name = "powerColumn";
            powerColumn.ReadOnly = true;
            powerColumn.Resizable = DataGridViewTriState.False;
            powerColumn.Width = 100;
            powerColumn.DataPropertyName = "PowerLevel";
            powerColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            powerColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            view.Columns.Add(powerColumn);     


            // dwellColumn
            dwellColumn.HeaderText = "Dwell Time (milliseconds)";
            dwellColumn.MinimumWidth = 100;
            dwellColumn.Name = "dwellColumn";
            dwellColumn.ReadOnly = true;
            dwellColumn.Resizable = DataGridViewTriState.False;
            dwellColumn.Width = 100;
            dwellColumn.DataPropertyName = "DwellTime";
            dwellColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dwellColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            view.Columns.Add(dwellColumn);     


            // roundsColumn
            roundsColumn.HeaderText = "Inventory Rounds";
            roundsColumn.MinimumWidth = 100;
            roundsColumn.Name = "roundsColumn";
            roundsColumn.ReadOnly = true;
            roundsColumn.Resizable = DataGridViewTriState.False;
            roundsColumn.Width = 100;
            roundsColumn.DataPropertyName = "NumberInventoryCycles";
            roundsColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            roundsColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            view.Columns.Add(roundsColumn);     
            

            // antennaSenseValueColumn
            antennaSenseValueColumn.HeaderText = "Antenna Sense Value (Ohms)";
            antennaSenseValueColumn.MinimumWidth = 100;
            antennaSenseValueColumn.Name = "antennaSenseValueColumn";
            antennaSenseValueColumn.ReadOnly = true;
            antennaSenseValueColumn.Resizable = DataGridViewTriState.False;
            antennaSenseValueColumn.Width = 100;
            antennaSenseValueColumn.DataPropertyName = "AntennaSenseValue";
            antennaSenseValueColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            antennaSenseValueColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            view.Columns.Add(antennaSenseValueColumn);


            //clark not sure. Wait firmware support this function.
            //Hide MTI Item
            //Global Antenna Sense Threshold
            label_PowerThreshold.Visible          = false;
            globalSenseThreshold.Visible          = false;
            editGlobalSenseThreshold.Visible      = false;
            //RF Reverse Power Threshold
            label_GlobalThreshold.Visible         = false;
            numericUpDownRfPowerThreshold.Visible = false;
            Btn_SetRfPowerThreshold.Visible       = false;


            this.antennaList.load( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );

            this.bindingSource.DataSource = this.antennaList;

            this.view.DataSource = this.bindingSource;

            this.updateThreshold( );
        }



        //Clark 2011.2.10 Cpoied from R1000 Tracer.
        void CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {

           switch (e.ColumnIndex)
            {
                //Clark 2011.2.10 Cpoied from R1000 Tracer
                case (int)enumColumn.ENUM_PHYSICAL_PORT:
                    e.Value = this.antennaList[e.RowIndex].PhysicalPort.ToString();
                break;

                case (int)enumColumn.ENUM_POWER_LEVEL:
                    e.Value = (this.antennaList[e.RowIndex].PowerLevel / 1).ToString();
                    break;

                case (int)enumColumn.ENUM_DWELL_TIME:
                    e.Value = this.antennaList[e.RowIndex].DwellTime.ToString();
                    break;

                case (int)enumColumn.ENUM_INVENTORY_CYCLE:
                    e.Value = this.antennaList[e.RowIndex].NumberInventoryCycles.ToString();
                    break;

                default:
                    break;
            }


            if (this.antennaList[e.RowIndex].State == rfid.Constants.AntennaPortState.DISABLED)
            {
                e.CellStyle.ForeColor = SystemColors.GrayText;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.CellStyle.BackColor = SystemColors.ButtonFace;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;

                switch (e.ColumnIndex)
                {
                    case (int)enumColumn.ENUM_EDIT_BTN:
                        e.Value = "Edit";
                        break;

                    case (int)enumColumn.ENUM_PORT_INDEX:
                        break;

                    case (int)enumColumn.ENUM_PHYSICAL_PORT:
                        e.Value = "";//"Inactive";
                        break;


                    default:
                        e.Value = "";
                        break;
                }
            }
            else
            {
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;

                switch (e.ColumnIndex)
                {

                    case (int)enumColumn.ENUM_EDIT_BTN:
                        e.Value = "Edit";
                        break;

                    case (int)enumColumn.ENUM_DWELL_TIME://DWELL_INDEX:
                    case (int)enumColumn.ENUM_INVENTORY_CYCLE://ROUNDS_INDEX:
                        if (e.Value.ToString() == "0")
                        {
                            e.Value = "No Limit";
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        void timer_Tick( object sender, EventArgs e )
        {
            this.timer.Stop( );
        }


        void CellContentClick( object sender, DataGridViewCellEventArgs e )
        {
            if (e.ColumnIndex == (int)enumColumn.ENUM_EDIT_BTN)
            {
                using ( AntennaEditForm dlg = new AntennaEditForm( this.reader, this.antennaList[ e.RowIndex ] ) )
                {
                    if ( DialogResult.OK == dlg.ShowDialog( ) )
                    {             
                        view.Refresh( );
                    }
                    dlg.Close( );
                }
            }
        }


        void OnDialogClose( object sender, DialogCloseEventArgs e )
        {
            switch ( e.Result )
            {
                case DialogResult.Abort:
                    break;

                case DialogResult.Cancel:
                    break;

                case DialogResult.Ignore:
                    break;

                case DialogResult.No:
                    break;

                case DialogResult.None:
                    break;

                case DialogResult.OK:
                    break;

                case DialogResult.Retry:
                    break;

                case DialogResult.Yes:
                    break;

                default:
                    break;
            }
        }


//==================Button Event============================================================
        private void exportButton_Click( object sender, EventArgs e )
        {
            Cursor priorCursor = null;
            try
            {
                this.Capture   = true;
                priorCursor    = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                RFID_Explorer.ExcelExport.ExportAntennaConfig( this.reader );
            }
            finally
            {
                if ( priorCursor != null )
                {
                    Cursor.Current = priorCursor;
                }
                this.Capture = false;
            }
        }


        private void importButton_Click( object sender, EventArgs e )
        {
            Cursor priorCursor = null;
            RFID_Explorer.mainForm.CommonDialogSupport dlg =
                new mainForm.CommonDialogSupport( mainForm.CommonDialogSupport.DialogType.OpenAntenna );

            if ( dlg.ShowDialog( ) == DialogResult.OK )
            {
                try
                {
                    this.Capture   = true;
                    priorCursor    = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        Source_AntennaList loadedAntennaList =
                                RFID_Explorer.ExcelExport.ImportAntennaConfig( this.reader, dlg.FileName );

                        rfid.Constants.Result result =
                            loadedAntennaList.store( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );

                        if ( rfid.Constants.Result.OK != result )
                        {
                            throw new Exception( result.ToString( ) );
                        }

                        this.antennaList.Clear( );
                        this.antennaList.AddRange( loadedAntennaList );

                        view.Refresh( );
                    }
                    catch ( Exception e2 )
                    {
                        MessageBox.Show( String.Format( "Error importing antenna settings.\n\nThe import file contains this.errors and cannot be used until corrected.\n\n{0}", e2.Message ), "Invalid import file", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        return;
                    }
                }
                finally
                {
                    if ( priorCursor != null )
                    {
                        Cursor.Current = priorCursor;
                    }
                    this.Capture = false;
                }
            }
        }

        // Add on mods for global sense threshold display and edit

        private uint globalSenseThresholdVal = 0xFFFFF;

        private void updateThreshold( )
        {
            rfid.Constants.Result status = rfid.Constants.Result.OK;

            try
            {
                status = reader.API_AntennaPortGetSenseThreshold( ref globalSenseThresholdVal );
            }
            catch ( Exception )
            {
                status = rfid.Constants.Result.RADIO_FAILURE;

                globalSenseThreshold.Text = "UNKNOWN"; 

                return;
            }
       
            globalSenseThreshold.Text = globalSenseThresholdVal.ToString( );

            globalSenseThreshold.Refresh( );
        }


        private void editGlobalSenseThreshold_Click( object sender, EventArgs e )
        {
            using ( AntennaSenseThresholdEdit dlg = new AntennaSenseThresholdEdit( this.reader, globalSenseThresholdVal) )
            {
                if ( DialogResult.OK == dlg.ShowDialog( ) )
                {
                    this.updateThreshold( ); // for thresh val only
                    
                    view.Refresh( ); // redraw everything ( columns ) - not necessary ?
                }
                dlg.Close( );
            }
        }


        private void RestoreDefaultButton_Click(object sender, EventArgs e)
        {
            this.antennaList.Clear();

            this.antennaList.AddRange(Source_AntennaList.DEFAULT_ANTENNA_LIST);

            rfid.Constants.Result result =
                  this.antennaList.store(LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle);

            if (rfid.Constants.Result.OK != result)
            {
                Console.WriteLine("Error {0} while setting default antenna configurations");
            }

            // Magic numbers - argh - just adding this for 'uniformity'
            // so that everything on the panel

            //Antenna Sense threshold
            globalSenseThresholdVal      = 0xFFFFF;
            rfid.Constants.Result status = rfid.Constants.Result.OK;

            try
            {
                status = reader.API_AntennaPortSetSenseThreshold( globalSenseThresholdVal );
            }
            catch (Exception)
            {
                status = rfid.Constants.Result.RADIO_FAILURE;

                globalSenseThreshold.Text = "UNKNOWN";

                return;
            }

            this.updateThreshold();



            //RfPowerThreshold
            UInt32 oem_rf_threshold = 0;

            numericUpDownRfPowerThreshold.Value = 0x0000001E;
            oem_rf_threshold = (UInt32)numericUpDownRfPowerThreshold.Value;

            //clark not sure. Wait firmware support this function.
            //status = reader.MacWriteOemData( (ushort) enumOEM_ADDR.ENUM_OEM_ADDR_RF_REVPWR_THRESHOLD,
            //                                          oem_rf_threshold                               );


            view.Refresh();
        }


        private void Btn_SetRfPowerThreshold_Click(object sender, EventArgs e)
        {
            //RfPowerThreshold
            UInt32 oem_rf_threshold = 0;

            oem_rf_threshold = (UInt32)numericUpDownRfPowerThreshold.Value;

            //clark not sure. Wait firmware support this function.
            //rfid.Constants.Result status = reader.MacWriteOemData(
            //                                 (ushort) enumOEM_ADDR.ENUM_OEM_ADDR_RF_REVPWR_THRESHOLD,//0x000000AE,
            //                                 oem_rf_threshold                                         );
              
            //clark not sure. Wait firmware support this function.
            //status  = reader.MacReadOemData( (ushort) enumOEM_ADDR.ENUM_OEM_ADDR_RF_REVPWR_THRESHOLD, 
            //                                  ref     oem_rf_threshold                                );
        }


    } // END partial class ConfigureAntenna : UserControl


} // END namespace RFID_Explorer
