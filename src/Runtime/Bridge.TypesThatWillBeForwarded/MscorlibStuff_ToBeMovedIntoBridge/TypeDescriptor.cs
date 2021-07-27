using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class TypeDescriptor
    {
        public static TypeConverter GetConverter(object component)
        {
            return GetConverter(component, false);
        }

        public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
        {
            throw new NotImplementedException();
        }
    }
}
