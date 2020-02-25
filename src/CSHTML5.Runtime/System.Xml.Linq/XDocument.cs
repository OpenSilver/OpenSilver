
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



#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

#if !CSHTML5NETSTANDARD
// already defined in .NET Standard
namespace System.Xml.Linq
{
    /// <summary>
    /// Represents an XML document.
    /// </summary>
    public partial class XDocument : XContainer
    {
        private string _text; //for parsing with XmlReader

        internal object INTERNAL_jsDocument;
        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XDocument class.
        /// </summary>
        public XDocument()
        {
            //this is a new XDocument, we create the js equivalent:
            INTERNAL_jsDocument = CSHTML5.Interop.ExecuteJavaScript(@"document.implementation.createDocument(null, ""root"", null)"); //called "root" for now, we'll need a proper name (like the name of the class that is going to be serialized).
            INTERNAL_jsnode = CSHTML5.Interop.ExecuteJavaScript(@"$0.documentElement", INTERNAL_jsDocument);

#if RECYCLE_XNODES //This is useful for example when comparing two identical XNodes, so that it says that it is the same node. We commented it for performance reasons when running in the Simulator, to reduce the number if Interop calls.
            //add a way to get the c# Document from the js Document:
            if (Interop.IsRunningInTheSimulator)
            {
                string newId = Guid.NewGuid().ToString();
                Interop.ExecuteJavaScriptAsync("$0.nodeId = $1", INTERNAL_jsDocument, newId);
                XDocument.INTERNAL_idsToXNodes.Add(newId, this);
            }
            else
            {
                Interop.ExecuteJavaScriptAsync("$0.associatedXNode = $1", INTERNAL_jsDocument, this);
            }
#endif
        }

        /// <summary>
        /// Gets the root element of the XML Tree for this document.
        /// </summary>
        public XElement Root
        {
            get
            {
                foreach (XNode xNode in this.Nodes())
                {
                    if (xNode is XElement)
                    {
                        return (XElement)xNode;
                    }
                }
                throw new Exception("Root not found");
            }
            internal set
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.documentElement.appendChild($1)", INTERNAL_jsDocument, value.INTERNAL_jsnode);
            }
        }

        /// <summary>
        /// Creates a new System.Xml.Linq.XDocument from a string.
        /// </summary>
        /// <param name="text">A string that contains XML.</param>
        /// <returns>An System.Xml.Linq.XDocument populated from the string that contains XML.</returns>
        public static XDocument Parse(string text)
        {
            XDocument newDocument = new XDocument();
            newDocument._text = text;
            //string textWithoutNewLines = System.Text.RegularExpressions.Regex.Replace(text, "\r\n[ ]*", "");
            object parser = CSHTML5.Interop.ExecuteJavaScript(@"new DOMParser()");
            newDocument.INTERNAL_jsnode = CSHTML5.Interop.ExecuteJavaScript(@"
$0.parseFromString($1, ""application/xml"")
", parser, text);

            return newDocument;
        }

        public override string ToString()
        {
            return Root.ToString();
        }

        internal override string ToString(bool indentXml)
        {
            return Root.ToString(indentXml);
        }


#region should probaly be moved to a helper class

        static XDocument()
        {
            if (Interop.IsRunningInTheSimulator)
            {
                INTERNAL_idsToXNodes = new Dictionary<string, XNode>();
            }
        }

#if !BRIDGE
        [JSIgnore]
#endif
        internal static Dictionary<string, XNode> INTERNAL_idsToXNodes;

        internal static XNode GetXNodeFromJSNode(object node)
        {
            XNode result = null;

            if (Interop.IsRunningInTheSimulator)
            {
                // In the Simulator, we get the CSharp object associated to a DOM element by searching for the DOM element ID in the "INTERNAL_idsToUIElements" dictionary.

#if RECYCLE_XNODES //This is useful for example when comparing two identical XNodes, so that it says that it is the same node. We commented it for performance reasons when running in the Simulator, to reduce the number if Interop calls.
                object jsId = Interop.ExecuteJavaScript("$0.nodeId", node);
                if (!IsNullOrUndefined(jsId))
                {
                    string id = Convert.ToString(jsId);
                    if (XDocument.INTERNAL_idsToXNodes.ContainsKey(id))
                    {
                        result = XDocument.INTERNAL_idsToXNodes[id];
                    } //else throw an exception ?
                }
                else
                {
                    // generate a new id for the node
#endif
                // get the type of the node (probably through node.nodeType, see the comment at the beginning of the XContainer class).
                // create the node
                // add the whole thing to the dictionary
                // result = nodeCreated

                string newId = Guid.NewGuid().ToString();
                int nodeType = Convert.ToInt32(Interop.ExecuteJavaScript("$0.nodeType", node));
                //nodeType = : 1 is XElement, 3 is XText, 8 is XComment, 9 is XDocument but is useless I think


                switch (nodeType)
                {
                    case 1: //XElement
                        result = new XElement(node);
                        break;
                    case 3: //Xtext
                        result = new XText(node);
                        result.INTERNAL_jsnode = node;
                        break;
                    case 8: //XComment
                        break;
                    case 9: //XDocument
                        break;
                    default:
                        break;
                }
#if RECYCLE_XNODES //This is useful for example when comparing two identical XNodes, so that it says that it is the same node. We commented it for performance reasons when running in the Simulator, to reduce the number if Interop calls.
                    XDocument.INTERNAL_idsToXNodes.Add(newId, result);
                    Interop.ExecuteJavaScriptAsync("$0.nodeId = $1", node, newId);
                }
#endif
            }
            else
            {
#if RECYCLE_XNODES //This is useful for example when comparing two identical XNodes, so that it says that it is the same node. We commented it for performance reasons when running in the Simulator, to reduce the number if Interop calls.
                // In JavaScript, we get the CSharp object associated to a DOM element by reading the "associatedUIElement" property:

                object associatedXNode = Interop.ExecuteJavaScript("$0.associatedXNode", node);
                if (!IsNullOrUndefined(associatedXNode))
                {
                    result = (XNode)associatedXNode;
                }
                else
                {
#endif
                // get the type of the node (probably through node.nodeType, see the comment at the beginning of the XContainer class).
                // create the Xnode
                // set the XNode to node.associatedXNode
                //result = nodeCreated

                int nodeType = Convert.ToInt32(Interop.ExecuteJavaScript("$0.nodeType", node));
                //nodeType = : 1 is XElement, 3 is XText, 8 is XComment, 9 is XDocument but is useless I think


                switch (nodeType)
                {
                    case 1: //XElement
                        result = new XElement(node);
                        break;
                    case 3: //Xtext
                        result = new XText(node);
                        result.INTERNAL_jsnode = node;
                        break;
                    case 8: //XComment
                        break;
                    case 9: //XDocument
                        break;
                    default:
                        break;
                }
#if RECYCLE_XNODES //This is useful for example when comparing two identical XNodes, so that it says that it is the same node. We commented it for performance reasons when running in the Simulator, to reduce the number if Interop calls.
                    //todo: shouldn't it be ".associatedXNode" instead of ".nodeId" below?
                    Interop.ExecuteJavaScript("$0.nodeId = $1", node, result);
                }
#endif
            }
            return result;
        }

        public static bool IsNullOrUndefined(object jsObject)
        {
            if (Interop.IsRunningInTheSimulator)
                return Interop.IsUndefined(jsObject) || Interop.IsNull(jsObject);
            else
                return Convert.ToBoolean(Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", jsObject));
        }


#endregion

        /// <summary>
        /// Creates an <see cref="XmlWriter"/> used to add either nodes 
        /// or attributes to the <see cref="XContainer"/>. The later option
        /// applies only for <see cref="XElement"/>.
        /// </summary>
        /// <returns>An <see cref="XmlWriter"/></returns>
        public XmlWriter CreateWriter()
        {
            return new Cshtml5_XmlWriter(this);
        }

        public XmlReader CreateReader()
        {
            return new Cshtml5_XmlReader(this, _text);
        }

#region not implemented

        ///// <summary>
        ///// Initializes a new instance of the System.Xml.Linq.XDocument class with the
        ///// specified content.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects to add to this document.</param>
        //public XDocument(params object[] content);

        ///// <summary>
        ///// Initializes a new instance of the System.Xml.Linq.XDocument class from an
        ///// existing System.Xml.Linq.XDocument object.
        ///// </summary>
        ///// <param name="other">The System.Xml.Linq.XDocument object that will be copied.</param>
        //public XDocument(XDocument other);

        ///// <summary>
        ///// Initializes a new instance of the System.Xml.Linq.XDocument class with the
        ///// specified System.Xml.Linq.XDeclaration and content.
        ///// </summary>
        ///// <param name="declaration">An System.Xml.Linq.XDeclaration for the document.</param>
        ///// <param name="content">The content of the document.</param>
        //public XDocument(XDeclaration declaration, params object[] content);

        ///// <summary>
        ///// Gets or sets the XML declaration for this document.
        ///// </summary>
        //public XDeclaration Declaration { get; set; }

        ///// <summary>
        ///// Gets the Document Type Definition (DTD) for this document.
        ///// </summary>
        //public XDocumentType DocumentType { get; }

        //// Returns:
        ////     The node type. For System.Xml.Linq.XDocument objects, this value is System.Xml.XmlNodeType.Document.
        ///// <summary>
        ///// Gets the node type for this node.
        ///// </summary>
        //public override XmlNodeType NodeType { get; }

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument instance using the specified stream.
        ///// </summary>
        ///// <param name="stream">The stream containing the XML data.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument object used to read the data contained in the
        ///// stream.
        ///// </returns>
        //public static XDocument Load(Stream stream);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from a file located in the application's
        ///// XAP package.
        ///// </summary>
        ///// <param name="uri">
        ///// A URI string that references the file to be loaded into a new System.Xml.Linq.XDocument.
        ///// This file is located in the application's XAP package. If you want to download
        ///// a file from some other location, follow the steps described in How to: Load
        ///// an XML file from an Arbitrary URI Location with LINQ to XML.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the contents of the specified
        ///// file.
        ///// </returns>
        //public static XDocument Load(string uri);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from a System.IO.TextReader.
        ///// </summary>
        ///// <param name="textReader">A System.IO.TextReader that contains the content for the System.Xml.Linq.XDocument.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the contents of the specified
        ///// System.IO.TextReader.
        ///// </returns>
        //public static XDocument Load(TextReader textReader);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from an System.Xml.XmlReader.
        ///// </summary>
        ///// <param name="reader">A System.Xml.XmlReader that contains the content for the System.Xml.Linq.XDocument.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the contents of the specified
        ///// System.Xml.XmlReader.
        ///// </returns>
        //public static XDocument Load(XmlReader reader);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument instance using the specified stream,
        ///// optionally preserving white space, setting the base URI, and retaining line
        ///// information.
        ///// </summary>
        ///// <param name="stream">The stream containing the XML data.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies whether to load base URI and
        ///// line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument object used to read the data contained in the
        ///// stream.
        ///// </returns>
        //public static XDocument Load(Stream stream, LoadOptions options);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from a file located in the application's
        ///// XAP package, optionally preserving white space, setting the base URI, and
        ///// retaining line information.
        ///// </summary>
        ///// <param name="uri">
        ///// A URI string that references the file to be loaded into a new System.Xml.Linq.XDocument.
        ///// This file is located in the application's XAP package. If you want to download
        ///// a file from some other location, follow the steps described in How to: Load
        ///// an XML file from an Arbitrary URI Location with LINQ to XML.
        ///// </param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies how white space is handled and
        ///// whether to load base URI and line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the contents of the specified
        ///// file.
        ///// </returns>
        //public static XDocument Load(string uri, LoadOptions options);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from a System.IO.TextReader, optionally
        ///// preserving white space, setting the base URI, and retaining line information.
        ///// </summary>
        ///// <param name="textReader">A System.IO.TextReader that contains the content for the System.Xml.Linq.XDocument.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the XML that was read from the
        ///// specified System.IO.TextReader.
        ///// </returns>
        //public static XDocument Load(TextReader textReader, LoadOptions options);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from an System.Xml.XmlReader, optionally
        ///// setting the base URI, and retaining line information.
        ///// </summary>
        ///// <param name="reader">A System.Xml.XmlReader that will be read for the content of the System.Xml.Linq.XDocument.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies whether to load base URI and
        ///// line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XDocument that contains the XML that was read from the
        ///// specified System.Xml.XmlReader.
        ///// </returns>
        //public static XDocument Load(XmlReader reader, LoadOptions options);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XDocument from a string, optionally preserving
        ///// white space, setting the base URI, and retaining line information.
        ///// </summary>
        ///// <param name="text">A string that contains XML.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>An System.Xml.Linq.XDocument populated from the string that contains XML.</returns>
        //public static XDocument Parse(string text, LoadOptions options);

        ///// <summary>
        ///// Outputs this System.Xml.Linq.XDocument to the specified System.IO.Stream.
        ///// </summary>
        ///// <param name="stream">The stream to output this System.Xml.Linq.XDocument to.</param>
        //public void Save(Stream stream);

        ///// <summary>
        ///// Serialize this System.Xml.Linq.XDocument to a System.IO.TextWriter.
        ///// </summary>
        ///// <param name="textWriter">
        ///// A System.IO.TextWriter that the System.Xml.Linq.XDocument will be written
        ///// to.
        ///// </param>
        //public void Save(TextWriter textWriter);

        ///// <summary>
        ///// Serialize this System.Xml.Linq.XDocument to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer">
        ///// A System.Xml.XmlWriter that the System.Xml.Linq.XDocument will be written
        ///// to.
        ///// </param>
        //public void Save(XmlWriter writer);

        ///// <summary>
        ///// Outputs this System.Xml.Linq.XDocument to the specified System.IO.Stream,
        ///// optionally specifying formatting behavior.
        ///// </summary>
        ///// <param name="stream">The stream to output this System.Xml.Linq.XDocument to.</param>
        ///// <param name="options">A System.Xml.Linq.SaveOptions that specifies formatting behavior.</param>
        //public void Save(Stream stream, SaveOptions options);

        ///// <summary>
        ///// Serialize this System.Xml.Linq.XDocument to a System.IO.TextWriter, optionally
        ///// disabling formatting.
        ///// </summary>
        ///// <param name="textWriter">The System.IO.TextWriter to output the XML to.</param>
        ///// <param name="options">A System.Xml.Linq.SaveOptions that specifies formatting behavior.</param>
        //public void Save(TextWriter textWriter, SaveOptions options);

        ///// <summary>
        ///// Write this document to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer">An System.Xml.XmlWriter into which this method will write.</param>
        //public override void WriteTo(XmlWriter writer);
#endregion
    }
}
#endif