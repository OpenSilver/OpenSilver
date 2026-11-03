using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Xaml
{
    [DebuggerDisplay("Prefix={Prefix} Namespace={Namespace}")]
    public class NamespaceDeclaration
	{
		public NamespaceDeclaration(string ns, string prefix)
		{
			// null arguments are allowed.
			Namespace = ns;
			Prefix = prefix;
		}

		public string Namespace { get; private set; }
		public string Prefix { get; private set; }
	}
}
