
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
using System.Collections;
using System.Reflection;
using System.Xml;

namespace System.Xml.Serialization
{
    /// <summary>
    /// Controls deserialization by the System.Xml.Serialization.XmlSerializer class.
    /// </summary>
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public abstract class XmlSerializationReader : XmlSerializationGeneratedCode
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializationReader
        /// class.
        /// </summary>
        protected XmlSerializationReader()
        {
            throw new NotImplementedException();
        }

        //// Summary:
        ////     Gets or sets a value that determines whether XML strings are translated into
        ////     valid .NET Framework type names.
        ////
        //// Returns:
        ////     true if XML strings are decoded into valid .NET Framework type names; otherwise,
        ////     false.
        //protected bool DecodeName { get; set; }
        ////
        //// Summary:
        ////     Gets the XML document object into which the XML document is being deserialized.
        ////
        //// Returns:
        ////     An System.Xml.XmlDocument that represents the deserialized System.Xml.XmlDocument
        ////     data.
        //protected XmlDocument Document { get; }
        ////
        //// Summary:
        ////     Gets or sets a value that should be true for a SOAP 1.1 return value.
        ////
        //// Returns:
        ////     true, if the value is a return value.
        //protected bool IsReturnValue { get; set; }
       
        /// <summary>
        /// Gets the System.Xml.XmlReader object that is being used by System.Xml.Serialization.XmlSerializationReader.
        /// </summary>
        protected XmlReader Reader
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Gets the current count of the System.Xml.XmlReader.
        /// </summary>
        protected int ReaderCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //// Summary:
        ////     Stores an object that contains a callback method that will be called, as
        ////     necessary, to fill in .NET Framework collections or enumerations that map
        ////     to SOAP-encoded arrays or SOAP-encoded, multi-referenced elements.
        ////
        //// Parameters:
        ////   fixup:
        ////     A System.Xml.Serialization.XmlSerializationCollectionFixupCallback delegate
        ////     and the callback method's input data.
        //protected void AddFixup(XmlSerializationReader.CollectionFixup fixup);
        ////
        //// Summary:
        ////     Stores an object that contains a callback method instance that will be called,
        ////     as necessary, to fill in the objects in a SOAP-encoded array.
        ////
        //// Parameters:
        ////   fixup:
        ////     An System.Xml.Serialization.XmlSerializationFixupCallback delegate and the
        ////     callback method's input data.
        //protected void AddFixup(XmlSerializationReader.Fixup fixup);
        ////
        //// Summary:
        ////     Stores an implementation of the System.Xml.Serialization.XmlSerializationReadCallback
        ////     delegate and its input data for a later invocation.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the .NET Framework type that is being deserialized.
        ////
        ////   ns:
        ////     The namespace of the .NET Framework type that is being deserialized.
        ////
        ////   type:
        ////     The System.Type to be deserialized.
        ////
        ////   read:
        ////     An System.Xml.Serialization.XmlSerializationReadCallback delegate.
        //protected void AddReadCallback(string name, string ns, Type type, XmlSerializationReadCallback read);
        ////
        //// Summary:
        ////     Stores an object that is being deserialized from a SOAP-encoded multiRef
        ////     element for later access through the System.Xml.Serialization.XmlSerializationReader.GetTarget(System.String)
        ////     method.
        ////
        //// Parameters:
        ////   id:
        ////     The value of the id attribute of a multiRef element that identifies the element.
        ////
        ////   o:
        ////     The object that is deserialized from the XML element.
        //protected void AddTarget(string id, object o);
       
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Xml.Serialization.XmlSerializationReader.ReaderCount has not advanced.
        /// <summary>
        /// Checks whether the deserializer has advanced.
        /// </summary>
        /// <param name="whileIterations">The current count in a while loop.</param>
        /// <param name="readerCount">The current System.Xml.Serialization.XmlSerializationReader.ReaderCount.</param>
        protected void CheckReaderCount(ref int whileIterations, ref int readerCount)
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Removes all occurrences of white space characters from the beginning and
        ////     end of the specified string.
        ////
        //// Parameters:
        ////   value:
        ////     The string that will have its white space trimmed.
        ////
        //// Returns:
        ////     The trimmed string.
        //protected string CollapseWhitespace(string value);
        
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that an object
        /// being deserialized should be abstract.
        /// </summary>
        /// <param name="name">The name of the abstract type.</param>
        /// <param name="ns">The .NET Framework namespace of the abstract type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateAbstractTypeException(string name, string ns)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Populates an object from its XML representation at the current location of
        /// the System.Xml.XmlReader, with an option to read the inner element.
        /// </summary>
        /// <param name="xsdDerived">The local name of the derived XML Schema data type.</param>
        /// <param name="nsDerived">The namespace of the derived XML Schema data type.</param>
        /// <param name="xsdBase">The local name of the base XML Schema data type.</param>
        /// <param name="nsBase">The namespace of the base XML Schema data type.</param>
        /// <param name="clrDerived">The namespace of the derived .NET Framework type.</param>
        /// <param name="clrBase">The name of the base .NET Framework type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateBadDerivationException(string xsdDerived, string nsDerived, string xsdBase, string nsBase, string clrDerived, string clrBase)
        {
            throw new NotImplementedException();
        }
      
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that an object
        /// being deserialized cannot be instantiated because the constructor throws
        /// a security exception.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateCtorHasSecurityException(string typeName)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that an object
        /// being deserialized cannot be instantiated because there is no constructor
        /// available.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateInaccessibleConstructorException(string typeName)
        {
            throw new NotImplementedException();
        }
     
        /// <summary>
        /// Creates an System.InvalidCastException that indicates that an explicit reference
        /// conversion failed.
        /// </summary>
        /// <param name="type">
        /// The System.Type that an object cannot be cast to. This type is incorporated
        /// into the exception message.
        /// </param>
        /// <param name="value">
        /// The object that cannot be cast. This object is incorporated into the exception
        /// message.
        /// </param>
        /// <returns>An System.InvalidCastException exception.</returns>
        protected Exception CreateInvalidCastException(Type type, object value)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Creates an System.InvalidCastException that indicates that an explicit reference
        /// conversion failed.
        /// </summary>
        /// <param name="type">
        /// The System.Type that an object cannot be cast to. This type is incorporated
        /// into the exception message.
        /// </param>
        /// <param name="value">
        /// The object that cannot be cast. This object is incorporated into the exception
        /// message.
        /// </param>
        /// <param name="id">A string identifier.</param>
        /// <returns>An System.InvalidCastException exception.</returns>
        protected Exception CreateInvalidCastException(Type type, object value, string id)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that a derived
        /// type that is mapped to an XML Schema data type cannot be located.
        /// </summary>
        /// <param name="name">
        /// The local name of the XML Schema data type that is mapped to the unavailable
        /// derived type.
        /// </param>
        /// <param name="ns">
        /// The namespace of the XML Schema data type that is mapped to the unavailable
        /// derived type.
        /// </param>
        /// <param name="clrType">
        /// The full name of the .NET Framework base type for which a derived type cannot
        /// be located.
        /// </param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateMissingIXmlSerializableType(string name, string ns, string clrType)
        {
            throw new NotImplementedException();
        }
      
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that a SOAP-encoded
        /// collection type cannot be modified and its values cannot be filled in.
        /// </summary>
        /// <param name="name">
        /// The fully qualified name of the .NET Framework type for which there is a
        /// mapping.
        /// </param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateReadOnlyCollectionException(string name)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that an enumeration
        /// value is not valid.
        /// </summary>
        /// <param name="value">The enumeration value that is not valid.</param>
        /// <param name="enumType">The enumeration type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateUnknownConstantException(string value, Type enumType)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that the current
        /// position of System.Xml.XmlReader represents an unknown XML node.
        /// </summary>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateUnknownNodeException()
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that a type is
        /// unknown.
        /// </summary>
        /// <param name="type">An System.Xml.XmlQualifiedName that represents the name of the unknown type.</param>
        /// <returns>An System.InvalidOperationException exception.</returns>
        protected Exception CreateUnknownTypeException(XmlQualifiedName type)
        {
            throw new NotImplementedException(); //todo
        }
      
        /// <summary>
        /// Ensures that a given array, or a copy, is large enough to contain a specified
        /// index.
        /// </summary>
        /// <param name="a">The System.Array that is being checked.</param>
        /// <param name="index">The required index.</param>
        /// <param name="elementType">The System.Type of the array's elements.</param>
        /// <returns>
        /// The existing System.Array, if it is already large enough; otherwise, a new,
        /// larger array that contains the original array's elements.
        /// </returns>
        protected Array EnsureArrayIndex(Array a, int index, Type elementType)
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Fills in the values of a SOAP-encoded array whose data type maps to a .NET
        ////     Framework reference type.
        ////
        //// Parameters:
        ////   fixup:
        ////     An object that contains the array whose values are filled in.
        //protected void FixupArrayRefs(object fixup);
        ////
        //// Summary:
        ////     Gets the length of the SOAP-encoded array where the System.Xml.XmlReader
        ////     is currently positioned.
        ////
        //// Parameters:
        ////   name:
        ////     The local name that the array should have.
        ////
        ////   ns:
        ////     The namespace that the array should have.
        ////
        //// Returns:
        ////     The length of the SOAP array.
        //protected int GetArrayLength(string name, string ns);
      
        /// <summary>
        /// Determines whether the XML element where the System.Xml.XmlReader is currently
        /// positioned has a null attribute set to the value true.
        /// </summary>
        /// <returns>
        /// true if System.Xml.XmlReader is currently positioned over a null attribute
        /// with the value true; otherwise, false.
        /// </returns>
        protected bool GetNullAttr()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Gets an object that is being deserialized from a SOAP-encoded multiRef element
        //     and that was stored earlier by System.Xml.Serialization.XmlSerializationReader.AddTarget(System.String,System.Object).
        //
        // Parameters:
        //   id:
        //     The value of the id attribute of a multiRef element that identifies the element.
        //
        // Returns:
        //     An object to be deserialized from a SOAP-encoded multiRef element.
        //protected object GetTarget(string id);
        //
        // Summary:
        //     
        //     
        //
        // Returns:
        //     A
        /// <summary>
        /// Gets the value of the xsi:type attribute for the XML element at the current
        /// location of the System.Xml.XmlReader.
        /// </summary>
        /// <returns>An XML qualified name that indicates the data type of an XML element.</returns>
        protected XmlQualifiedName GetXsiType()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes callback methods that populate objects that map to SOAP-encoded
        /// XML data.
        /// </summary>
        protected abstract void InitCallbacks();
        /// <summary>
        /// Stores element and attribute names in a System.Xml.NameTable object.
        /// </summary>
        protected abstract void InitIDs();
       
        //     
        /// <summary>
        /// Determines whether an XML attribute name indicates an XML namespace.
        /// </summary>
        /// <param name="name">The name of an XML attribute.</param>
        /// <returns>true if the XML attribute name indicates an XML namespace; otherwise, false.</returns>
        protected bool IsXmlnsAttribute(string name)
        {
            throw new NotImplementedException(); //todo
        }
        ////
        //// Summary:
        ////     Sets the value of the XML attribute if it is of type arrayType from the Web
        ////     Services Description Language (WSDL) namespace.
        ////
        //// Parameters:
        ////   attr:
        ////     An System.Xml.XmlAttribute that may have the type wsdl:array.
        //protected void ParseWsdlArrayType(XmlAttribute attr);
        ////
        //// Summary:
        ////     Makes the System.Xml.XmlReader read the fully qualified name of the element
        ////     where it is currently positioned.
        ////
        //// Returns:
        ////     The fully qualified name of the current XML element.
        //protected XmlQualifiedName ReadElementQualifiedName();
        
        /// <summary>
        /// Makes the System.Xml.XmlReader read an XML end tag.
        /// </summary>
        protected void ReadEndElement()
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Instructs the System.Xml.XmlReader to read the current XML element if the
        /// element has a null attribute with the value true.
        /// </summary>
        /// <returns>
        /// true if the element has a null="true" attribute value and has been read;
        /// otherwise, false.
        /// </returns>
        protected bool ReadNull()
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Instructs the System.Xml.XmlReader to read the fully qualified name of the
        //     element where it is currently positioned.
        //
        // Returns:
        //     A System.Xml.XmlQualifiedName that represents the fully qualified name of
        //     the current XML element; otherwise, null if a null="true" attribute value
        //     is present.
        //protected XmlQualifiedName ReadNullableQualifiedName();
        ////
        //// Summary:
        ////     Instructs the System.Xml.XmlReader to read a simple, text-only XML element
        ////     that could be null.
        ////
        //// Returns:
        ////     The string value; otherwise, null.
        //protected string ReadNullableString();
        ////
        //// Summary:
        ////     Reads the value of the href attribute (ref attribute for SOAP 1.2) that is
        ////     used to refer to an XML element in SOAP encoding.
        ////
        //// Parameters:
        ////   fixupReference:
        ////     An output string into which the href attribute value is read.
        ////
        //// Returns:
        ////     true if the value was read; otherwise, false.
        //protected bool ReadReference(out string fixupReference);
        ////
        //// Summary:
        ////     Deserializes an object from a SOAP-encoded multiRef XML element.
        ////
        //// Returns:
        ////     The value of the referenced element in the document.
        //protected object ReadReferencedElement();
        ////
        //// Summary:
        ////     Deserializes an object from a SOAP-encoded multiRef XML element.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the element's XML Schema data type.
        ////
        ////   ns:
        ////     The namespace of the element's XML Schema data type.
        ////
        //// Returns:
        ////     The value of the referenced element in the document.
        //protected object ReadReferencedElement(string name, string ns);
        ////
        //// Summary:
        ////     Deserializes objects from the SOAP-encoded multiRef elements in a SOAP message.
        //protected void ReadReferencedElements();
        ////
        //// Summary:
        ////     Deserializes an object from an XML element in a SOAP message that contains
        ////     a reference to a multiRef element.
        ////
        //// Parameters:
        ////   fixupReference:
        ////     An output string into which the href attribute value is read.
        ////
        //// Returns:
        ////     The deserialized object.
        //protected object ReadReferencingElement(out string fixupReference);
        ////
        //// Summary:
        ////     Deserializes an object from an XML element in a SOAP message that contains
        ////     a reference to a multiRef element.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the element's XML Schema data type.
        ////
        ////   ns:
        ////     The namespace of the element's XML Schema data type.
        ////
        ////   fixupReference:
        ////     An output string into which the href attribute value is read.
        ////
        //// Returns:
        ////     The deserialized object.
        //protected object ReadReferencingElement(string name, string ns, out string fixupReference);
        ////
        //// Summary:
        ////     Deserializes an object from an XML element in a SOAP message that contains
        ////     a reference to a multiRef element.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the element's XML Schema data type.
        ////
        ////   ns:
        ////     The namespace of the element's XML Schema data type.
        ////
        ////   elementCanBeType:
        ////     true if the element name is also the XML Schema data type name; otherwise,
        ////     false.
        ////
        ////   fixupReference:
        ////     An output string into which the value of the href attribute is read.
        ////
        //// Returns:
        ////     The deserialized object.
        //protected object ReadReferencingElement(string name, string ns, bool elementCanBeType, out string fixupReference);
       
        /// <summary>
        /// Populates an object from its XML representation at the current location of
        /// the System.Xml.XmlReader.
        /// </summary>
        /// <param name="serializable">
        /// An System.Xml.Serialization.IXmlSerializable that corresponds to the current
        /// position of the System.Xml.XmlReader.
        /// </param>
        /// <returns>
        /// An object that implements the System.Xml.Serialization.IXmlSerializable interface
        /// with its members populated from the location of the System.Xml.XmlReader.
        /// </returns>
        protected IXmlSerializable ReadSerializable(IXmlSerializable serializable)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// This method supports the .NET Framework infrastructure and is not intended
        /// to be used directly from your code.
        /// </summary>
        /// <param name="serializable">
        /// An IXmlSerializable object that corresponds to the current position of the
        /// XMLReader.
        /// </param>
        /// <param name="wrappedAny">Specifies whether the serializable object is wrapped.</param>
        /// <returns>
        /// An object that implements the IXmlSerializable interface with its members
        /// populated from the location of the XmlReader.
        /// </returns>
        protected IXmlSerializable ReadSerializable(IXmlSerializable serializable, bool wrappedAny)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Produces the result of a call to the System.Xml.XmlReader.ReadString() method
        /// appended to the input value.
        /// </summary>
        /// <param name="value">
        /// A string to prefix to the result of a call to the System.Xml.XmlReader.ReadString()
        /// method.
        /// </param>
        /// <returns>
        /// The result of call to the System.Xml.XmlReader.ReadString() method appended
        /// to the input value.
        /// </returns>
        protected string ReadString(string value)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns the result of a call to the System.Xml.XmlReader.ReadString() method
        /// of the System.Xml.XmlReader class, trimmed of white space if needed, and
        /// appended to the input value.
        /// </summary>
        /// <param name="value">A string that will be appended to.</param>
        /// <param name="trim">true if the result of the read operation should be trimmed; otherwise, false.</param>
        /// <returns>The result of the read operation appended to the input value.</returns>
        protected string ReadString(string value, bool trim)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Produces a System.Char object from an input string.
        /// </summary>
        /// <param name="value">A string to translate into a System.Char object.</param>
        /// <returns>A System.Char object.</returns>
        protected static char ToChar(string value) { throw new NotImplementedException(); }
    
        /// <summary>
        /// Produces a System.DateTime object from an input string.
        /// </summary>
        /// <param name="value">A string to translate into a System.DateTime object.</param>
        /// <returns>A System.DateTime object.</returns>
        protected static DateTime ToDateTime(string value)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Reads an XML element that allows null values (xsi:nil = 'true') and returns
        //     a generic System.Nullable<T> value.
        //
        // Parameters:
        //   type:
        //     The System.Xml.XmlQualifiedName that represents the simple data type for
        //     the current location of the System.Xml.XmlReader.
        //
        // Returns:
        //     A generic System.Nullable<T> that represents a null XML value.
        //protected object ReadTypedNull(XmlQualifiedName type)
        ////
        //// Summary:
        ////     Gets the value of the XML node at which the System.Xml.XmlReader is currently
        ////     positioned.
        ////
        //// Parameters:
        ////   type:
        ////     The System.Xml.XmlQualifiedName that represents the simple data type for
        ////     the current location of the System.Xml.XmlReader.
        ////
        //// Returns:
        ////     The value of the node as a .NET Framework value type, if the value is a simple
        ////     XML Schema data type.
        //protected object ReadTypedPrimitive(XmlQualifiedName type);
        ////
        //// Summary:
        ////     Instructs the System.Xml.XmlReader to read an XML document root element at
        ////     its current position.
        ////
        //// Parameters:
        ////   wrapped:
        ////     true if the method should read content only after reading the element's start
        ////     element; otherwise, false.
        ////
        //// Returns:
        ////     An System.Xml.XmlDocument that contains the root element that has been read.
        //protected XmlDocument ReadXmlDocument(bool wrapped);
        ////
        //// Summary:
        ////     Instructs the System.Xml.XmlReader to read the XML node at its current position.
        ////
        //// Parameters:
        ////   wrapped:
        ////     true to read content only after reading the element's start element; otherwise,
        ////     false.
        ////
        //// Returns:
        ////     An System.Xml.XmlNode that represents the XML node that has been read.
        //protected XmlNode ReadXmlNode(bool wrapped);
        ////
        //// Summary:
        ////     Stores an object to be deserialized from a SOAP-encoded multiRef element.
        ////
        //// Parameters:
        ////   o:
        ////     The object to be deserialized.
        //protected void Referenced(object o);
        ////
        //// Summary:
        ////     Gets a dynamically generated assembly by name.
        ////
        //// Parameters:
        ////   assemblyFullName:
        ////     The full name of the assembly.
        ////
        //// Returns:
        ////     A dynamically generated System.Reflection.Assembly.
        //protected static Assembly ResolveDynamicAssembly(string assemblyFullName);
       
        /// <summary>
        /// Ensures that a given array, or a copy, is no larger than a specified length.
        /// </summary>
        /// <param name="a">The array that is being checked.</param>
        /// <param name="length">The maximum length of the array.</param>
        /// <param name="elementType">The System.Type of the array's elements.</param>
        /// <param name="isNullable">
        /// true if null for the array, if present for the input array, can be returned;
        /// otherwise, a new, smaller array.
        /// </param>
        /// <returns>
        /// The existing System.Array, if it is already small enough; otherwise, a new,
        /// smaller array that contains the original array's elements up to the size
        /// of length.
        /// </returns>
        protected Array ShrinkArray(Array a, int length, Type elementType, bool isNullable)
        {
            return null; //todo: implement this
        }

        /// <summary>
        /// Instructs the System.Xml.XmlReader to read the string value at its current
        /// position and return it as a base-64 byte array.
        /// </summary>
        /// <param name="isNull">true to return null; false to return a base-64 byte array.</param>
        /// <returns>A base-64 byte array; otherwise, null if the value of the isNull parameter
        /// is true.</returns>
        protected byte[] ToByteArrayBase64(bool isNull)
        {
            if (isNull)
            {
                return null;
            }
            return null; //todo: implement this
        }

        /// <summary>
        /// Produces a base-64 byte array from an input string.
        /// </summary>
        /// <param name="value">A string to translate into a base-64 byte array.</param>
        /// <returns>A base-64 byte array.</returns>
        protected static byte[] ToByteArrayBase64(string value)
        {
            if (value == null) return null;
            value = value.Trim();
            if (value.Length == 0)
                return new byte[0];
            return Convert.FromBase64String(value);
        }
        ////
        //// Summary:
        ////     Instructs the System.Xml.XmlReader to read the string value at its current
        ////     position and return it as a hexadecimal byte array.
        ////
        //// Parameters:
        ////   isNull:
        ////     true to return null; false to return a hexadecimal byte array.
        ////
        //// Returns:
        ////     A hexadecimal byte array; otherwise, null if the value of the isNull parameter
        ////     is true.
        //protected byte[] ToByteArrayHex(bool isNull);
        ////
        //// Summary:
        ////     Produces a hexadecimal byte array from an input string.
        ////
        //// Parameters:
        ////   value:
        ////     A string to translate into a hexadecimal byte array.
        ////
        //// Returns:
        ////     A hexadecimal byte array.
        //protected static byte[] ToByteArrayHex(string value);
        
        ////
        //// Summary:
        ////     Produces a System.DateTime object from an input string.
        ////
        //// Parameters:
        ////   value:
        ////     A string to translate into a System.DateTime class object.
        ////
        //// Returns:
        ////     A System.DateTimeobject.
        //protected static DateTime ToDate(string value);
        
        ////
        //// Summary:
        ////     Produces a numeric enumeration value from a string that consists of delimited
        ////     identifiers that represent constants from the enumerator list.
        ////
        //// Parameters:
        ////   value:
        ////     A string that consists of delimited identifiers where each identifier represents
        ////     a constant from the set enumerator list.
        ////
        ////   h:
        ////     A System.Collections.Hashtable that consists of the identifiers as keys and
        ////     the constants as integral numbers.
        ////
        ////   typeName:
        ////     The name of the enumeration type.
        ////
        //// Returns:
        ////     A long value that consists of the enumeration value as a series of bitwise
        ////     OR operations.
        //protected static long ToEnum(string value, Hashtable h, string typeName);
        ////
        //// Summary:
        ////     Produces a System.DateTime from a string that represents the time.
        ////
        //// Parameters:
        ////   value:
        ////     A string to translate into a System.DateTime object.
        ////
        //// Returns:
        ////     A System.DateTime object.
        //protected static DateTime ToTime(string value);
        ////
        //// Summary:
        ////     Decodes an XML name.
        ////
        //// Parameters:
        ////   value:
        ////     An XML name to be decoded.
        ////
        //// Returns:
        ////     A decoded string.
        //protected static string ToXmlName(string value);
        ////
        //// Summary:
        ////     Decodes an XML name.
        ////
        //// Parameters:
        ////   value:
        ////     An XML name to be decoded.
        ////
        //// Returns:
        ////     A decoded string.
        //protected static string ToXmlNCName(string value);
        ////
        //// Summary:
        ////     Decodes an XML name.
        ////
        //// Parameters:
        ////   value:
        ////     An XML name to be decoded.
        ////
        //// Returns:
        ////     A decoded string.
        //protected static string ToXmlNmToken(string value);
        ////
        //// Summary:
        ////     Decodes an XML name.
        ////
        //// Parameters:
        ////   value:
        ////     An XML name to be decoded.
        ////
        //// Returns:
        ////     A decoded string.
        //protected static string ToXmlNmTokens(string value);
        ////
        //// Summary:
        ////     Obtains an System.Xml.XmlQualifiedName from a name that may contain a prefix.
        ////
        //// Parameters:
        ////   value:
        ////     A name that may contain a prefix.
        ////
        //// Returns:
        ////     An System.Xml.XmlQualifiedName that represents a namespace-qualified XML
        ////     name.
        //protected XmlQualifiedName ToXmlQualifiedName(string value);
        ////
        //// Summary:
        ////     Raises an System.Xml.Serialization.XmlSerializer.UnknownAttribute event for
        ////     the current position of the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   o:
        ////     An object that the System.Xml.Serialization.XmlSerializer is attempting to
        ////     deserialize, subsequently accessible through the System.Xml.Serialization.XmlAttributeEventArgs.ObjectBeingDeserialized
        ////     property.
        ////
        ////   attr:
        ////     An System.Xml.XmlAttribute that represents the attribute in question.
        //protected void UnknownAttribute(object o, XmlAttribute attr);
        ////
        //// Summary:
        ////     Raises an System.Xml.Serialization.XmlSerializer.UnknownAttribute event for
        ////     the current position of the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   o:
        ////     An object that the System.Xml.Serialization.XmlSerializer is attempting to
        ////     deserialize, subsequently accessible through the System.Xml.Serialization.XmlAttributeEventArgs.ObjectBeingDeserialized
        ////     property.
        ////
        ////   attr:
        ////     A System.Xml.XmlAttribute that represents the attribute in question.
        ////
        ////   qnames:
        ////     A comma-delimited list of XML qualified names.
        //protected void UnknownAttribute(object o, XmlAttribute attr, string qnames);
        ////
        //// Summary:
        ////     Raises an System.Xml.Serialization.XmlSerializer.UnknownElement event for
        ////     the current position of the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   o:
        ////     The System.Object that is being deserialized.
        ////
        ////   elem:
        ////     The System.Xml.XmlElement for which an event is raised.
        //protected void UnknownElement(object o, XmlElement elem);
        ////
        //// Summary:
        ////     Raises an System.Xml.Serialization.XmlSerializer.UnknownElement event for
        ////     the current position of the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   o:
        ////     An object that the System.Xml.Serialization.XmlSerializer is attempting to
        ////     deserialize, subsequently accessible through the System.Xml.Serialization.XmlAttributeEventArgs.ObjectBeingDeserialized
        ////     property.
        ////
        ////   elem:
        ////     The System.Xml.XmlElement for which an event is raised.
        ////
        ////   qnames:
        ////     A comma-delimited list of XML qualified names.
        //protected void UnknownElement(object o, XmlElement elem, string qnames);
     
        /// <summary>
        /// Raises an System.Xml.Serialization.XmlSerializer.UnknownNode event for the
        /// current position of the System.Xml.XmlReader.
        /// </summary>
        /// <param name="o">The object that is being deserialized.</param>
        protected void UnknownNode(object o)
        {
            throw new NotImplementedException(); //todo
        }
       
        /// <summary>
        /// Raises an System.Xml.Serialization.XmlSerializer.UnknownNode event for the
        /// current position of the System.Xml.XmlReader.
        /// </summary>
        /// <param name="o">The object being deserialized.</param>
        /// <param name="qnames">A comma-delimited list of XML qualified names.</param>
        protected void UnknownNode(object o, string qnames)
        {
            throw new NotImplementedException(); //todo
        }
        ////
        //// Summary:
        ////     Raises an System.Xml.Serialization.XmlSerializer.UnreferencedObject event
        ////     for the current position of the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   id:
        ////     A unique string that is used to identify the unreferenced object, subsequently
        ////     accessible through the System.Xml.Serialization.UnreferencedObjectEventArgs.UnreferencedId
        ////     property.
        ////
        ////   o:
        ////     An object that the System.Xml.Serialization.XmlSerializer is attempting to
        ////     deserialize, subsequently accessible through the System.Xml.Serialization.UnreferencedObjectEventArgs.UnreferencedObject
        ////     property.
        //protected void UnreferencedObject(string id, object o);

        //// Summary:
        ////     Holds an System.Xml.Serialization.XmlSerializationCollectionFixupCallback
        ////     delegate instance, plus the method's inputs; also supplies the method's parameters.
        //protected class CollectionFixup
        //{
        //    // Summary:
        //    //     Initializes a new instance of the System.Xml.Serialization.XmlSerializationReader.CollectionFixup
        //    //     class with parameters for a callback method.
        //    //
        //    // Parameters:
        //    //   collection:
        //    //     A collection into which the callback method copies the collection items array.
        //    //
        //    //   callback:
        //    //     A method that instantiates the System.Xml.Serialization.XmlSerializationCollectionFixupCallback
        //    //     delegate.
        //    //
        //    //   collectionItems:
        //    //     An array into which the callback method copies a collection.
        //    public CollectionFixup(object collection, XmlSerializationCollectionFixupCallback callback, object collectionItems);

        //    // Summary:
        //    //     Gets the callback method that instantiates the System.Xml.Serialization.XmlSerializationCollectionFixupCallback
        //    //     delegate.
        //    //
        //    // Returns:
        //    //     The System.Xml.Serialization.XmlSerializationCollectionFixupCallback delegate
        //    //     that points to the callback method.
        //    public XmlSerializationCollectionFixupCallback Callback { get; }
        //    //
        //    // Summary:
        //    //     Gets the object collection for the callback method.
        //    //
        //    // Returns:
        //    //     The collection that is used for the fixup.
        //    public object Collection { get; }
        //    //
        //    // Summary:
        //    //     Gets the array into which the callback method copies a collection.
        //    //
        //    // Returns:
        //    //     The array into which the callback method copies a collection.
        //    public object CollectionItems { get; }
        //}

        //// Summary:
        ////     Holds an System.Xml.Serialization.XmlSerializationFixupCallback delegate
        ////     instance, plus the method's inputs; also serves as the parameter for the
        ////     method.
        //protected class Fixup
        //{
        //    // Summary:
        //    //     Initializes a new instance of the System.Xml.Serialization.XmlSerializationReader.Fixup
        //    //     class.
        //    //
        //    // Parameters:
        //    //   o:
        //    //     The object that contains other objects whose values get filled in by the
        //    //     callback implementation.
        //    //
        //    //   callback:
        //    //     A method that instantiates the System.Xml.Serialization.XmlSerializationFixupCallback
        //    //     delegate.
        //    //
        //    //   count:
        //    //     The size of the string array obtained through the System.Xml.Serialization.XmlSerializationReader.Fixup.Ids
        //    //     property.
        //    public Fixup(object o, XmlSerializationFixupCallback callback, int count);
        //    //
        //    // Summary:
        //    //     Initializes a new instance of the System.Xml.Serialization.XmlSerializationReader.Fixup
        //    //     class.
        //    //
        //    // Parameters:
        //    //   o:
        //    //     The object that contains other objects whose values get filled in by the
        //    //     callback implementation.
        //    //
        //    //   callback:
        //    //     A method that instantiates the System.Xml.Serialization.XmlSerializationFixupCallback
        //    //     delegate.
        //    //
        //    //   ids:
        //    //     The string array obtained through the System.Xml.Serialization.XmlSerializationReader.Fixup.Ids
        //    //     property.
        //    public Fixup(object o, XmlSerializationFixupCallback callback, string[] ids);

        //    // Summary:
        //    //     Gets the callback method that creates an instance of the System.Xml.Serialization.XmlSerializationFixupCallback
        //    //     delegate.
        //    //
        //    // Returns:
        //    //     The callback method that creates an instance of the System.Xml.Serialization.XmlSerializationFixupCallback
        //    //     delegate.
        //    public XmlSerializationFixupCallback Callback { get; }
        //    //
        //    // Summary:
        //    //     Gets or sets an array of keys for the objects that belong to the System.Xml.Serialization.XmlSerializationReader.Fixup.Source
        //    //     property whose values get filled in by the callback implementation.
        //    //
        //    // Returns:
        //    //     The array of keys.
        //    public string[] Ids { get; }
        //    //
        //    // Summary:
        //    //     Gets or sets the object that contains other objects whose values get filled
        //    //     in by the callback implementation.
        //    //
        //    // Returns:
        //    //     The source containing objects with values to fill.
        //    public object Source { get; set; }
        //}
    }
}
