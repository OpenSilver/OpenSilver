

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


using DotNetForHtml5.Core;
using System;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
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
        private ToggleButton _dropDownToggle;
        private ContentPresenter _contentPresenter;
        private FrameworkElement _emptyContent;
        private bool _suppressCloseOnOutsideClick;

        [Obsolete("ComboBox does not support Native ComboBox. Use 'CSHTML5.Native.Html.Controls.NativeComboBox' instead.")]
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

        private void UpdatePresenter()
        {
            object content;
            DataTemplate template;
            object selectionBoxItem;
            DataTemplate selectionBoxItemTemplate;

            int index = SelectedIndex;
            if (index == -1 || (IsDropDownOpen && SelectedItem is FrameworkElement))
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
                _popup.OutsideClick -= OnOutsideClick;
                _popup.ClosedDueToOutsideClick -= Popup_ClosedDueToOutsideClick; // Note: we do this here rather than at "OnDetached" because it may happen that the popup is closed after the ComboBox has been removed from the visual tree (in which case, when putting it back into the visual tree, we want the drop down to be in its initial closed state).
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
            }

            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            _contentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;
            if (_contentPresenter != null)
            {
                if (_contentPresenter.HasDefaultValue(IsHitTestVisibleProperty))
                {
                    _contentPresenter.IsHitTestVisible = false;
                }

                _emptyContent = _contentPresenter.Content as FrameworkElement;
            }

            //todo: once we will have made the following properties (PlacementTarget and Placement) Dependencyproperties, unset it here and set it in the default style.
            _popup.PlacementTarget = this;
            _popup.Placement = PlacementMode.Bottom;
            _popup.INTERNAL_PopupMoved += _popup_INTERNAL_PopupMoved;

            if (_dropDownToggle != null)
            {
                _dropDownToggle.Checked += DropDownToggle_Checked;
                _dropDownToggle.Unchecked += DropDownToggle_Unchecked;
            }

            // Make sure the popup gets closed when the user clicks outside the combo box, and listen to the Closed event in order to update the drop-down toggle:
            if (_popup != null)
            {
                _popup.StayOpen = false;
                _popup.OutsideClick += OnOutsideClick;
                _popup.ClosedDueToOutsideClick += Popup_ClosedDueToOutsideClick;
            }
            
            UpdatePresenter();
        }

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

            _suppressCloseOnOutsideClick = true;
            IsDropDownOpen = true;
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdatePresenter();
        }

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

        private void _popup_INTERNAL_PopupMoved(object sender, EventArgs e)
        {
            INTERNAL_PopupsManager.EnsurePopupStaysWithinScreenBounds(_popup);
        }

        void DropDownToggle_Checked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = true;
        }

        void DropDownToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
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

#if MIGRATION
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
            IsDropDownOpen = !IsDropDownOpen;

            e.Handled = true;

#if MIGRATION
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
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
            if (_suppressCloseOnOutsideClick)
            {
                e.Handled = true;
                _suppressCloseOnOutsideClick = false;
            }
        }

        void Popup_ClosedDueToOutsideClick(object sender, EventArgs e)
        {
            //------------------
            // The user clicked outside the combo box, so the Popup closed itself. We now need to reflect this on the ComboBox appearance (drop-down toggle...).
            //------------------

            if (this._dropDownToggle != null)
            {
#if MIGRATION
                // See comment below
                if (this._dropDownToggle.IsMouseCaptured)
                {
                    this._dropDownToggle.ReleaseMouseCapture();
                }

#else
                // In case the pointer is captured by the toggle button, we need to release it because the Click event would be triggered right after the popup was closed, 
                // resulting in the popup to reopen right away.
                // To reproduce the issue that happens if we remove the "if" block of code below: create a ComboBox with items, click the ToggleButton of the ComboBox to
                // open the drop -down popup, then click it again to close the drop-down. Expected result: the drop-down is closed. Actual result: the popup closes and re-opens.
                // The issue was due to the fact that, when we clicked on the ToggleButton to close the drop-down, the toggle button became Unchecked due to the
                // "Popup.ClosedDueToOutsideClick" event (resulting in the popup being successfully closed), but then it reopened because the "PointerReleased" event of the
                // ToggleButton was raised, which re-checked the unchecked ToggleButton. By releasing the capture, we prevent the "PointerReleased" event of the ToggleButton
                // to be raised.
                if (this._dropDownToggle.IsPointerCaptured)
                {
                    this._dropDownToggle.ReleasePointerCapture();
                }
#endif
                this._dropDownToggle.IsChecked = false; // Note: this has other effects as well: see the "IsDropDownOpen_Changed" method
            }
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