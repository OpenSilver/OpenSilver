

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


using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.System;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class AutoCompleteBox : Selector
    {
        Popup _popup;
        ToggleButton _dropDownToggle;
        TextBox _textBox;
        DispatcherTimer _timer; // timer used to manage the MinimumPopulateDelay property.

        private AutoCompleteFilterMode _filterMode; // filter mode
        private AutoCompleteFilterPredicate<object> _filter; // currently selected filter

        private bool _updatingTextOnSelection;

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBox class.
        /// </summary>
        public AutoCompleteBox()
        {
            this.DefaultStyleKey = typeof(AutoCompleteBox);
            this.FilterMode = AutoCompleteFilterMode.StartsWith;
            this._timer = new DispatcherTimer();
            this._timer.Tick += (o, e) =>
            {
                this._timer.Stop();
                this.UpdateItemsVisibilityOnTextChanged();
            };
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._updatingTextOnSelection)
            {
                return;
            }

            if (this._textBox == null ||
                this._textBox.Text.Length < this.MinimumPrefixLength)
            {
                this._timer.Stop();
                this.SetCurrentValue(SelectedIndexProperty, -1);
                this.IsDropDownOpen = false;

                return;
            }

            if (this._timer.IsEnabled)
            {
                // if the timer has already started, we need
                // to reset it.
                this._timer.Stop();
            }
            this._timer.Start();
        }

        private void UpdateItemsVisibilityOnTextChanged()
        {
            // Opening DropDown will instantiate and assign ItemsHost
            this.IsDropDownOpen = true;

            if (this.HasItems)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ComboBoxItem container = this.ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItem;
                    
                    if (container == null)
                    {
                        continue;
                    }

                    if (this._filter(this._textBox.Text, container.Content))
                    {
                        container.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        container.Visibility = Visibility.Collapsed;
                        if (this.SelectedIndex == i)
                        {
                            this.SetCurrentValue(SelectedIndexProperty, -1);
                        }
                    }
                }
            }
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild("Popup") as Popup;
            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            _textBox = GetTemplateChild("Text") as TextBox;
            if (_textBox == null)
            {
                // Note: Silverlight's expected template part is "Text". But since we named it 
                // "PART_TextBox", we keep this for backward compatibility.
                _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            }

            if (_dropDownToggle != null)
            {
                _dropDownToggle.Checked += DropDownToggle_Checked;
                _dropDownToggle.Unchecked += DropDownToggle_Unchecked;
            }

            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextBoxTextChanged;
#if MIGRATION
                _textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
#else
                _textBox.PointerPressed += TextBox_PointerPressed;
#endif
            }

            this.UpdateToggleButton(this.IsArrowVisible);

            this.ApplySelectedIndex(this.SelectedIndex);
        }


        void DropDownToggle_Checked(object sender, RoutedEventArgs e)
        {
            //Open the pop up
            IsDropDownOpen = true;
        }

        void DropDownToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            //Close the pop up
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the drop-down portion of the
        /// ComboBox is currently open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        /// <summary>
        /// Identifies the IsDropDownOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", 
                                        typeof(bool), 
                                        typeof(AutoCompleteBox), 
                                        new PropertyMetadata(false, OnIsDropDownOpenChanged));

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(autoCompleteBox))
            {
                bool isDropDownOpen = (bool)e.NewValue;

                if (isDropDownOpen)
                {
                    //-----------------------------
                    // Show the Popup
                    //-----------------------------

                    RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(
                        IsDropDownOpenProperty, (bool)e.OldValue, true, true);
                    autoCompleteBox.OnDropDownOpening(args);

                    // Show the popup:
                    if (autoCompleteBox._popup != null)
                    {
                        autoCompleteBox._popup.IsOpen = true;

                        // Make sure the Width of the popup is the same as the popup:
                        double actualWidth = autoCompleteBox._popup.ActualWidth;
                        if (!double.IsNaN(actualWidth) && autoCompleteBox._popup.Child is FrameworkElement)
                            ((FrameworkElement)autoCompleteBox._popup.Child).Width = actualWidth;
                    }

                    // Reflect the state of the popup on the toggle button:
                    if (autoCompleteBox._dropDownToggle != null)
                    {
                        if (autoCompleteBox._popup.IsOpen)
                        {
                            if (autoCompleteBox._dropDownToggle.IsChecked == false) //to prevent possible infinite loop
                                autoCompleteBox._dropDownToggle.IsChecked = true;
                        }
                        else
                        {
                            if (autoCompleteBox._dropDownToggle.IsChecked == true) //to prevent possible infinite loop
                                autoCompleteBox._dropDownToggle.IsChecked = false;
                        }
                    }

                    // Raise the Opened event if we opened the popup:
                    if (autoCompleteBox._popup.IsOpen)
                    {
#if MIGRATION
                        autoCompleteBox.OnDropDownOpened(new EventArgs());
#else
                        autoCompleteBox.OnDropDownOpened(new RoutedEventArgs());
#endif
                    }
                }
                else
                {
                    //-----------------------------
                    // Hide the Popup
                    //-----------------------------

                    RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(
                        IsDropDownOpenProperty, (bool)e.OldValue, false, true);
                    autoCompleteBox.OnDropDownClosing(args);

                    // Close the popup:
                    if (autoCompleteBox._popup != null)
                        autoCompleteBox._popup.IsOpen = false;

                    // Ensure that the toggle button is unchecked:
                    if (autoCompleteBox._dropDownToggle != null
                        && autoCompleteBox._dropDownToggle.IsChecked == true)
                    {
                        autoCompleteBox._dropDownToggle.IsChecked = false;
                    }

                    if (autoCompleteBox._textBox != null)
                    {
                        autoCompleteBox._updatingTextOnSelection = true;

                        try
                        {
                            autoCompleteBox._textBox.Text = (autoCompleteBox.SelectedItem ?? string.Empty).ToString();
                        }
                        finally
                        {
                            autoCompleteBox._updatingTextOnSelection = false;
                        }
                    }

                    // Raise the Closed event:
#if MIGRATION
                    autoCompleteBox.OnDropDownClosed(new EventArgs());
#else
                    autoCompleteBox.OnDropDownClosed(new RoutedEventArgs());
#endif
                }
            }
        }

        /// <summary>
        /// The IsArrowVisible property is true when it's an AutoCompleteComboBox and false if it is an AutoCompleteBox
        /// </summary>
        public bool IsArrowVisible
        {
            get { return (bool)GetValue(IsArrowVisibleProperty); }
            set { SetValue(IsArrowVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsArrowVisibleProperty =
            DependencyProperty.Register("IsArrowVisible", 
                                        typeof(bool), 
                                        typeof(AutoCompleteBox), 
                                        new PropertyMetadata(false, OnIsArrowVisibleChanged));

        private static void OnIsArrowVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoCompleteBox)d).UpdateToggleButton((bool)e.NewValue);
        }

        private void UpdateToggleButton(bool isVisible)
        {
            if (this._textBox != null && this._dropDownToggle != null)
            {
                if (isVisible)
                {
                    //-----------------------------
                    // Show the toggle button
                    //-----------------------------
                    this._textBox.Margin = new Thickness(0, 0, 30, 0);
                    this._dropDownToggle.Visibility = Visibility.Visible;
                }
                else
                {
                    //-----------------------------
                    // Hide the toggle button
                    //-----------------------------
                    this._textBox.Margin = new Thickness(0, 0, 0, 0);
                    this._dropDownToggle.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum delay, in milliseconds, after text is typed in the text box before the AutoCompleteBox control populates the list of possible matches in the drop-down.
        /// </summary>
        public int MinimumPopulateDelay
        {
            get { return (int)GetValue(MinimumPopulateDelayProperty); }
            set { SetValue(MinimumPopulateDelayProperty, value); }
        }

        public static readonly DependencyProperty MinimumPopulateDelayProperty =
            DependencyProperty.Register("MinimumPopulateDelay", 
                                        typeof(int), 
                                        typeof(AutoCompleteBox), 
                                        new PropertyMetadata(0, OnMinimumPopulateDelayChanged));

        private static void OnMinimumPopulateDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)d;
            autoCompleteBox._timer.Interval = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the minimum number of characters required to be entered in the text box before the AutoCompleteBox displays possible matches.
        /// </summary>
        public int MinimumPrefixLength
        {
            get { return (int)GetValue(MinimumPrefixLengthProperty); }
            set { SetValue(MinimumPrefixLengthProperty, value); }
        }

        public static readonly DependencyProperty MinimumPrefixLengthProperty =
            DependencyProperty.Register("MinimumPrefixLength", 
                                        typeof(int), 
                                        typeof(AutoCompleteBox), 
                                        new PropertyMetadata(1));

        /// <summary>
        /// Gets the text that is used to filter items in the ItemsSource item collection.
        /// </summary>
        public string SearchText
        {
            get
            {
                return _textBox.Text;
            }
        }

        /// <summary>
        /// Gets or sets the text in the text box portion of the AutoCompleteBox control.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", 
                                        typeof(string), 
                                        typeof(AutoCompleteBox),
                                        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)d;

            autoCompleteBox.OnTextChanged(new RoutedEventArgs());
        }

        protected virtual void OnTextChanged(RoutedEventArgs e)
        {
            if (!_updatingTextOnSelection)
            {
                if (TextChanged != null)
                    TextChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets how the text in the text box is used to filter items specified
        /// by the AutoCompleteBox.ItemsSource property for display
        /// in the drop-down.
        /// </summary>
        public AutoCompleteFilterMode FilterMode
        {
            get { return _filterMode; }
            set
            {
                SelectFilter(value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ItemFilter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemFilterProperty = 
            DependencyProperty.Register(
                nameof(ItemFilter), 
                typeof(AutoCompleteFilterPredicate<object>), 
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnItemFilterPropertyChanged));

        /// <summary>
        /// The custom method that uses the user-entered text to filter the items specified by the ItemsSource property.
        /// </summary>
        public AutoCompleteFilterPredicate<object> ItemFilter
        {
            get { return (AutoCompleteFilterPredicate<object>)GetValue(ItemFilterProperty); }
            set { SetValue(ItemFilterProperty, value); }
        }

        private static void OnItemFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = (AutoCompleteBox)d;
            AutoCompleteFilterPredicate<object> value = (AutoCompleteFilterPredicate<object>)e.NewValue;

            if (value == null)
            {
                source.FilterMode = AutoCompleteFilterMode.None;
            }
            else
            {
                source.FilterMode = AutoCompleteFilterMode.Custom;
            }
        }

        #region Filters

        private void SelectFilter(AutoCompleteFilterMode mode)
        {
            switch (mode)
            {
                case AutoCompleteFilterMode.None:
                    this._filterMode = AutoCompleteFilterMode.None;
                    this._filter = None;
                    break;
                case AutoCompleteFilterMode.StartsWith:
                    this._filterMode = AutoCompleteFilterMode.StartsWith;
                    this._filter = StartsWith;
                    break;
                case AutoCompleteFilterMode.StartsWithCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.StartsWithCaseSensitive;
                    this._filter = StartsWithCaseSensitive;
                    break;
                case AutoCompleteFilterMode.StartsWithOrdinal:
                    this._filterMode = AutoCompleteFilterMode.StartsWithOrdinal;
                    this._filter = StartsWithOrdinal;
                    break;
                case AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive;
                    this._filter = StartsWithOrdinalCaseSensitive;
                    break;
                case AutoCompleteFilterMode.Contains:
                    this._filterMode = AutoCompleteFilterMode.Contains;
                    this._filter = Contains;
                    break;
                case AutoCompleteFilterMode.ContainsCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.ContainsCaseSensitive;
                    this._filter = ContainsCaseSensitive;
                    break;
                case AutoCompleteFilterMode.ContainsOrdinal:
                    this._filterMode = AutoCompleteFilterMode.ContainsOrdinal;
                    this._filter = ContainsOrdinal;
                    break;
                case AutoCompleteFilterMode.ContainsOrdinalCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.ContainsOrdinalCaseSensitive;
                    this._filter = ContainsOrdinalCaseSensitive;
                    break;
                case AutoCompleteFilterMode.Equals:
                    this._filterMode = AutoCompleteFilterMode.Equals;
                    this._filter = Equals;
                    break;
                case AutoCompleteFilterMode.EqualsCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.EqualsCaseSensitive;
                    this._filter = EqualsCaseSensitive;
                    break;
                case AutoCompleteFilterMode.EqualsOrdinal:
                    this._filterMode = AutoCompleteFilterMode.EqualsOrdinal;
                    this._filter = EqualsOrdinal;
                    break;
                case AutoCompleteFilterMode.EqualsOrdinalCaseSensitive:
                    this._filterMode = AutoCompleteFilterMode.EqualsOrdinalCaseSensitive;
                    this._filter = EqualsOrdinalCaseSensitive;
                    break;
                case AutoCompleteFilterMode.Custom:
                    this._filterMode = AutoCompleteFilterMode.Custom;
                    this._filter = Custom;
                    break;
                default:
                    throw new ArgumentException("Filters can't be combined.");
            }
        }

        // filter for AutoCompleteFilterMode.None
        private static bool None(string search, object item)
        {
            return true;
        }

        // filter for AutoCompleteFilterMode.StartsWith
        private static bool StartsWith(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().StartsWith(search, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.StartsWithCaseSensitive
        private static bool StartsWithCaseSensitive(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().StartsWith(search, StringComparison.CurrentCulture);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.StartsWithOrdinal
        private static bool StartsWithOrdinal(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().StartsWith(search, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive
        private static bool StartsWithOrdinalCaseSensitive(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().StartsWith(search, StringComparison.Ordinal);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.Contains
        private static bool Contains(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().ToLower().Contains(search.ToLower());
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.ContainsCaseSensitive
        private static bool ContainsCaseSensitive(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().Contains(search);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.ContainsOrdinal
        private static bool ContainsOrdinal(string search, object item)
        {
            return Contains(search, item);
        }

        // filter for AutoCompleteFilterMode.ContainsOrdinalCaseSensitive
        private static bool ContainsOrdinalCaseSensitive(string search, object item)
        {
            return ContainsCaseSensitive(search, item);
        }

        // filter for AutoCompleteFilterMode.Equals
        private static bool Equals(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().Equals(search, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.EqualsCaseSensitive
        private static bool EqualsCaseSensitive(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().Equals(search, StringComparison.CurrentCulture);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.EqualsOrdinal
        private static bool EqualsOrdinal(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().Equals(search, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.EqualsOrdinalCaseSensitive
        private static bool EqualsOrdinalCaseSensitive(string search, object item)
        {
            if (item != null && search != null)
            {
                return item.ToString().Equals(search, StringComparison.Ordinal);
            }
            else
            {
                return false;
            }
        }

        // filter for AutoCompleteFilterMode.Custom
        private bool Custom(string search, object item)
        {
            if (item != null && search != null)
            {
                return this.ItemFilter(search, item);
            }
            else
            {
                return false;
            }
        }

        #endregion Filters

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ComboBoxItem container = element as ComboBoxItem;
            if (container != null)
            {
#if MIGRATION
                container.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(ComboBoxItem_Click), true);
#else
                container.AddHandler(PointerPressedEvent, new PointerEventHandler(ComboBoxItem_Click), true);
#endif
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            ComboBoxItem container = element as ComboBoxItem;
            if (container != null)
            {
#if MIGRATION
                container.RemoveHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(ComboBoxItem_Click));
#else
                container.RemoveHandler(PointerPressedEvent, new PointerEventHandler(ComboBoxItem_Click));
#endif
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ComboBoxItem);
        }

        [Obsolete]
        protected override SelectorItem INTERNAL_GenerateContainer(object item)
        {
            return (SelectorItem)this.GetContainerFromItem(item);
        }

        [Obsolete]
        protected override DependencyObject GetContainerFromItem(object item)
        {
            ComboBoxItem comboBoxItem = item as ComboBoxItem ?? new ComboBoxItem();
            return comboBoxItem;
        }

        void ComboBoxItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedContainer = (SelectorItem)sender;

            // Updates the text but without triggering the text change event
            _updatingTextOnSelection = true;
            try
            {
                this.Text = selectedContainer.ToString();
            }
            finally
            {
                _updatingTextOnSelection = false;
            }

            // Select the item:
            this.SelectedItem = ItemContainerGenerator.ItemFromContainer(selectedContainer);

            // Close the popup:
            if (_dropDownToggle != null)
                _dropDownToggle.IsChecked = false; // Note: this has other effects as well: see the "IsDropDownOpen_Changed" method.
        }

#if MIGRATION
        void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void TextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            //if we click on the textbox

            //todo: remove this when closing the dropdown on click away is implemented

            if (IsDropDownOpen)
            {
                //Close the popup if it was open
                IsDropDownOpen = false;
            }
        }



        /// <summary>
        /// Invoked when the DropDownClosed event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
#if MIGRATION
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            DropDownClosed?.Invoke(this, new RoutedPropertyChangedEventArgs<bool>(true, false));
        }

        protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            DropDownClosed?.Invoke(this, e);
        }
#else
        protected virtual void OnDropDownClosed(RoutedEventArgs e)
        {
            if (DropDownClosed != null)
                DropDownClosed(this, e);
        }
#endif

        /// <summary>
        /// Invoked when the DropDownOpened event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
#if MIGRATION
        protected virtual void OnDropDownOpened(EventArgs e)
#else
        protected virtual void OnDropDownOpened(RoutedEventArgs e)
#endif
        {
            if (DropDownOpened != null)
                DropDownOpened(this, e);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Controls.AutoCompleteBox.DropDownClosing" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.Controls.RoutedPropertyChangingEventArgs`1" /> that contains the event data.</param>
#if MIGRATION
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
#else
        protected virtual void OnDropDownClosing(RoutedEventArgs e)
#endif
        {
            if (DropDownClosing != null)
            {
                DropDownClosing(this, e);
            }
        }

        /// <summary>
        /// Raises the System.Windows.Controls.AutoCompleteBox.DropDownOpening event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RoutedPropertyChangingEventArgs{T}"/> that contains
        /// the event data.
        /// </param>
#if MIGRATION
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
#else
        protected virtual void OnDropDownOpening(RoutedEventArgs e)
#endif
        {
            if (DropDownOpening != null)
            {
                DropDownOpening(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the maximum height for a combo box drop-down.
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxDropDownHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", 
                                        typeof(double), 
                                        typeof(AutoCompleteBox), 
                                        new PropertyMetadata(200d));

#region event

        /// <summary>
        /// Occurs when the text in the text box portion of the AutoCompleteBox changes.
        /// </summary>
        public event RoutedEventHandler TextChanged;

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox closes.
        /// </summary>
#if MIGRATION
        public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;
#else
        public event RoutedEventHandler DropDownClosed;
#endif

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox opens.
        /// </summary>
#if MIGRATION
        public event EventHandler DropDownOpened;
#else
        public event RoutedEventHandler DropDownOpened;
#endif

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox is closing.
        /// </summary>
#if MIGRATION
        public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;
#else
        public event RoutedEventHandler DropDownClosing;
#endif

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox is opening.
        /// </summary>
#if MIGRATION
        public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;
#else
        public event RoutedEventHandler DropDownOpening;
#endif

#endregion
    }

    /// <summary>
    /// Specifies how text in the text box portion of the AutoCompleteBox
    /// control is used to filter items specified by the AutoCompleteBox.ItemsSource
    /// property for display in the drop-down.
    /// </summary>
    public enum AutoCompleteFilterMode
    {
        /// <summary>
        /// Specifies that no filter is used. All items are returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the returned items
        /// start with the specified text. The filter uses the <see cref="String.StartsWith(string, StringComparison)"/>
        /// method.
        /// </summary>
        StartsWith = 1,
        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the returned items
        /// start with the specified text. The filter uses the <see cref="String.StartsWith(string, StringComparison)"/>
        /// method.
        /// </summary>
        StartsWithCaseSensitive = 2,
        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned items start
        /// with the specified text. The filter uses the <see cref="String.StartsWith(string, StringComparison)"/>
        /// method.
        /// </summary>
        StartsWithOrdinal = 3,
        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items start with
        /// the specified text. The filter uses the <see cref="String.StartsWith(string, StringComparison)"/>
        /// method.
        /// </summary>
        StartsWithOrdinalCaseSensitive = 4,
        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the returned items
        /// contain the specified text.
        /// </summary>
        Contains = 5,
        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the returned items
        /// contain the specified text.
        /// </summary>
        ContainsCaseSensitive = 6,
        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned items contain
        /// the specified text.
        /// </summary>
        ContainsOrdinal = 7,
        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items contain
        /// the specified text.
        /// </summary>
        ContainsOrdinalCaseSensitive = 8,
        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the returned items
        /// equal the specified text. The filter uses the <see cref="String.Equals(string, StringComparison)"/>
        /// method.
        /// </summary>
        Equals = 9,
        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the returned items
        /// equal the specified text. The filter uses the <see cref="String.Equals(string, StringComparison)"/>
        /// method.
        /// </summary>
        EqualsCaseSensitive = 10,
        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned items equal
        /// the specified text. The filter uses the <see cref="String.Equals(string, StringComparison)"/>
        /// method.
        /// </summary>
        EqualsOrdinal = 11,
        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items equal the
        /// specified text. The filter uses the <see cref="String.Equals(string, StringComparison)"/>
        /// method.
        /// </summary>
        EqualsOrdinalCaseSensitive = 12,
        /// <summary>
        /// Specifies that a custom filter is used. This mode is used when the <see cref="AutoCompleteBox.TextFilter"/>
        /// or <see cref="AutoCompleteBox.ItemFilter"/> properties are set.
        /// </summary>
        Custom = 13
    }
}
