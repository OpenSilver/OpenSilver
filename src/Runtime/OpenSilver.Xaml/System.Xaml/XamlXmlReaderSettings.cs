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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Xml;

namespace System.Xaml
{
    internal class XamlXmlReaderSettings : XamlReaderSettings
	{
		Dictionary<string, string> _defaultNamespaces;

		public XamlXmlReaderSettings()
		{
		}

		public XamlXmlReaderSettings(XamlXmlReaderSettings settings)
			: base(settings)
		{
			var s = settings;
			if (s == null)
				return;
			CloseInput = s.CloseInput;
			SkipXmlCompatibilityProcessing = s.SkipXmlCompatibilityProcessing;
			XmlLang = s.XmlLang;
			XmlSpacePreserve = s.XmlSpacePreserve;
			if (s._defaultNamespaces != null)
				_defaultNamespaces = new Dictionary<string, string>(s._defaultNamespaces);
		}

		public bool CloseInput { get; set; }
		public bool SkipXmlCompatibilityProcessing { get; set; }
		public string XmlLang { get; set; }
		public bool XmlSpacePreserve { get; set; }

		/// <summary>
		/// Adds a default namespace to read the xaml as a fragment.
		/// </summary>
		/// <param name="prefix">Prefix of namespace, or null for the default namespace</param>
		/// <param name="xmlNamespace">Uri or clr-namespace to set as default</param>
		[EnhancedXaml]
		public void AddNamespace(string prefix, string xmlNamespace)
		{
			if (_defaultNamespaces == null)
				_defaultNamespaces = new Dictionary<string, string>();
			_defaultNamespaces[prefix ?? string.Empty] = xmlNamespace;
		}

		/// <summary>
		/// Adds all namespace prefixes defined in the assembly of the specified type as default using <see cref="XmlnsPrefixAttribute"/>
		/// </summary>
		/// <param name="type">Type of the assembly to lookup default prefixes for</param>
		[EnhancedXaml]
		public void AddNamespaces(Type type) => AddNamespaces(type.GetTypeInfo().Assembly);

		/// <summary>
		/// Adds all namespace prefixes defined in the assembly as default using <see cref="XmlnsPrefixAttribute"/>
		/// </summary>
		/// <param name="assembly">Assembly to lookup default prefixes for</param>
		[EnhancedXaml]
		public void AddNamespaces(Assembly assembly)
		{
			var prefixes = assembly.GetCustomAttributes<XmlnsPrefixAttribute>();
			foreach (var prefix in prefixes)
			{
				AddNamespace(prefix.Prefix, prefix.XmlNamespace);
			}
		}

		public IEnumerable<KeyValuePair<string, string>> DefaultNamespaces => _defaultNamespaces ?? Enumerable.Empty<KeyValuePair<string, string>>();

		internal bool RequiresXmlContext => _defaultNamespaces?.Count > 0;

		internal XmlParserContext CreateXmlContext()
		{
			var nt = new NameTable();
			var nsmgr = new XmlNamespaceManager(nt);

			if (_defaultNamespaces != null)
			{
				foreach (var ns in _defaultNamespaces)
					nsmgr.AddNamespace(ns.Key, ns.Value);
			}

			return new XmlParserContext(null, nsmgr, null, XmlSpace.None);
		}
	}
}
