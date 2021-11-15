using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class OpenFileDialog
	{
		[OpenSilver.NotImplemented]
		public string InitialDirectory
		{
			get;
			set;
		}
	}
}
