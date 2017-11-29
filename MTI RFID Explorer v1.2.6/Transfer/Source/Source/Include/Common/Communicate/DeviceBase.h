#ifndef _DEVICE_BASE
#define _DEVICE_BASE

#include "stdafx.h"

class ClsDeviceBase
{
public:
	ClsDeviceBase();
	virtual ~ClsDeviceBase() {}

	virtual bool		API_ClearBuffer() = 0;
	virtual RESULT		API_Write(UINT uiMode, UCHAR *cData, UINT iLength) = 0;
	virtual RESULT		API_Read(UINT uiMode, UCHAR *cData, UINT *iLength) = 0;
	virtual void		API_Close() = 0;
	virtual UINT		API_AskRevCount() = 0;
	virtual DEVICE_TYPE API_AskDevType() = 0;
	virtual void		API_SetOverlapTime(UINT r_uiWtOverlapTime, UINT r_uiRdOverlapTime);

	UINT uiWtOverlapTime;
	UINT uiRdOverlapTime;

	static CString m_strPath;

};

#endif