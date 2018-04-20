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
 * $Id: ConfigureTrouble.cs,v 1.6 2009/12/30 23:05:55 dciampi Exp $
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
    public partial class ConfigureTroubleControl : UserControl
    {
        private class CarrierWaveObject
        {
            LakeChabotReader.CarrierWaveValue _state;
            public static CarrierWaveObject[ ] States;

            static CarrierWaveObject( )
            {
                List<CarrierWaveObject> tempStates = new List<CarrierWaveObject>( );
                foreach ( object var in Enum.GetValues( typeof( LakeChabotReader.CarrierWaveValue ) ) )
                {
                    LakeChabotReader.CarrierWaveValue state = ( LakeChabotReader.CarrierWaveValue ) var;
                    if ( state != LakeChabotReader.CarrierWaveValue.Unknown )
                    {
                        tempStates.Add( new CarrierWaveObject( state ) );
                    }
                }
                States = tempStates.ToArray( );
            }

            public static CarrierWaveObject GetCarrierWaveObject( LakeChabotReader.CarrierWaveValue state )
            {
                foreach ( CarrierWaveObject s in States )
                {
                    if ( s.State == state ) return s;
                }
                return null;
            }


            public CarrierWaveObject( LakeChabotReader.CarrierWaveValue state )
            {
                _state = state;
            }


            public override string ToString( )
            {
                return String.Format( "Carrier Wave {0}", _state );
            }

            public LakeChabotReader.CarrierWaveValue State
            {
                get { return _state; }
            }
        }



        private LakeChabotReader _reader = null;
        private Timer _timer = null;
        private MacErrorList _errorList = null;
        private uint _errorCode = uint.MaxValue;
        private uint _lastErrorCode = uint.MaxValue;

        private Panel errorPanel = new Panel( );
        private TextBox errorTextBox = new TextBox( );


        public ConfigureTroubleControl( LakeChabotReader reader )
        {
            if ( reader == null )
                throw new ArgumentNullException( "reader", "Null reader passed to ConfigureGeneral CTOR()" );

            if ( reader.Mode != rfidReader.OperationMode.BoundToReader )
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureGeneral()" );

            InitializeComponent( );

            _reader = reader;

            _timer = new Timer( );
            _timer.Interval = 5000;
            _timer.Tick += new EventHandler( timer_Tick );

            _errorList = new MacErrorList( );




            // errorPanel
            errorPanel.BackColor = System.Drawing.Color.Silver;
            errorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            errorPanel.Name = "errorPanel";
            errorPanel.Size = new System.Drawing.Size( 227, 114 );
            errorPanel.TabIndex = 15;
            errorPanel.Visible = false;


            // errorTextBox
            errorTextBox.BackColor = System.Drawing.SystemColors.Info;
            errorTextBox.Location = new System.Drawing.Point( -1, -1 );
            errorTextBox.Multiline = true;
            errorTextBox.Name = "errorTextBox";
            errorTextBox.Size = new System.Drawing.Size( 226, 113 );
            errorTextBox.TabIndex = 0;


            errorPanel.Controls.Add( this.errorTextBox );
            this.Controls.Add( this.errorPanel );
        }


        void timer_Tick( object sender, EventArgs e )
        {
            _timer.Stop( );
            statusTextBox.Visible = false;
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );


        }

        protected override void OnVisibleChanged( EventArgs e )
        {
            base.OnVisibleChanged( e );

            statusTextBox.Text = "";
            if ( this.Visible )
            {
                rfid.Constants.Result result = rfid.Constants.Result.NOT_INITIALIZED;

                errorCodeTextBox.Text = "";
                lastErrorCodeTextBox.Text = "";
                
                try
                {
                    result = Reader.MacGetError(out _errorCode, out _lastErrorCode);
                    if (result == rfid.Constants.Result.OK)
                    {
                        errorCodeTextBox.Text = String.Format(" 0x{0:X}", _errorCode);
                        lastErrorCodeTextBox.Text = String.Format(" 0x{0:X}", _lastErrorCode);
                    }
                    else
                    {
                        errorCodeTextBox.Text = "Reader Error";
                        lastErrorCodeTextBox.Text = "Reader Error";
                        statusTextBox.ForeColor = Color.Red;
                        statusTextBox.Text = "Error Code Read Error:" + result;
                        statusTextBox.Visible = true;
                        statusTextBox.Refresh();
                    }
                }
                catch ( rfidReaderException exp )
                {
                    errorCodeTextBox.Text = "Reader Error";
                    lastErrorCodeTextBox.Text = "Reader Error";
                    statusTextBox.ForeColor = Color.Red;
                    statusTextBox.Text = "Error Code Read Error:" + exp.Message;
                    statusTextBox.Visible = true;
                    statusTextBox.Refresh( );
                }

                if ( _errorCode > 0 && _errorCode < uint.MaxValue )
                {
                    infoPictureBox.Visible = true;
                }

                if (_lastErrorCode > 0 && _lastErrorCode < uint.MaxValue)
                {
                    lastInfoPictureBox.Visible = true;
                }
            }
        }



        public LakeChabotReader Reader
        {
            get { return _reader; }
        }

        private void errorClearButton_Click( object sender, EventArgs e )
        {
            statusTextBox.Visible = false;

            rfid.Constants.Result result = rfid.Constants.Result.OK;
            try
            {
                result = Reader.MacClearError( );

                if ( rfid.Constants.Result.OK != result )
                {
                    statusTextBox.ForeColor = System.Drawing.Color.Red;
                    statusTextBox.Text = "Clear Error: " + result.ToString( );
                }
                else
                {
                    try
                    {
                        errorCodeTextBox.Text = "";
                        //clark 2011.5.10 Doesn't recommend to get data from OEM directly.
                        //_errorCode = Reader.FirmwareErrorCode;
                        Reader.MacGetError(out _errorCode, out _lastErrorCode);
                        errorCodeTextBox.Text = String.Format( " 0x{0:X}", _errorCode );
                        if ( _errorCode == 0 )
                        {
                            statusTextBox.ForeColor = System.Drawing.Color.Green;
                            statusTextBox.Text = "Successful Error Clear";
                            infoPictureBox.Visible = false;
                        }
                        else
                        {
                            statusTextBox.ForeColor = System.Drawing.Color.Red;
                            statusTextBox.Text = "Unable to clear error.";
                            infoPictureBox.Visible = false;
                        }
                    }
                    catch ( rfidReaderException exp )
                    {
                        statusTextBox.ForeColor = System.Drawing.Color.Red;
                        statusTextBox.Text = "Clear Error: " + exp.Message;
                    }
                }
            }
            catch ( Exception exp )
            {
                statusTextBox.ForeColor = System.Drawing.Color.Red;
                statusTextBox.Text = "Clear Error: " + exp.Message;
            }

            statusTextBox.Visible = true;
            statusTextBox.Refresh( );
            _timer.Stop( );
            _timer.Start( );
        }


        private void resetButton_Click( object sender, EventArgs e )
        {
          
            statusTextBox.Visible = false;
            Cursor old = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                rfid.Constants.Result result = Reader.MacReset( );
                if ( rfid.Constants.Result.OK == result )
                {
                    statusTextBox.ForeColor = System.Drawing.Color.Green;
                    statusTextBox.Text = "Firmware Reset Successful.";
                    
                    // Force Application Close
                    MessageBox.Show("Explorer will be closed.  Restart Explorer to control reader.",
                                    "Reader - Reset",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                }
                else
                {
                    statusTextBox.ForeColor = System.Drawing.Color.Red;
                    statusTextBox.Text = "Firmware Reset Error: " + result.ToString( );
                }
            }
            catch ( Exception exp )
            {
                statusTextBox.ForeColor = System.Drawing.Color.Red;
                statusTextBox.Text = exp.Message;
            }
            statusTextBox.Visible = true;
            statusTextBox.Refresh( );
            Cursor.Current = old;
            _timer.Stop( );
            _timer.Start( );
        }



        private void infoPictureBox_MouseHover( object sender, EventArgs e )
        {
            Point where = errorCodeTextBox.Location;
            where.Offset( 0, errorCodeTextBox.Height + 3 );
            where = errorCodeTextBox.Parent.PointToScreen( where );
            errorPanel.Location = PointToClient( where );
            errorPanel.BringToFront( );
            errorPanel.Visible = true;
            errorTextBox.Text = "";
            try
            {
                errorTextBox.Text = _errorList[ _errorCode ].Description;
            }
            catch ( Exception )
            {
                errorTextBox.Text = "No information available.";
            }
        }


        private void infoPictureBox_MouseLeave( object sender, EventArgs e )
        {
            errorPanel.Visible = false;
        }

        private void lastInfoPictureBox_MouseHover(object sender, EventArgs e)
        {
            Point where = lastErrorCodeTextBox.Location;
            where.Offset(0, lastErrorCodeTextBox.Height + 3);
            where = lastErrorCodeTextBox.Parent.PointToScreen(where);
            errorPanel.Location = PointToClient(where);
            errorPanel.BringToFront();
            errorPanel.Visible = true;
            errorTextBox.Text = "";
            try
            {
                errorTextBox.Text = _errorList[_lastErrorCode].Description;
            }
            catch (Exception)
            {
                errorTextBox.Text = "No information available.";
            }
        }


        private void lastInfoPictureBox_MouseLeave(object sender, EventArgs e)
        {
            errorPanel.Visible = false;
        }

    }


}
