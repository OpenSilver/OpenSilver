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
    internal class XamlNodeList
	{
		readonly List<XamlNodeInfo> nodes;

		internal XamlSchemaContext SchemaContext { get; }

		public XamlNodeList(XamlSchemaContext schemaContext)
		{
			if (schemaContext == null)
				throw new ArgumentNullException("schemaContext");
			Writer = new XamlNodeListWriter(this);
			SchemaContext = schemaContext;
			nodes = new List<XamlNodeInfo>();
		}

		public XamlNodeList(XamlSchemaContext schemaContext, int size)
		{
			if (schemaContext == null)
				throw new ArgumentNullException("schemaContext");
			Writer = new XamlNodeListWriter(this);
			SchemaContext = schemaContext;
			nodes = new List<XamlNodeInfo>(size);
		}

		public int Count
		{
			get { return nodes.Count; }
		}

		public XamlWriter Writer { get; }

		public void Clear()
		{
			nodes.Clear();
		}

		public XamlReader GetReader()
		{
			if (!((XamlNodeListWriter)Writer).IsClosed)
				throw new XamlException("Writer must be closed");
			return new XamlNodeListReader(this);
		}

		internal void Add(XamlNodeInfo node)
		{
			nodes.Add(node);
		}

		internal XamlNodeInfo GetNode(int position)
		{
			return nodes[position];
		}
	}
}
