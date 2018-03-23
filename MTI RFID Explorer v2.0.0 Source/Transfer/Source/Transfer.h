// Transfer.h : main header file for the Transfer DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CTransferApp
// See Transfer.cpp for the implementation of this class
//

class CTransferApp : public CWinApp
{
public:
	CTransferApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
