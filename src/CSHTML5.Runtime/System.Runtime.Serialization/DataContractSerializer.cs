
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// TODOBRIDGE: usefull using?
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Serializes and deserializes an instance of a type into an XML stream or document
    /// using a supplied data contract. This class cannot be inherited.
    /// </summary>
    public class DataContractSerializer
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
}
