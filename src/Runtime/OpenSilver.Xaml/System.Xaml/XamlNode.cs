//
// Copyright (C) 2011 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xaml
{
	class XamlNodeInfo
	{
		public static readonly XamlNodeInfo EndMember = new XamlNodeInfo(XamlNodeType.EndMember, (XamlMember)null);

		public static readonly XamlNodeInfo EndObject = new XamlNodeInfo(XamlNodeType.EndObject, (XamlObject)null);

		public static readonly XamlNodeInfo GetObject = new XamlNodeInfo(XamlNodeType.GetObject, (XamlObject)null);

		public XamlNodeInfo()
		{
		}

		public XamlNodeInfo Set(XamlNodeType nodeType, XamlObject value)
		{
			NodeType = nodeType;
			Value = value;
			return this;
		}
		public XamlNodeInfo Set(XamlNodeType nodeType, XamlMember value)
		{
			NodeType = nodeType;
			Value = value;
			return this;
		}

		public XamlNodeInfo Set(object value)
		{
			NodeType = XamlNodeType.Value;
			Value = value;
			return this;
		}

		public XamlNodeInfo(XamlNodeType nodeType, XamlObject value)
		{
			NodeType = nodeType;
			Value = value;
		}

		public XamlNodeInfo(XamlNodeType nodeType, XamlMember member)
		{
			NodeType = nodeType;
			Value = member;
		}

		public XamlNodeInfo(object value)
		{
			NodeType = XamlNodeType.Value;
			Value = value;
		}

		public XamlNodeInfo(NamespaceDeclaration ns)
		{
			NodeType = XamlNodeType.NamespaceDeclaration;
			Value = ns;
		}

		public XamlNodeType NodeType { get; private set; }

		public XamlObject Object => (XamlObject)Value;

		public XamlMember Member => (XamlMember)Value;

		public object Value { get; private set; }

		public XamlNodeInfo Copy()
		{
			var node = new XamlNodeInfo();
			node.NodeType = NodeType;
			node.Value = Value;
			var obj = node.Value as XamlObject;
			if (obj != null)
				node.Value = new XamlObject(obj.Type, obj.Value);
			return node;
		}

		public override string ToString()
		{
			return $"[XamlNodeInfo: NodeType={NodeType}, Value={Value}]";
		}
	}

	struct XamlNodeLineInfo
	{
		public readonly XamlNodeInfo Node;
		public readonly int LineNumber, LinePosition;
		public XamlNodeLineInfo(XamlNodeInfo node, int line, int column)
		{
			Node = node;
			LineNumber = line;
			LinePosition = column;
		}
	}

	class XamlObject
	{
		public XamlObject()
		{
		}

		public XamlObject Set(XamlType type, object instance)
		{
			Type = type;
			Value = instance;
			return this;
		}

		public XamlObject(XamlType type, object instance)
		{
			Type = type;
			Value = instance;
		}

		public object Value { get; private set; }

		public XamlType Type { get; private set; }

		public object GetMemberValue(XamlMember xm)
		{
			if (xm.IsUnknown)
				return null;

			var obj = Value;
			// FIXME: this looks like an ugly hack. Is this really true? What if there's MarkupExtension that uses another MarkupExtension type as a member type.
			if (xm.IsAttachable
				|| xm.IsDirective // is this correct?
				/*
				|| ReferenceEquals(xm, XamlLanguage.Initialization)
				|| ReferenceEquals(xm, XamlLanguage.Items) // collection itself
				|| ReferenceEquals(xm, XamlLanguage.Arguments) // object itself
				|| ReferenceEquals(xm, XamlLanguage.PositionalParameters) // dummy value
				*/
				)
				return obj;
			return xm.Invoker.GetValue(obj);
		}

		public XamlObject GetMemberObjectValue(XamlMember xm)
		{
			var mv = GetMemberValue(xm);
			return new XamlObject(GetType(mv), mv);
		}


		XamlType GetType(object obj)
		{
			return obj == null ? XamlLanguage.Null : Type.SchemaContext.GetXamlType(obj.GetType());
		}
	}

	class XamlNodeMember
	{
		public XamlNodeMember()
		{
		}

		public XamlNodeMember Set(XamlObject owner, XamlMember member)
		{
			Owner = owner;
			Member = member;
			return this;
		}

		public XamlObject Owner;

		public XamlMember Member;

		public XamlObject GetValue(XamlObject xobj)
		{
			var mv = Owner.GetMemberValue(Member);
			return xobj.Set(GetType(mv), mv);
		}

		XamlType GetType(object obj)
		{
			return obj == null ? XamlLanguage.Null : Owner.Type.SchemaContext.GetXamlType(obj.GetType());
		}
	}
}
