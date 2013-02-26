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
 * $Id: MacError.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace RFID_Explorer
{
	class MacError
	{
		private uint _errorNumber;
		private string _name;
		private string _description;

		public MacError(uint number, string name, string description)
		{
			_errorNumber = number;
			_name = name;
			_description = description;
		}

		public uint ErrorNumber
		{
			get { return _errorNumber; }
			set { _errorNumber = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}


	class MacErrorList : Dictionary<uint, MacError>
	{
		static private Dictionary<uint, MacError> Errors = null;


		public MacErrorList()
			: base(Errors)
		{

		}
		
		
		static MacErrorList()
		{
			

			string name = RFID_Explorer.Properties.Settings.Default.macErrorsFileName;
			string fileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), name);

			if (!File.Exists(fileName))
			{
				throw new Exception(String.Format("A critial configuration file ({0}) is missing.", fileName));
			}
			
			Dictionary<uint, MacError> errorList = new Dictionary<uint, MacError>();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.CheckCharacters = true;
			settings.CloseInput = true;
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;

            
            try
            {
                //clark not sure. 2011.10.24. In 64-bit win7, the application will crash.
                FileStream tmpStream = File.OpenRead(fileName);

                using( XmlReader xmlReader = XmlReader.Create( tmpStream, settings ) )
                {
                    xmlReader.MoveToContent();

                    while (!xmlReader.IsStartElement())
                    {
                        xmlReader.Read();
                    }

                    if (xmlReader.Name != "MacErrors")
                    {
                        throw new Exception("The \"MacErrors\" element is missing.");
                    }

                    xmlReader.ReadToFollowing("error");
                    do
                    {
                        string id = xmlReader.GetAttribute("id");
                        if (id.StartsWith("0x"))
                            id = id.Substring(2);
                        UInt16 errorCode = UInt16.Parse(id, System.Globalization.NumberStyles.HexNumber);
                        string errorName = xmlReader.GetAttribute("name");
                        string errorDesc = xmlReader.ReadElementContentAsString();

                        errorList.Add(errorCode, new MacError(errorCode, errorName, errorDesc));
                    } while (xmlReader.IsStartElement("error"));

                }

            }
            catch(Exception ex)
            {
            
            }

			Errors = errorList;
		}
	}




}
