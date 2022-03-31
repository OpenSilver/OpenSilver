

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    internal interface IXamlReader
    {
        XamlNodeType NodeType { get; }

        ObjectNodeData ObjectData { get; }

        MemberNodeData MemberData { get; }

        bool Read();
    }

    internal class XamlReader : IXamlReader
    {
        private IEnumerator<XamlNode> _it;

        public XamlReader(XDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public XDocument Document { get; }

        public bool IsEOF => _it != null && _it.Current.NodeType == XamlNodeType.None;

        public XamlNodeType NodeType => _it?.Current.NodeType ?? XamlNodeType.None;

        public ObjectNodeData ObjectData => (NodeType == XamlNodeType.StartObject || NodeType == XamlNodeType.EndObject) ? (ObjectNodeData)_it.Current.Value : null;

        public MemberNodeData MemberData => (NodeType == XamlNodeType.StartMember || NodeType == XamlNodeType.EndMember) ? (MemberNodeData)_it.Current.Value : null;

        public bool Read()
        {
            if (_it == null)
            {
                _it = XamlParser.Parse(Document).GetEnumerator();
            }

            _it.MoveNext();

            return _it.Current.NodeType != XamlNodeType.None;
        }
    }

    internal static class XamlParser
    {
        public static IEnumerable<XamlNode> Parse(XDocument document) => ParseDocument(document);

        private static IEnumerable<XamlNode> ParseDocument(XDocument document)
        {
            VerifyObjectNode(document.Root);

            foreach (XamlNode node in ParseObjectNode(document.Root))
            {
                yield return node;
            }

            yield return Node(XamlNodeType.None, null);
        }

        private static IEnumerable<XamlNode> ParseObjectNode(XElement element)
        {
            ObjectNodeData data = new ObjectNodeData(element);
            yield return Node(XamlNodeType.StartObject, data);

            foreach (XElement child in element.Elements())
            {
                if (IsMemberNode(child))
                {
                    foreach (XamlNode node in ParseMemberNode(child))
                    {
                        yield return node;
                    }
                }
                else
                {
                    foreach (XamlNode node in ParseObjectNode(child))
                    {
                        yield return node;
                    }

                    yield return Node(XamlNodeType.EndMember, new MemberNodeData(element, null, child));
                }
            }

            yield return Node(XamlNodeType.EndObject, data);
        }

        private static IEnumerable<XamlNode> ParseMemberNode(XElement element)
        {
            yield return Node(XamlNodeType.StartMember, new MemberNodeData(element.Parent, element, null));

            foreach (XElement child in element.Elements())
            {
                VerifyObjectNode(child);

                foreach (XamlNode node in ParseObjectNode(child))
                {
                    yield return node;
                }

                yield return Node(XamlNodeType.EndMember, new MemberNodeData(element.Parent, element, child));
            }
        }

        private static XamlNode Node(XamlNodeType nodeType, object value)
        {
            return new XamlNode(nodeType, value);
        }

        private static bool IsMemberNode(XElement element) => element.Name.LocalName.Contains(".");

        private static void VerifyObjectNode(XElement element)
        {
            if (IsMemberNode(element))
            {
                throw new XamlParseException("Nested properties", element);
            }
        }
    }

    internal enum XamlNodeType
    {
        None,
        StartObject,
        EndObject,
        StartMember,
        EndMember,
    }

    internal struct XamlNode
    {
        public XamlNode(XamlNodeType nodeType, object value)
        {
            NodeType = nodeType;
            Value = value;
        }

        public XamlNodeType NodeType { get; }

        public object Value { get; }
    }

    internal class ObjectNodeData
    {
        public ObjectNodeData(XElement element)
        {
            Element = element;
        }

        public XElement Element { get; }
    }

    internal class MemberNodeData
    {
        public MemberNodeData(XElement target, XElement member, XElement value)
        {
            Target = target;
            Member = member;
            Value = value;
        }

        public XElement Target { get; }

        public XElement Member { get; }

        public XElement Value { get; }
    }
}
