
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System.Collections.Generic;
using System.Xml.Linq;
using CSHTML5.Internal;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document
    /// using a supplied data contract. This class cannot be inherited.
    /// </summary>
    public class DataContractSerializer_CSHTML5Ver
    {
        private static readonly XmlReaderSettings DefaultXmlReaderSettings = new()
        {
            CheckCharacters = false,
            IgnoreWhitespace = true,
        };

        private static readonly XmlWriterSettings DefaultXmlWriterSettings = new()
        {
            CheckCharacters = false,
            OmitXmlDeclaration = true,
        };

        private readonly Type _type;
        private bool _useXmlSerializerFormat;
        private List<Type> _knownTypes;
        private XmlSerializer _xmlSerializer;
        private DataContractSerializer _dataContractSerializer;

        /// <summary>
        /// Initializes a new instance of the System.Runtime.Serialization.DataContractSerializer
        /// class to serialize or deserialize an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the instances that are serialized or deserialized.</param>
        /// <param name="useXmlSerializerFormat"></param>
        public DataContractSerializer_CSHTML5Ver(Type type, bool useXmlSerializerFormat = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this._type = type;

            if (this._useXmlSerializerFormat = useXmlSerializerFormat)
            {
                this._xmlSerializer = new XmlSerializer(type);
            }
            else
            {
                this._dataContractSerializer = new DataContractSerializer(type);
            }
        }

        /// <summary>
        /// Initializes a new instance of the System.Runtime.Serialization.DataContractSerializer
        /// class to serialize or deserialize an object of the specified type, and a
        /// collection of known types that may be present in the object graph.
        /// </summary>
        /// <param name="type">The type of the instances that are serialized or deserialized.</param>
        /// <param name="knownTypes">
        /// An System.Collections.Generic.IEnumerable`1 of System.Type that contains
        /// the types that may be present in the object graph.
        /// </param>
        /// <param name="useXmlSerializerFormat"></param>
        public DataContractSerializer_CSHTML5Ver(Type type, IEnumerable<Type> knownTypes, bool useXmlSerializerFormat = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this._type = type;

            if (knownTypes != null)
            {
                this._knownTypes = new List<Type>(knownTypes);
            }
            if (this._knownTypes == null)
            {
                this._knownTypes = new List<Type>(KnownTypesHelper.KnownTypes);
            }
            else
            {
                this._knownTypes.AddRange(KnownTypesHelper.KnownTypes);
            }
            if (this._useXmlSerializerFormat = useXmlSerializerFormat)
            {
                this._xmlSerializer = new XmlSerializer(type);
            }
            else
            {
                this._dataContractSerializer = new DataContractSerializer(type, this._knownTypes);
            }
        }

        /// <summary>
        /// Gets a collection of types that may be present in the object graph serialized
        /// using this instance of the System.Runtime.Serialization.DataContractSerializer.
        /// </summary>
        public IReadOnlyList<Type> KnownTypes
        {
            get { return this._knownTypes; }
        }

        public string SerializeToString(object obj, bool indentXml = false, bool omitXmlDeclaration = false)
        {
            return (omitXmlDeclaration ? "" : (@"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine)) +
                this.SerializeToXDocument(obj).ToString(indentXml ? SaveOptions.None : SaveOptions.DisableFormatting);
        }

        public XDocument SerializeToXDocument(object obj)
        {
            using (var ms = new MemoryStream())
            {
                SerializePrivate(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(ms))
                {
                    return ParseToXDocument(sr);
                }
            }
        }

        internal static XDocument ParseToXDocument(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                return ParseToXDocument(sr);
            }
        }

        private static XDocument ParseToXDocument(TextReader tr)
        {
            using (var xr = XmlReader.Create(tr, DefaultXmlReaderSettings))
            {
                return XDocument.Load(xr);
            }
        }

        public object DeserializeFromString(string xml)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    return DeserializePrivate(ms);
                }
            }
        }

        public object DeserializeFromXElement(XElement xElement)
        {
            return DeserializeFromString(XElementToString(xElement));
        }

        internal static string XElementToString(XElement xElement)
        {
            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var xw = XmlWriter.Create(sw, DefaultXmlWriterSettings))
                {
                    xElement.Save(xw);
                }
                return sw.ToString();
            }
        }

        private void SerializePrivate(Stream s, object o)
        {
            if (this._useXmlSerializerFormat)
            {
                this._xmlSerializer.Serialize(s, o);
            }
            else
            {
                this._dataContractSerializer.WriteObject(s, o);
            }
        }

        private object DeserializePrivate(Stream s)
        {
            using (var xr = XmlReader.Create(s, DefaultXmlReaderSettings))
            {
                if (_useXmlSerializerFormat)
                {
                    return _xmlSerializer.Deserialize(xr);
                }
                else
                {
                    return _dataContractSerializer.ReadObject(xr, false);
                }
            }
        }
    }
}