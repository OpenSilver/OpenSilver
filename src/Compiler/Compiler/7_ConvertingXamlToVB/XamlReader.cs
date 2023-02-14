

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
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler
{
    internal interface IXamlReaderVB
    {
        XamlNodeTypeVB NodeType { get; }

        ObjectNodeDataVB ObjectData { get; }

        MemberNodeDataVB MemberData { get; }

        bool Read();
    }

    internal class XamlReaderVB : IXamlReaderVB
    {
        private IEnumerator<XamlNodeVB> _it;

        public XamlReaderVB(XDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public XDocument Document { get; }

        public bool IsEOF => _it != null && _it.Current.NodeType == XamlNodeTypeVB.None;

        public XamlNodeTypeVB NodeType => _it?.Current.NodeType ?? XamlNodeTypeVB.None;

        public ObjectNodeDataVB ObjectData => (NodeType == XamlNodeTypeVB.StartObject || NodeType == XamlNodeTypeVB.EndObject) ? (ObjectNodeDataVB)_it.Current.Value : null;

        public MemberNodeDataVB MemberData => (NodeType == XamlNodeTypeVB.StartMember || NodeType == XamlNodeTypeVB.EndMember) ? (MemberNodeDataVB)_it.Current.Value : null;

        public bool Read()
        {
            if (_it == null)
            {
                _it = XamlParserVB.Parse(Document).GetEnumerator();
            }

            _it.MoveNext();

            return _it.Current.NodeType != XamlNodeTypeVB.None;
        }
    }

    internal static class XamlParserVB
    {
        public static IEnumerable<XamlNodeVB> Parse(XDocument document) => ParseDocument(document);

        private static IEnumerable<XamlNodeVB> ParseDocument(XDocument document)
        {
            VerifyObjectNode(document.Root);

            foreach (XamlNodeVB node in ParseObjectNode(document.Root))
            {
                yield return node;
            }

            yield return Node(XamlNodeTypeVB.None, null);
        }

        private static IEnumerable<XamlNodeVB> ParseObjectNode(XElement element)
        {
            ObjectNodeDataVB data = new ObjectNodeDataVB(element);
            yield return Node(XamlNodeTypeVB.StartObject, data);

            foreach (XElement child in element.Elements())
            {
                if (IsMemberNode(child))
                {
                    foreach (XamlNodeVB node in ParseMemberNode(child))
                    {
                        yield return node;
                    }
                }
                else
                {
                    foreach (XamlNodeVB node in ParseObjectNode(child))
                    {
                        yield return node;
                    }

                    yield return Node(XamlNodeTypeVB.EndMember, new MemberNodeDataVB(element, null, child));
                }
            }

            yield return Node(XamlNodeTypeVB.EndObject, data);
        }

        private static IEnumerable<XamlNodeVB> ParseMemberNode(XElement element)
        {
            yield return Node(XamlNodeTypeVB.StartMember, new MemberNodeDataVB(element.Parent, element, null));

            foreach (XElement child in element.Elements())
            {
                VerifyObjectNode(child);

                foreach (XamlNodeVB node in ParseObjectNode(child))
                {
                    yield return node;
                }

                yield return Node(XamlNodeTypeVB.EndMember, new MemberNodeDataVB(element.Parent, element, child));
            }
        }

        private static XamlNodeVB Node(XamlNodeTypeVB nodeType, object value)
        {
            return new XamlNodeVB(nodeType, value);
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

    internal enum XamlNodeTypeVB
    {
        None,
        StartObject,
        EndObject,
        StartMember,
        EndMember,
    }

    internal struct XamlNodeVB
    {
        public XamlNodeVB(XamlNodeTypeVB nodeType, object value)
        {
            NodeType = nodeType;
            Value = value;
        }

        public XamlNodeTypeVB NodeType { get; }

        public object Value { get; }
    }

    internal class ObjectNodeDataVB
    {
        public ObjectNodeDataVB(XElement element)
        {
            Element = element;
        }

        public XElement Element { get; }
    }

    internal class MemberNodeDataVB
    {
        public MemberNodeDataVB(XElement target, XElement member, XElement value)
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
