using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{

    //===============================Frequency=============================
    public enum ENUM_RF_US : uint
    {
        CHANNEL_01 = 902750,
        CHANNEL_02 = 903250,
        CHANNEL_03 = 903750,
        CHANNEL_04 = 904250,
        CHANNEL_05 = 904750,
        CHANNEL_06 = 905250,
        CHANNEL_07 = 905750,
        CHANNEL_08 = 906250,
        CHANNEL_09 = 906750,
        CHANNEL_10 = 907250,
        CHANNEL_11 = 907750,
        CHANNEL_12 = 908250,
        CHANNEL_13 = 908750,
        CHANNEL_14 = 909250,
        CHANNEL_15 = 909750,
        CHANNEL_16 = 910250,
        CHANNEL_17 = 910750,
        CHANNEL_18 = 911250,
        CHANNEL_19 = 911750,
        CHANNEL_20 = 912250,
        CHANNEL_21 = 912750,
        CHANNEL_22 = 913250,
        CHANNEL_23 = 913750,
        CHANNEL_24 = 914250,
        CHANNEL_25 = 914750,
        CHANNEL_26 = 915250,
        CHANNEL_27 = 915750,
        CHANNEL_28 = 916250,
        CHANNEL_29 = 916750,
        CHANNEL_30 = 917250,
        CHANNEL_31 = 917750,
        CHANNEL_32 = 918250,
        CHANNEL_33 = 918750,
        CHANNEL_34 = 919250,
        CHANNEL_35 = 919750,
        CHANNEL_36 = 920250,
        CHANNEL_37 = 920750,
        CHANNEL_38 = 921250,
        CHANNEL_39 = 921750,
        CHANNEL_40 = 922250,
        CHANNEL_41 = 922750,
        CHANNEL_42 = 923250,
        CHANNEL_43 = 923750,
        CHANNEL_44 = 924250,
        CHANNEL_45 = 924750,
        CHANNEL_46 = 925250,
        CHANNEL_47 = 925750,
        CHANNEL_48 = 926250,
        CHANNEL_49 = 926750,
        CHANNEL_50 = 927250,
    }


    public enum ENUM_RF_EU : uint
    {
        CHANNEL_01 = 865700,
        CHANNEL_02 = 866300,
        CHANNEL_03 = 866900,
        CHANNEL_04 = 867500,
    }


    public enum ENUM_RF_JP : uint
    {
        CHANNEL_01 = 952200,
        CHANNEL_02 = 952400,
        CHANNEL_03 = 952600,
        CHANNEL_04 = 952800,
        CHANNEL_05 = 953000,
        CHANNEL_06 = 953200,
        CHANNEL_07 = 953400,
        CHANNEL_08 = 953600,
        CHANNEL_09 = 953800,
    }



    public enum ENUM_RF_EU2 : uint
    {
        CHANNEL_01 = 869850,
    }



    public enum ENUM_RF_TW : uint
    {
        CHANNEL_01 = 922250,
        CHANNEL_02 = 923750,
        CHANNEL_03 = 923250,
        CHANNEL_04 = 923750,
        CHANNEL_05 = 924250,
        CHANNEL_06 = 924750,
        CHANNEL_07 = 925250,
        CHANNEL_08 = 925750,
        CHANNEL_09 = 926250,
        CHANNEL_10 = 926750,
        CHANNEL_11 = 927250,
        CHANNEL_12 = 927750,
    }


    public enum ENUM_RF_CN : uint
    {
        CHANNEL_01 = 920625,
        CHANNEL_02 = 920875,
        CHANNEL_03 = 921125,
        CHANNEL_04 = 921375,
        CHANNEL_05 = 921625,
        CHANNEL_06 = 921875,
        CHANNEL_07 = 922125,
        CHANNEL_08 = 922375,
        CHANNEL_09 = 922625,
        CHANNEL_10 = 922875,
        CHANNEL_11 = 923125,
        CHANNEL_12 = 923375,
        CHANNEL_13 = 923625,
        CHANNEL_14 = 923875,
        CHANNEL_15 = 924125,
        CHANNEL_16 = 924375,
    }


    public enum ENUM_RF_KR : uint
    {
        CHANNEL_01 = 917320,
        CHANNEL_02 = 917900,
        CHANNEL_03 = 918500,
        CHANNEL_04 = 919100,
        CHANNEL_05 = 919700,
        CHANNEL_06 = 920300,
    }



    public enum ENUM_RF_AU : uint
    {
        CHANNEL_01 = 922250,
        CHANNEL_02 = 922750,
        CHANNEL_03 = 923250,
        CHANNEL_04 = 923750,
        CHANNEL_05 = 924250,
        CHANNEL_06 = 924750,
        CHANNEL_07 = 925250,
    }




    public enum ENUM_RF_BR : uint
    {
        CHANNEL_01 = 902750,
        CHANNEL_02 = 903250,
        CHANNEL_03 = 903750,
        CHANNEL_04 = 904250,
        CHANNEL_05 = 904750,
        CHANNEL_06 = 905250,
        CHANNEL_07 = 905750,
        CHANNEL_08 = 906250,
        CHANNEL_09 = 906750,
        CHANNEL_10 = 907250,
        CHANNEL_11 = 915250,
        CHANNEL_12 = 915750,
        CHANNEL_13 = 916250,
        CHANNEL_14 = 916750,
        CHANNEL_15 = 917250,
        CHANNEL_16 = 917750,
        CHANNEL_17 = 918250,
        CHANNEL_18 = 918750,
        CHANNEL_19 = 919250,
        CHANNEL_20 = 919750,
        CHANNEL_21 = 920250,
        CHANNEL_22 = 920750,
        CHANNEL_23 = 921250,
        CHANNEL_24 = 921750,
        CHANNEL_25 = 922250,
        CHANNEL_26 = 922750,
        CHANNEL_27 = 923250,
        CHANNEL_28 = 923750,
        CHANNEL_29 = 924250,
        CHANNEL_30 = 924750,
        CHANNEL_31 = 925250,
        CHANNEL_32 = 925750,
        CHANNEL_33 = 926250,
        CHANNEL_34 = 926750,
        CHANNEL_35 = 927250,
    }



    public enum ENUM_RF_HK : uint
    {
        CHANNEL_01 = 920750,
        CHANNEL_02 = 921250,
        CHANNEL_03 = 921750,
        CHANNEL_04 = 922250,
        CHANNEL_05 = 922750,
        CHANNEL_06 = 923250,
        CHANNEL_07 = 923750,
        CHANNEL_08 = 924250,
    }


    public enum ENUM_RF_MY : uint
    {
        CHANNEL_01 = 919750,
        CHANNEL_02 = 920250,
        CHANNEL_03 = 920750,
        CHANNEL_04 = 921250,
        CHANNEL_05 = 921750,
        CHANNEL_06 = 922250,
    }



    public enum ENUM_RF_SG : uint
    {
        CHANNEL_01 = 920750,
        CHANNEL_02 = 921250,
        CHANNEL_03 = 921750,
        CHANNEL_04 = 922250,
        CHANNEL_05 = 922750,
        CHANNEL_06 = 923250,
        CHANNEL_07 = 923750,
        CHANNEL_08 = 924250,
    }


    public enum ENUM_RF_TH : uint
    {
        CHANNEL_01 = 920750,
        CHANNEL_02 = 921250,
        CHANNEL_03 = 921750,
        CHANNEL_04 = 922250,
        CHANNEL_05 = 922750,
        CHANNEL_06 = 923250,
        CHANNEL_07 = 923750,
        CHANNEL_08 = 924250,
    }


    public enum ENUM_RF_IL : uint
    {
        CHANNEL_01 = 915750,
        CHANNEL_02 = 916250,
    }

    public enum ENUM_RF_RU : uint
    {
        CHANNEL_01 = 866300,
        CHANNEL_02 = 866900,
    }


    public enum ENUM_RF_IN : uint
    {
        CHANNEL_01 = 865700,
        CHANNEL_02 = 866300,
    }



    public enum ENUM_RF_SA : uint
    {
        CHANNEL_01 = 865700,
        CHANNEL_02 = 866300,
        CHANNEL_03 = 866900,
        CHANNEL_04 = 867500,
    }


    public enum ENUM_RF_JO : uint
    {
        CHANNEL_01 = 865700,
        CHANNEL_02 = 866300,
        CHANNEL_03 = 866900,
        CHANNEL_04 = 867500,
    }




    public enum ENUM_RF_MX : uint
    {
        CHANNEL_01 = 902750,
        CHANNEL_02 = 903250,
        CHANNEL_03 = 903750,
        CHANNEL_04 = 904250,
        CHANNEL_05 = 904750,
        CHANNEL_06 = 905250,
        CHANNEL_07 = 905750,
        CHANNEL_08 = 906250,
        CHANNEL_09 = 906750,
        CHANNEL_10 = 907250,
        CHANNEL_11 = 907750,
        CHANNEL_12 = 908250,
        CHANNEL_13 = 908750,
        CHANNEL_14 = 909250,
        CHANNEL_15 = 909750,
        CHANNEL_16 = 910250,
        CHANNEL_17 = 910750,
        CHANNEL_18 = 911250,
        CHANNEL_19 = 911750,
        CHANNEL_20 = 912250,
        CHANNEL_21 = 912750,
        CHANNEL_22 = 913250,
        CHANNEL_23 = 913750,
        CHANNEL_24 = 914250,
        CHANNEL_25 = 914750,
        CHANNEL_26 = 915250,
        CHANNEL_27 = 915750,
        CHANNEL_28 = 916250,
        CHANNEL_29 = 916750,
        CHANNEL_30 = 917250,
        CHANNEL_31 = 917750,
        CHANNEL_32 = 918250,
        CHANNEL_33 = 918750,
        CHANNEL_34 = 919250,
        CHANNEL_35 = 919750,
        CHANNEL_36 = 920250,
        CHANNEL_37 = 920750,
        CHANNEL_38 = 921250,
        CHANNEL_39 = 921750,
        CHANNEL_40 = 922250,
        CHANNEL_41 = 922750,
        CHANNEL_42 = 923250,
        CHANNEL_43 = 923750,
        CHANNEL_44 = 924250,
        CHANNEL_45 = 924750,
        CHANNEL_46 = 925250,
        CHANNEL_47 = 925750,
        CHANNEL_48 = 926250,
        CHANNEL_49 = 926750,
        CHANNEL_50 = 927250,
    }









}
