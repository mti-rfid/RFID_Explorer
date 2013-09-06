1. Select the device intertface to open.
	DLLMyLib UINT  WINAPI API_USB_Open(UINT uiPID, UINT uiVID);
	DLLMyLib UINT  WINAPI API_Serial_Open(UINT uiComPort, DCB &r_Dcb);

2. Clear the buffer
	DLLMyLib UINT  WINAPI API_ClearBuffer();

3. Set overlap time if you want to use overlap.
	DLLMyLib UINT  WINAPI API_SetOverlapTime(UINT r_uiWtOverlapTime, UINT r_uiRdOverlapTime);

4. If you are using USB, you can get the Input/Output Max Length(Report count)  from below.
	DLLMyLib UINT  WINAPI API_AskRevCount(UINT *r_uiRxCount);
	DLLMyLib UINT  WINAPI API_AskUsbWtCount(UINT *r_uiTxCount);

5. Send command to check whether connected or not.
	DLLMyLib UINT  WINAPI API_Write(UINT uiMode, UCHAR *cData, UINT iLength);

6. Using USB. You don't need to use this API. It always return Max Input size.
   Using RS232. You can use this api to know how much data are in your buffer.	
   	DLLMyLib UINT  WINAPI API_AskRevCount(UINT *r_uiRxCount);

7. Receive data
	DLLMyLib UINT  WINAPI API_Read(UINT uiMode, UCHAR *cData, UINT *iLength);

8. Close the communication.
	DLLMyLib UINT  WINAPI API_Close();


Note:

1. MTI RFID ME series (RU888....)   Important!!
   Write should use REPORT mode.
   Read should use  INTERRUPT_WITH_OVERLAP or INTERRUPT_WITHOUT_OVERLAP mode.


2. MTI R1000/R2000 chip (RU861¡BRU859.....)  Important!!
   Write should use INTERRUPT_WITH_OVERLAP or INTERRUPT_WITHOUT_OVERLAP mode.
   Read should use INTERRUPT_WITH_OVERLAP or INTERRUPT_WITHOUT_OVERLAP mode.
	
3. If you want detail Path to do something like detect the USB remove, you can use this api the get the total path.
	DLLMyLib char* WINAPI API_AskDevPath( );

4. You can modify Output Directory by yourself.