

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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CSHTML5.Internal;

#if NETSTANDARD
using System.IO;
using System.Xml.Serialization;
using System.Xml;
#endif

#if BRIDGE
using Bridge;
#else
using JSIL.Meta;
#endif

namespace System.Runtime.Serialization
{
#if NETSTANDARD
    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document
    /// using a supplied data contract. This class cannot be inherited.
    /// </summary>
    public class DataContractSerializer_CSHTML5Ver
    {
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
            if (KnownTypesHelper._additionalKnownTypes != null)
            {
                if (this._knownTypes == null)
                {
                    this._knownTypes = new List<Type>(KnownTypesHelper._additionalKnownTypes);
                }
                else
                {
                    this._knownTypes.AddRange(KnownTypesHelper._additionalKnownTypes);
                }

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
                this.SerializeToXmlDocument(obj).OuterXml;
        }

        public XmlDocument SerializeToXmlDocument(object obj)
        {
            XmlDocument doc = null;
            using (MemoryStream ms = new MemoryStream())
            {
                this.SerializePrivate(ms, obj);
                using (StreamReader sr = new StreamReader(ms))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    string xml = sr.ReadToEnd();
                    doc = new XmlDocument();
                    doc.LoadXml(xml);
                }
            }
            return doc;
        }
        public XDocument SerializeToXDocument(object obj)
        {
            XDocument doc = null;
            using (MemoryStream ms = new MemoryStream())
            {
                this.SerializePrivate(ms, obj);
                using (StreamReader sr = new StreamReader(ms))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    string xml = sr.ReadToEnd();
                    doc = XDocument.Parse(xml);
                }
            }
            return doc;
        }

        public object DeserializeFromString(string xml)
        {
            object o = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    o = this.DeserializePrivate(ms);
                }
            }
            return o;
        }
        public object DeserializeFromXmlNode(XmlNode xElement)
        {
            return this.DeserializeFromString(xElement.OuterXml);
        }

        public object DeserializeFromXElement(XElement xElement)
        {
            return this.DeserializeFromString(xElement.ToString(SaveOptions.DisableFormatting));
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
            var settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            var reader = XmlReader.Create(s, settings);
            if (this._useXmlSerializerFormat)
            {
                return this._xmlSerializer.Deserialize(reader);
            }
            else
            {
                return this._dataContractSerializer.ReadObject(reader, false);
            }
        }
    }
#else
    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document
    /// using a supplied data contract. This class cannot be inherited.
    /// </summary>
    public partial class DataContractSerializer
    {
        Type _type;
        bool _useXmlSerializerFormat;

        private IReadOnlyList<Type> _knownTypes = new List<Type>();
        // Returns:
        //     A System.Collections.ObjectModel.ReadOnlyCollection<T> that contains the
        //     expected types passed in as known types to the System.Runtime.Serialization.DataContractSerializer
        //     constructor.
        /// <summary>
        /// Gets a collection of types that may be present in the object graph serialized
        /// using this instance of the System.Runtime.Serialization.DataContractSerializer.
        /// </summary>
        public IReadOnlyList<Type> KnownTypes { get { return _knownTypes; } }


        /// <summary>
        /// Initializes a new instance of the System.Runtime.Serialization.DataContractSerializer
        /// class to serialize or deserialize an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the instances that are serialized or deserialized.</param>
        /// <param name="useXmlSerializerFormat"></param>
        public DataContractSerializer(Type type, bool useXmlSerializerFormat = false)
        {
            DataContractSerializer_ValueTypesHandler.EnsureInitialized();

            _type = type;
            _useXmlSerializerFormat = useXmlSerializerFormat;
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
        public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, bool useXmlSerializerFormat = false)
        {
            DataContractSerializer_ValueTypesHandler.EnsureInitialized();

            _type = type;
            _useXmlSerializerFormat = useXmlSerializerFormat;

            if (knownTypes != null)
            { 
                ((List<Type>)_knownTypes).AddRange(knownTypes);
            }
            if (KnownTypesHelper._additionalKnownTypes != null)
            {
                ((List<Type>)_knownTypes).AddRange(KnownTypesHelper._additionalKnownTypes); //todo: add only the items that are not already in the destination collection?
            }
        }

        public string SerializeToString(object obj, bool indentXml = false, bool omitXmlDeclaration = false)
        {
            XDocument xdoc = new XDocument();

            string defaultNamespace = DataContractSerializer_Helpers.GetDefaultNamespace(_type.Namespace, _useXmlSerializerFormat);

            //todo: USEMETHODTOGETTYPE: make a method that returns the actual type of object and replace all USEMETHODTOGETTYPE with a call to this method. (also find other places where we could use it)
            Type objectType = obj.GetType();
            if (obj is char) //special case because JSIL thinks a variable of type object containing a char contains a string.
            {
                objectType = typeof(char);
            }

            TypeInformation typeInformation = null;
            if (objectType != _type)
            {
                // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
                typeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(_type, defaultNamespace, _useXmlSerializerFormat);
            }

            // Add the root:
            List<XObject> xnodesForRoot = DataContractSerializer_Serialization.SerializeToXObjects(obj, _type, _knownTypes, _useXmlSerializerFormat, isRoot: true, isContainedInsideEnumerable: false, parentTypeInformation: typeInformation, nodeDefaultNamespaceIfAny: null);
            xdoc.Add(xnodesForRoot.First());
            string xml = xdoc.ToString(indentXml);
            // Add the header:
            if (!omitXmlDeclaration)
            {
                xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>" + Environment.NewLine + xml;
            }

            return xml;
        }

        public XDocument SerializeToXDocument(object obj)
        {
            XDocument xdoc = new XDocument();

            string defaultNamespace = DataContractSerializer_Helpers.GetDefaultNamespace(_type.Namespace, _useXmlSerializerFormat);

            //todo: USEMETHODTOGETTYPE: make a method that returns the actual type of object and replace all USEMETHODTOGETTYPE with a call to this method. (also find other places where we could use it)
            Type objectType = obj.GetType();
            if (obj is char) //special case because JSIL thinks a variable of type object containing a char contains a string.
            {
                objectType = typeof(char);
            }

            TypeInformation typeInformation = null;
            if (objectType != _type)
            {
                // Get the type information (namespace, etc.) by reading the DataContractAttribute and similar attributes, if present:
                typeInformation = DataContractSerializer_Helpers.GetTypeInformationByReadingAttributes(_type, defaultNamespace, _useXmlSerializerFormat);
            }

            // Add the root:
            List<XObject> xnodesForRoot = DataContractSerializer_Serialization.SerializeToXObjects(obj, _type, _knownTypes, _useXmlSerializerFormat, isRoot: true, isContainedInsideEnumerable: false, parentTypeInformation: typeInformation, nodeDefaultNamespaceIfAny: null);
            xdoc.Add(xnodesForRoot.First());
            return xdoc;
        }

        public object DeserializeFromString(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);
            XElement root = xdoc.Root;
            Type expectedType = this._type;
            Type actuallyExpectedType = DataContractSerializer_KnownTypes.GetCSharpTypeForNode(root, this._type, expectedType, _knownTypes, null, _useXmlSerializerFormat);
            object result = DataContractSerializer_Deserialization.DeserializeToCSharpObject(root.Nodes(), actuallyExpectedType, root, _knownTypes, ignoreErrors: false, useXmlSerializerFormat: _useXmlSerializerFormat);
            return result;
        }

        public object DeserializeFromXElement(XElement xElement)
        {
            Type expectedType = this._type;
            Type actuallyExpectedType = DataContractSerializer_KnownTypes.GetCSharpTypeForNode(xElement, this._type, expectedType, _knownTypes, null, _useXmlSerializerFormat);
            object result = DataContractSerializer_Deserialization.DeserializeToCSharpObject(xElement.Nodes(), actuallyExpectedType, xElement, _knownTypes, ignoreErrors: false, useXmlSerializerFormat: _useXmlSerializerFormat);
            return result;
        }
    }
#endif
}