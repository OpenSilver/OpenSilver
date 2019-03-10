
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
    /// <summary>
    /// Represents a reader that provides fast, noncached, forward-only access to
    /// XML data.
    /// </summary>
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public abstract class XmlReader : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the XmlReader class.
        /// </summary>
        protected XmlReader()
        {

        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the number of attributes on the
        /// current node.
        /// </summary>
        public abstract int AttributeCount { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the base URI of the current node.
        ////
        //// Returns:
        ////     The base URI of the current node.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract string BaseURI { get; }
        ////
        //// Summary:
        ////     Gets a value indicating whether the System.Xml.XmlReader implements the binary
        ////     content read methods.
        ////
        //// Returns:
        ////     true if the binary content read methods are implemented; otherwise false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool CanReadBinaryContent { get; }
        ////
        //// Summary:
        ////     Gets a value indicating whether the System.Xml.XmlReader implements the System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)
        ////     method.
        ////
        //// Returns:
        ////     true if the System.Xml.XmlReader implements the System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)
        ////     method; otherwise false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool CanReadValueChunk { get; }
        ////
        //// Summary:
        ////     Gets a value indicating whether this reader can parse and resolve entities.
        ////
        //// Returns:
        ////     true if the reader can parse and resolve entities; otherwise, false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool CanResolveEntity { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the depth of the current node in
        ////     the XML document.
        ////
        //// Returns:
        ////     The depth of the current node in the XML document.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract int Depth { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets a value indicating whether the reader
        ////     is positioned at the end of the stream.
        ////
        //// Returns:
        ////     true if the reader is positioned at the end of the stream; otherwise, false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract bool EOF { get; }
        ////
        //// Summary:
        ////     Gets a value indicating whether the current node has any attributes.
        ////
        //// Returns:
        ////     true if the current node has attributes; otherwise, false.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool HasAttributes { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets a value indicating whether the current
        ////     node can have a System.Xml.XmlReader.Value.
        ////
        //// Returns:
        ////     true if the node on which the reader is currently positioned can have a Value;
        ////     otherwise, false. If false, the node has a value of String.Empty.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool HasValue { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets a value indicating whether the current
        ////     node is an attribute that was generated from the default value defined in
        ////     the DTD or schema.
        ////
        //// Returns:
        ////     true if the current node is an attribute whose value was generated from the
        ////     default value defined in the DTD or schema; false if the attribute value
        ////     was explicitly set.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool IsDefault { get; }
        
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current
        /// node is an empty element (for example, <MyElement/>).
        /// </summary>
        public abstract bool IsEmptyElement { get; }
        
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the local name of the current node.
        /// </summary>
        public abstract string LocalName { get; }
        
        // Returns:
        //     The qualified name of the current node. For example, Name is bk:book for
        //     the element <bk:book>.The name returned is dependent on the System.Xml.XmlReader.NodeType
        //     of the node. The following node types return the listed values. All other
        //     node types return an empty string.Node type Name AttributeThe name of the
        //     attribute. DocumentTypeThe document type name. ElementThe tag name. EntityReferenceThe
        //     name of the entity referenced. ProcessingInstructionThe target of the processing
        //     instruction. XmlDeclarationThe literal string xml.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the qualified name of the current
        /// node.
        /// </summary>
        public virtual string Name
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        
        // Returns:
        //     The namespace URI of the current node; otherwise an empty string.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the namespace URI (as defined in
        /// the W3C Namespace specification) of the node on which the reader is positioned.
        /// </summary>
        public abstract string NamespaceURI { get; }
       
        // Returns:
        //     The XmlNameTable enabling you to get the atomized version of a string within
        //     the node.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the System.Xml.XmlNameTable associated
        /// with this implementation.
        /// </summary>
        public abstract XmlNameTable NameTable { get; }
        
        // Returns:
        //     One of the System.Xml.XmlNodeType values representing the type of the current
        //     node.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the type of the current node.
        /// </summary>
        public abstract XmlNodeType NodeType { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the namespace prefix associated
        ////     with the current node.
        ////
        //// Returns:
        ////     The namespace prefix associated with the current node.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract string Prefix { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the quotation mark character used
        ////     to enclose the value of an attribute node.
        ////
        //// Returns:
        ////     The quotation mark character (" or ') used to enclose the value of an attribute
        ////     node.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual char QuoteChar { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the state of the reader.
        ////
        //// Returns:
        ////     One of the System.Xml.ReadState values.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract ReadState ReadState { get; }
        ////
        //// Summary:
        ////     Gets the schema information that has been assigned to the current node as
        ////     a result of schema validation.
        ////
        //// Returns:
        ////     An System.Xml.Schema.IXmlSchemaInfo object containing the schema information
        ////     for the current node. Schema information can be set on elements, attributes,
        ////     or on text nodes with a non-null System.Xml.XmlReader.ValueType (typed values).If
        ////     the current node is not one of the above node types, or if the XmlReader
        ////     instance does not report schema information, this property returns null.If
        ////     this property is called from an System.Xml.XmlTextReader or an System.Xml.XmlValidatingReader
        ////     object, this property always returns null. These XmlReader implementations
        ////     do not expose schema information through the SchemaInfo property.NoteIf you
        ////     have to get the post-schema-validation information set (PSVI) for an element,
        ////     position the reader on the end tag of the element, rather than on the start
        ////     tag. You get the PSVI through the SchemaInfo property of a reader. The validating
        ////     reader that is created through Overload:System.Xml.XmlReader.Create with
        ////     the System.Xml.XmlReaderSettings.ValidationType property set to System.Xml.ValidationType.Schema
        ////     has complete PSVI for an element only when the reader is positioned on the
        ////     end tag of an element.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual IXmlSchemaInfo SchemaInfo { get; }
        ////
        //// Summary:
        ////     Gets the System.Xml.XmlReaderSettings object used to create this System.Xml.XmlReader
        ////     instance.
        ////
        //// Returns:
        ////     The System.Xml.XmlReaderSettings object used to create this reader instance.
        ////     If this reader was not created using the Overload:System.Xml.XmlReader.Create
        ////     method, this property returns null.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual XmlReaderSettings Settings { get; }
        
        // Returns:
        //     The value returned depends on the System.Xml.XmlReader.NodeType of the node.
        //     The following table lists node types that have a value to return. All other
        //     node types return String.Empty.Node type Value AttributeThe value of the
        //     attribute. CDATAThe content of the CDATA section. CommentThe content of the
        //     comment. DocumentTypeThe internal subset. ProcessingInstructionThe entire
        //     content, excluding the target. SignificantWhitespaceThe white space between
        //     markup in a mixed content model. TextThe content of the text node. WhitespaceThe
        //     white space between markup. XmlDeclarationThe content of the declaration.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, gets the text value of the current node.
        /// </summary>
        public abstract string Value { get; }
        ////
        //// Summary:
        ////     Gets The Common Language Runtime (CLR) type for the current node.
        ////
        //// Returns:
        ////     The CLR type that corresponds to the typed value of the node. The default
        ////     is System.String.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual Type ValueType { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the current xml:lang scope.
        ////
        //// Returns:
        ////     The current xml:lang scope.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string XmlLang { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the current xml:space scope.
        ////
        //// Returns:
        ////     One of the System.Xml.XmlSpace values. If no xml:space scope exists, this
        ////     property defaults to XmlSpace.None.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual XmlSpace XmlSpace { get; }

        //// Summary:
        ////     When overridden in a derived class, gets the value of the attribute with
        ////     the specified index.
        ////
        //// Parameters:
        ////   i:
        ////     The index of the attribute.
        ////
        //// Returns:
        ////     The value of the specified attribute.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string this[int i] { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the value of the attribute with
        ////     the specified System.Xml.XmlReader.Name.
        ////
        //// Parameters:
        ////   name:
        ////     The qualified name of the attribute.
        ////
        //// Returns:
        ////     The value of the specified attribute. If the attribute is not found, null
        ////     is returned.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string this[string name] { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the value of the attribute with
        ////     the specified System.Xml.XmlReader.LocalName and System.Xml.XmlReader.NamespaceURI.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the attribute.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the attribute.
        ////
        //// Returns:
        ////     The value of the specified attribute. If the attribute is not found, null
        ////     is returned.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string this[string name, string namespaceURI] { get; }

        //// Summary:
        ////     When overridden in a derived class, changes the System.Xml.XmlReader.ReadState
        ////     to Closed.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void Close();
     
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        //
        //   System.Security.SecurityException:
        //     The System.Xml.XmlReader does not have sufficient permissions to access the
        //     location of the XML data.
        /// <summary>
        /// Creates a new System.Xml.XmlReader instance using the specified stream.
        /// </summary>
        /// <param name="input">
        /// The stream containing the XML data.The System.Xml.XmlReader scans the first
        /// bytes of the stream looking for a byte order mark or other sign of encoding.
        /// When encoding is determined, the encoding is used to continue reading the
        /// stream, and processing continues parsing the input as a stream of (Unicode)
        /// characters.
        /// </param>
        /// <returns>An System.Xml.XmlReader object used to read the data contained in the stream.</returns>
        public static XmlReader Create(Stream input)
        {
#if BRIDGE
            return Cshtml5_XmlReader.Create(input);
#else
            throw new NotImplementedException();
#endif
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     The inputUri value is null.
        //
        //   System.Security.SecurityException:
        //     The System.Xml.XmlReader does not have sufficient permissions to access the
        //     location of the XML data.
        //
        //   System.IO.FileNotFoundException:
        //     The file identified by the URI does not exist.
        //
        //   System.UriFormatException:
        //     The URI format is not correct.
        /// <summary>
        /// Creates a new System.Xml.XmlReader instance with specified URI.
        /// </summary>
        /// <param name="inputUri">
        /// The URI for the file containing the XML data. The System.Xml.XmlUrlResolver
        /// class is used to convert the path to a canonical data representation.
        /// </param>
        /// <returns>An System.Xml.XmlReader object to read the XML data.</returns>
        public static XmlReader Create(string inputUri)
        {
            throw new NotImplementedException();
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        /// <summary>
        /// Creates a new System.Xml.XmlReader instance with the specified System.IO.TextReader.
        /// </summary>
        /// <param name="input">
        /// The System.IO.TextReader from which to read the XML data. Because a System.IO.TextReader
        /// returns a stream of Unicode characters, the encoding specified in the XML
        /// declaration is not used by the System.Xml.XmlReader to decode the data stream.
        /// </param>
        /// <returns>An System.Xml.XmlReader object to read the XML data.</returns>
        public static XmlReader Create(TextReader input)
        {
            throw new NotImplementedException();
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        /// <summary>
        /// Creates a new System.Xml.XmlReader instance with the specified stream and
        /// System.Xml.XmlReaderSettings object.
        /// </summary>
        /// <param name="input">
        /// The stream containing the XML data.The System.Xml.XmlReader scans the first
        /// bytes of the stream looking for a byte order mark or other sign of encoding.
        /// When encoding is determined, the encoding is used to continue reading the
        /// stream, and processing continues parsing the input as a stream of (Unicode)
        /// characters.
        /// </param>
        /// <param name="settings">
        /// The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        /// instance. This value can be null.
        /// </param>
        /// <returns>An System.Xml.XmlReader object to read the XML data.</returns>
        public static XmlReader Create(Stream input, XmlReaderSettings settings)
        {
            throw new NotImplementedException();
        }
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     The inputUri value is null.
        //
        //   System.IO.FileNotFoundException:
        //     The file specified by the URI cannot be found.
        //
        //   System.UriFormatException:
        //     The URI format is not correct.
        /// <summary>
        /// Creates a new instance with the specified URI and System.Xml.XmlReaderSettings.
        /// </summary>
        /// <param name="inputUri">
        /// The URI for the file containing the XML data. The System.Xml.XmlResolver
        /// object on the System.Xml.XmlReaderSettings object is used to convert the
        /// path to a canonical data representation. If System.Xml.XmlReaderSettings.XmlResolver
        /// is null, a new System.Xml.XmlUrlResolver object is used.
        /// </param>
        /// <param name="settings">
        /// The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        /// instance. This value can be null.
        /// </param>
        /// <returns>An System.Xml.XmlReader object to read XML data.</returns>
        public static XmlReader Create(string inputUri, XmlReaderSettings settings)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified System.IO.TextReader
        //     and System.Xml.XmlReaderSettings objects.
        //
        // Parameters:
        //   input:
        //     The System.IO.TextReader from which to read the XML data. Because a System.IO.TextReader
        //     returns a stream of Unicode characters, the encoding specified in the XML
        //     declaration is not used by the System.Xml.XmlReader to decode the data stream
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader.
        //     This value can be null.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        public static XmlReader Create(TextReader input, XmlReaderSettings settings)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance with the specified System.Xml.XmlReader
        //     and System.Xml.XmlReaderSettings objects.
        //
        // Parameters:
        //   reader:
        //     The System.Xml.XmlReader object that you wish to use as the underlying reader.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance.The conformance level of the System.Xml.XmlReaderSettings object
        //     must either match the conformance level of the underlying reader, or it must
        //     be set to System.Xml.ConformanceLevel.Auto.
        //
        // Returns:
        //     An System.Xml.XmlReader object that is wrapped around the specified System.Xml.XmlReader
        //     object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The reader value is null.
        //
        //   System.InvalidOperationException:
        //     If the System.Xml.XmlReaderSettings object specifies a conformance level
        //     that is not consistent with conformance level of the underlying reader.-or-The
        //     underlying System.Xml.XmlReader is in an System.Xml.ReadState.Error or System.Xml.ReadState.Closed
        //     state.
        public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified stream, base
        //     URI, and System.Xml.XmlReaderSettings object.
        //
        // Parameters:
        //   input:
        //     The stream containing the XML data. The System.Xml.XmlReader scans the first
        //     bytes of the stream looking for a byte order mark or other sign of encoding.
        //     When encoding is determined, the encoding is used to continue reading the
        //     stream, and processing continues parsing the input as a stream of (Unicode)
        //     characters.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance. This value can be null.
        //
        //   baseUri:
        //     The base URI for the entity or document being read. This value can be null.Security
        //     Note   The base URI is used to resolve the relative URI of the XML document.
        //     Do not use a base URI from an untrusted source.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        public static XmlReader Create(Stream input, XmlReaderSettings settings, string baseUri)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified stream, System.Xml.XmlReaderSettings,
        //     and System.Xml.XmlParserContext objects.
        //
        // Parameters:
        //   input:
        //     The stream containing the XML data. The System.Xml.XmlReader scans the first
        //     bytes of the stream looking for a byte order mark or other sign of encoding.
        //     When encoding is determined, the encoding is used to continue reading the
        //     stream, and processing continues parsing the input as a stream of (Unicode)
        //     characters.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance. This value can be null.
        //
        //   inputContext:
        //     The System.Xml.XmlParserContext object that provides the context information
        //     required to parse the XML fragment. The context information can include the
        //     System.Xml.XmlNameTable to use, encoding, namespace scope, the current xml:lang
        //     and xml:space scope, base URI, and document type definition. This value can
        //     be null.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        public static XmlReader Create(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified URI, System.Xml.XmlReaderSettings,
        //     and System.Xml.XmlParserContext objects.
        //
        // Parameters:
        //   inputUri:
        //     The URI for the file containing the XML data. The System.Xml.XmlResolver
        //     object on the System.Xml.XmlReaderSettings object is used to convert the
        //     path to a canonical data representation. If System.Xml.XmlReaderSettings.XmlResolver
        //     is null, a new System.Xml.XmlUrlResolver object is used.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance. This value can be null.
        //
        //   inputContext:
        //     The System.Xml.XmlParserContext object that provides the context information
        //     required to parse the XML fragment. The context information can include the
        //     System.Xml.XmlNameTable to use, encoding, namespace scope, the current xml:lang
        //     and xml:space scope, base URI, and document type definition. This value can
        //     be null.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The inputUri value is null.
        //
        //   System.Security.SecurityException:
        //     The System.Xml.XmlReader does not have sufficient permissions to access the
        //     location of the XML data.
        //
        //   System.ArgumentException:
        //     The System.Xml.XmlReaderSettings.NameTable and System.Xml.XmlParserContext.NameTable
        //     properties both contain values. (Only one of these NameTable properties can
        //     be set and used).
        //
        //   System.IO.FileNotFoundException:
        //     The file specified by the URI cannot be found.
        //
        //   System.UriFormatException:
        //     The URI format is not correct.
        public static XmlReader Create(string inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified System.IO.TextReader,
        //     System.Xml.XmlReaderSettings, and base URI.
        //
        // Parameters:
        //   input:
        //     The System.IO.TextReader from which to read the XML data. Because a System.IO.TextReader
        //     returns a stream of Unicode characters, the encoding specified in the XML
        //     declaration is not used by the System.Xml.XmlReader to decode the data stream.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance. This value can be null.
        //
        //   baseUri:
        //     The base URI for the entity or document being read. This value can be null.Security
        //     Note   The base URI is used to resolve the relative URI of the XML document.
        //     Do not use a base URI from an untrusted source.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        public static XmlReader Create(TextReader input, XmlReaderSettings settings, string baseUri)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Creates a new System.Xml.XmlReader instance using the specified System.IO.TextReader,
        //     System.Xml.XmlReaderSettings, and System.Xml.XmlParserContext objects.
        //
        // Parameters:
        //   input:
        //     The System.IO.TextReader from which to read the XML data. Because a System.IO.TextReader
        //     returns a stream of Unicode characters, the encoding specified in the XML
        //     declaration is not used by the System.Xml.XmlReader to decode the data stream.
        //
        //   settings:
        //     The System.Xml.XmlReaderSettings object used to configure the new System.Xml.XmlReader
        //     instance. This value can be null.
        //
        //   inputContext:
        //     The System.Xml.XmlParserContext object that provides the context information
        //     required to parse the XML fragment. The context information can include the
        //     System.Xml.XmlNameTable to use, encoding, namespace scope, the current xml:lang
        //     and xml:space scope, base URI, and document type definition.This value can
        //     be null.
        //
        // Returns:
        //     An System.Xml.XmlReader object to read XML data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The input value is null.
        //
        //   System.ArgumentException:
        //     The System.Xml.XmlReaderSettings.NameTable and System.Xml.XmlParserContext.NameTable
        //     properties both contain values. (Only one of these NameTable properties can
        //     be set and used).
        public static XmlReader Create(TextReader input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Releases all resources used by the current instance of the System.Xml.XmlReader
        //     class.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Releases the unmanaged resources used by the System.Xml.XmlReader and optionally
        //     releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only
        //     unmanaged resources.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        protected virtual void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     When overridden in a derived class, gets the value of the attribute with
        //     the specified index.
        //
        // Parameters:
        //   i:
        //     The index of the attribute. The index is zero-based. (The first attribute
        //     has index 0.)
        //
        // Returns:
        //     The value of the specified attribute. This method does not move the reader.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     i is out of range. It must be non-negative and less than the size of the
        //     attribute collection.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract string GetAttribute(int i);
        //
        // Summary:
        //     When overridden in a derived class, gets the value of the attribute with
        //     the specified System.Xml.XmlReader.Name.
        //
        // Parameters:
        //   name:
        //     The qualified name of the attribute.
        //
        // Returns:
        //     The value of the specified attribute. If the attribute is not found or the
        //     value is String.Empty, null is returned.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     name is null.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract string GetAttribute(string name);
        //
        // Summary:
        //     When overridden in a derived class, gets the value of the attribute with
        //     the specified System.Xml.XmlReader.LocalName and System.Xml.XmlReader.NamespaceURI.
        //
        // Parameters:
        //   name:
        //     The local name of the attribute.
        //
        //   namespaceURI:
        //     The namespace URI of the attribute.
        //
        // Returns:
        //     The value of the specified attribute. If the attribute is not found or the
        //     value is String.Empty, null is returned. This method does not move the reader.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     name is null.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract string GetAttribute(string name, string namespaceURI);
        ////
        //// Summary:
        ////     Asynchronously gets the value of the current node.
        ////
        //// Returns:
        ////     The value of the current node.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<string> GetValueAsync();
        ////
        //// Summary:
        ////     Returns a value indicating whether the string argument is a valid XML name.
        ////
        //// Parameters:
        ////   str:
        ////     The name to validate.
        ////
        //// Returns:
        ////     true if the name is valid; otherwise, false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The str value is null.
        //public static bool IsName(string str);
        ////
        //// Summary:
        ////     Returns a value indicating whether or not the string argument is a valid
        ////     XML name token.
        ////
        //// Parameters:
        ////   str:
        ////     The name token to validate.
        ////
        //// Returns:
        ////     true if it is a valid name token; otherwise false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The str value is null.
        //public static bool IsNameToken(string str);
        ////
        //// Summary:
        ////     Calls System.Xml.XmlReader.MoveToContent() and tests if the current content
        ////     node is a start tag or empty element tag.
        ////
        //// Returns:
        ////     true if System.Xml.XmlReader.MoveToContent() finds a start tag or empty element
        ////     tag; false if a node type other than XmlNodeType.Element was found.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     Incorrect XML is encountered in the input stream.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        public virtual bool IsStartElement()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Calls System.Xml.XmlReader.MoveToContent() and tests if the current content
        //     node is a start tag or empty element tag and if the System.Xml.XmlReader.Name
        //     property of the element found matches the given argument.
        //
        // Parameters:
        //   name:
        //     The string matched against the Name property of the element found.
        //
        // Returns:
        //     true if the resulting node is an element and the Name property matches the
        //     specified string. false if a node type other than XmlNodeType.Element was
        //     found or if the element Name property does not match the specified string.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML is encountered in the input stream.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual bool IsStartElement(string name)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Calls System.Xml.XmlReader.MoveToContent() and tests if the current content
        //     node is a start tag or empty element tag and if the System.Xml.XmlReader.LocalName
        //     and System.Xml.XmlReader.NamespaceURI properties of the element found match
        //     the given strings.
        //
        // Parameters:
        //   localname:
        //     The string to match against the LocalName property of the element found.
        //
        //   ns:
        //     The string to match against the NamespaceURI property of the element found.
        //
        // Returns:
        //     true if the resulting node is an element. false if a node type other than
        //     XmlNodeType.Element was found or if the LocalName and NamespaceURI properties
        //     of the element do not match the specified strings.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML is encountered in the input stream.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual bool IsStartElement(string localname, string ns)
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     When overridden in a derived class, resolves a namespace prefix in the current
        ////     element's scope.
        ////
        //// Parameters:
        ////   prefix:
        ////     The prefix whose namespace URI you want to resolve. To match the default
        ////     namespace, pass an empty string.
        ////
        //// Returns:
        ////     The namespace URI to which the prefix maps or null if no matching prefix
        ////     is found.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract string LookupNamespace(string prefix);
        ////
        //// Summary:
        ////     When overridden in a derived class, moves to the attribute with the specified
        ////     index.
        ////
        //// Parameters:
        ////   i:
        ////     The index of the attribute.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The parameter has a negative value.
        //public virtual void MoveToAttribute(int i);
        ////
        //// Summary:
        ////     When overridden in a derived class, moves to the attribute with the specified
        ////     System.Xml.XmlReader.Name.
        ////
        //// Parameters:
        ////   name:
        ////     The qualified name of the attribute.
        ////
        //// Returns:
        ////     true if the attribute is found; otherwise, false. If false, the reader's
        ////     position does not change.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.ArgumentException:
        ////     The parameter is an empty string.
        //public abstract bool MoveToAttribute(string name);
        ////
        //// Summary:
        ////     When overridden in a derived class, moves to the attribute with the specified
        ////     System.Xml.XmlReader.LocalName and System.Xml.XmlReader.NamespaceURI.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI of the attribute.
        ////
        //// Returns:
        ////     true if the attribute is found; otherwise, false. If false, the reader's
        ////     position does not change.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.ArgumentNullException:
        ////     Both parameter values are null.
        //public abstract bool MoveToAttribute(string name, string ns);
        //
        // Summary:
        //     Checks whether the current node is a content (non-white space text, CDATA,
        //     Element, EndElement, EntityReference, or EndEntity) node. If the node is
        //     not a content node, the reader skips ahead to the next content node or end
        //     of file. It skips over nodes of the following type: ProcessingInstruction,
        //     DocumentType, Comment, Whitespace, or SignificantWhitespace.
        //
        // Returns:
        //     The System.Xml.XmlReader.NodeType of the current node found by the method
        //     or XmlNodeType.None if the reader has reached the end of the input stream.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML encountered in the input stream.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual XmlNodeType MoveToContent()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Asynchronously checks whether the current node is a content node. If the
        ////     node is not a content node, the reader skips ahead to the next content node
        ////     or end of file.
        ////
        //// Returns:
        ////     The System.Xml.XmlReader.NodeType of the current node found by the method
        ////     or XmlNodeType.None if the reader has reached the end of the input stream.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<XmlNodeType> MoveToContentAsync();
        //
        // Summary:
        //     When overridden in a derived class, moves to the element that contains the
        //     current attribute node.
        //
        // Returns:
        //     true if the reader is positioned on an attribute (the reader moves to the
        //     element that owns the attribute); false if the reader is not positioned on
        //     an attribute (the position of the reader does not change).
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract bool MoveToElement();
        //
        // Summary:
        //     When overridden in a derived class, moves to the first attribute.
        //
        // Returns:
        //     true if an attribute exists (the reader moves to the first attribute); otherwise,
        //     false (the position of the reader does not change).
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract bool MoveToFirstAttribute();
        //
        // Summary:
        //     When overridden in a derived class, moves to the next attribute.
        //
        // Returns:
        //     true if there is a next attribute; false if there are no more attributes.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract bool MoveToNextAttribute();
        //
        // Summary:
        //     When overridden in a derived class, reads the next node from the stream.
        //
        // Returns:
        //     true if the next node was read successfully; false if there are no more nodes
        //     to read.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     An error occurred while parsing the XML.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public abstract bool Read();
        ////
        //// Summary:
        ////     Asynchronously reads the next node from the stream.
        ////
        //// Returns:
        ////     true if the next node was read successfully; false if there are no more nodes
        ////     to read.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<bool> ReadAsync();
        ////
        //// Summary:
        ////     When overridden in a derived class, parses the attribute value into one or
        ////     more Text, EntityReference, or EndEntity nodes.
        ////
        //// Returns:
        ////     true if there are nodes to return.false if the reader is not positioned on
        ////     an attribute node when the initial call is made or if all the attribute values
        ////     have been read.An empty attribute, such as, misc="", returns true with a
        ////     single node with a value of String.Empty.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract bool ReadAttributeValue();
        ////
        //// Summary:
        ////     Reads the content as an object of the type specified.
        ////
        //// Parameters:
        ////   returnType:
        ////     The type of the value to be returned.Note   With the release of the .NET
        ////     Framework 3.5, the value of the returnType parameter can now be the System.DateTimeOffset
        ////     type.
        ////
        ////   namespaceResolver:
        ////     An System.Xml.IXmlNamespaceResolver object that is used to resolve any namespace
        ////     prefixes related to type conversion. For example, this can be used when converting
        ////     an System.Xml.XmlQualifiedName object to an xs:string.This value can be null.
        ////
        //// Returns:
        ////     The concatenated text content or attribute value converted to the requested
        ////     type.
        ////
        //// Exceptions:
        ////   System.FormatException:
        ////     The content is not in the correct format for the target type.
        ////
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.ArgumentNullException:
        ////     The returnType value is null.
        ////
        ////   System.InvalidOperationException:
        ////     The current node is not a supported node type. See the table below for details.
        ////
        ////   System.OverflowException:
        ////     Read Decimal.MaxValue.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver);
        ////
        //// Summary:
        ////     Asynchronously reads the content as an object of the type specified.
        ////
        //// Parameters:
        ////   returnType:
        ////     The type of the value to be returned.
        ////
        ////   namespaceResolver:
        ////     An System.Xml.IXmlNamespaceResolver object that is used to resolve any namespace
        ////     prefixes related to type conversion.
        ////
        //// Returns:
        ////     The concatenated text content or attribute value converted to the requested
        ////     type.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver);
        ////
        //// Summary:
        ////     Reads the content and returns the Base64 decoded binary bytes.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The buffer value is null.
        ////
        ////   System.InvalidOperationException:
        ////     System.Xml.XmlReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)
        ////     is not supported on the current node.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The index into the buffer or index + count is larger than the allocated buffer
        ////     size.
        ////
        ////   System.NotSupportedException:
        ////     The System.Xml.XmlReader implementation does not support this method.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadContentAsBase64(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously reads the content and returns the Base64 decoded binary bytes.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Reads the content and returns the BinHex decoded binary bytes.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The buffer value is null.
        ////
        ////   System.InvalidOperationException:
        ////     System.Xml.XmlReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)
        ////     is not supported on the current node.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The index into the buffer or index + count is larger than the allocated buffer
        ////     size.
        ////
        ////   System.NotSupportedException:
        ////     The System.Xml.XmlReader implementation does not support this method.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously reads the content and returns the BinHex decoded binary bytes.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Reads the text content at the current position as a Boolean.
        ////
        //// Returns:
        ////     The text content as a System.Boolean object.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool ReadContentAsBoolean();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a System.DateTime object.
        ////
        //// Returns:
        ////     The text content as a System.DateTime object.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual DateTime ReadContentAsDateTime();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a System.DateTimeOffset
        ////     object.
        ////
        //// Returns:
        ////     The text content as a System.DateTimeOffset object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual DateTimeOffset ReadContentAsDateTimeOffset();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a System.Decimal object.
        ////
        //// Returns:
        ////     The text content at the current position as a System.Decimal object.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual decimal ReadContentAsDecimal();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a double-precision floating-point
        ////     number.
        ////
        //// Returns:
        ////     The text content as a double-precision floating-point number.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual double ReadContentAsDouble();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a single-precision floating
        ////     point number.
        ////
        //// Returns:
        ////     The text content at the current position as a single-precision floating point
        ////     number.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual float ReadContentAsFloat();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a 32-bit signed integer.
        ////
        //// Returns:
        ////     The text content as a 32-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadContentAsInt();
        ////
        //// Summary:
        ////     Reads the text content at the current position as a 64-bit signed integer.
        ////
        //// Returns:
        ////     The text content as a 64-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual long ReadContentAsLong();
        ////
        //// Summary:
        ////     Reads the text content at the current position as an System.Object.
        ////
        //// Returns:
        ////     The text content as the most appropriate common language runtime (CLR) object.
        ////
        //// Exceptions:
        ////   System.InvalidCastException:
        ////     The attempted cast is not valid.
        ////
        ////   System.FormatException:
        ////     The string format is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadContentAsObject();
        ////
        //// Summary:
        ////     Asynchronously reads the text content at the current position as an System.Object.
        ////
        //// Returns:
        ////     The text content as the most appropriate common language runtime (CLR) object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<object> ReadContentAsObjectAsync();
        //
        // Summary:
        //     Reads the text content at the current position as a System.String object.
        //
        // Returns:
        //     The text content as a System.String object.
        //
        // Exceptions:
        //   System.InvalidCastException:
        //     The attempted cast is not valid.
        //
        //   System.FormatException:
        //     The string format is not valid.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadContentAsString()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Asynchronously reads the text content at the current position as a System.String
        ////     object.
        ////
        //// Returns:
        ////     The text content as a System.String object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<string> ReadContentAsStringAsync();
        ////
        //// Summary:
        ////     Reads the element content as the requested type.
        ////
        //// Parameters:
        ////   returnType:
        ////     The type of the value to be returned.Note   With the release of the .NET
        ////     Framework 3.5, the value of the returnType parameter can now be the System.DateTimeOffset
        ////     type.
        ////
        ////   namespaceResolver:
        ////     An System.Xml.IXmlNamespaceResolver object that is used to resolve any namespace
        ////     prefixes related to type conversion.
        ////
        //// Returns:
        ////     The element content converted to the requested typed object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.OverflowException:
        ////     Read Decimal.MaxValue.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver);
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the element content as the requested type.
        ////
        //// Parameters:
        ////   returnType:
        ////     The type of the value to be returned.Note   With the release of the .NET
        ////     Framework 3.5, the value of the returnType parameter can now be the System.DateTimeOffset
        ////     type.
        ////
        ////   namespaceResolver:
        ////     An System.Xml.IXmlNamespaceResolver object that is used to resolve any namespace
        ////     prefixes related to type conversion.
        ////
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content converted to the requested typed object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.OverflowException:
        ////     Read Decimal.MaxValue.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Asynchronously reads the element content as the requested type.
        ////
        //// Parameters:
        ////   returnType:
        ////     The type of the value to be returned.
        ////
        ////   namespaceResolver:
        ////     An System.Xml.IXmlNamespaceResolver object that is used to resolve any namespace
        ////     prefixes related to type conversion.
        ////
        //// Returns:
        ////     The element content converted to the requested typed object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver);
        ////
        //// Summary:
        ////     Reads the element and decodes the Base64 content.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The buffer value is null.
        ////
        ////   System.InvalidOperationException:
        ////     The current node is not an element node.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The index into the buffer or index + count is larger than the allocated buffer
        ////     size.
        ////
        ////   System.NotSupportedException:
        ////     The System.Xml.XmlReader implementation does not support this method.
        ////
        ////   System.Xml.XmlException:
        ////     The element contains mixed-content.
        ////
        ////   System.FormatException:
        ////     The content cannot be converted to the requested type.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously reads the element and decodes the Base64 content.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Reads the element and decodes the BinHex content.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The buffer value is null.
        ////
        ////   System.InvalidOperationException:
        ////     The current node is not an element node.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The index into the buffer or index + count is larger than the allocated buffer
        ////     size.
        ////
        ////   System.NotSupportedException:
        ////     The System.Xml.XmlReader implementation does not support this method.
        ////
        ////   System.Xml.XmlException:
        ////     The element contains mixed-content.
        ////
        ////   System.FormatException:
        ////     The content cannot be converted to the requested type.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously reads the element and decodes the BinHex content.
        ////
        //// Parameters:
        ////   buffer:
        ////     The buffer into which to copy the resulting text. This value cannot be null.
        ////
        ////   index:
        ////     The offset into the buffer where to start copying the result.
        ////
        ////   count:
        ////     The maximum number of bytes to copy into the buffer. The actual number of
        ////     bytes copied is returned from this method.
        ////
        //// Returns:
        ////     The number of bytes written to the buffer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a System.Boolean object.
        ////
        //// Returns:
        ////     The element content as a System.Boolean object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a System.Boolean object.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool ReadElementContentAsBoolean();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a System.Boolean object.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a System.Boolean object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a System.DateTime object.
        ////
        //// Returns:
        ////     The element content as a System.DateTime object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a System.DateTime object.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual DateTime ReadElementContentAsDateTime();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a System.DateTime object.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element contents as a System.DateTime object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a System.Decimal object.
        ////
        //// Returns:
        ////     The element content as a System.Decimal object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a System.Decimal.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual decimal ReadElementContentAsDecimal();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a System.Decimal object.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a System.Decimal object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a System.Decimal.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a double-precision
        ////     floating-point number.
        ////
        //// Returns:
        ////     The element content as a double-precision floating-point number.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a double-precision floating-point number.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual double ReadElementContentAsDouble();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a double-precision floating-point number.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a double-precision floating-point number.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual double ReadElementContentAsDouble(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as single-precision floating-point
        ////     number.
        ////
        //// Returns:
        ////     The element content as a single-precision floating point number.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a single-precision floating-point number.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual float ReadElementContentAsFloat();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a single-precision floating-point number.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a single-precision floating point number.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a single-precision floating-point number.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual float ReadElementContentAsFloat(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a 32-bit signed integer.
        ////
        //// Returns:
        ////     The element content as a 32-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a 32-bit signed integer.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadElementContentAsInt();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a 32-bit signed integer.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a 32-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a 32-bit signed integer.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadElementContentAsInt(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as a 64-bit signed integer.
        ////
        //// Returns:
        ////     The element content as a 64-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a 64-bit signed integer.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual long ReadElementContentAsLong();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as a 64-bit signed integer.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     The element content as a 64-bit signed integer.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to a 64-bit signed integer.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual long ReadElementContentAsLong(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Reads the current element and returns the contents as an System.Object.
        ////
        //// Returns:
        ////     A boxed common language runtime (CLR) object of the most appropriate type.
        ////     The System.Xml.XmlReader.ValueType property determines the appropriate CLR
        ////     type. If the content is typed as a list type, this method returns an array
        ////     of boxed objects of the appropriate type.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadElementContentAsObject();
        ////
        //// Summary:
        ////     Checks that the specified local name and namespace URI matches that of the
        ////     current element, then reads the current element and returns the contents
        ////     as an System.Object.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element.
        ////
        //// Returns:
        ////     A boxed common language runtime (CLR) object of the most appropriate type.
        ////     The System.Xml.XmlReader.ValueType property determines the appropriate CLR
        ////     type. If the content is typed as a list type, this method returns an array
        ////     of boxed objects of the appropriate type.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on an element.
        ////
        ////   System.Xml.XmlException:
        ////     The current element contains child elements.-or-The element content cannot
        ////     be converted to the requested type.
        ////
        ////   System.ArgumentNullException:
        ////     The method is called with null arguments.
        ////
        ////   System.ArgumentException:
        ////     The specified local name and namespace URI do not match that of the current
        ////     element being read.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual object ReadElementContentAsObject(string localName, string namespaceURI);
        ////
        //// Summary:
        ////     Asynchronously reads the current element and returns the contents as an System.Object.
        ////
        //// Returns:
        ////     A boxed common language runtime (CLR) object of the most appropriate type.
        ////     The System.Xml.XmlReader.ValueType property determines the appropriate CLR
        ////     type. If the content is typed as a list type, this method returns an array
        ////     of boxed objects of the appropriate type.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<object> ReadElementContentAsObjectAsync();
        //
        // Summary:
        //     Reads the current element and returns the contents as a System.String object.
        //
        // Returns:
        //     The element content as a System.String object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Xml.XmlReader is not positioned on an element.
        //
        //   System.Xml.XmlException:
        //     The current element contains child elements.-or-The element content cannot
        //     be converted to a System.String object.
        //
        //   System.ArgumentNullException:
        //     The method is called with null arguments.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadElementContentAsString()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the specified local name and namespace URI matches that of the
        //     current element, then reads the current element and returns the contents
        //     as a System.String object.
        //
        // Parameters:
        //   localName:
        //     The local name of the element.
        //
        //   namespaceURI:
        //     The namespace URI of the element.
        //
        // Returns:
        //     The element content as a System.String object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Xml.XmlReader is not positioned on an element.
        //
        //   System.Xml.XmlException:
        //     The current element contains child elements.-or-The element content cannot
        //     be converted to a System.String object.
        //
        //   System.ArgumentNullException:
        //     The method is called with null arguments.
        //
        //   System.ArgumentException:
        //     The specified local name and namespace URI do not match that of the current
        //     element being read.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadElementContentAsString(string localName, string namespaceURI)
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Asynchronously reads the current element and returns the contents as a System.String
        ////     object.
        ////
        //// Returns:
        ////     The element content as a System.String object.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<string> ReadElementContentAsStringAsync();
        //
        // Summary:
        //     Reads a text-only element.
        //
        // Returns:
        //     The text contained in the element that was read. An empty string if the element
        //     is empty (<item></item> or <item/>).
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     The next content node is not a start tag; or the element found does not contain
        //     a simple text value.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadElementString()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the System.Xml.XmlReader.Name property of the element found matches
        //     the given string before reading a text-only element.
        //
        // Parameters:
        //   name:
        //     The name to check.
        //
        // Returns:
        //     The text contained in the element that was read. An empty string if the element
        //     is empty (<item></item> or <item/>).
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     If the next content node is not a start tag; if the element Name does not
        //     match the given argument; or if the element found does not contain a simple
        //     text value.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadElementString(string name)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the System.Xml.XmlReader.LocalName and System.Xml.XmlReader.NamespaceURI
        //     properties of the element found matches the given strings before reading
        //     a text-only element.
        //
        // Parameters:
        //   localname:
        //     The local name to check.
        //
        //   ns:
        //     The namespace URI to check.
        //
        // Returns:
        //     The text contained in the element that was read. An empty string if the element
        //     is empty (<item></item> or <item/>).
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     If the next content node is not a start tag; if the element LocalName or
        //     NamespaceURI do not match the given arguments; or if the element found does
        //     not contain a simple text value.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadElementString(string localname, string ns)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the current content node is an end tag and advances the reader
        //     to the next node.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     The current node is not an end tag or if incorrect XML is encountered in
        //     the input stream.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void ReadEndElement()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     When overridden in a derived class, reads all the content, including markup,
        ////     as a string.
        ////
        //// Returns:
        ////     All the XML content, including markup, in the current node. If the current
        ////     node has no children, an empty string is returned.If the current node is
        ////     neither an element nor attribute, an empty string is returned.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     The XML was not well-formed, or an error occurred while parsing the XML.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string ReadInnerXml();
        ////
        //// Summary:
        ////     Asynchronously reads all the content, including markup, as a string.
        ////
        //// Returns:
        ////     All the XML content, including markup, in the current node. If the current
        ////     node has no children, an empty string is returned.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<string> ReadInnerXmlAsync();
        ////
        //// Summary:
        ////     When overridden in a derived class, reads the content, including markup,
        ////     representing this node and all its children.
        ////
        //// Returns:
        ////     If the reader is positioned on an element or an attribute node, this method
        ////     returns all the XML content, including markup, of the current node and all
        ////     its children; otherwise, it returns an empty string.
        ////
        //// Exceptions:
        ////   System.Xml.XmlException:
        ////     The XML was not well-formed, or an error occurred while parsing the XML.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string ReadOuterXml();
        ////
        //// Summary:
        ////     Asynchronously reads the content, including markup, representing this node
        ////     and all its children.
        ////
        //// Returns:
        ////     If the reader is positioned on an element or an attribute node, this method
        ////     returns all the XML content, including markup, of the current node and all
        ////     its children; otherwise, it returns an empty string.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task<string> ReadOuterXmlAsync();
        //
        // Summary:
        //     Checks that the current node is an element and advances the reader to the
        //     next node.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML was encountered in the input stream.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void ReadStartElement()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the current content node is an element with the given System.Xml.XmlReader.Name
        //     and advances the reader to the next node.
        //
        // Parameters:
        //   name:
        //     The qualified name of the element.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML was encountered in the input stream. -or- The System.Xml.XmlReader.Name
        //     of the element does not match the given name.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void ReadStartElement(string name)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Checks that the current content node is an element with the given System.Xml.XmlReader.LocalName
        //     and System.Xml.XmlReader.NamespaceURI and advances the reader to the next
        //     node.
        //
        // Parameters:
        //   localname:
        //     The local name of the element.
        //
        //   ns:
        //     The namespace URI of the element.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     Incorrect XML was encountered in the input stream.-or-The System.Xml.XmlReader.LocalName
        //     and System.Xml.XmlReader.NamespaceURI properties of the element found do
        //     not match the given arguments.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void ReadStartElement(string localname, string ns)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     When overridden in a derived class, reads the contents of an element or text
        //     node as a string.
        //
        // Returns:
        //     The contents of the element or an empty string.
        //
        // Exceptions:
        //   System.Xml.XmlException:
        //     An error occurred while parsing the XML.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual string ReadString()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Returns a new XmlReader instance that can be used to read the current node,
        ////     and all its descendants.
        ////
        //// Returns:
        ////     A new XmlReader instance set to ReadState.Initial. A call to the System.Xml.XmlReader.Read()
        ////     method positions the new XmlReader on the node that was current before the
        ////     call to ReadSubtree method.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The XmlReader is not positioned on an element when this method is called.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual XmlReader ReadSubtree();
        ////
        //// Summary:
        ////     Advances the System.Xml.XmlReader to the next descendant element with the
        ////     specified qualified name.
        ////
        //// Parameters:
        ////   name:
        ////     The qualified name of the element you wish to move to.
        ////
        //// Returns:
        ////     true if a matching descendant element is found; otherwise false. If a matching
        ////     child element is not found, the System.Xml.XmlReader is positioned on the
        ////     end tag (System.Xml.XmlReader.NodeType is XmlNodeType.EndElement) of the
        ////     element.If the System.Xml.XmlReader is not positioned on an element when
        ////     System.Xml.XmlReader.ReadToDescendant(System.String) was called, this method
        ////     returns false and the position of the System.Xml.XmlReader is not changed.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.ArgumentException:
        ////     The parameter is an empty string.
        //public virtual bool ReadToDescendant(string name);
        ////
        //// Summary:
        ////     Advances the System.Xml.XmlReader to the next descendant element with the
        ////     specified local name and namespace URI.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element you wish to move to.
        ////
        ////   namespaceURI:
        ////     The namespace URI of the element you wish to move to.
        ////
        //// Returns:
        ////     true if a matching descendant element is found; otherwise false. If a matching
        ////     child element is not found, the System.Xml.XmlReader is positioned on the
        ////     end tag (System.Xml.XmlReader.NodeType is XmlNodeType.EndElement) of the
        ////     element.If the System.Xml.XmlReader is not positioned on an element when
        ////     System.Xml.XmlReader.ReadToDescendant(System.String,System.String) was called,
        ////     this method returns false and the position of the System.Xml.XmlReader is
        ////     not changed.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.ArgumentNullException:
        ////     Both parameter values are null.
        //public virtual bool ReadToDescendant(string localName, string namespaceURI);
        //
        // Summary:
        //     Reads until an element with the specified qualified name is found.
        //
        // Parameters:
        //   name:
        //     The qualified name of the element.
        //
        // Returns:
        //     true if a matching element is found; otherwise false and the System.Xml.XmlReader
        //     is in an end of file state.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        //
        //   System.ArgumentException:
        //     The parameter is an empty string.
        public virtual bool ReadToFollowing(string name)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Reads until an element with the specified local name and namespace URI is
        //     found.
        //
        // Parameters:
        //   localName:
        //     The local name of the element.
        //
        //   namespaceURI:
        //     The namespace URI of the element.
        //
        // Returns:
        //     true if a matching element is found; otherwise false and the System.Xml.XmlReader
        //     is in an end of file state.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        //
        //   System.ArgumentNullException:
        //     Both parameter values are null.
        public virtual bool ReadToFollowing(string localName, string namespaceURI)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Advances the XmlReader to the next sibling element with the specified qualified
        //     name.
        //
        // Parameters:
        //   name:
        //     The qualified name of the sibling element you wish to move to.
        //
        // Returns:
        //     true if a matching sibling element is found; otherwise false. If a matching
        //     sibling element is not found, the XmlReader is positioned on the end tag
        //     (System.Xml.XmlReader.NodeType is XmlNodeType.EndElement) of the parent element.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        //
        //   System.ArgumentException:
        //     The parameter is an empty string.
        public virtual bool ReadToNextSibling(string name)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Advances the XmlReader to the next sibling element with the specified local
        //     name and namespace URI.
        //
        // Parameters:
        //   localName:
        //     The local name of the sibling element you wish to move to.
        //
        //   namespaceURI:
        //     The namespace URI of the sibling element you wish to move to.
        //
        // Returns:
        //     true if a matching sibling element is found; otherwise, false. If a matching
        //     sibling element is not found, the XmlReader is positioned on the end tag
        //     (System.Xml.XmlReader.NodeType is XmlNodeType.EndElement) of the parent element.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        //
        //   System.ArgumentNullException:
        //     Both parameter values are null.
        public virtual bool ReadToNextSibling(string localName, string namespaceURI)
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Reads large streams of text embedded in an XML document.
        ////
        //// Parameters:
        ////   buffer:
        ////     The array of characters that serves as the buffer to which the text contents
        ////     are written. This value cannot be null.
        ////
        ////   index:
        ////     The offset within the buffer where the System.Xml.XmlReader can start to
        ////     copy the results.
        ////
        ////   count:
        ////     The maximum number of characters to copy into the buffer. The actual number
        ////     of characters copied is returned from this method.
        ////
        //// Returns:
        ////     The number of characters read into the buffer. The value zero is returned
        ////     when there is no more text content.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The current node does not have a value (System.Xml.XmlReader.HasValue is
        ////     false).
        ////
        ////   System.ArgumentNullException:
        ////     The buffer value is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     The index into the buffer, or index + count is larger than the allocated
        ////     buffer size.
        ////
        ////   System.NotSupportedException:
        ////     The System.Xml.XmlReader implementation does not support this method.
        ////
        ////   System.Xml.XmlException:
        ////     The XML data is not well-formed.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual int ReadValueChunk(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously reads large streams of text embedded in an XML document.
        ////
        //// Parameters:
        ////   buffer:
        ////     The array of characters that serves as the buffer to which the text contents
        ////     are written. This value cannot be null.
        ////
        ////   index:
        ////     The offset within the buffer where the System.Xml.XmlReader can start to
        ////     copy the results.
        ////
        ////   count:
        ////     The maximum number of characters to copy into the buffer. The actual number
        ////     of characters copied is returned from this method.
        ////
        //// Returns:
        ////     The number of characters read into the buffer. The value zero is returned
        ////     when there is no more text content.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task<int> ReadValueChunkAsync(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     When overridden in a derived class, resolves the entity reference for EntityReference
        ////     nodes.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The reader is not positioned on an EntityReference node; this implementation
        ////     of the reader cannot resolve entities (System.Xml.XmlReader.CanResolveEntity
        ////     returns false).
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void ResolveEntity();
        //
        // Summary:
        //     Skips the children of the current node.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlReader method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        public virtual void Skip()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Asynchronously skips the children of the current node.
        ////
        //// Returns:
        ////     The current node.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlReader asynchronous method was called without setting the
        ////     System.Xml.XmlReaderSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlReaderSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task SkipAsync();



        //(new region and 3 elements to compile)
#region NoCommentedFunctionsAndAttributes

        public abstract bool MoveToAttribute(string name);

        public abstract bool MoveToAttribute(string name, string ns);

        public abstract bool ReadAttributeValue();

        public abstract ReadState ReadState { get; }

        public abstract string LookupNamespace(string prefix);

        public abstract void ResolveEntity();

        public abstract XmlReaderSettings Settings { get; }
        public abstract string Prefix { get; }
        public abstract bool HasValue { get; }
        public abstract int Depth { get; }
        public abstract string BaseURI { get; }
        public abstract bool IsDefault { get; }
        public abstract char QuoteChar { get; }
        //public abstract XmlSpace XmlSpace { get; }
        public abstract string XmlLang { get; }
        public abstract IXmlSchemaInfo SchemaInfo { get; }
        public abstract System.Type ValueType { get; }
        public abstract string this[int i] { get; }
        public abstract string this[string name] { get;}
        public abstract string this[string name, string namespaceURI] { get; }
        public abstract bool CanResolveEntity { get; }
        public abstract bool EOF { get; }
        //public abstract ReadState ReadState { get; }
        public abstract bool HasAttributes { get; }

#endregion



    }

    //todo move to its own file?
    public enum XmlSpace { }

    //todo move to its own file?
    public interface IXmlSchemaInfo { }

    //todo move to its own file?
    public enum ReadState { }

}
