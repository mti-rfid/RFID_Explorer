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
 * $Id: ConfigurePostSingulation_Display.cs,v 1.4 2010/01/20 22:21:27 dciampi Exp $
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
using Global;
using System.Collections;




namespace RFID_Explorer
{


    public partial class ConfigurePostSingulation_Display : UserControl
    {

        public int CRITERIA_MINIM
        {
            get { return global::RFID_Explorer.Properties.Settings.Default.MinimumPreSingulation; }
        }

        public int CRITERIA_AVAIL
        {
            get { return global::RFID_Explorer.Properties.Settings.Default.AvailablePreSingulation; }
        }

        public int CRITERIA_MAXIM
        {
            get { return global::RFID_Explorer.Properties.Settings.Default.MaximumPreSingulation; }
        }


        public static readonly int VIEW_MODE = 0;
        public static readonly int EDIT_MODE = 1;
        


        LakeChabotReader reader;        

        SingulationCriteria singulationCriteria;

        int mode;

        ConfigurePostSingulation_Edit dlg;  // frame ( form ) for containing edit mode panel




        public ConfigurePostSingulation_Display( )
            :
            base( )
        {
            InitializeComponent( );

            this.singulationCriteria = new SingulationCriteria( );
            this.singulationCriteria.pCriteria = new SingulationCriterion[ CRITERIA_MAXIM ];

            for( int index = 0; index < CRITERIA_MAXIM; ++ index )
            {
                this.singulationCriteria.pCriteria[ index ] = new SingulationCriterion( );
            }

            this.maskMatch.Items.Add( "INVERSE" );
            this.maskMatch.Items.Add( "REGULAR" );

            // active criteria value set to 1 in props to force flip 
            // view ( invis ) of center panel if first pass shows now 
            // criteria active...

            Mode = VIEW_MODE; // always start in view mode...


            //Set memoryBankOffset Max
            memoryBankOffset.Maximum = ValueLimit.l8K6C_POST_MATCH_CRITERIA_OFFSET_MAX;
            
        }



        public ConfigurePostSingulation_Display( LakeChabotReader reader )
            :
            this( )
        {
            this.setReader( reader );

            this.loadButton_Click( this.loadButton, null );
        }


        public ConfigurePostSingulation_Display( SingulationCriteria criteria )
            :
            this( )
        {
            this.setSource( criteria );
        }
        

        public void setReader( LakeChabotReader reader )
        {
            this.reader = reader;            
        }


        public void setSource( SingulationCriteria criteria )
        {
            this.singulationCriteria = criteria;

            // to trigger redraw etc. events... wont occur if values
            // are actually equal...

            if ( this.activeCriteria.Value == criteria.countCriteria )
            {
                activeCriteria_ValueChanged( null, null );
            }
            else
            {
                this.activeCriteria.Value = criteria.countCriteria;
            }
        }


        public int Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;

                Boolean flag = this.mode == VIEW_MODE ? false : true;

                activeCriteria.Enabled = flag;

                maskMatch.Enabled = flag;
                memoryBankOffset.Enabled = flag;
                maskBitCount.Enabled = flag;

                this.maskByte_0.Enabled = flag;
                this.maskByte_1.Enabled = flag;
                this.maskByte_2.Enabled = flag;
                this.maskByte_3.Enabled = flag;
                this.maskByte_4.Enabled = flag;
                this.maskByte_5.Enabled = flag;
                this.maskByte_6.Enabled = flag;
                this.maskByte_7.Enabled = flag;
                this.maskByte_8.Enabled = flag;
                this.maskByte_9.Enabled = flag;
                this.maskByte_10.Enabled = flag;
                this.maskByte_11.Enabled = flag;
                this.maskByte_12.Enabled = flag;
                this.maskByte_13.Enabled = flag;
                this.maskByte_14.Enabled = flag;
                this.maskByte_15.Enabled = flag;
                this.maskByte_16.Enabled = flag;
                this.maskByte_17.Enabled = flag;
                this.maskByte_18.Enabled = flag;
                this.maskByte_19.Enabled = flag;
                this.maskByte_20.Enabled = flag;
                this.maskByte_21.Enabled = flag;
                this.maskByte_22.Enabled = flag;
                this.maskByte_23.Enabled = flag;
                this.maskByte_24.Enabled = flag;
                this.maskByte_25.Enabled = flag;
                this.maskByte_26.Enabled = flag;
                this.maskByte_27.Enabled = flag;
                this.maskByte_28.Enabled = flag;
                this.maskByte_29.Enabled = flag;
                this.maskByte_30.Enabled = flag;
                this.maskByte_31.Enabled = flag;
                this.maskByte_32.Enabled = flag;
                this.maskByte_33.Enabled = flag;
                this.maskByte_34.Enabled = flag;
                this.maskByte_35.Enabled = flag;
                this.maskByte_36.Enabled = flag;
                this.maskByte_37.Enabled = flag;
                this.maskByte_38.Enabled = flag;
                this.maskByte_39.Enabled = flag;
                this.maskByte_40.Enabled = flag;
                this.maskByte_41.Enabled = flag;
                this.maskByte_42.Enabled = flag;
                this.maskByte_43.Enabled = flag;
                this.maskByte_44.Enabled = flag;
                this.maskByte_45.Enabled = flag;
                this.maskByte_46.Enabled = flag;
                this.maskByte_47.Enabled = flag;
                this.maskByte_48.Enabled = flag;
                this.maskByte_49.Enabled = flag;
                this.maskByte_50.Enabled = flag;
                this.maskByte_51.Enabled = flag;
                this.maskByte_52.Enabled = flag;
                this.maskByte_53.Enabled = flag;
                this.maskByte_54.Enabled = flag;
                this.maskByte_55.Enabled = flag;
                this.maskByte_56.Enabled = flag;
                this.maskByte_57.Enabled = flag;
                this.maskByte_58.Enabled = flag;
                this.maskByte_59.Enabled = flag;
                this.maskByte_60.Enabled = flag;
                this.maskByte_61.Enabled = flag;

                if ( VIEW_MODE == this.mode )
                {
                    editButton.Show( );
                    loadButton.Show( );
                    cancelButton.Hide( );
                    saveButton.Hide( );
                }
                else
                {
                    editButton.Hide( );
                    loadButton.Hide( );
                    cancelButton.Show( );
                    saveButton.Show( );
                }

            }
        }


        public void displayValues( )
        {
            if ( 0 < this.activeCriteria.Value )
            {
                this.memoryBankOffset.Value =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.offset;

                this.maskBitCount.Value =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.count;

                this.maskMatch.SelectedIndex =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].match
                    == 0 ? 0 : 1;


                // FUGLY - but Windows Designer keeps destroying the interface
                // and code everytime I turn these into an array...

                this.maskByte_0.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 0 ].ToString( "X" );

                this.maskByte_1.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 1 ].ToString( "X" );

                this.maskByte_2.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 2 ].ToString( "X" );

                this.maskByte_3.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 3 ].ToString( "X" );

                this.maskByte_4.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 4 ].ToString( "X" );

                this.maskByte_5.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 5 ].ToString( "X" );

                this.maskByte_6.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 6 ].ToString( "X" );

                this.maskByte_7.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 7 ].ToString( "X" );

                this.maskByte_8.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 8 ].ToString( "X" );

                this.maskByte_9.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 9 ].ToString( "X" );

                this.maskByte_10.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 10 ].ToString( "X" );

                this.maskByte_11.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 11 ].ToString( "X" );

                this.maskByte_12.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 12 ].ToString( "X" );

                this.maskByte_13.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 13 ].ToString( "X" );

                this.maskByte_14.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 14 ].ToString( "X" );

                this.maskByte_15.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 15 ].ToString( "X" );

                this.maskByte_16.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 16 ].ToString( "X" );

                this.maskByte_17.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 17 ].ToString( "X" );

                this.maskByte_18.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 18 ].ToString( "X" );

                this.maskByte_19.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 19 ].ToString( "X" );

                this.maskByte_20.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 20 ].ToString( "X" );

                this.maskByte_21.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 21 ].ToString( "X" );

                this.maskByte_22.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 22 ].ToString( "X" );

                this.maskByte_23.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 23 ].ToString( "X" );

                this.maskByte_24.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 24 ].ToString( "X" );

                this.maskByte_25.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 25 ].ToString( "X" );

                this.maskByte_26.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 26 ].ToString( "X" );

                this.maskByte_27.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 27 ].ToString( "X" );

                this.maskByte_28.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 28 ].ToString( "X" );

                this.maskByte_29.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 29 ].ToString( "X" );

                this.maskByte_30.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 30 ].ToString( "X" );

                this.maskByte_31.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 31 ].ToString( "X" );

                this.maskByte_32.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 32 ].ToString( "X" );

                this.maskByte_33.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 33 ].ToString( "X" );

                this.maskByte_34.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 34 ].ToString( "X" );

                this.maskByte_35.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 35 ].ToString( "X" );

                this.maskByte_36.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 36 ].ToString( "X" );

                this.maskByte_37.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 37 ].ToString( "X" );

                this.maskByte_38.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 38 ].ToString( "X" );

                this.maskByte_39.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 39 ].ToString( "X" );

                this.maskByte_40.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 40 ].ToString( "X" );

                this.maskByte_41.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 41 ].ToString( "X" );

                this.maskByte_42.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 42 ].ToString( "X" );

                this.maskByte_43.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 43 ].ToString( "X" );

                this.maskByte_44.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 44 ].ToString( "X" );

                this.maskByte_45.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 45 ].ToString( "X" );

                this.maskByte_46.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 46 ].ToString( "X" );

                this.maskByte_47.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 47 ].ToString( "X" );

                this.maskByte_48.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 48 ].ToString( "X" );

                this.maskByte_49.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 49 ].ToString( "X" );

                this.maskByte_50.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 50 ].ToString( "X" );

                this.maskByte_51.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 51 ].ToString( "X" );

                this.maskByte_52.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 52 ].ToString( "X" );

                this.maskByte_53.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 53 ].ToString( "X" );

                this.maskByte_54.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 54 ].ToString( "X" );

                this.maskByte_55.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 55 ].ToString( "X" );

                this.maskByte_56.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 56 ].ToString( "X" );

                this.maskByte_57.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 57 ].ToString( "X" );

                this.maskByte_58.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 58 ].ToString( "X" );

                this.maskByte_59.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 59 ].ToString( "X" );

                this.maskByte_60.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 60 ].ToString( "X" );

                this.maskByte_61.Text =
                    this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 61 ].ToString( "X" );
            }

            this.Refresh( );
        }



        private void displayCriterion_ValueChanged( object sender, EventArgs e )
        {
            this.displayValues( );
        }

        private void activeCriteria_ValueChanged( object sender, EventArgs e )
        {
            this.singulationCriteria.countCriteria = ( uint ) this.activeCriteria.Value;

            if (0 == this.activeCriteria.Value)
            {
                this.displayCriterionLabel.Hide();
                this.displayCriterion.Hide();
                this.criterionParamPanel.Hide();
            }
            else
            {
                this.displayCriterionLabel.Show();
                this.displayCriterion.Show();
                this.criterionParamPanel.Show();

                if (mode == EDIT_MODE)
                {
                    this.saveButton.Show();
                }

                this.displayCriterion.Minimum = 1;
                this.displayCriterion.Maximum = activeCriteria.Value;

                if (this.displayCriterion.Value < this.displayCriterion.Minimum)
                {
                    this.displayCriterion.Value = this.displayCriterion.Minimum;
                }
                else if (this.displayCriterion.Value > this.displayCriterion.Maximum)
                {
                    this.displayCriterion.Value = this.displayCriterion.Maximum;
                }
            }

            this.displayValues( );
        }



        private void memoryBankOffset_ValueChanged( object sender, EventArgs e )
        {
            this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.offset =
                    ( UInt32 ) this.memoryBankOffset.Value;
        }

        private void maskBitCount_ValueChanged( object sender, EventArgs e )
        {
            this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.count =
                    ( UInt32 ) this.maskBitCount.Value;
        }

        private void maskMatch_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].match =
                ( byte ) this.maskMatch.SelectedIndex;
        }



        private void maskByte_TextChanged( object sender, EventArgs e )
        {
            this.singulationCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask
                [
                    ( ( Widgets.HexNumberTextBox ) sender ).GroupIndex
                ] =
            (
                0 == ( ( Widgets.HexNumberTextBox ) sender ).Text.Length ?
                ( Byte ) 0 :
                Byte.Parse
                (
                    ( ( Widgets.HexNumberTextBox ) sender ).Text,
                    System.Globalization.NumberStyles.HexNumber
                )
            );
        }



        private void editButton_Click( object sender, EventArgs e )
        {
            dlg = new ConfigurePostSingulation_Edit( this.reader, this.singulationCriteria );

            dlg.ShowDialog( );

            // cheating and doing re-load after modal dlg closes so restore
            // base info if a cancel occured or update if save occured...

            this.loadButton_Click( null, null );
        }


        private void loadButton_Click( object sender, EventArgs e )
        {
            rfid.Structures.SingulationCriteria retrievedCriteria
                = new rfid.Structures.SingulationCriteria( );

            Result result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetPostMatchCriteria
                (
                    ref retrievedCriteria
                );

            if ( Result.OK == result )
            {
                // Copy over incoming criteria...

                this.singulationCriteria.countCriteria = retrievedCriteria.countCriteria;                

                // Ref copy since safe creation in managed lib now...

                for ( int index = 0; index < retrievedCriteria.countCriteria; ++index )
                {
                    this.singulationCriteria.pCriteria[ index ] = retrievedCriteria.pCriteria[ index ];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria

                for ( uint index = this.singulationCriteria.countCriteria; index < CRITERIA_MAXIM; ++index )
                {
                    this.singulationCriteria.pCriteria[ index ] =
                        new rfid.Structures.SingulationCriterion( );
                }

                this.activeCriteria.Value = this.singulationCriteria.countCriteria;

                this.displayValues( );
            }
            else
            {
                System.Windows.Forms.MessageBox.Show( "Post Singulation Criteria load error" );
            }

        }




        // Should only see this event if in edit mode and in a form
        // container, otherwise the save button is hidden...

        private void saveButton_Click( object sender, EventArgs e )
        {
            // MUST BE CONTAINED in Form

            SingulationCriteria criteria = new SingulationCriteria( );

            criteria.countCriteria = this.singulationCriteria.countCriteria;

            criteria.pCriteria = new SingulationCriterion[ criteria.countCriteria ];

            for ( int index = 0; index < criteria.countCriteria; ++index )
            {
                criteria.pCriteria[ index ] = this.singulationCriteria.pCriteria[ index ];
            }

            Result result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetPostMatchCriteria
                (
                    this.singulationCriteria,
                    0 // currently not used...
                );

            if ( Result.OK == result )
            {
                // triggered only when in enclosing frame ( form )

                ( ( Form ) this.Parent ).Close( );
            }
            else
            {
                // ERR
                MessageBox.Show
                    (
                        "Reader Error.\n\nThe Reader was unable to configure the specified settings.\n\nThe follow error occurred: " + result.ToString( ),
                        "Configure Post Singulation Criteria Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
            }
        }



        // Should only see this event if in edit mode and in a form
        // container, otherwise the cancel button is hidden...

        private void cancelButton_Click( object sender, EventArgs e )
        {
            // MUST BE CONTAINED in Form

            ( ( Form ) this.Parent ).DialogResult = DialogResult.Cancel;
        }




    } // End partial class ConfigurePostSingulation_Display


} // End namespace RFID_Explorer
