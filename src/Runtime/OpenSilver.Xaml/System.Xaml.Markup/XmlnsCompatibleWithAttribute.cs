using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xaml.Markup
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class XmlnsCompatibleWithAttribute : Attribute
	{
		public XmlnsCompatibleWithAttribute(string oldNamespace, string newNamespace)
		{
			OldNamespace = oldNamespace;
			NewNamespace = newNamespace;
		}

		public string NewNamespace { get; private set; }
		public string OldNamespace { get; private set; }
	}
}
