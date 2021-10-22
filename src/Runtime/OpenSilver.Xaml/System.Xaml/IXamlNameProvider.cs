using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xaml
{
	public interface IXamlNameProvider
	{
		string GetName(object value);
	}
}
