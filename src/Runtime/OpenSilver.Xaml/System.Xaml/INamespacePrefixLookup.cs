using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml
{
	public interface INamespacePrefixLookup
	{
		string LookupPrefix(string ns);
	}
}
