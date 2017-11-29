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
 * $Id: ConfigureAlgorithm_Display.cs,v 1.9 2010/01/20 22:21:27 dciampi Exp $
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

using rfid.Structures;
using rfid.Constants;

using System.Collections;



namespace RFID_Explorer
{

    public partial class ConfigureAlgorithm_Display : UserControl
    {

        private LakeChabotReader reader = null;

        private Source_QueryParms parmsMaster; // used in view mode
        private Source_QueryParms parmsActive; // used in edit mode

        Boolean masterEnabled;


        

        // This is so the widget can be used inside a form window
        // but ( warning ) requires setReader and setSource calls
        // prior to use of other methods or >> BOOM <<

        public ConfigureAlgorithm_Display( )
        {
            InitializeComponent( );

            this.parmsActive = new Source_QueryParms( );

            this.MasterEnabled = false;
            
            foreach (SingulationAlgorithm item in Enum.GetValues(typeof(SingulationAlgorithm)))
            {
                this.activeAlgorithm.Items.Add(item);
            }
            this.activeAlgorithm.Items.Remove(SingulationAlgorithm.UNKNOWN);

            this.activeAlgorithm.DropDownStyle = ComboBoxStyle.DropDown;
            this.commonParmsPanel.VisibleChanged += new System.EventHandler( this.commonParmsPanel_VisibleChanged );

            foreach (Selected item in Enum.GetValues(typeof(Selected)))
            {
                this.tagGroupSelected.Items.Add(item);
            }
            this.tagGroupSelected.Items.Remove(Selected.UNKNOWN);
            //clark 2011.7.8 Doesn't support reseerved.
            this.tagGroupSelected.Items.Remove(Selected.RESERVED);

            foreach (Session item in Enum.GetValues(typeof(Session)))
            {
                this.tagGroupSession.Items.Add(item);
            }
            this.tagGroupSession.Items.Remove(Session.UNKNOWN);

            foreach (SessionTarget item in Enum.GetValues(typeof(SessionTarget)))
            {
                this.tagGroupTarget.Items.Add(item);
            }
            this.tagGroupTarget.Items.Remove(SessionTarget.UNKNOWN);
        }
        

        public ConfigureAlgorithm_Display( LakeChabotReader reader )
            :
            this( )
        {
            this.setReader( reader );

            this.setSource( new Source_QueryParms( ) );

            this.retrieveData( );
        }
            

        // This MUST be called prior to any accessors etc. and
        // each and every call here will reset all data causing
        // a re-read of info from the radio...

        public void setReader( LakeChabotReader reader )
        {
            if ( null == reader )
            {
                throw new ArgumentNullException( "reader", "Null reader passed to Algorithm_Display( )" );
            }

            if ( rfidReader.OperationMode.BoundToReader != reader.Mode )
            {
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to Algorithm_Display( )" );
            }

            this.reader = reader;
        }


        // Set the utilized parameters ( by ref only ) and reset
        // all bindings to utilize it...

        public void setSource( Source_QueryParms parms )
        {
            this.activeAlgorithm.DataBindings.Clear( );
            this.tagGroupSelected.DataBindings.Clear( );
            this.tagGroupSession.DataBindings.Clear( );
            this.tagGroupTarget.DataBindings.Clear( );

            this.parmsMaster = parms;

            this.activeAlgorithm.DataBindings.Add ( "SelectedItem", this.parmsMaster, "SingulationAlgorithm" );
            this.tagGroupSelected.DataBindings.Add( "SelectedItem", this.parmsMaster, "TagGroupSelected" );
            this.tagGroupSession.DataBindings.Add( "SelectedItem", this.parmsMaster, "TagGroupSession" );
            this.tagGroupTarget.DataBindings.Add( "SelectedItem", this.parmsMaster, "TagGroupTarget" );
        }


        // This override is REQUIRED due to the 'quick set' "feature"
        // in the configure settings panel...

        protected override void OnVisibleChanged( EventArgs e )
        {
            base.OnVisibleChanged( e );

            // cludge to handle edit window event mis-ordering that
            // seems to occur about 50% or so of the time...

            if ( null != this.reader )
            {
                this.retrieveData( );
            }
        }


        // For regular retrieval of current algorithm & settings
        // utilizing existing library function

        public void retrieveData( )
        {
            Result result = this.parmsMaster.load
                (
                    LakeChabotReader.MANAGED_ACCESS,
                    this.reader.ReaderHandle
                );

            if ( Result.OK == result )
            {
                setSource( this.parmsMaster );
                displayData( );
            }
            else
            {
                // ERR
                MessageBox.Show
                    (
                        "Reader Error.\n\nThe Reader was unable to load the current algorithm settings.\n\nThe follow error occurred: " + result.ToString( ),
                        "Configure Algorithm Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
            }
        }


        // For retrieving params for a specific algorithm bypassing
        // the normal lib function(s) - none exist for performing 
        // this action... on success updates gui with new info...

        public void retrieveData( SingulationAlgorithm algorithm )
        {
            Result result = this.parmsMaster.loadForAlgorithm
                ( 
                    LakeChabotReader.MANAGED_ACCESS,
                    this.reader.ReaderHandle,
                    algorithm
                );

            if ( Result.OK == result )
            {
                setSource( this.parmsMaster );
                displayData( );
            }
            else
            {
                // ERR
                MessageBox.Show
                    (
                        "Reader Error.\n\nThe Reader was unable to load the current algorithm settings.\n\nThe follow error occurred: " + result.ToString( ),
                        "Configure Algorithm Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
            }
        }


        // Determine and show the appropriate center panel on the
        // gui ( the one showing algorithm specific info )...

        public void displayData( )
        {
            algorithmParms_0.Hide( );
            algorithmParms_1.Hide( );

            switch ( this.parmsMaster.SingulationAlgorithm )
            {
                case SingulationAlgorithm.FIXEDQ:
                    {
                        algorithmParms_0.setSource
                        (
                            ( Source_SingulationParametersFixedQ ) this.parmsMaster.SingulationAlgorithmParameters
                        );
                        ( ( ConfigureAlgorithmParms_0_Display ) algorithmParms_0 ).MasterEnabled = this.MasterEnabled;
                        algorithmParms_0.Show( );
                    }
                    break;

                case SingulationAlgorithm.DYNAMICQ:
                    {
                        algorithmParms_1.setSource
                        (
                            ( Source_SingulationParametersDynamicQ ) this.parmsMaster.SingulationAlgorithmParameters
                        );
                        ( ( ConfigureAlgorithmParms_1_Display ) algorithmParms_1 ).MasterEnabled = this.MasterEnabled;
                        algorithmParms_1.Show( );
                    }
                    break;

                default:

                    // ERR - what type of message ?
                    System.Windows.Forms.MessageBox.Show( "Unknown singulation type in displayData( )" );
                    break;
            };

            this.Refresh( );
        }



        // Mark if we are in master view mode ( MasterEnabled = false )
        // or in edit mode ( MasterEnabled = true ) and configure the
        // appropriate form widgets to be disabled | enabled ...

        public Boolean MasterEnabled
        {
            get
            {
                return this.masterEnabled;
            }

            set
            {
                this.masterEnabled = value;

                if ( ! masterEnabled )
                {
                    activeAlgorithm.Enabled                 = false;

                    tagGroupSelected.Enabled                = false;
                    tagGroupSession.Enabled                 = false;
                    tagGroupTarget.Enabled                  = false;

                    editButton.Show( );
                    loadButton.Show( );

                    submitButton.Hide( );
                    cancelButton.Hide( );
                }
                else
                {
                    activeAlgorithm.Enabled              = true;

                    tagGroupSelected.Enabled             = true;
                    tagGroupSession.Enabled              = true;
                    tagGroupTarget.Enabled               = true;

                    editButton.Hide( );
                    loadButton.Hide( );

                    submitButton.Show( );
                    cancelButton.Show( );
                }

                this.Refresh( );
            }
        }



        // The following is tied to trigger when the common parms panel becomes
        // visible - this appears to correct behaviour for getting ComboBox
        // updates ( where the common panel loading / becoming visible does NOT
        // sync always with its parent control ( this ) loading / becoming visible ).

        private void commonParmsPanel_VisibleChanged( object sender, EventArgs e )
        {
            this.activeAlgorithm.SelectedValueChanged -= new System.EventHandler( this.activeAlgorithm_SelectedIndexChanged );

            this.activeAlgorithm.SelectedValueChanged += new System.EventHandler( this.activeAlgorithm_SelectedIndexChanged );
        }


        // When the active algorithm changes, bypass the library and load the
        // onboard radio info for that register - this load leaves the radio
        // in its original state...

        private void activeAlgorithm_SelectedIndexChanged( object sender, EventArgs e )
        {
            Result result = this.parmsMaster.loadForAlgorithm
                (
                    LakeChabotReader.MANAGED_ACCESS,
                    ( uint ) reader.ReaderHandle,
                    ( SingulationAlgorithm ) ( ( ComboBox ) sender ).SelectedItem
                );

            this.displayData( );
        }


        private void tagGroupSelected_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void tagGroupSession_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void tagGroupTarget_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }


        // Should only see this event in view mode, otherwise the
        // edit button is hidden...

        private void editButton_Click( object sender, EventArgs e )
        {
            // Passing in a copy of the parms in case of cancel or failure so
            // we have a comparison and restore point reference...

            this.parmsActive.Copy( this.parmsMaster );

            using ( ConfigureAlgorithm_Edit dlg = new ConfigureAlgorithm_Edit( this.reader, this.parmsActive  ) )
            {
                if ( DialogResult.OK == dlg.ShowDialog( ) )
                {
                    Result result = this.parmsActive.store
                        (
                            LakeChabotReader.MANAGED_ACCESS,
                            this.reader.ReaderHandle
                        );

  
                    if ( Result.OK == result )
                    {
                        this.parmsMaster.Copy( this.parmsActive );
                        this.setSource( this.parmsMaster );

                        this.displayData( );
                    }
                    else
                    {
                        // ERR
                        MessageBox.Show
                            ( 
                                "Reader Error.\n\nThe Reader was unable to configure the specified settings.\n\nThe follow error occurred: " + result.ToString( ), 
                                "Configure Algorithm Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                    }
                }
                dlg.Close( );
            }
        }

        // Should only see this event in view mode, otherwise the
        // load button is hidden...

        private void loadButton_Click( object sender, EventArgs e )
        {
            this.retrieveData( );
        }



        // Should only see this event if in edit mode and in a form
        // container, otherwise the save button is hidden...

        private void saveButton_Click( object sender, EventArgs e )
        {
            // MUST BE CONTAINED in Form

            ( ( Form ) this.Parent ).DialogResult = DialogResult.OK;
        }

        // Should only see this event if in edit mode and in a form
        // container, otherwise the cancel button is hidden...

        private void cancelButton_Click( object sender, EventArgs e )
        {
            // MUST BE CONTAINED in Form

            ( ( Form ) this.Parent ).DialogResult = DialogResult.Cancel;
        }


    }
    
}
