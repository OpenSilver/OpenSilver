
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

using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualStates();
        }

        /// <summary>
        /// Returns a <see cref="ListBoxItemAutomationPeer"/> for the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="ListBoxItemAutomationPeer"/> for the <see cref="ListBoxItem"/>.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new ListBoxItemAutomationPeer(this);

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                Focus();
                ListBox parent = ParentListBox;
                if (parent != null)
                {
                    parent.NotifyListItemClicked(this);
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (this.ParentSelector != null)
            {
                this.ParentSelector.NotifyItemMouseEnter(this);
            }

            this.IsMouseOver = true;
            this.UpdateVisualStates();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            this.UpdateVisualStates();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            this.IsFocused = true;
            this.UpdateVisualStates();
            
            ParentSelector?.NotifyListItemGotFocus(this);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this.IsFocused = false;
            this.UpdateVisualStates();

            ParentSelector?.NotifyListItemLostFocus(this);
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
                if (IsFocused)
                {
                    VisualStateManager.GoToState(this, VisualStates.StateSelected, false);                    
                }
                else
                {
                    VisualStates.GoToState(this, false, VisualStates.StateSelectedUnfocused, VisualStates.StateSelected);
                }               
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
