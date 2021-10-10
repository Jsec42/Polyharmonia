using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using SharpDX.DirectInput;

namespace ANPSupport
{
    class rangeInt
    {
        public int lower_bound;
        public int upper_bound;
        rangeInt(int lower, int upper)
        {
            lower_bound = lower;
            upper_bound = upper;
        }
        public override bool Equals(object o) => this == (rangeInt)o;
        public override int GetHashCode()
        {
            return ((lower_bound << 1) + (upper_bound << 1)) >> 1;
        }
        public static bool operator ==(rangeInt initial, int condition) => condition <= initial.upper_bound && condition >= initial.lower_bound;
        public static bool operator !=(rangeInt initial, int condition) => !initial.Equals(condition);
        public static bool operator ==(rangeInt initial, rangeInt condition) => initial.lower_bound ==  condition.lower_bound && initial.upper_bound == condition.upper_bound;
        public static bool operator !=(rangeInt initial, rangeInt condition) => !initial.Equals(condition);
        public static bool operator >(rangeInt initial, int condition) => condition > initial.upper_bound;
        public static bool operator <=(rangeInt initial, int condition) => condition <= initial.upper_bound;
        public static bool operator <(rangeInt initial, int condition) => condition < initial.lower_bound;
        public static bool operator >=(rangeInt initial, int condition) => condition >= initial.lower_bound;
    }
    class json_dto<t>
    {
        private t _data;
        private string _json_string;
        public t Data
        {
            get
            {
                if (newJson)
                {
                    _data = JsonConvert.DeserializeObject<t>(_json_string);
                }
                newJson = false;
                return _data;
            }
            set
            {
                newData = true;
                _data = value;
            }
        }
        public string Json_string
        {
            get
            {
                if (newData)
                {
                    _json_string = JsonConvert.SerializeObject(_data);
                }
                newData = false;
                return _json_string;
            }
            set
            {
                newJson = true;
                _json_string = value;
            }
        }
        bool newJson = false;
        bool newData = false;
        public json_dto(string new_json)
        {
            Json_string = new_json;
        }
        public json_dto(t new_data)
        {
            Data = new_data;
        }
    }
    class List_helpers
    {
        public static void move_element(ref IList<object> itemList, int oldIndex, int newIndex)
        {
            itemList.Insert(newIndex, itemList[oldIndex]);
            itemList.RemoveAt(newIndex < oldIndex ? oldIndex + 1 : oldIndex);
        }
    }
}
