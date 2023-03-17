
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
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = FocusedDropDownState, GroupName = VisualStates.GroupFocus)]
    public class ComboBox : Selector
    {
        private const string FocusedDropDownState = "FocusedDropDown";

        private Popup _popup;
        private UIElement _popupChild;
        private ToggleButton _dropDownToggle;
        private ContentPresenter _contentPresenter;
        private FrameworkElement _emptyContent;
        private ScrollViewer _scrollHost;
        private bool _isMouseOver;
        private bool _isFocused;

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
            DefaultStyleKey = typeof(ComboBox);
            IsEnabledChanged += (o, e) =>
            {
                if (!(bool)e.NewValue)
                {
                    _isMouseOver = false;
                }

                UpdateVisualStates();
            };
        }

        internal sealed override bool HandlesScrolling => true;

        internal sealed override ScrollViewer ScrollHost => _scrollHost;

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
            if (_popup != null)
            {
                _popup.PlacementTarget = null;
                _popup.OutsideClick -= new EventHandler<OutsideClickEventArgs>(OnOutsideClick);
            }

            if (_popupChild != null)
            {
                if (_popupChild is FrameworkElement fe)
                    fe.SizeChanged -= new SizeChangedEventHandler(OnPopupChildSizeChanged);
                _popupChild.KeyDown -= new KeyEventHandler(OnPopupKeyDown);
                _popupChild.TextInput -= new TextCompositionEventHandler(OnPopupTextInput);
                _popupChild = null;
            }

            // _scrollHost must be set before calling base
            _scrollHost = GetTemplateChild("ScrollViewer") as ScrollViewer;

            base.OnApplyTemplate();

            _popup = GetTemplateChild("Popup") as Popup;

            //this will enable virtualization in combo box without templating the whole style
            if (_popup != null)
            {
                //reason for following if condition is backward compatibility, till now we had to define template and manually set custom layout of popup to true
                //and thus we don't need to set custom layout for combo box to make this work.
                if (CustomLayout)
                {
                    _popup.CustomLayout = true;
                }
                _popup.MaxHeight = MaxDropDownHeight;

                //todo: once we will have made the following properties (PlacementTarget and Placement) Dependencyproperties, unset it here and set it in the default style.
                _popup.PlacementTarget = this;
                _popup.Placement = PlacementMode.Bottom;
                _popup.StaysWithinScreenBounds = true;

                // Make sure the popup gets closed when the user clicks outside the combo box, and listen to the Closed event in order to update the drop-down toggle:
                _popup.StayOpen = false;
                _popup.OutsideClick += new EventHandler<OutsideClickEventArgs>(OnOutsideClick);

                _popupChild = _popup.Child;
                if (_popupChild != null)
                {
                    if (_popupChild is FrameworkElement fe)
                        fe.SizeChanged += new SizeChangedEventHandler(OnPopupChildSizeChanged);
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
                _contentPresenter.ClipToBounds = true;
            }

            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            if (_dropDownToggle != null)
            {
                _dropDownToggle.Click += new RoutedEventHandler(OnDropDownToggleClick);
            }

            UpdatePresenter();
            UpdateVisualStates();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            if (IsDropDownOpen)
            {
                HandlePopupKeyDown(e);
            }
            else
            {
                HandleComboBoxKeyDown(e);
            }
        }

        private void HandlePopupKeyDown(KeyEventArgs e)
        {
            bool handled = false;
            int newFocusedIndex = -1;
            switch (e.Key)
            {
                case Key.Enter:
                    KeyboardCloseDropDown(true);
                    break;
                case Key.Escape:
                    KeyboardCloseDropDown(false);
                    break;
                case Key.Home:
                    newFocusedIndex = NavigateToStart();
                    break;
                case Key.End:
                    newFocusedIndex = NavigateToEnd();
                    break;
                case Key.PageUp:
                    newFocusedIndex = NavigateByPage(false);
                    break;
                case Key.PageDown:
                    newFocusedIndex = NavigateByPage(true);
                    break;
                case Key.Left:
                    if (IsVerticalOrientation())
                    {
                        ElementScrollViewerScrollInDirection(Key.Left);
                    }
                    else
                    {
                        newFocusedIndex = NavigateByLine(false);
                    }
                    break;
                case Key.Up:
                    if (IsVerticalOrientation())
                    {
                        newFocusedIndex = NavigateByLine(false);
                    }
                    else
                    {
                        ElementScrollViewerScrollInDirection(Key.Up);
                    }
                    break;
                case Key.Right:
                    if (IsVerticalOrientation())
                    {
                        ElementScrollViewerScrollInDirection(Key.Right);
                    }
                    else
                    {
                        newFocusedIndex = NavigateByLine(true);
                    }
                    break;
                case Key.Down:
                    if (IsVerticalOrientation())
                    {
                        newFocusedIndex = NavigateByLine(true);
                    }
                    else
                    {
                        ElementScrollViewerScrollInDirection(Key.Down);
                    }
                    break;
                default:
                    Debug.Assert(!handled);
                    break;
            }

            if (newFocusedIndex >= 0 && newFocusedIndex < Items.Count)
            {
                FocusItemInternal(newFocusedIndex);
                handled = true;
            }

            if (handled)
            {
                e.Handled = true;
            }
        }

        private void HandleComboBoxKeyDown(KeyEventArgs e)
        {
            bool handled = false;
            int newSelectedIndex = -1;
            switch (e.Key)
            {
                case Key.Enter:
                    IsDropDownOpen = true;
                    handled = true;
                    break;
                case Key.End:
                    SelectedIndex = Items.Count - 1;
                    break;
                case Key.Home:
                    newSelectedIndex = 0;
                    break;
                case Key.Up:
                case Key.Left:
                    if ((!IsVerticalOrientation() || e.Key == Key.Up) && SelectedIndex >= 0)
                    {
                        newSelectedIndex = GetNextSelectableIndex(SelectedIndex - 1, -1, -1);
                    }
                    break;
                case Key.Down:
                case Key.Right:
                    if ((!IsVerticalOrientation() || e.Key == Key.Down) && SelectedIndex < Items.Count)
                    {
                        newSelectedIndex = GetNextSelectableIndex(SelectedIndex + 1, 1, Items.Count);
                    }
                    break;
                default:
                    Debug.Assert(!handled);
                    break;
            }

            if (newSelectedIndex >= 0 && newSelectedIndex < Items.Count)
            {
                SelectionChange.SelectJustThisItem(ItemInfoFromIndex(newSelectedIndex), true);
                handled = true;
            }

            if (handled)
            {
                e.Handled = true;
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

        /// <summary>
        /// Provides handling for the <see cref="UIElement.MouseEnter"/> event that occurs
        /// when the mouse pointer enters this control.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs e)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
            _isMouseOver = true;
            UpdateVisualStates();
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.MouseLeave"/> event that occurs
        /// when the mouse pointer leaves the combo box.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
#if MIGRATION
        protected override void OnMouseLeave(MouseEventArgs e)
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(e);
#else
            base.OnPointerReleased(e);
#endif
            _isMouseOver = false;
            UpdateVisualStates();
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.GotFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;
            UpdateVisualStates();
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;
            UpdateVisualStates();
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
                infoToSelect = ItemInfoFromIndex(FocusedIndex);
            }

            IsDropDownOpen = openDropDown;

            if (openDropDown == false && commitSelection && (infoToSelect != null))
            {
                SelectionChange.SelectJustThisItem(infoToSelect, true /* assumeInItemsCollection */);
            }
        }

        private void OnPopupTextInput(object sender, TextCompositionEventArgs e) => OnTextInput(e);

        private void OnPopupChildSizeChanged(object sender, SizeChangedEventArgs e) => _popup?.Reposition();

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
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                typeof(ComboBox),
                new PropertyMetadata(false, OnIsDropDownOpenChanged, CoerceIsDropDownOpen));

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = (ComboBox)d;
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
                comboBox.OnDropDownOpened(EventArgs.Empty);
#else
                comboBox.OnDropDownOpened(new RoutedEventArgs());
#endif

                if (FocusManager.HasFocus(comboBox, false))
                {
                    comboBox.ScrollTo(comboBox.SelectedIndex);
                }
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
                comboBox.OnDropDownClosed(EventArgs.Empty);
#else
                comboBox.OnDropDownClosed(new RoutedEventArgs());
#endif

                if (FocusManager.HasFocus(comboBox, true))
                {
                    comboBox.ScrollTo(-1);
                }
            }

            comboBox.UpdateVisualStates();
        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                ComboBox cb = (ComboBox)d;
                if (!cb.IsLoaded)
                {
                    cb.Loaded += new RoutedEventHandler(OpenOnLoad);
                    return false;
                }
            }

            return value;
        }
        
        private static void OpenOnLoad(object sender, RoutedEventArgs e)
        {
            var cb = (ComboBox)sender;
            cb.Loaded -= new RoutedEventHandler(OpenOnLoad);
            cb.Dispatcher.BeginInvoke(() => cb.CoerceValue(IsDropDownOpenProperty));
        }

        private void ScrollTo(int index)
        {
            if (index > -1)
            {
                UpdateLayout();
                FocusItemInternal(index);
                ScrollIntoViewImpl(index);
            }
            else
            {
                Focus();
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

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, false);
            }
            else if (_isMouseOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, false);
            }

            if (IsDropDownOpen)
            {
                VisualStateManager.GoToState(this, FocusedDropDownState, false);
            }
            else if (_isFocused)
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, false);
            }
        }
    }
}