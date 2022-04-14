﻿

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
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
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
        Popup _popup;
        ToggleButton _dropDownToggle;
        ContentPresenter _contentPresenter;

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

        private void UpdateContentPresenter()
        {
            if (this._contentPresenter == null)
            {
                return;
            }

            object content;
            DataTemplate template;

            object item = this.SelectedItem;
            if (item == null)
            {
                content = null;
                template = null;
            }
            else
            {
                ComboBoxItem cbi = item as ComboBoxItem;
                if (cbi != null)
                {
                    content = cbi.Content;
                    template = cbi.ContentTemplate;
                }
                else
                {
                    content = item;
                    if (item is UIElement)
                    {
                        template = null;
                    }
                    else
                    {
                        template = this.ItemTemplate ?? ItemsControl.GetDataTemplateForDisplayMemberPath(this.DisplayMemberPath);
                    }
                }
            }

            this._contentPresenter.Content = content;
            this._contentPresenter.ContentTemplate = template;

            // Update the SelectionBoxItem and SelectionBoxItemTemplate properties
            this.SelectionBoxItem = content;
            this.SelectionBoxItemTemplate = template;
        }

        private ScrollViewer _scroller;
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (_popup != null)
                _popup.ClosedDueToOutsideClick -= Popup_ClosedDueToOutsideClick; // Note: we do this here rather than at "OnDetached" because it may happen that the popup is closed after the ComboBox has been removed from the visual tree (in which case, when putting it back into the visual tree, we want the drop down to be in its initial closed state).

            _popup = GetTemplateChild("Popup") as Popup;
            _dropDownToggle = GetTemplateChild("DropDownToggle") as ToggleButton;
            _contentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;
            //todo: once we will have made the following properties (PlacementTarget and Placement) Dependencyproperties, unset it here and set it in the default style.
            _popup.PlacementTarget = _dropDownToggle;
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
                _popup.ClosedDueToOutsideClick += Popup_ClosedDueToOutsideClick;
            }

            if (!IsDropDownOpen)
            {
                UpdateContentPresenter();
            }

            _scroller = FindChildElementOfType<ScrollViewer>(_popup.Child as FrameworkElement);
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (!IsDropDownOpen)
            {
                UpdateContentPresenter();
            }
        }

        internal void NotifyComboBoxItemMouseUp(ComboBoxItem comboBoxItem)
        {
            object item = ItemContainerGenerator.ItemFromContainer(comboBoxItem);
            if (item != null && item != DependencyProperty.UnsetValue)
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

                    // Empty the ContentPresenter so that, in case it is needed, the same item can be placed in the popup:
                    if (comboBox._contentPresenter != null)
                    {
                        comboBox._contentPresenter.Content = null;
                        comboBox.SelectionBoxItem = null;
                        comboBox.SelectionBoxItemTemplate = null;
                    }

                    // Show the popup:
                    if (comboBox._popup != null)
                    {
                        // add removed 

                        comboBox._popup.IsOpen = true;
                        comboBox.UpdatePopupHeight();

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

                    comboBox.UpdateContentPresenter();

                    // Ensure that the toggle button is unchecked:
                    if (comboBox._dropDownToggle != null && comboBox._dropDownToggle.IsChecked == true)
                    {
                        comboBox._dropDownToggle.IsChecked = false;
                    }

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
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(ComboBox), new PropertyMetadata(double.PositiveInfinity, OnMaxDropDownHeightChanged));

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).UpdatePopupHeight();
        }

        public static T FindChildElementOfType<T>(FrameworkElement element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;

            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {

                int numChildren = System.Windows.Media.VisualTreeHelper.GetChildrenCount(element);

                for (int i = 0; i < numChildren; i++)
                {
                    T child = System.Windows.Media.VisualTreeHelper.GetChild(element, i) as T;
                    if (child != null)
                    {
                        return child;
                    }
                }

                Popup popup = element as Popup;

                if (popup != null)
                {
                    return popup.Child as T;
                }
            }

            return null;
        }

        private void UpdatePopupHeight()
        {
            if (_popup == null || _scroller == null) return;
            if (!_popup.IsOpen || _popup.PopupRoot == null || !(_popup.PopupRoot.Content is FrameworkElement)) return;

            // Determine the size of the popup:
            FrameworkElement content = (FrameworkElement)_popup.PopupRoot.Content;
            double popupActualWidth = content.ActualWidth;
            double popupActualHeight = content.ActualHeight;
            if (double.IsNaN(popupActualWidth) || double.IsNaN(popupActualHeight) || popupActualWidth <= 0 || popupActualHeight <= 0) return;


            Point popupPosition = new Point(0, 0);
            if (_popup.IsConnectedToLiveTree)
            {
                popupPosition = _popup.TransformToVisual(Application.Current.RootVisual).Transform(popupPosition);
            }

            //Rect windowBounds = Window.Current.Bounds;
            //double popupX = popup.HorizontalOffset + popupPosition.X;
            double popupY = _popup.VerticalOffset + popupPosition.Y;

            var newValue = MaxDropDownHeight;
            if (double.IsNaN(newValue)) newValue = double.PositiveInfinity;
            var parentWindowHeight = this.INTERNAL_ParentWindow != null ? this.INTERNAL_ParentWindow.INTERNAL_GetActualWidthAndHeight().Height : double.PositiveInfinity;
            if (!double.IsInfinity(parentWindowHeight))
            {
                parentWindowHeight -= popupY;
                var comboHeight = this.INTERNAL_GetActualWidthAndHeight().Height;
                if (!double.IsInfinity(comboHeight) && !double.IsNaN(comboHeight))
                {
                    parentWindowHeight -= comboHeight;
                }
            }

            var minHeight = Math.Min(newValue, parentWindowHeight);

            if (!double.IsInfinity(minHeight) && !double.IsNaN(minHeight))
            {
                _scroller.MaxHeight = minHeight;
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