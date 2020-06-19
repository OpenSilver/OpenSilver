using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DotNetForHtml5.EmulatorWithoutJavascript
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
