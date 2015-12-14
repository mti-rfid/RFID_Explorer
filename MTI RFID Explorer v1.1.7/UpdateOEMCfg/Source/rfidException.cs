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
 * $Id: rfidException.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateOEMCfgTool.exception
{

	public enum rfidErrorCode
	{
		NoError,
		LibraryNotFound,
		LibraryNotInitialized,
		LibraryFailedToInitialize,
		NoContext,
		ReaderFailedToInitialize,
		AlreadyBoundToAReader,
		CannotBindToStaticReader,
		InvalidRfidReaderID,				// Handle of rfidReaderID is invalid
		LocationTypeNotSupported,
		StandardNameUsedAsCustom,
		ReaderIsNotBound,
		ParsingError,
		PacketDataTooSmall,
		PacketSizeTooSmall,
		UnsupportedPacketVersion,
		UnknownPacketType,
		InvalidState,
		InvalidField,
		InvalidPacketFile,
		NoSaveOfFileContext,
		TablesAreNotReady,
		DeserializeError,
		DuplicateLinkProfileID,
		UnableToConnect,
		ConnectionLost,
		ReaderError,
		GeneralError,
	}

	public class rfidError
	{
		public rfidErrorCode ErrorCode;

		public rfidError(rfidErrorCode errorCode) 
		{
			ErrorCode = errorCode; 
		}
		
		public override string ToString()
		{
			return Enum.GetName(ErrorCode.GetType(), ErrorCode);
		}
		
	}

	public class rfidException : SystemException
	{
		private rfidError _errorCode = new rfidError(rfidErrorCode.GeneralError);

		public rfidError ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}
		public rfidException() : base() { }
		public rfidException(string message) : base(message) { }
		public rfidException(rfidErrorCode err)
			: base()
		{
			_errorCode = new rfidError(err);
		}

		public rfidException(rfidErrorCode err, String message)
			: base(message)
		{
			_errorCode = new rfidError(err);
		}

		public rfidException(Exception innerException)
			: base("See inner Exception", innerException)
		{

		}
		public override string  Message
		{
		get 
			{
				return "*RFID Exception*  " + 
					(_errorCode != null ? _errorCode.ToString() + " [" : " [") +
					base.Message +
					"]";
			}
		}
	} // public class rfidException : SystemException


	public class rfidReaderException : rfidException
	{
		private string _message;
		public rfidReaderException(string message)
			: base(rfidErrorCode.ReaderError, message)
		{
			_message = message;
		}

		public override string Message
		{
			get
			{
				return "Reader Error: " + _message;
			}
		}
	}

	public class rfidLogErrorException : rfidException
	{
		public bool HasBeenLogged = false;

//		public rfidLogErrorException(string Message)
//			: base(Message)
//		{
//		}

		public rfidLogErrorException(Exception innerException)
			: base(innerException)
		{
		}

		public rfidLogErrorException(Exception innerException, bool hasBeenLogged)
			: base(innerException)
		{
			this.HasBeenLogged = hasBeenLogged;
		}

	}
	
	public class rfidUnknownReaderTypeException : rfidException
	{
		public rfidUnknownReaderTypeException(string Name)
			: base(String.Format("'{0}' is not a known reader type.", Name))
		{
		}



	}

	public class rfidInvalidPacketException : rfidException
	{
		public rfidInvalidPacketException(rfidErrorCode errorCode, byte[] packetData, int offset) 
			: base(errorCode, String.Format("Bad Packet: '{0}'.", BitConverter.ToString(packetData, offset))) { }

		public rfidInvalidPacketException(rfidErrorCode errorCode, string Message)
			: base(errorCode, Message) { }

		public rfidInvalidPacketException(string Message) 
			:base(rfidErrorCode.ParsingError, Message) { }

	}

	public class rfidInvalidPacketFieldException : rfidInvalidPacketException
	{
		public rfidInvalidPacketFieldException(string fieldName, string expectedValue, string actualValue)
			: base(rfidErrorCode.InvalidField, String.Format("Packet field '{0}' expected ({1}) but packet had ({2}).", fieldName, expectedValue, actualValue)) { }

		

	}


}

