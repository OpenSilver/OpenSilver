using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml
{
    internal class NamespaceDeclaration
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
