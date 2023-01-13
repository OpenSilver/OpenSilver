//
// Copyright (C) 2010 Novell Inc. http://novell.com
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml.Schema;
using System.Xml;

namespace System.Xaml
{
    internal class XamlXmlReader : XamlReader, IXamlLineInfo
	{
		#region constructors

		public XamlXmlReader(Stream stream)
			: this(stream, (XamlXmlReaderSettings)null)
		{
		}

		public XamlXmlReader(string fileName)
			: this(fileName, (XamlXmlReaderSettings)null)
		{
		}

		public XamlXmlReader(TextReader textReader)
			: this(textReader, (XamlXmlReaderSettings)null)
		{
		}

		public XamlXmlReader(XmlReader xmlReader)
			: this(xmlReader, (XamlXmlReaderSettings)null)
		{
		}

		public XamlXmlReader(Stream stream, XamlSchemaContext schemaContext)
			: this(stream, schemaContext, null)
		{
		}

		public XamlXmlReader(Stream stream, XamlXmlReaderSettings settings)
			: this(stream, new XamlSchemaContext(null, null), settings)
		{
		}

		public XamlXmlReader(string fileName, XamlSchemaContext schemaContext)
			: this(fileName, schemaContext, null)
		{
		}

		public XamlXmlReader(string fileName, XamlXmlReaderSettings settings)
			: this(fileName, new XamlSchemaContext(null, null), settings)
		{
		}

		public XamlXmlReader(TextReader textReader, XamlSchemaContext schemaContext)
			: this(textReader, schemaContext, null)
		{
		}

		public XamlXmlReader(TextReader textReader, XamlXmlReaderSettings settings)
			: this(textReader, new XamlSchemaContext(null, null), settings)
		{
		}

		public XamlXmlReader(XmlReader xmlReader, XamlSchemaContext schemaContext)
			: this(xmlReader, schemaContext, null)
		{
		}

		public XamlXmlReader(XmlReader xmlReader, XamlXmlReaderSettings settings)
			: this(xmlReader, new XamlSchemaContext(null, null), settings)
		{
		}

		public XamlXmlReader(Stream stream, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			Initialize(CreateReader(stream, settings), schemaContext, settings);
		}

		public XamlXmlReader(string fileName, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			Initialize(CreateReader(fileName, settings), schemaContext, settings);
		}

		static XmlReader CreateReader(string fileName, XamlXmlReaderSettings settings)
		{
			return CreateReader(XmlReader.Create(fileName, CreateReaderSettings(settings, closeInput: true)), settings);
		}

		public XamlXmlReader(TextReader textReader, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			Initialize(CreateReader(textReader, settings), schemaContext, settings);
		}

		public XamlXmlReader(XmlReader xmlReader, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			Initialize(CreateReader(xmlReader, settings), schemaContext, settings);
		}

		void Initialize(XmlReader xmlReader, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			parser = new XamlXmlParser(xmlReader, schemaContext, settings);
		}

		static XmlReader CreateReader(Stream stream, XamlXmlReaderSettings settings)
		{
			if (settings?.RequiresXmlContext != true)
				return XmlReader.Create(stream, CreateReaderSettings(settings));

			return XmlReader.Create(stream, CreateReaderSettings(settings, ConformanceLevel.Fragment), settings.CreateXmlContext());
		}

		static XmlReader CreateReader(TextReader reader, XamlXmlReaderSettings settings)
		{
			if (settings?.RequiresXmlContext != true)
				return XmlReader.Create(reader, CreateReaderSettings(settings));

			return XmlReader.Create(reader, CreateReaderSettings(settings, ConformanceLevel.Fragment), settings.CreateXmlContext());
		}

		static XmlReader CreateReader(XmlReader xmlReader, XamlXmlReaderSettings settings)
		{
			if (settings?.RequiresXmlContext != true)
				return XmlReader.Create(xmlReader, CreateReaderSettings(settings));

			// need to read from a TextReader to load a fragment, so we copy the xml of the current reader
			xmlReader.Read();
			var reader = new StringReader(xmlReader.ReadOuterXml());
			return XmlReader.Create(reader, CreateReaderSettings(settings, ConformanceLevel.Fragment), settings.CreateXmlContext());
		}

		static XmlReaderSettings CreateReaderSettings(XamlXmlReaderSettings settings, ConformanceLevel conformance = ConformanceLevel.Document, bool? closeInput = null)
		{
			return new XmlReaderSettings
			{
				CloseInput = closeInput ?? settings?.CloseInput ?? false,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true,
				ConformanceLevel = conformance
			};
		}

		#endregion

		XamlXmlParser parser;
		IEnumerator<XamlXmlNodeInfo> iter;

		public bool HasLineInfo
		{
			get { return iter != null ? iter.Current.HasLineInfo : false; }
		}

		public override bool IsEof
		{
			get { return iter != null ? iter.Current.NodeType == XamlNodeType.None : false; }
		}

		public int LineNumber
		{
			get { return iter != null ? iter.Current.LineNumber : 0; }
		}

		public int LinePosition
		{
			get { return iter != null ? iter.Current.LinePosition : 0; }
		}

		public override XamlMember Member
		{
			get { return iter != null && iter.Current.NodeType == XamlNodeType.StartMember ? (XamlMember)iter.Current.NodeValue : null; }
		}

		public override NamespaceDeclaration Namespace
		{
			get { return iter != null && iter.Current.NodeType == XamlNodeType.NamespaceDeclaration ? (NamespaceDeclaration)iter.Current.NodeValue : null; }
		}

		public override XamlNodeType NodeType
		{
			get { return iter != null ? iter.Current.NodeType : XamlNodeType.None; }
		}

		public override XamlSchemaContext SchemaContext
		{
			get { return parser.SchemaContext; }
		}

		public override XamlType Type
		{
			get { return iter != null && iter.Current.NodeType == XamlNodeType.StartObject ? (XamlType)iter.Current.NodeValue : null; }
		}

		public override object Value
		{
			get { return iter != null && iter.Current.NodeType == XamlNodeType.Value ? iter.Current.NodeValue : null; }
		}

		public override bool Read()
		{
			if (IsDisposed)
				throw new ObjectDisposedException("reader");
			if (iter == null)
				iter = parser.Parse().GetEnumerator();
			iter.MoveNext();
			return iter.Current.NodeType != XamlNodeType.None;
		}
	}

	struct XamlXmlNodeInfo
	{
		public XamlXmlNodeInfo(XamlNodeType nodeType, object nodeValue, IXmlLineInfo lineInfo)
		{
			NodeType = nodeType;
			NodeValue = nodeValue;
			if (lineInfo != null && lineInfo.HasLineInfo())
			{
				HasLineInfo = true;
				LineNumber = lineInfo.LineNumber;
				LinePosition = lineInfo.LinePosition;
			}
			else
			{
				HasLineInfo = false;
				LineNumber = 0;
				LinePosition = 0;
			}
		}

		public bool HasLineInfo;
		public int LineNumber;
		public int LinePosition;
		public XamlNodeType NodeType;
		public object NodeValue;
	}

	class XamlXmlParser
	{
		static XamlXmlReaderSettings default_settings = new XamlXmlReaderSettings();
		public XamlXmlParser(XmlReader xmlReader, XamlSchemaContext schemaContext, XamlXmlReaderSettings settings)
		{
			if (xmlReader == null)
				throw new ArgumentNullException(nameof(xmlReader));
			if (schemaContext == null)
				throw new ArgumentNullException(nameof(schemaContext));

			sctx = schemaContext;
			this.settings = settings ?? default_settings;
			if (settings?.SkipXmlCompatibilityProcessing != true)
			{
				xmlReader = new CompatibleXmlReader(xmlReader, schemaContext);
			}

			r = xmlReader;
			line_info = r as IXmlLineInfo;
			xaml_namespace_resolver = new NamespaceResolver(r as IXmlNamespaceResolver);
		}

		XmlReader r;
		IXmlLineInfo line_info;
		XamlSchemaContext sctx;
		XamlXmlReaderSettings settings;
		IXamlNamespaceResolver xaml_namespace_resolver;

		internal XmlReader Reader
		{
			get { return r; }
		}

		public XamlSchemaContext SchemaContext
		{
			get { return sctx; }
		}

		XamlXmlNodeInfo Node(XamlNodeType nodeType, object nodeValue)
		{
			return new XamlXmlNodeInfo(nodeType, nodeValue, line_info);
		}

		/// <summary>
		/// Create new <see cref="XamlXmlNodeInfo"/> Use this overload only then you want pass deferred line info information to instance
		/// </summary>
		/// <param name="nodeType">Type of new node</param>
		/// <param name="nodeValue">Value of new node</param>
		/// <param name="lineInfo">Line info</param>
		/// <returns></returns>
		XamlXmlNodeInfo Node(XamlNodeType nodeType, object nodeValue, IXmlLineInfo lineInfo)
		{
			return new XamlXmlNodeInfo(nodeType, nodeValue, lineInfo);
		}

		public IEnumerable<XamlXmlNodeInfo> Parse()
		{
			r.MoveToContent();
			foreach (var xi in ReadObjectElement(null, null))
				yield return xi;
			yield return Node(XamlNodeType.None, null);
		}

		string ResolveLocalNamespace(string ns)
		{
			if (settings.LocalAssembly != null && ns.StartsWith("clr-namespace:", StringComparison.Ordinal) && ns.IndexOf(';') == -1)
			{
				ns += ";assembly=" + settings.LocalAssembly.GetName().Name;
			}
			return ns;
		}

		IEnumerable<XamlXmlNodeInfo> ReadElementContent(XamlType parentType, XamlMember currentMember)
		{
			while (r.NodeType != XmlNodeType.EndElement)
			{
				if (currentMember == XamlLanguage.Items)
				{
					foreach (var x in ReadCollectionItems(parentType, currentMember))
					{
						if (x.NodeType == XamlNodeType.None)
							yield break;
						yield return x;
					}
				}
				else if (r.NodeType == XmlNodeType.Text || r.NodeType == XmlNodeType.CDATA)
				{
					if (currentMember.Type.IsCollection ||
						currentMember.Type.IsDictionary ||
						currentMember == XamlLanguage.UnknownContent)
					{
						yield return Node(XamlNodeType.GetObject, currentMember.Type);
						foreach (var ni in ReadMembers(parentType, currentMember.Type))
							yield return ni;
						yield return Node(XamlNodeType.EndObject, currentMember.Type);
					}
					else
					{
						yield return Node(XamlNodeType.Value, NormalizeWhitespace(r.Value));
						r.Read();
					}
				}
				else
				{
					foreach (var x in ReadObjectElement(parentType, currentMember))
					{
						if (x.NodeType == XamlNodeType.None)
							yield break;
						yield return x;
					}
				}
			}
		}

		Stack<bool> _preserveWhitespace = new Stack<bool>();

		// Note that it could return invalid (None) node to tell the caller that it is not really an object element.
		IEnumerable<XamlXmlNodeInfo> ReadObjectElement(XamlType parentType, XamlMember currentMember)
		{
			if (r.NodeType == XmlNodeType.Whitespace)
				r.MoveToContent();

			if (r.NodeType == XmlNodeType.EndElement)
				yield break;

			if (r.NodeType != XmlNodeType.Element)
			{
				throw new XamlParseException(String.Format("Element is expected, but got {0}", r.NodeType));
			}

			if (r.MoveToFirstAttribute())
			{
				do
				{
					if (r.NamespaceURI == XamlLanguage.Xmlns2000Namespace)
						yield return Node(XamlNodeType.NamespaceDeclaration, new NamespaceDeclaration(ResolveLocalNamespace(r.Value), r.Prefix == "xmlns" ? r.LocalName : String.Empty));
				} while (r.MoveToNextAttribute());
				r.MoveToElement();
			}

			var sti = GetStartTagInfo();

			var xt = sctx.GetXamlType(sti.TypeName);
			if (ReferenceEquals(xt, null))
			{
				// Current element could be for another member in the parent type (if exists)
				if (parentType != null && (r.LocalName.IndexOf('.') > 0 || parentType.GetMember(r.LocalName) != null))
				{
					// stop the iteration and signal the caller to not read current element as an object. (It resolves conflicts between "start object for current collection's item" and "start member for the next member in the parent object".
					yield return Node(XamlNodeType.None, null);
					yield break;
				}

				// creates name-only XamlType. Also, it does not seem that it does not store this XamlType to XamlSchemaContext (Try GetXamlType(xtn) after reading such xaml node, it will return null).
				xt = new XamlType(sti.Namespace, sti.Name, sti.TypeName.TypeArguments?.Select(xxtn => sctx.GetXamlType(xxtn)).ToArray(), sctx);
			}

			// It could still be GetObject if current_member is not defer-loaded, it
			// is not a directive, and current type is not
			// a markup extension.
			// (I'm not very sure about the condition;
			// it could be more complex.)
			// seealso: bug #682131
			if (!ReferenceEquals(currentMember, null)
				&& ReferenceEquals(currentMember.DeferringLoader, null)
				&& !xt.CanAssignTo(currentMember.Type)
				&& !ReferenceEquals(xt, XamlLanguage.Reference)
				&& (
					currentMember.DeclaringType?.ContentProperty == currentMember
					|| (!currentMember.IsDirective && !xt.IsMarkupExtension)
				   )
			   )
			{
				yield return Node(XamlNodeType.GetObject, currentMember.Type);
				foreach (var ni in ReadMembers(parentType, currentMember.Type))
					yield return ni;
				yield return Node(XamlNodeType.EndObject, currentMember.Type);
				yield break;
			}

			yield return Node(XamlNodeType.StartObject, xt);

			// process attribute members (including MarkupExtensions)
			ProcessAttributesToMember(sti, xt);

			for (int i = 0; i < sti.Members.Count; i++)
			{
				var memberInfo = sti.Members[i];
				yield return Node(XamlNodeType.StartMember, memberInfo.Member, memberInfo.LineInfo);

				// Try markup extension
				// FIXME: is this rule correct?
				var v = memberInfo.Value;
				if (!string.IsNullOrEmpty(v) && v[0] == '{')
				{
					if (v.Length >= 2 && v[1] == '}')
					{
						// escaped value with {} at the beginning of the string
						yield return Node(XamlNodeType.Value, v.Substring(2));
					}
					else
					{
						var pai = new ParsedMarkupExtensionInfo(v, xaml_namespace_resolver, sctx);
						pai.Parse();
						foreach (var node in ReadMarkup(pai))
							yield return node;
					}
				}
				else
					yield return Node(XamlNodeType.Value, v, memberInfo.LineInfo);

				yield return Node(XamlNodeType.EndMember, memberInfo.Member, memberInfo.LineInfo);
			}

			// process content members
			if (!r.IsEmptyElement)
			{
				if (sti.PreserveWhitespace != null)
					_preserveWhitespace.Push(sti.PreserveWhitespace.Value);

				r.Read();
				foreach (var ni in ReadMembers(parentType, xt))
					yield return ni;
				r.ReadEndElement();

				if (sti.PreserveWhitespace != null)
					_preserveWhitespace.Pop();
			}
			else
				r.Read(); // consume empty element.

			yield return Node(XamlNodeType.EndObject, xt);
		}

		IEnumerable<XamlXmlNodeInfo> ReadMarkup(ParsedMarkupExtensionInfo pai)
		{
			yield return Node(XamlNodeType.StartObject, pai.Type);

			foreach (var xepair in pai.Arguments)
			{
				yield return Node(XamlNodeType.StartMember, xepair.Key);
				var list = xepair.Value as List<object>;
				if (list != null)
				{
					foreach (var s in list)
					{
						foreach (var node in ReadMarkupArgument(s))
							yield return node;
					}
				}
				else
				{
					foreach (var node in ReadMarkupArgument(xepair.Value))
						yield return node;
				}
				yield return Node(XamlNodeType.EndMember, xepair.Key);
			}

			yield return Node(XamlNodeType.EndObject, pai.Type);
		}

		IEnumerable<XamlXmlNodeInfo> ReadMarkupArgument(object value)
		{
			var markup = value as ParsedMarkupExtensionInfo;
			if (markup != null)
			{
				foreach (var node in ReadMarkup(markup))
					yield return node;
			}
			else
				yield return Node(XamlNodeType.Value, value);
		}

		IEnumerable<XamlXmlNodeInfo> ReadMembers(XamlType parentType, XamlType xt)
		{
			for (r.MoveToContent(); r.NodeType != XmlNodeType.EndElement; r.MoveToContent())
			{
				switch (r.NodeType)
				{
					case XmlNodeType.Element:
						// FIXME: parse type arguments etc.
						foreach (var x in ReadMemberElement(parentType, xt))
						{
							if (x.NodeType == XamlNodeType.None)
								yield break;
							yield return x;
						}
						continue;
					default:
						foreach (var x in ReadMemberText(parentType, xt))
							yield return x;
						continue;
				}
			}
		}

		StartTagInfo GetStartTagInfo()
		{
			string name = r.LocalName;
			string ns = ResolveLocalNamespace(r.NamespaceURI);
			string typeArgNames;

			var members = new List<MemberInfo>();
			var atts = ProcessAttributes(r, members, out typeArgNames, out var preserveWhitespace);

			IList<XamlTypeName> typeArgs = typeArgNames == null ? null : XamlTypeName.ParseList(typeArgNames, xaml_namespace_resolver);
			var xtn = new XamlTypeName(ns, name, typeArgs);
			return new StartTagInfo { Name = name, Namespace = ns, TypeName = xtn, Members = members, Attributes = atts, PreserveWhitespace = preserveWhitespace };
		}

		bool xmlbase_done;

		// returns remaining attributes to be processed
		List<AttributeInfo> ProcessAttributes(XmlReader r, List<MemberInfo> members, out string typeArgNames, out bool? preserveWhitespace)
		{
			// base (top element)
			if (!xmlbase_done)
			{
				xmlbase_done = true;
				string xmlbase = r.GetAttribute("base", XamlLanguage.Xml1998Namespace) ?? r.BaseURI;
				if (xmlbase != null)
					members.Add(new MemberInfo(XamlLanguage.Base, xmlbase, line_info));
			}
			typeArgNames = null;
			preserveWhitespace = null;
			var atts = new List<AttributeInfo>();
			var tagNamespace = r.NamespaceURI;
			if (r.MoveToFirstAttribute())
			{
				do
				{
					switch (r.NamespaceURI)
					{
						case XamlLanguage.Xml1998Namespace:
							switch (r.LocalName)
							{
								case "base":
									continue; // already processed.
								case "lang":
									members.Add(new MemberInfo(XamlLanguage.Lang, r.Value, line_info));
									continue;
								case "space":
									members.Add(new MemberInfo(XamlLanguage.Space, r.Value, line_info));
									if (r.Value == "preserve")
										preserveWhitespace = true;
									if (r.Value == "default")
										preserveWhitespace = false;
									continue;
							}
							break;
						case XamlLanguage.Xmlns2000Namespace:
							continue;
						case XamlLanguage.Xaml2006Namespace:
							XamlDirective d = FindStandardDirective(r.LocalName, AllowedMemberLocations.Attribute);
							if (d != null)
							{
								// check TypeArguments to resolve Type, and remove them from the list. They don't appear as a node.
								if (ReferenceEquals(d, XamlLanguage.TypeArguments))
								{
									typeArgNames = r.Value;
									continue;
								}
								members.Add(new MemberInfo(d, r.Value, line_info));
								continue;
							}
							throw new NotSupportedException(String.Format("Attribute '{0}' is not supported", r.Name));
						default:
							if (string.IsNullOrEmpty(r.NamespaceURI) || tagNamespace == r.NamespaceURI || r.LocalName.IndexOf('.') > 0)
							{
								atts.Add(new AttributeInfo(r.Name, r.Value, line_info));
								continue;
							}
							// Custom directive
							members.Add(new MemberInfo(SchemaContext.GetXamlDirective(r.NamespaceURI, r.LocalName), r.Value,
								line_info));
							break;
					}
				} while (r.MoveToNextAttribute());
				r.MoveToElement();
			}
			return atts;
		}

		XamlMember FindAttachableMember(string prefix, string name)
		{
			var idx = name.IndexOf('.');
			if (idx <= 0)
				return null;
			var typeName = name.Substring(0, idx);
			var memberName = name.Substring(idx + 1);
			return FindAttachableMember(prefix, typeName, memberName);
		}

		XamlMember FindAttachableMember(string prefix, string typeName, string memberName)
		{
			string apns = r.LookupNamespace(prefix);
			var axtn = new XamlTypeName(apns, typeName, null);
			var at = sctx.GetXamlType(axtn);
			return at?.GetAttachableMember(memberName);
		}

		void ProcessAttributesToMember(StartTagInfo sti, XamlType xt)
		{
			sti.Members.Capacity = Math.Max(sti.Members.Capacity, sti.Members.Count + sti.Attributes.Count);
			for (int i = 0; i < sti.Attributes.Count; i++)
			{
				var p = sti.Attributes[i];
				int idx = p.Name.IndexOf(':');
				string prefix = idx > 0 ? p.Name.Substring(0, idx) : String.Empty;
				string name = idx > 0 ? p.Name.Substring(idx + 1) : p.Name;

				var am = FindAttachableMember(prefix, name);
				if (am != null)
				{
					sti.Members.Add(new MemberInfo(am, p.Value, p.LineInfo));
					continue;
				}
				var xm = xt.GetMember(name);
				if (xm != null)
					sti.Members.Add(new MemberInfo(xm, p.Value, p.LineInfo));
				else
					// unknown attributes go through!
					sti.Members.Add(new MemberInfo(new XamlMember(name, xt, false), p.Value, p.LineInfo));
			}
		}

		// returns an optional member without xml node.
		XamlMember GetExtraMember(XamlType xt)
		{
			if (xt.ContentProperty != null) // e.g. Array.Items
				return xt.ContentProperty;
			if (xt.IsCollection || xt.IsDictionary)
				return XamlLanguage.Items;
			return null;
		}

		static XamlDirective FindStandardDirective(string name, AllowedMemberLocations loc)
		{
			return XamlLanguage.AllDirectives.FirstOrDefault(dd => (dd.AllowedLocation & loc) != 0 && dd.Name == name);
		}

		string NormalizeWhitespace(
			string value,
			bool normalizeStart = true,
			bool normalizeEnd = true)
		{
			var preserve = _preserveWhitespace.Count > 0 ? _preserveWhitespace.Peek() : settings.XmlSpacePreserve;
			if (preserve)
				return value;

			StringBuilder sb = null;
			bool lastWasWhitesp = false;
			for (var index = 0; index < value.Length; index++)
			{
				var c = value[index];
				if (c == ' ' || c == '\n' || c == '\t' || c == '\r')
				{
					if (sb == null)
					{
						sb = new StringBuilder(value.Length);
						sb.Append(value, 0, index);
					}
					if (lastWasWhitesp || (sb.Length == 0 && normalizeStart))
						continue;
					lastWasWhitesp = true;
					sb.Append(' ');
					continue;
				}
				else
				{
					lastWasWhitesp = false;
				}

				sb?.Append(c);
			}

			if (sb == null)
				return value;

			if (lastWasWhitesp && normalizeEnd)
				sb.Length--;

			return sb.ToString();
		}

		IEnumerable<XamlXmlNodeInfo> ReadMemberText(XamlType parentType, XamlType xt)
		{
			if (!xt.IsCollection)
			{
				if (xt.ContentProperty == null)
				{
					yield return Node(XamlNodeType.StartMember, XamlLanguage.Initialization);
					yield return Node(XamlNodeType.Value, NormalizeWhitespace(r.Value));
					r.Read();
					yield return Node(XamlNodeType.EndMember, XamlLanguage.Initialization);
				}
				else
				{
					foreach (var x in ReadMember(xt, xt.ContentProperty))
						yield return x;
				}
			}
			else
			{
				yield return Node(XamlNodeType.StartMember, XamlLanguage.Items);
				foreach (var x in ReadCollectionItems(xt, XamlLanguage.Items))
					yield return x;
				yield return Node(XamlNodeType.EndMember, XamlLanguage.Items);
			}
		}

		// member element, implicit member, children via content property, or value
		IEnumerable<XamlXmlNodeInfo> ReadMemberElement(XamlType parentType, XamlType xt)
		{
			XamlMember xm = null;
			var name = r.LocalName;
			int idx = name.IndexOf('.');
			string typeName = null;
			if (idx >= 0)
			{
				typeName = name.Substring(0, idx);
				name = name.Substring(idx + 1);
				// check if it is an attachable member first, either of this type or another type
				// Should this also check the namespace to find the correct type?
				xm = FindAttachableMember(r.Prefix, typeName, name);
				if (xm == null)
				{
					var memberType = SchemaContext.GetXamlType(new XamlTypeName(r.LookupNamespace(r.Prefix), typeName, null));
					if (xt.CanAssignTo(memberType))
						xm = memberType.GetMember(name);
				}
			}
			else
			{
				xm = (XamlMember)FindStandardDirective(name, AllowedMemberLocations.MemberElement);
				if (xm == null)
				{
					// still not? could it be omitted as content property or items ?
					if ((xm = GetExtraMember(xt)) != null)
					{
						// Note that this does not involve r.Read()
						foreach (var ni in ReadMember(xt, xm))
							yield return ni;
						yield break;
					}
				}
			}
			if (xm == null)
			{
				// Current element could be for another member in the parent type (if exists)
				if (parentType != null
					&& typeName != null
					&& typeName == parentType.InternalXmlName
					&& parentType.GetMember(name) != null)
				{
					// stop the iteration and signal the caller to not read current element as an object. (It resolves conflicts between "start object for current collection's item" and "start member for the next member in the parent object".
					yield return Node(XamlNodeType.None, null);
					yield break;
				}

				if (idx >= 0)
				{
					// Unknown property.
					xm = new XamlMember(name, xt, false); // FIXME: not sure if isAttachable is always false.
				}
				else
				{
					// Unknown content member.
					xm = XamlLanguage.UnknownContent;
					foreach (var ni in ReadMember(xt, xm))
						yield return ni;
					yield break;
				}
			}

			if (!r.IsEmptyElement)
			{
				bool popWhitespace = false;
				if (r.HasAttributes)
				{
					var space = r.GetAttribute("space", XamlLanguage.Xml1998Namespace);
					if (space == "preserve")
					{
						_preserveWhitespace.Push(true);
						popWhitespace = true;
					}
					else if (space == "default")
					{
						_preserveWhitespace.Push(false);
						popWhitespace = true;
					}
				}

				r.Read();
				foreach (var ni in ReadMember(xt, xm))
					yield return ni;
				r.MoveToContent();
				r.ReadEndElement();

				if (popWhitespace)
					_preserveWhitespace.Pop();
			}
			else
				r.Read();
		}

		IEnumerable<XamlXmlNodeInfo> ReadMember(XamlType parentType, XamlMember xm)
		{
			yield return Node(XamlNodeType.StartMember, xm);

			if (xm.IsEvent)
			{
				yield return Node(XamlNodeType.Value, r.Value);
				r.Read();
			}
			else if (!xm.IsWritePublic)
			{
				if (xm.Type.IsXData)
					foreach (var ni in ReadXData())
						yield return ni;
				else if (xm.Type.IsCollection || xm.Type.IsDictionary)
				{
					yield return Node(XamlNodeType.GetObject, xm.Type);
					yield return Node(XamlNodeType.StartMember, XamlLanguage.Items);
					foreach (var ni in ReadCollectionItems(xm.Type, XamlLanguage.Items))
					{
						if (ni.NodeType == XamlNodeType.None)
							break;
						yield return ni;
					}
					yield return Node(XamlNodeType.EndMember, XamlLanguage.Items);
					yield return Node(XamlNodeType.EndObject, xm.Type);
				}
				else
					throw WithLineInfo(new XamlParseException(String.Format("Read-only member '{0}' showed up in the source XML, and the xml contains element content that cannot be read.", xm.Name)));
			}
			else
			{
				foreach (var ni in ReadElementContent(parentType, xm))
				{
					if (ni.NodeType == XamlNodeType.None)
						throw new Exception("should not happen");
					yield return ni;
				}
			}

			yield return Node(XamlNodeType.EndMember, xm);
		}

		IEnumerable<XamlXmlNodeInfo> ReadCollectionItems(XamlType parentType, XamlMember xm)
		{
			bool GetNextElementTrim()
			{
				if (r.NodeType == XmlNodeType.Element)
				{
					var sti = GetStartTagInfo();
					var xt = sctx.GetXamlType(sti.TypeName);
					return xt.TrimSurroundingWhitespace;
				}

				return true;
			}

			XamlTypeName previous = null;

			r.MoveToContent();

			while (r.NodeType != XmlNodeType.EndElement)
			{
				switch (r.NodeType)
				{
					case XmlNodeType.Text:
						var text = r.Value;
						r.Read();

						var trimStart = previous == null ||
							sctx.GetXamlType(previous).TrimSurroundingWhitespace;
						var trimEnd = r.NodeType == XmlNodeType.EndElement ||
							GetNextElementTrim();

						yield return Node(XamlNodeType.Value, NormalizeWhitespace(text, trimStart, trimEnd));
						break;

					case XmlNodeType.Element:
						if (parentType != null && (r.LocalName.IndexOf('.') > 0 || parentType.GetMember(r.LocalName) != null))
						{
							yield return Node(XamlNodeType.None, null);
							yield break;
						}

						var sti = GetStartTagInfo();
						previous = sti.TypeName;

						foreach (var x in ReadObjectElement(parentType, xm))
						{
							yield return x;
						}
						break;

					case XmlNodeType.Whitespace:
						r.Read();
						if (previous != null &&
							parentType.IsWhitespaceSignificantCollection &&
							!sctx.GetXamlType(previous).TrimSurroundingWhitespace)
						{
							if (r.NodeType != XmlNodeType.EndElement)
								yield return Node(XamlNodeType.Value, " ");
						}
						break;

					default:
						throw new XamlParseException(String.Format("Text or Element is expected, but got {0}", r.NodeType));
				}
			}
		}

		IEnumerable<XamlXmlNodeInfo> ReadXData()
		{
			var xt = XamlLanguage.XData;
			var xm = xt.GetMember("Text");
			yield return Node(XamlNodeType.StartObject, xt);
			yield return Node(XamlNodeType.StartMember, xm);
			yield return Node(XamlNodeType.Value, r.ReadInnerXml());
			yield return Node(XamlNodeType.EndMember, xm);
			yield return Node(XamlNodeType.EndObject, xt);
		}

        XamlParseException WithLineInfo(XamlParseException ex)
		{
			ex.SetLineInfo(LineNumber, LinePosition);
			return ex;
		}

		public int LineNumber
		{
			get { return line_info != null && line_info.HasLineInfo() ? line_info.LineNumber : 0; }
		}

		public int LinePosition
		{
			get { return line_info != null && line_info.HasLineInfo() ? line_info.LinePosition : 0; }
		}

		internal struct LineInfo : IXmlLineInfo
		{
			public LineInfo(IXmlLineInfo xamlLineInfo)
			{
				if (xamlLineInfo != null)
				{
					LineNumber = xamlLineInfo.LineNumber;
					LinePosition = xamlLineInfo.LinePosition;
					LinePosition = xamlLineInfo.LinePosition;
				}
				else
				{
					LineNumber = 0;
					LinePosition = 0;
				}
			}

			public bool HasLineInfo() => true;
			public int LineNumber { get; }
			public int LinePosition { get; }
		}

		/// <summary>
		/// Information about member in xaml document
		/// </summary>
		internal struct MemberInfo
		{
			private MemberInfo(XamlMember member, string name)
			{
				Member = member;
				Value = name;
				LineInfo = new LineInfo();
			}

			public MemberInfo(XamlMember member, string value, LineInfo lineInfo) : this(member, value)
			{
				LineInfo = lineInfo;
			}

			public MemberInfo(XamlMember member, string value, IXmlLineInfo lineInfo) : this(member, value)
			{
				LineInfo = new LineInfo(lineInfo);
			}

			/// <summary>
			/// Xaml member connected with this information
			/// </summary>
			public XamlMember Member;

			/// <summary>
			/// Value of the member
			/// </summary>
			public string Value;

			/// <summary>
			/// Provided line information
			/// </summary>
			public LineInfo LineInfo;
		}

		/// <summary>
		/// Information about attribute in xaml document
		/// </summary>
		internal struct AttributeInfo
		{
			private AttributeInfo(string name, string value)
			{
				Name = name;
				Value = value;
				LineInfo = new LineInfo();
			}

			public AttributeInfo(string name, string value, LineInfo lineInfo) : this(name, value)
			{
				LineInfo = lineInfo;
			}

			public AttributeInfo(string name, string value, IXmlLineInfo lineInfo) : this(name, value)
			{
				LineInfo = new LineInfo(lineInfo);
			}

			/// <summary>
			/// Name of the attribute
			/// </summary>
			public string Name;

			/// <summary>
			/// Value of the attribute
			/// </summary>
			public string Value;

			/// <summary>
			/// Provided line information
			/// </summary>
			public LineInfo LineInfo;
		}

		internal struct StartTagInfo
		{
			public string Name;
			public string Namespace;
			public bool? PreserveWhitespace;
			public XamlTypeName TypeName;
			public List<MemberInfo> Members;
			public List<AttributeInfo> Attributes;
		}

		internal class NamespaceResolver : IXamlNamespaceResolver
		{
			IXmlNamespaceResolver source;

			public NamespaceResolver(IXmlNamespaceResolver source)
			{
				this.source = source;
			}

			public string GetNamespace(string prefix)
			{
				return source.LookupNamespace(prefix);
			}

			public IEnumerable<NamespaceDeclaration> GetNamespacePrefixes()
			{
				foreach (var p in source.GetNamespacesInScope(XmlNamespaceScope.All))
					yield return new NamespaceDeclaration(p.Value, p.Key);
			}
		}
	}
}