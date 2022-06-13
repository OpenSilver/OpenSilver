
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.IO;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a dialog box that enables the user to select one or more files.
    /// </summary>
    [Obsolete("Use OpenSilver.Controls.OpenFileDialog instead.")]
    [OpenSilver.NotImplemented]
    public sealed class OpenFileDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileDialog"/> class.
        /// </summary>
        public OpenFileDialog() { }

        /// <summary>
        /// Gets a <see cref="FileInfo"/> object for the selected file. If multiple files are
        /// selected, returns the first selected file.
        /// </summary>
        /// <returns>
        /// The selected file. If multiple files are selected, returns the first selected
        /// file.
        /// </returns>
        public FileInfo File { get; }

        /// <summary>
        /// Gets a collection of <see cref="FileInfo"/> objects for the selected files.
        /// </summary>
        public IEnumerable<FileInfo> Files { get; }

        /// <summary>
        /// Gets or sets a filter string that specifies the file types and descriptions to
        /// display in the <see cref="OpenFileDialog"/>.
        /// </summary>
        /// <returns>
        /// A filter string that specifies the file types and descriptions to display in
        /// the <see cref="OpenFileDialog"/>. The default is <see cref="string.Empty"/>.
        /// </returns>
        public string Filter { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the index of the selected item in the <see cref="OpenFileDialog"/>
        /// filter drop-down list.
        /// </summary>
        /// <returns>
        /// The index of the selected item in the <see cref="OpenFileDialog"/>
        /// filter drop-down list. The default is 1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The filter index is less than 1.
        /// </exception>
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets the directory displayed when the dialog starts.
        /// </summary>
        /// <returns>
        /// The directory displayed when the dialog starts. The default is an empty string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The directory specified is not a valid file path.
        /// </exception>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="OpenFileDialog"/>
        /// allows users to select multiple files.
        /// </summary>
        /// <returns>
        /// true if multiple selections are allowed; otherwise, false. The default is false.
        /// </returns>
        public bool Multiselect { get; set; }

        /// <summary>
        /// Displays an System.Windows.Controls.OpenFileDialog that is modal to the Web browser
        /// or main window.
        /// </summary>
        /// <returns>
        /// true if the user clicked OK; false if the user clicked Cancel or closed the dialog box.
        /// </returns>
        public bool? ShowDialog() => null;

        /// <summary>
        /// Displays an <see cref="OpenFileDialog"/> that is modal to the specified window.
        /// </summary>
        /// <param name="owner">
        /// The window that serves as the top-level window for the dialog.
        /// </param>
        /// <returns>
        /// true if the user clicked OK; false if the user clicked Cancel or closed the dialog box.
        /// </returns>
        public bool? ShowDialog(Window owner) => null;
    }
}