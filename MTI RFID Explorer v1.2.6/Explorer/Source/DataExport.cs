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
 * $Id: DataExport.cs,v 1.7 2009/12/10 22:49:58 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using RFID.RFIDInterface;


namespace RFID_Explorer
{


    public static class ExcelExport
    {
        public const int MaxRowsInExcel = 65536;
        public const int MaxColsInExcel = 256;
        public const int MaxDisplayCharInCol = 1024;
        public const int MaxSheetTabChars = 30;

        public static char[ ] BadSheetNameCharSet;

        static ExcelExport( )
        {
            List<char> badones = new List<char>( 156 );
            for ( int i = 0; i < 31; i++ )
                badones.Add( ( char ) i );

            badones.AddRange( new char[ ] { '*', '/', ':', '?', '[', ']', '\\', '~', '`', } );

            for ( int i = 128; i < 254; i++ )
                badones.Add( ( char ) i );

            BadSheetNameCharSet = badones.ToArray( );
        }


        static string FormatSheetTabName( string s )
        {
            string result = ( string ) s.Clone( );
            result.Trim( );
            result.Replace( "\t", "  " );
            int loc;
            while ( ( loc = result.IndexOfAny( BadSheetNameCharSet ) ) != -1 )
            {
                result = result.Replace( result[ loc ], '-' );
            }

            return result.Substring( 0, Math.Min( result.Length, MaxSheetTabChars ) );
        }

        public static ReportBase ExportData( Object context, IReader reader, BackgroundWorker worker, int refreshMS, string Format, List<GridControl.GridType> dataSources )
        {

            if ( null == context )
            {
                throw new ArgumentNullException( "context" );
            }

            if ( reader == null )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "reader" ) );
            }

            if ( null == worker )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker" ) );
            }

            if ( String.IsNullOrEmpty( Format ) )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "Format" ) );
            }

            if ( dataSources == null )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "dataSources" ) );
            }

            if ( reader.TableResult == TableState.Building || reader.TableResult == TableState.BuildRequired )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new rfidException( rfidErrorCode.TablesAreNotReady, "You must build the post-capture views before exporting data." ) );
            }

            if ( dataSources.Count == 0 )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new ArgumentOutOfRangeException( "dataSources", "dataSources must identify at least one view to export." ) );
            }

            try
            {
                bool dataWasTruncated = false;
                rfidSimpleReport report = new rfidSimpleReport( context, 0 );

                string fileName = RFID_Explorer.Properties.Settings.Default.strExportPath;

                XmlWriterSettings settings = new XmlWriterSettings( );
                settings.Encoding = Encoding.UTF8;
                settings.ConformanceLevel = ConformanceLevel.Document;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.Replace;
                settings.CheckCharacters = true;

                XmlWriter writer = XmlWriter.Create( fileName, settings );

                writer.WriteStartDocument( );
                writer.WriteProcessingInstruction( "mso-application", "progid=\"Excel.Sheet\"" );

                writer.WriteStartElement( "Workbook", "urn:schemas-microsoft-com:office:spreadsheet" );
                writer.WriteAttributeString( "xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet" );
                writer.WriteAttributeString( "xmlns", "x", null, "urn:schemas-microsoft-com:office:excel" );
                writer.WriteAttributeString( "xmlns", "x2", null, "urn:schemas-microsoft-com:office:excel2" );
                writer.WriteAttributeString( "xmlns", "html", null, "http://www.w3.org/TR/REC-html40" );

                // Write the styles
                writer.WriteStartElement( "ss", "Styles", null );

                // Write Default Style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "Default" );
                writer.WriteAttributeString( "ss", "Name", null, "Normal" );

                writer.WriteStartElement( "ss", "Alignment", null );
                writer.WriteAttributeString( "ss", "Horizontal", null, "Center" );
                writer.WriteAttributeString( "ss", "Vertical", null, "Bottom" );
                writer.WriteEndElement( );

                writer.WriteElementString( "ss", "Borders", null, null );

                writer.WriteElementString( "ss", "Font", null, null );

                writer.WriteElementString( "ss", "Interior", null, null );

                writer.WriteElementString( "ss", "NumberFormat", null, null );

                writer.WriteStartElement( "ss", "Protection", null );
                writer.WriteAttributeString( "ss", "Protected", null, "0" );
                writer.WriteEndElement( ); //</Protection>

                writer.WriteEndElement( ); //</Style>
                
                // Write Data Style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "Data" );

                writer.WriteStartElement( "ss", "Alignment", null );
                writer.WriteAttributeString( "ss", "Horizontal", null, "Center" );
                writer.WriteAttributeString( "ss", "Vertical", null, "Bottom" );
                writer.WriteEndElement( );

                writer.WriteElementString( "ss", "Borders", null, null );

                writer.WriteElementString( "ss", "Font", null, null );

                writer.WriteElementString( "ss", "Interior", null, null );

                writer.WriteElementString( "ss", "NumberFormat", null, null );

                writer.WriteStartElement( "ss", "Protection", null );
                //				writer.WriteAttributeString("ss", "Protected", null, "1");
                writer.WriteAttributeString( "ss", "Protected", null, "0" );
                writer.WriteEndElement( ); //</Protection>

                writer.WriteEndElement( ); //</Style>

                // Write Data Left Style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "DataLeft" );

                writer.WriteStartElement( "ss", "Alignment", null );
                writer.WriteAttributeString( "ss", "Horizontal", null, "Left" );
                writer.WriteAttributeString( "ss", "Vertical", null, "Bottom" );
                writer.WriteEndElement( );

                writer.WriteElementString( "ss", "Borders", null, null );

                writer.WriteElementString( "ss", "Font", null, null );

                writer.WriteElementString( "ss", "Interior", null, null );

                writer.WriteElementString( "ss", "NumberFormat", null, null );

                writer.WriteStartElement( "ss", "Protection", null );
                writer.WriteAttributeString( "ss", "Protected", null, "1" );
                //				writer.WriteAttributeString("ss", "Protected", null, "0");
                writer.WriteEndElement( ); //</Protection>

                writer.WriteEndElement( ); //</Style>

                // Write Title Style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "TitleStyle" );


                writer.WriteStartElement( "ss", "Font", null );
                writer.WriteAttributeString( "ss", "Size", null, "14" );
                writer.WriteEndElement( ); // </Font>

                writer.WriteEndElement( ); //</Style>


                // Write sub-title style SubtitleStyle
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Font", null );
                writer.WriteAttributeString( "ss", "Size", null, "12" );
                writer.WriteAttributeString( "ss", "Bold", null, "1" );

                writer.WriteEndElement( ); // </Font>

                writer.WriteEndElement( ); //</Style>

                // Write the SummaryCaption Style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Alignment", null );
                writer.WriteAttributeString( "ss", "Horizontal", null, "Right" );
                writer.WriteAttributeString( "ss", "Vertical", null, "Center" );
                writer.WriteEndElement( );

                writer.WriteStartElement( "ss", "Font", null );
                writer.WriteAttributeString( "ss", "Bold", null, "1" );
                writer.WriteAttributeString( "ss", "Size", null, "10" );

                writer.WriteEndElement( ); // </Font>

                writer.WriteEndElement( ); //</Style>


                // Write PacketTime style
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "PacketTime" );

                writer.WriteStartElement( "ss", "NumberFormat", null );
                writer.WriteAttributeString( "ss", "Format", null, "hh:mm:ss.000" );
                writer.WriteEndElement( ); // </NumberFormat>

                writer.WriteStartElement( "ss", "Protection", null );
                writer.WriteAttributeString( "ss", "Protected", null, "1" );
                writer.WriteEndElement( ); //</Protection>

                writer.WriteEndElement( ); //</Style>

                // Write the header style	
                writer.WriteStartElement( "ss", "Style", null );
                writer.WriteAttributeString( "ss", "ID", null, "s42" );

                writer.WriteStartElement( "ss", "Alignment", null );
                writer.WriteAttributeString( "ss", "Horizontal", null, "Center" );
                writer.WriteAttributeString( "ss", "Vertical", null, "Center" );
                writer.WriteEndElement( );

                writer.WriteElementString( "ss", "Interior", null, null );

                writer.WriteStartElement( "ss", "Borders", null );

                writer.WriteStartElement( "ss", "Border", null );
                writer.WriteAttributeString( "ss", "Position", null, "Bottom" );
                writer.WriteAttributeString( "ss", "LineStyle", null, "Double" );
                writer.WriteAttributeString( "ss", "Weight", null, "3" );
                writer.WriteEndElement( ); // </Border>

                writer.WriteStartElement( "ss", "Border", null );
                writer.WriteAttributeString( "ss", "Position", null, "Left" );
                writer.WriteAttributeString( "ss", "LineStyle", null, "Continuous" );
                writer.WriteAttributeString( "ss", "Weight", null, "1" );
                writer.WriteEndElement( ); // </Border>

                writer.WriteStartElement( "ss", "Border", null );
                writer.WriteAttributeString( "ss", "Position", null, "Right" );
                writer.WriteAttributeString( "ss", "LineStyle", null, "Continuous" );
                writer.WriteAttributeString( "ss", "Weight", null, "1" );
                writer.WriteEndElement( ); // </Border>

                writer.WriteEndElement( ); // </Borders>

                writer.WriteStartElement( "ss", "Font", null );
                //writer.WriteAttributeString("x", "Family", null, "Arial");
                writer.WriteAttributeString( "ss", "Bold", null, "1" );
                writer.WriteEndElement( ); // </Font>

                writer.WriteStartElement( "ss", "Protection", null );
                writer.WriteAttributeString( "ss", "Protected", null, "1" );
                writer.WriteEndElement( ); //</Protection>

                writer.WriteEndElement( ); // </Style>

                writer.WriteEndElement( ); //</Styles>

                worker.ReportProgress( 10, report.GetProgressReport( 0 ) );


                // Worksheet for Summary Sheet
 
                writer.WriteStartElement( "Worksheet" );
                writer.WriteAttributeString( "ss", "Name", null, "Summary" );
                writer.WriteAttributeString( "ss", "Protected", null, "1" );

                writer.WriteStartElement( "ss", "Table", null );

                writer.WriteStartElement( "ss", "Column", null );
                writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
                writer.WriteAttributeString( "ss", "Width", null, "12.5" );
                writer.WriteEndElement( );


                writer.WriteStartElement( "ss", "Column", null );
                writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
                writer.WriteAttributeString( "ss", "Width", null, "108.76" );
                writer.WriteAttributeString( "ss", "Span", null, "3" );
                writer.WriteEndElement( );


                // Title
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "3" );
                writer.WriteAttributeString( "ss", "StyleID", null, "TitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString("RFID Explorer - Sample Reader Application");
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                // Write Environment subtitle
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Index", null, "4" );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "3" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "T E S T   E N V I R O N M E N T" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // Date of Run
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Date of Run" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );

                string d = reader.GetPropertyAsString( "SaveDate" );
                if ( String.IsNullOrEmpty( d ) )
                {
                    d = String.Format( "{0:MMMM d, yyyy}", DateTime.Now );
                }

                writer.WriteString( d );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // Time of run
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Time of Run" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                string t = reader.GetPropertyAsString( "SaveTime" );

                if ( String.IsNullOrEmpty( t ) )
                {
                    t = String.Format( "{0:T}", DateTime.Now );
                }

                writer.WriteString( t );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                //Serial Number
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Serial Number" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );
                //				writer.WriteAttributeString("ss", "MergeAcross", null, "1");

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );

                if ( String.IsNullOrEmpty( reader.Name ) )
                {
                    writer.WriteString( reader.GetPropertyAsString( "ReaderName" ) );
                }
                else
                {
                    writer.WriteString(reader.Name.Trim());
                }

                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>



                // Hardware Version
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Hardware Version" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                string version;
                if ( reader.Mode == rfidReader.OperationMode.BoundToReader )
                {
                    version = String.Format
                    (
                        " Control:{0:X4} Chip:{1:X4}",
                        reader.HardwareVersion.major,
                        reader.HardwareVersion.minor
                    );
                }
                else
                {
                    version = reader.GetPropertyAsString( "HardwareVersion" );
                }

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( version );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // BootLoader Version
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Bootloader Version" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                if ( reader.Mode == rfidReader.OperationMode.BoundToReader )
                {
                    version = String.Format
                    (
                        " {0}.{1}.{2}",
                        reader.BootLoaderVersion.major,
                        reader.BootLoaderVersion.minor,
                        reader.BootLoaderVersion.release
                    );
                }
                else
                {
                    version = reader.GetPropertyAsString("BootLoaderVersion");
                }

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( version );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // Firmware Version
                writer.WriteStartElement("ss", "Row", null);
                writer.WriteAttributeString("ss", "Height", null, "18");

                writer.WriteStartElement("ss", "Cell", null);
                writer.WriteAttributeString("ss", "Index", null, "2");
                writer.WriteAttributeString("ss", "StyleID", null, "SummaryCaption");

                writer.WriteStartElement("ss", "Data", null);
                writer.WriteAttributeString("ss", "Type", null, "String");
                writer.WriteString("Firmware Version");
                writer.WriteEndElement(); //</Data>

                writer.WriteEndElement(); // </Cell>

                writer.WriteStartElement("ss", "Cell", null);
                writer.WriteAttributeString("ss", "StyleID", null, "Data");

                if (reader.Mode == rfidReader.OperationMode.BoundToReader)
                {
                    version = String.Format
                    (
                        " {0}.{1}.{2}",
                        reader.FirmwareVersion.major,
                        reader.FirmwareVersion.minor,
                        reader.FirmwareVersion.release
                    );
                }
                else
                {
                    version = reader.GetPropertyAsString("FirmwareVersion");
                }

                writer.WriteStartElement("ss", "Data", null);
                writer.WriteAttributeString("ss", "Type", null, "String");
                writer.WriteString(version);
                writer.WriteEndElement(); //</Data>

                writer.WriteEndElement(); // </Cell>

                writer.WriteEndElement(); // </Row>



                // Library Version
                writer.WriteStartElement("ss", "Row", null);
                writer.WriteAttributeString("ss", "Height", null, "18");

                writer.WriteStartElement("ss", "Cell", null);
                writer.WriteAttributeString("ss", "Index", null, "2");
                writer.WriteAttributeString("ss", "StyleID", null, "SummaryCaption");

                writer.WriteStartElement("ss", "Data", null);
                writer.WriteAttributeString("ss", "Type", null, "String");
                writer.WriteString("Library Version");
                writer.WriteEndElement(); //</Data>

                writer.WriteEndElement(); // </Cell>

                writer.WriteStartElement("ss", "Cell", null);
                writer.WriteAttributeString("ss", "StyleID", null, "Data");

                if (reader.Mode == rfidReader.OperationMode.BoundToReader)
                {
                    version = String.Format
                    (
                        " {0}.{1}.{2}",
                        reader.LibraryVersion.major,
                        reader.LibraryVersion.minor,
                        reader.LibraryVersion.release
                    );
                }
                else
                {
                    version = reader.GetPropertyAsString("LibraryVersion");
                }

                writer.WriteStartElement("ss", "Data", null);
                writer.WriteAttributeString("ss", "Type", null, "String");
                writer.WriteString(version);
                writer.WriteEndElement(); //</Data>

                writer.WriteEndElement(); // </Cell>

                writer.WriteEndElement(); // </Row>



                // System
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Test System" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( System.Environment.MachineName );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // Bank Spacer Row
                writer.WriteStartElement("ss", "Row", null);
                writer.WriteEndElement();

                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "32" );
                writer.WriteEndElement( );

                writer.WriteStartElement("ss", "Row", null);
                writer.WriteEndElement();


                // S U M M A R Y row
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Index", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "3" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "R E S U L T S   S U M M A R Y" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>
                // Spacer row
                writer.WriteElementString( "ss", "Row", null, null );

                // headings row

                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "3" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Session" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Command" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "SubtitleStyle" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Current" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                //Duration row
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Duration" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // Session Duration
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "SessionDuration" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // Total Duration
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "TotalDuration" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // Duration
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "Duration" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                //Packets Count
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Packet Count" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // Session Packet Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionPacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Total Packets Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalPacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Packet Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "PacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                //Reader Cycles
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Antenna Cycles" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // Session AntennaCycles
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionAntennaCycleCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Total Antenna Cycles
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalAntennaCycleCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                // Antenna Cycles
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "AntennaCycleCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                //Inventory Cycles
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Inventory Cycles" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Session Inventory Cycle Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );
                // Row Header 
                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionInventoryCycles" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Total Inventory Cycle Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalInventoryCycles" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Inventory Cycles
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "InventoryCycles" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                //Inventory Rounds
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Inventory Rounds" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Session Round Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );
                // Row Header 
                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionRoundCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Total Round Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalRoundCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Round Count
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "RoundCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                //Total Singulations
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Total Singulations" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // SessionTagCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionTagCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // TotalTagCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalTagCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // TagCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TagCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                writer.WriteEndElement( ); // </Row>

                //Unique Singulations
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );
                // Row Header 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Unique Singulations" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // SessionUniqueTags
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionUniqueTags" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // RequestUniqueTags
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "RequestUniqueTags" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // CurrentUniqueTags
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "CurrentUniqueTags" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                // blank row
                writer.WriteElementString( "ss", "Row", null, null );


                //Singulations / Second
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Singulations / Second" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                // SingulationRateAverage
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SingulationRateAverage" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // TotalRate
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalRate" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Rate
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "Rate" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                // blank row
                writer.WriteElementString( "ss", "Row", null, null );

                //Bad Packets
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "Bad Packets" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>
                // SessionBadPacketCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "SessionBadPacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // TotalBadPacketCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "TotalBadPacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // BadPacketCount
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( reader.GetPropertyAsString( "BadPacketCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>


                //LBT Collisions
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "LBT Collisions" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                //
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );

                writer.WriteString( "-" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "-" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                //
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "-" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>

                //CRC Errors
                writer.WriteStartElement( "ss", "Row", null );
                writer.WriteAttributeString( "ss", "Height", null, "18" );

                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "Index", null, "2" );
                writer.WriteAttributeString( "ss", "StyleID", null, "SummaryCaption" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( "CRC Errors" );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                //
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "SessionCrcErrorCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // 
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "TotalCrcErrorCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                //
                writer.WriteStartElement( "ss", "Cell", null );
                writer.WriteAttributeString( "ss", "StyleID", null, "Data" );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( reader.GetPropertyAsString( "CrcErrorCount" ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                writer.WriteEndElement( ); // </Row>



                /// END OF TABLE
                writer.WriteEndElement( ); // </Table>

                writer.WriteEndElement( ); // </Worksheet>

                worker.ReportProgress( 10, report.GetProgressReport( 0 ) );


                foreach ( GridControl.GridType gridType in dataSources )
                {
                    GridControl.GridClass grid = GridControl.GridTypeCollection.GridCollection[ gridType ];

                    writer.WriteStartElement( "Worksheet" );
                    writer.WriteAttributeString( "ss", "Name", null, FormatSheetTabName( grid.Caption ) );
                    writer.WriteAttributeString( "ss", "Protected", null, "1" );

                    writer.WriteStartElement( "ss", "Table", null );

                    foreach ( GridControl.DisplayRule rule in grid.DisplayRules )
                    {
                        writer.WriteStartElement( "ss", "Column", null );

                        writer.WriteAttributeString( "ss", "Index", null, XmlConvert.ToString( rule.DisplayIndex + 1 ) );

                        writer.WriteAttributeString( "ss", "Width", null, XmlConvert.ToString( rule.Width ) );

                        writer.WriteEndElement( );
                    }

                    // Write the Col headings
                    writer.WriteStartElement( "ss", "Row", null );
                    writer.WriteAttributeString( "ss", "Height", null, "21.00" );
                    foreach ( GridControl.DisplayRule rule in grid.DisplayRules )
                    {
                        writer.WriteStartElement( "ss", "Cell", null );
                        writer.WriteAttributeString( "ss", "StyleID", null, "s42" );

                        //<ss:Data ss:Type="Number">1</ss:Data>
                        writer.WriteStartElement( "ss", "Data", null );

                        writer.WriteAttributeString( "ss", "Type", null, "String" );

                        writer.WriteString( rule.HeaderText );

                        writer.WriteEndElement( ); //</Data>

                        writer.WriteEndElement( ); // </Cell>
                    }
                    writer.WriteEndElement( ); // </Row>


                    // Write the Data
                    dataWasTruncated = WriteData( grid, reader, writer ) || dataWasTruncated;

                    writer.WriteEndElement( ); // </Table>

                    // Wite the Options
                    writer.WriteStartElement( "x", "WorksheetOptions", null );

                    // <Selected/>
                    writer.WriteElementString( "x", "FreezePanes", null, null );

                    writer.WriteElementString( "x", "FrozenNoSplit", null, null );

                    writer.WriteElementString( "x", "SplitHorizontal", null, "1" );

                    writer.WriteElementString( "x", "TopRowBottomPane", null, "1" );

                    writer.WriteElementString( "x", "ActivePane", null, "2" );

                    // <Panes>
                    //  <Pane>
                    //   <Number>3</Number>
                    //  </Pane>
                    //  <Pane>
                    //   <Number>2</Number>
                    //  </Pane>
                    // </Panes>
                    // <ProtectObjects>False</ProtectObjects>
                    // <ProtectScenarios>False</ProtectScenarios>

                    writer.WriteEndElement( ); // </WorksheetOptions>


                    writer.WriteEndElement( ); // </Worksheet>


                    worker.ReportProgress( 0, report.GetProgressReport( 0 ) );
                }
                writer.WriteEndElement( ); // </Workbook>

                writer.Close( );

                System.Diagnostics.Process.Start( fileName );

                if ( dataWasTruncated )
                {
                    report.OperationCompleted( OperationOutcome.SuccessWithInformation, "Data truncated due to Excel's row limit of 65536", 0 );
                }
                else
                {
                    report.OperationCompleted( OperationOutcome.Success, 0 );
                }
                return report;

            }
            catch ( Exception e )
            {               
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, e );
            }

        }

        private static bool WriteData( GridControl.GridClass grid, IReader reader, XmlWriter writer )
        {
            switch ( grid.Grid )
            {
                case GridControl.GridType.StandardView:
                    return WriteData<TagInventory>( grid, reader.TagInventoryData, writer );

                case GridControl.GridType.ReaderCommands:
                    return WriteData<ReaderCommand>( grid, reader.ReaderCommandData, writer );

                case GridControl.GridType.ReaderAntennaCycles:
                    return WriteData<ReaderAntennaCycle>( grid, reader.ReaderAntennaCycleData, writer );

                case GridControl.GridType.InventoryCycle:
                    return WriteData<InventoryCycle>( grid, reader.InventoryCycleData, writer );

                case GridControl.GridType.InventoryRounds:
                    return WriteData<InventoryRound>( grid, reader.InventoryRoundData, writer );

                case GridControl.GridType.InventoryParameters:
                    return WriteData<InventoryRound>( grid, reader.InventoryRoundData, writer );

                case GridControl.GridType.BadPackets:
                    return WriteData<BadPacket>( grid, reader.BadPacketData, writer );

                case GridControl.GridType.InventoryCycleDiag:
                    return WriteData<InventoryCycle>( grid, reader.InventoryCycleData, writer );

                case GridControl.GridType.InventoryRoundDiag:
                    return WriteData<InventoryRound>( grid, reader.InventoryRoundData, writer );

                case GridControl.GridType.TagAccess:
                    return WriteData<TagRead>( grid, reader.TagReadData, writer );

                case GridControl.GridType.TagDataDiagnostics:
                    return WriteData<TagRead>( grid, reader.TagReadData, writer );

                case GridControl.GridType.ReadRate:
                    return WriteData<ReadRate>( grid, reader.ReadRateData, writer );

                case GridControl.GridType.RawPackets:
                    return WriteData<PacketStream>( grid, reader.PacketStreamData, writer );

                default:
                    throw new InvalidEnumArgumentException( String.Format( "GridControl.GridType grid={0:f}", grid.Grid ), ( int ) grid.Grid, grid.GetType( ) );
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <param name="dataFile">Either a DataFile SequenceDataFile /param>
        /// <param name="writer"></param>
        /// <returns>True if data was truncated due to Excel's row limit.</returns>
        public static bool WriteData<T>( GridControl.GridClass grid, Object dataFile, XmlWriter writer ) where T : DatabaseRowTemplate, new( )
        {
            bool useSequentialDataFile = dataFile.GetType( ) == typeof( SequentialDataFile<T> );
            bool dataWasTruncated = false;

            // Get the list of display rules and sort them into display order
            List<GridControl.DisplayRule> ruleList = new List<GridControl.DisplayRule>( );

            foreach ( GridControl.DisplayRule rule in grid.DisplayRules )
            {
                ruleList.Add( rule );
            }
            ruleList.Sort( delegate( GridControl.DisplayRule ruleX, GridControl.DisplayRule ruleY )
                            { return ruleX.DisplayIndex - ruleY.DisplayIndex; } );

            // Get all the public properties for T
            PropertyInfo[ ] propInfoArray = typeof( T ).GetProperties( );

            // Build an index into the propInfoArray for each display rule
            int[ ] propIndex = new int[ ruleList.Count ];

            for ( int i = 0; i < propIndex.Length; i++ )
            {
                int index = Array.FindIndex<PropertyInfo>( propInfoArray, delegate( PropertyInfo p ) { return ruleList[ i ].DataPropertyName == p.Name; } );

                if ( index == -1 )
                {
                    throw new Exception( String.Format( "Cannot find property {0} in {1}.", ruleList[ i ].DataPropertyName, typeof( T ).Name ) );
                }

                propIndex[ i ] = index;
            }

            // Extract the data values

            int cnt = ( useSequentialDataFile ? ( ( SequentialDataFile<T> ) dataFile ).Count : ( ( DataFile<T> ) dataFile ).Count );

            if ( cnt >= MaxRowsInExcel )
            {
                dataWasTruncated = true;
                cnt = MaxRowsInExcel - 1; // subtract 1 to account for col headers
            }

            for ( int i = 0; i < cnt; i++ )
            {
                writer.WriteStartElement( "ss", "Row", null );

                T t = ( T ) ( useSequentialDataFile ? ( ( SequentialDataFile<T> ) dataFile )[ i ] : ( ( DataFile<T> ) dataFile )[ i ] );

                for ( int j = 0; j < ruleList.Count; j++ )
                {
                    Object data = propInfoArray[ propIndex[ j ] ].GetValue( t, null );

                    writer.WriteStartElement( "ss", "Cell", null );

                    string StyleID = GetExcelStyleID( data, grid, j );

                    if ( StyleID != null )
                    {
                        writer.WriteAttributeString( "ss", "StyleID", null, StyleID );
                    }

                    writer.WriteStartElement( "ss", "Data", null );

                    writer.WriteAttributeString( "ss", "Type", null, GetExcelTypeName( data, grid, j ) );
                    
                    writer.WriteString( GetExcelString( data, grid, j ) );
                    
                    writer.WriteEndElement( ); //</Data>

                    writer.WriteEndElement( ); // </Cell>
                }

                writer.WriteEndElement( ); // </Row>
            }

            return dataWasTruncated;
        }



        /// <summary>
        /// Find the string name of the Excel StyleID for the data and display rule
        /// </summary>
        /// <param name="data"></param>
        /// <param name="grid"></param>
        /// <returns>An Excel StyleID to be added as a Style attribue to the Cell element or null not to add a style.</returns>
        private static string GetExcelStyleID( object data, GridControl.GridClass grid, int ruleIndex )
        {
            if ( data == null )
            {
                return "Data";
            }

            if ( grid.DisplayRules[ ruleIndex ].HeaderText == "Packet Time" )
            {
                return "PacketTime";
            }

            if ( grid.DisplayRules[ ruleIndex ].HeaderText == "App First Seen UTC" )
            {
                return "PacketTime";
            }

            if ( grid.DisplayRules[ ruleIndex ].HeaderText == "Raw Packet Data" )
            {
                return "DataLeft";
            }

            return "Data";
        }


        /// <summary>
        /// Finds the string name of an Excel data type.
        /// </summary>
        /// <param name="data">The data to be mapped</param>
        /// <returns>Return a string of one of the valid excel data types (Number, DateTime, Boolean, String, or Error)</returns>
        public static string GetExcelTypeName( Object data, GridControl.GridClass grid, int ruleIndex )
        {
            String[ ] TypeName = { "Number", "DateTime", "Boolean", "String", "Error" };
            int NumberType = 0;
            int DateTimeType = 1;
            int BooleanType = 2;
            int StringType = 3;
            int ErrorType = 4;

            if ( data == null )
            {
                return TypeName[ StringType ];
            }

            Type t = data.GetType( );
            if ( t.IsArray )
            {
                Type elementType = t.GetElementType( );
                switch ( elementType.Name )
                {
                    case "Byte":
                    case "Char":
                        return TypeName[ StringType ];

                    default:
                        return TypeName[ ErrorType ];
                }
            }
            else
            {
                switch ( t.Name )
                {
                    case "String":
                        return TypeName[ StringType ];

                    case "DateTime":
                        return TypeName[ DateTimeType ];

                    default:
                        if ( t.IsPrimitive )
                        {
                            switch ( t.Name )
                            {
                                case "Boolean":
                                    return TypeName[ BooleanType ];

                                case "Byte":
                                case "SByte":
                                case "Int16":
                                case "UInt16":
                                case "Int32":
                                case "UInt32":
                                case "Int64":
                                case "UInt64":
                                case "Double":
                                case "Single":
                                    return TypeName[ NumberType ];

                                case "Char":
                                    return TypeName[ StringType ];

                                default:
                                    return TypeName[ StringType ];
                            }
                        }
                        else
                        {
                            Type underlyingType = Nullable.GetUnderlyingType( t );
                            if ( underlyingType != null )
                            {
                                if ( Object.Equals( data, null ) )
                                {
                                    return TypeName[ StringType ];
                                }
                                else
                                {
                                    switch ( underlyingType.Name )
                                    {
                                        case "Boolean":
                                            return TypeName[ BooleanType ];

                                        case "Byte":
                                        case "SByte":
                                        case "Int16":
                                        case "UInt16":
                                        case "Int32":
                                        case "UInt32":
                                        case "Int64":
                                        case "UInt64":
                                        case "Double":
                                        case "Single":
                                            return TypeName[ NumberType ];

                                        case "Char":
                                            return TypeName[ StringType ];

                                        case "DateTime":
                                            return TypeName[ DateTimeType ];

                                        default:
                                            return TypeName[ ErrorType ];
                                    }
                                }
                            }
                            else
                            {
                                return TypeName[ StringType ];
                            }
                        }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="grid"></param>
        /// <returns>String representation of data</returns>
        public static string GetExcelString( object data, GridControl.GridClass grid, int ruleIndex )
        {
            String dateFormatString = "{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffff}";

            if ( data == null )
                return String.Empty;

            Type t = data.GetType( );

            if ( t.IsArray )
            {
                Type elementType = t.GetElementType( );
                switch ( elementType.Name )
                {
                    case "Byte":
                    case "Char":
                        return String.Format( "{0:g}", data );

                    default:
                        return String.Format( "Unsupported array type \"{0}\".", t.Name );
                }
            }
            else
            {
                switch ( t.Name )
                {
                    case "String":
                        return ( String ) data;

                    case "DateTime":
                        return String.Format( dateFormatString, data );

                    default:
                        if ( t.IsPrimitive )
                        {

                            return String.Format( String.Format( "{{0:{0}}}", grid.DisplayRules[ ruleIndex ].Format ), data );
                        }
                        else
                        {
                            Type underlyingType = Nullable.GetUnderlyingType( t );
                            if ( underlyingType != null )
                            {
                                if ( Object.Equals( data, null ) )
                                {
                                    return String.Empty;
                                }
                                else
                                {
                                    switch ( underlyingType.Name )
                                    {
                                        case "Boolean":
                                        case "Byte":
                                        case "SByte":
                                        case "Int16":
                                        case "UInt16":
                                        case "Int32":
                                        case "UInt32":
                                        case "Int64":
                                        case "UInt64":
                                        case "Char":
                                        case "Double":
                                        case "Single":
                                            return String.Format( String.Format( "{{0:{0}}}", grid.DisplayRules[ ruleIndex ].Format ), data );

                                        case "DateTime":
                                            return String.Format( dateFormatString, data );

                                        default:
                                            return String.Format( "Unsupported nullable field type: {0}?.", underlyingType.Name );

                                    }
                                }
                            }
                            else
                            {
                                return String.Format( "Encountered an unsupported field type \"{0}\".", t.Name );
                            }
                        }
                }
            }
        } // public static string GetExcelString(object data, GridControl.GridClass grid, int ruleIndex)



        public static Source_AntennaList ImportAntennaConfig( LakeChabotReader reader, String filename )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( "reader", "ImportAntennaConfig expects valid reader" );
            }

            if ( String.IsNullOrEmpty( filename ) )
            {
                throw new ArgumentNullException( "filename", "ImportAntennaConfig expects valid filename" );
            }

            if ( !File.Exists( filename ) )
            {
                throw new ArgumentException( String.Format( "File {0} does not exist.", filename ), "filename" );
            }

            Source_AntennaList antennaList = new Source_AntennaList( );

            XmlReaderSettings settings = new XmlReaderSettings( );
            settings.CheckCharacters = true;
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;


            using ( XmlReader xmlReader = XmlReader.Create( new FileStream( filename, FileMode.Open ), settings ) )
            {
                xmlReader.MoveToContent( );

                while ( !xmlReader.IsStartElement( ) )
                {
                    xmlReader.Read( );
                }

                if ( xmlReader.Name != "Workbook" )
                {
                    throw new Exception( "The \"Workbook\" element is missing." );
                }

                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || xmlReader.Name != "Worksheet" ) );

                if ( xmlReader.Name != "Worksheet" )
                {
                    throw new Exception( "The \"Worksheet\" element is missing." );
                }

                // read to table element
                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || xmlReader.Name != "Table" ) );

                if ( xmlReader.Name != "Table" )
                {
                    throw new Exception( "The \"Table\" element is missing." );
                }

                // read to Row element
                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Row" || xmlReader.Name == "Row" ) ) );

                if ( !xmlReader.Name.EndsWith( "Row" ) )
                {
                    throw new Exception( "The \"Row\" element is missing." );
                }

                int rowcount = 0;

                while ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                {
                    ++rowcount;

                    if ( rowcount > 1 )
                    {

                        byte   port;
                        rfid.Constants.AntennaPortState state;
                        UInt16 powerLevel;
                        UInt16 dwellTime;
                        UInt16 numberInventoryCycles;
                        byte   physicalPort;

                        // find the antenna data node
                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The address \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String antennaString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( antennaString ) )
                        {
                            throw new Exception( String.Format( "Null or empty antenna number in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !byte.TryParse( antennaString, out port ) )
                            {
                                throw new Exception( String.Format( "Unable to parse antenna number in row {0}.", rowcount ) );
                            }

                        }

                        // find the Enabled data node
                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The name \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String stateString = xmlReader.ReadString( ).Trim( );

                        if ( stateString.StartsWith( "Enabled", StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            state = rfid.Constants.AntennaPortState.ENABLED;
                        }
                        else
                        {
                            state = rfid.Constants.AntennaPortState.DISABLED;
                        }


                        // find the txPort data node
                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The name \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String PortString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( PortString ) )
                        {
                            throw new Exception( String.Format( "Null or empty TX Physical Port in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !byte.TryParse( PortString, out physicalPort ) )
                            {
                                throw new Exception( String.Format( "Unable to parse TX Physical Port  in row {0}.", rowcount ) );
                            }

                        }


                        // find the power data node
                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String valuePower = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( valuePower ) )
                        {
                            throw new Exception( String.Format( "Null or empty power value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if (!UInt16.TryParse(valuePower, out powerLevel))
                            {
                                throw new Exception( String.Format( "Unable to parse power value in row {0}.", rowcount ) );
                            }
                        }


                        // find the dwell data node
                        do
                        {
                            xmlReader.Read( );

                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String dwellString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( dwellString ) )
                        {
                            throw new Exception( String.Format( "Null or empty dwell time value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if (!UInt16.TryParse(dwellString, out dwellTime))
                            {
                                throw new Exception( String.Format( "Unable to parse dwell time value in row {0}.", rowcount ) );
                            }
                        }


                        // find the rounds data node
                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String roundsString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( roundsString ) )
                        {
                            throw new Exception( String.Format( "Null or empty inventory rounds value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if (!UInt16.TryParse(roundsString, out numberInventoryCycles))
                            {
                                throw new Exception( String.Format( "Unable to parse inventory rounds value in row {0}.", rowcount ) );
                            }
                        }

                        antennaList.Add
                        (
                            new Source_Antenna
                            (
                                port,
                                state,
                                powerLevel,
                                dwellTime,
                                numberInventoryCycles,
                                physicalPort,
                                0  // antennaSenseThreshold - store ops in Source_AntennaConfig
                                   // now always ignore and write out the global thresh val
                            )
                        );

                    }

                    // To skip column headers...

                    do
                    {
                        xmlReader.Read( );
                    } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Row" || xmlReader.Name == "Row" ) ) );


                }

                xmlReader.Close( );
            }

            return antennaList;

        } // public static LakeChabotReader.AntennaDataList ImportAntennaConfig(LakeChabotReader reader, String filename) 




        // Export antenna values ON RADIO to Excel xml format

        public static void ExportAntennaConfig( LakeChabotReader reader )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( "reader", "ExportAntennaConfig expects valid reader" );
            }

            int i = 1;
            string fileName;
            while ( File.Exists( ( fileName = Path.Combine( System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), String.Format( "{0}{1:d03}.{2}", "RFID Antenna Data", i++, "xml" ) ) ) ) )
            {
                // NOP
            }

            XmlWriterSettings settings = new XmlWriterSettings( );
            settings.Encoding = Encoding.UTF8;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.CheckCharacters = true;

            XmlWriter writer = XmlWriter.Create( fileName, settings );


            writer.WriteStartDocument( );
            writer.WriteProcessingInstruction( "mso-application", "progid=\"Excel.Sheet\"" );

            writer.WriteStartElement( "Workbook", "urn:schemas-microsoft-com:office:spreadsheet" );
            writer.WriteAttributeString( "xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet" );
            writer.WriteAttributeString( "xmlns", "x", null, "urn:schemas-microsoft-com:office:excel" );
            writer.WriteAttributeString( "xmlns", "x2", null, "urn:schemas-microsoft-com:office:excel2" );
            writer.WriteAttributeString( "xmlns", "html", null, "http://www.w3.org/TR/REC-html40" );

            // Write the styles

            writer.WriteStartElement( "ss", "Styles", null );

            // Write Default Style

            writer.WriteStartElement( "ss", "Style", null );
            writer.WriteAttributeString( "ss", "ID", null, "Default" );
            writer.WriteAttributeString( "ss", "Name", null, "Normal" );

            writer.WriteStartElement( "ss", "Alignment", null );
            writer.WriteAttributeString( "ss", "Horizontal", null, "Center" );
            writer.WriteAttributeString( "ss", "Vertical", null, "Bottom" );
            writer.WriteEndElement( );

            writer.WriteElementString( "ss", "Borders", null, null );

            writer.WriteElementString( "ss", "Font", null, null );

            writer.WriteElementString( "ss", "Interior", null, null );

            writer.WriteElementString( "ss", "NumberFormat", null, null );

            writer.WriteStartElement( "ss", "Protection", null );
            writer.WriteAttributeString( "ss", "Protected", null, "0" );
            writer.WriteEndElement( ); //</Protection>

            writer.WriteEndElement( ); //</Style>


            writer.WriteEndElement( ); //</Styles>

            // Worksheet for Summary Sheet

            writer.WriteStartElement( "Worksheet" );
            writer.WriteAttributeString( "ss", "Name", null, "Antenna Settings" );

            writer.WriteStartElement( "Table" );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            // Write the Col headings
            writer.WriteStartElement( "ss", "Row", null );
            writer.WriteAttributeString( "ss", "Height", null, "21.00" );

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Antenna" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "State" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>



            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "TX Physical Port" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "RX Physical Port" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Power Level" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Dwell Time" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Inventory Rounds" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteEndElement( ); // </Row>

            Source_AntennaList currentAntennaList = new Source_AntennaList( );

            currentAntennaList.load( LakeChabotReader.MANAGED_ACCESS, reader.ReaderHandle );

            foreach ( RFID.RFIDInterface.Source_Antenna antenna in currentAntennaList )
            {

                writer.WriteStartElement( "ss", "Row", null );

                // antenna
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( String.Format( "{0}", antenna.Port ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // TX Port
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                if ( rfid.Constants.AntennaPortState.ENABLED == antenna.State )
                {
                    writer.WriteString( "Enabled" );
                }
                else
                {
                    writer.WriteString( "Disabled" ); // all else disabled
                }

                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                // TX Port
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( antenna.PhysicalPort.ToString( ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

      
                // powerColumn
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", antenna.PowerLevel ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>


                // dwellColumn12
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", antenna.DwellTime ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // roundsColumn
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", antenna.NumberInventoryCycles ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>
            }



            /// END OF TABLE
            writer.WriteEndElement( ); // </Table>


            writer.WriteEndElement( ); // </Worksheet>

            writer.WriteEndElement( ); // </Workbook>

            writer.Close( );

            System.Diagnostics.Process.Start( fileName );


        } // public static void ExportAntennaConfig(LakeChabotReader reader)



        public static Source_FrequencyBandList ImportRFChannelConfig( LakeChabotReader reader, String filename )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( "reader", "ImportAntennaConfig expects valid reader" );
            }

            if ( String.IsNullOrEmpty( filename ) )
            {
                throw new ArgumentNullException( "filename", "ImportAntennaConfig expects valid filename" );
            }

            if ( !File.Exists( filename ) )
            {
                throw new ArgumentException( String.Format( "File {0} does not exist.", filename ), "filename" );
            }

            Source_FrequencyBandList channelList = new Source_FrequencyBandList( );

            XmlReaderSettings settings = new XmlReaderSettings( );
            settings.CheckCharacters = true;
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;


            using ( XmlReader xmlReader = XmlReader.Create( new FileStream( filename, FileMode.Open ), settings ) )
            {
                xmlReader.MoveToContent( );

                while ( !xmlReader.IsStartElement( ) )
                {
                    xmlReader.Read( );
                }

                if ( xmlReader.Name != "Workbook" )
                {
                    throw new Exception( "The \"Workbook\" element is missing." );
                }

                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || xmlReader.Name != "Worksheet" ) );

                if ( xmlReader.Name != "Worksheet" )
                {
                    throw new Exception( "The \"Worksheet\" element is missing." );
                }

                // read to table element
                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || xmlReader.Name != "Table" ) );

                if ( xmlReader.Name != "Table" )
                {
                    throw new Exception( "The \"Table\" element is missing." );
                }

                // read to Row element
                do
                {
                    xmlReader.Read( );
                } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Row" || xmlReader.Name == "Row" ) ) );

                if ( !xmlReader.Name.EndsWith( "Row" ) )
                {
                    throw new Exception( "The \"Row\" element is missing." );
                }

                int rowcount = 0;

                while ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                {
                    ++rowcount;

                    if ( rowcount > 1 )
					{
						UInt32                         band;
                        Source_FrequencyBand.BandState state;
						UInt16                         multiplier;
						UInt16                         divider;
						UInt16                         minDacBand;
                        UInt16                         affinityBand;
						UInt16                         maxDacBand;
                        UInt16                         guardBand;

						// find the antenna data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The address \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String channelString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( channelString ) )
                        {
                            throw new Exception( String.Format( "Null or empty channel number in row {0}.", rowcount ) );
                        }

                        if ( channelString.StartsWith( "channel ", true, null ) && channelString.Length > 8 )
                        {
                            channelString = channelString.Substring( 8 );

                            if ( !UInt32.TryParse( channelString, out band ) )
                            {
                                throw new Exception( String.Format( "Unable to parse channel number in row {0}.", rowcount ) );
                            }
                        }
                        else
                        {
                            if ( !UInt32.TryParse( channelString, out band ) )
                            {
                                throw new Exception( String.Format( "Unable to parse channel number in row {0}.", rowcount ) );
                            }
                        }

						// find the Enabled data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The name \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String stateString = xmlReader.ReadString( ).Trim( );

                        if( stateString.StartsWith( "Enabled", StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            state = Source_FrequencyBand.BandState.ENABLED;
                        }
                        else
                        {
                            state = Source_FrequencyBand.BandState.DISABLED; // not enabled - all others disabled
                        }

                        // find the Frequency data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The Frequency \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String frequencyString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( frequencyString ) )
                        {
                            throw new Exception( String.Format( "Null or empty frequency in row {0}.", rowcount ) );
                        }


                        // find the Multiplier data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The Multiplier \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String multiplierString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( multiplierString ) )
                        {
                            throw new Exception( String.Format( "Null or empty multiplier value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( multiplierString, out multiplier ) )
                            {
                                throw new Exception( String.Format( "Unable to parse multiplier value in row {0}.", rowcount ) );
                            }
                        }


                        // find the divider data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The divider \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String dividerString = xmlReader.ReadString( ).Trim( );
                        if ( String.IsNullOrEmpty( dividerString ) )
                        {
                            throw new Exception( String.Format( "Null or empty divider value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( dividerString, out divider ) )
                            {
                                throw new Exception( String.Format( "Unable to parse divider value in row {0}.", rowcount ) );
                            }
                        }


                        // find the min data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String minString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( minString ) )
                        {
                            throw new Exception( String.Format( "Null or empty min value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( minString, out minDacBand ) )
                            {
                                throw new Exception( String.Format( "Unable to parse min value in row {0}.", rowcount ) );
                            }
                        }


                        // find the affinity data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        minString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( minString ) )
                        {
                            throw new Exception( String.Format( "Null or empty min value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( minString, out affinityBand ) )
                            {
                                throw new Exception( String.Format( "Unable to parse affinity value in row {0}.", rowcount ) );
                            }
                        }


                        // find the max data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The max value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        String maxString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( maxString ) )
                        {
                            throw new Exception( String.Format( "Null or empty max value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( maxString, out maxDacBand ) )
                            {
                                throw new Exception( String.Format( "Unable to parse max value in row {0}.", rowcount ) );
                            }
                        }



                        // find the guard band data node

                        do
                        {
                            xmlReader.Read( );
                            if ( xmlReader.IsStartElement( ) && xmlReader.Name.EndsWith( "Row" ) )
                            {
                                break;
                            }
                        } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || !( xmlReader.Name == "ss:Data" || xmlReader.Name == "Data" ) ) );

                        if ( !xmlReader.Name.EndsWith( "Data" ) )
                        {
                            throw new Exception( String.Format( "The max value \"Data\" element is missing from row {0}.", rowcount ) );
                        }

                        maxString = xmlReader.ReadString( ).Trim( );

                        if ( String.IsNullOrEmpty( maxString ) )
                        {
                            throw new Exception( String.Format( "Null or empty max value in row {0}.", rowcount ) );
                        }
                        else
                        {
                            if ( !UInt16.TryParse( maxString, out guardBand ) )
                            {
                                throw new Exception( String.Format( "Unable to parse guard value in row {0}.", rowcount ) );
                            }
                        }

                        channelList.Add
                        (
                            new Source_FrequencyBand
                            (
                                band,
                                state,
                                multiplier,
                                divider,
                                minDacBand,
                                affinityBand,
                                maxDacBand,
                                guardBand
                            )
                        );


					}
					do
					{
						xmlReader.Read();
					} while (!xmlReader.EOF && (!xmlReader.IsStartElement() || !(xmlReader.Name == "ss:Row" || xmlReader.Name == "Row")));

				}

				xmlReader.Close();
			}

			return channelList;

        } // Source_FrequencyBandList ImportRFChannelConfig( LakeChabotReader reader, String filename )



        // Writes out the values currently set ON THE RADIO
        // TODO: re-write to take list as parameter...

        public static void ExportRFChannelConfig( LakeChabotReader reader )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( "reader", "ExportRFChannelConfig expects valid reader" );
            }

            int i = 1;
            string fileName;
            while ( File.Exists( ( fileName = Path.Combine( System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), String.Format( "{0}{1:d03}.{2}", "RFID RF Channel Data", i++, "xml" ) ) ) ) )
            {
                // NOP
            }

            XmlWriterSettings settings = new XmlWriterSettings( );
            settings.Encoding = Encoding.UTF8;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.CheckCharacters = true;

            XmlWriter writer = XmlWriter.Create( fileName, settings );


            writer.WriteStartDocument( );
            writer.WriteProcessingInstruction( "mso-application", "progid=\"Excel.Sheet\"" );

            writer.WriteStartElement( "Workbook", "urn:schemas-microsoft-com:office:spreadsheet" );
            writer.WriteAttributeString( "xmlns", "ss", null, "urn:schemas-microsoft-com:office:spreadsheet" );
            writer.WriteAttributeString( "xmlns", "x", null, "urn:schemas-microsoft-com:office:excel" );
            writer.WriteAttributeString( "xmlns", "x2", null, "urn:schemas-microsoft-com:office:excel2" );
            writer.WriteAttributeString( "xmlns", "html", null, "http://www.w3.org/TR/REC-html40" );

            // Write the styles
            writer.WriteStartElement( "ss", "Styles", null );

            // Write Default Style
            writer.WriteStartElement( "ss", "Style", null );
            writer.WriteAttributeString( "ss", "ID", null, "Default" );
            writer.WriteAttributeString( "ss", "Name", null, "Normal" );

            writer.WriteStartElement( "ss", "Alignment", null );
            writer.WriteAttributeString( "ss", "Horizontal", null, "Center" );
            writer.WriteAttributeString( "ss", "Vertical", null, "Bottom" );
            writer.WriteEndElement( );

            writer.WriteElementString( "ss", "Borders", null, null );

            writer.WriteElementString( "ss", "Font", null, null );

            writer.WriteElementString( "ss", "Interior", null, null );

            writer.WriteElementString( "ss", "NumberFormat", null, null );

            writer.WriteStartElement( "ss", "Protection", null );
            writer.WriteAttributeString( "ss", "Protected", null, "0" );
            writer.WriteEndElement( ); //</Protection>

            writer.WriteEndElement( ); //</Style>


            writer.WriteEndElement( ); //</Styles>

            // Worksheet for Summary Sheet

            writer.WriteStartElement( "Worksheet" );
            writer.WriteAttributeString( "ss", "Name", null, "RF Channel Settings" );


            writer.WriteStartElement( "Table" );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );


            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            writer.WriteStartElement( "ss", "Column", null );
            writer.WriteAttributeString( "ss", "AutoFitWidth", null, "0" );
            writer.WriteAttributeString( "ss", "Width", null, "108.76" );
            writer.WriteAttributeString( "ss", "Span", null, "2" );
            writer.WriteEndElement( );

            // Write the Col headings
            writer.WriteStartElement( "ss", "Row", null );
            writer.WriteAttributeString( "ss", "Height", null, "21.00" );

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Channel" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "State" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>



            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Frequency" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Multiplier" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Divider" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Min DAC Band" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Affinity Band" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Max DAC Band" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>


            writer.WriteStartElement( "ss", "Cell", null );

            writer.WriteStartElement( "ss", "Data", null );
            writer.WriteAttributeString( "ss", "Type", null, "String" );
            writer.WriteString( "Guard Band Mhz" );
            writer.WriteEndElement( ); //</Data>

            writer.WriteEndElement( ); // </Cell>

            writer.WriteEndElement( ); // </Row>


            Source_FrequencyBandList currentChannelList = new Source_FrequencyBandList( );

            currentChannelList.load( LakeChabotReader.MANAGED_ACCESS, reader.ReaderHandle );

            foreach ( RFID.RFIDInterface.Source_FrequencyBand channel in currentChannelList )
            {
                writer.WriteStartElement( "ss", "Row", null );

                // Band
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( String.Format( "Channel {0}", channel.Band ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // State
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                if ( RFID.RFIDInterface.Source_FrequencyBand.BandState.ENABLED == channel.State )
                {
                    writer.WriteString( "Enabled" );
                }
                else
                {
                    writer.WriteString( "Disabled" ); // disabled | unknown -> disabled
                }

                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Frequency
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( String.Format( "{0} MHz", channel.Frequency ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // MultiplyRatio
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( channel.MultiplyRatio.ToString( ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // DivideRatio
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", channel.DivideRatio ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // MinimumDACBand
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", channel.MinimumDACBand ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Affinity DACBand
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", channel.AffinityBand ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // MaximumDACBand
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "Number" );
                writer.WriteString( String.Format( "{0}", channel.MaximumDACBand ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                // Guard Band ( MHz )
                writer.WriteStartElement( "ss", "Cell", null );

                writer.WriteStartElement( "ss", "Data", null );
                writer.WriteAttributeString( "ss", "Type", null, "String" );
                writer.WriteString( String.Format( "{0}", channel.GuardBand ) );
                writer.WriteEndElement( ); //</Data>

                writer.WriteEndElement( ); // </Cell>

                writer.WriteEndElement( ); // </Row>
            }



            /// END OF TABLE
            writer.WriteEndElement( ); // </Table>


            writer.WriteEndElement( ); // </Worksheet>

            writer.WriteEndElement( ); // </Workbook>

            writer.Close( );

            System.Diagnostics.Process.Start( fileName );


        } // public static void ExportRFChannelConfig(LakeChabotReader reader)




    } // public static class ExcelExport

}
