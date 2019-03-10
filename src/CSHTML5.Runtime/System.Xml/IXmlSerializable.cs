
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
using System.Xml;

namespace System.Xml.Serialization
{
    /// <summary>
    /// Provides custom formatting for XML serialization and deserialization.
    /// </summary>
    public interface IXmlSerializable
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
