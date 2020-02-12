
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



// NOTE: Some of the types below may require to be moved to other DLLs.

using System;

namespace System.Xml.Schema
{

    using System.Xml.Serialization;

    /// <summary>
    /// Indicates if attributes or elements need to be qualified with a namespace
    /// prefix.
    /// </summary>
    public enum XmlSchemaForm
    {
        /// <summary>
        /// Element and attribute form is not specified in the schema.
        /// </summary>
        [XmlIgnore]
        None,
        /// <summary>
        /// Elements and attributes must be qualified with a namespace prefix.
        /// </summary>
        [XmlEnum("qualified")]
        Qualified,
        /// <summary>
        /// Elements and attributes are not required to be qualified with a namespace
        /// prefix.
        /// </summary>
        [XmlEnum("unqualified")]
        Unqualified,
    }
}

namespace System.Xml.Serialization
{
    /// <summary>
    /// Allows the System.Xml.Serialization.XmlSerializer to recognize a type when
    /// it serializes or deserializes an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public partial class XmlIncludeAttribute : System.Attribute
    {
        Type type;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlIncludeAttribute
        /// class.
        /// </summary>
        /// <param name="type">The System.Type of the object to include.</param>
        public XmlIncludeAttribute(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets or sets the type of the object to include.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}

namespace System.Xml.Serialization
{
    using System;

    /// <summary>
    /// Controls the XML schema that is generated when the attribute target is serialized
    /// by the System.Xml.Serialization.XmlSerializer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
    public partial class XmlTypeAttribute : System.Attribute
    {
        bool includeInSchema = true;
        bool anonymousType;
        string ns;
        string typeName;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlTypeAttribute
        /// class.
        /// </summary>
        public XmlTypeAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlTypeAttribute
        /// class and specifies the name of the XML type.
        /// </summary>
        /// <param name="typeName">
        /// The name of the XML type that the System.Xml.Serialization.XmlSerializer
        /// generates when it serializes the class instance (and recognizes when it deserializes
        /// the class instance).
        /// </param>
        public XmlTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }

        /// <summary>
        /// Gets or sets a value that determines whether the resulting schema type is
        /// an XSD anonymous type.
        /// </summary>
        public bool AnonymousType
        {
            get { return anonymousType; }
            set { anonymousType = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the type in XML schema
        /// documents.
        /// </summary>
        public bool IncludeInSchema
        {
            get { return includeInSchema; }
            set { includeInSchema = value; }
        }

        /// <summary>
        /// Gets or sets the name of the XML type.
        /// </summary>
        public string TypeName
        {
            get { return typeName == null ? string.Empty : typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the namespace of the XML type.
        /// </summary>
        public string Namespace
        {
            get { return ns; }
            set { ns = value; }
        }
    }
}

namespace System.Xml.Serialization
{

    using System;

    /// <summary>
    /// Instructs the System.Xml.Serialization.XmlSerializer.Serialize(System.IO.TextWriter,System.Object)
    /// method of the System.Xml.Serialization.XmlSerializer not to serialize the
    /// public field or public read/write property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public partial class XmlIgnoreAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlIgnoreAttribute
        /// class.
        /// </summary>
        public XmlIgnoreAttribute()
        {
        }
    }
}

namespace System.Xml.Serialization
{

    using System;
    using System.Xml.Schema;

    /// <summary>
    /// Specifies that the System.Xml.Serialization.XmlSerializer must serialize
    /// a particular class member as an array of XML elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
    public partial class XmlArrayAttribute : System.Attribute
    {
        string elementName;
        string ns;
        bool nullable;
        XmlSchemaForm form = XmlSchemaForm.None;
        int order = -1;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayAttribute
        /// class.
        /// </summary>
        public XmlArrayAttribute()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayAttribute
        /// class and specifies the XML element name generated in the XML document instance.
        /// </summary>
        /// <param name="elementName">
        /// The name of the XML element that the System.Xml.Serialization.XmlSerializer
        /// generates.
        /// </param>
        public XmlArrayAttribute(string elementName)
        {
            this.elementName = elementName;
        }
        
        /// <summary>
        /// Gets or sets the XML element name given to the serialized array.
        /// </summary>
        public string ElementName
        {
            get { return elementName == null ? string.Empty : elementName; }
            set { elementName = value; }
        }
       
        /// <summary>
        /// Gets or sets the namespace of the XML element.
        /// </summary>
        public string Namespace
        {
            get { return ns; }
            set { ns = value; }
        }
        
        /// <summary>
        /// Gets or sets a value that indicates whether the System.Xml.Serialization.XmlSerializer
        /// must serialize a member as an empty XML tag with the xsi:nil attribute set
        /// to true.
        /// </summary>
        public bool IsNullable
        {
            get { return nullable; }
            set { nullable = value; }
        }
        
        /// <summary>
        /// Gets or sets a value that indicates whether the XML element name generated
        /// by the System.Xml.Serialization.XmlSerializer is qualified or unqualified.
        /// </summary>
        public XmlSchemaForm Form
        {
            get { return form; }
            set { form = value; }
        }
       
        /// <summary>
        /// Gets or sets the explicit order in which the elements are serialized or deserialized.
        /// </summary>
        public int Order
        {
            get { return order; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Order cannot be negative.");
                order = value;
            }
        }
    }
}

namespace System.Xml.Serialization
{

    using System;

    /// <summary>
    /// Controls how the System.Xml.Serialization.XmlSerializer serializes an enumeration
    /// member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public partial class XmlEnumAttribute : System.Attribute
    {
        string name;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlEnumAttribute
        /// class.
        /// </summary>
        public XmlEnumAttribute()
        {
        }
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlEnumAttribute
        /// class, and specifies the XML value that the System.Xml.Serialization.XmlSerializer
        /// generates or recognizes (when it serializes or deserializes the enumeration,
        /// respectively).
        /// </summary>
        /// <param name="name">The overriding name of the enumeration member.</param>
        public XmlEnumAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets or sets the value generated in an XML-document instance when the System.Xml.Serialization.XmlSerializer
        /// serializes an enumeration, or the value recognized when it deserializes the
        /// enumeration member.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}

namespace System.Xml.Serialization
{

    using System;
    using System.Xml.Schema;

    /// <summary>
    /// Represents an attribute that specifies the derived types that the System.Xml.Serialization.XmlSerializer
    /// can place in a serialized array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    public partial class XmlArrayItemAttribute : System.Attribute
    {
        string elementName;
        Type type;
        string ns;
        string dataType;
        bool nullable;
        bool nullableSpecified = false;
        XmlSchemaForm form = XmlSchemaForm.None;
        int nestingLevel;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayItemAttribute
        /// class.
        /// </summary>
        public XmlArrayItemAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayItemAttribute
        /// class and specifies the name of the XML element generated in the XML document.
        /// </summary>
        /// <param name="elementName">The name of the XML element.</param>
        public XmlArrayItemAttribute(string elementName)
        {
            this.elementName = elementName;
        }
      
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayItemAttribute
        /// class and specifies the System.Type that can be inserted into the serialized
        /// array.
        /// </summary>
        /// <param name="type">The System.Type of the object to serialize.</param>
        public XmlArrayItemAttribute(Type type)
        {
            this.type = type;
        }
     
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlArrayItemAttribute
        /// class and specifies the name of the XML element generated in the XML document
        /// and the System.Type that can be inserted into the generated XML document.
        /// </summary>
        /// <param name="elementName">The name of the XML element.</param>
        /// <param name="type">The System.Type of the object to serialize.</param>
        public XmlArrayItemAttribute(string elementName, Type type)
        {
            this.elementName = elementName;
            this.type = type;
        }
       
        /// <summary>
        /// Gets or sets the type allowed in an array.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
        
        /// <summary>
        /// Gets or sets the name of the generated XML element.
        /// </summary>
        public string ElementName
        {
            get { return elementName == null ? string.Empty : elementName; }
            set { elementName = value; }
        }
        
        /// <summary>
        /// Gets or sets the namespace of the generated XML element.
        /// </summary>
        public string Namespace
        {
            get { return ns; }
            set { ns = value; }
        }
       
        /// <summary>
        /// Gets or sets the level in a hierarchy of XML elements that the System.Xml.Serialization.XmlArrayItemAttribute
        /// affects.
        /// </summary>
        public int NestingLevel
        {
            get { return nestingLevel; }
            set { nestingLevel = value; }
        }
      
        /// <summary>
        /// Gets or sets the XML data type of the generated XML element.
        /// </summary>
        public string DataType
        {
            get { return dataType == null ? string.Empty : dataType; }
            set { dataType = value; }
        }
      
        /// <summary>
        /// Gets or sets a value that indicates whether the System.Xml.Serialization.XmlSerializer
        /// must serialize a member as an empty XML tag with the xsi:nil attribute set
        /// to true.
        /// </summary>
        public bool IsNullable
        {
            get { return nullable; }
            set { nullable = value; nullableSpecified = true; }
        }

        internal bool IsNullableSpecified
        {
            get { return nullableSpecified; }
        }
      
        // Exceptions:
        //   System.Exception:
        //     The System.Xml.Serialization.XmlArrayItemAttribute.Form property is set to
        //     XmlSchemaForm.Unqualified and a System.Xml.Serialization.XmlArrayItemAttribute.Namespace
        //     value is specified.
        /// <summary>
        /// Gets or sets a value that indicates whether the name of the generated XML
        /// element is qualified.
        /// </summary>
        public XmlSchemaForm Form
        {
            get { return form; }
            set { form = value; }
        }
    }
}

namespace System.Xml.Serialization
{
    using System;
    using System.Xml.Schema;
    /// <summary>
    /// Controls XML serialization of the attribute target as an XML root element.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct)]
    public partial class XmlRootAttribute : System.Attribute
    {
        string elementName;
        string ns;
        string dataType;
        bool nullable = true;
        bool nullableSpecified;
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlRootAttribute
        /// class.
        /// </summary>
        public XmlRootAttribute()
        {
        }
       
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlRootAttribute
        /// class and specifies the name of the XML root element.
        /// </summary>
        /// <param name="elementName">The name of the XML root element.</param>
        public XmlRootAttribute(string elementName)
        {
            this.elementName = elementName;
        }

        /// <summary>
        /// Gets or sets the name of the XML element that is generated and recognized
        /// by the System.Xml.Serialization.XmlSerializer class's System.Xml.Serialization.XmlSerializer.Serialize(System.IO.TextWriter,System.Object)
        /// and System.Xml.Serialization.XmlSerializer.Deserialize(System.IO.Stream)
        /// methods, respectively.
        /// </summary>
        public string ElementName
        {
            get { return elementName == null ? string.Empty : elementName; }
            set { elementName = value; }
        }

        /// <summary>
        /// Gets or sets the namespace for the XML root element.
        /// </summary>
        public string Namespace
        {
            get { return ns; }
            set { ns = value; }
        }

        /// <summary>
        /// Gets or sets the XSD data type of the XML root element.
        /// </summary>
        public string DataType
        {
            get { return dataType == null ? string.Empty : dataType; }
            set { dataType = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the System.Xml.Serialization.XmlSerializer
        /// must serialize a member that is set to null into the xsi:nil attribute set
        /// to true.
        /// </summary>
        public bool IsNullable
        {
            get { return nullable; }
            set
            {
                nullable = value;
                nullableSpecified = true;
            }
        }

        internal bool IsNullableSpecified
        {
            get { return nullableSpecified; }
        }

        internal string Key
        {
            get { return (ns == null ? String.Empty : ns) + ":" + ElementName + ":" + nullable.ToString(); }
        }
    }
}