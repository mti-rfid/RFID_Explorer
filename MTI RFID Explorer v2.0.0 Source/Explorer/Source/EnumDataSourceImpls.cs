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
 * $Id: EnumDataSourceImpls.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description:
 *     
 *     Singleton DataSource classes with one class per enumeration accessible from
 *     the lower level ManagedReaderLibrary.  The naming convention used is
 *     [ENUM_NAME]_Source so that e.g. an enum named Foo in the MRL would have a
 *     source implementation here named Foo_Source.
 *     
 *     These classes are ( will be ) used to populate and validate fields in the
 *     reader app GUI by assigning them as the specific GUI item's DataSource to
 *     [ENUM_NAME]_Source.Instance and the DisplayMember field to the value of
 *     [ENUM_NAME]_Source.Instance.DisplayName.
 *     
 *     Currently all supply a DisplayName generated via a name generator that
 *     simply converts the default name to all lower case lettering.
 *     
 *     TODO:
 *     
 *     Create a couple of custom name generators for types where required e.g. the
 *     singulation algorithm type so 'FIXEDQ' -> 'Fixed Q' and so forth.
 *     
 *
 *****************************************************************************
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;



namespace RFID_Explorer
{

    public class LibraryMode_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.LibraryMode,
                LowerCaseNameGenerator
                <
                    rfid.Constants.LibraryMode
                >
            >
        >
    {
        private LibraryMode_Source() : base( ) { /* NOP */ }
    }

    public class RadioOperationMode_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.RadioOperationMode,
                LowerCaseNameGenerator
                <
                    rfid.Constants.RadioOperationMode
                >
            >
        >
    {
        private RadioOperationMode_Source() : base( ) { /* NOP */ }
    }

    public class RadioMode_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.RadioOperationMode,
                LowerCaseNameGenerator
                <
                    rfid.Constants.RadioOperationMode
                >
            >
        >
    {
        private RadioMode_Source() : base( ) { /* NOP */ }
    }

    public class RadioPowerState_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.RadioPowerState,
                LowerCaseNameGenerator
                <
                    rfid.Constants.RadioPowerState
                >
            >
        >
    {
        private RadioPowerState_Source() : base( ) { /* NOP */ }
    }

    public class AntennaPortState_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.AntennaPortState,
                LowerCaseNameGenerator
                <
                    rfid.Constants.AntennaPortState
                >
            >
        >
    {
        private AntennaPortState_Source() : base( ) { /* NOP */ }
    }

    public class ModulationType_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.ModulationType,
                LowerCaseNameGenerator
                <
                    rfid.Constants.ModulationType
                >
            >
        >
    {
        private ModulationType_Source() : base( ) { /* NOP */ }
    }

    public class DataDifference_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.DataDifference,
                LowerCaseNameGenerator
                <
                    rfid.Constants.DataDifference
                >
            >
        >
    {
        private DataDifference_Source() : base( ) { /* NOP */ }
    }

    public class DivideRatio_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.DivideRatio,
                LowerCaseNameGenerator
                <
                    rfid.Constants.DivideRatio
                >
            >
        >
    {
        private DivideRatio_Source() : base( ) { /* NOP */ }
    }

    public class MillerNumber_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.MillerNumber,
                LowerCaseNameGenerator
                <
                    rfid.Constants.MillerNumber
                >
            >
        >
    {
        private MillerNumber_Source() : base( ) { /* NOP */ }
    }

    public class RadioProtocol_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.RadioProtocol,
                LowerCaseNameGenerator
                <
                    rfid.Constants.RadioProtocol
                >
            >
        >
    {
        private RadioProtocol_Source() : base( ) { /* NOP */ }
    }

    public class MemoryBank_18K6C_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.MemoryBank,
                LowerCaseNameGenerator
                <
                    rfid.Constants.MemoryBank
                >
            >
        >
    {
        private MemoryBank_18K6C_Source() : base( ) { /* NOP */ }
    }

    public class Target_18K6C_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.Target,
                LowerCaseNameGenerator
                <
                    rfid.Constants.Target
                >
            >
        >
    {
        private Target_18K6C_Source() : base( ) { /* NOP */ }
    }

    public class Action_18K6C_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.Action,
                LowerCaseNameGenerator
                <
                    rfid.Constants.Action
                >
            >
        >
    {
        private Action_18K6C_Source() : base( ) { /* NOP */ }
    }

    public class Selected_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.Selected,
                LowerCaseNameGenerator
                <
                    rfid.Constants.Selected
                >
            >
        >
    {
        private Selected_Source() : base( ) { /* NOP */ }
    }

    public class InventorySession_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.Session,
                LowerCaseNameGenerator
                <
                    rfid.Constants.Session
                >
            >
        >
    {
        private InventorySession_Source() : base( ) { /* NOP */ }
    }

    public class InventorySessionTarget_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.SessionTarget,
                LowerCaseNameGenerator
                <
                    rfid.Constants.SessionTarget
                >
            >
        >
    {
        private InventorySessionTarget_Source() : base( ) { /* NOP */ }
    }

    public class SingulationAlgorithm_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.SingulationAlgorithm,
                LowerCaseNameGenerator
                <
                    rfid.Constants.SingulationAlgorithm
                >
            >
        >
    {
        private SingulationAlgorithm_Source() : base( ) { /* NOP */ }
    }

    public class WriteType_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.WriteType,
                LowerCaseNameGenerator
                <
                    rfid.Constants.WriteType
                >
            >
        >
    {
        private WriteType_Source() : base( ) { /* NOP */ }
    }

    public class PasswordPermission_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.PasswordPermission,
                LowerCaseNameGenerator
                <
                    rfid.Constants.PasswordPermission
                >
            >
        >
    {
        private PasswordPermission_Source() : base( ) { /* NOP */ }
    }

    public class MemoryPermission_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.MemoryPermission,
                LowerCaseNameGenerator
                <
                    rfid.Constants.MemoryPermission
                >
            >
        >
    {
        private MemoryPermission_Source() : base( ) { /* NOP */ }
    }

    public class ResponseType_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.ResponseType,
                LowerCaseNameGenerator
                <
                    rfid.Constants.ResponseType
                >
            >
        >
    {
        private ResponseType_Source() : base( ) { /* NOP */ }
    }

    public class ResponseMode_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.ResponseMode,
                LowerCaseNameGenerator
                <
                    rfid.Constants.ResponseMode
                >
            >
        >
    {
        private ResponseMode_Source() : base( ) { /* NOP */ }
    }

    public class MacResetType_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.MacResetType,
                LowerCaseNameGenerator
                <
                    rfid.Constants.MacResetType
                >
            >
        >
    {
        private MacResetType_Source() : base( ) { /* NOP */ }
    }

    public class MacRegion_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.MacRegion,
                LowerCaseNameGenerator
                <
                    rfid.Constants.MacRegion
                >
            >
        >
    {
        private MacRegion_Source() : base( ) { /* NOP */ }
    }

    public class GpioPin_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.GpioPin,
                LowerCaseNameGenerator
                <
                    rfid.Constants.GpioPin
                >
            >
        >
    {
        private GpioPin_Source() : base( ) { /* NOP */ }
    }

    /*
    public class TagOpFlag_18K6C_Source
        :
        Patterns.Singleton
        <
            EnumDataSource
            <
                rfid.Constants.TagOpFlag_18K6C,
                LowerCaseNameGenerator
                <
                    rfid.Constants.TagOpFlag_18K6C
                >
            >
        >
    {
        private TagOpFlag_18K6C_Source() : base( ) { }
    }
    */

}
