
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
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetForHtml5.Core;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a selection control that combines a non-editable text box and a drop-down
    /// containing a list box that allows users to select an item from a list.
    /// </summary>
    [TemplatePart(Name = "ContentPresenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "ContentPresenterBorder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "FocusedDropDown", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    public class ComboBox : Selector
    {
        private Popup _popup;
        private UIElement _popupChild;
        private ToggleButton _dropDownToggle;
        private ContentPresenter _contentPresenter;
        private FrameworkElement _emptyContent;
        private ScrollViewer _scrollHost;
        private ItemInfo _highlightedInfo;

        [Obsolete(Helper.ObsoleteMemberMessage + " Use 'CSHTML5.Native.Html.Controls.NativeComboBox' instead.")]
        public bool UseNativeComboBox
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Initializes a new instance of the ComboBox class.
        /// </summary>
        public ComboBox()
        {
            this.DefaultStyleKey = typeof(ComboBox);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ComboBoxItem);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        protected override SelectorItem INTERNAL_GenerateContainer(object item)
        {
            return (SelectorItem)this.GetContainerFromItem(item);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        protected override DependencyObject GetContainerFromItem(object item)
        {
            ComboBoxItem comboBoxItem = item as ComboBoxItem ?? new ComboBoxItem();
            return comboBoxItem;
        }

        private void UpdatePresenter()
        {
            object content;
            DataTemplate template;
            object selectionBoxItem;
            DataTemplate selectionBoxItemTemplate;

            int index = SelectedIndex;
            if (index <= -1 || (IsDropDownOpen && SelectedItem is FrameworkElement))
            {
                content = _emptyContent;
                selectionBoxItem = null;
                template = selectionBoxItemTemplate = null;
            }
            else
            {
                ComboBoxItem cbi = (ItemContainerGenerator.ContainerFromIndex(index) ?? Items[index]) as ComboBoxItem;
                if (cbi != null)
                {
                    content = selectionBoxItem = cbi.Content;
                    template = selectionBoxItemTemplate = cbi.ContentTemplate;
                }
                else
                {
                    object item = Items[index];
                    content = selectionBoxItem = item;
                    if (item is UIElement)
                    {
                        template = selectionBoxItemTemplate = null;
                    }
                    else
                    {
                        template = selectionBoxItemTemplate = ItemTemplate ?? GetDataTemplateForDisplayMemberPath(DisplayMemberPath);
                    }
                }
            }

            if (_contentPresenter != null)
            {
                _contentPresenter.Content = content;
                _contentPresenter.ContentTemplate = template;
            }

            // Update the SelectionBoxItem and SelectionBoxItemTemplate properties
            SelectionBoxItem = selectionBoxItem;
            SelectionBoxItemTemplate = selectionBoxItemTemplate;
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_popup != null)
            {
                _popup.PlacementTarget = null;
                _popup.OutsideClick -= new EventHandler<OutsideClickEventArgs>(OnOutsideClick);
            }

            if (_popupChild != null)
            {
                _popupChild.KeyDown -= new KeyEventHandler(OnPopupKeyDown);
                _popupChild.TextInput -= new TextCompositionEventHandler(OnPopupTextInput);
                _popupChild = null;
            }

            _popup = GetTemplateChild("Popup") as Popup;
          
            //this will enable virtualization in combo box without templating the whole style
            if (_popup != null)
            {
                //reason for following if condition is backward compatibility, till now we had to define template and manually set custom layout of popup to true
                //and thus we don't need to set custom layout for combo box to make this work.
                if (this.CustomLayout)
                {
                    _popup.CustomLayout = true;
                }
                _popup.MaxHeight = this.MaxDropDownHeight;
                
                //todo: once we will have made the following properties (PlacementTarget and Placement) Dependencyproperties, unset it here and set it in the default style.
                _popup.PlacementTarget = this;
                _popup.Placement = PlacementMode.Bottom;
                _popup.INTERNAL_PopupMoved += _popup_INTERNAL_PopupMoved;

                // Make sure the popup gets closed when the user clicks outside the combo box, and listen to the Closed event in order to update the drop-down toggle:
                _popup.StayOpen = false;
                _popup.OutsideClick += new EventHandler<OutsideClickEventArgs>(OnOutsideClick);

                _popupChild = _popup.Child;
                if (_popupChild != null)
                {
                    _popupChild.KeyDown += new KeyEventHandler(OnPopupKeyDown);
                    _popupChild.TextInput += new TextCompositionEventHandler(OnPopupTextInput);
                }
            }

            _contentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;
            if (_contentPresenter != null)
            {
                if (_contentPresenter.HasDefaultValue(IsHitTestVisibleProperty))
                {
                    _contentPresenter.IsHitTestVisible = false;
                }

                _emptyContent = _contentPresenter.Content as FrameworkElement;
            }

            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            if (_dropDownToggle != null)
            {
                _dropDownToggle.Click += new RoutedEventHandler(OnDropDownToggleClick);
            }

            _scrollHost = GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (_scrollHost != null)
            {
                if (_scrollHost.ReadLocalValue(ScrollViewer.HorizontalScrollBarVisibilityProperty) == DependencyProperty.UnsetValue)
                {
                    _scrollHost.HorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(this);
                }
                if (_scrollHost.ReadLocalValue(ScrollViewer.VerticalScrollBarVisibilityProperty) == DependencyProperty.UnsetValue)
                {
                    _scrollHost.VerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(this);
                }
            }

            UpdatePresenter();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    if (IsDropDownOpen)
                    {
                        NavigateToPrev();
                    }
                    else
                    {
                        SelectPrev();
                    }
                    break;

                case Key.Down:
                    if (IsDropDownOpen)
                    {
                        NavigateToNext();
                    }
                    else
                    {
                        SelectNext();
                    }
                    break;

                case Key.Escape:
                    if (IsDropDownOpen)
                    {
                        KeyboardCloseDropDown(false);
                    }
                    break;

                case Key.Enter:
                    if (IsDropDownOpen)
                    {
                        KeyboardCloseDropDown(true);
                    }
                    else
                    {
                        IsDropDownOpen = true;
                    }
                    break;
            }
        }

        private void OnPopupKeyDown(object sender, KeyEventArgs e) => OnKeyDown(e);

        /// <summary>
        /// Provides handling for the <see cref="UIElement.MouseLeftButtonDown"/> event
        /// that occurs when the left mouse button is pressed while the mouse pointer is
        /// over the combo box.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;

            IsDropDownOpen = true;
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (!IsDropDownOpen)
            {
                UpdatePresenter();
            }
        }

        /// <summary>
        /// Returns a <see cref="ComboBoxAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="ComboBoxAutomationPeer"/> for the <see cref="ComboBox"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ComboBoxAutomationPeer(this);

        internal void NotifyComboBoxItemMouseUp(ComboBoxItem comboBoxItem)
        {
            object item = ItemContainerGenerator.ItemFromContainer(comboBoxItem);
            if (item != DependencyProperty.UnsetValue)
            {
                SelectionChange.SelectJustThisItem(NewItemInfo(item, comboBoxItem), true /* assumeInItemsCollection */);
            }

            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }
        }

        internal override bool FocusItem(ItemInfo info)
        {
            bool returnValue = base.FocusItem(info);

            _highlightedInfo = info.Container is ComboBoxItem ? info : null;

            if (!IsDropDownOpen)
            {
                int index = info.Index;

                if (index < 0)
                {
                    index = Items.IndexOf(info.Item);
                }

                SetCurrentValue(SelectedIndexProperty, index);

                returnValue = true;
            }
            
            return returnValue;
        }

        /// <summary>
        /// Called to close the DropDown using the keyboard.
        /// </summary>
        private void KeyboardCloseDropDown(bool commitSelection)
        {
            KeyboardToggleDropDown(false /* openDropDown */, commitSelection);
        }

        private void KeyboardToggleDropDown(bool openDropDown, bool commitSelection)
        {
            // Close the dropdown and commit the selection if requested.
            // Make sure to set the selection after the dropdown has closed
            // so we don't trigger any unnecessary navigation as a result
            // of changing the selection.
            ItemInfo infoToSelect = null;
            if (commitSelection)
            {
                infoToSelect = _highlightedInfo;
            }

            IsDropDownOpen = openDropDown;

            if (openDropDown == false && commitSelection && (infoToSelect != null))
            {
                SelectionChange.SelectJustThisItem(infoToSelect, true /* assumeInItemsCollection */);
            }
        }

        private void SelectPrev()
        {
            if (Items.Count > 0)
            {
                int selectedIndex = InternalSelectedIndex;

                // Search backwards from SelectedIndex - 1 but don't start before the beginning.
                // If SelectedIndex is less than 0, there is nothing to select before this item.
                if (selectedIndex > 0)
                {
                    SelectItemHelper(selectedIndex - 1, -1, -1);
                }
            }
        }

        private void SelectNext()
        {
            int count = Items.Count;
            if (count > 0)
            {
                int selectedIndex = InternalSelectedIndex;

                // Search forwards from SelectedIndex + 1 but don't start past the end.
                // If SelectedIndex is before the last item then there is potentially
                // something afterwards that we could select.
                if (selectedIndex < count - 1)
                {
                    SelectItemHelper(selectedIndex + 1, 1, count);
                }
            }
        }

        private void NavigateToPrev()
        {
            if (Items.Count > 0)
            {
                int focusedIndex = _highlightedInfo != null ? _highlightedInfo.Index : -1;
                if (focusedIndex > 0)
                {
                    ItemInfo info = GetNextItemInfoHelper(focusedIndex - 1, -1, -1);
                    if (info != null)
                    {
                        FocusItem(info);
                    }
                }
            }
        }

        private void NavigateToNext()
        {
            int count = Items.Count;
            if (count > 0)
            {
                int focusedIndex = _highlightedInfo != null ? _highlightedInfo.Index : -1;
                if (focusedIndex < count - 1)
                {
                    ItemInfo info = GetNextItemInfoHelper(focusedIndex + 1, 1, count);
                    if (info != null)
                    {
                        FocusItem(info);
                    }
                }
            }
        }

        private void SelectItemHelper(int startIndex, int increment, int stopIndex)
        {
            ItemInfo info = GetNextItemInfoHelper(startIndex, increment, stopIndex);
            if (info != null)
            {
                SelectionChange.SelectJustThisItem(info, true /* assumeInItemsCollection */);
            }
        }

        // Walk in the specified direction until we get to a selectable
        // item or to the stopIndex.
        // NOTE: stopIndex is not inclusive (it should be one past the end of the range)
        private ItemInfo GetNextItemInfoHelper(int startIndex, int increment, int stopIndex)
        {
            Debug.Assert((increment > 0 && startIndex <= stopIndex) || (increment < 0 && startIndex >= stopIndex), "Infinite loop detected");

            for (int i = startIndex; i != stopIndex; i += increment)
            {
                // If the item is selectable and the wrapper is selectable, select it.
                // Need to check both because the user could set any combination of
                // IsSelectable and IsEnabled on the item and wrapper.
                object item = Items[i];
                DependencyObject container = ItemContainerGenerator.ContainerFromIndex(i);
                if (IsSelectableHelper(item) && IsSelectableHelper(container))
                {
                    return NewItemInfo(item, container, i);
                }
            }

            return null;
        }

        private bool IsSelectableHelper(object o)
        {
            // If o is not a DependencyObject, it is just a plain
            // object and must be selectable and enabled.
            if (o is not FrameworkElement fe)
            {
                return true;
            }
            // It's selectable if IsSelectable is true and IsEnabled is true.
            return (bool)fe.GetValue(IsEnabledProperty);
        }

        private void OnPopupTextInput(object sender, TextCompositionEventArgs e) => OnTextInput(e);

        private void _popup_INTERNAL_PopupMoved(object sender, EventArgs e)
        {
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(_popup);
        }

        private void OnDropDownToggleClick(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = _dropDownToggle.IsChecked.GetValueOrDefault();
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
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ComboBox), new PropertyMetadata(false, IsDropDownOpen_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void IsDropDownOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = (ComboBox)d;

            // IMPORTANT: we must NOT require that the element be in the visual tree because the DropDown may be closed even when the ComboBox is not in the visual tree (for example, after it has been removed from the visual tree)
            if (e.NewValue is bool)
            {
                bool isDropDownOpen = (bool)e.NewValue;

                if (isDropDownOpen)
                {
                    //-----------------------------
                    // Show the Popup
                    //-----------------------------

                    // Show the popup:
                    if (comboBox._popup != null)
                    {
                        // add removed

                        comboBox._popup.IsOpen = true;

                        // Make sure the Width of the popup is at least the same as the popup
                        if (comboBox._popup.Child is FrameworkElement child)
                        {
                            child.MinWidth = comboBox._popup.ActualWidth;
                        }
                    }

                    // Ensure that the toggle button is checked:
                    if (comboBox._dropDownToggle != null
                        && comboBox._dropDownToggle.IsChecked == false)
                    {
                        comboBox._dropDownToggle.IsChecked = true;
                    }

                    comboBox.UpdatePresenter();

                    // Raise the Opened event:
#if MIGRATION
                    comboBox.OnDropDownOpened(new EventArgs());
#else
                    comboBox.OnDropDownOpened(new RoutedEventArgs());
#endif

                    comboBox.EnsurePopupIsWithinBoundaries();
                }
                else
                {
                    //-----------------------------
                    // Hide the Popup
                    //-----------------------------

                    // Close the popup:
                    if (comboBox._popup != null)
                        comboBox._popup.IsOpen = false;

                    // Ensure that the toggle button is unchecked:
                    if (comboBox._dropDownToggle != null && comboBox._dropDownToggle.IsChecked == true)
                    {
                        comboBox._dropDownToggle.IsChecked = false;
                    }

                    comboBox.UpdatePresenter();

                    // Raise the Closed event:
#if MIGRATION
                    comboBox.OnDropDownClosed(new EventArgs());
#else
                    comboBox.OnDropDownClosed(new RoutedEventArgs());
#endif
                }
            }
        }

        private async void EnsurePopupIsWithinBoundaries()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(_popup);
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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(ComboBox), new PropertyMetadata(200d));

        private void OnOutsideClick(object sender, OutsideClickEventArgs e)
        {
            e.Handled = true;

            IsDropDownOpen = false;
        }

        /// <summary>
        /// Gets a value that indicates whether the user can edit text in the text box
        /// portion of the ComboBox. This property always returns false.
        /// </summary>
        public bool IsEditable 
        { 
            get { return false; } 
        }

        /// <summary>
        /// Identifies the <see cref="ComboBox.SelectionBoxItem"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty SelectionBoxItemProperty =
            DependencyProperty.Register(
                nameof(SelectionBoxItem),
                typeof(object),
                typeof(ComboBox),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets the item displayed in the selection box.
        /// </summary>
        public object SelectionBoxItem
        {
            get { return this.GetValue(SelectionBoxItemProperty); }
            private set { this.SetValue(SelectionBoxItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ComboBox.SelectionBoxItemTemplate"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty SelectionBoxItemTemplateProperty =
            DependencyProperty.Register(
                nameof(SelectionBoxItemTemplate),
                typeof(DataTemplate),
                typeof(ComboBox),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Gets the template applied to the selection box content.
        /// </summary>
        public DataTemplate SelectionBoxItemTemplate
        {
            get { return (DataTemplate)this.GetValue(SelectionBoxItemTemplateProperty); }
            private set { this.SetValue(SelectionBoxItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ComboBox.IsSelectionBoxHighlighted"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsSelectionBoxHighlightedProperty =
            DependencyProperty.Register(
                nameof(IsSelectionBoxHighlighted),
                typeof(bool),
                typeof(ComboBox),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets a value that indicates whether the SelectionBoxItem component is highlighted.
        /// </summary>
        [OpenSilver.NotImplemented]
        public bool IsSelectionBoxHighlighted
        {
            get { return (bool)this.GetValue(IsSelectionBoxHighlightedProperty); }
            private set { this.SetValue(IsSelectionBoxHighlightedProperty, value); }
        }
    }
}