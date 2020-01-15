
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
    /// Represents a node that can contain other nodes.
    /// </summary>
    public abstract class XContainer : XNode
    {
        //todo: handle the comment nodes type XComment in c#
        //todo: change the way we determine the type of the nodes (cf. https://developer.mozilla.org/en-US/docs/Web/API/Node/nodeType)
        //      basically, nodeType = : 1 is XElement, 3 is XText, 8 is XComment, 9 is XDocument but is useless I think

        /// <summary>
        /// Returns a collection of the child nodes of this element or document, in document
        /// order.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable&lt;T&gt; of System.Xml.Linq.XNode containing
        /// the contents of this System.Xml.Linq.XContainer, in document order.
        /// </returns>
        public IEnumerable<XNode> Nodes()
        {
            int i = 0;
            while (true)
            {
                object nodeAtI = CSHTML5.Interop.ExecuteJavaScript("$0.childNodes[$1]", INTERNAL_jsnode, i);
                if (CSHTML5.Interop.IsUndefined(nodeAtI))
                {
                    break;
                }

                yield return XDocument.GetXNodeFromJSNode(nodeAtI);

                ++i;
            }
            yield break;
        }

        /// <summary>
        /// Adds the specified content as children of this System.Xml.Linq.XContainer.
        /// </summary>
        /// <param name="content">
        /// A content object containing simple content or a collection of content objects
        /// to be added.
        /// </param>
        public void Add(object content)
        {
            if (content is XNode)
            {
                XNode contentAsXNode = ((XNode)content);
                if (contentAsXNode.HasParent())
                {
                    contentAsXNode = contentAsXNode.Clone();
                }
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.appendChild($1)", INTERNAL_jsnode, contentAsXNode.INTERNAL_jsnode);
            }
            else if (content is XAttribute)
            {
                XAttribute contentAsXAttribute = (XAttribute)content;
                if (contentAsXAttribute.INTERNAL_containerJSNode == null)
                {
                    contentAsXAttribute.INTERNAL_containerJSNode = INTERNAL_jsnode;
                }
                else
                {
                    XAttribute attributeCopy = new XAttribute(contentAsXAttribute);
                    attributeCopy.INTERNAL_containerJSNode = INTERNAL_jsnode;
                }


                if (string.IsNullOrWhiteSpace(contentAsXAttribute.Name.NamespaceName))
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttribute($1, $2)", INTERNAL_jsnode, contentAsXAttribute._nameWithPrefix.LocalName, contentAsXAttribute.Value);
                }
                else
                {
                    XName attributeName = contentAsXAttribute._nameWithPrefix;
                    string attributeLocalName = attributeName.LocalName;
                    if (attributeName.NamespaceName == DataContractSerializer_Helpers.XMLNS_NAMESPACE && !attributeName.LocalName.StartsWith("xmlns:"))
                    {
                        attributeLocalName = "xmlns:" + attributeLocalName; //this is required to have DataContractSerializer_Helpers.XMLNS_NAMESPACE as the namespace. 
                    }
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttributeNS($1, $2, $3)", INTERNAL_jsnode, attributeName.NamespaceName, attributeLocalName, contentAsXAttribute.Value);
                }
            }
        }

        /// <summary>
        /// Gets the first (in document order) child element with the specified System.Xml.Linq.XName.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns>
        /// A System.Xml.Linq.XElement that matches the specified System.Xml.Linq.XName,
        /// or null.
        /// </returns>
        public XElement Element(XName name)
        {
            //note: we couldn't use INTERNAL_jsnode.getElementsByTagName(name) because it returns the children of the children nodes as well (we only want the immediate children).

            //Note about the js below: Array.from is to get the possibility to call the filter method:

            string nameAsString = name.LocalName;
            // object jsNodeForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(node => node.tagName == $1)[0]", INTERNAL_jsnode, name.ToString());
            //todo: check whether the line above is more efficient than the line below and change accordingly (if IE allows it, see todo in Elements(XName))
            object jsNodeForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(function(node) { return node.tagName == $1})[0]", INTERNAL_jsnode, nameAsString);//}

            if (XDocument.IsNullOrUndefined(jsNodeForElement)) //not found, we return null.
            {
                return null;
            }

            XNode node = XDocument.GetXNodeFromJSNode(jsNodeForElement);
            if (node is XElement)
            {
                return (XElement)node;
            }
            return null;
        }

        /// <summary>
        /// Returns a collection of the child elements of this element or document, in
        /// document order.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable&lt;XElement&gt; of System.Xml.Linq.XElement
        /// containing the child elements of this System.Xml.Linq.XContainer, in document
        /// order.
        /// </returns>
        public IEnumerable<XElement> Elements()
        {
            //Note on the JS below: the filter is to get rid of the XText elements which have no tagName.

            //object jsNodesForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(node => node.tagName !== undefined)", INTERNAL_jsnode);
            //todo: check whether the line above is more efficient than the line below and change accordingly (if IE allows it, see todo in Elements(XName))
            object jsNodesForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(function(node) { return node.tagName !== undefined})", INTERNAL_jsnode);

            if (XDocument.IsNullOrUndefined(jsNodesForElement))
            {
                yield break; // no children.
            }

            int i = 0;

            while (true)
            {
                object nodeAtI = CSHTML5.Interop.ExecuteJavaScript("$0[$1]", jsNodesForElement, i);
                if (CSHTML5.Interop.IsUndefined(nodeAtI))
                {
                    break;
                }

                //note: they have a tagName so they are definitely XElements.
                yield return (XElement)XDocument.GetXNodeFromJSNode(nodeAtI);

                ++i;
            }
            yield break;
        }

        /// <summary>
        /// Returns a filtered collection of the child elements of this element or document,
        /// in document order. Only elements that have a matching System.Xml.Linq.XName
        /// are included in the collection.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable`1 of System.Xml.Linq.XElement
        /// containing the children of the System.Xml.Linq.XContainer that have a matching
        /// System.Xml.Linq.XName, in document order.
        /// </returns>
        public IEnumerable<XElement> Elements(XName name)
        {
            //note: we couldn't use INTERNAL_jsnode.getElementsByTagName(name) because it returns the children of the children nodes as well (we only want the immediate children).

            //jsNodesForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(node => node.tagName == $1)", INTERNAL_jsnode, name.ToString());
            //todo: if IE learns that => isn't a syntax error (which breaks the whole file) and can use it properly, use the line above instead of the following because it seems to be more efficient.
            object jsNodesForElement = CSHTML5.Interop.ExecuteJavaScript("Array.from($0.childNodes).filter(document.functionToCompareWordForFilter($1))", INTERNAL_jsnode, name.ToString());
#region explanation of the line above
            //note: normal use of filter: myArray.filter( function(node) { return testOnNode; })
            //problem here: testOnNode depends on the name than node and we cannot put $1 in there because JSIL changes it to "this.$name", but "this" does not exist where "testOnNode" is.
            //solution: document.functionToCompareWordForFilter(name) returns A FUNCTION that takes a node and compares its tagName to name (see in cshtml5.js)
            //          when calling document.functionToCompareWordForFilter, this.$name exists
            //          and the created method that is used by filter can use it normally.
            //in short: the function created by document.functionToCompareWordForFilter provides the name, filter provides the node.
#endregion

            if (XDocument.IsNullOrUndefined(jsNodesForElement)) //nothing fits the request.
            {
                yield break;
            }

            int i = 0;

            while (true)
            {
                object nodeAtI = CSHTML5.Interop.ExecuteJavaScript("$0[$1]", jsNodesForElement, i);
                if (CSHTML5.Interop.IsUndefined(nodeAtI))
                {
                    break;
                }

                //note: they have a tagName so they are definitely XElements.
                yield return (XElement)XDocument.GetXNodeFromJSNode(nodeAtI);

                ++i;
            }
            yield break;
        }

        /// <summary>
        /// Get the first child node of this node.
        /// </summary>
        public XNode FirstNode
        {
            get
            {
                //return the first node.
                object jsNodeForElement = CSHTML5.Interop.ExecuteJavaScript("$0.firstChild", INTERNAL_jsnode);
                if(XDocument.IsNullOrUndefined(jsNodeForElement)) //no child, we return null.
                {
                    return null;
                }
                return XDocument.GetXNodeFromJSNode(jsNodeForElement);
            }
        }


        /// <summary>
        /// Returns a collection of the descendant elements for this document or element,
        /// in document order.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable`1 of System.Xml.Linq.XElement
        /// containing the descendant elements of the System.Xml.Linq.XContainer.
        /// </returns>
        public IEnumerable<XElement> Descendants()
        {
            foreach (XNode node in Nodes()) //I guess we'll keep it that way for now, I couldn't find a way to get the descendants directly in JS.
            {
                if (node is XElement)
                {
                    XElement nodeAsXElement = (XElement)node;
                    yield return nodeAsXElement;
                    foreach (XElement descendantElement in nodeAsXElement.Descendants())
                    {
                        yield return descendantElement;
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Returns a filtered collection of the descendant elements for this document
        /// or element, in document order. Only elements that have a matching System.Xml.Linq.XName
        /// are included in the collection.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable`1 of System.Xml.Linq.XElement
        /// containing the descendant elements of the System.Xml.Linq.XContainer that
        /// match the specified System.Xml.Linq.XName.
        /// </returns>
        public IEnumerable<XElement> Descendants(XName name)
        {
            //this one is much simpler than Elements since we can call the getElementsByTagName method that does exactly what we want.
            object jsNodesForElement = CSHTML5.Interop.ExecuteJavaScript("$0.getElementsByTagName($1)", INTERNAL_jsnode, name.ToString());

            if (XDocument.IsNullOrUndefined(jsNodesForElement)) //no children.
            {
                yield break;
            }

            int i = 0;

            while (true)
            {
                object nodeAtI = CSHTML5.Interop.ExecuteJavaScript("$0[$1]", jsNodesForElement, i);
                if (CSHTML5.Interop.IsUndefined(nodeAtI))
                {
                    break;
                }

                //note: they have a tagName so they are definitely XElements.
                yield return (XElement)XDocument.GetXNodeFromJSNode(nodeAtI);

                ++i;
            }
            yield break;
        }

#region not implemented

        ///// <summary>
        ///// Get the last child node of this node.
        ///// </summary>
        //public XNode LastNode { get; }

        ///// <summary>
        ///// Adds the specified content as children of this System.Xml.Linq.XContainer.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void Add(params object[] content);

        ///// <summary>
        ///// Adds the specified content as the first children of this document or element.
        ///// </summary>
        ///// <param name="content">
        ///// A content object containing simple content or a collection of content objects
        ///// to be added.
        ///// </param>
        //public void AddFirst(object content);

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent is null.
        ///// <summary>
        ///// Adds the specified content as the first children of this document or element.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void AddFirst(params object[] content);

        ///// <summary>
        ///// Creates an System.Xml.XmlWriter that can be used to add nodes to the System.Xml.Linq.XContainer.
        ///// </summary>
        ///// <returns>An System.Xml.XmlWriter that is ready to have content written to it.</returns>
        //public XmlWriter CreateWriter();

        ///// <summary>
        ///// Returns a collection of the descendant nodes for this document or element,
        ///// in document order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XNode containing
        ///// the descendant nodes of the System.Xml.Linq.XContainer, in document order.
        ///// </returns>
        //public IEnumerable<XNode> DescendantNodes();

        ///// <summary>
        ///// Removes the child nodes from this document or element.
        ///// </summary>
        //public void RemoveNodes();

        ///// <summary>
        ///// Replaces the children nodes of this document or element with the specified
        ///// content.
        ///// </summary>
        ///// <param name="content">
        ///// A content object containing simple content or a collection of content objects
        ///// that replace the children nodes.
        ///// </param>
        //public void ReplaceNodes(object content);

        ///// <summary>
        ///// Replaces the children nodes of this document or element with the specified
        ///// content.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void ReplaceNodes(params object[] content);

#endregion

    }
}
#endif