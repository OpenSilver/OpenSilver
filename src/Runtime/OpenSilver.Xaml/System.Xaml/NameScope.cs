using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
#endif

namespace System.Xaml
{
	class NameScope : INameScope
	{
		Dictionary<string, object> table = new Dictionary<string, object>();
		// It is an external read-only namescope.
		INameScope external;

		public NameScope(INameScope external)
		{
			this.external = external;
		}

		public object FindName(string name)
		{
			object obj = external != null ? external.FindName(name) : null;
			if (obj != null)
				return obj;
			return table.TryGetValue(name, out obj) ? obj : null;
		}

		public void RegisterName(string name, object scopedElement)
		{
			table.Add(name, scopedElement);
		}

		public void UnregisterName(string name)
		{
			table.Remove(name);
		}
	}
}
