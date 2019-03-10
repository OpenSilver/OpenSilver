
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

namespace System.Xml
{
    /// <summary>
    /// Specifies the type of node.
    /// </summary>
    public enum XmlNodeType
    {
        /// <summary>
        /// This is returned by the System.Xml.XmlReader if a Read method has not been
        /// called.
        /// </summary>
        None = 0,
        /// <summary>
        /// An element.
        /// </summary>
        Element = 1,
        /// <summary>
        /// An attribute (for example, id='123').
        /// </summary>
        Attribute = 2,
        /// <summary>
        /// The text content of a node.
        /// </summary>
        Text = 3,
        /// <summary>
        /// A CDATA section.
        /// </summary>
        CDATA = 4,
        /// <summary>
        /// A reference to an entity.
        /// </summary>
        EntityReference = 5,
        /// <summary>
        /// An entity declaration.
        /// </summary>
        Entity = 6,
        /// <summary>
        /// A processing instruction.
        /// </summary>
        ProcessingInstruction = 7,
        /// <summary>
        /// A comment.
        /// </summary>
        Comment = 8,
        /// <summary>
        /// A document object that, as the root of the document tree, provides access
        /// to the entire XML document.
        /// </summary>
        Document = 9,
        /// <summary>
        /// The document type declaration, indicated by the following tag.
        /// </summary>
        DocumentType = 10,
        /// <summary>
        /// A document fragment.
        /// </summary>
        DocumentFragment = 11,
        /// <summary>
        /// A notation in the document type declaration.
        /// </summary>
        Notation = 12,
        /// <summary>
        /// White space between markup.
        /// </summary>
        Whitespace = 13,
        /// <summary>
        /// White space between markup in a mixed content model or white space within
        /// the xml:space="preserve" scope.
        /// </summary>
        SignificantWhitespace = 14,
        /// <summary>
        /// An end element tag.
        /// </summary>
        EndElement = 15,
        /// <summary>
        /// Returned when XmlReader gets to the end of the entity replacement as a result
        /// of a call to System.Xml.XmlReader.ResolveEntity().
        /// </summary>
        EndEntity = 16,
        /// <summary>
        /// The XML declaration.
        /// </summary>
        XmlDeclaration = 17,
    }
}
