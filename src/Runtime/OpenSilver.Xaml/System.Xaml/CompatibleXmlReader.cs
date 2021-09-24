using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Xaml
{
	internal class CompatibleXmlReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		class Scope
		{
			public List<string> Ignore { get; set; }
			public int Depth { get; set; }
		}
		private readonly XmlReader _base;
		private readonly Dictionary<string, string> _nsmap = new Dictionary<string, string>();
		private readonly HashSet<string> _knownNamespaces = new HashSet<string>();
		public delegate bool TryGetCompatibleNamespaceDelegate(string ns, out string compatible);
		private readonly TryGetCompatibleNamespaceDelegate _getCompatible;
		private Stack<Scope> _scopeStack = new Stack<Scope>();
		private Scope _scope = new Scope { Ignore = new List<string>() };
		private bool _previousWasEmpty;
		private int _ignoredAttributes;

		private bool _savedPositionWasAttribute;
		private string _savedPositionAttributeName;
		private IXmlLineInfo _baseLineInfo;
		private IXmlNamespaceResolver _baseNamespaceResolver;

		public CompatibleXmlReader(XmlReader baseReader, XamlSchemaContext context)
		{
			_base = baseReader;
			_baseLineInfo = _base as IXmlLineInfo;
			_baseNamespaceResolver = _base as IXmlNamespaceResolver;
			_getCompatible = context.TryGetCompatibleXamlNamespace;
		}

		public CompatibleXmlReader(XmlReader baseReader, TryGetCompatibleNamespaceDelegate compatible)
		{
			_base = baseReader;
			_getCompatible = compatible;
		}

		private static readonly char[] Splitter = new[] { ' ' };
		void PushScope(string prefixes)
		{
			var newIgnore = new HashSet<string>(_scope.Ignore);
			foreach (var prefix in prefixes.Split(Splitter, StringSplitOptions.RemoveEmptyEntries))
				newIgnore.Add(LookupNamespace(prefix));
			var newScope = new Scope
			{
				Depth = Depth,
				Ignore = newIgnore.ToList()
			};
			_scopeStack.Push(_scope);
			_scope = newScope;
		}

		void PopScopeIfNeeded()
		{
			if (_scope.Depth == Depth && Depth != 0)
				_scope = _scopeStack.Pop();
		}

		bool ShouldIgnore(string ns)
		{
			if (ns == "http://schemas.openxmlformats.org/markup-compatibility/2006")
				return true;
			if (_knownNamespaces.Contains(ns))
				return false;
			return _scope.Ignore.Contains(ns);
		}


		string GetMapped(string ns)
		{
			if (_nsmap.TryGetValue(ns, out var rv))
				return rv;
			if (_getCompatible(ns, out var mapped))
				_knownNamespaces.Add(mapped);
			else
				mapped = ns;
			return _nsmap[ns] = mapped;
		}

		public override bool Read()
		{
			while (_base.Read())
			{
				if (_base.NodeType == XmlNodeType.Element)
				{
					if (_previousWasEmpty)
						PopScopeIfNeeded();
					_previousWasEmpty = _base.IsEmptyElement;
					if (ProcessStartElement())
						continue;
					return true;
				}
				if (_base.NodeType == XmlNodeType.EndElement)
				{
					PopScopeIfNeeded();
					return true;
				}
				return true;
			}
			return false;
		}


		// Returns TRUE if element WAS skipped
		bool ProcessStartElement()
		{
			if (ShouldIgnore(NamespaceURI))
			{
				if (IsEmptyElement)
					return true;
				_base.Skip();
				return true;
			}
			_ignoredAttributes = 0;
			if (_base.HasAttributes)
			{
				var ignorable = _base.GetAttribute("Ignorable", "http://schemas.openxmlformats.org/markup-compatibility/2006");
				if (ignorable != null)
					PushScope(ignorable);
				_base.MoveToFirstAttribute();
				do
				{
					if (ShouldIgnore(NamespaceURI))
						_ignoredAttributes++;

				} while (_base.MoveToNextAttribute());
			}
			_base.MoveToElement();
			return false;
		}


		#region Attribute handling

		public override bool MoveToFirstAttribute()
		{
			if (_ignoredAttributes == 0)
				return _base.MoveToFirstAttribute();
			if (!HasAttributes)
				return false;
			if (!_base.MoveToFirstAttribute())
				return false;
			while (ShouldIgnore(NamespaceURI))
			{
				if (!_base.MoveToNextAttribute())
					return false;
			}
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (_ignoredAttributes == 0)
				return _base.MoveToNextAttribute();
			do
			{
				if (!_base.MoveToNextAttribute())
					return false;
			} while (ShouldIgnore(NamespaceURI));
			return true;
		}

		void SavePosition()
		{
			_savedPositionWasAttribute = NodeType == XmlNodeType.Attribute;
			_savedPositionAttributeName = _base.Name;
		}

		void RestorePosition()
		{
			if (_savedPositionWasAttribute)
				_base.MoveToAttribute(_savedPositionAttributeName);
			else
				_base.MoveToElement();
		}

		void MoveTo(int i)
		{
			if (i < 0 || i >= AttributeCount)
				throw new ArgumentException();
			MoveToElement();
			MoveToFirstAttribute();
			for (var c = 0; c < i; c++)
				MoveToNextAttribute();
		}

		public override string GetAttribute(int i)
		{
			if (_ignoredAttributes == 0)
				return _base.GetAttribute(i);
			if (i > AttributeCount)
				throw new ArgumentException();
			SavePosition();
			MoveTo(i);
			var res = Value;
			RestorePosition();
			return res;
		}

		public override string GetAttribute(string name) => _base.GetAttribute(name);

		public override string GetAttribute(string name, string namespaceURI)
		{
			if (!HasAttributes)
				return null;
			SavePosition();
			string res = null;
			MoveToFirstAttribute();
			do
			{
				if (LocalName == name && NamespaceURI == namespaceURI)
				{
					res = Value;
					break;
				}
			} while (MoveToNextAttribute());

			RestorePosition();
			return res;
		}

		public override bool MoveToAttribute(string name)
		{
			return _base.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			if (!HasAttributes)
				return false;
			MoveToFirstAttribute();
			do
			{
				if (LocalName == name && NamespaceURI == ns)
				{
					return true;
				}
			} while (MoveToNextAttribute());
			return false;
		}

		public override int AttributeCount => _base.AttributeCount - _ignoredAttributes;
		#endregion


		#region Wrappers
		public override string NamespaceURI => GetMapped(_base.NamespaceURI);

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope) =>
			_baseNamespaceResolver?.GetNamespacesInScope(scope);
		public override string LookupNamespace(string prefix) => GetMapped(_base.LookupNamespace(prefix));

		public string LookupPrefix(string namespaceName) =>
			_baseNamespaceResolver?.LookupPrefix(
				_nsmap.FirstOrDefault(x => x.Value == namespaceName).Value ?? namespaceName);

		public override string Value
		{
			get
			{
				if (_base.LocalName == "xmlns")
				{
					return LookupNamespace(string.Empty);
				}
				if (_base.Prefix == "xmlns")
				{
					return LookupNamespace(_base.LocalName);
				}
				return _base.Value;

			}
		}

		#endregion


		#region Simple wrappers
		public override bool ReadAttributeValue() => _base.ReadAttributeValue();
		public override bool MoveToElement() => _base.MoveToElement();
		public override void ResolveEntity() => _base.ResolveEntity();

		public override string BaseURI => _base.BaseURI;

		public override int Depth => _base.Depth;
		public override bool EOF => _base.EOF;

		public override bool IsEmptyElement => _base.IsEmptyElement;
		public override string LocalName => _base.LocalName;

		public override XmlNameTable NameTable => _base.NameTable;

		public override XmlNodeType NodeType => _base.NodeType;

		public override string Prefix => _base.Prefix;

		public override ReadState ReadState => _base.ReadState;

		public bool HasLineInfo() => _baseLineInfo?.HasLineInfo() ?? false;

		public int LineNumber => _baseLineInfo?.LineNumber ?? 0;
		public int LinePosition => _baseLineInfo?.LinePosition ?? 0;

		#endregion
	}
}
