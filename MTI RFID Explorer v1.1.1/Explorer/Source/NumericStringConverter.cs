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

        public class NumericStringConverter
            : 
            IFormatProvider,
            ICustomFormatter
        {


            public object GetFormat ( Type service )
            {
                // Always return self - could do custom but...
                // not required here... 

                if ( typeof( ICustomFormatter ) == service )
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }


            public String Format( String format, object arg, IFormatProvider provider ) 
            {
                if ( null == format || 0 == format.Length )
                {
                    return String.Format( "{0}", arg );
                }

                // TODO: figure out better way of calculating incoming type
                // bit width etc. instead of if elsing to death... but compiler
                // keeps complaining about 'explicit conversions exist' when
                // attempting to use generic converters (?!)

                int    argBits;
                UInt64 argConv;

                if (arg is Boolean)
                {
                    argBits = sizeof(Boolean);
                    argConv = (UInt64)(((Boolean)arg) == false ? 0 : 1);
                }
                else if (arg is Byte)
                {
                    argBits = sizeof(Byte);
                    argConv = (UInt64)(Byte)arg;
                }
                else if (arg is Int16)
                {
                    argBits = sizeof(Int16);
                    argConv = (UInt64)(Int16)arg;
                }
                else if (arg is UInt16)
                {
                    argBits = sizeof(UInt16);
                    argConv = (UInt64)(UInt16)arg;
                }
                else if (arg is Int32)
                {
                    argBits = sizeof(Int32);
                    argConv = (UInt64)(Int32)arg;
                }
                else if (arg is UInt32)
                {
                    argBits = sizeof(UInt32);
                    argConv = (UInt64)(UInt32)arg;
                }
                else if (arg is Int64)
                {
                    argBits = sizeof(Int64);
                    argConv = (UInt64)(Int64)arg;
                }
                else if (arg is UInt64)
                {
                    argBits = sizeof(UInt64);
                    argConv = (UInt64)arg;
                }
                else
                {
                    if (arg is IFormattable)
                    {
                        return ((IFormattable)arg).ToString(format, provider);
                    }
                    else
                    {
                        return arg.ToString();
                    }
                }

                argBits <<= 3;

                String retVal = "";

                switch( format[ 0 ] )
                {
                    case 'b' :
                    case 'B' :
                    {
                        StringBuilder sb = new StringBuilder( argBits + ( ( argBits - 1 ) >> 2 ) );

                        for ( ; 0 < argBits; )
                        {
                            sb.Insert( 0, ( Char ) ( '0' + ( argConv & 0x01 ) ) );

                            -- argBits;
 
                            if ( 0 != argBits )
                            {
                                if ( 0 == ( argBits % 4 ) )
                                {
                                    sb.Insert( 0, ' ' );
                                }

                            }

                            argConv >>= 1;
                        }

                        retVal = sb.ToString( );
                    }
                    break;

                    default:
                    {
                        if ( arg is IFormattable )
                        {
                            retVal = ( ( IFormattable ) arg ).ToString( format, provider );
                        }
                        else
                        {
                            retVal = arg.ToString( );
                        }
                        
                    }
                    break;
                }

                return retVal;
 
            }


        } // END class NumericStringConverter


    } // END namespace Widgets


} // END namespace RFID_Explorer
