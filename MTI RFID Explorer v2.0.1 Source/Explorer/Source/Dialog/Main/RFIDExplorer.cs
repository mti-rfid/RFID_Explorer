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
 * $Id: RFIDTracer.cs,v 1.20 2009/12/11 02:46:42 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using rfid.Constants;
using RFID.RFIDInterface;
using System.IO;
using System.Xml;


using Global;


namespace RFID_Explorer
{
    public partial class mainForm : Form
    {
        public event EventHandler CurrentContextChanged;
        private bool pauseButtonChecked = false;
        private int _oldRecentPacketCount = 0; //Add by Chingsheng for display extention data, 2015-01-14
        private class PopupMessage
        {
            
            public enum PopupType
            {
                QueryExit,
                QueryRestart,
                StartupError,
                FailToPauseInventory,
                FailToResumeInventory,
                UnableToComplete,
                UnableToContinue,
                NoLibrary,
                Busy,
                NoClearWhenBusy,
                NoCloseWhenBusy,
                NoSaveWhenReaderBusy,
                NoSaveWithoutIndexing,
                NoSaveWithoutData,
                NoOpenWhenReaderBusy,
                NoOptionWhenReaderBusy,
                NoVirtualReaderWhenReaderBusy,
                NoCloseOfLastFile,
                NoFileOverwrite,
                NoSaveWithoutContext,
                NoClearWithoutContext,
                RegisterErrors,
                MacRegisterStartup,
                TildenRegisterStartup,
                GPIOStartup,
                AntennaStartup,
                RFChannelStartup,
                OperatingRegion,
                Algorithm,
                NotInEmulation,
                NoVirtualReaderWithoutTwo,
                VirtualReaderCreated,
                VirtualReaderKilled,
                VirtualReaderNotSupported,

            }

            private PopupType Type;
            private object[ ] args;
            private DialogResult result;
            private AutoResetEvent closeEvent;

            public DialogResult Result
            {
                get { return result; }
            }

            private PopupMessage( PopupType PopupType, params object[ ] args )
            {
                this.Type = PopupType;
                this.args = args;
                this.closeEvent = new AutoResetEvent( false );
            }

            public static DialogResult DoPopup( PopupType popupType, params object[ ] args )
            {
                PopupMessage box = new PopupMessage( popupType, args );
                ThreadPool.QueueUserWorkItem( new WaitCallback( box.Pop ) );

                return box.Result;
            }

            /// <summary>
            /// Popup a normal MessageBox and wait for close.
            /// </summary>
            /// <param name="popupType">Type of message to display.</param>
            /// <returns></returns>
            public static DialogResult Popup( PopupType popupType, params object[ ] args )
            {

                PopupMessage box = new PopupMessage( popupType, args );
                box.Pop( box );
                box.closeEvent.WaitOne( 10000, false );
                return box.Result;
            }

            /// <summary>
            /// Start a MessageBox on a worker thread and return right away
            /// </summary>
            /// <param name="o"></param>
            private void Pop( Object o )
            {
                switch ( this.Type )
                {
                    case PopupType.QueryExit:
                        result = MessageBox.Show( "Confirm Application Exit\n\nPlease confirm that you want the application to close.\nSelect 揧es?to close the application or 揘o?to resume.", "Question...", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
                        break;

                    case PopupType.QueryRestart:
                        result = MessageBox.Show( "Confirm Application Restart\n\nOne or more of your changes requires a restart to go into effect.\nSelect  揧es?to restart the application now.", "Question...", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
                        break;

                    case PopupType.StartupError:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Startup Error.\n\nThe following error occurred during initialization:\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.FailToPauseInventory:
                        result = MessageBox.Show("Error, pause Inventory unsuccessfully.\n\nPlease repeat again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case PopupType.FailToResumeInventory:
                        result = MessageBox.Show("Error, resume Inventory unsuccessfully.\n\nPlease repeat again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case PopupType.UnableToComplete:
                        result = MessageBox.Show( "Unable to complete task.\n\nThe application was unable to complete the requested task.\nPlease see the \"Application\" Event log for more information.", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        break;

                    case PopupType.UnableToContinue:
                        result = MessageBox.Show( String.Format( "{0} has encountered a problem and needs to close.\n\nAdditional information may be found in the \"Application\" Event log.\n\nWe are sorry for the inconvenience, would you like to try and restart the application?.", Application.ProductName ), "Application Error", MessageBoxButtons.YesNo, MessageBoxIcon.Stop );
                        break;

                    case PopupType.NoLibrary:
                        result = MessageBox.Show( "Reader Library Missing.\n\nUnable to load the Reader Library (rfid.dll).", "No Reader Library", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        break;

                    case PopupType.Busy:
                        result = MessageBox.Show( "Reader is busy.\n\nPlease try again after the current task completes.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoClearWhenBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nPlease stop the active reader before clearing.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoCloseWhenBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nPlease stop the active Reader(s) before closing.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoSaveWhenReaderBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nYou cannot save to a file when the reader is busy.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoSaveWithoutIndexing:
                        result = MessageBox.Show( "Captured data is not ready.\n\nBefore you save to a file you must perform the post-capture processing.\n\nClick on the View menu and select \"Build Post-Caputure Views\".", "Data Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                        break;

                    case PopupType.NoSaveWithoutData:
                        result = MessageBox.Show( "No available data to save.\n\nYou cannot save data when the option \"Do not save data to temporary file\" is selected.\n\nTo change this option, click on the Tools menu and select Options, then click on Data Logging and clear the option.", "No Available Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                        break;

                    case PopupType.NoOpenWhenReaderBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nYou cannot open a file when the reader is busy.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoOptionWhenReaderBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nYou cannot change the options when one or more readers are busy.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoVirtualReaderWhenReaderBusy:
                        result = MessageBox.Show( "Reader is busy.\n\nYou cannot change bridged reader device when one or more readers are busy.", "Reader Busy", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoCloseOfLastFile:
                        result = MessageBox.Show( "Cannot close file.\n\nYou cannot close the last file when no readers are present.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoFileOverwrite:
                        result = MessageBox.Show( "Cannot overwrite a data file.\n\nYou cannot update or modify a data file loaded from disk.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoSaveWithoutContext:
                        result = MessageBox.Show( "Nothing to save.\n\nYou cannot save without an active reader.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.NoClearWithoutContext:
                        result = MessageBox.Show( "Nothing to clear.\n\nYou cannot clear without an active reader.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.RegisterErrors:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error setting registers.\n\nThe following errors occurred while setting initial registers values:\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.MacRegisterStartup:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading MAC virtual registers values.\n\nThe following error occurred during initialization:\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;


                    case PopupType.TildenRegisterStartup:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading hardware registers values.\n\nThe following error occurred during initialization:\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.GPIOStartup:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading GPIO Pin configuration settings.\n\nThe following error occurred during initialization:\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.AntennaStartup:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading antenna settings.\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.RFChannelStartup:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading RF channel settings.\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.OperatingRegion:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading operating region setting.\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.Algorithm:
                        {
                            string s = "Unknown.";
                            if ( args.Length > 0 && args[ 0 ] is String )
                            {
                                s = ( string ) args[ 0 ];
                            }
                            result = MessageBox.Show( "Error reading inventory algorithm setting.\n\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        break;

                    case PopupType.NoVirtualReaderWithoutTwo:
                        result = MessageBox.Show( "Bridged Reader Error.\n\nA bridged reader cannnot be created unless there are at least two available readers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        break;


                    case PopupType.VirtualReaderCreated:
                        result = MessageBox.Show( "Bridged Reader Created.\n\nA bridged reader device has been created from the available readers and made the active device.", "Reader Bridge", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.VirtualReaderKilled:
                        result = MessageBox.Show( "Bridged Reader Removed.\n\nThe bridged reader device has been removed.", "Reader Bridge", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;

                    case PopupType.VirtualReaderNotSupported:
                        result = MessageBox.Show( "Not supported by Virtual Device.\n\nThe requested action is not supported by the Virtual Reader Device", "Reader Bridge", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        break;




                    default:
                        break;
                }
                closeEvent.Set( );
            }
        } //private class PopupMessage



        private static class Logger
        {
            private const int ERROR_EVENTLOG_FILE_CORRUPT = 1500;
            private const int ERROR_EVENTLOG_CANT_START = 1501;
            private const int ERROR_LOG_FILE_FULL = 1502;

            private static System.Diagnostics.EventLog _log;
            private static bool DO_NOT_LOG = true;

            static Logger( )
            {
                _log = new System.Diagnostics.EventLog( "Application", ".", System.Reflection.Assembly.GetExecutingAssembly( ).GetName( ).Name );
            }


            public static void Error( Exception exception )
            {
                if ( exception == null )
                {
                    exception = new ArgumentNullException( "exception", "Null exception passed to Logger.Error()" );
                }

                if ( exception is rfidLogErrorException )
                {
                    Error( exception.InnerException );
                    return;
                }

                RFID.RFIDInterface.rfidException rfid = exception as RFID.RFIDInterface.rfidException;

                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        if ( rfid != null )
                        {
                            _log.WriteEntry( FormatEventMessage( rfid ), System.Diagnostics.EventLogEntryType.Error, ( int ) rfid.ErrorCode.ErrorCode );
                        }
                        else
                        {
                            _log.WriteEntry( FormatEventMessage( exception ), System.Diagnostics.EventLogEntryType.Error );
                        }
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }

                }
                if ( DialogResult.Yes != PopupMessage.Popup( PopupMessage.PopupType.UnableToContinue ) )
                {
                    if ( mainForm.ActiveForm != null )
                    {
                        mainForm.ActiveForm.Close( );
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.Abort( );
                    }
                }
                else
                {
                    try
                    {
                        mainForm.ActiveForm.Close( );
                        Application.Restart( );
                    }
                    catch ( Exception )
                    {
                        MessageBox.Show( "Automatic Restart Failed." );
                    }
                }
            }


            public static void Warning( string Message )
            {
                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        _log.WriteEntry( Message, System.Diagnostics.EventLogEntryType.Warning );
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }

                PopupMessage.DoPopup( PopupMessage.PopupType.UnableToComplete );
            }

            public static void Warning( string Message, PopupMessage.PopupType popupType )
            {
                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        _log.WriteEntry( String.Format( "{0}", Message ), System.Diagnostics.EventLogEntryType.Warning );
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }

                PopupMessage.DoPopup( popupType, Message );
            }

            public static void Warning( string Message, rfidError Error )
            {
                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        _log.WriteEntry( String.Format( "{0}", Message ), System.Diagnostics.EventLogEntryType.Warning, ( int ) Error.ErrorCode );
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }

                PopupMessage.DoPopup( PopupMessage.PopupType.UnableToComplete );
            }

            public static void Warning( rfidError Error, PopupMessage.PopupType popupType )
            {
                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        _log.WriteEntry( String.Format( "Error Code: {0}", Error ), System.Diagnostics.EventLogEntryType.Warning, ( int ) Error.ErrorCode );
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }
                PopupMessage.DoPopup( popupType );
            }

            public static void Warning( string Message, rfidError Error, PopupMessage.PopupType popupType )
            {
                if ( !DO_NOT_LOG )
                {
                    try
                    {
                        _log.WriteEntry( String.Format( "{0}", Message ), System.Diagnostics.EventLogEntryType.Warning, ( int ) Error.ErrorCode );
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:
                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }

                PopupMessage.DoPopup( popupType, Message );
            }

            /// <summary>
            /// Log an error to the system error log and popup a "could not complete error" message.
            /// </summary>
            /// <param name="exception"></param>
            public static void Warning( Exception exception )
            {

                if ( exception == null )
                {
                    exception = new ArgumentNullException( "exception", "Null exception passed to Logger.Warning()" );
                }

                if ( exception is rfidLogErrorException )
                {
                    Warning( exception.InnerException );
                    return;
                }

                if ( !DO_NOT_LOG )
                {
                    RFID.RFIDInterface.rfidException rfid = exception as RFID.RFIDInterface.rfidException;
                    try
                    {
                        if ( rfid != null )
                        {
                            _log.WriteEntry( FormatEventMessage( rfid ), System.Diagnostics.EventLogEntryType.Warning, ( int ) rfid.ErrorCode.ErrorCode );
                        }
                        else
                        {
                            _log.WriteEntry( FormatEventMessage( exception ), System.Diagnostics.EventLogEntryType.Warning );
                        }
                    }
                    catch ( System.ComponentModel.Win32Exception win32 )
                    {
                        switch ( win32.NativeErrorCode )
                        {
                            case ERROR_EVENTLOG_FILE_CORRUPT:
                            case ERROR_EVENTLOG_CANT_START:
                            case ERROR_LOG_FILE_FULL:

                                MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                                DO_NOT_LOG = true;
                                break;

                            default:
                                throw;
                        }
                    }
                }

                PopupMessage.DoPopup( PopupMessage.PopupType.UnableToComplete );
            }


            public static void LogMessage( string Message )
            {
                if ( DO_NOT_LOG ) return;

                try
                {
                    _log.WriteEntry( Message, System.Diagnostics.EventLogEntryType.Information );
                }
                catch ( System.ComponentModel.Win32Exception win32 )
                {
                    switch ( win32.NativeErrorCode )
                    {
                        case ERROR_EVENTLOG_FILE_CORRUPT:
                        case ERROR_EVENTLOG_CANT_START:
                        case ERROR_LOG_FILE_FULL:

                            MessageBox.Show( String.Format( "Logging Error:\n\nError logging stopped due to error.\n\n{0}", win32.Message ) );
                            DO_NOT_LOG = true;
                            break;

                        default:
                            throw;
                    }
                }

            }

            private static string FormatEventMessage( Exception e )
            {
                return e == null ? "None." :
                String.Format(
                        "{0}\n\n" +
                    "Source Name:...{1}\n" +
                    "Thread Name:...{2}\n" +
                    "Stack Trace:\n{3}\n\n" +
                    "InnerException: {4}",
                        e.Message,
                        e.Source,
                        System.Threading.Thread.CurrentThread.Name,
                        e.StackTrace,
                        e.InnerException );
            }


        } // private static class Logger

        public static void LogWarning( string msg )
        {
            Logger.Warning( msg );
        }

        public static void LogWarning( Exception exception )
        {
            Logger.Warning( exception );
        }



        internal class CommonDialogSupport
        {
            public enum DialogType
            {
                Open,
                SaveAs,
                Folder,
                OpenRegister,
                OpenAntenna,
                OpenChannel,
            }

            private DialogResult _result = DialogResult.Cancel;
            private string _fileName = null;
            private DialogType _dialogType = DialogType.Open;


            public CommonDialogSupport( DialogType dialogType )
            {
                _dialogType = dialogType;
            }

            public DialogType Type
            {
                get { return _dialogType; }
            }

            public string FileName
            {
                get { return _fileName; }
            }

            public DialogResult Result
            {
                get { return _result; }
            }

            public DialogResult ShowDialog( )
            {
                CommonDialogThreadProc( );
                if ( Result == DialogResult.OK && FileName != null )
                {
                    _fileName = FileName;
                }

                return Result;

            }


            private void CommonDialogThreadProc( )
            {
                FileDialog fileDialog = null;
                FolderBrowserDialog folderDialog = null;
                switch ( Type )
                {
                    case DialogType.Open:
                        OpenFileDialog openDialog = new OpenFileDialog( );
                        openDialog.Title = "Open File";
                        openDialog.CheckFileExists = true;
                        openDialog.AddExtension = true;
                        openDialog.CheckPathExists = true;
                        openDialog.ValidateNames = true;
                        openDialog.DefaultExt = "rfi";
                        openDialog.RestoreDirectory = true;
                        openDialog.InitialDirectory = System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
                        openDialog.Filter = "Data files (*.rfi)|*.rfi|Config files (*.config)|*.config|All files (*.*)|*.*";
                        openDialog.FilterIndex = 0;

                        fileDialog = openDialog;
                        break;

                    case DialogType.SaveAs:
                        SaveFileDialog saveDialog = new SaveFileDialog( );
                        saveDialog.OverwritePrompt = true;
                        saveDialog.Title = "Save Data File";
                        saveDialog.CheckFileExists = false;
                        saveDialog.AddExtension = true;
                        saveDialog.CheckPathExists = true;
                        saveDialog.ValidateNames = true;
                        saveDialog.DefaultExt = "rfi";
                        saveDialog.RestoreDirectory = true;
                        saveDialog.InitialDirectory = System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
                        saveDialog.Filter = "data files (*.rfi)|*.rfi|All files (*.*)|*.*";
                        saveDialog.FilterIndex = 0;

                        fileDialog = saveDialog;
                        break;

                    case DialogType.Folder:
                        folderDialog = new FolderBrowserDialog( );
                        folderDialog.ShowNewFolderButton = true;
                        folderDialog.Description = "Select the directory to store log files.";
                        folderDialog.SelectedPath = Properties.Settings.Default.logPath;
                        break;

                    case DialogType.OpenRegister:
                        OpenFileDialog openRegDialog = new OpenFileDialog( );
                        openRegDialog.Title = "Open Excel XML Spreadsheet";
                        openRegDialog.CheckFileExists = true;
                        openRegDialog.AddExtension = true;
                        openRegDialog.CheckPathExists = true;
                        openRegDialog.ValidateNames = true;
                        openRegDialog.DefaultExt = "xls";
                        openRegDialog.RestoreDirectory = true;
                        openRegDialog.InitialDirectory = System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
                        openRegDialog.Filter = "Excel xml spreadsheet (*.xls;*.xml)|*.xls;*.xml|All files (*.*)|*.*";
                        openRegDialog.FilterIndex = 0;

                        fileDialog = openRegDialog;
                        break;

                    case DialogType.OpenAntenna:
                        OpenFileDialog openAntDialog = new OpenFileDialog( );
                        openAntDialog.Title = "Open Excel XML Spreadsheet";
                        openAntDialog.CheckFileExists = true;
                        openAntDialog.AddExtension = true;
                        openAntDialog.CheckPathExists = true;
                        openAntDialog.ValidateNames = true;
                        openAntDialog.DefaultExt = "xls";
                        openAntDialog.RestoreDirectory = true;
                        openAntDialog.InitialDirectory = System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
                        openAntDialog.Filter = "Excel xml spreadsheet (*.xls;*.xml)|*.xls;*.xml|All files (*.*)|*.*";
                        openAntDialog.FilterIndex = 0;

                        fileDialog = openAntDialog;
                        break;

                    case DialogType.OpenChannel:
                        OpenFileDialog openChannelDialog = new OpenFileDialog( );
                        openChannelDialog.Title = "Open Excel XML Spreadsheet";
                        openChannelDialog.CheckFileExists = true;
                        openChannelDialog.AddExtension = true;
                        openChannelDialog.CheckPathExists = true;
                        openChannelDialog.ValidateNames = true;
                        openChannelDialog.DefaultExt = "xls";
                        openChannelDialog.RestoreDirectory = true;
                        openChannelDialog.InitialDirectory = System.Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
                        openChannelDialog.Filter = "Excel xml spreadsheet (*.xls;*.xml)|*.xls;*.xml|All files (*.*)|*.*";
                        openChannelDialog.FilterIndex = 0;

                        fileDialog = openChannelDialog;
                        break;

                    default:
                        Logger.Warning( String.Format( "Unknown/Unsupported CommonDialogSupport.DialogType ({0}).", Type.ToString( ) ) );
                        _result = DialogResult.Abort;
                        return;
                }


                if ( fileDialog != null )
                {


                    if ( ( _result = fileDialog.ShowDialog( ) ) == DialogResult.OK )
                    {
                        _fileName = fileDialog.FileName;
                    }
                }
                else
                {
                    if ( ( _result = folderDialog.ShowDialog( ) ) == DialogResult.OK )
                    {
                        _fileName = folderDialog.SelectedPath;
                    }
                }
            }


        } // private class CommonDialogSupport


        private class ContextID
        {
            private static int _start;
            private readonly int _value;

            public int Value
            {
                get { return _value; }
            }
            static ContextID( )
            {
                _start = 100;
            }
            public ContextID( )
            {
                _value = Interlocked.Increment( ref _start );
            }


            public static bool operator ==( ContextID lhs, ContextID rhs )
            {
                return lhs.Value == rhs.Value;
            }

            public static bool operator !=( ContextID lhs, ContextID rhs )
            {
                return !( lhs == rhs );
            }

            public override bool Equals( object obj )
            {
                return ( obj is ContextID ) ? this == ( ContextID ) obj : false;
            }

            public override int GetHashCode( )
            {
                return ( int ) _value;
            }

            public override string ToString( )
            {
                return _value.ToString( );
            }

        } //private class ContextID



        /// <summary>
        /// Base clase for holding the GUI application context information.
        /// </summary>
        private abstract class ContextBag
        {
            public enum ContextType
            {
                RfidReader,
                File,
                VirtualReader,
            }

            public enum ReaderTask
            {
                Idle,
                ReadInventoryOnce,
                MonitoringInventory,
                TagAccess,
                BuildingTables,
                SaveFile,
                LoadFile,
                Export,
                Connect,
            }

            public ContextID ID;
            public ContextType Type;
            public StatusBarState.StateName StatusState;
            public String StatusMessage;
            public String FileName;
            public LakeChabotReader Reader;
            public ReaderTask CurrentTask;
            public ToolStripMenuItem MenuItem;
            public bool IsPaused;
            public BackgroundWorker Worker;
            public ExportTargets.ExportTargetClass ExportTarget;
            public GUIViewState.GUIViewClass[ ] ExportViewList;

            public ContextBag( ContextType type )
            {
                ID = new ContextID( );
                Type = type;

            }

            public string TaskName
            {
                get
                {
                    switch ( CurrentTask )
                    {
                        case ReaderTask.Idle:
                            return "Idle";

                        case ReaderTask.ReadInventoryOnce:
                            return "Inventoried";

                        case ReaderTask.MonitoringInventory:
                            return "Inventoried";

                        case ReaderTask.TagAccess:
                            return "Tag Access";

                        case ReaderTask.BuildingTables:
                            return "Indexed";

                        case ReaderTask.LoadFile:
                            return "Loading";

                        case ReaderTask.SaveFile:
                            return "Saving";

                        case ReaderTask.Export:
                            return "Exporting";

                        case ReaderTask.Connect:
                            return "Connecting";

                        default:
                            System.Diagnostics.Debug.Assert( false );
                            return "Unknown";
                    }
                }
            }

            public override int GetHashCode( )
            {
                return ID.GetHashCode( );
            }

        }

        /// <summary>
        /// Context bag for a file context
        /// </summary>
        private class ContextFile : ContextBag
        {

            public ContextFile( )
                : base( ContextType.File )
            {

            }
        }

        /// <summary>
        /// Context bag for a reader context
        /// </summary>
        private class ContextRfidReader : ContextBag
        {


            public rfidReaderID ReaderID;
            //public BackgroundWorker	Worker;
            //public ReaderTask		CurrentTask;
            public rfidReportBase CurrentReport;
            public double SingulationRateAverage;
            public int SingulationRateCount = 0;



            public ContextRfidReader( )
                : base( ContextType.RfidReader )
            {
            }
        }



        /// <summary>
        /// Context bag for a virtual reader context
        /// </summary>
        private class ContextVirtualReader : ContextBag
        {
            private const int INITIAL_QUEUE_SIZE = 2 * 1024;

            private static bool _IsVirtualReaderVisible = false;

            public static ContextVirtualReader VirtualReader = null;

            public static List<ContextRfidReader> ReadersInVR = new List<ContextRfidReader>( 0 );

            public static Queue<PacketData.PacketWrapper> PacketQueue = new Queue<PacketData.PacketWrapper>( INITIAL_QUEUE_SIZE );

            public static void DumpPacketQueue( )
            {
                System.Diagnostics.Debug.Flush( );
                System.Diagnostics.Debug.WriteLine( "\n*** Starting VR PacketQueue Dump ***" );
                int totalPackets = PacketQueue.Count;
                for ( int i = 0; i < totalPackets; i++ )
                {
                    PacketData.PacketWrapper envelope = null;
                    lock ( PacketQueue )
                    {
                        if ( PacketQueue.Count > 0 )
                            envelope = PacketQueue.Dequeue( );
                    }
                    if ( envelope == null )
                    {
                        break;
                    }
                    System.Diagnostics.Debug.WriteLine( String.Format( "{0}: [{1}] {2}", envelope.PacketNumber, envelope.ReaderName.Substring( 0, 8 ), envelope.PacketTypeName ) );
                }
                System.Diagnostics.Debug.WriteLine( "\n*** VR PacketQueue Dump Finished ***" );
                System.Diagnostics.Debug.Flush( );
            }

            public new VirtualReader Reader = null;

            public static bool IsVirtualReaderEnabled
            {
                get { return VirtualReader != null; }
            }

            public static int Count
            {
                get { return ReadersInVR.Count; }
            }

            public static bool Visible
            {
                get { return _IsVirtualReaderVisible; }
                set { _IsVirtualReaderVisible = value; }
            }


            public static void InitializeVirtualView( )
            {
                foreach ( ContextBag bag in ContextCollection.ContextList )
                {
                    if ( IsValidForVirtualReader( bag ) )
                    {
                        ReadersInVR.Add( bag as ContextRfidReader );
                    }
                }

                if ( ReadersInVR.Count >= 2 )
                    VirtualReader = new ContextVirtualReader( );

            }

            private static bool IsValidForVirtualReader( ContextBag context )
            {
                switch ( context.Type )
                {
                    case ContextType.RfidReader:
                        ContextRfidReader reader = context as ContextRfidReader;
                        System.Diagnostics.Debug.Assert( reader != null );
                        if ( reader.Reader.Mode != rfidReader.OperationMode.BoundToReader )
                        {
                            return false;
                        }

                        return true;

                    case ContextType.File:
                        return false;

                    case ContextType.VirtualReader:
                        return false;

                    default:
                        throw new Exception( "Unknown context type." );
                }
            }



            //public rfidReaderID ReaderID;
            //public BackgroundWorker	Worker;
            //public ReaderTask		CurrentTask;
            public rfidReportBase CurrentReport;
            public double SingulationRateAverage;
            public int SingulationRateCount = 0;



            private ContextVirtualReader( )
                : base( ContextType.VirtualReader )
            {
                // NOP
            }
        }


        /// <summary>
        /// Holds status bar info
        /// </summary>
        private static class StatusBarState
        {

            public enum StateType
            {
                Sticky,
                Transient,
            }

            public enum StateName
            {
                Blank,

                Ready,

                Working,

                Paused,

                Canceled,

                Infomation,

                Error,

                NoContext,

                NoDevice,

                NotImplemented,

                TagAccessOptError

            }

            public const string PausedMessage = "Operation paused...";

            public static string GetStatusBarStateText( StateName newState )
            {
                switch ( newState )
                {
                    case StateName.Blank:
                        return "";

                    case StateName.Ready:
                        return "Ready";

                    case StateName.Working:
                        return "Working";

                    case StateName.Paused:
                        return "Paused";

                    case StateName.Canceled:
                        return "Stopped";

                    case StateName.Infomation:
                        return "Notice";

                    case StateName.Error:
                        return "Error";

                    case StateName.NoDevice:
                        return "No Device";

                    case StateName.NotImplemented:
                        return "NYI";

                    case StateName.TagAccessOptError:
                        return "Status";

                    default:
                        return "Unknown";
                }
            }


            public static Color GetStatusBarStateColor( StateName newState )
            {
                switch ( newState )
                {
                    case StateName.Blank:
                        return SystemColors.Control;

                    case StateName.Ready:
                        return SystemColors.Control;

                    case StateName.Working:
                        return SystemColors.Control;

                    case StateName.Paused:
                        return Color.FromArgb( 0xAB, 0xC8, 0xF7 );

                    case StateName.Canceled:
                        return Color.FromArgb( 0xAB, 0xC8, 0xF7 );

                    case StateName.Infomation:
                        return Color.FromArgb( 0xAB, 0xC8, 0xF7 );

                    case StateName.Error:
                        return Color.Red;

                    case StateName.NoDevice:
                        return Color.Red;

                    case StateName.NotImplemented:
                        return Color.Gold;

                    case StateName.TagAccessOptError:
                        return Color.FromArgb( 0xFF , 0xFF , 0xC9 , 0 );

                    default:
                        return SystemColors.Control;
                }
            }

            public static StateType GetStatusBarStateType( StateName state )
            {
                switch ( state )
                {
                    case StateName.Blank:
                    case StateName.Ready:
                    case StateName.Working:
                    case StateName.Paused:
                    case StateName.Canceled:
                    case StateName.NoDevice:
                        return StateType.Sticky;

                    case StateName.Infomation:
                    case StateName.Error:
                    case StateName.NoContext:
                    case StateName.NotImplemented:
                    case StateName.TagAccessOptError:
                        return StateType.Transient;

                    default:
                        System.Diagnostics.Debug.Assert( false, "Unknown Result Bar State" );
                        return StateType.Sticky;
                }
            }

        } //private static class StatusBarState


        private static class ExportTargets
        {
            public class ExportTargetClass
            {
                public TargetType ExportType;
                public string Name;

                public ExportTargetClass( TargetType exportType, string name )
                {
                    ExportType = exportType;
                    Name = name;
                }

                public override string ToString( )
                {
                    return Name;
                }
            }

            public enum TargetType
            {
                Excel,
                //		IE
            }

            public static List<ExportTargetClass> SupportedTypes;

            static ExportTargets( )
            {
                SupportedTypes = new List<ExportTargetClass>( Enum.GetNames( typeof( TargetType ) ).Length );

                foreach ( TargetType t in Enum.GetValues( typeof( TargetType ) ) )
                {
                    switch ( t )
                    {
                        case TargetType.Excel:
                            SupportedTypes.Add( new ExportTargetClass( t, "Microsoft Excel 2003 (*.xls)" ) );
                            break;

                        //					case TargetType.IE:
                        //						SupportedTypes.Add(new ExportTargetClass(t, "Microsoft Internet Explorer (*.htm)"));
                        //						break;

                        default:
                            System.Diagnostics.Debug.Assert( false );
                            break;
                    }
                }


            }
        } // private static class ExportTargets


        /// <summary>
        /// 
        /// </summary>
        private static class GUIViewState
        {
            static GUIViewState( )
            {
                InitializeViewCollection( );
                InitializeDefault( );
            }

            /// <summary>
            /// The GUIView of the current context
            /// </summary>
            public static GUIView View;
            public static GUIView Default = GUIView.StandardView;
            public static Dictionary<GUIView, GUIViewClass> ViewCollection;
            public static Dictionary<GUIView, Panel> GuiPanelCollection;

            public static GridControl.GridType MapGuiViewToGridType( GUIViewClass viewClass )
            {
                return MapGuiViewToGridType( viewClass.View );
            }

            public static GridControl.GridType MapGuiViewToGridType( GUIView view )
            {
                switch ( view )
                {
                    case GUIView.StandardView:
                        return GridControl.GridType.StandardView;

                    case GUIView.RawPackets:
                        return GridControl.GridType.RawPackets;

                    case GUIView.ReaderRequests:
                        return GridControl.GridType.ReaderRequests;

                    case GUIView.ReaderCommands:
                        return GridControl.GridType.ReaderCommands;

                    case GUIView.ReaderAntennaCycles:
                        return GridControl.GridType.ReaderAntennaCycles;

                    case GUIView.AntennaPacket:
                        return GridControl.GridType.AntennaPacket;

                    case GUIView.InventoryCycle:
                        return GridControl.GridType.InventoryCycle;

                    case GUIView.InventoryRounds:
                        return GridControl.GridType.InventoryRounds;

                    case GUIView.InventoryParameters:
                        return GridControl.GridType.InventoryParameters;

                    case GUIView.InventoryData:
                        return GridControl.GridType.TagAccess;

                    case GUIView.BadPackets:
                        return GridControl.GridType.BadPackets;

                    case GUIView.InventoryCycleDiagnostics:
                        return GridControl.GridType.InventoryCycleDiag;

                    case GUIView.InventoryRoundDiagnostics:
                        return GridControl.GridType.InventoryRoundDiag;

                    case GUIView.EPCDataDiagnostics:
                        return GridControl.GridType.TagDataDiagnostics;

                    case GUIView.ReadRateData:
                        return GridControl.GridType.ReadRate;

                    default:
                        throw new InvalidEnumArgumentException( "view", ( int ) view, view.GetType( ) );
                }
            }

            public class GUIViewClass
            {
                private GUIView _view;
                private GUIView _parent;
                private GUIViewType _type;
                private string _menuCaption;
                private ToolStripItem _menu;

                public GUIViewClass( GUIView view, GUIView parent, GUIViewType type, string menuCaption )
                {
                    _type = type;
                    _parent = parent;
                    _view = view;
                    _menuCaption = menuCaption;
                }

                public GUIView View { get { return _view; } }
                public GUIView Parent { get { return _parent; } }
                public GUIViewType ViewType { get { return _type; } }
                public string MenuCaption { get { return _menuCaption; } }
                public string ViewName { get { return Enum.GetName( _view.GetType( ), _view ); } }
                public ToolStripItem Menu
                {
                    get { return _menu; }
                    set { _menu = value; }
                }

                public bool HasPanel
                {
                    get
                    {
                        switch ( ViewType )
                        {

                            case GUIViewType.Basic:
                            case GUIViewType.Specialized:
                                return true;

                            case GUIViewType.Root:
                            case GUIViewType.Command:
                            case GUIViewType.Header:
                            case GUIViewType.Break:
                                return false;

                            default:
                                throw new InvalidEnumArgumentException( "ViewType", ( int ) ViewType, typeof( GUIViewType ) );
                        }
                    }
                }

                public override string ToString( )
                {
                    return _menuCaption;
                }
            } //public class GUIViewClass


            public enum GUIViewType
            {
                Root,

                Basic,

                Specialized,

                Command,

                Header,

                Break,
            }


            public enum GUIView
            {
                Root,

                SummaryView,

                StandardView,

                ReaderProtocol,

                Break1,

                CalculateCommand,

                Break2,

                SummaryViewCollection,

                InventoryData,

                ReaderRequests,	// not used

                ReaderCommands,

                ReaderAntennaCycles,

                AntennaPacket,	// not used

                InventoryCycle,

                InventoryRounds,

                InventoryParameters,	// not used


                DiagnosticsViewCollection,

                RawPackets,

                BadPackets,

                InventoryCycleDiagnostics,

                InventoryRoundDiagnostics,

                EPCDataDiagnostics,

                PerformanceViewCollection,

                ReadRateData,

            }


            public static string ViewName( GUIView view )
            {
                return Enum.GetName( view.GetType( ), view );
            }


            public static Control GetViewControl( )
            {
                return GUIViewState.GuiPanelCollection[ GUIViewState.View ].Controls[ GUIViewState.ViewName( GUIViewState.View ) ];
            }


            /// <summary>
            /// Recursive function to build the view menu tree
            /// </summary>
            /// <param name="root"></param>
            /// <param name="result"></param>
            /// <returns></returns>
            public static List<ToolStripItem> GetChildMenuItems( GUIViewClass root, List<ToolStripItem> result )
            {
                foreach ( GUIViewClass view in ViewCollection.Values )
                {
                    if ( view.Parent == root.View )
                    {
                        switch ( view.ViewType )
                        {
                            case GUIViewType.Root:
                                break;

                            case GUIViewType.Basic:
                            case GUIViewType.Specialized:
                            case GUIViewType.Command:
                            case GUIViewType.Header:
                                ToolStripMenuItem menuItem = new ToolStripMenuItem( view.MenuCaption );
                                menuItem.Tag = view.View;
                                menuItem.DropDownItems.AddRange( GetChildMenuItems( view, new List<ToolStripItem>( 50 ) ).ToArray( ) );
                                view.Menu = menuItem;
                                result.Add( menuItem );
                                break;

                            case GUIViewType.Break:
                                ToolStripSeparator line = new ToolStripSeparator( );
                                line.Tag = view.View;
                                view.Menu = line;
                                result.Add( line );
                                break;

                            default:
                                break;
                        }
                    }
                }
                return result;
            }

            public static ToolStripItem[ ] GuiStateMenuItems
            {
                get
                {
                    return GetChildMenuItems( ViewCollection[ GUIView.Root ], new List<ToolStripItem>( 50 ) ).ToArray( );
                }
            }

            private static void InitializeViewCollection( )
            {
                ViewCollection = new Dictionary<GUIView, GUIViewClass>( 30 );

                foreach ( GUIView view in Enum.GetValues( typeof( GUIView ) ) )
                {
                    switch ( view )
                    {
                        case GUIView.Root:
                            ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Root, "root" ) );
                            break;

                        case GUIView.SummaryView:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Basic, "Summary View" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.StandardView:
                            ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Basic, "Standard View" ) );
                            break;

                        case GUIView.ReaderProtocol:
                            ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Basic, "Protocol Trace" ) );
                            break;

                        case GUIView.Break1:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Break, "" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.CalculateCommand:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Command, "Build Post-Capture Views" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.Break2:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Break, "" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.SummaryViewCollection:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Header, "Summary Views" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;


                        case GUIView.ReaderRequests:
                            //ViewCollection.Add(view, new GUIViewClass(view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Issued Commands"));
                            break;

                        case GUIView.ReaderCommands:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Command Summary" ) );//Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.ReaderAntennaCycles:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Antenna Cycle Summary" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.AntennaPacket:
                            //ViewCollection.Add(view, new GUIViewClass(view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Antenna Overview"));
                            break;

                        case GUIView.InventoryCycle:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Inventory Cycle Summary" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.InventoryRounds:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Inventory Round Summary" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.InventoryParameters:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Inventory Parameters" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.InventoryData:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.SummaryViewCollection, GUIViewType.Specialized, "Tag Access" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.DiagnosticsViewCollection:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Header, "Diagnostics Views" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.RawPackets:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.DiagnosticsViewCollection, GUIViewType.Specialized, "All Packets (Raw Format)" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;


                        case GUIView.BadPackets:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.DiagnosticsViewCollection, GUIViewType.Specialized, "Invalid Packets" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.InventoryCycleDiagnostics:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.DiagnosticsViewCollection, GUIViewType.Specialized, "Inventory Cycle Diagnostics" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.InventoryRoundDiagnostics:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.DiagnosticsViewCollection, GUIViewType.Specialized, "Inventory Round Diagnostics" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.EPCDataDiagnostics:
                            //						ViewCollection.Add(view, new GUIViewClass(view, GUIView.DiagnosticsViewCollection, GUIViewType.Specialized, "Singulation Diagnostics"));
                            break;

                        case GUIView.PerformanceViewCollection:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.Root, GUIViewType.Header, "Performance Views" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        case GUIView.ReadRateData:
                            //ViewCollection.Add( view, new GUIViewClass( view, GUIView.PerformanceViewCollection, GUIViewType.Specialized, "Singulation Rate Data" ) ); //Mod by FJ for hiding Summary View Function, 2018-04-11
                            break;

                        default:
                            throw new InvalidEnumArgumentException( "view", ( int ) view, typeof( GUIView ) );
                    }
                }
            }

            public static void InitializeDefault( )
            {
                Default = GUIView.SummaryView;
                string defView = Properties.Settings.Default.defaultView;
                if ( defView != null && defView != "" )
                {
                    foreach ( GUIViewClass viewClass in ViewCollection.Values )
                    {
                        if ( viewClass.MenuCaption == defView )
                            Default = viewClass.View;
                    }
                }
            }

            public static void InitilizeGuiPanels( ToolStripContainer container )
            {
                container.SuspendLayout( );

                // First, clean up any existing panels
                if ( GuiPanelCollection != null )
                {
                    foreach ( Panel p in GuiPanelCollection.Values )
                    {
                        System.Diagnostics.Debug.Assert( p.Parent.Parent == container );
                        p.Parent.Controls.Remove( p );
                        p.Dispose( );
                    }
                    GuiPanelCollection.Clear( );
                }
                GuiPanelCollection = new Dictionary<GUIView, Panel>( );


                foreach ( GUIViewClass viewClass in GUIViewState.ViewCollection.Values )
                {

                    GUIView view = viewClass.View;

                    //ToolStripContentPanel basePanel = new ToolStripContentPanel();
                    Panel basePanel = new Panel( );

                    switch ( view )
                    {

                        case GUIView.StandardView:
                        case GUIView.ReaderRequests:
                        case GUIView.RawPackets:
                        case GUIView.ReaderCommands:
                        case GUIView.ReaderAntennaCycles:
                        case GUIView.AntennaPacket:
                        case GUIView.InventoryCycle:
                        case GUIView.InventoryRounds:
                        case GUIView.InventoryParameters:
                        case GUIView.BadPackets:
                        case GUIView.InventoryCycleDiagnostics:
                        case GUIView.InventoryRoundDiagnostics:
                        case GUIView.InventoryData:
                        case GUIView.EPCDataDiagnostics:
                        case GUIView.ReadRateData:
                            GridControl grid = new GridControl( MapGuiViewToGridType( viewClass ) );
                            grid.Name = viewClass.ViewName;
                            basePanel.Controls.Add( grid );
                            break;


                        case GUIView.SummaryView:
                            basePanel.AutoScroll = false;
                            SummaryControl summary = new SummaryControl( );
                            summary.Name = viewClass.ViewName;
                            basePanel.Controls.Add( summary );
                            break;

                        case GUIView.ReaderProtocol:
                            basePanel.AutoScroll = false;
                            ProtocolControl protocol = new ProtocolControl( );
                            protocol.Name = viewClass.ViewName;
                            basePanel.Controls.Add( protocol );
                            break;

                        default:
                            if ( viewClass.HasPanel )
                            {
                                Label l = new Label( );
                                l.Text = String.Format( "The {0} view is not yet implemented.", Enum.GetName( view.GetType( ), view ) );
                                l.Name = Enum.GetName( view.GetType( ), view );
                                l.Location = new Point( 2, 2 );
                                l.AutoSize = true;
                                basePanel.Controls.Add( l );
                            }
                            break;
                    }

                    basePanel.TabIndex = 0;
                    basePanel.Dock = DockStyle.Fill;
                    basePanel.Visible = false;


                    if ( viewClass.HasPanel )
                    {
                        GuiPanelCollection.Add( view, basePanel );
                        container.ContentPanel.Controls.Add( basePanel );
                    }
                }
                container.ResumeLayout( );
            }

        } // private static class GUIViewState 



        private class ContextCollection : KeyedCollection<ContextID, ContextBag>
        {
            public static ContextCollection ContextList = new ContextCollection( );
            public static ContextBag CurrentContext;

            public static bool HasCurrentContext
            {
                get { return CurrentContext != null; }
            }


            public static ContextRfidReader ContextAsRfidReader
            {
                get { return CurrentContext as ContextRfidReader; }
            }


            public static ContextFile ContextAsFile
            {
                get { return CurrentContext as ContextFile; }
            }


            public static ContextVirtualReader ContextAsVirtualReader
            {
                get { return CurrentContext as ContextVirtualReader; }
            }

            public ContextCollection( ) : base( ) { }

            protected override ContextID GetKeyForItem( ContextBag context )
            {
                return context.ID;
            }
        }




        /// <summary>
        /// Constructor for mainForm
        /// </summary>
        public mainForm( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "mainForm GUI ThreadID: {0:x}", System.Threading.Thread.CurrentThread.ManagedThreadId ) );
#endif
            HighResolutionTimer.NoOp( ); // Force the timer to init.
            InitializeComponent( );


            Logger.LogMessage( "MTI RFID Explorer Starting" );

            InitializeViews( );

            SetCurrentView( GUIViewState.Default );

            //Initial the value
            toolsSpacerStripMenuItem.Visible = false;
            this.bErrorKeepRunning = true;            

            LakeChabotReader.EnableLogging = Properties.Settings.Default.enableLogging;
            LakeChabotReader.LogPath = Properties.Settings.Default.logPath;
            LakeChabotReader.NoTempFileAccess = Properties.Settings.Default.noTempFile;

            VirtualReader.NoTempFileAccess = Properties.Settings.Default.noTempFile;

            rfid.Constants.RadioPowerState powerState = rfid.Constants.RadioPowerState.UNKNOWN;

            //LakeChabotReader.ReaderDataFormatValue		dataFormat	= LakeChabotReader.ReaderDataFormatValue.Unknown;

            rfid.Constants.RadioOperationMode opsMode = rfid.Constants.RadioOperationMode.UNKNOWN;
            rfid.Constants.MacRegion          region  = rfid.Constants.MacRegion.UNKNOWN;
            
            RFID.RFIDInterface.RegisterData.InventoryAlgorithmValue algorithm = 
                                        RFID.RFIDInterface.RegisterData.InventoryAlgorithmValue.Unknown;


            // Algorithm
            try
            {
                algorithm = ( RFID.RFIDInterface.RegisterData.InventoryAlgorithmValue ) Properties.Settings.Default.startupInventoryAlgorithm;

            }
            catch ( Exception e )
            {
                Logger.Warning( String.Format( "Error in startup algorithm ({0})", e.Message ), PopupMessage.PopupType.OperatingRegion );
                Properties.Settings.Default.startupInventoryAlgorithm = ( int ) RFID.RFIDInterface.RegisterData.InventoryAlgorithmValue.Unknown;
                Properties.Settings.Default.Save( );
                algorithm = RFID.RFIDInterface.RegisterData.InventoryAlgorithmValue.Unknown;
            }


            // Power State
            try
            {
                string state = Properties.Settings.Default.startupPowerState;
                if ( !String.IsNullOrEmpty( state ) )
                {
                    powerState =
                        ( rfid.Constants.RadioPowerState ) Enum.Parse( typeof( rfid.Constants.RadioPowerState ), state );
                }
            }
            catch ( Exception e )
            {
                Logger.Warning( String.Format( "Error reading startup power state ({0})", e.Message ) );
                Properties.Settings.Default.startupPowerState = "";
                Properties.Settings.Default.Save( );
            }


            // Startup Mode
            try
            {
                string mode = Properties.Settings.Default.startupOperationalMode;

                if ( !String.IsNullOrEmpty( mode ) )
                {
                    opsMode = ( rfid.Constants.RadioOperationMode ) Enum.Parse( typeof( rfid.Constants.RadioOperationMode ), mode );
                }
            }
            catch ( Exception e )
            {
                Logger.Warning( String.Format( "Error reading startup operation mode ({0})", e.Message ) );
                Properties.Settings.Default.startupOperationalMode = "";
                Properties.Settings.Default.Save( );
            }

            // Link Profile
            string startupProfileID = Properties.Settings.Default.startupProfileID;


            //
            new PropertyBag( ).ValidateOrdinalValues( );
            new BadPacket( ).ValidateOrdinalValues( );
            new ReaderRequest( ).ValidateOrdinalValues( );
            new PacketStream( ).ValidateOrdinalValues( );
            new ReaderCommand( ).ValidateOrdinalValues( );
            new ReaderAntennaCycle( ).ValidateOrdinalValues( );
            new AntennaPacket( ).ValidateOrdinalValues( );
            new InventoryCycle( ).ValidateOrdinalValues( );
            new InventoryRound( ).ValidateOrdinalValues( );
            new TagRead( ).ValidateOrdinalValues( );
            new TagInventory( ).ValidateOrdinalValues( );
            new ReadRate( ).ValidateOrdinalValues( );

            try
            {
                StringBuilder sbErrors = new StringBuilder( );
                using ( LakeChabotReader temp = new LakeChabotReader( ) )
                {
                    foreach ( rfidReaderID reader in LakeChabotReader.FindReaders( ) )
                    {
                        ContextRfidReader context = new ContextRfidReader( );
                        context.StatusState = StatusBarState.StateName.Ready;
                        context.IsPaused = false;
                        context.ReaderID = reader;
                        context.Reader = new LakeChabotReader( reader );
                        //						context.Reader.PacketArrival +=new LakeChabotReader.PacketArrivalHandler(Reader_PacketArrival);
                        context.MenuItem = new ToolStripMenuItem( String.Format( "{0}", reader.Name ) );
                        context.MenuItem.ToolTipText = reader.Location;
                        context.MenuItem.Tag = context.ID;
                        context.MenuItem.Click += new EventHandler( deviceMenuItem_Click );
                        context.Worker = new BackgroundWorker( );
                        context.StatusState = StatusBarState.StateName.Ready;
                        context.StatusMessage = String.Format( "{0} Activated.", reader.Name );
                        context.FileName = null;
                        DeviceToolStripMenuItem.DropDownItems.Add( context.MenuItem );

                        //Show "Device Interface" in status bar.
                        toolStripDropDownBtnDevice.Text = String.Format( "Device: {0}     ", reader.Name );

                        if ( region != rfid.Constants.MacRegion.UNKNOWN )
                        {
                            try
                            {
                                context.Reader.RegulatoryRegion = region;
                            }
                            catch ( Exception e )
                            {
                                Logger.Warning( "Unable to set region at startup: " + e.Message );
                            }
                        }


                        if ( powerState != rfid.Constants.RadioPowerState.UNKNOWN )
                        {
                            try
                            {
                                context.Reader.PowerState = powerState;
                            }
                            catch ( Exception e )
                            {
                                Logger.Warning( "Unable to set startup power state: " + e.Message );
                            }
                        }


                        ContextCollection.ContextList.Add( context );
                    }
                }

                if ( DeviceToolStripMenuItem.DropDownItems.Count == 0 )
                    DeviceToolStripMenuItem.Enabled = false;


                if ( sbErrors.Length > 0 )
                {
                    PopupMessage.DoPopup( PopupMessage.PopupType.RegisterErrors, sbErrors.ToString( ) );
                }


                CreateVirtualReader( );

            }
            catch ( Exception exp )
            {
                System.IO.FileNotFoundException inner = exp.InnerException as System.IO.FileNotFoundException;
                if ( inner != null && inner.Message == "The specified module could not be found. (Exception from HRESULT: 0x8007007E)" )
                {
                    PopupMessage.Popup( PopupMessage.PopupType.NoLibrary );
                    throw new rfidException( rfidErrorCode.LibraryNotFound );
                }
                else
                {
                    PopupMessage.Popup( PopupMessage.PopupType.StartupError, exp.Message );
                }
                throw;
            }


            //Device Count
            switch ( ContextCollection.ContextList.Count )
            {
                case 0:
                    UpdateStatusBar( StatusBarState.StateName.NoDevice, "No RFID readers found..." );
                    break;

                case 1:
                default:	// select the first device by default
                    ActivateContext( ContextCollection.ContextList[ 0 ] );
                    SetCurrentView( GUIViewState.Default );

                    //clark not sure. Wait firmware support this function.
                    ////clark 2011.4.26   use ini to get region item
                    //IniFile clsIni = new IniFile
                    //            (
                    //                System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Data.ini"
                    //            );
                    //
                    //UInt32 EU_Value = clsIni.ReadInteger("Region", "EU", (byte)rfid.Constants.MacRegion.UNKNOWN);
                    //
                    //if (oemData == EU_Value)
                    //{
                    //
                    //    if (readerContext != null && !readerContext.Worker.IsBusy)
                    //    {
                    //
                    //        ////////////// Disable Channels //////////////////
                    //        Int32[] channels = new Int32[6] { 4, 5, 7, 8, 10, 11 };
                    //        Source_FrequencyBandList channelList = new Source_FrequencyBandList();
                    //
                    //        channelList.load(LakeChabotReader.MANAGED_ACCESS, readerContext.Reader.ReaderHandle);
                    //
                    //        for (int i = 0; i < 6; i++)
                    //        {
                    //            channelList[ channels[i] ].State = Source_FrequencyBand.BandState.DISABLED;
                    //
                    //            channelList[ channels[i] ].store(LakeChabotReader.MANAGED_ACCESS, 
                    //                                             readerContext.Reader.ReaderHandle);
                    //            //Console.WriteLine("Disable channel:{0}", channels[i]);
                    //        }                            
                    //    }
                    //}


                    break;

            }//switcg

            
        }



        private void InitializeViews( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            viewToolStripMenuItem.DropDownItems.AddRange( GUIViewState.GuiStateMenuItems );
            foreach ( GUIViewState.GUIViewClass guiClass in GUIViewState.ViewCollection.Values )
            {
                if ( guiClass.Menu != null )
                {
                    switch ( guiClass.ViewType )
                    {

                        case GUIViewState.GUIViewType.Basic:
                            guiClass.Menu.Click += new EventHandler( viewBasicStripMenuItem_Click );
                            break;

                        case GUIViewState.GUIViewType.Specialized:
                            guiClass.Menu.Click += new EventHandler( viewSpecialStripMenuItem_Click );
                            break;

                        case GUIViewState.GUIViewType.Command:
                            guiClass.Menu.Click += new EventHandler( viewCommandStripMenuItem_Click );
                            break;

                        case GUIViewState.GUIViewType.Root:
                        case GUIViewState.GUIViewType.Header:
                        case GUIViewState.GUIViewType.Break:
                        default:
                            break;
                    }
                }
            }

            GUIViewState.InitilizeGuiPanels( toolStripContainer1 );

        }


        protected override void OnActivated( EventArgs e )
        {
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() TimeStamp = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, DateTime.Now.Ticks ) );
            base.OnActivated( e );
            ControlPanelForm.SetTopMost( true );
        }


        protected override void OnDeactivate( EventArgs e )
        {
            base.OnDeactivate( e );
            ControlPanelForm.SetTopMost( false );

        }

        protected override void OnResize( EventArgs e )
        {
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() TimeStamp = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, DateTime.Now.Ticks ) );
            base.OnResize( e );
            ControlPanelForm.ShowControlPanel( this.WindowState != FormWindowState.Minimized );
        }



        /// <summary>
        /// Parameters for a QueueWork (helper class)
        /// </summary>
        private class ThreadPoolCallParameters
        {
            public String FunctionName;
            public Object[ ] Args;
            public System.Diagnostics.StackTrace StackTrace = null;
        }




        /// <summary>
        /// Schedules a function call by using a ThreadPool thread.
        /// </summary>
        /// <example>QueueWork &lt;UpdateStatusBarDelegate\&gt;("UpdateStatusBar", StatusBarState.StateName.Error, "This is a test message");</example>
        /// <typeparam name="T">A delegate for the function to call.</typeparam>
        /// <param name="name">The function name as a string.</param>
        /// <param name="args">Parameters for the function.</param>
        private void QueueWork<T>( string name, params Object[ ] args )
        {
            ThreadPoolCallParameters P = new ThreadPoolCallParameters( );
            P.FunctionName = name;
            P.Args = args;
#if DEBUG
            P.StackTrace = new System.Diagnostics.StackTrace( );
#endif
            System.Diagnostics.Debug.WriteLine( String.Format( "About to QueueUserWorkItem ({0}), ThreadID: {1}", name, System.Threading.Thread.CurrentThread.ManagedThreadId ) );
            //ThreadPool.QueueUserWorkItem(new WaitCallback(zipFileName<T>), (Object)(args));
            ThreadPool.QueueUserWorkItem( new WaitCallback( fn<T> ), ( Object ) P );
        }

        /// <summary>
        /// Calls a functions by creating a delegate of type T and then invoking it. 
        /// </summary>
        /// <remarks>zipFileName has signature required by ThreadPool.QueueUserWorkItem WaitCallback delegate.</remarks>
        /// <typeparam name="T">A delegate for the function to call</typeparam>
        /// <example> zipFileName<ProgressTaskFinishedDelegate>((Object)(new Object[] { "ProgressTaskFinished" }));</example>
        /// <param name="contextPassed">An array of objects cast to an object. The first index [0]
        /// is a string containing the name of the function to invoke. Any parameters for the function 
        /// are stored in index [1] to [n].
        /// </param>
        private void fn<T>( object contextPassed )
        {
            Object[ ] context = contextPassed as Object[ ];
            ThreadPoolCallParameters callParams = contextPassed as ThreadPoolCallParameters;
            string functionName = null == context ? callParams.FunctionName : ( string ) context[ 0 ];
            object[ ] paramArray = null == context ? callParams.Args : new object[ context.Length - 1 ];
            if ( context != null ) Array.Copy( context, 1, paramArray, 0, paramArray.Length );
            Delegate theDelegate = Delegate.CreateDelegate( typeof( T ), this, functionName );
            if ( theDelegate == null )
            {
                System.Diagnostics.Debug.Assert( theDelegate != null, "zipFileName<T>() Did not create a delegate." );
                throw new ApplicationException( String.Format( "Unable to create the delegate needed to invoke {0} with {1} parameters.", functionName, paramArray.Length ) );
            }
            System.Diagnostics.Debug.WriteLine( String.Format( "Invoking {0} with {1} parameters.", functionName, paramArray.Length ) );
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace( );
            System.Diagnostics.StackFrame caller = trace.GetFrame( 1 );
            try
            {

                if ( paramArray.Length == 0 )
                    Invoke( theDelegate );
                else
                    Invoke( theDelegate, paramArray );
            }
            catch ( Exception e )
            {
                System.Diagnostics.Debug.Assert( false, String.Format( "Exception in function {0}: {1}", functionName, e.Message ) );
                System.Diagnostics.Debug.WriteLine( "***Exception Stack Trace***" );
                System.Diagnostics.Debug.WriteLine( e.StackTrace );
                if ( callParams != null )
                {
                    System.Diagnostics.Debug.WriteLine( "***Caller Stack Trace***" );
                    System.Diagnostics.Debug.Write( callParams.StackTrace.ToString( ) );
                }
                throw e;
            }
        }



        /*****************************************************************************
         * 
         * P U B L I C   P R O P E R I T I E S
         * 
         *****************************************************************************/


        public LakeChabotReader ActiveReader
        {
            get
            {
                ContextRfidReader context = ContextCollection.ContextAsRfidReader;
                return null == context ? null : context.Reader;
            }

        }

        public FunctionControl ActiveControler
        {
            get
            {
                ContextBag context = ContextCollection.CurrentContext;
                if ( null == context ) return null;

                switch ( context.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        return ( ( ContextRfidReader ) context ).Reader.FunctionController;


                    case ContextBag.ContextType.VirtualReader:
                        return ( ( ContextVirtualReader ) context ).Reader.FunctionController;

                    case ContextBag.ContextType.File:
                    default:
                        return null;
                }


            }
        }

        public bool IsVirtualReaderActiveContext
        {
            get { return ContextCollection.ContextAsVirtualReader != null; }
        }


        /*****************************************************************************
         * 
         * P U B L I C   M E T H O D S
         * 
         *****************************************************************************/

        /// <summary>
        /// Used to bind the FunctionStateChanged event of each context to the Control Panel.
        /// Only called once by the control panel.
        /// </summary>
        /// <param name="target"></param>
        public void BindAllFunctionControlers( EventHandler target, bool bBind )
        {
            foreach ( ContextBag bag in ContextCollection.ContextList )
            {
                switch ( bag.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        ContextRfidReader reader = bag as ContextRfidReader;

                        if (bBind == true)
                            reader.Reader.FunctionController.FunctionStateChanged += target;
                        else
                            reader.Reader.FunctionController.FunctionStateChanged -= target;
                        break;

                    case ContextBag.ContextType.File:
                        break;

                    case ContextBag.ContextType.VirtualReader:
                        ContextVirtualReader virtualContext = bag as ContextVirtualReader;
                        if (bBind == true)
                            virtualContext.Reader.FunctionController.FunctionStateChanged += target;
                        else
                            virtualContext.Reader.FunctionController.FunctionStateChanged -= target;
                        break;

                    default:
                        System.Diagnostics.Debug.Assert( false );
                        break;
                }

            }
        }


        public void FeatureNotImplemented( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( FeatureNotImplemented ) );
                return;
            }
            UpdateStatusBar( StatusBarState.StateName.NotImplemented, "Requested feature is not implemented." );

        }


        public void ClearSessionData( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( ClearSessionData ) );
                return;
            }

            ContextBag context = ContextCollection.CurrentContext;
            if ( context != null )
            {
                switch ( context.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        ClearSessionData( ContextCollection.ContextAsRfidReader );
                        break;

                    case ContextBag.ContextType.File:
                        break;

                    case ContextBag.ContextType.VirtualReader:
                        foreach ( ContextRfidReader r in ContextVirtualReader.ReadersInVR )
                        {
                            ClearSessionData( r );
                        }
                        ClearSessionData( ContextCollection.ContextAsVirtualReader );
                        break;

                    default:
                        break;
                }
            }

            GridControl grid = GUIViewState.GetViewControl( ) as GridControl;
            if ( grid != null )
            {
                grid.Clear( );
            }

            ProtocolControl pc = GUIViewState.GetViewControl( ) as ProtocolControl;
            if ( pc != null )
            {
                pc.Clear( );
            }

            UpdateStatusBar( StatusBarState.StateName.Ready );

            //Mod by FJ for hiding Summary View Function, 2018-04-11
            //GUIViewState.GUIViewClass viewClass = GUIViewState.ViewCollection[ GUIViewState.GUIView.SummaryView ];
            //( ( SummaryControl ) GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.SummaryView ].Controls[ viewClass.ViewName ] ).Clear( );
            //End by FJ for hiding Summary View Function, 2018-04-11


        }


        private void ClearSessionData( ContextRfidReader rfidContext )
        {

            if ( rfidContext == null )
                throw new ArgumentNullException( "rfidContext" );

            if ( rfidContext.Worker.IsBusy )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoClearWhenBusy );
                return;
            }
            else
            {
                rfidContext.Reader.ClearSession( );
                rfidContext.SingulationRateCount = 0;
                rfidContext.SingulationRateAverage = 0;
                rfidContext.CurrentReport = null;
            }
        }


        private void ClearSessionData( ContextVirtualReader virtualContext )
        {

            if ( virtualContext == null )
                throw new ArgumentNullException( "virtualContext" );

            if ( virtualContext.Worker.IsBusy )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoClearWhenBusy );
                return;
            }
            else
            {
                virtualContext.Reader.ClearSession( );
                virtualContext.SingulationRateCount = 0;
                virtualContext.SingulationRateAverage = 0;
                virtualContext.CurrentReport = null;
            }
        }

        public void PauseCurrentOperation( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( PauseCurrentOperation ) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;

            if ( readerContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            if ( !readerContext.Worker.IsBusy )
            {
                return;
            }

            //clark 2011.5.30
            if (LakeChabotReader.MANAGED_ACCESS.API_ControlPause() != Result.OK)
            {
                PopupMessage.Popup(PopupMessage.PopupType.UnableToComplete);
                return;
            }

            SaveStatusBarState( readerContext );
            readerContext.IsPaused = true;
            //clark 2011.2.8 
            toolStripButton5.Text = "Resume";           
            UpdateStatusBar( StatusBarState.StateName.Paused, StatusBarState.PausedMessage );
        }


        public void ContinueAfterPause( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( ContinueAfterPause ) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if ( readerContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            if ( !readerContext.Worker.IsBusy )
                return;

            //clark 2011.5.30
            if (LakeChabotReader.MANAGED_ACCESS.API_ControlResume() != Result.OK)
            {
                PopupMessage.Popup(PopupMessage.PopupType.UnableToContinue);
                return;
            }

            readerContext.IsPaused = false;
            //clark 2011.2.8 
            toolStripButton5.Text = "Pause";
            RestoreStatusBarState( readerContext );
        }



        private void StartBuildTables( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( this.InvokeRequired )
            {
                Invoke( new MethodInvoker( StartBuildTables ) );
                return;
            }


            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "StartBuildTables() called without a current context." );
                return;
            }

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    StartBuildTables( ContextCollection.ContextAsRfidReader );
                    break;

                case ContextBag.ContextType.VirtualReader:
                    foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                    {
                        if ( temp.Reader.TableResult == TableState.BuildRequired )
                        {
                            StartBuildTables( temp );
                            ( ( ContextVirtualReader ) context ).Reader.AddActiveReader( );
                        }
                    }
                    StartBuildTables( ContextCollection.ContextAsVirtualReader );
                    break;

                default:
                    System.Diagnostics.Debug.Assert( false );
                    Logger.Warning( String.Format( "StartBuildTables() called with wrong context ({0})", Enum.GetName( ContextCollection.CurrentContext.Type.GetType( ), ContextCollection.CurrentContext.Type ) ) );
                    return;
            }
        }


        private delegate void StartBuildTablesDelegate( ContextRfidReader readerContext );
        private void StartBuildTables( ContextRfidReader readerContext )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( readerContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new ArgumentNullException( "readerContext" ) );
                return;
            }

            if ( this.InvokeRequired )
            {
                Invoke( new StartBuildTablesDelegate( StartBuildTables ), readerContext );
                return;
            }


            if ( readerContext.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }

            ProgressReset( );

            UpdateStatusBar( StatusBarState.StateName.Working, "Creating indexes..." );

            readerContext.Worker = new BackgroundWorker( );
            readerContext.Worker.WorkerReportsProgress = true;
            readerContext.Worker.WorkerSupportsCancellation = false;
            readerContext.Worker.DoWork += new DoWorkEventHandler( BuildTables );
            readerContext.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            readerContext.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            readerContext.Worker.RunWorkerAsync( readerContext );

            ResetToolBar( );
        }

        private delegate void StartBuildTablesDelegate3( ContextVirtualReader virtualContext );
        private void StartBuildTables( ContextVirtualReader virtualContext )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( virtualContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new ArgumentNullException( "virtualContext" ) );
                return;
            }

            if ( !ContextVirtualReader.IsVirtualReaderEnabled )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new Exception( "VirtualReader is not enabled; but StartBuildTables called with VR context." ) );
                return;
            }


            if ( this.InvokeRequired )
            {
                Invoke( new StartBuildTablesDelegate3( StartBuildTables ), virtualContext );
                return;
            }


            if ( virtualContext.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }

            ProgressReset( );

            UpdateStatusBar( StatusBarState.StateName.Working, "Creating indexes..." );

            virtualContext.Worker = new BackgroundWorker( );
            virtualContext.Worker.WorkerReportsProgress = true;
            virtualContext.Worker.WorkerSupportsCancellation = false;
            virtualContext.Worker.DoWork += new DoWorkEventHandler( BuildTables );
            virtualContext.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            virtualContext.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            virtualContext.Worker.RunWorkerAsync( virtualContext );

            ResetToolBar( );
        }





        private delegate void StartFileSaveDelegate( ContextRfidReader readerContext );
        private void StartFileSave( ContextRfidReader readerContext )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( readerContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new ArgumentNullException( "readerContext" ) );
                return;
            }

            if ( this.InvokeRequired )
            {
                Invoke( new StartFileSaveDelegate( StartFileSave ) );
                return;
            }


            if ( readerContext.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }



            ProgressReset( );

            UpdateStatusBar( StatusBarState.StateName.Working, "Saving data file..." );

            readerContext.Worker = new BackgroundWorker( );
            readerContext.Worker.WorkerReportsProgress = true;
            readerContext.Worker.WorkerSupportsCancellation = false;
            readerContext.Worker.DoWork += new DoWorkEventHandler( SaveFile );
            readerContext.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            readerContext.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            readerContext.Worker.RunWorkerAsync( readerContext );

            ResetToolBar( );
        }



        private delegate void StartLoadFileDelegate( ContextFile fileContext );
        private void StartLoadFile( ContextFile fileContext )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( fileContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new ArgumentNullException( "fileContext" ) );
                return;
            }

            if ( this.InvokeRequired )
            {
                Invoke( new StartLoadFileDelegate( StartLoadFile ), fileContext );
                return;
            }


            if ( fileContext.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }



            ProgressReset( );

            UpdateStatusBar( StatusBarState.StateName.Working, "Loading indexes..." );

            fileContext.Worker = new BackgroundWorker( );
            fileContext.Worker.WorkerReportsProgress = true;
            fileContext.Worker.WorkerSupportsCancellation = false;
            fileContext.Worker.DoWork += new DoWorkEventHandler( LoadFile );
            fileContext.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            fileContext.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            fileContext.Worker.RunWorkerAsync( fileContext );

            ResetToolBar( );
        }




        public void StartInventoryOnce( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( this.InvokeRequired )
            {
                Invoke( new MethodInvoker( StartInventoryOnce ) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "StartInventoryOnce() called without a current context." );
                return;
            }

            //			ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            IReader reader = null;
            ContextBag context = ContextCollection.CurrentContext;

            if ( context.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }


            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    reader = ( ( ContextRfidReader ) context ).Reader;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ( ( ContextVirtualReader ) context ).Reader.ClearActiveReaders( );
                    foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                    {
                        //						temp.Reader.VirtualReaderQueue = ContextVirtualReader.PacketQueue;
                        temp.Reader.VirtualReaderQueue = ( ( ContextVirtualReader ) context ).Reader.PacketQueue;
                        ContextCollection.CurrentContext = temp;
                        StartInventoryOnce( );
                        ( ( ContextVirtualReader ) context ).Reader.AddActiveReader( );
                    }
                    ContextCollection.CurrentContext = context;
                    reader = ( ( ContextVirtualReader ) context ).Reader;

                    break;

                case ContextBag.ContextType.File:
                default:
                    System.Diagnostics.Debug.Assert( false );
                    Logger.Warning( String.Format( "StartInventoryOnce() called with wrong context ({0})", Enum.GetName( ContextCollection.CurrentContext.Type.GetType( ), ContextCollection.CurrentContext.Type ) ) );
                    return;
            }

            // Switch to the default view if current view is not a basic view
            if ( GUIViewState.ViewCollection[ GUIViewState.View ].ViewType != GUIViewState.GUIViewType.Basic )
            {
                SetCurrentView( GUIViewState.Default );
            }


            ProgressReset( );

            // Clear the recent packets (must be done before reset of control)
            reader.RecentPacketList.Clear( );
            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl != null )
            {
                ctrl.StatusChange( reader.Name, ProtocolControl.ControlEvent.CaptureMode, reader.RecentPacketList );
            }


            UpdateStatusBar( StatusBarState.StateName.Working, "Starting..." );

            context.Worker = new BackgroundWorker( );
            context.Worker.WorkerReportsProgress = true;
            context.Worker.WorkerSupportsCancellation = false;
            context.Worker.DoWork += new DoWorkEventHandler( InventoryOnce );
            context.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            context.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            context.Worker.RunWorkerAsync( context );

            ResetToolBar( );
        }


       public void StartMonitorPulse( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( this.InvokeRequired )
            {
                Invoke( new MethodInvoker(StartMonitorPulse) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "StartMonitorStartMonitorPulse() called without a current context." );
                return;
            }


            //ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            IReader reader = null;
            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( String.Format("StartMonitorPulse() called with wrong context ({0})", Enum.GetName(ContextCollection.CurrentContext.Type.GetType(), ContextCollection.CurrentContext.Type)) );
                return;
            }

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    reader = ( ( ContextRfidReader ) context ).Reader;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ( ( ContextVirtualReader ) context ).Reader.ClearActiveReaders( );
                    foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                    {
                        temp.Reader.VirtualReaderQueue = ( ( ContextVirtualReader ) context ).Reader.PacketQueue;
                        ContextCollection.CurrentContext = temp;
                        StartMonitorPulse();
                        ( ( ContextVirtualReader ) context ).Reader.AddActiveReader( );
                    }
                    ContextCollection.CurrentContext = context;

                    reader = ( ( ContextVirtualReader ) context ).Reader;
                    break;

                default:
                    System.Diagnostics.Debug.Assert( false );
                    Logger.Warning( String.Format("StartMonitorPulse() called with wrong context ({0})", Enum.GetName(ContextCollection.CurrentContext.Type.GetType(), ContextCollection.CurrentContext.Type)) );
                    return;
            }

            if ( reader == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( String.Format("StartMonitorPulse() could not get reader ({0})", Enum.GetName(ContextCollection.CurrentContext.Type.GetType(), ContextCollection.CurrentContext.Type)) );
                return;
            }

            if ( reader.IsClosed )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "Reader is closed." );
                return;
            }

            if ( context.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }

            // Switch to the default view if current view is not a basic view
            if ( GUIViewState.ViewCollection[ GUIViewState.View ].ViewType != GUIViewState.GUIViewType.Basic )
            {
                SetCurrentView( GUIViewState.Default );
            }

            ProgressReset( );


            // Clear must be done before reset of control
            reader.RecentPacketList.Clear( );


            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl != null )
            {
                //ctrl.StatusChange(readerContext.Reader.Name, ProtocolControl.ControlEvent.CaptureMode, readerContext.Reader.RecentPacketList);
                ctrl.StatusChange( reader.Name, ProtocolControl.ControlEvent.CaptureMode, reader.RecentPacketList );
            }
            
            UpdateStatusBar( StatusBarState.StateName.Working, "Starting..." );
            context.Worker = new BackgroundWorker( );
            context.Worker.WorkerReportsProgress = true;
            context.Worker.WorkerSupportsCancellation = false;
            context.Worker.DoWork += new DoWorkEventHandler( MonitorPulse );
            context.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            context.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            context.Worker.RunWorkerAsync( context );

            ResetToolBar( );
        }


        public void StartMonitorInventory( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( this.InvokeRequired )
            {
                Invoke( new MethodInvoker( StartMonitorInventory ) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "StartMonitorInventory() called without a current context." );
                return;
            }


            //ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            IReader reader = null;
            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( String.Format( "StartMonitorInventory() called with wrong context ({0})", Enum.GetName( ContextCollection.CurrentContext.Type.GetType( ), ContextCollection.CurrentContext.Type ) ) );
                return;
            }

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    reader = ( ( ContextRfidReader ) context ).Reader;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ( ( ContextVirtualReader ) context ).Reader.ClearActiveReaders( );
                    foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                    {
                        temp.Reader.VirtualReaderQueue = ( ( ContextVirtualReader ) context ).Reader.PacketQueue;
                        ContextCollection.CurrentContext = temp;
                        StartMonitorInventory( );
                        ( ( ContextVirtualReader ) context ).Reader.AddActiveReader( );
                    }
                    ContextCollection.CurrentContext = context;

                    reader = ( ( ContextVirtualReader ) context ).Reader;
                    break;

                default:
                    System.Diagnostics.Debug.Assert( false );
                    Logger.Warning( String.Format( "StartMonitorInventory() called with wrong context ({0})", Enum.GetName( ContextCollection.CurrentContext.Type.GetType( ), ContextCollection.CurrentContext.Type ) ) );
                    return;
            }

            if ( reader == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( String.Format( "StartMonitorInventory() could not get reader ({0})", Enum.GetName( ContextCollection.CurrentContext.Type.GetType( ), ContextCollection.CurrentContext.Type ) ) );
                return;
            }

            if ( reader.IsClosed )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "Reader is closed." );
                return;
            }

            if ( context.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }

            // Switch to the default view if current view is not a basic view
            if ( GUIViewState.ViewCollection[ GUIViewState.View ].ViewType != GUIViewState.GUIViewType.Basic )
            {
                SetCurrentView( GUIViewState.Default );
            }

            ProgressReset( );



            // Clear must be done before reset of control
            reader.RecentPacketList.Clear( );


            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl != null )
            {
                //ctrl.StatusChange(readerContext.Reader.Name, ProtocolControl.ControlEvent.CaptureMode, readerContext.Reader.RecentPacketList);
                ctrl.StatusChange( reader.Name, ProtocolControl.ControlEvent.CaptureMode, reader.RecentPacketList );
            }


            UpdateStatusBar( StatusBarState.StateName.Working, "Starting..." );
            context.Worker = new BackgroundWorker( );
            context.Worker.WorkerReportsProgress = true;
            context.Worker.WorkerSupportsCancellation = false;
            context.Worker.DoWork += new DoWorkEventHandler( MonitorInventory );
            context.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            context.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            context.Worker.RunWorkerAsync( context );

            ResetToolBar( );
        }

        public void StartTagAccess()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(StartTagAccess));
                return;
            }

            if (!ContextCollection.HasCurrentContext)
            {
                System.Diagnostics.Debug.Assert(false);
                Logger.Warning("StartTagAccess() called without a current context.");
                return;
            }

            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                System.Diagnostics.Debug.Assert(false);
                Logger.Warning(String.Format("StartTagAccess() called with wrong context ({0})", Enum.GetName(ContextCollection.CurrentContext.Type.GetType(), ContextCollection.CurrentContext.Type)));
                return;
            }

            if (readerContext.Worker.IsBusy)
            {
                PopupMessage.DoPopup(PopupMessage.PopupType.Busy);
                return;
            }

            // Switch to the default view if current view is not a basic view
            if (GUIViewState.ViewCollection[GUIViewState.View].ViewType != GUIViewState.GUIViewType.Basic)
            {
                SetCurrentView(GUIViewState.Default);
            }

            ProgressReset();

            // Clear must be done before reset of control
            readerContext.Reader.RecentPacketList.Clear();

            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[GUIViewState.GUIView.ReaderProtocol].Controls[GUIViewState.ViewCollection[GUIViewState.GUIView.ReaderProtocol].ViewName] as ProtocolControl;
            if (ctrl != null)
            {
                ctrl.StatusChange(readerContext.Reader.Name, ProtocolControl.ControlEvent.CaptureMode, readerContext.Reader.RecentPacketList);
            }

            UpdateStatusBar(StatusBarState.StateName.Working, "Starting...");
            readerContext.Worker = new BackgroundWorker();
            readerContext.Worker.WorkerReportsProgress = true;
            readerContext.Worker.WorkerSupportsCancellation = false;
            readerContext.Worker.ProgressChanged += new ProgressChangedEventHandler(BackgroundProgress);
            readerContext.Worker.DoWork += new DoWorkEventHandler(TagAccess);
            readerContext.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundTaskCompleted);
            readerContext.Worker.RunWorkerAsync(readerContext);
            ResetToolBar();
        }

        private void StartDataExport( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( this.InvokeRequired )
            {
                Invoke( new MethodInvoker( StartDataExport ) );
                return;
            }

            if ( !ContextCollection.HasCurrentContext )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( "StartDataExport() called without a current context." );
                return;
            }

            ContextBag context = ContextCollection.CurrentContext;

            if ( null == context )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( String.Format( "StartDataExport() called without a current context." ), PopupMessage.PopupType.UnableToComplete );
                return;
            }


            if ( context.Worker.IsBusy )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.Busy );
                return;
            }


            //LakeChabotReader reader = ContextCollection.CurrentContext.Reader;

            //if (reader == null)
            //{
            //    Logger.Warning(String.Format("StartDataExport() called with null reader."), PopupMessage.PopupType.UnableToComplete);
            //    return;
            //}




            // Switch to the default view if current view is not a basic view
            //if (GUIViewState.ViewCollection[GUIViewState.View].ViewType != GUIViewState.GUIViewType.Basic)
            //{
            //    SetCurrentView(GUIViewState.Default);
            //}

            ProgressReset( );

            UpdateStatusBar( StatusBarState.StateName.Working, "Collecting tag inventory..." );

            context.Worker = new BackgroundWorker( );
            context.Worker.WorkerReportsProgress = true;
            context.Worker.WorkerSupportsCancellation = false;
            context.Worker.DoWork += new DoWorkEventHandler( ExportData );
            context.Worker.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            context.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            context.Worker.RunWorkerAsync( context );

            ResetToolBar( );
        }


        /***********
         * 
         * G U I   C O N T R O L
         * 
         * *********/



        private void ActivateContext( ContextID ID )
        {
            if ( ContextCollection.ContextList.Contains( ID ) )
                ActivateContext( ContextCollection.ContextList[ ID ] );

        }

        private void ActivateContext( ContextBag nextContext )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( nextContext == ContextCollection.CurrentContext ) return;

            SaveStatusBarState( ContextCollection.CurrentContext );

            foreach ( ToolStripMenuItem i in DeviceToolStripMenuItem.DropDownItems )
                i.Checked = false;


            switch ( nextContext.Type )
            {

                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = nextContext as ContextRfidReader;
                    System.Diagnostics.Debug.Assert( readerContext != null );
                    if ( readerContext == null )
                        break;

                    readerContext.MenuItem.Checked = true;
                    this.Text = string.Format("MTI RFID Explorer");

                    ProgressShow( readerContext.Worker.IsBusy );

                    break;

                case ContextBag.ContextType.File:

                    nextContext.MenuItem.Checked = true;
                    this.Text = string.Format("{0} - MTI RFID Explorer", nextContext.MenuItem.Text);
                    ProgressShow( false );
                    break;

                case ContextBag.ContextType.VirtualReader:
                    nextContext.MenuItem.Checked = true;
                    this.Text = string.Format("{0} - MTI RFID Explorer", nextContext.MenuItem.Text);
                    ProgressShow( false );
                    break;


                default:
                    throw new InvalidEnumArgumentException( "nextContext", ( int ) nextContext.Type, typeof( ContextBag.ContextType ) );
            }


            //UpdateStatusBar(StatusBarState.StateName.Ready, String.Format("{0} Activated.", item.Text));
            RestoreStatusBarState( nextContext );


            ContextCollection.CurrentContext = nextContext;

            // Update the View
            SetCurrentView( GUIViewState.View );
            if ( CurrentContextChanged != null )
                CurrentContextChanged( this, EventArgs.Empty );

            ResetToolBar( );
        }



        /// <summary>
        /// Sets the current view based on user menu selection or inital default.
        /// </summary>
        /// <remarks>Should be called at startup after View menu is created.</remarks>
        /// <param name="newView"></param>
        private void SetCurrentView( GUIViewState.GUIView newView )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            GUIViewState.View = newView;
            string viewName = GUIViewState.ViewName( newView );

            //			foreach (ToolStripItem item in viewToolStripMenuItem.DropDownItems)
            foreach ( GUIViewState.GUIViewClass guiClass in GUIViewState.ViewCollection.Values )
            {
                if ( guiClass.Menu != null )
                {
                    ToolStripMenuItem menu = guiClass.Menu as ToolStripMenuItem;
                    if ( menu != null )
                    {
                        menu.CheckState = ( guiClass.View == newView ) ? CheckState.Indeterminate : CheckState.Unchecked;
                    }
                }
            }

            foreach ( Panel p in GUIViewState.GuiPanelCollection.Values )
            {
                p.Visible = false;
            }

            Control ctrl = GUIViewState.GuiPanelCollection[ newView ].Controls[ viewName ];
            ContextBag context = ContextCollection.CurrentContext;

            switch ( newView )
            {
                case GUIViewState.GUIView.SummaryView:
                    if ( context != null )
                    {
                        switch ( context.Type )
                        {
                            case ContextBag.ContextType.RfidReader:
                                ContextRfidReader rfidReader = context as ContextRfidReader;
                                if ( rfidReader.CurrentReport != null )
                                {
                                    UpdateSummaryPanel( rfidReader, rfidReader.CurrentReport, ( SummaryControl ) ctrl );
                                }
                                else
                                {
                                    ( ( SummaryControl ) ctrl ).Clear( );
                                }
                                break;

                            case ContextBag.ContextType.File:
                                ContextFile fileReader = context as ContextFile;
                                ( ( SummaryControl ) ctrl ).Clear( );
                                break;

                            case ContextBag.ContextType.VirtualReader:
                                ContextVirtualReader virtualReader = context as ContextVirtualReader;
                                if ( virtualReader.CurrentReport != null )
                                {
                                    UpdateSummaryPanel( virtualReader, virtualReader.CurrentReport, ( SummaryControl ) ctrl );
                                }
                                else
                                {
                                    ( ( SummaryControl ) ctrl ).Clear( );
                                }
                                break;

                            default:
                                ( ( SummaryControl ) ctrl ).Clear( );
                                break;
                        }
                    }
                    break;

                case GUIViewState.GUIView.StandardView:
                case GUIViewState.GUIView.RawPackets:
                case GUIViewState.GUIView.ReaderRequests:
                case GUIViewState.GUIView.ReaderCommands:
                case GUIViewState.GUIView.ReaderAntennaCycles:
                case GUIViewState.GUIView.AntennaPacket:
                case GUIViewState.GUIView.InventoryCycle:
                case GUIViewState.GUIView.InventoryRounds:
                case GUIViewState.GUIView.InventoryParameters:
                case GUIViewState.GUIView.InventoryData:
                case GUIViewState.GUIView.BadPackets:
                case GUIViewState.GUIView.InventoryCycleDiagnostics:
                case GUIViewState.GUIView.InventoryRoundDiagnostics:
                case GUIViewState.GUIView.EPCDataDiagnostics:
                case GUIViewState.GUIView.ReadRateData:

                    GridControl grid = ctrl as GridControl;
                    if ( grid != null && context != null )
                    {
                        switch ( context.Type )
                        {
                            case ContextBag.ContextType.RfidReader:
                            case ContextBag.ContextType.File:
                                grid.DataSources = context.Reader.DataSources;
                                break;

                            case ContextBag.ContextType.VirtualReader:
                                grid.DataSources = ( ( ContextVirtualReader ) context ).Reader.DataSources;
                                break;
                        }
                        grid.UpdateNow( );
                    }
                    break;


                case GUIViewState.GUIView.ReaderProtocol:
                    ProtocolControl protocol = ctrl as ProtocolControl;
                    System.Diagnostics.Debug.Assert( protocol != null );
                    if ( protocol != null && context != null )
                    {
                        switch ( context.Type )
                        {
                            case ContextBag.ContextType.RfidReader:
                                protocol.StatusChange( context.Reader.Name, ProtocolControl.ControlEvent.ContextChange, context.Reader.RecentPacketList );
                                break;

                            case ContextBag.ContextType.VirtualReader:
                                ContextVirtualReader vr = ( ContextVirtualReader ) context;
                                protocol.StatusChange( vr.Reader.Name, ProtocolControl.ControlEvent.ContextChange, vr.Reader.RecentPacketList );
                                break;
                        }

                    }
                    break;

                case GUIViewState.GUIView.Root:
                case GUIViewState.GUIView.Break1:
                case GUIViewState.GUIView.CalculateCommand:
                case GUIViewState.GUIView.Break2:
                case GUIViewState.GUIView.SummaryViewCollection:
                case GUIViewState.GUIView.DiagnosticsViewCollection:
                case GUIViewState.GUIView.PerformanceViewCollection:
                    break;

                default:
                    throw new InvalidEnumArgumentException( String.Format( "newView={0:f}", newView ), ( int ) newView, newView.GetType( ) );
            }


            GUIViewState.GuiPanelCollection[ newView ].Visible = true;
        }


        private void UpdatePanelProgressLC( object Sender, EventArgs e )
        {
            if ( InvokeRequired )
            {
                Invoke( new EventHandler( UpdatePanelProgressLC ) );
                return;
            }
            ProgressChangedEventArgs changedArgs = e as ProgressChangedEventArgs;
            if ( changedArgs == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            rfidProgressReport progressReport = changedArgs.UserState as rfidProgressReport;
            if ( progressReport == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextRfidReader theContext = progressReport.Context as ContextRfidReader;
            if ( theContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            if ( theContext.ID != ContextCollection.CurrentContext.ID )
                return;

            Control panel = GUIViewState.GetViewControl( );
            SummaryControl summary = panel as SummaryControl;
            if ( summary != null )
            {
                UpdateSummaryPanel( theContext, ( rfidReportBase ) progressReport, summary );
                return;
            }

            GridControl grid = panel as GridControl;
            if ( grid != null )
            {
                theContext.Reader.UpdateInventoryStats( );
                grid.UpdateNow( );
                return;
            }

            ProtocolControl protocol = panel as ProtocolControl;
            if ( protocol != null )
            {
                protocol.StatusChange( theContext.Reader.Name, ProtocolControl.ControlEvent.DataUpdate, theContext.Reader.RecentPacketList );
            }
        }


        private void UpdatePanelProgressVR( object Sender, EventArgs e )
        {
            if ( InvokeRequired )
            {
                Invoke( new EventHandler( UpdatePanelProgressVR ) );
                return;
            }
            ProgressChangedEventArgs changedArgs = e as ProgressChangedEventArgs;
            if ( changedArgs == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            rfidProgressReport progressReport = changedArgs.UserState as rfidProgressReport;
            if ( progressReport == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextVirtualReader theContext = progressReport.Context as ContextVirtualReader;
            if ( theContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            if ( theContext.ID != ContextCollection.CurrentContext.ID )
                return;

            Control panel = GUIViewState.GetViewControl( );
            SummaryControl summary = panel as SummaryControl;
            if ( summary != null )
            {
                UpdateSummaryPanel( theContext, ( rfidReportBase ) progressReport, summary );
                return;
            }

            GridControl grid = panel as GridControl;
            if ( grid != null )
            {
                theContext.Reader.UpdateInventoryStats( );
                grid.UpdateNow( );
                return;
            }

            ProtocolControl protocol = panel as ProtocolControl;
            if ( protocol != null )
            {
                protocol.StatusChange( theContext.Reader.Name, ProtocolControl.ControlEvent.DataUpdate, theContext.Reader.RecentPacketList );
            }
        }




        private void UpdatePanelCompletedLC( object Sender, EventArgs e )
        {

            if ( InvokeRequired )
            {
                Invoke( new EventHandler( UpdatePanelCompletedLC ) );
                return;
            }

            RunWorkerCompletedEventArgs completedArgs = e as RunWorkerCompletedEventArgs;
            if ( completedArgs == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            rfidOperationReport opsReport = completedArgs.Result as rfidOperationReport;
            if ( opsReport == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextRfidReader theContext = opsReport.Context as ContextRfidReader;
            if ( theContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }


            if ( theContext.ID != ContextCollection.CurrentContext.ID )
                return;

            Control panel = GUIViewState.GetViewControl( );
            SummaryControl summary = panel as SummaryControl;
            if ( summary != null )
            {
                UpdateSummaryPanel( theContext, ( rfidReportBase ) opsReport, summary );
                return;
            }

            GridControl grid = panel as GridControl;
            if ( grid != null )
            {
                theContext.Reader.UpdateInventoryStats( );
                grid.UpdateNow( );
                return;
            }

            ProtocolControl protocol = panel as ProtocolControl;
            if ( protocol != null )
            {
                protocol.StatusChange( theContext.Reader.Name, ProtocolControl.ControlEvent.DataUpdate, theContext.Reader.RecentPacketList );
            }
        }


        private void UpdatePanelCompletedVR( object Sender, EventArgs e )
        {

            if ( InvokeRequired )
            {
                Invoke( new EventHandler( UpdatePanelCompletedVR ) );
                return;
            }

            RunWorkerCompletedEventArgs completedArgs = e as RunWorkerCompletedEventArgs;
            if ( completedArgs == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            rfidOperationReport opsReport = completedArgs.Result as rfidOperationReport;
            if ( opsReport == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }

            ContextVirtualReader theContext = opsReport.Context as ContextVirtualReader;
            if ( theContext == null )
            {
                System.Diagnostics.Debug.Assert( false );
                return;
            }


            if ( theContext.ID != ContextCollection.CurrentContext.ID )
                return;

            Control panel = GUIViewState.GetViewControl( );
            SummaryControl summary = panel as SummaryControl;
            if ( summary != null )
            {
                UpdateSummaryPanel( theContext, ( rfidReportBase ) opsReport, summary );
                return;
            }

            GridControl grid = panel as GridControl;
            if ( grid != null )
            {
                theContext.Reader.UpdateInventoryStats( );
                grid.UpdateNow( );
                return;
            }

            ProtocolControl protocol = panel as ProtocolControl;
            if ( protocol != null )
            {
                protocol.StatusChange( theContext.Reader.Name, ProtocolControl.ControlEvent.DataUpdate, theContext.Reader.RecentPacketList );
            }
        }




        private static void UpdateSummaryProperties( ContextBag context )
        {
            if ( null == context )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new ArgumentNullException( "context", "Null context passed to UpdateSummaryProperties" ) );
                return;
            }

            IReader reader = null;
            rfidReportBase currentReport = null;
            double singulationRateAverage = 0;

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    reader = ( ( ContextRfidReader ) context ).Reader;
                    currentReport = ( ( ContextRfidReader ) context ).CurrentReport;
                    singulationRateAverage = ( ( ContextRfidReader ) context ).SingulationRateAverage;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    reader = ( ( ContextVirtualReader ) context ).Reader;
                    currentReport = ( ( ContextVirtualReader ) context ).CurrentReport;
                    singulationRateAverage = ( ( ContextVirtualReader ) context ).SingulationRateAverage;
                    break;

                default:
                    System.Diagnostics.Debug.Assert( false );
                    break;
            }

            if ( reader == null )
            {
                System.Diagnostics.Debug.Assert( false );
                Logger.Warning( new NullReferenceException( "Null reader passed to UpdateSummaryProperties" ) );
                return;
            }

            if ( currentReport == null )
            {
                return;
            }


            reader.SetProperty( "SessionDuration", currentReport.SessionDuration );
            reader.SetProperty( "TotalDuration", currentReport.TotalDuration );
            reader.SetProperty( "Duration", currentReport.Duration );
            reader.SetProperty( "SessionPacketCount", currentReport.SessionPacketCount );
            reader.SetProperty( "TotalPacketCount", currentReport.TotalPacketCount );
            reader.SetProperty( "PacketCount", currentReport.PacketCount );
            reader.SetProperty( "SessionAntennaCycleCount", currentReport.SessionAntennaCycleCount );
            reader.SetProperty( "TotalAntennaCycleCount", currentReport.TotalAntennaCycleCount );
            reader.SetProperty( "AntennaCycleCount", currentReport.AntennaCycleCount );
            reader.SetProperty( "SessionInventoryCycles", currentReport.SessionInventoryCycleCount );
            reader.SetProperty( "TotalInventoryCycles", currentReport.TotalInventoryCycleCount );
            reader.SetProperty( "InventoryCycles", currentReport.InventoryCycleCount );
            reader.SetProperty( "SessionRoundCount", currentReport.SessionRoundCount );
            reader.SetProperty( "TotalRoundCount", currentReport.TotalRoundCount );
            reader.SetProperty( "RoundCount", currentReport.RoundCount );
            reader.SetProperty( "SessionTagCount", currentReport.SessionTagCount );
            reader.SetProperty( "TotalTagCount", currentReport.TotalTagCount );
            reader.SetProperty( "TagCount", currentReport.TagCount );
            reader.SetProperty( "SessionUniqueTags", currentReport.SessionUniqueTags );
            reader.SetProperty( "RequestUniqueTags", currentReport.RequestUniqueTags );
            reader.SetProperty( "CurrentUniqueTags", currentReport.CurrentUniqueTags );
            reader.SetProperty( "SingulationRateAverage", singulationRateAverage );
            reader.SetProperty( "TotalRate", currentReport.TotalRate );
            reader.SetProperty( "Rate", currentReport.Rate );
            reader.SetProperty( "SessionBadPacketCount", currentReport.SessionBadPacketCount );
            reader.SetProperty( "TotalBadPacketCount", currentReport.TotalBadPacketCount );
            reader.SetProperty( "BadPacketCount", currentReport.BadPacketCount );
            reader.SetProperty( "SessionCrcErrorCount", currentReport.SessionCrcErrorCount );
            reader.SetProperty( "TotalCrcErrorCount", currentReport.TotalCrcErrorCount );
            reader.SetProperty( "CrcErrorCount", currentReport.CrcErrorCount );

        }

        //??private static Mutex UpdateSummaryPanelMutex = new Mutex( );

        private static void UpdateSummaryPanel( ContextBag theContext, rfidReportBase report, SummaryControl summary )
        {
            //??UpdateSummaryPanelMutex.WaitOne( );

            summary.SuspendLayout( );

            summary.Session_Duration = report.SessionDuration;
            summary.Command_Duration = report.TotalDuration;
            summary.Current_Duration = report.Duration;

            summary.Session_Packets = report.SessionPacketCount;
            summary.Command_Packets = report.TotalPacketCount;
            summary.Current_Packets = report.PacketCount;

            summary.Session_AntennaCycleCount = report.SessionAntennaCycleCount;
            summary.Command_AntennaCycleCount = report.TotalAntennaCycleCount;
            summary.Current_AntennaCycleCount = report.AntennaCycleCount;

            summary.Session_InventoryCycleCount = report.SessionInventoryCycleCount;
            summary.Command_InventoryCycleCount = report.TotalInventoryCycleCount;
            summary.Current_InventoryCycleCount = report.InventoryCycleCount;

            summary.Session_RoundCount = report.SessionRoundCount;
            summary.Command_RoundCount = report.TotalRoundCount;
            summary.Current_RoundCount = report.RoundCount;

            summary.Session_TotalTagReads = report.SessionTagCount;
            summary.Command_TotalTagReads = report.TotalTagCount;
            summary.Current_TotalTagReads = report.TagCount;

            summary.Session_UniqueTagReads = report.SessionUniqueTags;
            summary.Command_UniqueTagReads = report.RequestUniqueTags;
            summary.Current_UniqueTagReads = report.CurrentUniqueTags;

            summary.VR_CommandTotalReadValue = "";
            summary.VR_CurrentTotalReadValue = "";
            summary.VR_SessionTotalReadValue = "";

            summary.VR_Session_UniqueTagReads = "";
            summary.VR_Command_UniqueTagReads = "";
            summary.VR_Current_UniqueTagReads = "";

            summary.VR_Session_ReadsPerSecond = "";
            summary.VR_Command_ReadsPerSecond = "";
            summary.VR_Current_ReadsPerSecond = "";

            switch ( theContext.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = theContext as ContextRfidReader;
                    if ( readerContext.SingulationRateCount > ExponentialMovingAverage.n )
                        summary.Session_ReadsPerSecond = Convert.ToSingle( readerContext.SingulationRateAverage );
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ContextVirtualReader virtualContext = theContext as ContextVirtualReader;
                    if ( virtualContext.SingulationRateCount > ExponentialMovingAverage.n )
                        summary.Session_ReadsPerSecond = Convert.ToSingle( virtualContext.SingulationRateAverage );


                    StringBuilder sbCommandTotalReadValue = new StringBuilder( );
                    StringBuilder sbCurrentTotalReadValue = new StringBuilder( );
                    StringBuilder sbSessionTotalReadValue = new StringBuilder( );

                    StringBuilder sbSession_UniqueTagReads = new StringBuilder( );
                    StringBuilder sbCommand_UniqueTagReads = new StringBuilder( );
                    StringBuilder sbCurrent_UniqueTagReads = new StringBuilder( );

                    StringBuilder sbSession_ReadsPerSecond = new StringBuilder( );
                    StringBuilder sbCommand_ReadsPerSecond = new StringBuilder( );
                    StringBuilder sbCurrent_ReadsPerSecond = new StringBuilder( );

                    foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                    {

                        if ( temp.CurrentReport != null )
                        {
                            sbCommandTotalReadValue.AppendFormat( "{0}  ", temp.CurrentReport.TotalTagCount );
                            sbCurrentTotalReadValue.AppendFormat( "{0}  ", temp.CurrentReport.TagCount );
                            sbSessionTotalReadValue.AppendFormat( "{0}  ", temp.CurrentReport.SessionTagCount );

                            sbSession_UniqueTagReads.AppendFormat( "{0}  ", temp.CurrentReport.SessionUniqueTags );
                            sbCommand_UniqueTagReads.AppendFormat( "{0}  ", temp.CurrentReport.RequestUniqueTags );
                            sbCurrent_UniqueTagReads.AppendFormat( "{0}  ", temp.CurrentReport.CurrentUniqueTags );

                            sbCommand_ReadsPerSecond.AppendFormat( "{0:n1}  ", temp.CurrentReport.TotalRate );
                            sbCurrent_ReadsPerSecond.AppendFormat( "{0:n1}  ", temp.CurrentReport.Rate );
                        }
                    }

                    summary.VR_CommandTotalReadValue = sbCommandTotalReadValue.ToString( );
                    summary.VR_CurrentTotalReadValue = sbCurrentTotalReadValue.ToString( );
                    summary.VR_SessionTotalReadValue = sbSessionTotalReadValue.ToString( );

                    summary.VR_Session_UniqueTagReads = sbSession_UniqueTagReads.ToString( );
                    summary.VR_Command_UniqueTagReads = sbCommand_UniqueTagReads.ToString( );
                    summary.VR_Current_UniqueTagReads = sbCurrent_UniqueTagReads.ToString( );

                    summary.VR_Session_ReadsPerSecond = sbSession_ReadsPerSecond.ToString( );
                    summary.VR_Command_ReadsPerSecond = sbCommand_ReadsPerSecond.ToString( );
                    summary.VR_Current_ReadsPerSecond = sbCurrent_ReadsPerSecond.ToString( );

                    break;

            }

            summary.Command_ReadsPerSecond = report.TotalRate;
            summary.Current_ReadsPerSecond = report.Rate;

            summary.Session_BadPackets = report.SessionBadPacketCount;
            summary.Command_BadPackets = report.TotalBadPacketCount;
            summary.Current_BadPackets = report.BadPacketCount;

            summary.Session_CRCErrors = report.SessionCrcErrorCount;
            summary.Command_CRCErrors = report.TotalCrcErrorCount;
            summary.Current_CRCErrors = report.CrcErrorCount;

            summary.ResumeLayout( );

            //??UpdateSummaryPanelMutex.ReleaseMutex();

            summary.Refresh( );
        }


        private delegate void ProgressShowDelegate( bool Show );
        private void ProgressShow( bool Show )
        {
            if ( InvokeRequired )
            {
                Invoke( new ProgressShowDelegate( ProgressShow ), Show );
                return;
            }
            this.toolStripProgressBarMain.Visible = Show;
        }



        private delegate void ProgressDelegate( int HowMuch );
        private void Progress( int HowMuch )
        {
            if ( InvokeRequired )
            {
                ProgressDelegate dlg = new ProgressDelegate( Progress );
                Invoke( dlg, HowMuch );
                return;
            }

            if ( HowMuch > toolStripProgressBarMain.Maximum )
                HowMuch = toolStripProgressBarMain.Maximum;
            if ( HowMuch < toolStripProgressBarMain.Minimum )
                HowMuch = toolStripProgressBarMain.Minimum;
            this.toolStripProgressBarMain.Value = HowMuch;
        }

        private void AddProgress( int HowMuch )
        {
            if ( InvokeRequired )
            {
                ProgressDelegate dlg = new ProgressDelegate( AddProgress );
                Invoke( dlg, HowMuch );
                return;
            }
            int newValue = this.toolStripProgressBarMain.Value + HowMuch;
            if ( newValue > toolStripProgressBarMain.Maximum ) newValue = toolStripProgressBarMain.Minimum;
            if ( newValue < toolStripProgressBarMain.Minimum ) newValue = toolStripProgressBarMain.Minimum;
            this.toolStripProgressBarMain.Value = newValue;
        }


        private void ProgressTaskFinished( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( ProgressTaskFinished ) );
                return;
            }
            this.toolStripProgressBarMain.Value = 100;
            Thread.Sleep( 750 );
            this.toolStripProgressBarMain.Value = 0;
            this.toolStripProgressBarMain.Visible = false;
            this.Cursor = Cursors.Default;
        }


        private void ProgressReset( )
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( ProgressReset ) );
                return;
            }
            try
            {
                this.toolStripProgressBarMain.Value = 0;
                this.toolStripProgressBarMain.Visible = true;
                this.toolStripStatusMessage.Text = "";
                this.Cursor = Cursors.AppStarting;
            }
            catch ( Exception )
            {

            }
        }


        private void RefreshStatusBar( )
        {
            if ( Enum.IsDefined( typeof( StatusBarState.StateName ), toolStripStatusState.Tag ) )
            {
                StatusBarState.StateName state = ( StatusBarState.StateName ) toolStripStatusState.Tag;

                if ( StatusBarState.GetStatusBarStateType( state ) == StatusBarState.StateType.Transient )
                {
                    UpdateStatusBar( StatusBarState.StateName.Blank );
                }
                statusStrip1.Refresh( );
            }
        }

        private void UpdateStatusBar( StatusBarState.StateName newState )
        {
            UpdateStatusBar( newState, null );
        }

        private delegate void UpdateStatusBarDelegate( StatusBarState.StateName newState, string Message );
        private void UpdateStatusBar( StatusBarState.StateName newState, String Message )
        {
            if ( InvokeRequired )
            {
                UpdateStatusBarDelegate dlg = new UpdateStatusBarDelegate( UpdateStatusBar );
                Invoke( dlg, newState, Message );
                return;
            }

            if ( ContextCollection.HasCurrentContext )
            {
                toolStripStatusState.Text = StatusBarState.GetStatusBarStateText( newState );
                toolStripStatusState.BackColor = StatusBarState.GetStatusBarStateColor( newState );
                toolStripStatusState.Tag = newState;
            }
            else
            {
                toolStripStatusState.Text = StatusBarState.GetStatusBarStateText( newState );
                toolStripStatusState.BackColor = StatusBarState.GetStatusBarStateColor( newState );
                toolStripStatusState.Tag = newState;
            }

            toolStripStatusMessage.Text = Message == null ? "" : Message;
			//Add by Chingsheng for display extention data, 2015-01-14
            if (newState == StatusBarState.StateName.Ready)
                UpdateStatusTemperature(String.Format("N/A"));
			//End by Chingsheng for display extention data, 2015-01-14

            statusStrip1.Refresh( );
        }

		//Add by Chingsheng for display extention data, 2015-01-14
        private void UpdateStatusTemperature(String Message)
        {
            toolStripStatusTemperature.Text = String.Format("Temp: {0}",Message);
        }
		//End by Chingsheng for display extention data, 2015-01-14


        private void SaveStatusBarState( ContextBag context )
        {
            if ( null == context )
                return;

            StatusBarState.StateName tagState = ( StatusBarState.StateName ) toolStripStatusState.Tag;
            if ( tagState == StatusBarState.StateName.Paused )
            {
                context.IsPaused = true;
            }
            else
            {
                context.StatusState = tagState;
                context.StatusMessage = toolStripStatusMessage.Text;
                context.IsPaused = false;
            }
        }


        private void RestoreStatusBarState( ContextBag context )
        {
            if ( context.IsPaused )
            {
                toolStripStatusState.Text = StatusBarState.GetStatusBarStateText( StatusBarState.StateName.Paused );
                toolStripStatusState.BackColor = StatusBarState.GetStatusBarStateColor( StatusBarState.StateName.Paused );
                toolStripStatusState.Tag = StatusBarState.StateName.Paused;
                toolStripStatusMessage.Text = StatusBarState.PausedMessage;
            }
            else
            {
                toolStripStatusState.Text = StatusBarState.GetStatusBarStateText( context.StatusState );
                toolStripStatusState.BackColor = StatusBarState.GetStatusBarStateColor( context.StatusState );
                toolStripStatusState.Tag = context.StatusState;
                toolStripStatusMessage.Text = context.StatusMessage;
            }

            statusStrip1.Refresh( );
        }


        private void ResetToolBar( )
        {
            if ( ContextCollection.HasCurrentContext )
            {
                bool busy;
                switch ( ContextCollection.CurrentContext.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
                        busy = reader.Worker.IsBusy;
                        break;

                    case ContextBag.ContextType.File:
                        break;

                    case ContextBag.ContextType.VirtualReader:
                        ContextVirtualReader vReader = ContextCollection.ContextAsVirtualReader;
                        busy = vReader.Worker.IsBusy;
                        break;

                    default:
                        System.Diagnostics.Debug.Assert( false, "Unknown context." );
                        break;
                }
            }
        }


        private void ResetPauseBtn()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ResetPauseBtn));
                return;
            }
  
           ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                //System.Diagnostics.Debug.Assert(false, "readerContext is null");
                //Logger.Warning("Unknown reader context in Button3_Click_AccessTag");
                return;
            }

            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;
           
            readerContext.IsPaused = false;
            toolStripButton5.Text = "Pause";
        }

        /***********
         * 
         * R E A D E R   C O N T R O L
         * 
         * *********/


        private void OpenConfigFile( string filename )
        {
            if ( String.IsNullOrEmpty( filename ) )
            {
                throw new ArgumentNullException( "fileName" );
            }

            if ( !File.Exists( filename ) )
            {
                throw new ArgumentException( String.Format( "File {0} does not exist.", filename ), "filename" );
            }

            XmlReaderSettings settings = new XmlReaderSettings( );
            settings.CheckCharacters = true;
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;


            string type;
            string name;
            string description;
            string location;
            string locationType;

            rfidReaderID.ReaderType readerType;
            rfidReaderID.LocationTypeID locationTypeID;

            XmlReader xmlReader = null;

            try
            {
                using ( xmlReader = XmlReader.Create( new FileStream( filename, FileMode.Open ), settings ) )
                {
                    xmlReader.MoveToContent( );


                    while ( !xmlReader.IsStartElement( ) )
                    {
                        xmlReader.Read( );
                    }

                    if ( xmlReader.Name != "rfid-config" )
                    {
                        throw new Exception( "The root element is missing." );
                    }

                    do
                    {
                        xmlReader.Read( );
                    } while ( !xmlReader.EOF && ( !xmlReader.IsStartElement( ) || xmlReader.Name != "type" ) );

                    if ( xmlReader.EOF )
                    {
                        throw new Exception( "Type element is missing." );
                    }

                    type = xmlReader.ReadElementString( "type" );
                    name = xmlReader.ReadElementString( "name" );
                    description = xmlReader.ReadElementString( "description" );
                    location = xmlReader.ReadElementString( "location" );
                    locationType = xmlReader.ReadElementString( "locationType" );


                    if ( String.IsNullOrEmpty( type ) ) throw new Exception( "type is null or empty." );
                    if ( String.IsNullOrEmpty( name ) ) throw new Exception( "name is null or empty." );
                    if ( String.IsNullOrEmpty( description ) ) throw new Exception( "description is null or empty." );
                    if ( String.IsNullOrEmpty( location ) ) throw new Exception( " is null or empty." );
                    if ( String.IsNullOrEmpty( locationType ) ) throw new Exception( " is null or empty." );

                    if ( !Enum.IsDefined( typeof( rfidReaderID.ReaderType ), type ) ) throw new Exception( "type is invalid." );
                    if ( !Enum.IsDefined( typeof( rfidReaderID.LocationTypeID ), locationType ) ) throw new Exception( "LocationTypeID is invalid." );

                    readerType = ( rfidReaderID.ReaderType ) Enum.Parse( typeof( rfidReaderID.ReaderType ), type );
                    locationTypeID = ( rfidReaderID.LocationTypeID ) Enum.Parse( typeof( rfidReaderID.LocationTypeID ), locationType );



                    string msg = String.Format( "Reader type {0} and location type {1} is not a supported combination.", type, locationType );
                    switch ( readerType )
                    {
                        case rfidReaderID.ReaderType.MTI:
                            System.Diagnostics.Debug.WriteLine( msg );
                            throw new Exception( msg );

                        case rfidReaderID.ReaderType.Unknown:
                        default:
                            System.Diagnostics.Debug.WriteLine( msg );
                            throw new Exception( msg );
                    }

                   
                }
            }
            catch ( Exception ex )
            {
                System.Diagnostics.Debug.WriteLine( ex.Message );
                Logger.Warning( new Exception( "Invalid config file.", ex ) );
                return;
            }
            finally
            {
                try
                {
                    if ( null != xmlReader )
                    {
                        xmlReader.Close( );
                    }
                }
                catch( Exception )
                {

                }
            }

        }



        private void OpenContextFile( string filename )
        {
            try
            {
                String ShortName = System.IO.Path.GetFileName( filename );

                ContextFile context = new ContextFile( );
                context.StatusState = StatusBarState.StateName.Ready;
                context.IsPaused = false;
                context.Worker = new BackgroundWorker( );

                context.FileName = filename;
                /*
                context.Reader = new LakeChabotReader(filename);
                */
                context.MenuItem = new ToolStripMenuItem( String.Format( "File: {0}", ShortName ) );
                context.MenuItem.ToolTipText = filename;
                context.MenuItem.Tag = context.ID;
                context.MenuItem.Click += new EventHandler( deviceMenuItem_Click );
                context.StatusState = StatusBarState.StateName.Ready;
                context.StatusMessage = String.Format( "File {0} opened.", ShortName );
                //				context.FileName = null;
                DeviceToolStripMenuItem.DropDownItems.Add( context.MenuItem );
                ContextCollection.ContextList.Add( context );
                //ActivateContext(context);
                StartLoadFile( context );


            }
            catch ( Exception exp )
            {
                Logger.Warning( exp );
            }


        }


        private void CloseContextFile( ContextFile context )
        {
            if ( ContextCollection.ContextList.Count == 1 )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.NoCloseOfLastFile );
                return;
            }


            ContextCollection.ContextList.Remove( context.ID );
            DeviceToolStripMenuItem.DropDownItems.Remove( context.MenuItem );
            ActivateContext( ContextCollection.ContextList[ 0 ] );

            // Always notify the Protocol control of the file close
            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl != null )
            {
                ctrl.StatusChange( context.Reader.Name, ProtocolControl.ControlEvent.Closing, context.Reader.RecentPacketList );
            }
            context.Reader.Dispose( );
        }


        private void SaveToContextFile( ContextBag context )
        {

            if ( null == context )
            {
                Logger.Warning( new ArgumentNullException( "context", "Null context passed to SaveContextToFile" ) );
                return;
            }

            ContextRfidReader readerContext = context as ContextRfidReader;
            if ( readerContext != null && readerContext.Worker.IsBusy )
            {
                Logger.Warning( "SaveToContextFile() called with busy worker.", PopupMessage.PopupType.NoSaveWhenReaderBusy );
                return;
            }

            StartFileSave( readerContext );
            //try
            //{
            //    readerContext.Reader.SaveDataToFile(context.FileName);
            //    UpdateStatusBar(StatusBarState.StateName.NoChange, "Packet dataFile saved to file " + context.FileName);
            //}
            //catch (Exception exp)
            //{
            //    Logger.Warning(exp);
            //}

        }


        private void OpenReader( )
        {
            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if ( readerContext == null )
                return;

            String errorMessage = null;
            readerContext.Reader.BindReader( readerContext.ReaderID );
            if ( errorMessage != null )
                UpdateStatusBar( StatusBarState.StateName.Error, errorMessage );
        }




        private void CreateVirtualReader( )
        {
            ContextVirtualReader.InitializeVirtualView( );

            ContextVirtualReader context = ContextVirtualReader.VirtualReader;

            if ( context != null )
            {
                context.StatusState = StatusBarState.StateName.Ready;
                context.IsPaused = false;
                context.Worker = new BackgroundWorker( );
                context.ID = new ContextID( );

                List<LakeChabotReader> list = new List<LakeChabotReader>( 10 );

                foreach ( ContextRfidReader reader in ContextVirtualReader.ReadersInVR )
                {
                    list.Add( reader.Reader );
                }
                context.Reader = new VirtualReader( list );

                context.MenuItem = new ToolStripMenuItem( String.Format( "*** Virtual Device ****" ) );
                context.MenuItem.ToolTipText = String.Format( "Virtual view created by bridging {0} readers.", ContextVirtualReader.Count );
                context.MenuItem.Tag = context.ID;
                context.MenuItem.Click += new EventHandler( deviceMenuItem_Click );
                context.MenuItem.Visible = false;

                context.StatusState = StatusBarState.StateName.Ready;
                context.StatusMessage = String.Format( "Bridged Virtual Reader Activated." );
                context.FileName = null;
                DeviceToolStripMenuItem.DropDownItems.Add( context.MenuItem );
                ContextCollection.ContextList.Add( context );
            }
        }

        private void ShowVirtualReader( )
        {
            if ( !ContextVirtualReader.IsVirtualReaderEnabled )
            {
                Logger.Warning( "ShowVirtualReader() called with Virtual Reader NOT enabled.", PopupMessage.PopupType.UnableToComplete );
                return;
            }

            bridgeReadersToolStripMenuItem.Checked = true;
            ContextVirtualReader.Visible = true;
            ContextVirtualReader context = ContextVirtualReader.VirtualReader;

            context.MenuItem.Visible = true;
            context.MenuItem.PerformClick( );
            PopupMessage.DoPopup( PopupMessage.PopupType.VirtualReaderCreated );

        }


        private void HideVirtualReader( )
        {
            bridgeReadersToolStripMenuItem.Checked = false;
            ContextVirtualReader.Visible = false;
            ContextVirtualReader context = ContextVirtualReader.VirtualReader;
            context.MenuItem.Visible = false;
            PopupMessage.DoPopup( PopupMessage.PopupType.VirtualReaderKilled );
            if ( ContextCollection.ContextAsVirtualReader != null )
            {
                ActivateContext( ContextCollection.ContextList[ 0 ] );
            }

        }

        private void BuildTables( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ReportBase report = null;
            ContextBag context = e.Argument as ContextBag;
            if ( null == context )
                throw new ArgumentException( String.Format( "Expected a contextBag but got a {0}", e.Argument.GetType( ).Name ), "e" );


            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = e.Argument as ContextRfidReader;
                    readerContext.CurrentTask = ContextRfidReader.ReaderTask.BuildingTables;
                    report = readerContext.Reader.BuildTables( readerContext, worker, 500 );
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ContextVirtualReader virtualContext = e.Argument as ContextVirtualReader;
                    virtualContext.CurrentTask = ContextBag.ReaderTask.BuildingTables;

                    List<System.Collections.DictionaryEntry[ ]> list = new List<System.Collections.DictionaryEntry[ ]>( );
                    foreach ( ContextRfidReader reader in ContextVirtualReader.ReadersInVR )
                    {
                        list.Add( reader.Reader.DataSources );
                    }

                    report = virtualContext.Reader.BuildTables( virtualContext, worker, 500, StopDependentReaders, list );
                    break;

            }

            e.Result = report;
        }




        private void InventoryOnce( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ContextBag context = e.Argument as ContextBag;
            if ( null == context )
                throw new ArgumentException( String.Format( "Expected a contextBag but got a {0}", e.Argument.GetType( ).Name ), "e" );

            context.CurrentTask = ContextRfidReader.ReaderTask.ReadInventoryOnce;

            ReportBase report = null;

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = e.Argument as ContextRfidReader;
                    readerContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
                    report = readerContext.Reader.ReadInventory( readerContext, worker, 500 );
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ContextVirtualReader virtualContext = e.Argument as ContextVirtualReader;
                    virtualContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
                    report = virtualContext.Reader.ReadInventory( virtualContext, worker, 500, StopDependentReaders );
                    break;

                default:
                    throw new ArgumentException( String.Format( "Expected a ContextRfidReader or ContextVirtualReader, but got a {0}", e.Argument.GetType( ).Name ), "e" );
            }


            e.Result = report;
        }



        private void ExportData( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ContextBag context = e.Argument as ContextBag;
            if ( null == context )
                throw new ArgumentException( String.Format( "Expected a contextBag but got a {0}", e.Argument.GetType( ).Name ), "e" );

            context.CurrentTask = ContextRfidReader.ReaderTask.Export;

            List<GridControl.GridType> dataSources = new List<GridControl.GridType>( );

            foreach ( GUIViewState.GUIViewClass view in context.ExportViewList )
            {
                dataSources.Add( GUIViewState.MapGuiViewToGridType( view ) );
            }
            IReader reader = null;
            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    UpdateSummaryProperties( context );
                    reader = ( ( ContextRfidReader ) context ).Reader;
                    break;

                case ContextBag.ContextType.File:
                    reader = ( ( ContextFile ) context ).Reader;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    reader = ( ( ContextVirtualReader ) context ).Reader;
                    UpdateSummaryProperties( context );
                    break;

                default:
                    break;
            }

            if ( reader == null )
            {
                return;
            }

            ReportBase report = ExcelExport.ExportData( context, reader, worker, 500, context.ExportTarget.ToString( ), dataSources );


            e.Result = report;

        }



        private void SaveFile( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ContextRfidReader readerContext = e.Argument as ContextRfidReader;
            if ( readerContext == null )
                throw new ArgumentException( String.Format( "Expected a contextRfidReader but got a {0}", e.Argument.GetType( ).Name ), "e" );

            try
            {
                readerContext.Reader.SetProperty( "SaveDate", String.Format( "{0:MMMM d, yyyy}", DateTime.Now.Date ) );
                readerContext.Reader.SetProperty( "SaveTime", String.Format( "{0:T}", DateTime.Now.TimeOfDay ) );
                readerContext.Reader.SetProperty( "ReaderName", readerContext.Reader.Name );
                readerContext.Reader.SetProperty( "HardwareVersion", readerContext.Reader.HardwareVersion.ToString( ) ); // TODO: figure out old formatting (3));
                readerContext.Reader.SetProperty( "FirmwareVersion", readerContext.Reader.FirmwareVersion.ToString() ); // TODO: figure out old formatting (3));
                readerContext.Reader.SetProperty( "LibraryVersion", readerContext.Reader.LibraryVersion.ToString( ) ); // TODO: figure out old formatting (3));
                readerContext.Reader.SetProperty( "BootLoaderVersion", readerContext.Reader.BootLoaderVersion.ToString()); // TODO: figure out old formatting (3));
                
                UpdateSummaryProperties( readerContext );

                readerContext.CurrentTask = ContextRfidReader.ReaderTask.SaveFile;

                ReportBase report = readerContext.Reader.SaveDataToFile( readerContext, worker, 500, readerContext.FileName );

                e.Result = report;

            }
            catch ( Exception exp )
            {
                Logger.Warning( exp );
            }
        }




        private void LoadFile( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ContextFile fileContext = e.Argument as ContextFile;
            if ( fileContext == null )
                throw new ArgumentException( String.Format( "Expected a fileContext but got a {0}", e.Argument.GetType( ).Name ), "e" );

            fileContext.CurrentTask = ContextRfidReader.ReaderTask.LoadFile;
            ReportBase report = LakeChabotReader.LoadFileIntoStaticReader( fileContext, worker, 500, fileContext.FileName );



            e.Result = report;
        }



        private void MonitorPulse(object sender, DoWorkEventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ReportBase report = null;

            ContextBag context = e.Argument as ContextBag;
            if ( null == context )
            {
                throw new ArgumentException( String.Format( "Expected a ContextBag, but got a {0}", e.Argument.GetType( ).Name ), "e" );
            }
            
			switch (context.Type)
			{
				case ContextBag.ContextType.RfidReader:
					ContextRfidReader readerContext = e.Argument as ContextRfidReader;
					readerContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
                    //Mod by Wayne for add Tx random data feature in RFTest page, 2014-09-10
                    report = readerContext.Reader.MonitorPulse(readerContext, worker, 500, RFTest.RandomType, RFTest.ControlType);
                    //report = readerContext.Reader.MonitorPulse(readerContext, worker, 500);
                    //End by Wayne for add Tx random data feature in RFTest page, 2014-09-10
					break;

				case ContextBag.ContextType.VirtualReader:
					ContextVirtualReader virtualContext = e.Argument as ContextVirtualReader;
					virtualContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
                    report = virtualContext.Reader.MonitorPulse(virtualContext, worker, 500, StopDependentReaders);

					break;

				default:
					throw new ArgumentException(String.Format("Expected a ContextRfidReader or ContextVirtualReader, but got a {0}", e.Argument.GetType().Name), "e");
			}
            
            e.Result = report;
        }



        private void MonitorInventory( object sender, DoWorkEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            BackgroundWorker worker = sender as BackgroundWorker;
            if ( null == worker )
                throw new ArgumentException( String.Format( "Expected a BackgroundWorker but got a {0}", sender.GetType( ).Name ), "sender" );

            ReportBase report = null;

            ContextBag context = e.Argument as ContextBag;
            if ( null == context )
            {
                throw new ArgumentException( String.Format( "Expected a ContextBag, but got a {0}", e.Argument.GetType( ).Name ), "e" );
            }
            
			switch (context.Type)
			{
				case ContextBag.ContextType.RfidReader:
					ContextRfidReader readerContext = e.Argument as ContextRfidReader;
					readerContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
                    report = readerContext.Reader.MonitorInventory(readerContext, worker, 500);
					break;

				case ContextBag.ContextType.VirtualReader:
					ContextVirtualReader virtualContext = e.Argument as ContextVirtualReader;
					virtualContext.CurrentTask = ContextRfidReader.ReaderTask.MonitoringInventory;
					report = virtualContext.Reader.MonitorInventory(virtualContext, worker, 500, StopDependentReaders);
					break;

				default:
					throw new ArgumentException(String.Format("Expected a ContextRfidReader or ContextVirtualReader, but got a {0}", e.Argument.GetType().Name), "e");
			}
            
            e.Result = report;
        }


        private void TagAccess(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (null == worker)
                throw new ArgumentException(String.Format("Expected a BackgroundWorker but got a {0}", sender.GetType().Name), "sender");

            ContextRfidReader readerContext = e.Argument as ContextRfidReader;
            if (readerContext == null)
                throw new ArgumentException(String.Format("Expected a contextRfidReader but got a {0}", e.Argument.GetType().Name), "e");

             readerContext.CurrentTask = ContextRfidReader.ReaderTask.TagAccess;

            //clark 2011.2.18 Record that user request data length
            int iTagAccessReqCount = ActiveReader.TagAccessDataSet.count;
            int iTagAccessReqCountRead = ActiveReader.TagAccessReadSet.ReadWords;//??把计??
            //ReportBase report = readerContext.Reader.TagAccess(readerContext, worker, 500, iTagAccessReqCount);//把计??
            ReportBase report = readerContext.Reader.TagAccess(readerContext, worker, 500, iTagAccessReqCount, iTagAccessReqCountRead);//把计??


            e.Result = report;
        }


        //clark 2011.4.6 Find Retry Count value from toolStripDropDownButtonRetryCount
        public byte GetRetryCount()
        {
            byte index = 0;
            foreach (ToolStripMenuItem tsm in toolStripDropBtnRetryCount.DropDownItems)
            {

                if (tsm.Checked == true)
                    return index;

                index++;
            }

            return 0;
        }


        //clark 2011.05.18 Let control panel to get the Checked Value.
        public byte IsPostSingulationEnable
        {
            get
            {
                return (byte)(toolStripMenuItemPostSingulation.Checked ? 1 : 0);
            }
        }

        //clark 2011.05.18 Let control panel to get the Checked Value.
        public byte IsSelectCriteriaEnable
        {
            get
            {
                return (byte)(toolStripMenuItemSelectCriteria.Checked ? 1 : 0);
            }
        }

        /***********
         * 
         * E V E N T   H A N D L E R S  A N D   C A L L B A C K S
         * 
         * *********/
        #region Callback methods




        private string StopDependentReaders( bool abort )
        {
            string errorMessage = null;
            try
            {
                foreach ( ContextRfidReader temp in ContextVirtualReader.ReadersInVR )
                {
                    if ( temp.Worker.IsBusy )
                    {
                        if ( abort )
                        {
                            temp.Reader.FunctionController.RequestAbort( );
                        }
                        else
                        {
                            temp.Reader.FunctionController.RequestStop( );
                        }
                    }
                }
            }
            catch ( Exception exp )
            {
                errorMessage = "Error stopping readers: " + exp.Message;
            }
            return errorMessage;
        }

        #endregion

        #region mainForm Events


        private void mainForm_Load( object sender, EventArgs e )
        {
            if ( Properties.Settings.Default.maximizeOnStartUp )
                this.WindowState = FormWindowState.Maximized;


            //Clark 2011.2.10 Cpoied from R1000 Tracer. Don't support Control Panel.
            //ControlPanelForm.LaunchControlPanel( this );
        }


        private void mainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            LakeChabotReader.MANAGED_ACCESS.API_RadioClose();

            foreach ( ContextBag c in ContextCollection.ContextList )
            {
                ContextRfidReader r = c as ContextRfidReader;
                if ( r == null )
                    continue;

                if ( r.Worker.IsBusy )
                {
                    PopupMessage.DoPopup( PopupMessage.PopupType.NoCloseWhenBusy );
                    e.Cancel = true;
                    return;
                }
            }


            if ( RFID_Explorer.Properties.Settings.Default.confirmExit )
            {
                if ( PopupMessage.Popup( PopupMessage.PopupType.QueryExit ) == DialogResult.No )
                {
                    e.Cancel = true;
                    return;
                }
            }


            foreach ( GUIViewState.GUIViewClass viewClass in GUIViewState.ViewCollection.Values )
            {
                if ( viewClass.HasPanel )
                {
                    Panel p = GUIViewState.GuiPanelCollection[ viewClass.View ];
                    GridControl grid = p.Controls[ viewClass.ViewName ] as GridControl;
                    if ( grid != null )
                        grid.DataSources = null;
                }
            }

            foreach ( ContextBag c in ContextCollection.ContextList )
            {
                if ( c.Reader != null )
                    c.Reader.Dispose( );
            }


            //Close Device
            LakeChabotReader.MANAGED_ACCESS.API_RadioClose();

            ControlPanelForm.CloseControlPanel( );
            LakeChabotReader.AssemblyClosing( );
        }

        #endregion


        #region Background Events

        private void BackgroundProgress( object sender, ProgressChangedEventArgs e )
        {
            ReportBase progressReport = e.UserState as ReportBase;
            System.Diagnostics.Debug.Assert( progressReport != null );

            if ( progressReport == null )
                return;

            ContextBag theContext = progressReport.Context as ContextBag;
            System.Diagnostics.Debug.Assert( theContext != null );

            if ( theContext == null )
                return;


            if ( theContext.CurrentTask == ContextBag.ReaderTask.LoadFile )
            {
                UpdateStatusBar( StatusBarState.StateName.Working, String.Format( "Loading file {0}", theContext.FileName ) );
                if ( e.ProgressPercentage == 0 )
                {
                    AddProgress( 10 );
                }
                else
                {
                    Progress( e.ProgressPercentage );
                }
                return;
            }
            if ( theContext.CurrentTask == ContextBag.ReaderTask.SaveFile )
            {
                UpdateStatusBar( StatusBarState.StateName.Working, String.Format( "Saving file {0}", theContext.FileName ) );
                if ( e.ProgressPercentage == 0 )
                {
                    AddProgress( 10 );
                }
                else
                {
                    Progress( e.ProgressPercentage );
                }
                return;
            }

            if ( theContext.CurrentTask == ContextBag.ReaderTask.Export )
            {
                UpdateStatusBar( StatusBarState.StateName.Working, String.Format( "Exporting data..." ) );
                if ( e.ProgressPercentage == 0 )
                {
                    AddProgress( 10 );
                }
                else
                {
                    Progress( e.ProgressPercentage );
                }
                return;
            }

            if ( theContext.CurrentTask == ContextBag.ReaderTask.Connect )
            {
                if ( progressReport is rfidSimpleResultReport )
                {
                    UpdateStatusBar( StatusBarState.StateName.Working, ( ( rfidSimpleResultReport ) progressReport ).ToString( ) );
                }
                if ( e.ProgressPercentage == 0 )
                {
                    AddProgress( 10 );
                }
                else
                {
                    Progress( e.ProgressPercentage );
                }
                return;
            }


            IReader reader = null;
            switch ( theContext.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    reader = ( ( ContextRfidReader ) theContext ).Reader;
                    break;


                case ContextBag.ContextType.VirtualReader:
                    reader = ( ( ContextVirtualReader ) theContext ).Reader;
                    break;
            }


            if ( reader == null )
            {
                System.Diagnostics.Debug.Assert( false, "reader is null." );
                return;
            }
            // Always notify the Protocol control of the progress
            ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl != null )
            {
                //ctrl.StatusChange(theContext.Reader.Name, ProtocolControl.ControlEvent.DataUpdate, theContext.Reader.RecentPacketList);
                ctrl.StatusChange( reader.Name, ProtocolControl.ControlEvent.DataUpdate, reader.RecentPacketList );
            }

            // Check if the report is for the current context
            if ( theContext.ID != ContextCollection.CurrentContext.ID )
            {
                if ( ContextCollection.ContextAsVirtualReader != null && theContext.Type == ContextBag.ContextType.RfidReader )
                {
                    ( ( ContextRfidReader ) theContext ).CurrentReport = ( rfidReportBase ) progressReport;
                }
                return;
            }


            if ( reader.FunctionController.State == FunctionControl.FunctionState.Paused )
            {
                // nothing to do
            }
            else
            {
                UpdateStatusBar( StatusBarState.StateName.Working, String.Format( "{0} {1}.", theContext.TaskName, progressReport ) );
                if ( e.ProgressPercentage == 0 )
                {
                    AddProgress( 10 );
                }
                else
                {
                    Progress( e.ProgressPercentage );
                }

                switch ( theContext.CurrentTask )
                {
                    case ContextRfidReader.ReaderTask.Idle:
                        break;

                    case ContextRfidReader.ReaderTask.ReadInventoryOnce:
                    case ContextRfidReader.ReaderTask.MonitoringInventory:
                    case ContextBag.ReaderTask.TagAccess:
                        rfidProgressReport fullReport = progressReport as rfidProgressReport;
                        if ( theContext is ContextRfidReader )
                        {
                            ContextRfidReader readerContext = theContext as ContextRfidReader;
                            if ( readerContext != null )
                            {
                                readerContext.CurrentReport = fullReport;
                                readerContext.SingulationRateAverage = ExponentialMovingAverage.Calculate( readerContext.SingulationRateCount == 0 ? fullReport.Rate : readerContext.SingulationRateAverage, fullReport.Rate );
                                ++readerContext.SingulationRateCount;
                                UpdatePanelProgressLC( sender, e );
                            }
                        }
                        else if ( theContext is ContextVirtualReader )
                        {
                            ContextVirtualReader virtualContext = theContext as ContextVirtualReader;
                            if ( virtualContext != null )
                            {
                                virtualContext.CurrentReport = fullReport;
                                virtualContext.SingulationRateAverage = ExponentialMovingAverage.Calculate( virtualContext.SingulationRateCount == 0 ? fullReport.Rate : virtualContext.SingulationRateAverage, fullReport.Rate );
                                ++virtualContext.SingulationRateCount;
                                UpdatePanelProgressVR( sender, e );
                            }
                        }
                        else
                        {

                        }

                        break;

                    case ContextRfidReader.ReaderTask.BuildingTables:
                        break;

                    default:
                        break;
                }

            }


            //Have data in the queue
            if (reader.RecentPacketList.Count > 0)
            {
                //clark 2011.08.29.  Show error code during "Keep Inventory after received error"
                PacketData.PacketWrapper strcPkt = reader.RecentPacketList[reader.RecentPacketList.Count - 1];
                if (strcPkt.PacketType == PacketData.PacketType.CMD_END)
                {
                    PacketData.cmd_end strcPktEnd = (PacketData.cmd_end)strcPkt.Packet;
                    if (strcPktEnd.Result != 0)
                    {
                        UpdateStatusBar(StatusBarState.StateName.TagAccessOptError, String.Format("Error Code: 0x{0:X3}", strcPktEnd.Result));
                    }
                }
				//Add by Chingsheng for display extention data, 2015-01-14
                else if (strcPkt.PacketType == PacketData.PacketType.ISO18K6C_INVENTORY)
                {
                    
                    PacketData.inventory inventory = strcPkt.Packet as PacketData.inventory;
                    PacketData.inventory_extra inventory_extra = new PacketData.inventory_extra();
                    System.Collections.Specialized.BitVector32 flags = new System.Collections.Specialized.BitVector32(inventory.flags);
                    bool isExtra = flags[PacketData.PacketBase.extraHardware] == (int)PacketData.PacketBase.ExtraResultValues.Enable;
                    if (isExtra)
                    { // Extar Info 
                        inventory_extra.Temperature = inventory.inventory_data[2];
                        UpdateStatusTemperature(String.Format("{0:0} C", inventory_extra.Temperature));
                    }
                    else 
                    {
                        UpdateStatusTemperature(String.Format("N/A"));
                    }
                }

                rfidProgressReport fullReport = progressReport as rfidProgressReport;
                if (fullReport.SessionTagCount != _oldRecentPacketCount)
                    _oldRecentPacketCount = fullReport.SessionTagCount;
                else
                    UpdateStatusTemperature(String.Format("N/A"));
				//End by Chingsheng for display extention data, 2015-01-14
                
            }

        }


        private void BackgroundTaskCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( null != e.Error )
            {
                System.Diagnostics.Debug.Assert( null == e.Error );
                Logger.Warning( e.Error );
                PopupMessage.DoPopup( PopupMessage.PopupType.UnableToComplete );
                return;
            }

            ReportBase report = e.Result as ReportBase;
            //@@@@System.Diagnostics.Debug.Assert(report != null);
            if ( report == null )
                return;

            //ContextRfidReader theContext = report.Context as ContextRfidReader;
            ContextBag theContext = report.Context as ContextBag;


            System.Diagnostics.Debug.Assert( theContext != null );
            if ( theContext == null )
            {
                Logger.Warning( "BackgroundTaskCompleted() was passed a report with a null context" );
                return;
            }
            StatusBarState.StateName newState = StatusBarState.StateName.Blank;
            String statusMessage = null;

            switch ( report.Outcome )
            {
                case OperationOutcome.Success:
                    newState = StatusBarState.StateName.Ready;
                    switch ( theContext.CurrentTask )
                    {
                        case ContextBag.ReaderTask.LoadFile:
                            statusMessage = report.ToString( );
                            break;

                        case ContextBag.ReaderTask.SaveFile:
                            statusMessage = String.Format( "Successfully saved {0}.", theContext.FileName );
                            break;

                        case ContextBag.ReaderTask.Export:
                            statusMessage = String.Format( "Successfully exported data to Excel." );
                            break;

                        case ContextBag.ReaderTask.TagAccess:
                            statusMessage = String.Format( "Successfully accessed {0} tag(s).", theContext.Reader.AccessCount );
                            break;

                        case ContextBag.ReaderTask.Connect:
                            statusMessage = report.ToString( );
                            break;

                        default:
                            statusMessage = String.Format( "Successfully {0} {1}.", theContext.TaskName.ToLower( ), report.ToString( ) );
                            break;
                    }
                    break;

                case OperationOutcome.SuccessWithUserCancel:
                    newState = StatusBarState.StateName.Canceled;
                    switch ( theContext.CurrentTask )
                    {
                        case ContextBag.ReaderTask.BuildingTables:
                            statusMessage = String.Format( "Stopped: successfully {0} {1} before stopped by user request.", theContext.TaskName.ToLower( ), report.ToString( ) );
                            break;

                        default:
                            statusMessage = String.Format( "Stopped: successfully {0} {1}.", theContext.TaskName.ToLower( ), report.ToString( ) );
                            break;
                    }

                    break;

                case OperationOutcome.SuccessWithInformation:
                    newState = StatusBarState.StateName.Infomation;
                    statusMessage = report.ToString( );
                    break;

                case OperationOutcome.FailByReaderError:
                    newState = StatusBarState.StateName.Error;
                    string error = "Unknown Error";
                    if ( report.OperationException != null )
                    {
                        error = report.OperationException.Message;
                    }
                    statusMessage = String.Format( "Reader Error: {0}", error );

                    // Force Application Close
                    MessageBox.Show("Explorer will be closed due to Reader Error.",
                                    "Reader - Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                    
                    
                    break;

                case OperationOutcome.FailByUserAbort:
                    newState = StatusBarState.StateName.Canceled;
                    switch ( theContext.CurrentTask )
                    {
                        case ContextBag.ReaderTask.BuildingTables:
                            statusMessage = String.Format( "Aborted: {0}.", report.ToString( ) );
                            break;

                        default:
                            statusMessage = String.Format( "Aborted: {0}.", report.ToString( ) );
                            break;
                    }
                    break;

                case OperationOutcome.FailByApplicationError:
                case OperationOutcome.FailByContext:
                case OperationOutcome.Unknown:
                    newState = StatusBarState.StateName.Error;
                    statusMessage = report.OperationException == null ? "Unknown Error" : report.OperationException.Message;
                    break;
            }

            ResetToolBar( );
            ResetPauseBtn();            
            

            if ( theContext.CurrentTask == ContextBag.ReaderTask.LoadFile ||
                theContext.CurrentTask == ContextBag.ReaderTask.Connect ||
                theContext.ID == ContextCollection.CurrentContext.ID )
            {
                UpdateStatusBar( newState, statusMessage );
                QueueWork<MethodInvoker>( "ProgressTaskFinished" );

                switch ( theContext.CurrentTask )
                {
                    case ContextRfidReader.ReaderTask.Idle:
                        break;

                    case ContextRfidReader.ReaderTask.ReadInventoryOnce:
                    case ContextRfidReader.ReaderTask.MonitoringInventory:
                    case ContextBag.ReaderTask.TagAccess:
                        rfidOperationReport opsReport = report as rfidOperationReport;
                        if ( theContext is ContextRfidReader )
                        {
                            ContextRfidReader readerContext = theContext as ContextRfidReader;
                            if ( readerContext != null )
                            {
                                if ( opsReport != null )
                                {
                                    readerContext.CurrentReport = opsReport;
                                    readerContext.SingulationRateAverage = ExponentialMovingAverage.Calculate( readerContext.SingulationRateCount == 0 ? opsReport.Rate : readerContext.SingulationRateAverage, opsReport.Rate );
                                    ++ readerContext.SingulationRateCount;
                                }
                                UpdatePanelCompletedLC( sender, e );
                            }
                        }
                        else if ( theContext is ContextVirtualReader )
                        {
                            ContextVirtualReader virtualContext = theContext as ContextVirtualReader;
                            if ( virtualContext != null )
                            {
                                if ( opsReport != null )
                                {
                                    virtualContext.CurrentReport = opsReport;
                                    virtualContext.SingulationRateAverage = ExponentialMovingAverage.Calculate( virtualContext.SingulationRateCount == 0 ? opsReport.Rate : virtualContext.SingulationRateAverage, opsReport.Rate );
                                    ++virtualContext.SingulationRateCount;
                                }
                                UpdatePanelCompletedVR( sender, e );
                            }
                        }
                        else
                        {

                        }

                        break;

                    case ContextRfidReader.ReaderTask.BuildingTables:
                        break;

                    case ContextBag.ReaderTask.LoadFile:
                        if ( report.Outcome != OperationOutcome.Success )
                            return;

                        DeviceToolStripMenuItem.Enabled = true;
                        rfidSimpleReport simple = report as rfidSimpleReport;
                        theContext.Reader = simple.NewReader as LakeChabotReader;

                        System.Diagnostics.Debug.Assert( theContext.Reader != null );

                        ActivateContext( theContext );
                        ProtocolControl ctrl = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
                        if ( ctrl != null )
                        {
                            ctrl.StatusChange( theContext.Reader.Name, ProtocolControl.ControlEvent.Load, theContext.Reader.RecentPacketList );
                        }

                        break;

                    case ContextBag.ReaderTask.SaveFile:

                        break;


                    default:
                        break;
                }


            }
            else
            {
                theContext.StatusState = newState;
                theContext.StatusMessage = statusMessage;

                if ( ContextCollection.ContextAsVirtualReader != null &&
                    theContext.Type == ContextBag.ContextType.RfidReader )
                {
                    ( ( ContextRfidReader ) theContext ).CurrentReport = ( rfidReportBase ) report;
                }
            }

            // Always notify the Protocol control of the state change when a table build completes
            ProtocolControl ctrl3 = GUIViewState.GuiPanelCollection[ GUIViewState.GUIView.ReaderProtocol ].Controls[ GUIViewState.ViewCollection[ GUIViewState.GUIView.ReaderProtocol ].ViewName ] as ProtocolControl;
            if ( ctrl3 != null && theContext.CurrentTask == ContextBag.ReaderTask.BuildingTables )
            {
                switch ( theContext.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        ContextRfidReader readerContext = theContext as ContextRfidReader;
                        if ( readerContext.Reader.TableResult == TableState.Ready )
                        {
                            ctrl3.StatusChange( readerContext.Reader.Name, ProtocolControl.ControlEvent.ReivewMode, readerContext.Reader.RecentPacketList );

                            //clark 2011.08.18. Show error message in status bar.
                            PacketData.PacketWrapper strcPkt = readerContext.Reader.RecentPacketList[ readerContext.Reader.RecentPacketList.Count - 1 ];
                            if (strcPkt.PacketType == PacketData.PacketType.CMD_END)
                            {
                                PacketData.cmd_end strcPktEnd = (PacketData.cmd_end)strcPkt.Packet;
                                if (strcPktEnd.Result != 0)
                                { 
                                   UpdateStatusBar( StatusBarState.StateName.TagAccessOptError, String.Format("Error Code: 0x{0:X3}", strcPktEnd.Result) );
                                }
                            }
                        }
                        break;

                    case ContextBag.ContextType.VirtualReader:
                        ContextVirtualReader virtualContext = theContext as ContextVirtualReader;
                        if ( virtualContext.Reader.TableResult == TableState.Ready )
                        {
                            ctrl3.StatusChange( virtualContext.Reader.Name, ProtocolControl.ControlEvent.ReivewMode, virtualContext.Reader.RecentPacketList );
                        }
                        break;
                }
            }

            // Start the automatic update
            if ( Properties.Settings.Default.automaticIndex &&
                theContext.CurrentTask != ContextBag.ReaderTask.BuildingTables &&
                ContextVirtualReader.Visible == false )
            {
                switch ( theContext.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        ContextRfidReader rfidContext = theContext as ContextRfidReader;
                        if ( rfidContext != null && rfidContext.Reader.TableResult == TableState.BuildRequired )
                        {
                            QueueWork<StartBuildTablesDelegate>( "StartBuildTables", rfidContext );
                        }
                        break;

                    case ContextBag.ContextType.VirtualReader:
                        break;

                    case ContextBag.ContextType.File:
                    default:
                        break;
                }
            }

            if ( ContextVirtualReader.IsVirtualReaderEnabled && theContext.Type == ContextBag.ContextType.RfidReader )
            {
                ContextVirtualReader.VirtualReader.Reader.RemoveActiveReader( );
            }

        }

        #endregion


        private void RestartApplication( )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            ControlPanelForm.CloseControlPanel( );
            Thread.Sleep( 100 );
            LakeChabotReader.AssemblyClosing( );
            Thread.Sleep( 1000 );
            Application.Restart( );
        }




//===============ToolMeanu Click Event==============================================

        //clark 2011.05.20 Let user to set comport in disconnected status.
        private void SetComPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check status. If the device is inventories, can't set com port.
            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if (reader != null && reader.Worker.IsBusy == true)
            {
                PopupMessage.Popup(PopupMessage.PopupType.Busy);
                return;
            }


            using (SetComport frm = new SetComport())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }
        }



        private void rfTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;
            
            //Check status. If the device is inventories, can't set com port.
            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if (reader != null && reader.Worker.IsBusy == true)
            {
                PopupMessage.Popup(PopupMessage.PopupType.Busy);
                return;
            }


            using (RFTest frm = new RFTest(this, reader.Reader))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }
        }
   


        private void ReturnLossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;
            
            //Check status. If the device is inventories, can't set com port.
            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if (reader != null && reader.Worker.IsBusy == true)
            {
                PopupMessage.Popup(PopupMessage.PopupType.Busy);
                return;
            }


            using (ReturnLoss frm = new ReturnLoss(this, reader.Reader))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }

        }

		// Add by Chingsheng for Command Test Window, 2015-01-30
        private void CommandTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            //Check status. If the device is inventories, can't set com port.
            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if (reader != null && reader.Worker.IsBusy == true)
            {
                PopupMessage.Popup(PopupMessage.PopupType.Busy);
                return;
            }

            using (CommandTest frm = new CommandTest(this, reader.Reader))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }
        }
		// End by Chingsheng for Command Test Window, 2015-01-30

        //Add FJ for NXP authentication function, 2018-02-02
        private void nxpAuthenticatetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            //Check status. If the device is inventories, can't set com port.
            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if (reader != null && reader.Worker.IsBusy == true)
            {
                PopupMessage.Popup(PopupMessage.PopupType.Busy);
                return;
            }

            using (NXPAuthentication frm = new NXPAuthentication(this, reader.Reader))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }
        }
        //End FJ for NXP authentication function, 2018-02-02

        //Add by FJ for CB-2 Tag Access function, 2018-03-20
        private void cB2TagAccessStripMenuItem_Click(object sender, EventArgs e)
        {
            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                return;
            }

            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            if (readerContext.Worker.IsBusy == true)
            {
                PopupMessage.DoPopup(PopupMessage.PopupType.Busy);
                return;
            }

            using (cB2TagAccess dlg = new cB2TagAccess(this, ActiveReader, ActiveReader.TagAccessDataSet))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    return;
                }
            }
        }
        //End by FJ for CB-2 Tag Access function, 2018-03-20

        private void deviceMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if ( item.Checked ) return;

            foreach ( ToolStripMenuItem i in DeviceToolStripMenuItem.DropDownItems ) i.Checked = false;

            item.Checked = true;
            ActivateContext( ( ContextID ) item.Tag );
        }


        private void fileToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );

            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                exportToolStripMenuItem.Enabled = false;
                return;
            }

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = context as ContextRfidReader;
                    exportToolStripMenuItem.Enabled = ( !readerContext.Worker.IsBusy ) && ( readerContext.Reader.TableResult == TableState.Ready || readerContext.Reader.TableResult == TableState.NotAvailable );
                    break;

                case ContextBag.ContextType.File:
                    exportToolStripMenuItem.Enabled = true;
                    break;

                case ContextBag.ContextType.VirtualReader:
                    ContextVirtualReader virtualContext = context as ContextVirtualReader;
                    exportToolStripMenuItem.Enabled = ( !virtualContext.Worker.IsBusy ) && ( virtualContext.Reader.TableResult == TableState.Ready || virtualContext.Reader.TableResult == TableState.NotAvailable );
                    break;

                default:
                    exportToolStripMenuItem.Enabled = false;
                    break;
            }

        }




        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ContextBag context = ContextCollection.CurrentContext;
            if ( context != null && context.Worker.IsBusy )
            {
                Logger.Warning( "openToolStripMenuItem_Click() called with busy worker.", PopupMessage.PopupType.NoOpenWhenReaderBusy );
                return;
            }

            CommonDialogSupport dlg = new CommonDialogSupport( CommonDialogSupport.DialogType.Open );
            if ( dlg.ShowDialog( ) == DialogResult.OK )
            {
                if ( Path.GetExtension( dlg.FileName ) == ".config" )
                {
                    OpenConfigFile( dlg.FileName );
                }
                else
                {
                    OpenContextFile( dlg.FileName );
                }
            }

        }



        private void closeFileToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ContextFile context = ContextCollection.ContextAsFile;
            if ( null == context )
            {
                Logger.Warning( "closeFileToolStripMenuItem_Click() called when ContextCollection.ContextAsFile is null." );
                return;
            }


            CloseContextFile( context );
        }


        private void saveToolStrip_Click( object sender, EventArgs e )
        {
            ContextBag context = ContextCollection.CurrentContext;

            if ( null == context )
            {
                Logger.Warning( new rfidError( rfidErrorCode.NoContext ), PopupMessage.PopupType.NoSaveWithoutContext );
                return;
            }
            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    break;

                case ContextBag.ContextType.File:
                    Logger.Warning( new rfidError( rfidErrorCode.NoSaveOfFileContext ), PopupMessage.PopupType.NoFileOverwrite );
                    return;

                case ContextBag.ContextType.VirtualReader:
                    PopupMessage.Popup( PopupMessage.PopupType.VirtualReaderNotSupported );
                    return;

                default:
                    Logger.Warning( new rfidError( rfidErrorCode.GeneralError ), PopupMessage.PopupType.UnableToComplete );
                    return;
            }


            ContextRfidReader readerContext = context as ContextRfidReader;

            if ( Properties.Settings.Default.noTempFile )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoSaveWithoutData );
                return;
            }

            if ( readerContext.Worker.IsBusy )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoSaveWhenReaderBusy );
                return;
            }


            if ( readerContext.Reader.TableResult != TableState.Ready )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoSaveWithoutIndexing );
                return;
            }


            if ( context.FileName == null )
            {
                CommonDialogSupport dlg = new CommonDialogSupport( CommonDialogSupport.DialogType.SaveAs );
                if ( dlg.ShowDialog( ) == DialogResult.OK )
                {
                    context.FileName = dlg.FileName;
                    SaveToContextFile( context );
                }
            }
            else
            {
                SaveToContextFile( context );
            }
        }


        private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                Logger.Warning( "saveAsToolStripMenuItem_Click() called when ContextCollection.CurrentContext is null." );
                return;
            }
            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    break;

                case ContextBag.ContextType.File:
                    Logger.Warning( new rfidError( rfidErrorCode.NoSaveOfFileContext ), PopupMessage.PopupType.NoFileOverwrite );
                    return;

                case ContextBag.ContextType.VirtualReader:
                    PopupMessage.Popup( PopupMessage.PopupType.VirtualReaderNotSupported );
                    return;

                default:
                    Logger.Warning( new rfidError( rfidErrorCode.GeneralError ), PopupMessage.PopupType.UnableToComplete );
                    return;
            }



            ContextRfidReader readerContext = context as ContextRfidReader;
            if ( readerContext != null )
            {
                if ( Properties.Settings.Default.noTempFile )
                {
                    PopupMessage.Popup( PopupMessage.PopupType.NoSaveWithoutData );
                    return;
                }

                if ( readerContext.Worker.IsBusy )
                {
                    PopupMessage.Popup( PopupMessage.PopupType.NoSaveWhenReaderBusy );
                    return;
                }


                if ( readerContext.Reader.TableResult != TableState.Ready )
                {
                    PopupMessage.Popup( PopupMessage.PopupType.NoSaveWithoutIndexing );
                    return;
                }
            }

            CommonDialogSupport dlg = new CommonDialogSupport( CommonDialogSupport.DialogType.SaveAs );
            if ( dlg.ShowDialog( ) == DialogResult.OK )
            {
                context.FileName = dlg.FileName;
                SaveToContextFile( context );
            }
        }


        private void exportToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                Logger.Warning( "exportToolStripMenuItem_Click() called without a current context.", PopupMessage.PopupType.UnableToComplete );
                return;
            }


            using ( ExportForm dlg = new ExportForm( ) )
            {
                foreach ( ExportTargets.ExportTargetClass target in ExportTargets.SupportedTypes )
                {
                    dlg.ExportTargetComboBox.Items.Add( target );
                }

                foreach ( GUIViewState.GUIView view in GUIViewState.ViewCollection.Keys )
                {
                    switch ( view )
                    {
                        case GUIViewState.GUIView.SummaryView:
                        case GUIViewState.GUIView.ReaderProtocol:
                            break;

                        case GUIViewState.GUIView.StandardView:
                            dlg.ExportViewCheckedListBox.Items.Add( GUIViewState.ViewCollection[ view ] );
                            break;


                        case GUIViewState.GUIView.InventoryData:
                        case GUIViewState.GUIView.RawPackets:
                        case GUIViewState.GUIView.ReaderRequests:
                        case GUIViewState.GUIView.ReaderCommands:
                        case GUIViewState.GUIView.ReaderAntennaCycles:
                        case GUIViewState.GUIView.AntennaPacket:
                        case GUIViewState.GUIView.InventoryCycle:
                        case GUIViewState.GUIView.InventoryRounds:
                        case GUIViewState.GUIView.InventoryParameters:
                        case GUIViewState.GUIView.BadPackets:
                        case GUIViewState.GUIView.InventoryCycleDiagnostics:
                        case GUIViewState.GUIView.InventoryRoundDiagnostics:
                        case GUIViewState.GUIView.EPCDataDiagnostics:
                        case GUIViewState.GUIView.ReadRateData:
                            switch ( context.Type )
                            {
                                case ContextBag.ContextType.RfidReader:
                                case ContextBag.ContextType.File:

                                    if ( ( ( ContextRfidReader ) context ).Reader.TableResult != TableState.NotAvailable )
                                        dlg.ExportViewCheckedListBox.Items.Add( GUIViewState.ViewCollection[ view ] );
                                    break;

                                case ContextBag.ContextType.VirtualReader:
                                    if ( ( ( ContextVirtualReader ) context ).Reader.TableResult != TableState.NotAvailable )
                                        dlg.ExportViewCheckedListBox.Items.Add( GUIViewState.ViewCollection[ view ] );
                                    break;
                                default:
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                }

                if ( context.ExportTarget != null )
                {
                    dlg.ExportTargetComboBox.SelectedItem = context.ExportTarget;
                }
                else
                {
                    dlg.ExportTargetComboBox.SelectedIndex = 0;
                }

                if ( context.ExportViewList == null )
                {
                    // 20090401 MTI Set default expot selection File->Export->Satndard View
                    /*
                    for ( int i = 0; i < dlg.ExportViewCheckedListBox.Items.Count; i++ )
                    {
                        dlg.ExportViewCheckedListBox.SetItemChecked( i, true );
                    }
                   */
                    dlg.ExportViewCheckedListBox.SetItemChecked(0, true);

                }
                else
                {
                    foreach ( GUIViewState.GUIViewClass view in context.ExportViewList )
                    {
                        int ndx = dlg.ExportViewCheckedListBox.FindStringExact( view.ToString( ) );
                        if ( ndx != -1 )
                            dlg.ExportViewCheckedListBox.SetItemChecked( ndx, true );
                    }
                }

                if ( dlg.ShowDialog( ) == DialogResult.OK )
                {
                    if ( dlg.ExportViewCheckedListBox.CheckedItems.Count > 0 )
                    {

                        GUIViewState.GUIViewClass[ ] views = new GUIViewState.GUIViewClass[ dlg.ExportViewCheckedListBox.CheckedItems.Count ];
                        for ( int i = 0; i < views.Length; i++ )
                            views[ i ] = ( GUIViewState.GUIViewClass ) dlg.ExportViewCheckedListBox.CheckedItems[ i ];
                        context.ExportTarget = ( ExportTargets.ExportTargetClass ) dlg.ExportTargetComboBox.SelectedItem;
                        context.ExportViewList = views;
                        StartDataExport( );
                    }
                }

                dlg.Close( );
            }

        }


        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }


        private void editToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );

            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                foreach ( ToolStripItem item in editToolStripMenuItem.DropDownItems )
                {
                    item.Enabled = false;
                }
                return;
            }

            switch ( context.Type )
            {
                case ContextBag.ContextType.RfidReader:
                    ContextRfidReader readerContext = context as ContextRfidReader;
                    if ( readerContext.Worker.IsBusy && !readerContext.IsPaused )
                    {
                        copyToolStripMenuItem.Enabled = false;
                        selectAllToolStripMenuItem.Enabled = false;
                        copyAllToolStripMenuItem.Enabled = false;
                        clearSessionToolStripMenuItem.Enabled = false;
                        return;
                    }
                    clearSessionToolStripMenuItem.Enabled = !readerContext.Worker.IsBusy;
                    break;


                case ContextBag.ContextType.VirtualReader:
                    ContextVirtualReader virtualContext = context as ContextVirtualReader;
                    if ( virtualContext.Worker.IsBusy && !virtualContext.IsPaused )
                    {
                        copyToolStripMenuItem.Enabled = false;
                        selectAllToolStripMenuItem.Enabled = false;
                        copyAllToolStripMenuItem.Enabled = false;
                        clearSessionToolStripMenuItem.Enabled = false;
                        return;
                    }
                    clearSessionToolStripMenuItem.Enabled = !virtualContext.Worker.IsBusy;
                    break;

                case ContextBag.ContextType.File:
                default:
                    foreach ( ToolStripItem item in editToolStripMenuItem.DropDownItems )
                    {
                        item.Enabled = false;
                    }
                    return;
            }


            GUIViewState.GUIView view = GUIViewState.View;
            Control ctrl = GUIViewState.GuiPanelCollection[ view ].Controls[ GUIViewState.ViewCollection[ view ].ViewName ];
            GridControl grid = ctrl as GridControl;
            if ( grid != null )
            {
                copyToolStripMenuItem.Enabled = grid.HasSelection;
                selectAllToolStripMenuItem.Enabled = grid.HasSelection;
                copyAllToolStripMenuItem.Enabled = grid.HasSelection;
            }
            else
            {
                copyToolStripMenuItem.Enabled = false;
                selectAllToolStripMenuItem.Enabled = false;
                copyAllToolStripMenuItem.Enabled = false;
            }

        }


        private void copyToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( ContextCollection.CurrentContext == null )
            {
                Logger.Warning( "copyToolStripMenuItem_Click() called with null context." );
                return;
            }

            GUIViewState.GUIView view = GUIViewState.View;
            Control ctrl = GUIViewState.GuiPanelCollection[ view ].Controls[ GUIViewState.ViewCollection[ view ].ViewName ];
            GridControl grid = ctrl as GridControl;
            if ( grid != null )
            {
                UpdateStatusBar( StatusBarState.StateName.Ready, grid.Copy( ) );
            }

        }


        private void selectAllToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( ContextCollection.CurrentContext == null )
            {
                Logger.Warning( "selectAllToolStripMenuItem_Click() called with null context." );
                return;
            }

            GUIViewState.GUIView view = GUIViewState.View;
            Control ctrl = GUIViewState.GuiPanelCollection[ view ].Controls[ GUIViewState.ViewCollection[ view ].ViewName ];
            GridControl grid = ctrl as GridControl;
            if ( grid != null )
            {
                grid.SelectAll( );
            }

        }


        private void copyAllToolStripMenuItem_Click( object sender, EventArgs e )
        {

            if ( ContextCollection.CurrentContext == null )
            {
                Logger.Warning( "selectAllToolStripMenuItem_Click() called with null context." );
                return;
            }

            GUIViewState.GUIView view = GUIViewState.View;
            Control ctrl = GUIViewState.GuiPanelCollection[ view ].Controls[ GUIViewState.ViewCollection[ view ].ViewName ];
            GridControl grid = ctrl as GridControl;
            if ( grid != null )
            {
                UpdateStatusBar( StatusBarState.StateName.Ready, grid.CopyAll( ) );
            }

        }


        private void clearSessionToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ClearSessionData( );
        }


        private void viewToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );

            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                Logger.LogMessage( "viewToolStripMenuItem_DropDownOpening() called when ContextCollection.CurrentContext is null." );
                foreach ( ToolStripItem item in viewToolStripMenuItem.DropDownItems )
                {
                    item.Enabled = false;
                }
                return;
            }

            switch ( context.Type )
            {

                case ContextBag.ContextType.RfidReader:
                case ContextBag.ContextType.VirtualReader:

                    IReader reader = null;
                    switch ( context.Type )
                    {
                        case ContextBag.ContextType.RfidReader:
                            reader = ( ( ContextRfidReader ) context ).Reader;
                            break;


                        case ContextBag.ContextType.VirtualReader:
                            reader = ( ( ContextVirtualReader ) context ).Reader;
                            break;
                    }

                    if ( reader == null )
                    {
                        System.Diagnostics.Debug.Assert( false, "Unknown context in view viewToolStripMenuItem_DropDownOpening" );
                        Logger.Warning( "Unknown context in view viewToolStripMenuItem_DropDownOpening" );
                        return;
                    }

                    // Check table state
                    TableState tableState = reader.TableResult;
                    switch ( tableState )
                    {
                        case TableState.Ready:
                        case TableState.Building:
                        case TableState.BuildRequired:
                        case TableState.NotAvailable:
                            // Table state is okay
                            break;

                        default:
                            System.Diagnostics.Debug.Assert( false );
                            Logger.LogMessage( String.Format( "viewToolStripMenuItem_DropDownOpening() got an unknown tablestatus: {0:g}", tableState ) );
                            return;
                    }


                    foreach ( GUIViewState.GUIViewClass viewClass in GUIViewState.ViewCollection.Values )
                    {
                        switch ( viewClass.ViewType )
                        {
                            case GUIViewState.GUIViewType.Root:
                                break;

                            case GUIViewState.GUIViewType.Basic:
                                if ( null != viewClass.Menu )
                                {
                                    viewClass.Menu.Enabled = true;
                                }
                                break;

                            case GUIViewState.GUIViewType.Break:
                                if ( null != viewClass.Menu )
                                {
                                    viewClass.Menu.Enabled = true;
                                    if ( tableState == TableState.NotAvailable )
                                    {
                                        viewClass.Menu.Visible = false;
                                    }
                                    else
                                    {
                                        if ( viewClass.View == GUIViewState.GUIView.Break1 )
                                            viewClass.Menu.Visible = true;
                                        else
                                            viewClass.Menu.Visible = tableState == TableState.Ready;
                                    }
                                }
                                break;

                            case GUIViewState.GUIViewType.Header:
                                if ( null != viewClass.Menu )
                                {
                                    viewClass.Menu.Enabled = true;
                                    //viewClass.Menu.Visible = tableState != TableState.NotAvailable;
                                    viewClass.Menu.Visible = tableState == TableState.Ready;
                                }
                                break;

                            case GUIViewState.GUIViewType.Specialized:
                                if ( null != viewClass.Menu )
                                {
                                    viewClass.Menu.Enabled = tableState == TableState.Ready;
                                    //viewClass.Menu.Visible = tableState != TableState.NotAvailable;
                                    viewClass.Menu.Visible = tableState == TableState.Ready;
                                }
                                break;

                            case GUIViewState.GUIViewType.Command:
                                if ( null != viewClass.Menu )
                                {
                                    switch ( viewClass.View )
                                    {
                                        case GUIViewState.GUIView.CalculateCommand:
                                            viewClass.Menu.Enabled = tableState == TableState.BuildRequired;
                                            viewClass.Menu.Visible = tableState != TableState.NotAvailable;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;

                            default:
                                System.Diagnostics.Debug.Assert( false );
                                Logger.LogMessage( String.Format( "viewToolStripMenuItem_DropDownOpening() got an unknown ViewType: {0:g} MenuCaption = {1}", viewClass.ViewType, viewClass.MenuCaption ) );
                                break;
                        }
                    }
                    break;

                case ContextBag.ContextType.File:
                    ContextFile fileContext = context as ContextFile;
                    if ( fileContext == null )
                    {
                        System.Diagnostics.Debug.Assert( false, "fileContext is null" );
                        Logger.Warning( "Unknown file context in view viewToolStripMenuItem_DropDownOpening" );
                        return;
                    }

                    foreach ( GUIViewState.GUIViewClass viewClass in GUIViewState.ViewCollection.Values )
                    {
                        switch ( viewClass.ViewType )
                        {
                            case GUIViewState.GUIViewType.Root:
                                break;

                            case GUIViewState.GUIViewType.Basic:
                            case GUIViewState.GUIViewType.Header:
                            case GUIViewState.GUIViewType.Break:
                            case GUIViewState.GUIViewType.Specialized:
                                if ( null != viewClass.Menu )
                                {
                                    viewClass.Menu.Enabled = true;
                                    viewClass.Menu.Visible = true;
                                }
                                break;

                            case GUIViewState.GUIViewType.Command:
                                if ( null != viewClass.Menu )
                                {
                                    switch ( viewClass.View )
                                    {
                                        case GUIViewState.GUIView.CalculateCommand:
                                            viewClass.Menu.Enabled = false;
                                            viewClass.Menu.Visible = true;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                break;

                            default:
                                System.Diagnostics.Debug.Assert( false );
                                Logger.LogMessage( String.Format( "viewToolStripMenuItem_DropDownOpening() got an unknown ViewType: {0:g} MenuCaption = {1}", viewClass.ViewType, viewClass.MenuCaption ) );
                                break;
                        }
                    }

                    break;


                default:
                    Logger.LogMessage( String.Format( "viewToolStripMenuItem_DropDownOpening() got an unknown context type: {0:g}.", context.Type ) );
                    System.Diagnostics.Debug.Assert( false );
                    break;
            }


        }


        private void viewBasicStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem menuItemClicked = sender as ToolStripMenuItem;

            if ( null == menuItemClicked || null == menuItemClicked.Tag )
            {
                return;
            }

            if ( !Enum.IsDefined( typeof( GUIViewState.GUIView ), menuItemClicked.Tag ) )
            {
                Logger.Warning( "Invalid view menu tag: " + menuItemClicked.Tag.ToString( ) );
                return;
            }

            GUIViewState.GUIViewClass view = GUIViewState.ViewCollection[ ( GUIViewState.GUIView ) menuItemClicked.Tag ];
            SetCurrentView( view.View );

        }


        private void viewSpecialStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem menuItemClicked = sender as ToolStripMenuItem;

            if ( null == menuItemClicked || null == menuItemClicked.Tag )
            {
                return;
            }

            if ( !Enum.IsDefined( typeof( GUIViewState.GUIView ), menuItemClicked.Tag ) )
            {
                Logger.Warning( "Invalid view menu tag: " + menuItemClicked.Tag.ToString( ) );
                return;
            }

            GUIViewState.GUIViewClass viewClass = GUIViewState.ViewCollection[ ( GUIViewState.GUIView ) menuItemClicked.Tag ];
            SetCurrentView( viewClass.View );

        }


        private void viewCommandStripMenuItem_Click( object sender, EventArgs e )
        {
            ToolStripMenuItem menuItemClicked = sender as ToolStripMenuItem;

            if ( null == menuItemClicked || null == menuItemClicked.Tag )
            {
                return;
            }

            if ( !Enum.IsDefined( typeof( GUIViewState.GUIView ), menuItemClicked.Tag ) )
            {
                Logger.Warning( "Invalid view menu tag: " + menuItemClicked.Tag.ToString( ) );
                return;
            }

            GUIViewState.GUIViewClass viewClass = GUIViewState.ViewCollection[ ( GUIViewState.GUIView ) menuItemClicked.Tag ];
            switch ( viewClass.View )
            {
                case GUIViewState.GUIView.CalculateCommand:
                    StartBuildTables( );
                    break;

                default:
                    Logger.Warning( String.Format( "Unexpected GUIViewClass in viewCommandStripMenuItem() ({0})", viewClass.ViewName ) );
                    break;
            }
        }


        private void DeviceToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );
        }

        private void readerToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );

            bool enableConfigure = false;
            bool enableInventory = false;
            bool enableOther     = false;
            bool enableClear     = false;
            bool enableStop      = false;
            bool enableAbort     = false;


            ContextBag context = ContextCollection.CurrentContext;
            if ( null == context )
            {
                //clark 2011.5.9 If disconnected, only ignore event and doesn't show menu.
                //System.Diagnostics.Debug.Assert( false );
                //Logger.Warning( "readerToolStripMenuItem_DropDownOpening called without a current context." );
                enableConfigure = false;
                enableInventory = false;
                enableOther     = false;
                enableClear     = false;
                enableStop      = false;
                enableAbort     = false;
            }
            else
            {
                //!!
                switch ( context.Type )
                {
                    case ContextBag.ContextType.RfidReader:
                        if ( ( ( ContextRfidReader ) context ).Worker.IsBusy )
                        {
                            enableConfigure = false;
                            enableInventory = false;
                            enableOther     = false;
                            enableClear     = false;
                            enableStop      = true;
                            enableAbort     = true;
                        }
                        else
                        {
                            switch ( LakeChabotReader.LibraryMode )
                            {
                                case rfid.Constants.LibraryMode.DEFAULT:
                                    {
                                        switch ( LakeChabotReader.RadioOperationMode )
                                        {
                                            case rfid.Constants.RadioOperationMode.NONCONTINUOUS:
                                                {
                                                    enableConfigure = true;
                                                    enableInventory = true;
                                                    enableOther     = true;
                                                    enableClear     = true;
                                                }
                                                break;
                                            case rfid.Constants.RadioOperationMode.CONTINUOUS:
                                                {
                                                    enableConfigure = true;
                                                    enableInventory = true;
                                                    enableOther     = true;
                                                    enableClear     = true;
                                                }
                                                break;
                                            default:
                                                {
                                                    enableConfigure = true;
                                                    enableInventory = true;
                                                    enableOther     = true;
                                                    enableClear     = true;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                default: // UNKNOWN Library mode
                                    {
                                        enableConfigure = false;
                                        enableInventory = false;
                                        enableOther     = false;
                                        enableClear     = true;
                                    }
                                    break;
                            }
                        }
                        break;

                    case ContextBag.ContextType.File:
                        enableConfigure = false;
                        enableInventory = false;
                        enableOther     = false;
                        enableClear     = true;
                        break;


                    case ContextBag.ContextType.VirtualReader:
                        if ( ( ( ContextVirtualReader ) context ).Worker.IsBusy )
                        {
                            enableConfigure = false;
                            enableInventory = false;
                            enableClear     = false;
                        }
                        else
                        {
                            enableConfigure = false;
                            enableInventory = true;
                            enableClear     = true;
                        }
                        enableOther = false;
                        break;

                    default:
                        System.Diagnostics.Debug.Assert( false );
                        Logger.Warning( "Unknown reader context in readerToolStripMenuItem_DropDownOpening" );
                        break;
                }
            }


            foreach ( ToolStripItem item in readerToolStripMenuItem.DropDownItems )
            {
                if ( item == configureReaderToolStripMenuItem )
                {
                    item.Enabled = enableConfigure;
                }
                else if ( item == MonitorInventoryMenuItem ||
                        item == inventoryOnceMenuItem )
                {
                    item.Enabled = enableInventory;
                }
                else if (item == tagAccessToolStripMenuItem)
                {
                    item.Enabled = enableOther;
                }
                else if (item == registerAccessToolStripMenuItem)
                {
                    item.Enabled = enableOther;
                }
                else if (item == stopOperationToolStripMenuItem)
                {
                    item.Enabled = enableStop;
                }
                else if (item == abortToolStripMenuItem)
                {
                    item.Enabled = enableAbort;
                }
                else if (item == ClearSession_toolStripMenuItem1)
                {
                    item.Enabled = enableClear;
                }
                else
                {
                    if ( item is ToolStripMenuItem )
                    {
                        ( ( ToolStripMenuItem ) item ).Enabled = false;
                    }
                }
            }
        }



        private void configureReaderToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( ContextCollection.ContextAsVirtualReader != null )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.VirtualReaderNotSupported, "Virtual Reader" );
                return;
            }

            ContextRfidReader reader = ContextCollection.ContextAsRfidReader;
            if ( reader != null && !reader.Worker.IsBusy )
            {
                using ( ConfigureForm frm = new ConfigureForm( reader.Reader ) )
                {
                    frm.ShowDialog( );
                }
            }
        }




        /// <summary>
        /// Starts a continuous inventory operation
        /// <remarks>This is also called by the Control Panel</remarks>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">May be empty, don't use</param>


        private void registerAccessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                System.Diagnostics.Debug.Assert(false, "readerContext is null");
                Logger.Warning("Unknown reader context in tagAccessMenuItem_Click");
                return;
            }

            using (FORM_RegisterAccess dlg = new FORM_RegisterAccess(readerContext.Reader))
            {
                dlg.ShowDialog();
            }
        }


        private void toolsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            RefreshStatusBar( );

            bool notBusy = true;
            foreach ( ContextBag bag in ContextCollection.ContextList )
            {
                ContextRfidReader readerContext = bag as ContextRfidReader;
                if ( readerContext != null && readerContext.Worker.IsBusy )
                {
                    notBusy = false;
                    break;
                }
            }
            optionsToolStripMenuItem.Enabled = notBusy;


            //clark 2011.7.26 RF Test Dialog show/hide
            if (ActiveReader == null)
            {
                RfTestToolStripMenuItem.Enabled = false;
                ReturnLossToolStripMenuItem.Enabled = false;
                commandTestToolStripMenuItem.Enabled = false; // Add by Chingsheng for Command Test Window, 2015-01-30
                nxpAuthenticatetoolStripMenuItem.Enabled = false; //Add FJ for NXP authentication function, 2018-02-02
            }     
        }

        //Add FJ for NXP authentication function, 2018-02-02
        private void extraFeaturestoolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshStatusBar();

            LakeChabotReader reader = new LakeChabotReader();

            if (rfid.Constants.Result.OK != reader.result_major)
            {
                throw new Exception(reader.result_major.ToString());
            }

            if (reader.uiModelNameMAJOR != 0x4D303358 && reader.uiModelNameMAJOR != 0x4D303658)
            {
                nxpAuthenticatetoolStripMenuItem.Enabled = false;
            }

            if (ActiveReader == null)
            {
                nxpAuthenticatetoolStripMenuItem.Enabled = false;
            } 
        }
        //End FJ for NXP authentication function, 2018-02-02

        private void runTestsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ContextVirtualReader.DumpPacketQueue( );
        }

        private void bridgeReadersToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( rfid.Constants.LibraryMode.DEFAULT != LakeChabotReader.LibraryMode &&
                 rfid.Constants.RadioOperationMode.NONCONTINUOUS != LakeChabotReader.RadioOperationMode
                )
            {
                PopupMessage.DoPopup( PopupMessage.PopupType.NotInEmulation, "Bridged Reader" );
                return;
            }

            foreach ( ContextBag bag in ContextCollection.ContextList )
            {
                ContextRfidReader readerContext = bag as ContextRfidReader;

                if ( readerContext != null && readerContext.Worker.IsBusy )
                {
                    PopupMessage.DoPopup( PopupMessage.PopupType.NoVirtualReaderWhenReaderBusy );
                    return;
                }
            }

            if ( !ContextVirtualReader.IsVirtualReaderEnabled )
            {
                PopupMessage.Popup( PopupMessage.PopupType.NoVirtualReaderWithoutTwo );
                return;
            }

            if ( ContextVirtualReader.Visible )
            {
                HideVirtualReader( );
            }
            else
            {
                ShowVirtualReader( );
            }

        }


        private void optionsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            foreach ( ContextBag bag in ContextCollection.ContextList )
            {
                ContextRfidReader readerContext = bag as ContextRfidReader;
                if ( readerContext != null && readerContext.Worker.IsBusy )
                {
                    Logger.Warning( "optionsToolStripMenuItem_Click() called with busy worker.", PopupMessage.PopupType.NoOptionWhenReaderBusy );
                    return;
                }
            }


            List<String> views = new List<String>( 10 );
            foreach ( GUIViewState.GUIViewClass viewClass in GUIViewState.ViewCollection.Values )
            {
                if ( viewClass.ViewType == GUIViewState.GUIViewType.Basic )
                    views.Add( viewClass.MenuCaption );
            }

            using ( OptionsForm frm = new OptionsForm( views.ToArray( ) ) )
            {
                if ( frm.ShowDialog( ) == DialogResult.OK )
                {
                    GUIViewState.InitializeDefault( );
                    if ( frm.RestartRequired )
                    {
                        if ( PopupMessage.Popup( PopupMessage.PopupType.QueryRestart ) == DialogResult.Yes )
                        {
                            QueueWork<MethodInvoker>( "RestartApplication" );
                            return;
                        }
                    }

                    if ( LakeChabotReader.LogPath != Properties.Settings.Default.logPath )
                        LakeChabotReader.LogPath = Properties.Settings.Default.logPath;
                    if ( LakeChabotReader.EnableLogging != Properties.Settings.Default.enableLogging )
                        LakeChabotReader.EnableLogging = Properties.Settings.Default.enableLogging;


                }
            }
        }


        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {  
            using ( AboutBox b = new AboutBox( ) )
            {
                b.ShowDialog( );
            }

        }


        
        //clark 2011.5.6 show "Retry Count" to let user choose.
        private void toolStripMenuItem_RETRY_COUNT_Click(object sender, EventArgs e)
        {
            //cancel all item
            foreach (ToolStripMenuItem tsm in toolStripDropBtnRetryCount.DropDownItems)
            {
                tsm.Checked = false; 
            }

            //checked current item
            ((System.Windows.Forms.ToolStripMenuItem)sender).Checked = true;
        }

        //Add by Wayne for get advanced inventory information, 2014-12-16
        private void StripDropBtnInventoryRule_Click(object sender, EventArgs e)
        {
			//Add by Chingsheng for display extention data, 2015-01-14
            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                return;
            }

            if (readerContext.Worker.IsBusy == true)
            {
                PopupMessage.DoPopup(PopupMessage.PopupType.Busy);
                return;
            }
			//End by Chingsheng for display extention data, 2015-01-14
            //Mod by Wayne for fix model integration bug, 2015-04-13
            //UInt32 uiModelNameMajor = 0;
            LakeChabotReader reader = new LakeChabotReader();
            //Mod by FJ for model name judgement, 2015-01-22 
            //reader.MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_MAIN), ref uiModelNameMajor);
            //if (uiModelNameMajor == 0x4D303358) //0x4D303358==M03X
			if (rfid.Constants.Result.OK != reader.result_major)
            {
                throw new Exception(reader.result_major.ToString());
            }
			//End by Wayne for fix model integration bug, 2015-04-13
            //Mod by Wayne for enable functions to fit M06 requirement, 2016-10-21
			//if (reader.uiModelNameMAJOR == 0x4D303358) //0x4D303358==M03X
            if (reader.uiModelNameMAJOR == 0x4D303358 || reader.uiModelNameMAJOR == 0x4D303658)
            //End by Wayne for enable functions to fit M06 requirement, 2016-10-21
			//End by FJ for model name judgement, 2015-01-22
            {
				//Add by Chingsheng for display extention data, 2015-01-14
                LakeChabotReader Reader = new LakeChabotReader();
                ResponseResponseFormat extarInfo = ResponseResponseFormat.DISABLED;
                Result result;
 
                result = Reader.API_GetInventoryResponseControl(ref extarInfo); 

                if (Result.OK != result)
                {
                    return ;
                }

                StripMenuExtenedInfo.Checked = (extarInfo == ResponseResponseFormat.ENABLED);
                //End by Chingsheng for display extention data, 2015-01-14
                StripMenuExtenedInfo.Enabled = true;
            }
        }
        //End by Wayne for get advanced inventory information, 2014-12-16

        //clark 2011.12.12. Support Inventory Continue Mode.
        private void toolStripMenuItemContinueMode_Click(object sender, EventArgs e)
        {
            this.bErrorKeepRunning = toolStripMenuItemContinueMode.Checked;
        }




//Button Event================================================================================
        private void Button0_Click_Configure(object sender, EventArgs e)
        {
            // chain to main menu item handler
            configureReaderToolStripMenuItem.PerformClick();
        }


        public void Button1_Click_Inventory(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            //clark 2011.4.25 Set tag access flag to inventory structure
            Global.TagAccessFlag strcTagFlag;
            strcTagFlag.PostMatchFlag     = IsPostSingulationEnable;
            strcTagFlag.SelectOpsFlag     = IsSelectCriteriaEnable;
            strcTagFlag.RetryCount        = GetRetryCount();
            strcTagFlag.bErrorKeepRunning = Properties.Settings.Default.bErrorKeepRunning;

            ActiveReader.strcTagFlag = strcTagFlag;

            this.StartMonitorInventory();
        }


        private void Button2_Click_InventoryOnce(object sender, EventArgs e)
        {
            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            //clark 2011.4.25 Set tag access flag to inventory structure
            Global.TagAccessFlag strcTagFlag;
            strcTagFlag.PostMatchFlag     = IsPostSingulationEnable;
            strcTagFlag.SelectOpsFlag     = IsSelectCriteriaEnable;
            strcTagFlag.RetryCount        = GetRetryCount();
            strcTagFlag.bErrorKeepRunning = false;

            ActiveReader.strcTagFlag = strcTagFlag;

            this.StartInventoryOnce();
        }

        private void Button3_Click_AccessTag(object sender, EventArgs e)
        {
            ContextRfidReader readerContext = ContextCollection.ContextAsRfidReader;
            if (readerContext == null)
            {
                //System.Diagnostics.Debug.Assert(false, "readerContext is null");
                //Logger.Warning("Unknown reader context in Button3_Click_AccessTag");
                return;
            }

            //Check whether device was detected or not.
            if (null == ActiveReader)
                return;

            if (readerContext.Worker.IsBusy == true)
            {
                PopupMessage.DoPopup(PopupMessage.PopupType.Busy);
                return;
            }


            using (FORM_TagAccess dlg = new FORM_TagAccess(ActiveReader, ActiveReader.TagAccessDataSet))
            {
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    //Set "RetryCount" to Tag Flag Structure.
                    TagAccessData TagAccessDataSet = dlg.TagAccessDataSet;
                    TagAccessReads TagAccessReadSet = dlg.TagAccessReadSet;//эボよΑTagAccessReadSet
                    TagAccessDataSet.strcTagFlag.RetryCount        = GetRetryCount();
                    TagAccessDataSet.strcTagFlag.bErrorKeepRunning = false;

                    ActiveReader.TagAccessDataSet = TagAccessDataSet;
                    ActiveReader.TagAccessReadSet = TagAccessReadSet;//??
                    //ActiveReader.TagAccessDataSet = dlg.TagAccessDataSet;
                    StartTagAccess();
                }
                else
                {
                    return;
                }
            }
        }

        private void Button4_Click_Stop(object sender, EventArgs e)
        {
            if (ActiveControler != null)
            {
                ActiveControler.RequestStop();
            }
        }

        private void Button5_Click_Pause(object sender, EventArgs e)
        {
            if (ActiveControler != null)
            {
                if (!pauseButtonChecked)
                {
                    PauseCurrentOperation();
                    ActiveControler.RequestPause();
                    pauseButtonChecked = true;
                }
                else
                {
                    ContinueAfterPause();
                    ActiveControler.Continue();
                    pauseButtonChecked = false;
                }

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void Button6_Click_abort(object sender, EventArgs e)/*Mod by Rick for ICON奔,2013-01-29*/
        {
            if (ActiveControler != null)
            {
                ActiveControler.RequestAbort();
            }
        }

        private void Button7_Click_Clear(object sender, EventArgs e)
        {
            // need to trace rationale ~ shouldn't there always be a context ?!
            ContextBag context = ContextCollection.CurrentContext;
            ContextRfidReader readerContext = context as ContextRfidReader;

            if (null == context)
            {
                Logger.Warning(new rfidError(rfidErrorCode.NoContext), PopupMessage.PopupType.NoClearWithoutContext);
                return;
            }

            if (readerContext.Worker.IsBusy)
            {
                PopupMessage.DoPopup(PopupMessage.PopupType.Busy);
                return;
            }

            // chain to main menu item handler
            ClearSessionData();
            //clearSessionToolStripMenuItem.PerformClick();
        }

        //Add by Wayne for get advanced inventory information, 2014-12-16
        private void StripMenuExtenedInfo_Checked(object sender, EventArgs e)
        {
            LakeChabotReader Reader = new LakeChabotReader();
            //Add by Wayne for change header text if extended info enabled, 2014-12-31
            GUIViewState.GUIView view = GUIViewState.View;
            Control ctrl = GUIViewState.GuiPanelCollection[view].Controls[GUIViewState.ViewCollection[view].ViewName];
            GridControl grid = ctrl as GridControl;
            //End by Wayne for change header text if extended info enabled, 2014-12-31

            //Mod by Chingsheng for display extention data, 2015-01-14
            if (StripMenuExtenedInfo.Checked == true)
            {
                Reader.API_SetInventoryResponseControl(ResponseResponseFormat.ENABLED);
                //Reader.API_SetInventoryResponseControl(1);
                //grid.setHeaderText(true); //Add by Wayne for change header text if extended info enabled, 2014-12-31
            }
            else
            {
                Reader.API_SetInventoryResponseControl(ResponseResponseFormat.DISABLED);
                //Reader.API_SetInventoryResponseControl(0);
                //grid.setHeaderText(false); //Add by Wayne for change header text if extended info enabled, 2014-12-31
            }
            // End by Chingsheng for display extention data, 2015-01-14
        }
        //End by Wayne for get advanced inventory information, 2014-12-16


//=======================Value======================================================
        public bool bErrorKeepRunning
        {
            set
            {
                Properties.Settings.Default.bErrorKeepRunning = value;
                Properties.Settings.Default.Save();

                toolStripMenuItemContinueMode.Checked = value;
            }
            get
            { 
                return Properties.Settings.Default.bErrorKeepRunning;
            }    
        }

    } //public partial class mainForm : Form


} //namespace RFID_Explorer

