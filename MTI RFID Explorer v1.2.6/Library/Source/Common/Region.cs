using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using rfid.Constants;

namespace Global
{

    public class CGlobalRegion
    {
        ArrayList list = new ArrayList();

        public void GetRegionList()
        {            
            foreach ( MacRegion item in Enum.GetValues(typeof(MacRegion)) )
            { 
                ValueObject vo = new ValueObject();

                if (item == MacRegion.CUSTOMER)
                {
                    vo.Name = item.ToString();
                }
                else
                {
                    vo.Name = item.ToString();
                }

                vo.Value = Enum.Format( typeof(MacRegion), item, "d" );
                list.Add(vo);            
            }
        }   
 
        //public void ChangeRegionName( MacRegion r_macRegion,string r_strRegion)
        //{
        //    //(MacRegion)(list[(int)r_macRegion]).
        //    list.
        //}  
    }


    public class ValueObject
    {
        private string _name;
        private string _value;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }



}
