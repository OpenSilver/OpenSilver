#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class AutoCompleteBox
	{
		
		//
		// Summary:
		//     Gets or sets a value that indicates whether the first possible match found during
		//     the filtering process will be displayed automatically in the text box.
		//
		// Returns:
		//     true if the first possible match found will be displayed automatically in the
		//     text box; otherwise, false. The default is false.
		public bool IsTextCompletionEnabled { get; set; }

        /// <summary>
        /// Notifies the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> that
        /// the <see cref="P:System.Windows.Controls.AutoCompleteBox.ItemsSource" /> property has been set
        /// and the data can be filtered to provide possible matches in the drop-down.
        /// </summary>
        public void PopulateComplete()
        {
            throw new NotImplementedException();
        }
    }
}

#endif