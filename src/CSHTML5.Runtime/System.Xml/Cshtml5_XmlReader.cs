
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
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text.RegularExpressions;

namespace System.Xml
{

    // this class s here to help interating with the child nodes, since XAttribute and XText are not XNode
    internal class INTERNAL_NodeType
    {
        IEnumerator _nodeEnumerator = null;
        IEnumerator _attributsEnumerator = null;
        XmlNodeType _type;
        object _node;

        public INTERNAL_NodeType(object node, XmlNodeType type)
        {
            _node = node;
            _type = type;

            if (_node is XContainer)
            {
                IEnumerable<XNode> nodeEnumerable = ((XContainer)_node).Nodes();

                if (nodeEnumerable.Any())
                {
                    _nodeEnumerator = nodeEnumerable.GetEnumerator();
                    MoreChild = _nodeEnumerator.MoveNext();
                }

                else
                    MoreChild = false;
            }
            else
                MoreChild = false;


            if (_node is XElement)
            {
                _attributsEnumerator = ((XElement)_node).Attributes().GetEnumerator();

                _attributsEnumerator.MoveNext();
            }
            else
                _attributsEnumerator = new List<XNode>().GetEnumerator();

        }

        internal bool MoreChild { get; set; }

        public XmlNodeType Type
        {
            get { return _type; }
        }

        public object RawNode
        {
            get { return _node; }
        }

        public XElement ElementOrNull
        {
            get
            {
                return _type == XmlNodeType.Element ? (XElement)_node : null;
            }
        }

        public IEnumerator AttributEnumerator
        {
            get { return _attributsEnumerator; }
        }

        public IEnumerator NodeEnumerator
        {
            get { return _nodeEnumerator; }
        }

        public static INTERNAL_NodeType Default = new INTERNAL_NodeType(null, XmlNodeType.None);
    }

    // allows to instanciate XmlReader without a complete implementation
    // the original one works directly on text, here we use an Xml document already parsed.
    // the constructor takes the original text but it is not used for now.
    internal class Cshtml5_XmlReader : XmlReader
    {
        private XDocument _xmlDoc;

        private XmlReader _reader;

        private List<INTERNAL_NodeType> _nodeStack;

        private string _text; // no more used because IsEmptyElement seem to act correctly

        public Cshtml5_XmlReader(XDocument xmlDoc, string text)
        {
            _text = text;
            _xmlDoc = xmlDoc;
            _reader = this;

            _nodeStack = new List<INTERNAL_NodeType>();

            AddNewNode(new INTERNAL_NodeType(xmlDoc, XmlNodeType.Document));
        }

        public override string ToString()
        {
            return _xmlDoc.ToString();
        }

        private INTERNAL_NodeType LastNode
        {
            get { return _nodeStack.Count > 0 ? _nodeStack[_nodeStack.Count - 1] : INTERNAL_NodeType.Default; }
        }

        private IEnumerator LastChildList
        {
            get { return _nodeStack.Count > 0 ? LastNode.NodeEnumerator : null; }
        }

        private void RemoveLastNode()
        {
            _nodeStack.RemoveAt(_nodeStack.Count - 1);
        }

        private void AddNewNode(INTERNAL_NodeType node)
        {
            _nodeStack.Add(node);
        }

        public override string GetAttribute(string name)
        {
            XElement element = LastNode.ElementOrNull;

            if (element != null)
            {
                if (element.Attributes(name).Count<XAttribute>() > 0)
                {
                    XAttribute attribute = element.Attributes(name).ElementAt<XAttribute>(0);

                    if (attribute != null)
                    {
                        if (attribute.Value != string.Empty)
                            return attribute.Value;
                    }
                }
            }
            return null;
        }

        // the inheritance tree is different from the original XmlReader, the result can be different too. (Attribute and text especially)
        public override XmlNodeType NodeType
        {
            get
            {
                return LastNode.Type;
            }
        }

        private bool IsWhitespaceNode(XNode xNode)
        {
            if (xNode is XText)
            {
                string value = ((XText)xNode).Value;

                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value == null || value == string.Empty)
                {
                    return true;
                }
            }

            return false;
        }

        // This method seems to work like the original one, but I don't know why
        public override bool IsEmptyElement
        {
            get
            {
                return LastNode.NodeEnumerator == null ? true : false;
            }
        }

        // read the document until it finds something that is not whitespace
        private void ReadUntilNotNull()
        {
            do
            {
                if (!Read())
                    return;

            } while (LastNode.Type == XmlNodeType.Whitespace);
        }

        // go to the next element
        public override bool Read()
        {
            ReadNext();

            if (LastNode.Type == XmlNodeType.None) // we've reach the end of the document
                return false;

            return true;
        }

        private bool MoveToNextNode()
        {
            if (_nodeStack.Count > 0)
            {

                LastNode.MoreChild = LastNode.NodeEnumerator.MoveNext();
                return true;
            }
            return false;
        }

        private string BuildName(XName Name)
        {
            if (!string.IsNullOrWhiteSpace(Name.NamespaceName))
                return Name.NamespaceName + ":" + Name.LocalName;

            return Name.LocalName;
        }

        //This method reads the start tag, the contents of the element, and moves the reader past the end element tag.
        public override string ReadElementContentAsString(string localName, string namespaceURI)
        {
            string fullName = localName;

            if (namespaceURI != string.Empty)
                fullName = namespaceURI + ":" + localName;

            if (fullName != Name)
                throw new InvalidOperationException("XmlException : " + Name + " is different of " + fullName);

            string value = string.Empty;

            // if we try to read an empty element, we should return the empty string
            if (!IsEmptyElement)
            {
                // we are on the element, so we need to move to content
                ReadUntilNotNull();

                if (LastNode.Type == XmlNodeType.Text)
                    value = ((XText)LastNode.RawNode).Value;

                else
                    throw new InvalidOperationException("XmlException : the current element content can not be read !");

                // the content is supposed to be read, so we move to the next node that should be the end element
                ReadUntilNotNull();

                // the end element should be read too
                if (LastNode.Type == XmlNodeType.EndElement)
                    ReadUntilNotNull();
            }
            else
            {
                // if the element was empty, this line reads the start element. An empty element has no endElement
                // so this should move to the next node
                ReadUntilNotNull();
            }

            return value;
        }

        public override string Name
        {
            get
            {
                object node = LastNode.RawNode;

                if (node is XElement)
                    return BuildName(((XElement)node).Name);

                else if (node is XAttribute)
                    return BuildName(((XAttribute)node).Name);

                // XDocument, XText
                return string.Empty;
            }
        }

        public override string LocalName
        {
            get
            {
                object node = LastNode.RawNode;

                if (node is XElement)
                    return ((XElement)node).Name.LocalName;

                else if (node is XAttribute)
                    return ((XAttribute)node).Name.LocalName;

                // XDocument, XText
                return string.Empty;
            }

        }

        public override void ReadStartElement(string name)
        {
            // white space nodes are not supposed to be a limitation
            if (LastNode.Type == XmlNodeType.Whitespace)
                ReadUntilNotNull();

            if (Name != name)
                throw new InvalidOperationException("XmlException : " + Name + " is different of " + name);

            ReadStartElement();
        }

        // returns the current node in the same order as with the original XmlReader (also means that attributes are not included)
        private INTERNAL_NodeType GetNode(IEnumerator enumerator)
        {
            if (enumerator == null) // no child
                return INTERNAL_NodeType.Default;

            if (!LastNode.MoreChild) // no more child
                return INTERNAL_NodeType.Default;

            object node = enumerator.Current;

            if (node == null)
            {
                // since comments are not translated to XComment, when we read a comment, current is set to null
                // that normally represent the end of the list. Here we use LastNode.MoreChild to be sure it's really the end
                // we don't consider this case, so we return the next one if any
                if (MoveToNextNode())
                    return GetNode(enumerator);

                return INTERNAL_NodeType.Default;
            }

            if (node is XNode)
            {
                XNode xNode = (XNode)node;

                if (xNode is XElement)
                    return new INTERNAL_NodeType(xNode, XmlNodeType.Element);

                else if (IsWhitespaceNode(xNode))
                    return new INTERNAL_NodeType(xNode, XmlNodeType.Whitespace);

                else if (xNode is XText)
                    return new INTERNAL_NodeType(xNode, XmlNodeType.Text);

                return INTERNAL_NodeType.Default;
            }

            throw new Exception(node.ToString() + " is not an XNode");
        }

        // we try to read the next sibling if it exists, otherwise we return to the previous node
        private bool ToNextSiblingOrPreviousNode()
        {
            RemoveLastNode();
            if (MoveToNextNode()) // condition equivalent to stack count > 0
            {
                INTERNAL_NodeType node = GetNode(LastChildList);

                if (node.Type != XmlNodeType.None) // find a sibling
                {
                    AddNewNode(node);
                    return true;
                }
                else if (LastNode.Type == XmlNodeType.Element)// find an end element 
                {
                    AddNewNode(new INTERNAL_NodeType(LastNode, XmlNodeType.EndElement));
                    return true;
                }
                // otherwise, it means the end of another container than element, typically the end of XDocument
            }

            return false; // end of the document
        }

        // reads the next thing in the document, xText and endElement are also included, but not attributes
        private void ReadNext()
        {
            if (LastNode.Type == XmlNodeType.Attribute) // attributs are not supposed to be read by Read()
            {
                RemoveLastNode();
            }

            if (LastNode.Type == XmlNodeType.EndElement) // we remove the endElement, to avoid reread it, then we find the next element
            {
                RemoveLastNode();
                ToNextSiblingOrPreviousNode();
            }
            else
            {
                INTERNAL_NodeType node = GetNode(LastChildList);

                if (node.Type == XmlNodeType.None)// no more child, we return to the previous node
                {
                    if (_nodeStack.Count > 0)
                    {
                        if (LastNode.Type == XmlNodeType.Element && !IsEmptyElement) // we now read the endElement of the LastNode element
                        {
                            AddNewNode(new INTERNAL_NodeType(LastNode, XmlNodeType.EndElement));
                        }
                        else
                        {
                            ToNextSiblingOrPreviousNode();
                        }
                    }
                    else
                        return; // end of document
                }
                else
                {
                    AddNewNode(node);
                }
            }
        }

        // if the reader is on a start element, we read it
        public override void ReadStartElement()
        {
            // white space nodes are not supposed to be a limitation 
            if (LastNode.Type == XmlNodeType.Whitespace)
                ReadUntilNotNull();

            if (LastNode.Type == XmlNodeType.Element)
                ReadUntilNotNull();
        }

        // if the reader is on an end element, we read it
        public override void ReadEndElement()
        {
            // white space nodes are not supposed to be a limitation 
            if (LastNode.Type == XmlNodeType.Whitespace)
                ReadUntilNotNull();

            if (LastNode.Type == XmlNodeType.EndElement)
                ReadUntilNotNull();
        }

        public static XmlReader Create(Stream input)
        {
            StreamReader streamReader = new StreamReader(input);

            String xmlString = streamReader.ReadToEnd();

            XDocument xDocument = XDocument.Parse(xmlString);

            return xDocument.CreateReader();
        }

#if BRIDGE // this method is replaced by JSIL but not by BRIDGE
        public override void Dispose()
        {
            //throw new NotImplementedException();
        }
#endif

        public override string Value
        {
            get
            {
                object node = LastNode.RawNode;

                if (node is XAttribute)
                    return ((XAttribute)node).Value;

                else if (node is XText)
                    return ((XText)node).Value;

                // XDocument, XElement
                return string.Empty;
            }
        }

        public override bool MoveToNextAttribute()
        {
            if (LastNode.Type == XmlNodeType.Element)
            {
                return MoveToFirstAttribute();
            }
            else if (LastNode.Type == XmlNodeType.Attribute)
            {
                XAttribute attribute = (XAttribute)LastNode.RawNode;

                RemoveLastNode(); // return to the element

                if (LastNode.AttributEnumerator.MoveNext())
                {
                    attribute = (XAttribute)LastNode.AttributEnumerator.Current;

                    AddNewNode(new INTERNAL_NodeType(attribute, XmlNodeType.Attribute));

                    return true; // we find the next one 
                }

                AddNewNode(new INTERNAL_NodeType(attribute, XmlNodeType.Attribute));
            }

            return false; // there is no other attribut, so we keep the last
        }

        // if currently reading an attribute, this method is supposed to return to the element that contains the attribute
        public override bool MoveToElement()
        {
            if (LastNode.Type != XmlNodeType.Element)
            {
                if (LastNode.Type == XmlNodeType.Attribute)
                {
                    RemoveLastNode(); // return to the last node, but we do not increase the index to return on the same node than before
                }
                else
                {
                    do
                    {
                        if (!ToNextSiblingOrPreviousNode())
                            return false;

                    } while (LastNode.Type != XmlNodeType.Element);
                }
            }

            return true;
        }

        public override XmlNodeType MoveToContent()
        {
            if (LastNode.Type == XmlNodeType.Document) // the root document is not content, but everything else is.
                Read();

            return NodeType;
        }

        public override bool MoveToFirstAttribute()
        {
            if (LastNode.Type == XmlNodeType.Attribute)
            {
                RemoveLastNode();
            }

            if (LastNode.Type == XmlNodeType.Element)
            {
                XElement element = LastNode.ElementOrNull;

                if (element != null)
                {
                    if (element.Attributes().Count() > 0)
                    {
                        AddNewNode(new INTERNAL_NodeType(element.Attributes().ElementAt(0), XmlNodeType.Attribute));

                        return true;
                    }
                }
            }

            return false;
        }

        #region Override Attributes

        public override XmlReaderSettings Settings { get { return _reader.Settings; } }
        //public override XmlNodeType NodeType { get { return _reader.NodeType; } } // Implemented above
        //public override string Name { get { return _reader.Name; } } // Implemented above
        //public override string LocalName { get { return _reader.LocalName; } } // Implemented above
        public override string NamespaceURI { get { return _reader.NamespaceURI; } }
        public override string Prefix { get { return _reader.Prefix; } }
        public override bool HasValue { get { return _reader.HasValue; } }
        //public override string Value { get { return _reader.Value; } }// Implemented above
        public override int Depth { get { return _reader.Depth; } }
        public override string BaseURI { get { return _reader.BaseURI; } }
        //public override bool IsEmptyElement { get { return _reader.IsEmptyElement; } } // Implemented above
        public override bool IsDefault { get { return _reader.IsDefault; } }
        public override char QuoteChar { get { return _reader.QuoteChar; } }
        //public override XmlSpace XmlSpace { get { return _reader.XmlSpace; } } // Implemented above
        public override string XmlLang { get { return _reader.XmlLang; } }
        public override IXmlSchemaInfo SchemaInfo { get { return _reader.SchemaInfo; } }
        public override System.Type ValueType { get { return _reader.ValueType; } }
        public override int AttributeCount { get { return _reader.AttributeCount; } }
        public override string this[int i] { get { return _reader[i]; } }
        public override string this[string name] { get { return _reader[name]; } }
        public override string this[string name, string namespaceURI] { get { return _reader[name, namespaceURI]; } }
        public override bool CanResolveEntity { get { return _reader.CanResolveEntity; } }
        public override bool EOF { get { return _reader.EOF; } }
        public override ReadState ReadState { get { return _reader.ReadState; } }
        public override bool HasAttributes { get { return _reader.HasAttributes; } }
        public override XmlNameTable NameTable { get { return _reader.NameTable; } }

        #endregion

        #region NotImplemented

        public override string GetAttribute(int i)
        {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            throw new NotImplementedException();
        }

        public override bool ReadAttributeValue()
        {
            throw new NotImplementedException();
        }

        public override string LookupNamespace(string prefix)
        {
            throw new NotImplementedException();
        }

        public override void ResolveEntity()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
