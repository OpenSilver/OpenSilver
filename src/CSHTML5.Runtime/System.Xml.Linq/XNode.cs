
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



using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

#if !CSHTML5NETSTANDARD
// already defined in .NET Standard
namespace System.Xml.Linq
{
    /// <summary>
    /// Represents the abstract concept of a node (element, comment, document type,
    /// processing instruction, or text node) in the XML tree.
    /// </summary>
    public abstract class XNode : XObject
    {
        internal object INTERNAL_jsnode;

        /// <summary>
        /// Returns the indented XML for this node.
        /// </summary>
        /// <returns>A System.String containing the indented XML.</returns>
        public override string ToString()
        {
            return ToString(true);
        }

        internal virtual string ToString(bool indentXml)
        {
            string nonIndentedXML = null;
            if (INTERNAL_HtmlDomManager.IsInternetExplorer() || Interop.IsRunningInTheSimulator)
            {
                nonIndentedXML = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("(new XMLSerializer()).serializeToString($0).replace(/Prefix\\d+:AttributeToRemoveFromXMLSerialization=\\$1/g, \"\")", INTERNAL_jsnode, DataContractSerializer_Helpers.ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_VALUE)); //todo: find a way to use DataContractSerializer_Helpers.ATTRIBUTE_TO_REMOVE_FROM_XMLSERIALIZATION_NAME while not having the quotation marks.
            }
            else
            {
                if (Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0.outerHTML != undefined", INTERNAL_jsnode)))
                {
                    nonIndentedXML = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.outerHTML", INTERNAL_jsnode));
                }
                else //we are at the root and we need to start from its first child:
                {
                    nonIndentedXML = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.firstchild.outerHTML", INTERNAL_jsnode));
                }
            }

            if (indentXml)
            {
#if OLD_ALGORITHM_TO_FORMAT_XML
                //now we need to put new lines and indentations to the xml:
                //locate beginning of tags (the '<' character)
                //  add a new line after them
                //  increase/decrease indentation depending on the next tag (decrease if </XXX>, increase otherwise)
                //should be good.

                string indentedXML = string.Empty;
                string[] splittedXML = nonIndentedXML.Split('<');
                string currentIndentation = "";
                int length = splittedXML.Length;
                bool isFirst = true;
                bool thisLoopImpactsIndentation = true; //required to avoid changing the indentation on lines that contain the start and end of a tag (those that contain text)
                for (int i = 1; i < length; ++i) //starts at one because the first element in splittedXml is empty.
                {
                    string part = splittedXML[i].Trim(); //we Trim just in case but I don't think it is useful
                    string formattedPart = "<" + part; //"<" removed by the split
                    if (!part.StartsWith("/"))
                    {
                        //now we check if there is a content as text to this tag:
                        int index = formattedPart.IndexOf('>');
                        if (index < formattedPart.Length - 1)
                        {
                            //the element contains a text so we add the closing tag:
                            ++i; //advance the loop to the index of the closing tag

                            string closingTag = splittedXML[i].Trim();
                            formattedPart += "<" + closingTag;

                            thisLoopImpactsIndentation = false;

                        }
                    }

                    if (!isFirst)
                    {
                        indentedXML += Environment.NewLine;
                    }
                    if (part.StartsWith("/"))
                    {
                        currentIndentation = currentIndentation.Substring(2); //we remove two spaces
                        thisLoopImpactsIndentation = false;
                    }

                    indentedXML += currentIndentation;
                    indentedXML += formattedPart;
                    if (thisLoopImpactsIndentation)
                    {
                        currentIndentation += "  ";
                    }

                    thisLoopImpactsIndentation = true;
                    isFirst = false;
                }
#else

                string indentedXML = Interop.ExecuteJavaScript("window.vkbeautify.xml($0, 2)", nonIndentedXML).ToString();
#endif
                return indentedXML;
            }
            else
            {
                return nonIndentedXML;
            }
        }

#region From XObject
        /// <summary>
        /// Gets the System.Xml.Linq.XDocument for this System.Xml.Linq.XObject.
        /// </summary>
        public XDocument Document
        {
            get
            {
                object documentJSNode = Interop.ExecuteJavaScript("$0.ownerDocument", INTERNAL_jsnode);
                if(documentJSNode != null)
                {
                    return (XDocument)XDocument.GetXNodeFromJSNode(documentJSNode);
                }
                return null;
            }
        }

        internal virtual void CascadeDocumentToChildren() { }
#endregion

#region not implemented
        ///// <summary>
        ///// Gets a comparer that can compare the relative position of two nodes.
        ///// </summary>
        //public static XNodeDocumentOrderComparer DocumentOrderComparer { get; }

        ///// <summary>
        ///// Gets a comparer that can compare two nodes for value equality.
        ///// </summary>
        //public static XNodeEqualityComparer EqualityComparer { get; }

        ///// <summary>
        ///// Gets the next sibling node of this node.
        ///// </summary>
        //public XNode NextNode { get; }

        ///// <summary>
        ///// Gets the previous sibling node of this node.
        ///// </summary>
        //public XNode PreviousNode { get; }

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Adds the specified content immediately after this node.
        ///// </summary>
        ///// <param name="content">
        ///// A content object that contains simple content or a collection of content
        ///// objects to be added after this node.
        ///// </param>
        //public void AddAfterSelf(object content);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Adds the specified content immediately after this node.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void AddAfterSelf(params object[] content);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Adds the specified content immediately before this node.
        ///// </summary>
        ///// <param name="content">
        ///// A content object that contains simple content or a collection of content
        ///// objects to be added before this node.
        ///// </param>
        //public void AddBeforeSelf(object content);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Adds the specified content immediately before this node.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void AddBeforeSelf(params object[] content);

        ///// <summary>
        ///// Returns a collection of the ancestor elements of this node.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the ancestor elements of this node.
        ///// </returns>
        //public IEnumerable<XElement> Ancestors();

        ///// <summary>
        ///// Returns a filtered collection of the ancestor elements of this node. Only
        ///// elements that have a matching System.Xml.Linq.XName are included in the collection.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName to match.</param>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the ancestor elements of this node. Only elements that have a matching
        ///// System.Xml.Linq.XName are included in the collection.The nodes in the returned
        ///// collection are in reverse document order.This method uses deferred execution.
        ///// </returns>
        //public IEnumerable<XElement> Ancestors(XName name);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The two nodes do not share a common ancestor.
        ///// <summary>
        ///// Compares two nodes to determine their relative XML document order.
        ///// </summary>
        ///// <param name="n1">First System.Xml.Linq.XNode to compare.</param>
        ///// <param name="n2">Second System.Xml.Linq.XNode to compare.</param>
        ///// <returns>
        ///// An int containing 0 if the nodes are equal; -1 if n1 is before n2; 1 if n1
        ///// is after n2.
        ///// </returns>
        //public static int CompareDocumentOrder(XNode n1, XNode n2);

        ///// <summary>
        ///// Creates an System.Xml.XmlReader for this node.
        ///// </summary>
        ///// <returns>
        ///// An System.Xml.XmlReader that can be used to read this node and its descendants.
        ///// </returns>
        //public XmlReader CreateReader();

        ///// <summary>
        ///// Creates an System.Xml.XmlReader with the options specified by the readerOptions
        ///// parameter.
        ///// </summary>
        ///// <param name="readerOptions">
        ///// A System.Xml.Linq.ReaderOptions object that specifies whether to omit duplicate
        ///// namespaces.
        ///// </param>
        ///// <returns>An System.Xml.XmlReader object.</returns>
        //public XmlReader CreateReader(ReaderOptions readerOptions);

        ///// <summary>
        ///// Compares the values of two nodes, including the values of all descendant
        ///// nodes.
        ///// </summary>
        ///// <param name="n1">The first System.Xml.Linq.XNode to compare.</param>
        ///// <param name="n2">The second System.Xml.Linq.XNode to compare.</param>
        ///// <returns>true if the nodes are equal; otherwise false.</returns>
        //public static bool DeepEquals(XNode n1, XNode n2);

        ///// <summary>
        ///// Returns a collection of the sibling elements after this node, in document
        ///// order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the sibling elements after this node, in document order.
        ///// </returns>
        //public IEnumerable<XElement> ElementsAfterSelf();

        ///// <summary>
        ///// Returns a filtered collection of the sibling elements after this node, in
        ///// document order. Only elements that have a matching System.Xml.Linq.XName
        ///// are included in the collection.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName to match.</param>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the sibling elements after this node, in document order. Only elements
        ///// that have a matching System.Xml.Linq.XName are included in the collection.
        ///// </returns>
        //public IEnumerable<XElement> ElementsAfterSelf(XName name);

        ///// <summary>
        ///// Returns a collection of the sibling elements before this node, in document
        ///// order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the sibling elements before this node, in document order.
        ///// </returns>
        //public IEnumerable<XElement> ElementsBeforeSelf();

        ///// <summary>
        ///// Returns a filtered collection of the sibling elements before this node, in
        ///// document order. Only elements that have a matching System.Xml.Linq.XName
        ///// are included in the collection.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName to match.</param>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of the sibling elements before this node, in document order. Only elements
        ///// that have a matching System.Xml.Linq.XName are included in the collection.
        ///// </returns>
        //public IEnumerable<XElement> ElementsBeforeSelf(XName name);

        ///// <summary>
        ///// Determines if the current node appears after a specified node in terms of
        ///// document order.
        ///// </summary>
        ///// <param name="node">The System.Xml.Linq.XNode to compare for document order.</param>
        ///// <returns>true if this node appears after the specified node; otherwise false.</returns>
        //public bool IsAfter(XNode node);

        ///// <summary>
        ///// Determines if the current node appears before a specified node in terms of
        ///// document order.
        ///// </summary>
        ///// <param name="node">The System.Xml.Linq.XNode to compare for document order.</param>
        ///// <returns>true if this node appears before the specified node; otherwise false.</returns>
        //public bool IsBefore(XNode node);

        ///// <summary>
        ///// Returns a collection of the sibling nodes after this node, in document order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XNode of
        ///// the sibling nodes after this node, in document order.
        ///// </returns>
        //public IEnumerable<XNode> NodesAfterSelf();

        ///// <summary>
        ///// Returns a collection of the sibling nodes before this node, in document order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XNode of
        ///// the sibling nodes before this node, in document order.
        ///// </returns>
        //public IEnumerable<XNode> NodesBeforeSelf();

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Xml.XmlReader is not positioned on a recognized node type.
        ////
        ////   System.Xml.XmlException:
        ////     The underlying System.Xml.XmlReader throws an exception.
        ///// <summary>
        ///// Creates an System.Xml.Linq.XNode from an System.Xml.XmlReader.
        ///// </summary>
        ///// <param name="reader">An System.Xml.XmlReader positioned at the node to read into this System.Xml.Linq.XNode.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XNode that contains the node and its descendant nodes
        ///// that were read from the reader. The runtime type of the node is determined
        ///// by the node type (System.Xml.Linq.XObject.NodeType) of the first node encountered
        ///// in the reader.
        ///// </returns>
        //public static XNode ReadFrom(XmlReader reader);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Removes this node from its parent.
        ///// </summary>
        //public void Remove();

        ///// <summary>
        ///// Replaces this node with the specified content.
        ///// </summary>
        ///// <param name="content">Content that replaces this node.</param>
        //public void ReplaceWith(object content);

        ///// <summary>
        ///// Replaces this node with the specified content.
        ///// </summary>
        ///// <param name="content">A parameter list of the new content.</param>
        //public void ReplaceWith(params object[] content);


        ///// <summary>
        ///// Returns the XML for this node, optionally disabling formatting.
        ///// </summary>
        ///// <param name="options">A System.Xml.Linq.SaveOptions that specifies formatting behavior.</param>
        ///// <returns>A System.String containing the XML.</returns>
        //public string ToString(SaveOptions options);

        ///// <summary>
        ///// Writes this node to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer">An System.Xml.XmlWriter into which this method will write.</param>
        //public abstract void WriteTo(XmlWriter writer);

#endregion

        internal virtual XNode Clone()
        {
            throw new NotImplementedException();
        }

        internal bool HasParent()
        {
            return Convert.ToBoolean(Interop.ExecuteJavaScript("$0.parentNode != undefined && $0.parentNode != null", INTERNAL_jsnode));
        }
    }
}
#endif