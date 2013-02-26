#ifndef CLS_SERIAL
#define CLS_SERIAL

#include "DeviceBase.h"

class ClsSerial : public virtual ClsDeviceBase
{
private:
	HANDLE	    m_hdlCom;
	HANDLE      m_hdlRdEvent;
	HANDLE      m_hdlWtEvent;
	OVERLAPPED  m_OverlapRd;
	OVERLAPPED  m_OverlapWt;
	DCB		    m_DCB;
	COMSTAT		m_COMSTAT;


public:
	ClsSerial();
	~ClsSerial();

	//Interface
	bool		API_Open(UINT uiComPort, DCB &r_Dcb);
	RESULT		API_Write(UINT uiMode, UCHAR *cData, UINT iLength);
	RESULT		API_Read(UINT uiMode, UCHAR *cData, UINT *iLength);
	void		API_Close();
	UINT		API_AskRevCount();
	DEVICE_TYPE API_AskDevType();
	bool		API_ClearBuffer();
};

#endif