using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xaml
{
    internal interface IXamlNameProvider
	{
		string GetName(object value);
	}
}
