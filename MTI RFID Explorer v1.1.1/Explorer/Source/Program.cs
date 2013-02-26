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
 * $Id: Program.cs,v 1.6 2009/12/03 05:57:11 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;


namespace RFID_Explorer
{

	static class Program
	{

		private const int ERROR_EVENTLOG_FILE_CORRUPT  =    1500;
		private const int ERROR_EVENTLOG_CANT_START    =    1501;
		private const int ERROR_LOG_FILE_FULL          =    1502;


        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += FindAssem;
        }

        static Assembly FindAssem(object sender, ResolveEventArgs args)
        {
            String path = Path.GetDirectoryName
                (
                    Assembly.GetExecutingAssembly().Location
                ) +
                "\\..\\Common\\CSharp\\" +
                new AssemblyName(args.Name).Name +
                ".dll";

            if (!File.Exists(path))
            {
                return null;     // Sanity check
            }

            return Assembly.LoadFrom(path);
        }



		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		//[MTAThread]
		[STAThread]
		static void Main()               
        {

            if ( true == isOpen() )
            { 
                MessageBox.Show("Already Opened");
                return;            
            }


			System.Threading.Thread.CurrentThread.Name = "MainGUIThread";
			System.Diagnostics.Debug.WriteLine(String.Format("{0} is threadID {1}", System.Threading.Thread.CurrentThread.Name, System.Threading.Thread.CurrentThread.ManagedThreadId));
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException +=new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);                

			try
			{
				string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

				string name = RFID_Explorer.Properties.Settings.Default.macErrorsFileName;
				string fileName = Path.Combine(dir, name);
				if (!System.IO.File.Exists(fileName))
					throw new Exception(String.Format("A critial configuration file ({0}) is missing.", fileName));

				Application.Run(new mainForm());
			}
			catch (TypeInitializationException exp)
			{
				MessageBox.Show(exp.Message);
				System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
				log.WriteEntry(FormatEventMessage(exp), System.Diagnostics.EventLogEntryType.Error, 1);
				if (exp.InnerException != null)
				{
					log.WriteEntry(FormatEventMessage(exp.InnerException), System.Diagnostics.EventLogEntryType.Error, 2);
				}
			}
			catch (RFID.RFIDInterface.rfidException rfid)
			{
				System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
				try
				{
					log.WriteEntry(rfid.Message, System.Diagnostics.EventLogEntryType.Error, (int)rfid.ErrorCode.ErrorCode);
				}
				catch (Exception)	{	}
			}
			catch (System.ComponentModel.Win32Exception win32)
			{
				
				switch (win32.NativeErrorCode)
				{
						// no point in trying to write the log
					case ERROR_EVENTLOG_FILE_CORRUPT:
					case ERROR_EVENTLOG_CANT_START:
					case ERROR_LOG_FILE_FULL:
						MessageBox.Show("Unable to write to Application Log.\n\n" + win32.Message);
						break;

					default:

						try
						{
							System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
							log.WriteEntry(win32.Message, System.Diagnostics.EventLogEntryType.Error, 1);
						}
						catch (Exception)	{	}
						break;
				}
			}
			catch (Exception e)
			{
				try
				{
					System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
					log.WriteEntry(e.Message, System.Diagnostics.EventLogEntryType.Error, 1);

                    MessageBox.Show(e.Message);
				}
				catch (Exception)
				{
					MessageBox.Show("Unable to write to Application Log.\n\n" + e.Message);
				}

				throw;
			}
		}

		private static string FormatEventMessage(Exception e)
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
					e.InnerException);
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			DialogResult result = DialogResult.Cancel;
			System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
			RFID.RFIDInterface.rfidLogErrorException logError = e.Exception as RFID.RFIDInterface.rfidLogErrorException;
			if (logError == null)
			{
				RFID.RFIDInterface.rfidException rfid = e.Exception as RFID.RFIDInterface.rfidException;
				if (rfid == null)
				{
					log.WriteEntry(FormatEventMessage(e.Exception), System.Diagnostics.EventLogEntryType.Error);
				}
				else
				{
					log.WriteEntry(FormatEventMessage(rfid), System.Diagnostics.EventLogEntryType.Error, (int)rfid.ErrorCode.ErrorCode);
				}
			}
			else
			{
				if (!logError.HasBeenLogged)
				{
					RFID.RFIDInterface.rfidException rfid = logError.InnerException as RFID.RFIDInterface.rfidException;
					if (rfid == null)
					{
						log.WriteEntry(FormatEventMessage(logError.InnerException), System.Diagnostics.EventLogEntryType.Error);
					}
					else
					{
						log.WriteEntry(FormatEventMessage(rfid), System.Diagnostics.EventLogEntryType.Error, (int)rfid.ErrorCode.ErrorCode);
					}
				}
				Application.Exit();
			}

			try
			{
			    result = ShowThreadExceptionDialog(e.Exception);
			}
			catch
			{
			    try
			    {
			        MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
			    }
			    finally
			    {
			        Application.Exit();
			    }
			}
            // Exits the program when the user clicks Abort.
			if (result == DialogResult.Abort)
                try
                {
                    Application.Exit();
                }
                catch
                {
                    Application.Exit();
                }
                finally
                {
                    Application.Exit();
                }
        }

		// Creates the error message and displays it.
		private static DialogResult ShowThreadExceptionDialog(Exception e)
		{
			// Show Full Stack Trace Information
			// string errorMsg = "An error occurred please contact the adminstrator with the following information:\n\n";
			// errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(e.Message, "Explorer Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
		}


        private static bool isOpen()
        {
            System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(current.ProcessName);
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (process.ProcessName == current.ProcessName)
                    {
                        return true;
                    }
                }
            }

            return false;        
        }



	}


}