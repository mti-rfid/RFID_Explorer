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
 * $Id: Source_SingulationParameters.cs,v 1.4 2009/11/11 23:34:43 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */


using System;
using System.Collections.Generic;
using System.Text;


using rfid.Structures;
using rfid.Constants;


namespace RFID.RFIDInterface
{

    // Definition of base class for gui display of singulation
    // parameter component of query algorithm... this is used
    // in conjunction with the class Source_QueryParms.

    public class Source_SingulationParameters
        :
        Object
    {

        protected rfid.Structures.SingulationAlgorithmParms nativeSingulationParms;
        
    } // End interface Source_SingulationParameters




    /*************************************************************************
     * Begin specialization according to currently available algorithms and
     * their internal fields...
     *************************************************************************/


    // Singulation fixed Q ( currently algo # 0 ) parameters

    public class Source_SingulationParametersFixedQ
        :
        Source_SingulationParameters
    {

        public Source_SingulationParametersFixedQ( )
            :
            base( )
        {
            base.nativeSingulationParms = new rfid.Structures.FixedQParms( );
        }

        // Constructor ( from reference held only ! )

        public Source_SingulationParametersFixedQ
        (
            rfid.Structures.FixedQParms from
        )
            :
            base( )
        {
            base.nativeSingulationParms = from;
        }


        // DEEP COPY - C# does not have template specialization
        // so casting to this type to access method required

        public void Copy( Source_SingulationParametersFixedQ from )
        {
            this.QValue            = from.QValue;
            this.RetryCount        = from.RetryCount;
            this.ToggleTarget      = from.ToggleTarget;
            this.RepeatUntilNoTags = from.RepeatUntilNoTags;
        }


        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_SingulationParametersFixedQ rhs_b = obj as Source_SingulationParametersFixedQ;

            if ( null != ( System.Object ) rhs_b )
            {
                return this.Equals( rhs_b );
            }

            return false;
        }

        public bool Equals( rfid.Structures.FixedQParms rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.QValue            == rhs.qValue
                && this.RetryCount        == rhs.retryCount
                && this.ToggleTarget      == rhs.toggleTarget
                && this.RepeatUntilNoTags == rhs.repeatUntilNoTags;
        }

        public bool Equals( Source_SingulationParametersFixedQ rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.QValue            == rhs.QValue
                && this.RetryCount        == rhs.RetryCount
                && this.ToggleTarget      == rhs.ToggleTarget
                && this.RepeatUntilNoTags == rhs.RepeatUntilNoTags;
        }

        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }



        // Begin field exposure ( getters & setters )

        public byte QValue
        {
            get { return ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).qValue; }

            set { ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).qValue = value; }
        }

        public byte RetryCount
        {
            get { return ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).retryCount; }

            set { ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).retryCount = value; }
        }

        public byte ToggleTarget
        {
            get { return ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).toggleTarget; }

            set { ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).toggleTarget = value; }
        }

        public byte RepeatUntilNoTags
        {
            get { return ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).repeatUntilNoTags; }

            set { ( ( rfid.Structures.FixedQParms ) this.nativeSingulationParms ).repeatUntilNoTags = value; }
        }

    } // End class Source_SingulationParametersFixedQ ( 0 )





    // Singulation dynamic Q ( currently algo # 1 ) parameters

    public class Source_SingulationParametersDynamicQ
        :
        Source_SingulationParameters
    {

        public Source_SingulationParametersDynamicQ( )
            :
            base( )
        {
            this.nativeSingulationParms = new rfid.Structures.DynamicQParms( );
        }


        // Constructor ( from reference held only ! )

        public Source_SingulationParametersDynamicQ
        (
            rfid.Structures.DynamicQParms from
        )
            :
            base( )
        {
            this.nativeSingulationParms = from;
        }


        // DEEP COPY

        public void Copy( Source_SingulationParametersDynamicQ from )
        {
            this.StartQValue         = from.StartQValue;
            this.MinQValue           = from.MinQValue;
            this.MaxQValue           = from.MaxQValue;
            this.RetryCount          = from.RetryCount;
            this.ToggleTarget        = from.ToggleTarget;
            this.ThresholdMultiplier = from.ThresholdMultiplier;
        }



        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_SingulationParametersDynamicQ rhs_b = obj as Source_SingulationParametersDynamicQ;

            if ( null != ( System.Object ) rhs_b )
            {
                return this.Equals( rhs_b );
            }

            return false;
        }

        public bool Equals( rfid.Structures.DynamicQParms rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.StartQValue         == rhs.startQValue
                && this.MinQValue           == rhs.minQValue
                && this.MaxQValue           == rhs.maxQValue
                && this.RetryCount          == rhs.retryCount
                && this.ToggleTarget        == rhs.toggleTarget
                && this.ThresholdMultiplier == rhs.thresholdMultiplier;
        }

        public bool Equals( Source_SingulationParametersDynamicQ rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.StartQValue == rhs.StartQValue
                && this.MinQValue == rhs.MinQValue
                && this.MaxQValue == rhs.MaxQValue
                && this.RetryCount == rhs.RetryCount
                && this.ToggleTarget == rhs.ToggleTarget
                && this.ThresholdMultiplier == rhs.ThresholdMultiplier;
        }

        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }



        // Begin field exposure ( getters & setters )

        public byte StartQValue
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).startQValue; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).startQValue = value; }
        }

        public byte MinQValue
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).minQValue; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).minQValue = value; }
        }

        public byte MaxQValue
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).maxQValue; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).maxQValue = value; }
        }

        public byte RetryCount
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).retryCount; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).retryCount = value; }
        }

        public byte ToggleTarget
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).toggleTarget; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).toggleTarget = value; }
        }

        public byte ThresholdMultiplier
        {
            get { return ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).thresholdMultiplier; }

            set { ( ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms ).thresholdMultiplier = value; }
        }

    } // End class Source_SingulationParametersDynamicQ ( 1 )

} // End namespace RFID.RFIDInterface

