
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class AutoCompleteBox : Selector
    {
        Popup _popup;
        ToggleButton _dropDownToggle;
        TextBox _textBox;
        UIElement _selectedContent;
        DispatcherTimer _timer = new DispatcherTimer();
        IEnumerable _filteredItems;// _filteredItems are Items that fit the SearchText using the _itemFilter. 
        string _searchText = "";

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBox class.
        /// </summary>
        public AutoCompleteBox()
        {
            // Set default style: //todo-perfs: verify that this technique does not result in duplicate calls to the setters of the style in case the user then specifies a different style.
            this.DefaultStyleKey = typeof(AutoCompleteBox);

            //MinimumDelay is a property that set the time before opening the PopUp, so we listen to the Tick event
            _timer.Tick += _timer_Tick;

            // Prevent rendering the items as direct children to this control. Instead, we wait for the "OnApplyTemplate" method that will find where the correct place to render the items is.
            _placeWhereItemsPanelWillBeRendered = null;
        }



#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            _popup = GetTemplateChild("Popup") as Popup;
            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            var itemsPresenter = GetTemplateChild("ItemsHost") as ItemsPresenter;

            if (itemsPresenter != null)
            {
                _placeWhereItemsPanelWillBeRendered = itemsPresenter;
            }
            else
            {
                _placeWhereItemsPanelWillBeRendered = this;
            }

            if (_dropDownToggle != null)
            {
                _dropDownToggle.Checked += DropDownToggle_Checked;
                _dropDownToggle.Unchecked += DropDownToggle_Unchecked;
            }

#if MIGRATION
            _textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
#else
            _textBox.PointerPressed += TextBox_PointerPressed;
#endif
            // Update the ItemsPanel:
            UpdateItemsPanel(ItemsPanel);
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
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoCompleteBox), new PropertyMetadata(false, IsDropDownOpen_Changed));

        private static void IsDropDownOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoCompleteBox = (AutoCompleteBox)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(autoCompleteBox)
                && e.NewValue is bool)
            {
                bool isDropDownOpen = (bool)e.NewValue;

                if (isDropDownOpen)
                {
                    //-----------------------------
                    // Show the Popup
                    //-----------------------------

                    // Show the popup:
                    if (autoCompleteBox._popup != null)
                    {
                        autoCompleteBox._popup.IsOpen = true;

                        // Make sure the Width of the popup is the same as the popup:
                        double actualWidth = autoCompleteBox._popup.ActualWidth;
                        if (!double.IsNaN(actualWidth) && autoCompleteBox._popup.Child is FrameworkElement)
                            ((FrameworkElement)autoCompleteBox._popup.Child).Width = actualWidth;

                        // Draw the list (it was not drawn before because it was not in the visual tree):
                        autoCompleteBox.UpdateItemsPanel(autoCompleteBox.ItemsPanel);
                    }

                    // Close the popup if, after the call above to "UpdateItemsPanel" (which applies the filter), it appears that there are actually no elements in the popup:
#if !(BRIDGE && MIGRATION)
                    if (autoCompleteBox._filteredItems.Cast<object>().Count() == 0)
                    {
                        autoCompleteBox._popup.IsOpen = false;
                    }
#endif

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

                    // Close the popup:
                    if (autoCompleteBox._popup != null)
                        autoCompleteBox._popup.IsOpen = false;

                    // Ensure that the toggle button is unchecked:
                    if (autoCompleteBox._dropDownToggle != null
                        && autoCompleteBox._dropDownToggle.IsChecked == true)
                    {
                        autoCompleteBox._dropDownToggle.IsChecked = false;
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
            DependencyProperty.Register("IsArrowVisible", typeof(bool), typeof(AutoCompleteBox), new PropertyMetadata(false, IsArrowVisible_Changed));

        private static void IsArrowVisible_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)d;
            if (autoCompleteBox._textBox != null && autoCompleteBox._dropDownToggle != null)
            {
                if ((bool)e.NewValue)
                {
                    //-----------------------------
                    // Show the toggle button
                    //-----------------------------
                    autoCompleteBox._textBox.Margin = new Thickness(0, 0, 30, 0);
                    autoCompleteBox._dropDownToggle.Visibility = Visibility.Visible;
                }
                else
                {
                    //-----------------------------
                    // Hide the toggle button
                    //-----------------------------
                    autoCompleteBox._textBox.Margin = new Thickness(0, 0, 0, 0);
                    autoCompleteBox._dropDownToggle.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Specifies how text in the text box portion of the AutoCompleteBox
        /// control is used to filter items specified by the AutoCompleteBox.ItemsSource
        /// property for display in the drop-down.
        /// </summary>
        public enum AutoCompleteFilterMode
        {
            StartsWith,
            Custom
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
            DependencyProperty.Register("MinimumPopulateDelay", typeof(int), typeof(AutoCompleteBox), new PropertyMetadata(0, MinimumPopulateDelay_Changed));

        private static void MinimumPopulateDelay_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            DependencyProperty.Register("MinimumPrefixLength", typeof(int), typeof(AutoCompleteBox), new PropertyMetadata(1));

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
            DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata("", Text_Changed));

        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)d;

            // Remember the "SearchText":
            if (autoCompleteBox._textBox != null)
                autoCompleteBox._searchText = autoCompleteBox._textBox.Text ?? "";
            else
                autoCompleteBox._searchText = "";

            // Raise the "OnTextChanged" user event:
            autoCompleteBox.OnTextChanged(new RoutedEventArgs());

            // Open or close the popup:
            if (autoCompleteBox.MinimumPrefixLength <= autoCompleteBox.Text.Length)
            {
                //----------------------------------
                // ENOUGH LENGTH
                //----------------------------------
                if (autoCompleteBox.MinimumPopulateDelay != 0)
                {
                    //----------------------------------
                    // TICK EVENT BECAUSE THERE IS A DELAY SET
                    //----------------------------------
                    autoCompleteBox._timer.Stop();
                    autoCompleteBox._timer.Start();
                }
                else
                {
                    //----------------------------------
                    // NO TICK EVENT BECAUSE DELAY IS 0
                    //----------------------------------
                    autoCompleteBox._timer.Stop();

                    // We update the panel, with the new text by opening the popup:
                    autoCompleteBox.IsDropDownOpen = true;
                }
            }
            else
            {
                //----------------------------------
                // NOT ENOUGH LENGTH
                //----------------------------------
                autoCompleteBox.IsDropDownOpen = false;
            }
        }

        protected virtual void OnTextChanged(RoutedEventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

        private void _timer_Tick(object sender, object e)
        {
            //Stopping a timer that was already started
            this._timer.Stop();

            //We update the panel, with the new text by opening the popup:
            IsDropDownOpen = true;
        }

        protected override void UpdateChildrenInVisualTree(IEnumerable oldChildrenEnumerable, IEnumerable newChildrenEnumerable, bool forceUpdateAllChildren = false)
        {
            // This function is called when we use UpdateItemsPanel(ItemsPanel);

            if (_itemFilter != null)
            {

#if !(BRIDGE && MIGRATION)
                newChildrenEnumerable = newChildrenEnumerable.Cast<object>().Where((obj) => (_itemFilter(_searchText, obj)));
#endif

                //Storing the filtered items for future use
                _filteredItems = newChildrenEnumerable;
            }
            base.UpdateChildrenInVisualTree(oldChildrenEnumerable, newChildrenEnumerable, true);
        }

        private AutoCompleteFilterMode _filterMode = AutoCompleteFilterMode.StartsWith;
        
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
                _filterMode = value;
                if (value == AutoCompleteFilterMode.StartsWith)
                {
                    _itemFilter = StartsWith;
                }
            }
        }

        private AutoCompleteFilterPredicate<object> _itemFilter = StartsWith;

        /// <summary>
        /// The custom method that uses the user-entered text to filter the items specified by the ItemsSource property.
        /// </summary>
        public AutoCompleteFilterPredicate<object> ItemFilter
        {
            get { return _itemFilter; }
            set
            {
                if (value != null)
                {
                    _itemFilter = value;
                    _filterMode = AutoCompleteFilterMode.Custom;
                }
                else
                {
                    // If value is null, we reset to the default behavior:
                    _itemFilter = StartsWith;
                    _filterMode = AutoCompleteFilterMode.StartsWith;
                }
            }
        }

        static bool StartsWith(string search, object item)
        {
            if (item != null && search != null)
                return item.ToString().ToLower().StartsWith(search.ToLower());
            else
            {
                return false;
            }
        }

        protected override SelectorItem INTERNAL_GenerateContainer(object item)
        {
            ComboBoxItem comboBoxItem;

            if (item is ComboBoxItem) //if the item is already defined as a ComboBoxItem (defined directly in the Xaml for example), we don't create a ComboBoxItem to contain it.
                comboBoxItem = (ComboBoxItem)item;
            else
                comboBoxItem = new ComboBoxItem();

            comboBoxItem.Click += ComboBoxItem_Click;

            return comboBoxItem;
        }

        void ComboBoxItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedContainer = (SelectorItem)sender;
            _selectedContent = selectedContainer.Content as UIElement;

            // Select the item:
            this.SelectedItem = selectedContainer.INTERNAL_CorrespondingItem;

            // Close the popup:
            if (_dropDownToggle != null)
                _dropDownToggle.IsChecked = false; // Note: this has other effects as well: see the "IsDropDownOpen_Changed" method.
        }

        protected override void ApplySelectedIndex(int index)
        {
            base.ApplySelectedIndex(index);


            UIElement newSelectedContent;

            if (index == -1)
            {
                // index is sometimes at -1, for exemple when the app is starting
                // not en exception but we don't want to treat it as there is no item
                newSelectedContent = null;
            }
            else if (this.Items != null && index < this.Items.Count)
            {
                var item = this.Items[index];

                // If the item is a FrameworkElement, we detach it from its current parent (if any) so that we can later put it into the ContentPresenter:
                if (item is FrameworkElement)
                {
                    // Detach it from the current parent:
                    if (((FrameworkElement)item).INTERNAL_VisualParent is UIElement)
                        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(((FrameworkElement)item), (UIElement)((FrameworkElement)item).INTERNAL_VisualParent);
                }
                else if (item != null)
                {
                    // Otherwise, we create a FrameworkElement out of the item:
                    item = GenerateFrameworkElementToRenderTheItem(item);
                }

                newSelectedContent = (FrameworkElement)item; // Note: this can be null.

                if (_textBox != null)
                    if (newSelectedContent is TextBlock)
                    {
                        TextBlock textblock = (TextBlock)newSelectedContent;
                        _textBox.Text = textblock.Text;
                    }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }

            _selectedContent = newSelectedContent;
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
#else
        protected virtual void OnDropDownClosed(RoutedEventArgs e)
#endif
        {
            if (DropDownClosed != null)
                DropDownClosed(this, e);
        }

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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(AutoCompleteBox), new PropertyMetadata(200d));

#region event

        /// <summary>
        /// Occurs when the text in the text box portion of the AutoCompleteBox changes.
        /// </summary>
        public event RoutedEventHandler TextChanged;

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox closes.
        /// </summary>
#if MIGRATION
        public event EventHandler DropDownClosed;
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
#endregion
    }
}
