#ifndef _TRANSFER_DLL
#define _TRANSFER_DLL

#define  _DLL_EXPORT

#ifdef _DLL_EXPORT
#define DLLMyLib __declspec(dllexport)
#else
#define DLLMyLib __declspec(dllimport)
#endif


enum TRANS_MODE
{
	INTERRUPT_WITH_OVERLAP,
	INTERRUPT_WITHOUT_OVERLAP,
	REPORT,
};


enum RESULT
{	
	FAILURE,
	OK,
	INVALID_HANDLE,
	INVALID_PARAMETER,
	CLASS_POINT_IS_EMPTY,
	USB_REPORT_COUNT_NOT_MATCH,
	NO_SUPPORT_THE_DEVICE,
	BUFFER_OVERFLOW,
};


extern "C"
{
	//Return result================================================================
	DLLMyLib UINT  WINAPI API_USB_Open(UINT uiPID, UINT uiVID);
	DLLMyLib UINT  WINAPI API_Serial_Open(UINT uiComPort, DCB &r_Dcb);
	DLLMyLib UINT  WINAPI API_Close();

	//Clear buffer. Suggest use this command to clear the buufer after reconnected.
	DLLMyLib UINT  WINAPI API_ClearBuffer();

	//UINT uiMode
	//TRANS_MODE::INTERRUPT_WITH_OVERLAP	    Support USB and RS232. Use overlap to request time out.
	//TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP     Support USB and RS232. It will block until finish the task.
	//TRANS_MODE::REPORT                        Only support USB report. It will block until finish the task.
	DLLMyLib UINT  WINAPI API_Write(UINT uiMode, UCHAR *cData, UINT iLength);

	//UINT uiMode
	//TRANS_MODE::INTERRUPT_WITH_OVERLAP	    Support USB and RS232. Use overlap to request time out.
	//TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP     Support USB and RS232. It will block until finish the task.
	//TRANS_MODE::REPORT                        Only support USB report. It will block until finish the task.
	DLLMyLib UINT  WINAPI API_Read(UINT uiMode, UCHAR *cData, UINT *iLength);

	//Set wait time. The default is 200 ms.
	//If uiMode is OVERLAP mode, write/read can use this time to wait.
	DLLMyLib UINT  WINAPI API_SetOverlapTime(UINT r_uiWtOverlapTime, UINT r_uiRdOverlapTime);

	//Get USB  Max output report count 
	//or
	//RS232 receive data in the buffer
	DLLMyLib UINT  WINAPI API_AskRevCount(UINT *r_uiRxCount);

	//Get USB  Max output report count
	DLLMyLib UINT  WINAPI API_AskUsbWtCount(UINT *r_uiTxCount);

	//Get device communication interface
	//DEV_TYPE_NO	   0
	//DEV_TYPE_RS232   1
	//DEV_TYPE_USB     2
	DLLMyLib UINT  WINAPI API_AskDevType(UINT *r_uiDevType);




	//Return string================================================================
	
	//Get transfer.dll version
	DLLMyLib char* WINAPI API_AskVersion( );

	//Get USB detail Path or RS232 port
	DLLMyLib char* WINAPI API_AskDevPath( );
}


#endif