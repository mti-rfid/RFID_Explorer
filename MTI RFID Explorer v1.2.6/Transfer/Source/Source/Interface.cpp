#include "stdafx.h"
#include "Interface.h"



ClsDeviceBase *clsDevice = NULL;
CString DLL_VERSION;
CString ClsDeviceBase::m_strPath = "";


UINT WINAPI API_USB_Open(UINT uiPID, UINT uiVID)
{
	if( NULL != clsDevice)
		return RESULT::CLASS_POINT_IS_EMPTY;

	ClsUSBPort  *clsUSBPort = NULL;


	clsDevice  = new ClsUSBPort;
	clsUSBPort = dynamic_cast<ClsUSBPort *>(clsDevice);
	
	if( NULL == clsUSBPort )
	{
		API_Close();
		return RESULT::CLASS_POINT_IS_EMPTY;	
	}

	if( false == clsUSBPort->API_Open(uiPID, uiVID) )
	{
		API_Close();
		return RESULT::FAILURE;
	}

	return RESULT::OK;
}


UINT WINAPI API_Serial_Open(UINT uiComPort, DCB &r_Dcb)
{
	if( NULL != clsDevice)
		return RESULT::CLASS_POINT_IS_EMPTY;

	ClsSerial *clsSerial = NULL;


	clsDevice = new ClsSerial;
	clsSerial = dynamic_cast<ClsSerial *>(clsDevice);

	if( NULL == clsSerial )
	{
		API_Close();
		return RESULT::CLASS_POINT_IS_EMPTY;	
	}

	if( false == clsSerial->API_Open(uiComPort, r_Dcb) )
	{
		API_Close();
		return RESULT::FAILURE;
	}

	return RESULT::OK;
}


UINT WINAPI API_Close()
{
	clsUTILITY::DelObj(CLOSE_TYPE_POINT, clsDevice);


	if( NULL == clsDevice)
		return RESULT::OK;
	else
		return RESULT::FAILURE;
}


//UINT uiMode
//TRANS_MODE::INTERRUPT_WITH_OVERLAP	    Support USB and RS232. Use overlap to request time out. 
//TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP     Support USB and RS232. It will block until finish the task.
//TRANS_MODE::REPORT                        Only support USB report.
UINT WINAPI API_Write(UINT uiMode, UCHAR *cData, UINT iLength)
{
	if( NULL == clsDevice )
		return RESULT::CLASS_POINT_IS_EMPTY;

	return clsDevice->API_Write(uiMode, cData, iLength);
}


//UINT uiMode
//TRANS_MODE::INTERRUPT_WITH_OVERLAP	    Support USB and RS232. Use overlap to request time out. 
//TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP     Support USB and RS232. It will block until finish the task.
//TRANS_MODE::REPORT                        Only support USB report.
UINT WINAPI API_Read(UINT uiMode, UCHAR *cData, UINT *iLength)
{

	if( NULL == clsDevice )
		return RESULT::CLASS_POINT_IS_EMPTY;

	return clsDevice->API_Read(uiMode, cData, iLength);
}


//Clear buffer. Suggest use this command to clear the buufer after reconnected.
UINT WINAPI API_ClearBuffer()
{
	if( NULL == clsDevice )
		return RESULT::FAILURE;

	if(clsDevice->API_ClearBuffer() == true)
		return RESULT::OK;
	else
		return RESULT::FAILURE;
}


//Set wait time. The default is 200 ms.
//If uiMode is OVERLAP mode, write/read can refer this time to wait.
UINT WINAPI API_SetOverlapTime(UINT r_uiWtOverlapTime, UINT r_uiRdOverlapTime)
{
	if( NULL == clsDevice )
		return RESULT::CLASS_POINT_IS_EMPTY;

	clsDevice->API_SetOverlapTime(r_uiWtOverlapTime, r_uiRdOverlapTime);
	
	return RESULT::OK;
}


//Get USB  Max output report count 
//or
//RS232 receive data in the buffer
UINT WINAPI API_AskRevCount(UINT *r_uiRxCount)
{
	*r_uiRxCount = 0;

	if( NULL == clsDevice)
		return RESULT::CLASS_POINT_IS_EMPTY;
		
	*r_uiRxCount = clsDevice->API_AskRevCount();

	return RESULT::OK;
}


//Get USB  Max output report count
UINT WINAPI API_AskUsbWtCount(UINT *r_uiTxCount)
{
	*r_uiTxCount = 0;

	if( NULL == clsDevice)
		return RESULT::CLASS_POINT_IS_EMPTY;

	//Only USB support API_AskUsbWtCount API.
	if( clsDevice->API_AskDevType() != DEVICE_TYPE::USB )
		return RESULT::NO_SUPPORT_THE_DEVICE;

	ClsUSBPort  *clsUSBPort = NULL;
	clsUSBPort = dynamic_cast<ClsUSBPort *>(clsDevice);

	if(clsUSBPort == NULL)
		return  RESULT::CLASS_POINT_IS_EMPTY;

	*r_uiTxCount = clsUSBPort->API_AskUsbWtCount();

	return RESULT::OK;
}


//Get device communication interface
//DEV_TYPE_NO	   0
//DEV_TYPE_RS232   1
//DEV_TYPE_USB     2
UINT WINAPI API_AskDevType(UINT *r_uiDevType)
{
	*r_uiDevType = DEVICE_TYPE::NO;

	if( NULL == clsDevice )
		return RESULT::CLASS_POINT_IS_EMPTY;

	*r_uiDevType = clsDevice->API_AskDevType();

	return RESULT::OK;
}


//==================================================================================

//Get transfer.dll version
char * WINAPI API_AskVersion( )
{
	if( DLL_VERSION.IsEmpty() == true)
		DLL_VERSION.Format("%d.%d.%d", VER_MAJOR, VER_MINOR, VER_RELEASE);

	return (char*)DLL_VERSION.GetString();	
}


//Get USB detail Path or RS232 port 
char*  WINAPI API_AskDevPath( )
{
	return (char*)ClsDeviceBase::m_strPath.GetString();
}

