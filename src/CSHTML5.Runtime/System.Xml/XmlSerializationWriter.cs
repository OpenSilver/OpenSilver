
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    // Summary:
    //     Represents an abstract class used for controlling serialization by the System.Xml.Serialization.XmlSerializer
    //     class.
    public abstract class XmlSerializationWriter : XmlSerializationGeneratedCode
    {
        //note: we assumed that this would not be used improperly so there are no tests on the elements enteredd as parameters.
        //todo: add these tests


        //test:
        // call: writer.Init(xmlWriter, namespaces, encodingStyle, id, null);
        private void Init(XmlWriter xmlWriter, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            _xmlWriter = xmlWriter;
            //  function Init (XmlReader r, XmlDeserializationEvents events, string encodingStyle, TempAssembly tempAssembly) {
  //    this.r = r;

  //    this.typeID = r.NameTable.Add("type");
  //    this.nullID = r.NameTable.Add("null");
  //    this.nilID = r.NameTable.Add("nil");

  //    this.instanceNsID = r.NameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
  //    this.instanceNs2000ID = r.NameTable.Add("http://www.w3.org/2000/10/XMLSchema-instance");
  //    this.instanceNs1999ID = r.NameTable.Add("http://www.w3.org/1999/XMLSchema-instance");

  //    this.InitIDs();
  //  }
  //);

        }

        XmlWriter _xmlWriter;
        
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializationWriter
        /// class.
        /// </summary>
        protected XmlSerializationWriter() { }

        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that a type being
        /// serialized is not being used in a valid manner or is unexpectedly encountered.
        /// </summary>
        /// <param name="o">The object whose type cannot be serialized.</param>
        /// <returns>The newly created exception.</returns>
        protected Exception CreateUnknownTypeException(object o) { return CreateUnknownTypeException(o.GetType()); }
        
        /// <summary>
        /// Creates an System.InvalidOperationException that indicates that a type being
        /// serialized is not being used in a valid manner or is unexpectedly encountered.
        /// </summary>
        /// <param name="type">The type that cannot be serialized.</param>
        /// <returns>The newly created exception.</returns>
        protected Exception CreateUnknownTypeException(Type type) { return new InvalidOperationException("Unknown type for serialization: " + type.FullName); }

        /// <summary>
        /// Produces a string from an input System.Char.
        /// </summary>
        /// <param name="value">A System.Char to translate to a string.</param>
        /// <returns>The System.Char value converted to a string.</returns>
        protected static string FromChar(char value)
        {
            return ((int)value).ToString();
        }

        /// <summary>
        /// Produces a string from an input System.DateTime.
        /// </summary>
        /// <param name="value">A System.DateTime to translate to a string.</param>
        /// <returns>A string representation of the System.DateTime that shows the date and time.</returns>
        protected static string FromDateTime(DateTime value)
        {
            return INTERNAL_DateTimeHelpers.FromDateTime(value);
        }
        
        protected abstract void InitCallbacks();


        /// <summary>
        /// Initializes object references only while serializing a SOAP-encoded SOAP
        /// message.
        /// </summary>
        protected void TopLevelElement() { } //we assume this is pretty much useless so we don't implement it yet. (no exception because it is called by the classes generated using sgen)

        /// <summary>
        /// Writes an XML element with a specified value in its body.
        /// </summary>
        /// <param name="localName">The local name of the XML element.</param>
        /// <param name="ns">The namespace of the XML element.</param>
        /// <param name="value">The text value of the XML element.</param>
        protected void WriteElementString(string localName, string ns, string value)
        {
            _xmlWriter.WriteStartElement(localName);//todo: add the namespace
            if (value != null) //todo: maybe add i:nil = true
            {
                _xmlWriter.WriteString(INTERNAL_EscapeHelpers.EscapeXml(value));
            }
            _xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an XML element with a specified value in its body.
        /// </summary>
        /// <param name="localName">The local name of the XML element.</param>
        /// <param name="ns">The namespace of the XML element.</param>
        /// <param name="value">The text value of the XML element.</param>
        protected void WriteElementStringRaw(string localName, string ns, string value)
        {
            if (value == null) return;
            _xmlWriter.WriteStartElement(localName);//todo: add the namespace
            _xmlWriter.WriteString(INTERNAL_EscapeHelpers.EscapeXml(value)); //todo: make it Raw (_xmlWriter.WriteStringRaw(value);)
            _xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes an XML element whose body is empty.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        protected void WriteEmptyTag(string name, string ns) //todo: implement this
        {
        }

        /// <summary>
        /// Writes a <closing> element tag.
        /// </summary>
        protected void WriteEndElement()
        {
            _xmlWriter.WriteEndElement();
        }
       
        /// <summary>
        /// Writes a <closing> element tag.
        /// </summary>
        /// <param name="o">The object being serialized.</param>
        protected void WriteEndElement(object o)
        {
            WriteEndElement(); //todo: find out what the object should do.
        }

        /// <summary>
        /// Writes an XML element that contains a string as the body. System.Xml.XmlWriter
        /// inserts an xsi:nil='true' attribute if the string's value is null.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        /// <param name="value">The string to write in the body of the XML element.</param>
        protected void WriteNullableStringLiteral(string name, string ns, string value)
        {
            if (value == null)
                WriteNullTagLiteral(name, ns);
            else
                WriteElementString(name, ns, value);
        }

        /// <summary>
        /// Writes an XML element that contains a string as the body. System.Xml.XmlWriter
        /// inserts a xsi:nil='true' attribute if the string's value is null.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        /// <param name="value">The string to write in the body of the XML element.</param>
        protected void WriteNullableStringLiteralRaw(string name, string ns, string value) //todo: implement this
        {
            if (value == null)
                WriteNullTagLiteral(name, ns);
            else
                WriteElementStringRaw(name, ns, value);
        }

        /// <summary>
        /// Writes an XML element with an xsi:nil='true' attribute.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        protected void WriteNullTagLiteral(string name, string ns)
        {
            if (name == null || name.Length == 0)
                return;
            WriteStartElement(name, ns, null, false, null);
            //_xmlWriter.WriteAttributeString("nil", XmlSchema.InstanceNamespace, "true");
            _xmlWriter.WriteAttributeString("xmlns:i", "http://www.w3.org/2001/XMLSchema-instance");
            _xmlWriter.WriteAttributeString("i:nil", "true");
            _xmlWriter.WriteEndElement();
            //todo: implement this (no NotImplementedException because it is called by the classes generated using sgen)
        }

        /// <summary>
        /// Writes the XML declaration if the writer is positioned at the start of an
        /// XML document.
        /// </summary>
        protected void WriteStartDocument()
        {
            _xmlWriter.WriteStartDocument();
        }

        /// <summary>
        /// Writes an opening element tag, including any attributes.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        /// <param name="o">The object being serialized as an XML element.</param>
        /// <param name="writePrefixed">
        /// true to write the element name with a prefix if none is available for the
        /// specified namespace; otherwise, false.
        /// </param>
        protected void WriteStartElement(string name, string ns, object o, bool writePrefixed)
        {
            WriteStartElement(name, ns, o, writePrefixed, null);
        }

        /// <summary>
        /// Writes an opening element tag, including any attributes.
        /// </summary>
        /// <param name="name">The local name of the XML element to write.</param>
        /// <param name="ns">The namespace of the XML element to write.</param>
        /// <param name="o">The object being serialized as an XML element.</param>
        /// <param name="writePrefixed">
        /// true to write the element name with a prefix if none is available for the
        /// specified namespace; otherwise, false.
        /// </param>
        /// <param name="xmlns">
        /// An instance of the System.Xml.Serialization.XmlSerializerNamespaces class
        /// that contains prefix and namespace pairs to be used in the generated XML.
        /// </param>
        protected void WriteStartElement(string name, string ns, object o, bool writePrefixed, XmlSerializerNamespaces xmlns)//this is muuuuch longer in the Microsoft reference source code so we are probably missing on something
        {
            if (string.IsNullOrWhiteSpace(ns))
            {
                // COMMENTED BECAUSE DOES NOT SEEM TO WORK (INVALID CAST EXCEPTION):
                //if (o != null)
                //{
                //    var attributes = o.GetType().GetCustomAttributes(typeof(System.Runtime.Serialization.DataContractAttribute), false);
                //    var dataContractAttribute = (System.Runtime.Serialization.DataContractAttribute)attributes[0];
                //    ns = dataContractAttribute.Namespace;

                //    _xmlWriter.WriteStartElement(name, ns); 
                //}
                //else
                //{
                    _xmlWriter.WriteStartElement(name);
                //}
            }
            else
            {
                _xmlWriter.WriteStartElement(name, ns);
            }
        }

        /// <summary>
        /// Writes an xsi:type attribute for an XML element that is being serialized
        /// into a document.
        /// </summary>
        /// <param name="name">The local name of an XML Schema data type.</param>
        /// <param name="ns">The namespace of an XML Schema data type.</param>
        protected void WriteXsiType(string name, string ns)
        {
            throw new NotImplementedException();
        }

        

        //// Summary:
        ////     Gets or sets a value that indicates whether the System.Xml.XmlConvert.EncodeName(System.String)
        ////     method is used to write valid XML.
        ////
        //// Returns:
        ////     true if the System.Xml.Serialization.XmlSerializationWriter.FromXmlQualifiedName(System.Xml.XmlQualifiedName)
        ////     method returns an encoded name; otherwise, false.
        //protected bool EscapeName { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a list of XML qualified name objects that contain the namespaces
        ////     and prefixes used to produce qualified names in XML documents.
        ////
        //// Returns:
        ////     An System.Collections.ArrayList that contains the namespaces and prefix pairs.
        //protected ArrayList Namespaces { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the System.Xml.XmlWriter that is being used by the System.Xml.Serialization.XmlSerializationWriter.
        ////
        //// Returns:
        ////     The System.Xml.XmlWriter used by the class instance.
        //protected XmlWriter Writer { get; set; }

        //// Summary:
        ////     Stores an implementation of the System.Xml.Serialization.XmlSerializationWriteCallback
        ////     delegate and the type it applies to, for a later invocation.
        ////
        //// Parameters:
        ////   type:
        ////     The System.Type of objects that are serialized.
        ////
        ////   typeName:
        ////     The name of the type of objects that are serialized.
        ////
        ////   typeNs:
        ////     The namespace of the type of objects that are serialized.
        ////
        ////   callback:
        ////     An instance of the System.Xml.Serialization.XmlSerializationWriteCallback
        ////     delegate.
        //protected void AddWriteCallback(Type type, string typeName, string typeNs, XmlSerializationWriteCallback callback);
        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates an unexpected
        ////     name for an element that adheres to an XML Schema choice element declaration.
        ////
        //// Parameters:
        ////   value:
        ////     The name that is not valid.
        ////
        ////   identifier:
        ////     The choice element declaration that the name belongs to.
        ////
        ////   name:
        ////     The expected local name of an element.
        ////
        ////   ns:
        ////     The expected namespace of an element.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateChoiceIdentifierValueException(string value, string identifier, string name, string ns);
        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates the System.Xml.Serialization.XmlAnyElementAttribute
        ////     which has been invalidly applied to a member; only members that are of type
        ////     System.Xml.XmlNode, or derived from System.Xml.XmlNode, are valid.
        ////
        //// Parameters:
        ////   o:
        ////     The object that represents the invalid member.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateInvalidAnyTypeException(object o);
        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates the System.Xml.Serialization.XmlAnyElementAttribute
        ////     which has been invalidly applied to a member; only members that are of type
        ////     System.Xml.XmlNode, or derived from System.Xml.XmlNode, are valid.
        ////
        //// Parameters:
        ////   type:
        ////     The System.Type that is invalid.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateInvalidAnyTypeException(Type type);
        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates a failure while
        ////     writing an array where an XML Schema choice element declaration is applied.
        ////
        //// Parameters:
        ////   type:
        ////     The type being serialized.
        ////
        ////   identifier:
        ////     A name for the choice element declaration.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateInvalidChoiceIdentifierValueException(string type, string identifier);

        /// <summary>
        /// Creates an System.InvalidOperationException for an invalid enumeration value.
        /// </summary>
        /// <param name="value">An object that represents the invalid enumeration.</param>
        /// <param name="typeName">The XML type name.</param>
        /// <returns>The newly created exception.</returns>
        protected Exception CreateInvalidEnumValueException(object value, string typeName) { return new InvalidOperationException("Invalid enum value '" + value + "' for type '" + typeName + "'."); }

        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates that a value for
        ////     an XML element does not match an enumeration type.
        ////
        //// Parameters:
        ////   value:
        ////     The value that is not valid.
        ////
        ////   elementName:
        ////     The name of the XML element with an invalid value.
        ////
        ////   enumValue:
        ////     The valid value.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateMismatchChoiceException(string value, string elementName, string enumValue);
        ////
        //// Summary:
        ////     Creates an System.InvalidOperationException that indicates that an XML element
        ////     that should adhere to the XML Schema any element declaration cannot be processed.
        ////
        //// Parameters:
        ////   name:
        ////     The XML element that cannot be processed.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        //// Returns:
        ////     The newly created exception.
        //protected Exception CreateUnknownAnyElementException(string name, string ns);

        //
        // Summary:
        //     Processes a base-64 byte array.
        //
        // Parameters:
        //   value:
        //     A base-64 System.Byte array.
        //
        // Returns:
        //     The same byte array that was passed in as an argument.
        protected static byte[] FromByteArrayBase64(byte[] value)
        {
            return value;
        }

        ////
        //// Summary:
        ////     Produces a string from an input hexadecimal byte array.
        ////
        //// Parameters:
        ////   value:
        ////     A hexadecimal byte array to translate to a string.
        ////
        //// Returns:
        ////     The byte array value converted to a string.
        //protected static string FromByteArrayHex(byte[] value);
        
        ////
        //// Summary:
        ////     Produces a string from a System.DateTime object.
        ////
        //// Parameters:
        ////   value:
        ////     A System.DateTime to translate to a string.
        ////
        //// Returns:
        ////     A string representation of the System.DateTime that shows the date but no
        ////     time.
        //protected static string FromDate(DateTime value);
        
        ////
        //// Summary:
        ////     Produces a string that consists of delimited identifiers that represent the
        ////     enumeration members that have been set.
        ////
        //// Parameters:
        ////   value:
        ////     The enumeration value as a series of bitwise OR operations.
        ////
        ////   values:
        ////     The enumeration's name values.
        ////
        ////   ids:
        ////     The enumeration's constant values.
        ////
        //// Returns:
        ////     A string that consists of delimited identifiers, where each represents a
        ////     member from the set enumerator list.
        //protected static string FromEnum(long value, string[] values, long[] ids);
        ////
        //// Summary:
        ////     Takes a numeric enumeration value and the names and constants from the enumerator
        ////     list for the enumeration and returns a string that consists of delimited
        ////     identifiers that represent the enumeration members that have been set.
        ////
        //// Parameters:
        ////   value:
        ////     The enumeration value as a series of bitwise OR operations.
        ////
        ////   values:
        ////     The values of the enumeration.
        ////
        ////   ids:
        ////     The constants of the enumeration.
        ////
        ////   typeName:
        ////     The name of the type
        ////
        //// Returns:
        ////     A string that consists of delimited identifiers, where each item is one of
        ////     the values set by the bitwise operation.
        //protected static string FromEnum(long value, string[] values, long[] ids, string typeName);
        ////
        //// Summary:
        ////     Produces a string from a System.DateTime object.
        ////
        //// Parameters:
        ////   value:
        ////     A System.DateTime that is translated to a string.
        ////
        //// Returns:
        ////     A string representation of the System.DateTime object that shows the time
        ////     but no date.
        //protected static string FromTime(DateTime value);
        ////
        //// Summary:
        ////     Encodes a valid XML name by replacing characters that are not valid with
        ////     escape sequences.
        ////
        //// Parameters:
        ////   name:
        ////     A string to be used as an XML name.
        ////
        //// Returns:
        ////     An encoded string.
        //protected static string FromXmlName(string name);
        ////
        //// Summary:
        ////     Encodes a valid XML local name by replacing characters that are not valid
        ////     with escape sequences.
        ////
        //// Parameters:
        ////   ncName:
        ////     A string to be used as a local (unqualified) XML name.
        ////
        //// Returns:
        ////     An encoded string.
        //protected static string FromXmlNCName(string ncName);
        ////
        //// Summary:
        ////     Encodes an XML name.
        ////
        //// Parameters:
        ////   nmToken:
        ////     An XML name to be encoded.
        ////
        //// Returns:
        ////     An encoded string.
        //protected static string FromXmlNmToken(string nmToken);
        ////
        //// Summary:
        ////     Encodes a space-delimited sequence of XML names into a single XML name.
        ////
        //// Parameters:
        ////   nmTokens:
        ////     A space-delimited sequence of XML names to be encoded.
        ////
        //// Returns:
        ////     An encoded string.
        //protected static string FromXmlNmTokens(string nmTokens);
        ////
        //// Summary:
        ////     Returns an XML qualified name, with invalid characters replaced by escape
        ////     sequences.
        ////
        //// Parameters:
        ////   xmlQualifiedName:
        ////     An System.Xml.XmlQualifiedName that represents the XML to be written.
        ////
        //// Returns:
        ////     An XML qualified name, with invalid characters replaced by escape sequences.
        //protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName);
        ////
        //// Summary:
        ////     Produces a string that can be written as an XML qualified name, with invalid
        ////     characters replaced by escape sequences.
        ////
        //// Parameters:
        ////   xmlQualifiedName:
        ////     An System.Xml.XmlQualifiedName that represents the XML to be written.
        ////
        ////   ignoreEmpty:
        ////     true to ignore empty spaces in the string; otherwise, false.
        ////
        //// Returns:
        ////     An XML qualified name, with invalid characters replaced by escape sequences.
        //protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName, bool ignoreEmpty);
        
        ////
        //// Summary:
        ////     Gets a dynamically generated assembly by name.
        ////
        //// Parameters:
        ////   assemblyFullName:
        ////     The full name of the assembly.
        ////
        //// Returns:
        ////     A dynamically generated assembly.
        //protected static Assembly ResolveDynamicAssembly(string assemblyFullName);
        
        ////
        //// Summary:
        ////     Instructs an System.Xml.XmlWriter object to write an XML attribute that has
        ////     no namespace specified for its name.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML attribute.
        ////
        ////   value:
        ////     The value of the XML attribute as a byte array.
        //protected void WriteAttribute(string localName, byte[] value);
        ////
        //// Summary:
        ////     Instructs the System.Xml.XmlWriter to write an XML attribute that has no
        ////     namespace specified for its name.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML attribute.
        ////
        ////   value:
        ////     The value of the XML attribute as a string.
        //protected void WriteAttribute(string localName, string value);
        ////
        //// Summary:
        ////     Instructs an System.Xml.XmlWriter object to write an XML attribute.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML attribute.
        ////
        ////   ns:
        ////     The namespace of the XML attribute.
        ////
        ////   value:
        ////     The value of the XML attribute as a byte array.
        //protected void WriteAttribute(string localName, string ns, byte[] value);
        ////
        //// Summary:
        ////     Writes an XML attribute.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML attribute.
        ////
        ////   ns:
        ////     The namespace of the XML attribute.
        ////
        ////   value:
        ////     The value of the XML attribute as a string.
        //protected void WriteAttribute(string localName, string ns, string value);
        ////
        //// Summary:
        ////     Writes an XML attribute where the namespace prefix is provided manually.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix to write.
        ////
        ////   localName:
        ////     The local name of the XML attribute.
        ////
        ////   ns:
        ////     The namespace represented by the prefix.
        ////
        ////   value:
        ////     The value of the XML attribute as a string.
        //protected void WriteAttribute(string prefix, string localName, string ns, string value);
        ////
        //// Summary:
        ////     Writes an XML node object within the body of a named XML element.
        ////
        //// Parameters:
        ////   node:
        ////     The XML node to write, possibly a child XML element.
        ////
        ////   name:
        ////     The local name of the parent XML element to write.
        ////
        ////   ns:
        ////     The namespace of the parent XML element to write.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the object to serialize is null;
        ////     otherwise, false.
        ////
        ////   any:
        ////     true to indicate that the node, if an XML element, adheres to an XML Schema
        ////     any element declaration; otherwise, false.
        //protected void WriteElementEncoded(XmlNode node, string name, string ns, bool isNullable, bool any);
        ////
        //// Summary:
        ////     Instructs an System.Xml.XmlWriter object to write an System.Xml.XmlNode object
        ////     within the body of a named XML element.
        ////
        //// Parameters:
        ////   node:
        ////     The XML node to write, possibly a child XML element.
        ////
        ////   name:
        ////     The local name of the parent XML element to write.
        ////
        ////   ns:
        ////     The namespace of the parent XML element to write.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the object to serialize is null;
        ////     otherwise, false.
        ////
        ////   any:
        ////     true to indicate that the node, if an XML element, adheres to an XML Schema
        ////     any element declaration; otherwise, false.
        //protected void WriteElementLiteral(XmlNode node, string name, string ns, bool isNullable, bool any);
        ////
        //// Summary:
        ////     Writes an XML element with a specified qualified name in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The name to write, using its prefix if namespace-qualified, in the element
        ////     text.
        //protected void WriteElementQualifiedName(string localName, XmlQualifiedName value);
        ////
        //// Summary:
        ////     Writes an XML element with a specified qualified name in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        ////   value:
        ////     The name to write, using its prefix if namespace-qualified, in the element
        ////     text.
        //protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value);
        ////
        //// Summary:
        ////     Writes an XML element with a specified qualified name in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The name to write, using its prefix if namespace-qualified, in the element
        ////     text.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementQualifiedName(string localName, XmlQualifiedName value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified qualified name in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        ////   value:
        ////     The name to write, using its prefix if namespace-qualified, in the element
        ////     text.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element to be written without namespace qualification.
        ////
        ////   value:
        ////     The text value of the XML element.
        //protected void WriteElementString(string localName, string value);
        
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementString(string localName, string value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementString(string localName, string ns, string value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        //protected void WriteElementStringRaw(string localName, byte[] value);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        //protected void WriteElementStringRaw(string localName, string value);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementStringRaw(string localName, byte[] value, XmlQualifiedName xsiType);

        /// <summary>
        /// Writes an XML element with a specified value in its body.
        /// </summary>
        /// <param name="localName">The local name of the XML element.</param>
        /// <param name="ns">The namespace of the XML element.</param>
        /// <param name="value">The text value of the XML element.</param>
        protected void WriteElementStringRaw(string localName, string ns, byte[] value)
        {
            if (value == null) return;
            _xmlWriter.WriteStartElement(localName);//todo: add the namespace
            //_xmlWriter.WriteString(INTERNAL_EscapeHelpers.EscapeXml(Encoding.UTF8.GetString(value))); //todo: make it Raw (_xmlWriter.WriteStringRaw(...);)
            _xmlWriter.WriteString(INTERNAL_EscapeHelpers.EscapeXml(Convert.ToBase64String(value))); //todo: make it Raw (_xmlWriter.WriteStringRaw(...);)
            _xmlWriter.WriteEndElement();
        }
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementStringRaw(string localName, string value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementStringRaw(string localName, string ns, byte[] value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element with a specified value in its body.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the XML element.
        ////
        ////   ns:
        ////     The namespace of the XML element.
        ////
        ////   value:
        ////     The text value of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteElementStringRaw(string localName, string ns, string value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element whose body is empty.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        //protected void WriteEmptyTag(string name);
        
        
        ////
        //// Summary:
        ////     Writes an id attribute that appears in a SOAP-encoded multiRef element.
        ////
        //// Parameters:
        ////   o:
        ////     The object being serialized.
        //protected void WriteId(object o);
        ////
        //// Summary:
        ////     Writes the namespace declaration attributes.
        ////
        //// Parameters:
        ////   xmlns:
        ////     The XML namespaces to declare.
        //protected void WriteNamespaceDeclarations(XmlSerializerNamespaces xmlns);
        ////
        //// Summary:
        ////     Writes an XML element whose body contains a valid XML qualified name. System.Xml.XmlWriter
        ////     inserts an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The XML qualified name to write in the body of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteNullableQualifiedNameEncoded(string name, string ns, XmlQualifiedName value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element whose body contains a valid XML qualified name. System.Xml.XmlWriter
        ////     inserts an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The XML qualified name to write in the body of the XML element.
        //protected void WriteNullableQualifiedNameLiteral(string name, string ns, XmlQualifiedName value);
        ////
        //// Summary:
        ////     Writes an XML element that contains a string as the body. System.Xml.XmlWriter
        ////     inserts an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The string to write in the body of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteNullableStringEncoded(string name, string ns, string value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes a byte array as the body of an XML element. System.Xml.XmlWriter inserts
        ////     an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The byte array to write in the body of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteNullableStringEncodedRaw(string name, string ns, byte[] value, XmlQualifiedName xsiType);
        ////
        //// Summary:
        ////     Writes an XML element that contains a string as the body. System.Xml.XmlWriter
        ////     inserts an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The string to write in the body of the XML element.
        ////
        ////   xsiType:
        ////     The name of the XML Schema data type to be written to the xsi:type attribute.
        //protected void WriteNullableStringEncodedRaw(string name, string ns, string value, XmlQualifiedName xsiType);
        
        ////
        //// Summary:
        ////     Writes a byte array as the body of an XML element. System.Xml.XmlWriter inserts
        ////     an xsi:nil='true' attribute if the string's value is null.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   value:
        ////     The byte array to write in the body of the XML element.
        //protected void WriteNullableStringLiteralRaw(string name, string ns, byte[] value);
        
        ////
        //// Summary:
        ////     Writes an XML element with an xsi:nil='true' attribute.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        //protected void WriteNullTagEncoded(string name);
        ////
        //// Summary:
        ////     Writes an XML element with an xsi:nil='true' attribute.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        //protected void WriteNullTagEncoded(string name, string ns);
        ////
        //// Summary:
        ////     Writes an XML element with an xsi:nil='true' attribute.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        //protected void WriteNullTagLiteral(string name);
        
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that can contain a reference to a <multiRef>
        ////     XML element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   o:
        ////     The object being serialized either in the current XML element or a multiRef
        ////     element that is referenced by the current element.
        //protected void WritePotentiallyReferencingElement(string n, string ns, object o);
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that can contain a reference to a <multiRef>
        ////     XML element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   o:
        ////     The object being serialized either in the current XML element or a multiRef
        ////     element that referenced by the current element.
        ////
        ////   ambientType:
        ////     The type stored in the object's type mapping (as opposed to the object's
        ////     type found directly through the typeof operation).
        //protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType);
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that can contain a reference to a <multiRef>
        ////     XML element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   o:
        ////     The object being serialized either in the current XML element or a multiRef
        ////     element that is referenced by the current element.
        ////
        ////   ambientType:
        ////     The type stored in the object's type mapping (as opposed to the object's
        ////     type found directly through the typeof operation).
        ////
        ////   suppressReference:
        ////     true to serialize the object directly into the XML element rather than make
        ////     the element reference another element that contains the data; otherwise,
        ////     false.
        //protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference);
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that can contain a reference to a multiRef
        ////     XML element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   o:
        ////     The object being serialized either in the current XML element or a multiRef
        ////     element that referenced by the current element.
        ////
        ////   ambientType:
        ////     The type stored in the object's type mapping (as opposed to the object's
        ////     type found directly through the typeof operation).
        ////
        ////   suppressReference:
        ////     true to serialize the object directly into the XML element rather than make
        ////     the element reference another element that contains the data; otherwise,
        ////     false.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the object to serialize is null;
        ////     otherwise, false.
        //protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference, bool isNullable);
        ////
        //// Summary:
        ////     Serializes objects into SOAP-encoded multiRef XML elements in a SOAP message.
        //protected void WriteReferencedElements();
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that contains a reference to a multiRef
        ////     element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the referencing element being written.
        ////
        ////   ns:
        ////     The namespace of the referencing element being written.
        ////
        ////   o:
        ////     The object being serialized.
        //protected void WriteReferencingElement(string n, string ns, object o);
        ////
        //// Summary:
        ////     Writes a SOAP message XML element that contains a reference to a multiRef
        ////     element for a given object.
        ////
        //// Parameters:
        ////   n:
        ////     The local name of the referencing element being written.
        ////
        ////   ns:
        ////     The namespace of the referencing element being written.
        ////
        ////   o:
        ////     The object being serialized.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the object to serialize is null;
        ////     otherwise, false.
        //protected void WriteReferencingElement(string n, string ns, object o, bool isNullable);
        ////
        //// Summary:
        ////     Writes a SOAP 1.2 RPC result element with a specified qualified name in its
        ////     body.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the result body.
        ////
        ////   ns:
        ////     The namespace of the result body.
        //protected void WriteRpcResult(string name, string ns);
        ////
        //// Summary:
        ////     Writes an object that uses custom XML formatting as an XML element.
        ////
        //// Parameters:
        ////   serializable:
        ////     An object that implements the System.Xml.Serialization.IXmlSerializable interface
        ////     that uses custom XML formatting.
        ////
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the System.Xml.Serialization.IXmlSerializable
        ////     class object is null; otherwise, false.
        //protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable);
        ////
        //// Summary:
        ////     Instructs System.Xml.XmlNode to write an object that uses custom XML formatting
        ////     as an XML element.
        ////
        //// Parameters:
        ////   serializable:
        ////     An object that implements the System.Xml.Serialization.IXmlSerializable interface
        ////     that uses custom XML formatting.
        ////
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   isNullable:
        ////     true to write an xsi:nil='true' attribute if the System.Xml.Serialization.IXmlSerializable
        ////     object is null; otherwise, false.
        ////
        ////   wrapped:
        ////     true to ignore writing the opening element tag; otherwise, false to write
        ////     the opening element tag.
        //protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable, bool wrapped);
        
        ////
        //// Summary:
        ////     Writes an opening element tag, including any attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        //protected void WriteStartElement(string name);
        ////
        //// Summary:
        ////     Writes an opening element tag, including any attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        //protected void WriteStartElement(string name, string ns);
        ////
        //// Summary:
        ////     Writes an opening element tag, including any attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   writePrefixed:
        ////     true to write the element name with a prefix if none is available for the
        ////     specified namespace; otherwise, false.
        //protected void WriteStartElement(string name, string ns, bool writePrefixed);
        ////
        //// Summary:
        ////     Writes an opening element tag, including any attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the XML element to write.
        ////
        ////   ns:
        ////     The namespace of the XML element to write.
        ////
        ////   o:
        ////     The object being serialized as an XML element.
        //protected void WriteStartElement(string name, string ns, object o);
        
        
        ////
        //// Summary:
        ////     Writes an XML element whose text body is a value of a simple XML Schema data
        ////     type.
        ////
        //// Parameters:
        ////   name:
        ////     The local name of the element to write.
        ////
        ////   ns:
        ////     The namespace of the element to write.
        ////
        ////   o:
        ////     The object to be serialized in the element body.
        ////
        ////   xsiType:
        ////     true if the XML element explicitly specifies the text value's type using
        ////     the xsi:type attribute; otherwise, false.
        //protected void WriteTypedPrimitive(string name, string ns, object o, bool xsiType);
        ////
        //// Summary:
        ////     Writes a base-64 byte array.
        ////
        //// Parameters:
        ////   value:
        ////     The byte array to write.
        //protected void WriteValue(byte[] value);
        ////
        //// Summary:
        ////     Writes a specified string value.
        ////
        //// Parameters:
        ////   value:
        ////     The value of the string to write.
        //protected void WriteValue(string value);
        ////
        //// Summary:
        ////     Writes the specified System.Xml.XmlNode as an XML attribute.
        ////
        //// Parameters:
        ////   node:
        ////     The XML node to write.
        //protected void WriteXmlAttribute(XmlNode node);
        ////
        //// Summary:
        ////     Writes the specified System.Xml.XmlNode object as an XML attribute.
        ////
        //// Parameters:
        ////   node:
        ////     The XML node to write.
        ////
        ////   container:
        ////     An System.Xml.Schema.XmlSchemaObject object (or null) used to generate a
        ////     qualified name value for an arrayType attribute from the Web Services Description
        ////     Language (WSDL) namespace ("http://schemas.xmlsoap.org/wsdl/").
        //protected void WriteXmlAttribute(XmlNode node, object container);
        
    }
}
