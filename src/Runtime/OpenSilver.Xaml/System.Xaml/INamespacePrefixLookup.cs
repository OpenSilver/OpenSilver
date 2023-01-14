using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml
{
    internal interface INamespacePrefixLookup
	{
		string LookupPrefix(string ns);
	}
}
