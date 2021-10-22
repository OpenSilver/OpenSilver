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
		public bool Multiselect
		{
			get;
			set;
		}

        [OpenSilver.NotImplemented]
		public string Filter
		{
			get;
			set;
		}

        [OpenSilver.NotImplemented]
		public int FilterIndex
		{
			get;
			set;
		}

        [OpenSilver.NotImplemented]
		public FileInfo File
		{
			get;
			private set;
		}

        [OpenSilver.NotImplemented]
		public IEnumerable<FileInfo> Files
		{
			get;
			private set;
		}

        [OpenSilver.NotImplemented]
		public bool? ShowDialog()
		{
			return null;
		}

        [OpenSilver.NotImplemented]
		public bool? ShowDialog(Window owner)
		{
			return null;
		}
	}
}
