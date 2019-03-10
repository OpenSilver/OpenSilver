
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    //todo: verify that the methods in this class are the same as those in .NET Framework.

    /// <summary>
    /// Serializes and deserializes objects into and from XML documents. The System.Xml.Serialization.XmlSerializer
    /// enables you to control how objects are encoded into XML.
    /// </summary>
    //[JSIL.Meta.JSStubOnly]
    public class XmlSerializer
    {
        //---------------------------------------------------------
        // Starting on Oct 6, 2017, the XmlSerializer was replaced
        // by the DataContractSerializer (in order to get rid of
        // SGen.exe), so we maintain backward compatibility by
        // "forwarding" the XmlSerializer calls to the
        // DataContractSerializer. However, the latter is defined
        // in an assembly that is not visible by this assembly,
        // therefore we use the static members below to have the
        // other assembly "inject" the DataContractSerializer.
        // Note that this "forwarding" only happens when running
        // in JavaScript, not in the Simulator, because in the
        // Simulator this assembly is replaced by the one provided
        // by the .NET Framework.
        //---------------------------------------------------------
        public static Func<object, Type, string> MethodToSerializeUsingDataContractSerializer { get; set; }
        public static Func<string, Type, object> MethodToDeserializeUsingDataContractSerializer { get; set; }


        Type _type;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        /// class.
        /// </summary>
        protected XmlSerializer()
        {
        }
       
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        /// class that can serialize objects of the specified type into XML documents,
        /// and deserialize XML documents into objects of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type of the object that this System.Xml.Serialization.XmlSerializer can
        /// serialize.
        /// </param>
        public XmlSerializer(Type type)
        {
            _type = type;
        }
        
        /// <summary>
        /// Gets a value that indicates whether this System.Xml.Serialization.XmlSerializer
        /// can deserialize a specified XML document.
        /// </summary>
        /// <param name="xmlReader">An System.Xml.XmlReader that points to the document to deserialize.</param>
        /// <returns>
        /// true if this System.Xml.Serialization.XmlSerializer can deserialize the object
        /// that the System.Xml.XmlReader points to; otherwise, false.
        /// </returns>
        public virtual bool CanDeserialize(XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }

        #region Deserialization

        /// <summary>
        /// Deserializes the XML document contained by the specified System.IO.Stream.
        /// </summary>
        /// <param name="stream">The System.IO.Stream that contains the XML document to deserialize.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public object Deserialize(Stream stream)
        {
            //---------------
            // We redirect this method to the one of the DataContractSerializer (read the note at the beginning of the XmlSerializer class to understand why).
            //---------------

            var reader = new StreamReader(stream);
            var serializedXml = reader.ReadToEnd();
            return Deserialize(serializedXml);
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during deserialization. The original exception is available
        //     using the System.Exception.InnerException property.
        /// <summary>
        /// Deserializes the XML document contained by the specified System.xml.XmlReader.
        /// </summary>
        /// <param name="xmlReader">The System.xml.XmlReader that contains the XML document to deserialize.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public object Deserialize(XmlReader xmlReader)
        {
            //---------------
            // We redirect this method to the one of the DataContractSerializer (read the note at the beginning of the XmlSerializer class to understand why).
            //---------------

#if !BRIDGE
            var domNode = JSIL.Verbatim.Expression("$0._domNode", xmlReader);
            var serializedXml = JSIL.Verbatim.Expression("(new XMLSerializer()).serializeToString($0)", domNode);
            //the following commented line causes the method to be untranslatable by JSIL for some reason (even though the exact same line in Showcase is properly translated by JSIL, see in DotNetForHtml5.ShowcaseApp.Page4_WCF.RefreshRestToDos())
            //var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedXml));
            byte[] bytes = Encoding.UTF8.GetBytes(serializedXml);
            var memoryStream = new MemoryStream(bytes);
            return Deserialize(serializedXml);
#else
            var domNode = Bridge.Script.Write<object>("$0._domNode", xmlReader);
            var serializedXml = Bridge.Script.Write<string>("(new XMLSerializer()).serializeToString($0)", domNode);
            byte[] bytes = Encoding.UTF8.GetBytes(serializedXml);
            var memoryStream = new MemoryStream(bytes);
            return Deserialize(serializedXml);
#endif
        }

        private object Deserialize(string serializedXml)
        {
            //---------------
            // We redirect this method to the one of the DataContractSerializer (read the note at the beginning of the XmlSerializer class to understand why).
            //---------------

            return MethodToDeserializeUsingDataContractSerializer(serializedXml, _type);
        }

        #endregion

        #region Serialization

        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a file
        /// using the specified System.IO.Stream.
        /// </summary>
        /// <param name="stream">The System.IO.Stream used to write the XML document.</param>
        /// <param name="o">The System.Object to serialize.</param>
        public void Serialize(Stream stream, object o)
        {
            //---------------
            // We redirect this method to the one of the DataContractSerializer (read the note at the beginning of the XmlSerializer class to understand why).
            //---------------

#if !BRIDGE
            JSIL.Verbatim.Expression("alert('The XmlSerializer has been replaced by the DataContractSerializer.')");
#else
            Bridge.Script.Write("alert('The XmlSerializer has been replaced by the DataContractSerializer.')");
#endif

            var serializedObject = MethodToSerializeUsingDataContractSerializer(o, _type);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(serializedObject);
                writer.Flush();
            }
        }
        
        // Exceptions:
        //   System.NotImplementedException:
        //     Any attempt is made to access the method when the method is not overridden
        //     in a descendant class.
        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a file
        /// using the specified System.Xml.Serialization.XmlSerializationWriter.
        /// </summary>
        /// <param name="o">The System.Object to serialize.</param>
        /// <param name="writer">
        /// The System.Xml.Serialization.XmlSerializationWriter used to write the XML
        /// document.
        /// </param>
        protected virtual void Serialize(object o, XmlSerializationWriter writer)
        {
            throw new NotImplementedException("Please use the DataContractSerializer instead of the XmlSerializer.");
        }

#if WORKINPROGRESS
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.TextWriter.
        //
        // Parameters:
        //   textWriter:
        //     The System.IO.TextWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        public void Serialize(TextWriter textWriter, object o)
        {
            throw new NotImplementedException("Please use the DataContractSerialiazer instead of the XmlSerializer.");
        }
#endif

        #endregion



        // Exceptions:
        //   System.NotImplementedException:
        //     Any attempt is made to access the method when the method is not overridden
        //     in a descendant class.
        /// <summary>
        /// Returns an object used to read the XML document to be serialized.
        /// </summary>
        /// <returns>An System.Xml.Serialization.XmlSerializationReader used to read the XML document.</returns>
        protected virtual XmlSerializationReader CreateReader()
        {
            throw new NotImplementedException();
        }
        
        // Exceptions:
        //   System.NotImplementedException:
        //     Any attempt is made to access the method when the method is not overridden
        //     in a descendant class.
        /// <summary>
        /// When overridden in a derived class, returns a writer used to serialize the
        /// object.
        /// </summary>
        /// <returns>
        /// An instance that implements the System.Xml.Serialization.XmlSerializationWriter
        /// class.
        /// </returns>
        protected virtual XmlSerializationWriter CreateWriter()
        {
            throw new NotImplementedException();
        }

        // Exceptions:
        //   System.NotImplementedException:
        //     Any attempt is made to access the method when the method is not overridden
        //     in a descendant class.
        /// <summary>
        /// Deserializes the XML document contained by the specified System.Xml.Serialization.XmlSerializationReader.
        /// </summary>
        /// <param name="reader">
        /// The System.Xml.Serialization.XmlSerializationReader that contains the XML
        /// document to deserialize.
        /// </param>
        /// <returns>The deserialized object.</returns>
        protected virtual object Deserialize(XmlSerializationReader reader)
        {
            throw new NotImplementedException();
        }


        #region Not supported stuff


        ////
        //// Summary:
        ////     Initializes an instance of the System.Xml.Serialization.XmlSerializer class
        ////     using an object that maps one type to another.
        ////
        //// Parameters:
        ////   xmlTypeMapping:
        ////     An System.Xml.Serialization.XmlTypeMapping that maps one type to another.
        //public XmlSerializer(XmlTypeMapping xmlTypeMapping);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML documents,
        ////     and deserialize XML documents into objects of the specified type. Specifies
        ////     the default namespace for all the XML elements.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   defaultNamespace:
        ////     The default namespace to use for all the XML elements.
        //public XmlSerializer(Type type, string defaultNamespace);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML documents,
        ////     and deserialize XML documents into object of a specified type. If a property
        ////     or field returns an array, the extraTypes parameter specifies objects that
        ////     can be inserted into the array.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   extraTypes:
        ////     A System.Type array of additional object types to serialize.
        //public XmlSerializer(Type type, Type[] extraTypes);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML documents,
        ////     and deserialize XML documents into objects of the specified type. Each object
        ////     to be serialized can itself contain instances of classes, which this overload
        ////     can override with other classes.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object to serialize.
        ////
        ////   overrides:
        ////     An System.Xml.Serialization.XmlAttributeOverrides.
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML documents,
        ////     and deserialize an XML document into object of the specified type. It also
        ////     specifies the class to use as the XML root element.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   root:
        ////     An System.Xml.Serialization.XmlRootAttribute that represents the XML root
        ////     element.
        //public XmlSerializer(Type type, XmlRootAttribute root);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of type System.Object into XML document
        ////     instances, and deserialize XML document instances into objects of type System.Object.
        ////     Each object to be serialized can itself contain instances of classes, which
        ////     this overload overrides with other classes. This overload also specifies
        ////     the default namespace for all the XML elements and the class to use as the
        ////     XML root element.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   overrides:
        ////     An System.Xml.Serialization.XmlAttributeOverrides that extends or overrides
        ////     the behavior of the class specified in the type parameter.
        ////
        ////   extraTypes:
        ////     A System.Type array of additional object types to serialize.
        ////
        ////   root:
        ////     An System.Xml.Serialization.XmlRootAttribute that defines the XML root element
        ////     properties.
        ////
        ////   defaultNamespace:
        ////     The default namespace of all XML elements in the XML document.
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of type System.Object into XML document
        ////     instances, and deserialize XML document instances into objects of type System.Object.
        ////     Each object to be serialized can itself contain instances of classes, which
        ////     this overload overrides with other classes. This overload also specifies
        ////     the default namespace for all the XML elements and the class to use as the
        ////     XML root element.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   overrides:
        ////     An System.Xml.Serialization.XmlAttributeOverrides that extends or overrides
        ////     the behavior of the class specified in the type parameter.
        ////
        ////   extraTypes:
        ////     A System.Type array of additional object types to serialize.
        ////
        ////   root:
        ////     An System.Xml.Serialization.XmlRootAttribute that defines the XML root element
        ////     properties.
        ////
        ////   defaultNamespace:
        ////     The default namespace of all XML elements in the XML document.
        ////
        ////   location:
        ////     The location of the types.
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location);
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML document
        ////     instances, and deserialize XML document instances into objects of the specified
        ////     type. This overload allows you to supply other types that can be encountered
        ////     during a serialization or deserialization operation, as well as a default
        ////     namespace for all XML elements, the class to use as the XML root element,
        ////     its location, and credentials required for access.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   overrides:
        ////     An System.Xml.Serialization.XmlAttributeOverrides that extends or overrides
        ////     the behavior of the class specified in the type parameter.
        ////
        ////   extraTypes:
        ////     A System.Type array of additional object types to serialize.
        ////
        ////   root:
        ////     An System.Xml.Serialization.XmlRootAttribute that defines the XML root element
        ////     properties.
        ////
        ////   defaultNamespace:
        ////     The default namespace of all XML elements in the XML document.
        ////
        ////   location:
        ////     The location of the types.
        ////
        ////   evidence:
        ////     An instance of the System.Security.Policy.Evidence class that contains credentials
        ////     required to access types.
        //[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. Please use a XmlSerializer constructor overload which does not take an Evidence parameter. See http://go2.microsoft.com/fwlink/?LinkId=131738 for more information.")]
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence);

        //
        // Summary:
        //     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        //     class that can serialize objects of the specified type into XML documents,
        //     and deserialize XML documents into objects of the specified type. Specifies
        //     the default namespace for all the XML elements.
        //
        // Parameters:
        //   type:
        //     The type of the object that this System.Xml.Serialization.XmlSerializer can
        //     serialize.
        //
        //   defaultNamespace:
        ////     The default namespace to use for all the XML elements.
        //public XmlSerializer(Type type, string defaultNamespace)
        //{
        //    throw new NotImplementedException();
        //}
        ////
        //// Summary:
        ////     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        ////     class that can serialize objects of the specified type into XML documents,
        ////     and deserialize XML documents into object of a specified type. If a property
        ////     or field returns an array, the extraTypes parameter specifies objects that
        ////     can be inserted into the array.
        ////
        //// Parameters:
        ////   type:
        ////     The type of the object that this System.Xml.Serialization.XmlSerializer can
        ////     serialize.
        ////
        ////   extraTypes:
        ////     A System.Type array of additional object types to serialize.
        //public XmlSerializer(Type type, Type[] extraTypes)
        //{
        //    throw new NotImplementedException();
        //}

        //
        // Summary:
        //     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        //     class that can serialize objects of the specified type into XML documents,
        //     and deserialize XML documents into objects of the specified type. Each object
        //     to be serialized can itself contain instances of classes, which this overload
        //     can override with other classes.
        //
        // Parameters:
        //   type:
        //     The type of the object to serialize.
        //
        //   overrides:
        //     An System.Xml.Serialization.XmlAttributeOverrides.
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides)
        //{
        //    throw new NotImplementedException();
        //}
        //
        // Summary:
        //     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        //     class that can serialize objects of the specified type into XML documents,
        //     and deserialize an XML document into object of the specified type. It also
        //     specifies the class to use as the XML root element.
        //
        // Parameters:
        //   type:
        //     The type of the object that this System.Xml.Serialization.XmlSerializer can
        //     serialize.
        //
        //   root:
        //     An System.Xml.Serialization.XmlRootAttribute that represents the XML root
        //     element.
        //public XmlSerializer(Type type, XmlRootAttribute root)
        //{
        //    throw new NotImplementedException();
        //}
        //
        // Summary:
        //     Initializes a new instance of the System.Xml.Serialization.XmlSerializer
        //     class that can serialize objects of type System.Object into XML document
        //     instances, and deserialize XML document instances into objects of type System.Object.
        //     Each object to be serialized can itself contain instances of classes, which
        //     this overload overrides with other classes. This overload also specifies
        //     the default namespace for all the XML elements and the class to use as the
        //     XML root element.
        //
        // Parameters:
        //   type:
        //     The type of the object that this System.Xml.Serialization.XmlSerializer can
        //     serialize.
        //
        //   overrides:
        //     An System.Xml.Serialization.XmlAttributeOverrides that extends or overrides
        //     the behavior of the class specified in the type parameter.
        //
        //   extraTypes:
        //     A System.Type array of additional object types to serialize.
        //
        //   root:
        //     An System.Xml.Serialization.XmlRootAttribute that defines the XML root element
        //     properties.
        //
        //   defaultNamespace:
        //     The default namespace of all XML elements in the XML document.
        //public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Not implemented in JSIL

        // //
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.IO.TextWriter.
        ////
        //// Parameters:
        ////   textWriter:
        ////     The System.IO.TextWriter used to write the XML document.
        ////
        ////   o:
        ////     The System.Object to serialize.
        //public void Serialize(TextWriter textWriter, object o);
        ////
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.Xml.XmlWriter.
        ////
        //// Parameters:
        ////   xmlWriter:
        ////     The System.xml.XmlWriter used to write the XML document.
        ////
        ////   o:
        ////     The System.Object to serialize.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during serialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public void Serialize(XmlWriter xmlWriter, object o);
        ////
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.IO.Streamthat references the specified namespaces.
        ////
        //// Parameters:
        ////   stream:
        ////     The System.IO.Stream used to write the XML document.
        ////
        ////   o:
        ////     The System.Object to serialize.
        ////
        ////   namespaces:
        ////     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during serialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces);
        ////
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.IO.TextWriter and references the specified namespaces.
        ////
        //// Parameters:
        ////   textWriter:
        ////     The System.IO.TextWriter used to write the XML document.
        ////
        ////   o:
        ////     The System.Object to serialize.
        ////
        ////   namespaces:
        ////     The System.Xml.Serialization.XmlSerializerNamespaces that contains namespaces
        ////     for the generated XML document.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during serialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public void Serialize(TextWriter textWriter, object o, XmlSerializerNamespaces namespaces);
        ////
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.Xml.XmlWriter and references the specified namespaces.
        ////
        //// Parameters:
        ////   xmlWriter:
        ////     The System.xml.XmlWriter used to write the XML document.
        ////
        ////   o:
        ////     The System.Object to serialize.
        ////
        ////   namespaces:
        ////     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during serialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces);
        ////
        //// Summary:
        ////     Serializes the specified object and writes the XML document to a file using
        ////     the specified System.Xml.XmlWriter and references the specified namespaces
        ////     and encoding style.
        ////
        //// Parameters:
        ////   xmlWriter:
        ////     The System.xml.XmlWriter used to write the XML document.
        ////
        ////   o:
        ////     The object to serialize.
        ////
        ////   namespaces:
        ////     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        ////
        ////   encodingStyle:
        ////     The encoding style of the serialized XML.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during serialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle);
        ////
        //// Summary:
        ////     Serializes the specified System.Object and writes the XML document to a file
        ////     using the specified System.Xml.XmlWriter, XML namespaces, and encoding.
        ////
        //// Parameters:
        ////   xmlWriter:
        ////     The System.Xml.XmlWriter used to write the XML document.
        ////
        ////   o:
        ////     The object to serialize.
        ////
        ////   namespaces:
        ////     An instance of the XmlSerializaerNamespaces that contains namespaces and
        ////     prefixes to use.
        ////
        ////   encodingStyle:
        ////     The encoding used in the document.
        ////
        ////   id:
        ////     For SOAP encoded messages, the base used to generate id attributes.
        //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id);


        //// Summary:
        ////     Gets a value that indicates whether this System.Xml.Serialization.XmlSerializer
        ////     can deserialize a specified XML document.
        ////
        //// Parameters:
        ////   xmlReader:
        ////     An System.Xml.XmlReader that points to the document to deserialize.
        ////
        //// Returns:
        ////     true if this System.Xml.Serialization.XmlSerializer can deserialize the object
        ////     that the System.Xml.XmlReader points to; otherwise, false.
        //public virtual bool CanDeserialize(XmlReader xmlReader);

        ////
        //// Summary:
        ////     Deserializes the XML document contained by the specified System.IO.TextReader.
        ////
        //// Parameters:
        ////   textReader:
        ////     The System.IO.TextReader that contains the XML document to deserialize.
        ////
        //// Returns:
        ////     The System.Object being deserialized.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during deserialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public object Deserialize(TextReader textReader);
        ////
        //// Summary:
        ////     Deserializes the XML document contained by the specified System.xml.XmlReader
        ////     and encoding style.
        ////
        //// Parameters:
        ////   xmlReader:
        ////     The System.xml.XmlReader that contains the XML document to deserialize.
        ////
        ////   encodingStyle:
        ////     The encoding style of the serialized XML.
        ////
        //// Returns:
        ////     The deserialized object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An error occurred during deserialization. The original exception is available
        ////     using the System.Exception.InnerException property.
        //public object Deserialize(XmlReader xmlReader, string encodingStyle);
        ////
        //// Summary:
        ////     Deserializes an XML document contained by the specified System.Xml.XmlReader
        ////     and allows the overriding of events that occur during deserialization.
        ////
        //// Parameters:
        ////   xmlReader:
        ////     The System.Xml.XmlReader that contains the document to deserialize.
        ////
        ////   events:
        ////     An instance of the System.Xml.Serialization.XmlDeserializationEvents class.
        ////
        //// Returns:
        ////     The System.Object being deserialized.
        //public object Deserialize(XmlReader xmlReader, XmlDeserializationEvents events);
        ////
        //// Summary:
        ////     Deserializes the object using the data contained by the specified System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   xmlReader:
        ////     An instance of the System.Xml.XmlReader class used to read the document.
        ////
        ////   encodingStyle:
        ////     The encoding used.
        ////
        ////   events:
        ////     An instance of the System.Xml.Serialization.XmlDeserializationEvents class.
        ////
        //// Returns:
        ////     The object being deserialized.
        //public object Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events);

        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        //// <summary>
        //// Serializes the specified System.Object and writes the XML document to a file
        //// using the specified System.Xml.XmlWriter.
        //// </summary>
        //// <param name="xmlWriter">The System.xml.XmlWriter used to write the XML document.</param>
        //// <param name="o">The System.Object to serialize.</param>
        //public void Serialize(XmlWriter xmlWriter, object o)
        //{
        //    throw new NotImplementedException();
        //}

        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        //// <summary>
        //// Serializes the specified System.Object and writes the XML document to a file
        //// using the specified System.Xml.XmlWriter and references the specified namespaces.
        //// </summary>
        //// <param name="xmlWriter">The System.xml.XmlWriter used to write the XML document.</param>
        //// <param name="o">The System.Object to serialize.</param>
        //// <param name="namespaces">The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.</param>
        //public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
        //{
        //    throw new NotImplementedException();
        //}

        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        //// <summary>
        //// Serializes the specified System.Object and writes the XML document to a file
        //// using the specified System.IO.Streamthat references the specified namespaces.
        //// </summary>
        //// <param name="stream">The System.IO.Stream used to write the XML document.</param>
        //// <param name="o">The System.Object to serialize.</param>
        //// <param name="namespaces">The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.</param>
        //public void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region other

        //// Summary:
        ////     Occurs when the System.Xml.Serialization.XmlSerializer encounters an XML
        ////     attribute of unknown type during deserialization.
        //public event XmlAttributeEventHandler UnknownAttribute;
        ////
        //// Summary:
        ////     Occurs when the System.Xml.Serialization.XmlSerializer encounters an XML
        ////     element of unknown type during deserialization.
        //public event XmlElementEventHandler UnknownElement;
        ////
        //// Summary:
        ////     Occurs when the System.Xml.Serialization.XmlSerializer encounters an XML
        ////     node of unknown type during deserialization.
        //public event XmlNodeEventHandler UnknownNode;
        ////
        //// Summary:
        ////     Occurs during deserialization of a SOAP-encoded XML stream, when the System.Xml.Serialization.XmlSerializer
        ////     encounters a recognized type that is not used or is unreferenced.
        //public event UnreferencedObjectEventHandler UnreferencedObject;


        ////
        //// Summary:
        ////     Returns an array of System.Xml.Serialization.XmlSerializer objects created
        ////     from an array of System.Xml.Serialization.XmlTypeMapping objects.
        ////
        //// Parameters:
        ////   mappings:
        ////     An array of System.Xml.Serialization.XmlTypeMapping that maps one type to
        ////     another.
        ////
        //// Returns:
        ////     An array of System.Xml.Serialization.XmlSerializer objects.
        //public static XmlSerializer[] FromMappings(XmlMapping[] mappings);
        ////
        //// Summary:
        ////     Returns an instance of the System.Xml.Serialization.XmlSerializer class created
        ////     from mappings of one XML type to another.
        ////
        //// Parameters:
        ////   mappings:
        ////     An array of System.Xml.Serialization.XmlMapping objects used to map one type
        ////     to another.
        ////
        ////   evidence:
        ////     An instance of the System.Security.Policy.Evidence class that contains host
        ////     and assembly data presented to the common language runtime policy system.
        ////
        //// Returns:
        ////     An instance of the System.Xml.Serialization.XmlSerializer class.
        //[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. Please use an overload of FromMappings which does not take an Evidence parameter. See http://go2.microsoft.com/fwlink/?LinkId=131738 for more information.")]
        //public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Evidence evidence);
        ////
        //// Summary:
        ////     Returns an instance of the System.Xml.Serialization.XmlSerializer class from
        ////     the specified mappings.
        ////
        //// Parameters:
        ////   mappings:
        ////     An array of System.Xml.Serialization.XmlMapping objects.
        ////
        ////   type:
        ////     The System.Type of the deserialized object.
        ////
        //// Returns:
        ////     An instance of the System.Xml.Serialization.XmlSerializer class.
        //public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Type type);
        ////
        //// Summary:
        ////     Returns an array of System.Xml.Serialization.XmlSerializer objects created
        ////     from an array of types.
        ////
        //// Parameters:
        ////   types:
        ////     An array of System.Type objects.
        ////
        //// Returns:
        ////     An array of System.Xml.Serialization.XmlSerializer objects.
        //public static XmlSerializer[] FromTypes(Type[] types);
        ////
        //// Summary:
        ////     Returns an assembly that contains custom-made serializers used to serialize
        ////     or deserialize the specified type or types, using the specified mappings.
        ////
        //// Parameters:
        ////   types:
        ////     A collection of types.
        ////
        ////   mappings:
        ////     A collection of System.Xml.Serialization.XmlMapping objects used to convert
        ////     one type to another.
        ////
        //// Returns:
        ////     An System.Reflection.Assembly object that contains serializers for the supplied
        ////     types and mappings.
        //public static Assembly GenerateSerializer(Type[] types, XmlMapping[] mappings);
        ////
        //// Summary:
        ////     Returns an assembly that contains custom-made serializers used to serialize
        ////     or deserialize the specified type or types, using the specified mappings
        ////     and compiler settings and options.
        ////
        //// Parameters:
        ////   types:
        ////     An array of type System.Type that contains objects used to serialize and
        ////     deserialize data.
        ////
        ////   mappings:
        ////     An array of type System.Xml.Serialization.XmlMapping that maps the XML data
        ////     to the type data.
        ////
        ////   parameters:
        ////     An instance of the System.CodeDom.Compiler.CompilerParameters class that
        ////     represents the parameters used to invoke a compiler.
        ////
        //// Returns:
        ////     An System.Reflection.Assembly that contains special versions of the System.Xml.Serialization.XmlSerializer.
        //public static Assembly GenerateSerializer(Type[] types, XmlMapping[] mappings, CompilerParameters parameters);
        ////
        //// Summary:
        ////     Returns the name of the assembly that contains one or more versions of the
        ////     System.Xml.Serialization.XmlSerializer especially created to serialize or
        ////     deserialize the specified type.
        ////
        //// Parameters:
        ////   type:
        ////     The System.Type you are deserializing.
        ////
        //// Returns:
        ////     The name of the assembly that contains an System.Xml.Serialization.XmlSerializer
        ////     for the type.
        //public static string GetXmlSerializerAssemblyName(Type type);
        ////
        //// Summary:
        ////     Returns the name of the assembly that contains the serializer for the specified
        ////     type in the specified namespace.
        ////
        //// Parameters:
        ////   type:
        ////     The System.Type you are interested in.
        ////
        ////   defaultNamespace:
        ////     The namespace of the type.
        ////
        //// Returns:
        ////     The name of the assembly that contains specially built serializers.
        //public static string GetXmlSerializerAssemblyName(Type type, string defaultNamespace);

        #endregion
    }



}

