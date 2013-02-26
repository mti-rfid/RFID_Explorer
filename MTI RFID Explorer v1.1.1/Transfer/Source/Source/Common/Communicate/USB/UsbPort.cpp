#include "stdafx.h"
#include "USBPort.h"


ClsUSBPort::ClsUSBPort()
{
	m_uiPID = 0;
	m_uiVID = 0;
	m_bIsCatchHid = false;
	m_pstrcDetail = NULL;
	m_pPreparsedData = NULL;
	m_hdlRdHID = INVALID_HANDLE_VALUE;
	m_hdlWtHID = INVALID_HANDLE_VALUE;
	m_hdlHID   = INVALID_HANDLE_VALUE;
	m_hDeviceInfo = INVALID_HANDLE_VALUE;

	ZeroMemory( &m_HidCaps,   sizeof(HIDP_CAPS) );
	ZeroMemory( &m_OverlapRd, sizeof(OVERLAPPED) );
	ZeroMemory( &m_OverlapWt, sizeof(OVERLAPPED) );
	ZeroMemory( &m_guidHID, sizeof(GUID) );
}



ClsUSBPort::~ClsUSBPort()
{
	CloseUSB();
}


bool ClsUSBPort::API_Open(UINT uiPID, UINT uiVID)
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_Open\n") );

	try{
#endif
		m_uiPID = uiPID;
		m_uiVID = uiVID;

		CloseUSB();

		if( FindHID() == true )			
			return true;
		else
			return false;

#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif
}

void ClsUSBPort::API_Close()
{
#ifdef	TRY_DEBUG
	try{
#endif


	CloseUSB();


#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif
}





RESULT ClsUSBPort::API_Write(UINT uiMode, UCHAR *r_cData, UINT iLength)
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_Write\n") );

	try{
#endif	

	HANDLE hdlWt = m_hdlHID;

#ifdef USE_PIPE
	hdlWt = m_hdlWtHID;
#endif

	if( hdlWt == INVALID_HANDLE_VALUE )
		return RESULT::INVALID_HANDLE;


	if( TRANS_MODE::INTERRUPT_WITH_OVERLAP    != uiMode  &&
		TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP != uiMode  &&
		TRANS_MODE::REPORT                    != uiMode      )	
	{
		return RESULT::INVALID_PARAMETER;
	}

	m_OverlapWt.hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);


	bool   result  = false;
	DWORD  dwWrite = 0;
	UCHAR *ucData  = new UCHAR[m_HidCaps.OutputReportByteLength];
	ZeroMemory(ucData, m_HidCaps.OutputReportByteLength);
	memcpy(ucData, r_cData, m_HidCaps.OutputReportByteLength);	

	switch(uiMode)
	{
		case TRANS_MODE::INTERRUPT_WITH_OVERLAP:
			result = WriteFile( hdlWt,
				 				ucData,
				 				m_HidCaps.OutputReportByteLength,
			   	 				&dwWrite,
				 				&m_OverlapWt );

			if( WAIT_OBJECT_0 == WaitForSingleObject(m_OverlapWt.hEvent, uiWtOverlapTime) )
			{
				result = true;
			}
			else
			{
				CancelIo(hdlWt);
				result = false;
			}
			break;

		case TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP:
			result = WriteFile( hdlWt,
			 					ucData,
			 					m_HidCaps.OutputReportByteLength,
		   	 					&dwWrite,
			 					NULL );
			break;

		case TRANS_MODE::REPORT:
			result = HidD_SetOutputReport(hdlWt,
										  ucData,
										  m_HidCaps.OutputReportByteLength);
			break;

		default:
			if(ucData)	 clsUTILITY::DelObj( CLOSE_TYPE_POINT, ucData);
			clsUTILITY::DelObj( CLOSE_TYPE_HANDLE, m_OverlapWt.hEvent);
			return RESULT::INVALID_PARAMETER;
	}

	
	if(ucData)	 clsUTILITY::DelObj( CLOSE_TYPE_POINT, ucData);	
	
	clsUTILITY::DelObj( CLOSE_TYPE_HANDLE, m_OverlapWt.hEvent);

	if(result == true)
		return RESULT::OK;
	else
		return RESULT::FAILURE;


#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif


}




RESULT ClsUSBPort::API_Read(UINT uiMode, UCHAR *cData, UINT *iLength)
{	
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_Read\n") );

	try{
#endif

	UINT   tmpLen     = *iLength;
	RESULT enumResult = RESULT::OK;
	HANDLE hdlRd	  = m_hdlHID;

#ifdef USE_PIPE
	hdlRd = m_hdlRdHID;
#endif


	//Initial parameter
	*iLength = 0;

	if( hdlRd == INVALID_HANDLE_VALUE )
		return RESULT::INVALID_HANDLE;

	if( tmpLen > m_HidCaps.InputReportByteLength)	
		return RESULT::USB_REPORT_COUNT_NOT_MATCH;

	if( TRANS_MODE::INTERRUPT_WITH_OVERLAP    != uiMode  &&
		TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP != uiMode  &&
		TRANS_MODE::REPORT                    != uiMode      )	
	{
		return RESULT::INVALID_PARAMETER;
	}

	
	m_OverlapRd.hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);


	DWORD dwRead = 0;
	bool  result = false;

	UCHAR *ucData = NULL;
	ucData = new UCHAR[m_HidCaps.InputReportByteLength];
	ZeroMemory(ucData, m_HidCaps.InputReportByteLength);	

	switch(uiMode)
	{
		case TRANS_MODE::INTERRUPT_WITH_OVERLAP:

			ReadFile( hdlRd,
			  	      ucData,
					  m_HidCaps.InputReportByteLength,
				   	  &dwRead,
					  &m_OverlapRd );

			if( WAIT_OBJECT_0 == WaitForSingleObject(m_OverlapRd.hEvent, uiRdOverlapTime) )
			{
				result = true;
			}
			else
			{
				CancelIo(hdlRd);
				result = false;
			}
			break;

		case TRANS_MODE::INTERRUPT_WITHOUT_OVERLAP:
			result = ReadFile( hdlRd,
							   ucData,
							   m_HidCaps.InputReportByteLength,
				   			   &dwRead,
							   NULL );
			break;

		case TRANS_MODE::REPORT:
			result = HidD_GetInputReport(hdlRd,
									     ucData,
									     m_HidCaps.OutputReportByteLength);
			break;

		default:
			if(ucData)	 clsUTILITY::DelObj( CLOSE_TYPE_POINT, ucData);
			clsUTILITY::DelObj( CLOSE_TYPE_HANDLE, m_OverlapRd.hEvent);
			return RESULT::INVALID_PARAMETER;
			break;
	}
	

	if(result == true)
	{
		memcpy(cData, ucData, tmpLen);
		*iLength = tmpLen;			

		enumResult = RESULT::OK;
	}
	else
	{
		enumResult = RESULT::FAILURE;
	}

	if(ucData)	 clsUTILITY::DelObj( CLOSE_TYPE_POINT, ucData);

	clsUTILITY::DelObj( CLOSE_TYPE_HANDLE, m_OverlapRd.hEvent);

	return enumResult;


#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif
}




//=====================================================================================================
bool ClsUSBPort::FindHID(void)
{

#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer FindHID\n") );

	try{
#endif


	SP_DEVICE_INTERFACE_DATA			strcInterfaceData;
	HIDD_ATTRIBUTES						strcAttributes;
	INT									iHidIndex       = 0;
	DWORD								dwInterfaceSize = 0;
	bool								LastDevice      = false;
	bool								bResult         = false;

	ZeroMemory(&strcInterfaceData, sizeof(SP_DEVICE_INTERFACE_DATA));

	//Get the HID Guid
	HidD_GetHidGuid(&m_guidHID);

	// Create a HDEVINFO with all USB devices.
	m_hDeviceInfo = SetupDiGetClassDevs(&m_guidHID,
									    NULL,
									    NULL,
									    DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

	if(m_hDeviceInfo == INVALID_HANDLE_VALUE)
		return false;

	
	strcInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

	// Enumerate through all devices in Set. To Ask whether there is a HID or not.
	for( iHidIndex = 0; SetupDiEnumDeviceInterfaces(m_hDeviceInfo,NULL,&m_guidHID,iHidIndex,&strcInterfaceData); iHidIndex++) 										
	{

		do
		{
			//Get Detail strcture size "dwInterfaceSize"
			SetupDiGetDeviceInterfaceDetail(m_hDeviceInfo,
											&strcInterfaceData,
											NULL,
											0,
											&dwInterfaceSize,				
											NULL);
			

			//Allocate   memory   for   the   hDevInfo   structure
			//Because the path of usb may change, the memory was different eveyy time.
			m_pstrcDetail         = (PSP_INTERFACE_DEVICE_DETAIL_DATA)malloc(dwInterfaceSize);
			m_pstrcDetail->cbSize = sizeof(SP_INTERFACE_DEVICE_DETAIL_DATA);


			//call the function again. Get the path of HID
			bResult = SetupDiGetDeviceInterfaceDetail(m_hDeviceInfo,			
													  &strcInterfaceData,		
													  m_pstrcDetail,	   	    
													  dwInterfaceSize,	    	
													  NULL,			
													  NULL);				
			if(bResult == false)
				break;
		


			//Get the HID Handle Point 
			m_hdlHID =  CreateFile(m_pstrcDetail->DevicePath,
						 		   GENERIC_WRITE | GENERIC_READ,
								   FILE_SHARE_READ | FILE_SHARE_WRITE,  
								   NULL,
								   OPEN_EXISTING,
								   FILE_FLAG_OVERLAPPED,
								   NULL);

			if( INVALID_HANDLE_VALUE == m_hdlHID)
				break;


			//Get PID¡BVID
			ZeroMemory(&strcAttributes, sizeof(HIDD_ATTRIBUTES));
			strcAttributes.Size = sizeof(HIDD_ATTRIBUTES);
			bResult = HidD_GetAttributes(m_hdlHID, &strcAttributes);


			//Check PID¡BVID
			if( !( (bResult == true )                    && 
				   (strcAttributes.VendorID  == m_uiVID) && 
				   (strcAttributes.ProductID == m_uiPID)     ) )
			{
				break;
			}



			//==================Already Link Device====================================

			GetTheTopLevel();

#ifdef USE_PIPE
			if( OpenEndpoint() == false )
				break;
#endif

			//Set buffer number
			HidD_SetNumInputBuffers(m_hdlHID, 128);

			m_strPath     = m_pstrcDetail->DevicePath;
			m_bIsCatchHid = TRUE;	
			
			return true;


		}while(false);


		//Clear fail data
		if(m_pstrcDetail != NULL)
		{
			free(m_pstrcDetail);
			m_pstrcDetail = NULL;
		}

		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlHID);
		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlRdHID);
		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlWtHID);	

	}//end for


	if(m_hDeviceInfo != INVALID_HANDLE_VALUE)
	{
		SetupDiDestroyDeviceInfoList(m_hDeviceInfo);
		m_hDeviceInfo = INVALID_HANDLE_VALUE;
	}

	if( m_bIsCatchHid == false)
	{
		CloseUSB();
		return false;
	}

	return true;


#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif

}

void ClsUSBPort::GetTheTopLevel()
{	
#ifdef	TRY_DEBUG
	try{
#endif

	ZeroMemory( &m_HidCaps, sizeof(HIDP_CAPS) );

	//If user-mode applications can call HidD_GetPreparsedData, 
	//Kernel-mode drivers can use an IOCTL_HID_GET_COLLECTION_DESCRIPTOR request.
	HidD_GetPreparsedData(m_hdlHID, &m_pPreparsedData);

	//know the HID setting
	HidP_GetCaps(m_pPreparsedData, &m_HidCaps);


#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif
}


bool ClsUSBPort::OpenEndpoint()
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer OpenEndpoint\n") );
#endif

	//Close Handle
	clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlHID);

	TCHAR  DeviceName[512];
	ZeroMemory(DeviceName, sizeof(DeviceName));

	//Get the HID Handle Point
	_stprintf(DeviceName, _T("%s\\%s"), m_pstrcDetail->DevicePath, _T("PIPE01") );	 
	m_hdlWtHID =  CreateFile(DeviceName,
					 	     GENERIC_WRITE,
						     FILE_SHARE_READ | FILE_SHARE_WRITE,
						     NULL,
						     OPEN_EXISTING,
						     FILE_FLAG_OVERLAPPED,
						     NULL);


	//Get the HID Handle Point 
	_stprintf(DeviceName, _T("%s\\%s"), m_pstrcDetail->DevicePath, _T("PIPE02") );	
	m_hdlRdHID =  CreateFile(DeviceName,
					 	     GENERIC_READ,
						     FILE_SHARE_READ | FILE_SHARE_WRITE,
						     NULL,
						     OPEN_EXISTING,
						     FILE_FLAG_OVERLAPPED,
						     NULL);


	if(m_hdlWtHID == INVALID_HANDLE_VALUE  ||  m_hdlRdHID == INVALID_HANDLE_VALUE)
		return false;
	else
		return true;
}


void ClsUSBPort::CloseUSB(void)
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer CloseUSB\n") );

	try{
#endif
		m_bIsCatchHid = false;

		if( INVALID_HANDLE_VALUE != m_hdlHID)
			CancelIo(m_hdlHID);

		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlHID);
		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlRdHID);
		clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlWtHID);

		if(m_pstrcDetail != NULL)
		{
			free(m_pstrcDetail);
			m_pstrcDetail = NULL;
		}

		if(m_pPreparsedData != NULL)
		{
			HidD_FreePreparsedData(m_pPreparsedData);
			m_pPreparsedData = NULL;
		}

		if(m_hDeviceInfo != INVALID_HANDLE_VALUE)
		{
			SetupDiDestroyDeviceInfoList(m_hDeviceInfo);
			m_hDeviceInfo = INVALID_HANDLE_VALUE;
		}

		m_strPath.Empty();

#ifdef	TRY_DEBUG
	}
	catch(...)
	{
		DWORD dwError = GetLastError();
	}
#endif
}



UINT ClsUSBPort::API_AskRevCount()
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_AskRevCount\n") );
#endif


#ifdef USE_PIPE

	if(m_hdlWtHID == INVALID_HANDLE_VALUE || m_hdlRdHID == INVALID_HANDLE_VALUE)	return 0;

#else

	if(m_hdlHID == INVALID_HANDLE_VALUE)	return 0;

#endif



	return m_HidCaps.InputReportByteLength;
}



UINT ClsUSBPort::API_AskUsbWtCount()
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_AskUsbWtCount\n") );
#endif


#ifdef USE_PIPE

	if(m_hdlWtHID == INVALID_HANDLE_VALUE || m_hdlRdHID == INVALID_HANDLE_VALUE)	return 0;

#else

	if(m_hdlHID == INVALID_HANDLE_VALUE)	return 0;

#endif



	return m_HidCaps.OutputReportByteLength;
}


DEVICE_TYPE ClsUSBPort::API_AskDevType()
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_AskDevType\n") );
#endif

	return DEVICE_TYPE::USB;
}


bool ClsUSBPort::API_ClearBuffer()
{
#ifdef	TRY_DEBUG
	OutputDebugString( _T("Transfer API_AskUsbWtCount\n") );
#endif

	HANDLE hdlRd = m_hdlHID;

#ifdef USE_PIPE

	if(m_hdlWtHID == INVALID_HANDLE_VALUE || m_hdlRdHID == INVALID_HANDLE_VALUE)	return 0;
	hdlRd = m_hdlRdHID
#endif


	return HidD_FlushQueue( hdlRd ); 
}

