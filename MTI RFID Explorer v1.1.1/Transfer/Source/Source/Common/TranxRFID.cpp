#include "stdafx.h"
#include "TranxRFID.h"


//#define _OPEN_SAS_COM_TRANS_DATA_LOG
#define _OPEN_SAS_DEBUG_LOG_TO_FILE

#ifdef _OPEN_SAS_DEBUG_LOG_TO_FILE
#include <stdio.h>

void gfLog( LPCTSTR pData )
{
	LPCTSTR g_pDebugFile = _T("DebugLog.txt") ;

	int iStrlen = _tcslen(pData);
	SYSTEMTIME Now;
	GetSystemTime(&Now);

	TCHAR sTemp[512];
	ZeroMemory(sTemp, sizeof(sTemp));
	_stprintf(sTemp, _T("%02d:02d:%02d:%03d "), Now.wHour,Now.wMinute,Now.wSecond,Now.wMilliseconds);
	_tcscat(sTemp,pData);

	int iLength = _tcslen(sTemp);
    HANDLE hFile = CreateFile( g_pDebugFile, GENERIC_WRITE, FILE_SHARE_READ,
            NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL ) ;

    SetFilePointer( hFile, 0, NULL, FILE_END ) ;
    
	DWORD dwWritten ;
    WriteFile( hFile, sTemp, iLength, &dwWritten, NULL ) ;
    
	TCHAR sEnter[ 2 ] = { _T('\r'), _T('\n') } ;
    WriteFile( hFile, sEnter, 2, &dwWritten, NULL ) ;
    
	CloseHandle( hFile ) ;

}
#endif


clsTRANX_RFID::clsTRANX_RFID()
{
	m_ucRxStep       = 0;
    m_ucRxLength     = 0;
	m_ucRxCnt        = 0;
	m_ucRxCmd        = 0;
	m_ucRxStatus     = 0;
	m_ucTxCnt	     = 0;
	m_TimeCnt        = 0;
	m_usOemAddr      = 0;
	m_ucOemData      = 0;
	m_uiCurFileCnt	 = 0;
	m_uiTotalFileCnt = 0;
	m_bIsOpen	     = false;
	m_clsUSBPort     = NULL;
	m_enumTxCmd[0]   = CMD_NOTHING;
	m_hdlImageFile   = INVALID_HANDLE_VALUE;

	ZeroMemory( m_ucTxBuf, sizeof(m_ucTxBuf) );
	ZeroMemory( m_ucRxBuf, sizeof(m_ucTxBuf) );

	m_clsUSBPort = new ClsUSBPort;
}


clsTRANX_RFID::~clsTRANX_RFID()
{
	clsUTILITY::DelObj(CLOSE_TYPE_POINT, m_clsUSBPort);
	clsUTILITY::DelObj(CLOSE_TYPE_HANDLE, m_hdlImageFile);

}

bool clsTRANX_RFID::API_StartComm(void)
{
	ResetFlow();
	m_bIsOpen = m_clsUSBPort->API_Open(PRODUCT_ID, VENDOR_ID);

	return m_bIsOpen;
}

void clsTRANX_RFID::API_CloseComm(void)
{
	ResetFlow();
	m_bIsOpen = false;
	
	m_clsUSBPort->API_Close();
}

//=================================PACKET=======================================//
bool clsTRANX_RFID::API_IsOpen(void)
{
	return m_bIsOpen;
}

bool clsTRANX_RFID::API_IsBusy(void)
{
	return m_enumTxCmd[0]==CMD_NOTHING?false:true;
}


bool clsTRANX_RFID::API_QueryOneCmd(UINT index)
{
	if(m_enumTxCmd[0] != CMD_NOTHING || !m_bIsOpen)
		return false;

	bool bResult = false;

	ZeroMemory(m_enumTxCmd, sizeof(m_enumTxCmd));
	ZeroMemory(m_ucTxBuf,   sizeof(m_ucTxBuf));


	m_enumTxCmd[0] = CMD_SEND_ONE_CMD_BY_USER;
	m_enumTxCmd[1] = CMD_TRANX_ALL_OK;


	switch(index)
	{	
		case CMD_VERSION_B:
			bResult = Tx_bVersion();
			break;

		case CMD_ENTER_UPDATE_FW_MODE_B:
			bResult = Tx_bEnterUpdateFwMode();
			break;

		case CMD_EXIT_UPDAYE_FW_MODE_B:
			bResult = Tx_bExitUpdateFwMode();
			if(bResult)		Sleep(4000);
			break;

		case CMD_BEGIN_UPDATE_B:
			bResult = Tx_bBeginUpdate();
			if(bResult)		Sleep(3000);
			break;

		case CMD_WRITE_IMAGE_B:
			bResult = Tx_bWriteImage();
			break;

		case CMD_SET_OEM_CONFIG_B:
			bResult = Tx_bSetOemConfig();
			break;

		default:
			return false;
	}

	if(bResult == false)
	{
	
		ResetFlow();   
		return false;
	}

	return true;
}


bool clsTRANX_RFID::API_QueryOneCmd(CString cstrData)
{
	if( !m_bIsOpen                      || 
		m_enumTxCmd[0] != CMD_NOTHING   || 
		cstrData.IsEmpty() == true      ||
		cstrData.GetLength()>USB_LENGTH    )
	{
		return false;
	}

	ZeroMemory(m_enumTxCmd, sizeof(m_enumTxCmd));
	ZeroMemory(m_ucTxBuf,   sizeof(m_ucTxBuf));


	m_enumTxCmd[0] = CMD_SEND_ONE_CMD_BY_USER;
	m_enumTxCmd[1] = CMD_TRANX_ALL_OK;


	TCHAR *pch = NULL;
	TCHAR  tmpData[USB_LENGTH];
	ZeroMemory(tmpData, sizeof(tmpData));
	_tcscpy( tmpData, cstrData.GetString() );

//Push data============================
	//Cmd
	pch = _tcstok( tmpData, _T(" ") );
	if(pch == NULL)	  return false;
	
	_stscanf(pch,  _T(" %x"), &m_ucTxBuf[0]);

	pch = _tcstok (NULL, _T(" ") );
	for(int i = 1; pch != NULL; i++)
	{
		_stscanf(pch, _T(" %x"), &m_ucTxBuf[i]);
		pch = _tcstok (NULL, _T(" ") );
	}  
//=====================================

	if(WriteData(m_ucTxBuf))
	{
		switch(m_ucTxBuf[0])
		{
			case CMD_EXIT_UPDAYE_FW_MODE_B:
				Sleep(4000);
			break;

			case CMD_BEGIN_UPDATE_B:
				Sleep(3000);
			break;
		}
		return true;
	}
	else
	{
	
		ResetFlow();   
		return false;
	}
}




bool clsTRANX_RFID::API_UpdateFw(HANDLE hdlImageFile, LARGE_INTEGER strcLargeInt, bool bAllProgress)
{
	if(m_enumTxCmd[0] != CMD_NOTHING || strcLargeInt.LowPart == 0 || !m_bIsOpen)
		return false;

	//Calculation Packet count
	m_uiTotalFileCnt =  (strcLargeInt.LowPart / PER_PACKET_UPDATE_IMAGE) +		 
						((strcLargeInt.LowPart % PER_PACKET_UPDATE_IMAGE)?1:0 );
	m_hdlImageFile = hdlImageFile;
	m_uiCurFileCnt = 0;

	if(bAllProgress == true)
	{
		m_enumTxCmd[0] = CMD_VERSION_B;
		m_enumTxCmd[1] = CMD_BEGIN_UPDATE_B;
		m_enumTxCmd[2] = CMD_WRITE_IMAGE_B;
		m_enumTxCmd[3] = CMD_EXIT_UPDAYE_FW_MODE_B;
		m_enumTxCmd[4] = CMD_TRANX_ALL_OK;
	}
	else
	{
		m_enumTxCmd[0] = CMD_WRITE_IMAGE_B;
		m_enumTxCmd[1] = CMD_TRANX_ALL_OK;
	}

	SendCmd();
	return true;
}


bool clsTRANX_RFID::API_bSetOemConfig(TBYTE ucAddr, TBYTE ucData)
{
	if(m_enumTxCmd[0] != CMD_NOTHING || !m_bIsOpen)
		return false;

	m_usOemAddr = ucAddr;
	m_ucOemData = ucData;

	m_enumTxCmd[0] = CMD_ENTER_UPDATE_FW_MODE_B;
	m_enumTxCmd[1] = CMD_SET_OEM_CONFIG_B;
	m_enumTxCmd[2] = CMD_EXIT_UPDAYE_FW_MODE_B;
	m_enumTxCmd[3] = CMD_TRANX_ALL_OK;

	SendCmd();
	return true;
}

bool clsTRANX_RFID::API_bVersion()
{
	if(m_enumTxCmd[0] != CMD_NOTHING || !m_bIsOpen)
		return false;

	m_enumTxCmd[0] = CMD_VERSION_B;
	m_enumTxCmd[1] = CMD_TRANX_ALL_OK;

	SendCmd();
	return true;
}

CString clsTRANX_RFID::API_AskVersion(void)
{
	return  m_cstrVersion;
}

bool clsTRANX_RFID::API_SetRegion(ENUM_REGION enumType)
{
	if(m_enumTxCmd[0] != CMD_NOTHING || !m_bIsOpen)
		return false;

	m_enumTxCmd[0] = CMD_ENTER_UPDATE_FW_MODE_B;
	m_enumTxCmd[1] = CMD_TRANX_ALL_OK;

	SendCmd();
	return true;
}

TCHAR clsTRANX_RFID::API_AskCmdStatus(void)
{
	return m_ucRxStatus;
}

UINT clsTRANX_RFID::API_AskProgressBarStep(void)
{
	if(m_uiTotalFileCnt == 0 || m_uiCurFileCnt ==0)
		return 0;

	UINT uiPercent = (m_uiCurFileCnt*100)/m_uiTotalFileCnt;
	return uiPercent>100?100:uiPercent;
}

//==================CMD===============================
bool  clsTRANX_RFID::Tx_bVersion()
{
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf));

	m_ucTxBuf[0] = CMD_VERSION_B;
	m_ucTxBuf[1] = 0x03;
	m_ucTxBuf[2] = 0x00;

#ifdef _DEBUG
	OutputDebugString( _T("Tx CMD_VERSION_B\n") );
#endif

	return WriteData(m_ucTxBuf);
}

bool clsTRANX_RFID::Tx_bEnterUpdateFwMode()
{
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf));

	m_ucTxBuf[0] = CMD_ENTER_UPDATE_FW_MODE_B;
	m_ucTxBuf[1] = 0x02;

#ifdef _DEBUG
	OutputDebugString(_T("Tx CMD_ENTER_UPDATE_FW_MODE_B\n") );
#endif

	return WriteData(m_ucTxBuf);		
}

bool clsTRANX_RFID::Tx_bExitUpdateFwMode()
{
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf));

	m_ucTxBuf[0] = CMD_EXIT_UPDAYE_FW_MODE_B;
	m_ucTxBuf[1] = 0x2;

#ifdef _DEBUG
	OutputDebugString( _T("Tx CMD_EXIT_UPDAYE_FW_MODE_B\n") );
#endif
	
	return WriteData(m_ucTxBuf);
}


bool clsTRANX_RFID::Tx_bBeginUpdate()
{
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf));

	m_ucTxBuf[0] = CMD_BEGIN_UPDATE_B;
	m_ucTxBuf[1] = 0x02;

#ifdef _DEBUG
	OutputDebugString( _T("Tx CMD_BEGIN_UPDATE_B\n") );
#endif

	return WriteData(m_ucTxBuf);
}


bool clsTRANX_RFID::Tx_bWriteImage(void)
{
	if( (m_uiTotalFileCnt == 0)					|| 
		(m_uiCurFileCnt>m_uiTotalFileCnt)		||
		(m_hdlImageFile == INVALID_HANDLE_VALUE)   )
		return false;


	DWORD dwRead = 0;
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf) );


#ifdef _UNICODE

	UCHAR tmpData[PER_PACKET_UPDATE_IMAGE];
	ZeroMemory(tmpData, sizeof(tmpData));

	ReadFile(m_hdlImageFile, tmpData, PER_PACKET_UPDATE_IMAGE, &dwRead, NULL);

	m_ucTxBuf[0] = CMD_WRITE_IMAGE_B;
	m_ucTxBuf[1] = PER_PACKET_UPDATE_IMAGE + 2;
 
	for(int i=0; i<USB_LENGTH; i++)   m_ucTxBuf[i+2] = tmpData[i];

#else

	m_ucTxBuf[0] = CMD_WRITE_IMAGE_B;
	m_ucTxBuf[1] = PER_PACKET_UPDATE_IMAGE + 2;

	ReadFile(m_hdlImageFile, m_ucTxBuf+2, PER_PACKET_UPDATE_IMAGE, &dwRead, NULL);

#endif


	if(dwRead == 0)
	{
		#ifdef _DEBUG
			OutputDebugString( _T("Tx CMD_WRITE_IMAGE_B Data is 0\n") );
		#endif

		return false;
	}


#ifdef _DEBUG
	OutputDebugString( _T("Tx CMD_WRITE_IMAGE_B\n") );
#endif

	return WriteData(m_ucTxBuf);
}


bool clsTRANX_RFID::Tx_bSetOemConfig(void)
{
	ZeroMemory(m_ucTxBuf, sizeof(m_ucTxBuf));

	m_ucTxBuf[0] = CMD_SET_OEM_CONFIG_B;
	m_ucTxBuf[1] = 0x05;
	memcpy(m_ucTxBuf+2, &m_usOemAddr,sizeof(USHORT));
	m_ucTxBuf[4] = m_ucOemData;


#ifdef _DEBUG
	OutputDebugString( _T("Tx CMD_SET_OEM_CONFIG_B\n") );
#endif

	return WriteData(m_ucTxBuf);
}

//==============================================
//enuRdCmd:  if return Cmd, it means that receive Packet and handle.
//           if return CMD_NOTHING, it means idle or receiveing data
ENUM_CMD WINAPI clsTRANX_RFID::API_Regular(int iTime)
{
	TBYTE	  RxChar = 0;
	//UCHAR	  ucRxBuf[USB_LENGTH];
	UCHAR	  *ucRxBuf = new UCHAR[USB_LENGTH];
	ENUM_CMD  enuRdCmd = CMD_NOTHING;

	ZeroMemory(ucRxBuf, sizeof(ucRxBuf));

	if(m_enumTxCmd[0] == CMD_NOTHING || !m_bIsOpen)
	{
		return CMD_NOTHING;
	}

	if( m_TimeCnt >= 2000)
	{
		ResetFlow();
		m_bIsOpen = false;
		return CMD_TIME_OUT;	
	}
	else if( (m_TimeCnt>=1000) && !(m_TimeCnt % 1000) )
	{
		m_TimeCnt+=iTime;
		WriteData(m_ucTxBuf);
		return CMD_NOTHING;
	}	
	else
		m_TimeCnt+=iTime;


	UINT uiRead = m_clsUSBPort->API_Read(ucRxBuf);
	for(UINT i=0 ; i< uiRead; i++)
	{
		if(m_ucRxCnt>= USB_LENGTH || m_ucRxCnt>=uiRead)		
			m_ucRxCnt = 0;
		if(i>=USB_LENGTH)
			break;


		RxChar = m_ucRxBuf[m_ucRxCnt] = ucRxBuf[i];
		switch(m_ucRxStep)
		{
			case STEP_RESPOEES_CMD: 
				if(m_ucTxBuf[0]+1 == RxChar )
				{
					m_ucRxStep++;
					m_ucRxCnt = 1;	//Receive 1 byte	
					m_ucRxCmd = RxChar;
				}
				else
				{
					m_ucRxStep   = 0;
					m_ucRxCnt    = 0;										
				}
				m_ucRxStatus = 0;
				break;

			case STEP_LENGTH:
				m_ucRxLength = RxChar;
				m_ucRxCnt++;

				(m_ucRxLength<=USB_LENGTH)?(m_ucRxStep++):(m_ucRxStep = 0);
				break;

			case STEP_DATA:
				
				m_ucRxCnt++;

				//Check Data, avoid dead lock or buffer overfolw
				if(m_ucRxCnt>USB_LENGTH)
				{
					m_ucRxStep = 0;
					break;
				}

				//Check packet, some cmd don't have data.
				if( m_ucRxCnt == m_ucRxLength)  //Receive Data Done
				{
					//Set cmd response status 
					m_ucRxStatus = m_ucRxBuf[2];
					//Set the Packet Cmd to let Usbcontrol Dialog know what data was received.
					enuRdCmd = (ENUM_CMD)m_ucRxCmd;

#ifdef _OPEN_SAS_COM_TRANS_DATA_LOG
LogMsg(m_ucRxBuf, m_ucRxBuf[1], LOG_RX_MSG);
#endif
					CheckPacket();


					//The length of cmd between AP and BL Mode is different.
					//Avoid AP mode to send too much leng. 
					clsUTILITY::DelObj(CLOSE_TYPE_POINT, ucRxBuf);
					return enuRdCmd;
				}
				break;

			default:
				m_ucRxStep = 0;
		}//Out switch
	}//for

	clsUTILITY::DelObj(CLOSE_TYPE_POINT, ucRxBuf);
	return enuRdCmd;
}


void clsTRANX_RFID::CheckPacket(void)
{
	//Reset Step Data
	m_TimeCnt  = 0;
	m_ucRxCnt  = 0;
	m_ucRxStep = 0;

	//Analyze the result of Packet Cmd
	AnalyzePacket();

	SendCmd();
	ZeroMemory(m_ucRxBuf, sizeof(m_ucRxBuf));
}

void clsTRANX_RFID::AnalyzePacket(void)
{
	switch(m_ucRxCmd)
	{
		case CMD_VERSION__BR:
			if(m_ucRxStatus == STATUS_OK)
				m_cstrVersion.SetString((TCHAR *)m_ucRxBuf+3, 45);
			
			ChangeCmd(CHANGE_CMD_TO_NEXT);

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_VERSION__BR\n") );
#endif
			break;

		case CMD_ENTER_UPDATE_FW_MODE_BR:
			ChangeCmd(CHANGE_CMD_TO_NEXT);

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_ENTER_UPDATE_FW_MODE_BR\n") );
#endif
			break;	


		case CMD_EXIT_UPDAYE_FW_MODE_BR:
			m_uiCurFileCnt   = 0;
			m_uiTotalFileCnt = 0;
			ChangeCmd(CHANGE_CMD_TO_NEXT);

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_EXIT_UPDAYE_FW_MODE_BR\n") );
#endif
			break;	


		case CMD_BEGIN_UPDATE_BR:
			ChangeCmd(CHANGE_CMD_TO_NEXT);

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_BEGIN_UPDATE_BR\n") );
#endif
			break;

		case CMD_WRITE_IMAGE_BR:
			HandleWriteImageResult();

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_WRITE_IMAGE_BR\n") );
#endif
			break;

		case CMD_SET_OEM_CONFIG_BR:
			//Reset tmp Data
			m_usOemAddr    = 0;
			m_ucOemData    = 0;
			ChangeCmd(CHANGE_CMD_TO_NEXT);

#ifdef _DEBUG
	OutputDebugString( _T("RX CMD_SET_OEM_CONFIG_BR\n") );
#endif
			break;

		default :
			ChangeCmd(CHANGE_CMD_TO_NEXT);
			m_ucRxStep = 0;
	}//switch
}



void clsTRANX_RFID::HandleWriteImageResult(void)
{
	m_uiCurFileCnt++;

	switch(m_ucRxStatus)
	{
		case STATUS_OK:
			//Transfer Completely
			if(m_uiCurFileCnt == m_uiTotalFileCnt)
			{
				m_hdlImageFile = INVALID_HANDLE_VALUE;
				ChangeCmd(CHANGE_CMD_TO_NEXT);
			}
			break;

		case STATUS_ERROR_OVER_RANGE:
			m_uiCurFileCnt   = 0;
			m_uiTotalFileCnt = 0;
			m_hdlImageFile   = INVALID_HANDLE_VALUE;
			ChangeCmd(CHANGE_CMD_TO_NEXT);
			break;

		case STATUS_ERROR_INVALID_DATA_LENGTH:
		case STATUS_ERROR_INVALID_PARAMETER:
		case STATUS_ERROR_WRONG_PROCEDURE:
		case STATUS_ERROR_SYS_MODULE_FAILURE:
			ChangeCmd(CHANGE_CMD_TO_NEXT);
			break;

		default:
			//if Error,skip "write image" step. 
			ResetFlow();

			break;
	}
}


void clsTRANX_RFID::SendCmd(void)
{
	switch(m_enumTxCmd[m_ucTxCnt])
	{
		case CMD_VERSION_B:
			Tx_bVersion();
			break;

		case CMD_ENTER_UPDATE_FW_MODE_B:
			Tx_bEnterUpdateFwMode();
			break;

		case CMD_EXIT_UPDAYE_FW_MODE_B:
			Tx_bExitUpdateFwMode();
			Sleep(4000);
			break;

		case CMD_BEGIN_UPDATE_B:
			Tx_bBeginUpdate();
			Sleep(3000);
			break;

		case CMD_WRITE_IMAGE_B:
			Tx_bWriteImage();
			break;

		case CMD_SET_OEM_CONFIG_B:
			Tx_bSetOemConfig();
			break;

		case CMD_NOTHING:
		case CMD_TRANX_ALL_OK:
			m_enumTxCmd[0] = CMD_NOTHING;
			m_ucTxCnt = 0;
			break;
	}
}

void clsTRANX_RFID::ResetFlow()
{
	m_ucTxCnt        = 0;
	m_ucRxStep       = 0;
	m_ucRxCnt        = 0;
	m_ucRxStatus     = 0;
	m_TimeCnt        = 0;
	m_uiCurFileCnt   = 0;
	m_uiTotalFileCnt = 0;
	m_hdlImageFile   = INVALID_HANDLE_VALUE;
	m_cstrVersion.Empty();

	ZeroMemory(m_enumTxCmd, sizeof(m_enumTxCmd));
	ZeroMemory(m_ucRxBuf,   sizeof(m_ucRxBuf));
}

bool clsTRANX_RFID::WriteData(TBYTE *ucTxBuf)
{
#ifdef _OPEN_SAS_COM_TRANS_DATA_LOG
LogMsg(ucTxBuf, m_ucTxBuf[1], LOG_TX_MSG);
#endif


#ifdef _UNICODE
	UCHAR tmpData[USB_LENGTH];
	ZeroMemory(tmpData, sizeof(tmpData));	
	for(int i=0; i<USB_LENGTH; i++)   tmpData[i] = *(ucTxBuf+i) & 0xFF;

	return m_clsUSBPort->API_Write(tmpData);

#else

	return m_clsUSBPort->API_Write(m_ucTxBuf);

#endif

}

void clsTRANX_RFID::LogMsg(TBYTE *ucData, UINT ucLength, TBYTE uc_Flag)
{
	TBYTE  uc_Log[512];

    ZeroMemory(uc_Log, sizeof(uc_Log));
    clsUTILITY::BinToHex(ucData, uc_Log, ucLength);

	TCHAR sTemp[512];
    ZeroMemory(sTemp, sizeof(sTemp));

    if (uc_Flag == LOG_TX_MSG)
    {       
		 _stprintf(sTemp, _T("  TX => %s ") ,uc_Log);
		 gfLog(sTemp);

    }
    else
    {     
		_stprintf(sTemp, _T("  RX => %s ") ,uc_Log);
		gfLog(sTemp);
    }

}