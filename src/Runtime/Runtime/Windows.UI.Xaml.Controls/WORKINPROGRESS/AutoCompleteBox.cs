#if WORKINPROGRESS

using System;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

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

#region public bool IsTextCompletionEnabled

        /// <summary>
        /// Gets or sets a value indicating whether the first possible match
        /// found during the filtering process will be displayed automatically
        /// in the text box.
        /// </summary>
        /// <value>
        /// True if the first possible match found will be displayed
        /// automatically in the text box; otherwise, false. The default is
        /// false.
        /// </value>
        public bool IsTextCompletionEnabled
        {
            get { return (bool)GetValue(IsTextCompletionEnabledProperty); }
            set { SetValue(IsTextCompletionEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AutoCompleteBox.IsTextCompletionEnabled" />
        /// dependency property.
        /// </summary>
        /// <value>The identifier for the <see cref="AutoCompleteBox.IsTextCompletionEnabled" />
        /// dependency property.</value>
        public static readonly DependencyProperty IsTextCompletionEnabledProperty =
            DependencyProperty.Register(
                "IsTextCompletionEnabled",
                typeof(bool),
                typeof(AutoCompleteBox),
                new PropertyMetadata(false, null));

#endregion public bool IsTextCompletionEnabled

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

        /// <summary>Gets or sets the <see cref="T:System.Windows.Data.Binding" /> that is used to get the value for display in the text box portion of the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control, and to filter items for display in the drop-down.</summary>
        /// <returns>The <see cref="T:System.Windows.Data.Binding" /> object used when binding to a collection property, and to filter items for display in the drop-down.</returns>
        public Binding ValueMemberBinding
        {
            get; set;
        }

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.AutoCompleteBox.SearchText" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.AutoCompleteBox.SearchText" /> dependency property.</returns>
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(string.Empty, OnSearchTextPropertyChanged));

        private static void OnSearchTextPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>Identifies the <see cref="P:System.Windows.Controls.AutoCompleteBox.TextFilter" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.AutoCompleteBox.TextFilter" /> dependency property.</returns>
        public static readonly DependencyProperty TextFilterProperty = DependencyProperty.Register(nameof(TextFilter), typeof(AutoCompleteFilterPredicate<string>), typeof(AutoCompleteBox), new PropertyMetadata(AutoCompleteSearch.GetFilter(AutoCompleteFilterMode.StartsWith)));

        /// <summary>Gets or sets the custom method that uses the user-entered text to filter items specified by the <see cref="P:System.Windows.Controls.AutoCompleteBox.ItemsSource" /> property in a text-based way for display in the drop-down.</summary>
        /// <returns>The custom method that uses the user-entered text to filter items specified by the <see cref="P:System.Windows.Controls.AutoCompleteBox.ItemsSource" /> property in a text-based way for display in the drop-down.</returns>
        public AutoCompleteFilterPredicate<string> TextFilter
        {
            get => GetValue(TextFilterProperty) as AutoCompleteFilterPredicate<string>;
            set => SetValue(TextFilterProperty, value);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Controls.AutoCompleteBox.DropDownClosing" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.Controls.RoutedPropertyChangingEventArgs`1" /> that contains the event data.</param>
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
        }

        /// <summary>
        /// Raises the System.Windows.Controls.AutoCompleteBox.DropDownOpening event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedPropertyChangingEventArgs{T}"/> that contains
        /// the event data.
        /// </param>
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
        }
    }
}

#endif