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
 * $Id: Singleton.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description:
 *     
 *     Generic Singleton pattern
 *     
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;



namespace RFID_Explorer
{

    namespace Patterns
    {

        /************************************************************************
         * Name:
         * 
         *   Singleton
         * 
         * Description:
         * 
         *   Basic thread safe and persistent generic Singleton implementation.
         * 
         *   Invoke via Singleton< CLASS TO WRAP >.Instance where the primary
         *   requirement is that the [CLASS TO WRAP] class either provides a
         *   zero parameter constructor or no constructor i.e. so the compiler
         *   emits a default constructor.
         * 
         *   The created Singleton will persist for as long as the current C#
         *   application domain is loaded.
         * 
         *   While safe across threads and SMP systems, it is NOT safe or set to
         *   provide direct comparability across application domains or networks.
         * 
         *   At this time only works with parameter less constructors - TODO on
         *   specifying target constructor and supplying parameters where desired
         * 
         ************************************************************************/

        public class Singleton< T > : System.Object
        {

            private class PrivateCreator
            {
                internal static readonly T INSTANCE_;

                // Under normal operation this NEVER throw an exception since the
                // generic param T is validated during compilation ?!

                static PrivateCreator()
                {                    
                    INSTANCE_ = ( T ) Activator.CreateInstance( typeof( T ) );
                }

            } // END class PrivateCreator


            protected Singleton()
            {
                // NOP
            }

            public static T Instance
            {
                get
                {
                    return PrivateCreator.INSTANCE_;
                }
            }


        } // END class Singleton< T >


    } // END namespace Patterns


} // END namespace RFID_Explorer

