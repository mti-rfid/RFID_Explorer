#include "stdafx.h"
#include "Utility.h"

////Convert Binary to Hex String
//short __fastcall clsUTILITY::BinToHex(unsigned char* strBINString,
//                                      char* strHexString,
//                                      int iBinLen)
//{
//  int sRet=0;
//  int i=0;
//  int iTemp1=0;
//  int iTemp2=0;
//  unsigned char ucTemp;
//  try{
//     for (i=0;i<iBinLen;i++){
//       ucTemp=*(strBINString+i);
//       iTemp1=(ucTemp>>4) &0x0F;   //High 4 Bit
//       iTemp2=ucTemp & 0x0F;       //Low  4 Bit
//       strHexString[i*4]=HexConvert(iTemp1);
//       strHexString[i*4+1]=HexConvert(iTemp2);
//       strHexString[i*4+2]=' ';
//
//	   if( i>20 && !(i%20) )	strHexString[i*4+3]='\n';
//	   else					    strHexString[i*4+3]=' ';
//
//     }
//  }
//  catch(...){
//    sRet=-2;
//  }
//  return sRet;
//}

//Convert Binary to Hex String
short __fastcall clsUTILITY::BinToHex(TBYTE* strBINString,
                                      TBYTE* strHexString,
                                      int iBinLen)
{
	int sRet=0;
	int i=0;
	int iTemp1=0;
	int iTemp2=0;
	TBYTE ucTemp = 0;

	try
	{
		for (i=0;i<iBinLen;i++)
		{
			ucTemp=*(strBINString+i);
			iTemp1=(ucTemp>>4) &0x0F;   //High 4 Bit
			iTemp2=ucTemp & 0x0F;       //Low  4 Bit
			strHexString[i*2]=HexConvert(iTemp1);
			strHexString[i*2+1]=HexConvert(iTemp2);
		}
	}
	catch(...)
	{
		sRet=-2;
	}
	return sRet;
}

//Convert int to Hex Character
TCHAR __fastcall clsUTILITY::HexConvert(int iNum)
{
	TCHAR cTemp = 0;

	switch (iNum){
	case 0:{
	  cTemp='0';
	  break;
	}
	case 1:{
	  cTemp='1';
	  break;
	}
	case 2:{
	  cTemp='2';
	  break;
	}
	case 3:{
	  cTemp='3';
	  break;
	}
	case 4:{
	  cTemp='4';
	  break;
	}
	case 5:{
	  cTemp='5';
	  break;
	}
	case 6:{
	  cTemp='6';
	  break;
	}
	case 7:{
	  cTemp='7';
	  break;
	}
	case 8:{
	  cTemp='8';
	  break;
	}
	case 9:{
	  cTemp='9';
	  break;
	}
	case 10:{
	  cTemp='A';
	  break;
	}
	case 11:{
	  cTemp='B';
	  break;
	}
	case 12:{
	  cTemp='C';
	  break;
	}
	case 13:{
	  cTemp='D';
	  break;
	}
	case 14:{
	  cTemp='E';
	  break;
	}
	case 15:{
	  cTemp='F';
	  break;
	}
	}
	return cTemp;
}