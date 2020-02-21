#if WORKINPROGRESS
using System.IO;
using System.Security;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public sealed partial class SaveFileDialog
	{
		public string Filter
		{
			get;
			set;
		}

		public string SafeFileName
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Opens the file specified by the System.Windows.Controls.SaveFileDialog.SafeFileName
		//     property.
		//
		// Returns:
		//     A read-write stream for the file specified by the System.Windows.Controls.SaveFileDialog.SafeFileName
		//     property.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     No file was selected in the dialog box.
		[SecuritySafeCritical]
		public Stream OpenFile()
		{
			return null;
		}

		//
		// Summary:
		//     Displays a System.Windows.Controls.SaveFileDialog that is modal to the Web browser
		//     or main window.
		//
		// Returns:
		//     true if the user clicked Save; false if the user clicked Cancel or closed the
		//     dialog box.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     Silverlight was unable to display the dialog box due to an improperly formatted
		//     filter, an invalid filter index or other reasons.
		//
		//   T:System.Security.SecurityException:
		//     Active Scripting in Internet Explorer is disabled.-or-The call to the System.Windows.Controls.OpenFileDialog.ShowDialog
		//     method was not made from user-initiated code or too much time passed between
		//     user-initiation and the display of the dialog.
		public bool? ShowDialog()
		{
			return null;
		}
	}
}
#endif