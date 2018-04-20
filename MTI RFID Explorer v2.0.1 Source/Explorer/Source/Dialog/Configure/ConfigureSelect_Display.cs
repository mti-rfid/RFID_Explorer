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
 * $Id: ConfigureSelect_Display.cs,v 1.7 2010/01/20 22:21:27 dciampi Exp $
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

using System.Runtime.InteropServices;


namespace RFID_Explorer
{


    public partial class ConfigureSelect_Display : UserControl
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

        SelectCriteria selectCriteria;

        int mode;

        ConfigureSelect_Edit dlg;  // frame ( form ) for containing edit mode panel



        public ConfigureSelect_Display( )
            :
            base( )
        {
            InitializeComponent( );

            this.selectCriteria = new SelectCriteria( );
            this.selectCriteria.pCriteria = new SelectCriterion[ CRITERIA_MAXIM ];

            for( int index = 0; index < CRITERIA_MAXIM; ++ index )
            {
                this.selectCriteria.pCriteria[ index ] = new SelectCriterion( );
            }

            foreach (Target item in Enum.GetValues(typeof(Target)))
            {
                this.inventoryTarget.Items.Add(item);
            }
            this.inventoryTarget.Items.Remove(Target.UNKNOWN);

            foreach (MemoryBank item in Enum.GetValues(typeof(MemoryBank)))
            {
                this.memoryBank.Items.Add(item);
            }
            //clark 2011.07.08 Doesn't support reserved,
            this.memoryBank.Items.Remove(MemoryBank.RESERVED);
            this.memoryBank.Items.Remove(MemoryBank.UNKNOWN);

			//Mod FJ for NXP authentication function, 2018-02-02
            foreach (rfid.Constants.Action item in Enum.GetValues(typeof(rfid.Constants.Action))) 
			//foreach (Action item in Enum.GetValues(typeof(Action)))
            {
                this.action.Items.Add(item);
            }
            this.action.Items.Remove(rfid.Constants.Action.UNKNOWN); 
			//this.action.Items.Remove(Action.UNKNOWN);
			//End FJ for NXP authentication function, 2018-02-02

            this.truncation.Items.Add( "DISABLE" ); 
            this.truncation.Items.Add( "ENABLE" );

                    
            // active criteria value set to 1 in props to force flip 
            // view ( invis ) of center panel if first pass shows now 
            // criteria active...

            Mode = VIEW_MODE; // always start in view mode...
        }



        public ConfigureSelect_Display( LakeChabotReader reader )
            :
            this( )
        {
            this.setReader( reader );

            this.loadButton_Click( this.loadButton, null );
        }


        public ConfigureSelect_Display( SelectCriteria criteria )
            :
            this( )
        {
            this.setSource( criteria );
        }
        

        public void setReader( LakeChabotReader reader )
        {
            this.reader = reader;            
        }


        public void setSource( SelectCriteria criteria )
        {
            this.selectCriteria = criteria;

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


        // Enables or disables editability of various components
        // depending on whether in viewing or editing mode...

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

                memoryBank.Enabled = flag;
                memoryBankOffset.Enabled = flag;
                maskBitCount.Enabled = flag;

                inventoryTarget.Enabled = flag;                
                //clark 2011.08.08. not sure. Wait for firmware to support "Enable".
                truncation.Enabled = false;
                //truncation.Enabled = flag;
                action.Enabled = flag;

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
            // This will cause a event trigger to re-align the current
            // display criteria and whether the center panel is hidden
            // or visible...

            if ( 0 < this.activeCriteria.Value )
            {
                if ( (this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].mask.bank == MemoryBank.UNKNOWN) ||
                     (this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].mask.bank == MemoryBank.RESERVED) )
                {
                    this.memoryBank.SelectedIndex = -1;
                }
                else
                {
                    this.memoryBank.SelectedItem = 
                        this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].mask.bank;
                }

                this.memoryBankOffset.Value =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.offset;

                this.maskBitCount.Value =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.count;

                if (this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].action.target == Target.UNKNOWN)
                {
                    this.inventoryTarget.SelectedIndex = -1;
                }
                else
                {
                    this.inventoryTarget.SelectedItem =
                        this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].action.target;
                }

				//Mod FJ for NXP authentication function, 2018-02-02
                if (this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].action.action == rfid.Constants.Action.UNKNOWN)
                //if (this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].action.action == Action.UNKNOWN)
				//End FJ for NXP authentication function, 2018-02-02
				{
                    this.action.SelectedIndex = -1;
                }
                else
                {
                    this.action.SelectedItem =
                        this.selectCriteria.pCriteria[(int)this.displayCriterion.Value - 1].action.action;
                }

                this.truncation.SelectedIndex =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].action.enableTruncate
                    == 0 ? 0 : 1;

                // FUGLY - but Windows Designer keeps destroying the interface
                // and code everytime I turn these into an array...

                this.maskByte_0.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 0 ].ToString( "X" );

                this.maskByte_1.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 1 ].ToString( "X" );

                this.maskByte_2.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 2 ].ToString( "X" );

                this.maskByte_3.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 3 ].ToString( "X" );

                this.maskByte_4.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 4 ].ToString( "X" );

                this.maskByte_5.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 5 ].ToString( "X" );

                this.maskByte_6.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 6 ].ToString( "X" );

                this.maskByte_7.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 7 ].ToString( "X" );

                this.maskByte_8.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 8 ].ToString( "X" );

                this.maskByte_9.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 9 ].ToString( "X" );

                this.maskByte_10.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 10 ].ToString( "X" );

                this.maskByte_11.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 11 ].ToString( "X" );

                this.maskByte_12.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 12 ].ToString( "X" );

                this.maskByte_13.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 13 ].ToString( "X" );

                this.maskByte_14.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 14 ].ToString( "X" );

                this.maskByte_15.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 15 ].ToString( "X" );

                this.maskByte_16.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 16 ].ToString( "X" );

                this.maskByte_17.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 17 ].ToString( "X" );

                this.maskByte_18.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 18 ].ToString( "X" );

                this.maskByte_19.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 19 ].ToString( "X" );

                this.maskByte_20.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 20 ].ToString( "X" );

                this.maskByte_21.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 21 ].ToString( "X" );

                this.maskByte_22.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 22 ].ToString( "X" );

                this.maskByte_23.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 23 ].ToString( "X" );

                this.maskByte_24.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 24 ].ToString( "X" );

                this.maskByte_25.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 25 ].ToString( "X" );

                this.maskByte_26.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 26 ].ToString( "X" );

                this.maskByte_27.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 27 ].ToString( "X" );

                this.maskByte_28.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 28 ].ToString( "X" );

                this.maskByte_29.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 29 ].ToString( "X" );

                this.maskByte_30.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 30 ].ToString( "X" );

                this.maskByte_31.Text =
                    this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask[ 31 ].ToString( "X" );

            }

            this.Refresh( );
        }



        private void displayCriterion_ValueChanged( object sender, EventArgs e )
        {
            // method displayValues does center panel display flipping...

            this.displayValues( );
        }

        protected void activeCriteria_ValueChanged( object sender, EventArgs e )
        {
            this.selectCriteria.countCriteria =( byte ) this.activeCriteria.Value;

            if ( 0 == this.activeCriteria.Value )
            {
                this.displayCriterionLabel.Hide( );
                this.displayCriterion.Hide( );
                this.criterionParamPanel.Hide( );
            }
            else
            {
                this.displayCriterionLabel.Show( );
                this.displayCriterion.Show( );                
                this.criterionParamPanel.Show( );

                if ( mode == EDIT_MODE )
                {
                    this.saveButton.Show( );
                }

                this.displayCriterion.Minimum = 1;
                this.displayCriterion.Maximum = activeCriteria.Value;

                if ( this.displayCriterion.Value < this.displayCriterion.Minimum )
                {
                    this.displayCriterion.Value = this.displayCriterion.Minimum;
                }
                else if ( this.displayCriterion.Value > this.displayCriterion.Maximum )
                {
                    this.displayCriterion.Value = this.displayCriterion.Maximum;
                }
            }

            this.displayValues( );
        }



        private void memoryBank_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( null != this.memoryBank.SelectedItem )
            {
                this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.bank =
                    ( MemoryBank ) this.memoryBank.SelectedItem;
            }
        }

        private void memoryBankOffset_ValueChanged( object sender, EventArgs e )
        {
            this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.offset =
                    ( UInt16 ) this.memoryBankOffset.Value;
        }

        private void maskBitCount_ValueChanged( object sender, EventArgs e )
        {
            this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.count =
                    ( byte ) this.maskBitCount.Value;
        }



        private void inventoryTarget_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( null != this.inventoryTarget.SelectedItem )
            {
                this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].action.target =
                    ( Target ) this.inventoryTarget.SelectedItem;
            }
        }

        private void truncation_SelectedIndexChanged( object sender, EventArgs e )
        {
            // Force Values if Truncation is enabled
            if (this.truncation.SelectedIndex == 1)
            {
                this.memoryBank.SelectedItem = MemoryBank.EPC;
                this.inventoryTarget.SelectedItem = Target.SELECTED;
            }
            
            
            this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].action.enableTruncate =
                ( byte ) this.truncation.SelectedIndex;
        }

        private void action_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( null != this.action.SelectedItem )
            {
                this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].action.action =
                    
					//Mod FJ for NXP authentication function, 2018-02-02
					(rfid.Constants.Action)this.action.SelectedItem;
            		//( Action ) this.action.SelectedItem;
					//End FJ for NXP authentication function, 2018-02-02
			}
        }




        private void maskByte_TextChanged( object sender, EventArgs e )
        {
            this.selectCriteria.pCriteria[ ( int ) this.displayCriterion.Value - 1 ].mask.mask
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
            dlg = new ConfigureSelect_Edit( this.reader, this.selectCriteria );

            dlg.ShowDialog( );

            // cheating and doing re-load after modal dlg closes so restore
            // base info if a cancel occured or update if save occured...

            this.loadButton_Click( null, null );
        }


        private void loadButton_Click( object sender, EventArgs e )
        {
            rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria( );
            Result result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
            if ( Result.OK == result )
            {
                // Copy over incoming criteria...

                this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                // Ref copy since safe creation in managed lib now...

                for ( int index = 0; index < retrievedCriteria.countCriteria; ++index )
                {
                    this.selectCriteria.pCriteria[ index ] = retrievedCriteria.pCriteria[ index ];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria

                for ( uint index = this.selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index )
                {
                    this.selectCriteria.pCriteria[ index ] =
                        new rfid.Structures.SelectCriterion( );
                }

                this.activeCriteria.Value = this.selectCriteria.countCriteria;

                this.displayValues( );
            }
            else
            {
                System.Windows.Forms.MessageBox.Show( "Select Criteria load error" );
            }

        }




        // Should only see this event if in edit mode and in a form
        // container, otherwise the save button is hidden...

        private void saveButton_Click( object sender, EventArgs e )
        {
            SelectCriteria criteria = new SelectCriteria( );

            criteria.countCriteria = this.selectCriteria.countCriteria;

            criteria.pCriteria = new SelectCriterion[ criteria.countCriteria ];

            uint select;
            reader.MacReadRegister(RegisterData.HST_INV_CFG, out select);
            select &= 0xFFFFBFFF;
            select |= (criteria.countCriteria >0) ? (uint)(1 << 14) : 0;
            reader.MacWriteRegister(RegisterData.HST_INV_CFG, select);

            for( int index = 0; index < criteria.countCriteria; ++ index )
            {
                criteria.pCriteria[ index ] = this.selectCriteria.pCriteria[ index ];
            }

            Result result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
    
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
                        "Configure Select Criteria Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
            }
        }

        // Should only see this event if in edit mode and in a form
        // container, otherwise the cancel button is hidden...

        private void cancelButton_Click( object sender, EventArgs e )
        {
            // triggered only when in enclosing frame ( form )

            ( ( Form ) this.Parent ).Close( );
        }

        private void button_actionHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gen2 Select - Tag Response to Action Parameter" + Environment.NewLine + 
                            "Action | Matching | Non Matching" + Environment.NewLine +
                            "000 ASLINVA_DSLINVB | assert SL or inventoried -> A | deassert SL or inventoried -> B " + Environment.NewLine +
                            "001 ASLINVA_NOTHING | assert SL or inventoried -> A | do nothing " + Environment.NewLine +
                            "010 NOTHING_DSLINVB | do nothing | deassert SL or inventoried -> B " + Environment.NewLine +
                            "011 NSLINVS_NOTHING | negate SL or ( A->B, B->A ) | do nothing " + Environment.NewLine +
                            "100 DSLINVB_ASLINVA | deassert SL or inventoried -> B | assert SL or inventoried -> A " + Environment.NewLine +
                            "101 DSLINVB_NOTHING | deassert SL or inventoried -> A | do nothing " + Environment.NewLine +
                            "110 NOTHING_ASLINVA | do nothing | assert SL or inventoried -> A " + Environment.NewLine +
                            "111 NOTHING_NSLINVS | do nothing | negate SL or ( A->B, B->A ) " + Environment.NewLine,
                            "Help - Action",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void button_truncationHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Enabling Truncation will force Memory Bank to EPC and Target to Selected." + Environment.NewLine +
                            "Truncation can only be enabled on the last select criteria.",
                            "Help - Truncation",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

    } // End partial class ConfigureSelectCriteria_Display


} // End namespace RFID_Explorer
