#if WORKINPROGRESS
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
	public sealed partial class OpenFileDialog
	{
		public bool Multiselect
		{
			get;
			set;
		}

		public string Filter
		{
			get;
			set;
		}

		public int FilterIndex
		{
			get;
			set;
		}

		public FileInfo File
		{
			get;
			private set;
		}

		public IEnumerable<FileInfo> Files
		{
			get;
			private set;
		}

		public bool? ShowDialog()
		{
			return null;
		}

		public bool? ShowDialog(Window owner)
		{
			return null;
		}
	}
}
#endif