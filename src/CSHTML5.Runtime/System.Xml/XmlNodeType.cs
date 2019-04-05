
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
