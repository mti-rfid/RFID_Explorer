#ifndef TRANS_RFID_H
#define TRANS_RFID_H

#include "USBPort.h"
#include "Utility.h"


//Rx Step
#define STEP_RESPOEES_CMD   0
#define STEP_LENGTH         1  
#define STEP_DATA           2  

//Product Information
#define VENDOR_ID		0x1325
#define PRODUCT_ID		0xC029

//RFID STATUS
#define STATUS_OK							0x00
#define STATUS_ERROR_INVALID_DATA_LENGTH    0x0E
#define STATUS_ERROR_INVALID_PARAMETER      0x0F
#define STATUS_ERROR_WRONG_PROCEDURE        0xE0
#define STATUS_ERROR_OVER_RANGE             0xE1
#define STATUS_ERROR_SYS_MODULE_FAILURE     0xFF

//Update Image Size per pagket
#define PER_PACKET_UPDATE_IMAGE    32

//Change Cmd
#define CHANGE_CMD_TO_BRFORE	   0
#define CHANGE_CMD_TO_NEXT	   	   1


enum ENUM_REGION
{
	REGION_US,
	REGION_EU,
	REGION_TW,
	REGION_CN,
	REGION_KR,
	REGION_AT,
	REGION_NZ,
	REGION_EU2,
};

enum ENUM_CMD
{
	CMD_NOTHING,
	CMD_VERSION_B = 0x10,
	CMD_VERSION__BR,			   
	CMD_ENTER_UPDATE_FW_MODE_B = 0xD0,
	CMD_ENTER_UPDATE_FW_MODE_BR,
	CMD_EXIT_UPDAYE_FW_MODE_B,
	CMD_EXIT_UPDAYE_FW_MODE_BR,
	CMD_BEGIN_UPDATE_B,
	CMD_BEGIN_UPDATE_BR,
	CMD_WRITE_IMAGE_B,
	CMD_WRITE_IMAGE_BR,
	CMD_SET_OEM_CONFIG_B,
	CMD_SET_OEM_CONFIG_BR,

	//====Define myself============================
	CMD_SEND_ONE_CMD_BY_USER,  //User
	CMD_TIME_OUT,
	CMD_TRANX_ALL_OK,
};


class clsTRANX_RFID
{
public:
	clsTRANX_RFID();
	~clsTRANX_RFID();

	bool API_StartComm(void);
	void API_CloseComm(void);
	bool API_IsOpen(void);
	bool API_IsBusy();
	bool API_QueryOneCmd(UINT index);
	bool API_QueryOneCmd(CString cstrData);
	bool API_SetRegion(ENUM_REGION enumType);
	bool API_UpdateFw(HANDLE hdlImageFile, LARGE_INTEGER strcLargeInt, bool bAllProgress = true);
	bool API_bSetOemConfig(TBYTE ucAddr, TBYTE ucData);
	bool API_bVersion();
	CString  API_AskVersion(void);
	TCHAR    API_AskCmdStatus(void);
	UINT     API_AskProgressBarStep(void);
	ENUM_CMD WINAPI API_Regular(int iTime);


private:
	ClsUSBPort *m_clsUSBPort;

	//Transfer
	ENUM_CMD  m_enumTxCmd[10];
	TBYTE 	  m_ucTxCnt;
	TBYTE     m_ucTxBuf[USB_LENGTH];
	TBYTE     m_ucRxBuf[USB_LENGTH];
	TBYTE 	  m_ucRxStep;
	TBYTE	  m_ucRxLength;
	TBYTE	  m_ucRxCnt;
	TBYTE     m_ucRxCmd;
	TBYTE     m_ucRxStatus;
	UINT	  m_TimeCnt;
	bool	  m_bIsOpen;
	//Data
	USHORT	  m_usOemAddr;
	TBYTE	  m_ucOemData;
	//Version Data
	CString   m_cstrVersion;
	UINT      m_uiCurFileCnt;
	UINT      m_uiTotalFileCnt;
	HANDLE    m_hdlImageFile;

	void SendCmd(void);
	bool Tx_bVersion();
	bool Tx_bEnterUpdateFwMode(void);
	bool Tx_bExitUpdateFwMode(void);
	bool Tx_bBeginUpdate();
	bool Tx_bWriteImage(void);
	bool Tx_bSetOemConfig(void);
	void ResetFlow(void);
	void CheckPacket(void);
	void AnalyzePacket(void);
	void HandleWriteImageResult(void);
	void LogMsg(TBYTE *ucData, UINT ucLength, TBYTE uc_Flag);
	bool WriteData(TBYTE *ucTxBuf);

	inline void ChangeCmd(bool bStep){(bStep==CHANGE_CMD_TO_NEXT)?m_ucTxCnt++:m_ucTxCnt--;};

};

#endif