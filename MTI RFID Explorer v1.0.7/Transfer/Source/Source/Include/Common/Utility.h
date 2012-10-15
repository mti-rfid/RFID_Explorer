#ifndef UTILITY_H
#define UTILITY_H

//Log
#define LOG_TX_MSG  0
#define LOG_RX_MSG  1
#define MAX_DATA_LENGTH  512

struct STRC_TRANX_DATA
{
   unsigned long ul_Length;
   TBYTE         uca_Data[MAX_DATA_LENGTH];
};


enum ENUM_CLOSE_TYPE
{
	CLOSE_TYPE_HANDLE,
	CLOSE_TYPE_POINT,
};


class clsUTILITY
{
public:
	template <class T>  static void DelObj(ENUM_CLOSE_TYPE enumType, T &p);

	static short __fastcall BinToHex(TBYTE* strBINString,
                                     TBYTE* strHexString,
                                     int iBinLen);
	static TCHAR __fastcall HexConvert(int iNum);
};


template <class T>
void clsUTILITY::DelObj(ENUM_CLOSE_TYPE enumType, T &p)
{
	switch(enumType)
	{
		case CLOSE_TYPE_HANDLE:
			if(p != INVALID_HANDLE_VALUE)
			{
				CloseHandle((T)p);
				p = (T)INVALID_HANDLE_VALUE;
			}

			break;

		case CLOSE_TYPE_POINT:
			if(p != NULL)
			{
				delete p;
				p = NULL;
			}
			break;
	}
}
#endif