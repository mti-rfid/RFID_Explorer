using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Global
{
    /// <summary>

    /// Create a New INI file to store or load data

    /// </summary>

    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key,
                                                              string val, string filePath);
        [DllImport("kernel32")]
        private static extern UInt32 GetPrivateProfileString(string section, string key, string def,
                                                             byte[] retVal, Int32 size, string filePath);


        /// <summary>

        /// INIFile Constructor.

        /// </summary>

        /// <PARAM name="INIPath"></PARAM>

        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>

        /// Write Data to the INI File

        /// </summary>

        /// <PARAM name="Section"></PARAM>

        /// Section name

        /// <PARAM name="Key"></PARAM>

        /// Key Name

        /// <PARAM name="Value"></PARAM>

        /// Value Name

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>

        /// Read Data Value From the Ini File

        /// </summary>

        /// <PARAM name="Section"></PARAM>

        /// <PARAM name="Key"></PARAM>

        /// <PARAM name="Path"></PARAM>

        /// <returns></returns>

        //public string IniReadValue(string Section, string Key)
        //{
        //    //string buffer;
        //    byte[] buf = new byte[40];
        //    Array.Clear(buf, 0, buf.Length);

        //    UInt32 i = GetPrivateProfileString(Section, Key, "", buf, buf.Length, this.path);
        //    return buf.ToString();
        //}



        //Write data to ini
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, path))
            {

                throw (new ApplicationException("Error: at the time that write data to ini."));
            }
        }


        //Read data from ini
        public string ReadString(string Section, string Ident, string Default)
        {
            //string buffer;
            byte[] buf = new byte[40];
            Array.Clear(buf, 0, buf.Length);

            UInt32 bufLen = GetPrivateProfileString(Section, Ident, Default, buf, buf.Length, path);

            //need to set 0 to support chinese
            string s = System.Text.Encoding.Default.GetString(buf);
            s = s.Substring(0, (Int32)bufLen);
            return s.Trim();
        }



        public UInt32 ReadInteger(string Section, string Ident, UInt32 Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToUInt32(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }


        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }



        public UInt64 ReadInteger64(string Section, string Ident, UInt32 Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToUInt64(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }



        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }


    }
}