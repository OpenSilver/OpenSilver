
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
using System.Xml;

namespace System.Xml.Serialization
{
    /// <summary>
    /// Provides custom formatting for XML serialization and deserialization.
    /// </summary>
    public partial interface IXmlSerializable
    {
        //// Summary:
        ////     This method is reserved and should not be used. When implementing the IXmlSerializable
        ////     interface, you should return null (Nothing in Visual Basic) from this method,
        ////     and instead, if specifying a custom schema is required, apply the System.Xml.Serialization.XmlSchemaProviderAttribute
        ////     to the class.
        ////
        //// Returns:
        ////     An System.Xml.Schema.XmlSchema that describes the XML representation of the
        ////     object that is produced by the System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)
        ////     method and consumed by the System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)
        ////     method.
        //XmlSchema GetSchema();

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        void ReadXml(XmlReader reader);
        ////
        //// Summary:
        ////     Converts an object into its XML representation.
        ////
        //// Parameters:
        ////   writer:
        ////     The System.Xml.XmlWriter stream to which the object is serialized.
        //void WriteXml(XmlWriter writer);
    }
}
