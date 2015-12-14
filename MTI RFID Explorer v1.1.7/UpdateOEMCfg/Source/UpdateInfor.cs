using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace UpdateOEMCfgTool.Global
{

    public enum ENUM_UPDATE_TOOL_VER : uint
    {
        // Version information
        UPDATE_TOOL_MAJOR_VER   = 1,
        UPDATE_TOOL_MINOR_VER   = 0,
        UPDATE_TOOL_RELEASE_VER = 3,
    }


    public enum ENUM_ITEM_TYPE
    { 
        //Text
        TEXT_MODEL,
        TEXT_VER,
        TEXT_FILE_PATH,
        TEXT_STATUS,
        TEXT_DEVICE,
        TEXT_OEMVER,
        
        //Label
        LABEL_PERCENT_TEXT,

        //Button
        BTN_RELINK,
        BTN_BROWSER,
        BTN_UPDATE,
        BTN_UPDATE_TEXT,

        //Progress
        PROG_SET_MAX,
        PROG_ADD_VALUE,
        PROG_CLEAR,


        //Menu
        MENU_SET_COM_PORT,
        MENU_DEVICE_INTERFACE,


        //Function
        FUNC_CONNECT,
        FUNC_DISCONNECT,
    }


    public enum DEV_MODE
    { 
        AP,
        BL,
        BL_ENTER_UPDATE,
        NO
    }


   [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_HDR
    { 
        internal Int32 dbcc_size;
        internal Int32 dbcc_deviceType;
        internal Int32 dbcc_reserved;   
    }



    [StructLayout(LayoutKind.Sequential)]
    public class DEV_BROADCAST_DEVICE_INTERFACE
    { 
        internal Int32 dbcc_size;
        internal Int32 dbcc_deviceType;
        internal Int32 dbcc_reserved;
        internal Guid  dbcc_classguid;
        internal Int32 dbcc_name;        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class DEV_BROADCAST_DEVICE_INTERFACE_2
    { 
        internal Int32 dbcc_size;
        internal Int32 dbcc_deviceType;
        internal Int32 dbcc_reserved;

        [ MarshalAs( UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16 ) ]
        internal Byte[] dbcc_classguid;

        [ MarshalAs( UnmanagedType.ByValArray, SizeConst = 255 ) ]
        internal Char[] dbcc_name;     
    }
           
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class DEV_BROADCAST_PORT
    { 
        internal Int32 dbcc_size;
        internal Int32 dbcc_deviceType;
        internal Int32 dbcc_reserved;

        [ MarshalAs( UnmanagedType.ByValArray, SizeConst = 255 ) ]
        internal Char[] dbcc_name;     
    }
       

    //Delegate
    public delegate void CONTROL_ITEM( ENUM_ITEM_TYPE r_enumItem, object r_obj );



    public class ClsRegisterHidEvent
    {
        public  readonly Int32 DBT_DEVICE_ARRIVAL          = 0x8000;
        public  readonly Int32 DBT_DEVICE_REMOVE_COMPLETE  = 0x8004;
        public  readonly Int32 WM_DEVICECHANGE             = 0x219;
        public  readonly Int32 DBT_DEVTYP_DEVICE_INTERFACE = 5;

        private const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        private const Int32 DIGCF_PRESENT               = 2;
        private const Int32 DIGCF_DEVICEINTERFACE       = 0x10;

        private IntPtr m_deviceNotificationHandle = IntPtr.Zero;


 
        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification
        (
            IntPtr hRecipient,
            IntPtr NotificationFilter,
            Int32  Flags
        );
              


        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean UnregisterDeviceNotification
        (
            IntPtr r_Handle
        ); 



        [DllImport("hid.dll", SetLastError = true)]
        private static extern void HidD_GetHidGuid
        (
            ref System.Guid HidGuid
        );


         public void RegisterHidEvent
         (
             IntPtr r_HANDLE
         )
         { 

            DEV_BROADCAST_DEVICE_INTERFACE devBoardcastDeviceInterface = new DEV_BROADCAST_DEVICE_INTERFACE();
            IntPtr devBroadcastDeviceInterfacebuffer;
            
            Int32 size = 0;
            System.Guid guid = new Guid();

            size = Marshal.SizeOf(devBoardcastDeviceInterface);
            
             HidD_GetHidGuid( ref guid);

            devBoardcastDeviceInterface.dbcc_size = size;
            devBoardcastDeviceInterface.dbcc_deviceType = DBT_DEVTYP_DEVICE_INTERFACE;
            devBoardcastDeviceInterface.dbcc_reserved = 0;
            devBoardcastDeviceInterface.dbcc_classguid = guid;

            devBroadcastDeviceInterfacebuffer = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr( devBoardcastDeviceInterface, devBroadcastDeviceInterfacebuffer, true );

            m_deviceNotificationHandle = RegisterDeviceNotification
                                         (
                                              r_HANDLE,
                                              devBroadcastDeviceInterfacebuffer,
                                              DEVICE_NOTIFY_WINDOW_HANDLE
                                         );

            Marshal.FreeHGlobal(devBroadcastDeviceInterfacebuffer);

         }



         public Boolean UnRegisterHidEvent
         (

         )
         { 
             Boolean bResult = UnregisterDeviceNotification( m_deviceNotificationHandle );

             if( true == bResult)
             {
                 if(m_deviceNotificationHandle != IntPtr.Zero)
                     m_deviceNotificationHandle = IntPtr.Zero;             
             }

            return bResult; 

         }



         public Boolean IsReg
         (

         )
         { 
             return m_deviceNotificationHandle != IntPtr.Zero ? true : false;        
         }



                    
    }


}
