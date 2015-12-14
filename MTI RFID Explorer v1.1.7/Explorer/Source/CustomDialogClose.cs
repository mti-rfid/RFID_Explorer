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
 * $Id: CustomDialogClose.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RFID_Explorer
{

	public delegate void DialogCloseDelegate(Object sender, DialogCloseEventArgs e);
	
	public class DialogCloseEventArgs : EventArgs
	{
		private DialogResult	_result			= DialogResult.OK;
		private bool			_restartRequired= false;
		private bool			_saveRequired	= false;
		private bool			_error			= false;
		private string			_errorMessage	= null;
		private object			_errorPage		= null;
		private Control			_errorControl	= null;
		private bool			_selectAll		= false;

		public DialogCloseEventArgs()
		{
		}

		public DialogCloseEventArgs(DialogResult result)
		{
			_result = result;
		}

		public DialogResult Result { get { return _result; } }
		public bool		Error				{get { return _error;			} }
		public string	ErrorMessage		{ get { return _errorMessage;	} }
		public object	ErrorPage			{ get { return _errorPage;		} }
		public Control	ErrorControl
		{
			get { return _errorControl;  }
			set { _errorControl = value; }
		}

		public bool	SelectAll
		{
			get { return _selectAll;  }
			set { _selectAll = value; }
		}


		public bool RestartRequired
		{
			get { return _restartRequired;  }
			set { _restartRequired = value; }
		}

		public bool SaveRequired 
		{
			get { return _saveRequired;		}
			set { _saveRequired = _saveRequired || value; }
		}

		public void PageError(string Message, object page)
		{
			_error = true;
			_errorMessage	= Message;
			_errorPage		= page;
		}
	}


}
