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
 * $Id: ConfigureVersion.cs,v 1.5 2009/12/03 03:57:50 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;
using Global;


namespace RFID_Explorer
{
	public partial class AboutReaderControl : UserControl
	{
		

		private LakeChabotReader reader = null;



		public AboutReaderControl( LakeChabotReader reader )
		{
			if ( null == reader )
            {
				throw new ArgumentNullException( "reader", "Null reader passed to ConfigureGeneral CTOR()" );
            }

			if (reader.Mode != rfidReader.OperationMode.BoundToReader)
            {
				throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureGeneral()" );
            }

			InitializeComponent( );

			this.reader = reader;
		}


        private string ShowOemData	
        (
            UInt16 Offset
        )
        {
            UInt32 OemData = 0;
            UInt32 [] pOemData  = null;
            UInt32 uiLength = 0;
            rfid.Constants.Result result = rfid.Constants.Result.OK;

            if( rfid.Constants.Result.OK != reader.MacReadOemData( Offset, ref OemData) )
                return null;

            uiLength =  (0 == (OemData & 0xFF) / 4) ? 1 : (OemData & 0xFF) / 4;
            uiLength += (uint)((0 == (OemData & 0xFF) % 4) ? 0 : 1);

            pOemData = new UInt32[uiLength];
            Array.Clear(pOemData, 0, pOemData.Length);

            pOemData[0] = OemData;
            //Offset++; //Point to  data address
            for (int i = 1; i < uiLength; i++)
            {
                result = reader.MacReadOemData( (UInt16)(Offset + i), ref pOemData[i] );

                if( rfid.Constants.Result.OK != result )
                    return null;
            }

            //return Source_OEMData.uint32ArrayToString(pOemData, 0);
            return CGlobalFunc.uint32ArrayToString(pOemData, uiLength);
        }



		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            UInt32 uiModelNameMajor = 0;
            UInt32 uiModelNameSub   = 0;

         
            //Source_OEMData data = new Source_OEMData( );

			try
			{         

                //clark 2011.4.11 doesn't load all oem.
                manufactureTextBox.Text  = ShowOemData((UInt16)Source_OEMData.OEMCFG_ADDRS.OEMCFGADDR_MFG_NAME_BASE);
                serialNumberTextBox.Text = ShowOemData((UInt16)Source_OEMData.OEMCFG_ADDRS.OEMCFGADDR_SERIAL_NUM_BASE);
                productTextBox.Text      = ShowOemData((UInt16)Source_OEMData.OEMCFG_ADDRS.OEMCFGADDR_PROD_NAME_BASE);


                //Get Model Name
                rfid.Constants.Result result = rfid.Constants.Result.OK;
                result  = reader.MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_MAIN), ref uiModelNameMajor);
                if (rfid.Constants.Result.OK != result)
                {
                    throw new Exception(result.ToString());
                }
                
                result  = reader.MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_SUB), ref uiModelNameSub);
                if (rfid.Constants.Result.OK != result)
                {
                    throw new Exception(result.ToString());
                }

                if (uiModelNameMajor == 0x4D303258)//0x4D303258==M02X
                {
                    ModelTextBox.Text = String.Format("RU00-{0}{1}{2}-{3}{4}{5}{6}",
                                                   (char)((uiModelNameMajor >> 24) & 0xFF),
                                                   (char)((uiModelNameMajor >> 16) & 0xFF),
                                                   (char)((uiModelNameMajor >> 8) & 0xFF),
                                                   (char)(uiModelNameMajor & 0xFF),
                                                   (char)((uiModelNameSub >> 16) & 0xFF),
                                                   (char)((uiModelNameSub >> 8) & 0xFF),
                                                   (char)(uiModelNameSub & 0xFF));
                }
                else
                {
                    ModelTextBox.Text = String.Format("RU-{0}{1}{2}-{3}{4}{5}{6}",
                                                       (char)((uiModelNameMajor >> 16) & 0xFF),
                                                       (char)((uiModelNameMajor >> 8) & 0xFF),
                                                       (char)(uiModelNameMajor & 0xFF),
                                                       (char)((uiModelNameSub >> 16) & 0xFF),
                                                       (char)((uiModelNameSub >> 8) & 0xFF),
                                                       (char)(uiModelNameSub & 0xFF),
                                                       (char)((uiModelNameSub >> 24) & 0xFF));
                }
                /*Mod by Rick for model name, 2013-01-29*/
			}
			catch (Exception)
			{
                manufactureTextBox.Text  = "Not Available";
				productTextBox.Text      = "Not Available";
				serialNumberTextBox.Text = "Not Available";
			}

            //manufactureTextBox.Text  = data.Manufacturer;
            //productTextBox.Text      = data.Product;
            //serialNumberTextBox.Text = data.SerialNumber;


			
			try
			{
				OemTextBox.Text = String.Format
                    (
                        "{0}.{1}{2}",                        
                         (char) this.reader.OEMCfgVersion.major,
                         (char) this.reader.OEMCfgVersion.minor,
                         (char) this.reader.OEMCfgVersion.release
                    );
			}
			catch (rfidReaderException exp)
			{
				OemTextBox.Text	= exp.Message;
			}


			try
			{
				updatePackTextBox.Text = String.Format
                    (
                        "{0}{1}",                        
                        (char) this.reader.OEMCfgUpdateNumber.major,
                        (char) this.reader.OEMCfgUpdateNumber.minor
                    );
			}
			catch (rfidReaderException exp)
			{
				updatePackTextBox.Text	= exp.Message;
			}




            try
            {
                fwVersionTextBox.Text = String.Format
                    (
                        "{0}.{1}.{2}",
                        this.reader.FirmwareVersion.major,
                        this.reader.FirmwareVersion.minor,
                        this.reader.FirmwareVersion.release
                    );
            }
            catch (rfidReaderException exp)
            {
                fwVersionTextBox.Text = exp.Message;
            }


			
			try
			{
                BootLoaderTextBox.Text = String.Format
                    (
                        "{0}.{1}.{2}",
                        this.reader.BootLoaderVersion.major,
                        this.reader.BootLoaderVersion.minor,
                        this.reader.BootLoaderVersion.release
                    );
			}
			catch (rfidReaderException exp)
			{
                BootLoaderTextBox.Text = exp.Message;
			}		
	

			try
			{
                if (this.reader.RegulatoryRegion == rfid.Constants.MacRegion.CUSTOMER)
                {
                    string strRegion = null;

                    rfid.Constants.Result result = rfid.Constants.Result.OK;
                    result = this.reader.API_MacGetCustomerRegion(ref strRegion);

                    switch (result)
                    {
                        case rfid.Constants.Result.OK:
                            regionTextBox.Text = String.Format("{0}", strRegion);
                            break;

                        case rfid.Constants.Result.NOT_SUPPORTED:
                            regionTextBox.Text = "Not support customer region";
                            break;

                        case rfid.Constants.Result.FAILURE:
                        default:
                            regionTextBox.Text = "Get customer region fail";
                            break;

                    }

                }
                else
                {
                    regionTextBox.Text = String.Format("{0}", this.reader.RegulatoryRegion);
                }

			}
			catch (rfidReaderException exp)
			{
				regionTextBox.Text = exp.Message;
			}

			
		}		

	}

}
