/*
 *****************************************************************************
 *                                                                           *
 *                   MTI CONFIDENTIAL AND PROPRIETARY                        *
 *                                                                           *
 * This source code is the sole property of MTI, Inc.  Reproduction or       *
 * utilization of this source code in whole or in part is forbidden without  *
 * the prior written consent of MTI, Inc.                                    *
 *                                                                           *
 * (c) Copyright MTI, Inc. 2011. All rights reserved.                        *
 *                                                                           *
 *****************************************************************************
 */

/*
 *****************************************************************************
 *
 * $Id: LinkageStaticConstructor.cs,v 1.3 2009/09/03 20:23:35 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */


using System;
using System.IO;
using System.Reflection;

using System.Runtime.InteropServices;


namespace rfid
{

    public partial class Linkage
    {
        /*
         * Explicitly attempt load of native layer dlls from the current
         * assembly directory AND if that fails, look in the Native dir.
         */

        [ DllImport( "kernel32.dll" ) ]
        public static extern IntPtr LoadLibrary( String lpFileName );

        static Linkage( )
        {
            String path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );

            if (IntPtr.Zero == LoadLibrary(path + "\\Transfer.dll"))
                LoadLibrary(path + "\\..\\Native\\Transfer.dll");

        } // static Linkage( )


    } // partial class Linkage


} // namespace rfid
