
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

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a selection control that combines a non-editable text box and a drop-down
    /// containing a list box that allows users to select an item from a list.
    /// </summary>
    [TemplatePart(Name = ContentPresenterTemplateName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = ContentPresenterBorderTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = DropDownToggleTemplateName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ScrollViewerTemplateName, Type = typeof(ScrollViewer))]
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
        private const string ContentPresenterTemplateName = "ContentPresenter";
        private const string ContentPresenterBorderTemplateName = "ContentPresenterBorder";
        private const string PopupTemplateName = "Popup";
        private const string DropDownToggleTemplateName = "DropDownToggle";
        private const string ScrollViewerTemplateName = "ScrollViewer";
        private const string FocusedDropDownState = "FocusedDropDown";

        private const string WpfContentPresenterName = "contentPresenter";
        private const string WpfPopupName = "PART_Popup";
        private const string WpfToggleButtonName = "toggleButton";
        private const string WpfScrollViewerName = "DropDownScrollViewer";

        private Popup _popup;
        private UIElement _popupChild;
        private ToggleButton _dropDownToggle;
        private ContentPresenter _contentPresenter;
        private FrameworkElement _emptyContent;
        private ScrollViewer _scrollHost;

        static ComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new PropertyMetadata(typeof(ComboBox)));
            IsEnabledProperty.OverrideMetadata(typeof(ComboBox), new PropertyMetadata(OnVisualStatePropertyChanged));
            IsSelectionActivePropertyKey.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(OnIsSelectionActiveChanged));
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(ComboBox), new PropertyMetadata(BooleanBoxes.TrueBox));
        }

        /// <summary>
        /// Initializes a new instance of the ComboBox class.
        /// </summary>
        public ComboBox()
        {
            CanSelectMultiple = false;
        }

        /// <inheritdoc />
        protected internal override bool HandlesScrolling => true;

        internal sealed override ScrollViewer ScrollHost => _scrollHost;

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ComboBoxItem);
        }

        private void UpdatePresenter()
        {
            // propagate the new selected item to the SelectionBoxItem property;
            // this displays it in the selection box
            object item = InternalSelectedItem;
            DataTemplate itemTemplate;
            DataTemplateSelector itemTemplateSelector;
            string stringFormat;

            int index = InternalSelectedIndex;
            if (index <= -1 || (!_useWpfTemplate && IsDropDownOpen && item is FrameworkElement))
            {
                item = _emptyContent;
                itemTemplate = null;
                itemTemplateSelector = null;
                stringFormat = null;
            }
            else
            {
                // if Items contains an explicit ContentControl, use its content instead
                // (this handles the case of ComboBoxItem)
                if (item is ContentControl contentControl)
                {
                    item = contentControl.Content;
                    itemTemplate = contentControl.ContentTemplate;
                    stringFormat = contentControl.ContentStringFormat;
                }
                else
                {
                    itemTemplate = ItemTemplate;
                    stringFormat = ItemStringFormat;
                }

                // When dropdown is open and the content is a UIElement, it can't be in two places.
                // In WPF mode, use a VisualBrush to paint a copy in the display area.
                // In SL mode, show empty content (original behavior).
                if (IsDropDownOpen && _useWpfTemplate && item is FrameworkElement fe)
                {
                    item = new Rectangle
                    {
                        Width = fe.ActualWidth,
                        Height = fe.ActualHeight,
                        Fill = new VisualBrush(fe),
                    };
                    itemTemplate = null;
                    itemTemplateSelector = null;
                }
                itemTemplateSelector = ItemTemplateSelector;
            }

            // display a null item by an empty string
            if (item is null)
            {
                item = string.Empty;
                itemTemplate = ContentPresenter.StringContentTemplate;
            }

            SelectionBoxItem = item;
            SelectionBoxItemTemplate = itemTemplate;
            SelectionBoxItemStringFormat = stringFormat;

            if (_contentPresenter is not null)
            {
                _contentPresenter.Content = item;
                _contentPresenter.ContentTemplate = itemTemplate;
                _contentPresenter.ContentTemplateSelector = itemTemplateSelector;
                _contentPresenter.ContentStringFormat = stringFormat;
            }
        }

        public override void OnApplyTemplate()
        {
            ResolveUseWpfTemplate();

            if (_popup != null)
            {
                _popup.PlacementTarget = null;
                _popup.OutsideClick -= new EventHandler<CancelEventArgs>(OnOutsideClick);
            }

            if (_popupChild != null)
            {
                _popupChild.KeyDown -= new KeyEventHandler(OnPopupKeyDown);
                _popupChild.TextInput -= new TextCompositionEventHandler(OnPopupTextInput);
                _popupChild.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(OnPopupIsKeyboardFocusWithinChanged);
                _popupChild = null;
            }

            // _scrollHost must be set before calling base
            string scrollViewerName = _useWpfTemplate ? WpfScrollViewerName : ScrollViewerTemplateName;
            _scrollHost = GetTemplateChild(scrollViewerName) as ScrollViewer;

            base.OnApplyTemplate();

            string popupName = _useWpfTemplate ? WpfPopupName : PopupTemplateName;
            _popup = GetTemplateChild(popupName) as Popup;

            //this will enable virtualization in combo box without templating the whole style
            if (_popup != null)
            {
                _popup.MaxHeight = MaxDropDownHeight;
                _popup.PlacementTarget = this;
                _popup.Placement = PlacementMode.Bottom;
                _popup.StaysWithinScreenBounds = true;

                // Make sure the popup gets closed when the user clicks outside the combo box, and listen to the Closed event in order to update the drop-down toggle:
                _popup.StayOpen = false;
                _popup.OutsideClick += new EventHandler<CancelEventArgs>(OnOutsideClick);

                _popupChild = _popup.Child;
                if (_popupChild != null)
                {
                    _popupChild.KeyDown += new KeyEventHandler(OnPopupKeyDown);
                    _popupChild.TextInput += new TextCompositionEventHandler(OnPopupTextInput);
                    _popupChild.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(OnPopupIsKeyboardFocusWithinChanged);
                }
            }

            string contentPresenterName = _useWpfTemplate ? WpfContentPresenterName : ContentPresenterTemplateName;
            _contentPresenter = GetTemplateChild(contentPresenterName) as ContentPresenter;
            if (_contentPresenter != null)
            {
                if (_contentPresenter.HasDefaultValue(IsHitTestVisibleProperty))
                {
                    _contentPresenter.IsHitTestVisible = false;
                }

                _emptyContent = _contentPresenter.Content as FrameworkElement;
            }

            string toggleName = _useWpfTemplate ? WpfToggleButtonName : DropDownToggleTemplateName;
            _dropDownToggle = GetTemplateChild(toggleName) as ToggleButton;
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
                    if (ItemsControlFromItemContainer(Keyboard.FocusedElement as DependencyObject) == this)
                    {
                        KeyboardCloseDropDown(true);
                    }
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
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;

            Focus();
            IsDropDownOpen = true;
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.MouseEnter"/> event that occurs
        /// when the mouse pointer enters this control.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateVisualStates();
        }

        /// <summary>
        /// Provides handling for the <see cref="UIElement.MouseLeave"/> event that occurs
        /// when the mouse pointer leaves the combo box.
        /// </summary>
        /// <param name="e">
        /// The event data.
        /// </param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
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

                SetCurrentValueInternal(SelectedIndexProperty, index);

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
                infoToSelect = FocusedInfo;
            }

            IsDropDownOpen = openDropDown;

            if (openDropDown == false && commitSelection && (infoToSelect != null))
            {
                SelectionChange.SelectJustThisItem(infoToSelect, true /* assumeInItemsCollection */);
            }
        }

        private void OnPopupTextInput(object sender, TextCompositionEventArgs e) => OnTextInput(e);

        private void OnPopupIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) =>
            SetValueInternal(IsSelectionActivePropertyKey, (bool)e.NewValue);

        private void OnDropDownToggleClick(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = _dropDownToggle.IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Invoked when the DropDownClosed event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected virtual void OnDropDownClosed(EventArgs e) => DropDownClosed?.Invoke(this, e);

        /// <summary>
        /// Invoked when the DropDownOpened event is raised.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected virtual void OnDropDownOpened(EventArgs e) => DropDownOpened?.Invoke(this, e);

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox closes.
        /// </summary>
        public event EventHandler DropDownClosed;

        /// <summary>
        /// Occurs when the drop-down portion of the ComboBox opens.
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Gets or sets a value that indicates whether the drop-down portion of the
        /// ComboBox is currently open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValueInternal(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                nameof(IsDropDownOpen),
                typeof(bool),
                typeof(ComboBox),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsDropDownOpenChanged, CoerceIsDropDownOpen));

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
                comboBox.OnDropDownOpened(EventArgs.Empty);

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

                bool hasFocus = FocusManager.HasFocus(comboBox, true);

                // Close the popup:
                if (comboBox._popup != null)
                {
                    comboBox._popup.IsOpen = false;
                }

                // Ensure that the toggle button is unchecked:
                if (comboBox._dropDownToggle != null && comboBox._dropDownToggle.IsChecked == true)
                {
                    comboBox._dropDownToggle.IsChecked = false;
                }

                comboBox.UpdatePresenter();

                // Raise the Closed event:
                comboBox.OnDropDownClosed(EventArgs.Empty);

                if (hasFocus)
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
                var cb = (ComboBox)d;
                if (!cb.IsLoaded)
                {
                    cb.Loaded += OpenOnLoad;
                    return false;
                }
            }

            return value;
        }

        private static void OpenOnLoad(object sender, RoutedEventArgs e)
        {
            var cb = (ComboBox)sender;
            cb.Loaded -= OpenOnLoad;
            cb.LayoutUpdated += cb.OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            LayoutUpdated -= OnLayoutUpdated;
            CoerceValue(IsDropDownOpenProperty);
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
            set { SetValueInternal(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(
                nameof(MaxDropDownHeight),
                typeof(double),
                typeof(ComboBox),
                new PropertyMetadata(200d));

        private void OnOutsideClick(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            IsDropDownOpen = false;
        }

        /// <summary>
        /// Identifies the <see cref="IsEditable"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                nameof(IsEditable),
                typeof(bool),
                typeof(ComboBox),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));


        /// <summary>
        /// Gets or sets a value that enables or disables editing of the text in text box of the <see cref="ComboBox"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="ComboBox"/> can be edited; otherwise <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValueInternal(IsEditableProperty, value);
        }

        private static readonly DependencyPropertyKey SelectionBoxItemPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectionBoxItem),
                typeof(object),
                typeof(ComboBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="SelectionBoxItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemProperty = SelectionBoxItemPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the item displayed in the selection box.
        /// </summary>
        public object SelectionBoxItem
        {
            get { return GetValue(SelectionBoxItemProperty); }
            private set { SetValueInternal(SelectionBoxItemPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectionBoxItemTemplatePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectionBoxItemTemplate),
                typeof(DataTemplate),
                typeof(ComboBox),
                new PropertyMetadata((DataTemplate)null));

        /// <summary>
        /// Identifies the <see cref="SelectionBoxItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemTemplateProperty = SelectionBoxItemTemplatePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the template applied to the selection box content.
        /// </summary>
        public DataTemplate SelectionBoxItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectionBoxItemTemplateProperty); }
            private set { SetValueInternal(SelectionBoxItemTemplatePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SelectionBoxItemStringFormatPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectionBoxItemStringFormat),
                typeof(string),
                typeof(ComboBox),
                new PropertyMetadata((string)null));

        /// <summary>
        /// Identifies the <see cref="SelectionBoxItemStringFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemStringFormatProperty = SelectionBoxItemStringFormatPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a composite string that specifies how to format the selected item in the selection box if it is 
        /// displayed as a string.
        /// </summary>
        /// <returns>
        /// A composite string that specifies how to format the selected item in the selection box if it is 
        /// displayed as a string.
        /// </returns>
        public string SelectionBoxItemStringFormat
        {
            get => (string)GetValue(SelectionBoxItemStringFormatProperty);
            private set => SetValueInternal(SelectionBoxItemStringFormatPropertyKey, value);
        }

        /// <summary>
        /// Identifies the IsSelectionActive attached property.
        /// </summary>
        new public static readonly DependencyProperty IsSelectionActiveProperty = Selector.IsSelectionActiveProperty;

        private static void OnIsSelectionActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).UpdateVisualStates();
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectionBoxHighlighted"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsSelectionBoxHighlightedProperty =
            DependencyProperty.Register(
                nameof(IsSelectionBoxHighlighted),
                typeof(bool),
                typeof(ComboBox),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets a value that indicates whether the SelectionBoxItem component is highlighted.
        /// </summary>
        [OpenSilver.NotImplemented]
        public bool IsSelectionBoxHighlighted
        {
            get { return (bool)GetValue(IsSelectionBoxHighlightedProperty); }
            private set { SetValueInternal(IsSelectionBoxHighlightedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TemplateMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateModeProperty =
            DependencyProperty.Register(
                nameof(TemplateMode),
                typeof(TemplateMode),
                typeof(ComboBox),
                new PropertyMetadata(TemplateMode.Auto));

        /// <summary>
        /// Gets or sets a value that determines whether the <see cref="ComboBox"/> uses WPF template
        /// parts and behaviors (e.g. keeping the selected item shown via a VisualBrush when the
        /// dropdown is open), the Silverlight template parts and behaviors, or automatically detects
        /// the applied template. The default is <see cref="TemplateMode.Auto"/>.
        /// </summary>
        public TemplateMode TemplateMode
        {
            get { return (TemplateMode)GetValue(TemplateModeProperty); }
            set { SetValue(TemplateModeProperty, value); }
        }

        private bool _useWpfTemplate;

        /// <summary>
        /// Resolves whether the applied template is a WPF-style template and caches the outcome in
        /// <see cref="_useWpfTemplate"/>.
        /// </summary>
        private void ResolveUseWpfTemplate()
        {
            switch (TemplateMode)
            {
                case TemplateMode.Wpf:
                    _useWpfTemplate = true;
                    break;
                case TemplateMode.Silverlight:
                    _useWpfTemplate = false;
                    break;
                default:
                    _useWpfTemplate = GetTemplateChild(WpfPopupName) is Popup;
                    break;
            }
        }

        internal override void UpdateVisualStates(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, useTransitions);
            }
            else if (IsMouseOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
            }

            if (!GetIsSelectionActive(this))
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, useTransitions);
            }
            else if (IsDropDownOpen)
            {
                VisualStateManager.GoToState(this, FocusedDropDownState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, useTransitions);
            }
        }
    }
}