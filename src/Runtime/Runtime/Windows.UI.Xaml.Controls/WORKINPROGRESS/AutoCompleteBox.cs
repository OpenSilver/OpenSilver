#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the method that will handle the <see cref="E:System.Windows.Controls.AutoCompleteBox.Populating" /> event of a <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control.
    /// </summary>
    /// <param name="sender">The source of the event. </param>
    /// <param name="e">A <see cref="T:System.Windows.Controls.PopulatingEventArgs" /> that contains the event data.</param>
    public delegate void PopulatingEventHandler(object sender, PopulatingEventArgs e);

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

        /// <summary>
        /// Occurs when the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> is populating the drop-down with possible matches based on the <see cref="P:System.Windows.Controls.AutoCompleteBox.Text" /> property.
        /// </summary>
        public event PopulatingEventHandler Populating;

        /// <summary>
        /// Gets or sets the property path that is used to get the value for display in the text box portion of the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control, and to filter items for display in the drop-down.
        /// </summary>
        /// <returns>
        /// The property path that is used to get values for display in the text box portion of the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control, and to filter items for display in the drop-down.
        /// </returns>
        public string ValueMemberPath
        {
            get; set;
        }
    }
}

#endif