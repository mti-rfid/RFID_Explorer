using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace RFID_Explorer
{

    namespace Widgets
    {

        class HexNumberTextBox : TextBox
        {
            protected static ToolTip Tip = new ToolTip();

            protected static NumericStringConverter BitStringCreator = new NumericStringConverter();


            public HexNumberTextBox() : base()
            {
                InitializeComponent();

                this.CharacterCasing = CharacterCasing.Upper;
            }


            private void InitializeComponent()
            {
                // NOP - pretending VS designer support
            }


            private Boolean useToolTip = true;

            public Boolean UseToolTip
            {
                get
                {
                    return this.useToolTip;
                }
                set
                {
                    this.useToolTip = value;
                }
            }


            // These have index since normally exist as a cluster
            // with each component working as an array index...

            public Int32 groupIndex = 0;

            public Int32 GroupIndex
            {
                get
                {
                    return this.groupIndex;
                }
                set
                {
                    this.groupIndex = value;
                }
            }


            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);

                if (!Char.IsDigit(e.KeyChar))
                {
                    if
                    ( 
                       !(
                          ('A' <= e.KeyChar && 'F' >= e.KeyChar) ||
                          ('a' <= e.KeyChar && 'f' >= e.KeyChar)
                        )
                    )
                    {
                        if (!Char.IsControl(e.KeyChar))
                        {
                            Console.Beep();
                            
                            e.Handled = true;
                        }
                    }
                }                
            }

            protected override void OnTextChanged( EventArgs e )
            {
                base.OnTextChanged( e );
   
                // TODO: fix so works with any max len - currently
                // restricted to two bytes with tips turned on...

                if ( true == this.useToolTip )
                {
                    HexNumberTextBox.Tip.SetToolTip
                    (
                        this,
                        String.Format
                        (
                            HexNumberTextBox.BitStringCreator,
                            "{0:B}",
                            Byte.Parse
                            (
                                this.Text.Length == 0 ? "0" : this.Text,
                                System.Globalization.NumberStyles.HexNumber
                            )
                        )
                    );
                }

                if ( this.Text.Length == this.MaxLength )
                {
                    SendKeys.Send("{TAB}");
                }
            }


        } // END class HexNumberTextBox


    } // END namespace Widgets


} // END namespace RFID_Explorer
