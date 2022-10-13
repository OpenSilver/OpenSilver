

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenSilver.Simulator
{
    public static class Serializer
    {
        public static string Save<T>(this T objToSerialize)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
                using (StringWriter stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, objToSerialize);
                    return stringWriter.ToString();
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static T Load<T>(string data) where T : new()
        {
            T t = new T();
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(data))
                {
                    return (T)xmlSerializer.Deserialize(stringReader);
                }
            }
            catch (IOException)
            {
                return t;
            }
        }
    }
}
