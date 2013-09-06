#include "stdafx.h"
#include "DeviceBase.h"


ClsDeviceBase::ClsDeviceBase()
{
	uiWtOverlapTime = WAIT_TIME;
	uiRdOverlapTime = WAIT_TIME;
}



void ClsDeviceBase::API_SetOverlapTime(UINT r_uiWtOverlapTime, UINT r_uiRdOverlapTime)
{
	uiWtOverlapTime = 
		(r_uiWtOverlapTime == 0) ? WAIT_TIME : r_uiWtOverlapTime;

	uiRdOverlapTime = 
		(r_uiRdOverlapTime == 0) ? WAIT_TIME : r_uiRdOverlapTime;
}