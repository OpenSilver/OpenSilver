using System.IO;
using System.Security;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
    public sealed partial class SaveFileDialog
    {
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
        public string SafeFileName
        {
            get;
            private set;
        }

        // Summary:
        //     Gets or sets the default file name extension applied to files that are saved
        //     with the System.Windows.Controls.SaveFileDialog.
        //
        // Returns:
        //     The default file name extension applied to files that are saved with the System.Windows.Controls.SaveFileDialog,
        //     which can optionally include the dot character (.).
        [OpenSilver.NotImplemented]
        public string DefaultExt { get; set; }

        //
        // Summary:
        //     Gets or sets the file name used if a file name is not specified by the user.
        //
        // Returns:
        //     The file name used if a file name is not specified by the user.System.String.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Occurs if the specified file name is null or contains invalid characters such
        //     as quotes ("), less than (<), greater than (>), pipe (|), backspace (\b), null
        //     (\0), tab (\t), colon (:), asterisk(*), question mark (?), and slashes (\\, /).
        [OpenSilver.NotImplemented]
        public string DefaultFileName { get; set; }

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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public bool? ShowDialog()
        {
            return null;
        }
    }
}
