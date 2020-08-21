

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

#if BRIDGE
using Bridge;
#else
using JSIL.Meta;
#endif

namespace System.Runtime.Serialization
{

    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document
    /// using a supplied data contract. This class cannot be inherited.
    /// </summary>

#if CSHTML5NETSTANDARD
    // already defined in .NET Standard we need another name for the class
    public partial class DataContractSerializer_CSHTML5Ver
#else
    public partial class DataContractSerializer
#endif
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

#if CSHTML5NETSTANDARD        
        public DataContractSerializer_CSHTML5Ver(Type type, bool useXmlSerializerFormat = false)
#else
        public DataContractSerializer(Type type, bool useXmlSerializerFormat = false)
#endif
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
#if CSHTML5NETSTANDARD
        public DataContractSerializer_CSHTML5Ver(Type type, IEnumerable<Type> knownTypes, bool useXmlSerializerFormat = false)
#else
        public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, bool useXmlSerializerFormat = false)
#endif
        {
            DataContractSerializer_ValueTypesHandler.EnsureInitialized();

            _type = type;
            _useXmlSerializerFormat = useXmlSerializerFormat;

            if (knownTypes != null)
            { 
                ((List<Type>)_knownTypes).AddRange(knownTypes);
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
#if CSHTML5NETSTANDARD
            string xml = xdoc.ToString(indentXml ? SaveOptions.None : SaveOptions.DisableFormatting);
#else
            string xml = xdoc.ToString(indentXml);
#endif
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
}