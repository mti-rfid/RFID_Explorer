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
 * $Id: EnumDataSource.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description:
 *     
 *     Wrapper class(es) for value struct enums to provide definition, display and
 *     lookup of held constants by direct or user specified ( via class extension )
 *     naming conventions.
 *     
 *     As implementing can be very verbose, its probably best to define a type
 *     using a statement at the start of a definition(s) file e.g. :
 *     
 *     using ASSIGNED_NAME = RFID_Explorer.EnumDataSource
 *     <
 *     rfid.Constants.SingulationAlgorithm_18K6C,
 *     RFID_Explorer.LowerCaseNameGenerator
 *     <
 *     rfid.Constants.SingulationAlgorithm_18K6C
 *     >
 *     >;
 *     
 *
 *****************************************************************************
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace RFID_Explorer
{

    /**********************************************************************
     * Name:
     * 
     *    NameGenerator< T >
     * 
     * Description:
     * 
     *    Simple interface that can be implemented by users to generate a
     *    string display name for an object of the specified type using
     *    implementation rules.
     * 
     *********************************************************************/

    public interface INameGenerator< T >
    {

        String displayName( T t );

    } // END interface NameGenerator< T >




    /**********************************************************************
     * Name:
     * 
     *    DefaultNameGenerator< T >
     * 
     * Description:
     * 
     *    Implementation of NameGenerator for use when the normal return
     *    from the object ToString( ) method is acceptable w/o manipulation
     * 
     *********************************************************************/

    public class DefaultNameGenerator< T > 
        : 
        INameGenerator< T >
    {
        public DefaultNameGenerator( )
        {
            // NOP
        }

        public String displayName( T t )
        {
            return t.ToString();
        }

    } // END class DefaultNameGenerator< T >




    /**********************************************************************
     * Name:
     * 
     *    LowerCaseNameGenerator< T >
     * 
     * Description:
     * 
     *    Implementation of NameGenerator for use when the normal return
     *    from the object ToString( ) method is converted to all lc chars.
     * 
     *********************************************************************/

    public class LowerCaseNameGenerator< T > 
        : 
        INameGenerator< T >
    {
        public LowerCaseNameGenerator( )
        {
            // NOP
        }

        public String displayName( T t )
        {
            return t.ToString( ).ToLower( );
        }

    } // END class LowerCaseNameGenerator< T >



    /**********************************************************************
     * Name:
     * 
     *    UpperCaseNameGenerator< T >
     * 
     * Description:
     * 
     *    Implementation of NameGenerator for use when the normal return
     *    from the object ToString( ) method is converted to all uc chars.
     * 
     *********************************************************************/

    public class UpperCaseNameGenerator< T > 
        : 
        INameGenerator< T >
    {
        public UpperCaseNameGenerator( )
        {
            // NOP
        }

        public String displayName( T t )
        {
            return t.ToString( ).ToUpper( );
        }

    } // END class UpperCaseNameGenerator< T >




    /**********************************************************************
     * Name:
     * 
     *   Item
     * 
     * Description:
     * 
     *   Simple wrapper for enums to provide default and display name props
     *   where no naming scheme is specified and the return values are
     *   always identical to a simple ToString() call.
     * 
     *********************************************************************/

    public class Item< T > : Object
     where T : struct
    {

        internal readonly T t;
        
        internal readonly String defaultName;
        internal readonly String displayName;


        internal Item()
        {
            // NOP - should NEVER be called
        }

        public Item(T t)
        {
            this.t = t;
            
            this.defaultName = this.displayName = this.t.ToString();
        }

        public Item(T t, String displayName)
        {
            this.t = t;

            this.defaultName = this.t.ToString();
            this.displayName = displayName;
        }

        // DefaultName property to use via [COMPONENT].DisplayMember property

        public String DefaultName
        {
            get
            {
                return this.defaultName;
            }
        }

        // DisplayName property to use via [COMPONENT].DisplayMember property

        public String DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        // Held property provides access ( ro ) to internally held obj

        public T Held
        {
            get
            {
                return this.t;
            }
        }


    } // END class Item< T >



    public class EnumDataSource< T, N > : List< Item< T > >
        where T : struct
        where N : INameGenerator< T >, new()

    {

        /*********************************************************************
         * Description:
         * 
         * Base constructor which will iterate through the entire enumeration,
         * wrap the components in Item objects and expose naming properties
         * along with lookup via custom naming.
         * 
         * TODO:
         * 
         * Currently this excludes any enum field named 'unknown'.
         * 
         * Perhaps a better scheme would be to include these and return them
         * when a lookup by display | default name fails.  
         * 
         ********************************************************************/

        public EnumDataSource( )
        {
            INameGenerator< T > n = new N();

            foreach ( T t in Enum.GetValues( typeof( T ) ) )
            {               
                this.Add( new Item< T >( t, n.displayName( t ) ) );
            }
        }


        /*********************************************************************
         * Description:
         * 
         * Returns the Item with the given default name or null if not found
         ********************************************************************/

        public Item< T > GetByDefaultName( String name )
        {
            foreach ( Item< T > item in this )
            {
                if ( item.DefaultName == name )
                {
                    return item;
                }
            }

            return null;
        }


        /*********************************************************************
         * Description:
         * 
         * Returns the Item with the given display name or null if not found
         ********************************************************************/

        public Item< T > GetByDisplayName( String name )
        {
            foreach ( Item< T > item in this )
            {
                if ( item.DisplayName.CompareTo( name ) == 0 )
                {
                    return item;
                }
            }

            return null;
        }


        /*********************************************************************
         * Description:
         * 
         * Returns the Item holding the given T or null if no match found.
         *
         * If the object(s) held by Item do not override Equals, this performs
         * a reference matching operation and not a true equality check.
         ********************************************************************/

        public Item< T > GetByHeld( T held )
        {
            try
            {
                foreach ( Item< T > item in this )
                {
                    if (item.Held.Equals( held ) )
                    {
                        return item;
                    }
                }
            }
            catch( Exception )
            {

            }

            return null;
        }



    } // END class EnumDataSource< T, N >



} // END namespace RFID_Explorer
