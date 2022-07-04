
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

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
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
    /// Represents a selectable item in a <see cref="ListBox"/>.
    /// </summary>
    [TemplateVisualState(Name = "SelectedUnfocused", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "BeforeUnloaded", GroupName = "LayoutStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "BeforeLoaded", GroupName = "LayoutStates")]
    [TemplateVisualState(Name = "AfterLoaded", GroupName = "LayoutStates")]
    public partial class ListBoxItem : SelectorItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItem"/> class.
        /// </summary>
        public ListBoxItem()
        {
            DisableBaseControlHandlingOfVisualStates = true;
            IsEnabledChanged += (o, e) => UpdateVisualStates();
            DefaultStyleKey = typeof(ListBoxItem);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty IsSelectedProperty =
            SelectorItem.IsSelectedProperty;

        /// <summary>
        /// Gets or sets a value that indicates whether a <see cref="ListBoxItem"/>
        /// is selected.
        /// </summary>
        /// <returns>
        /// true if the item is selected; otherwise, false. The default is false.
        /// </returns>
        public new bool IsSelected
        {
            get { return base.IsSelected; }
            set { base.IsSelected = value; }
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.UpdateVisualStates();
        }

#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
            if (!e.Handled)
            {
                ListBox parent = ParentListBox;
                if (parent != null)
                {
                    parent.NotifyListItemClicked(this);
                }
            }

#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif
        }

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
            if (this.ParentSelector != null)
            {
                this.ParentSelector.NotifyItemMouseEnter(this);
            }

            this.IsMouseOver = true;
            this.UpdateVisualStates();
        }

#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs e)
#else
        protected internal override void OnPointerExited(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
            this.IsMouseOver = false;
            this.UpdateVisualStates();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            this.IsFocused = true;
            this.UpdateVisualStates();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this.IsFocused = false;
            this.UpdateVisualStates();
        }

        internal ListBox ParentListBox
        {
            get
            {
                return ParentSelector as ListBox;
            }
        }

        internal override void UpdateVisualStates()
        {
            // Change to the correct state in the Interaction group
            if (!IsEnabled)
            {
                // [copied from SL code]
                // If our child is a control then we depend on it displaying a proper "disabled" state.  If it is not a control
                // (ie TextBlock, Border, etc) then we will use our visuals to show a disabled state.
                VisualStateManager.GoToState(this, Content is Control ? VisualStates.StateNormal : VisualStates.StateDisabled, false);
            }
            else if (IsMouseOver)
            {
                VisualStateManager.GoToState(this, VisualStates.StateMouseOver, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, false);
            }

            // Change to the correct state in the Selection group
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, VisualStates.StateSelected, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnselected, false);
            }

            if (IsFocused)
            {
                VisualStateManager.GoToState(this, VisualStates.StateFocused, false);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateUnfocused, false);
            }
        }

        internal bool IsMouseOver { get; private set; }

        internal bool IsFocused { get; private set; }
    }
}
