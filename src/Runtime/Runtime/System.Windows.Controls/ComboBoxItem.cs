

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
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the container for an item in a ListBox control.
    /// </summary>
    public partial class ComboBoxItem : SelectorItem
    {
        public ComboBoxItem()
        {
            this.DefaultStyleKey = typeof(ComboBoxItem);
            //this.Loaded += (sender, e) =>
            //{
            //    UpdateVisualStates();
            //};
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

        protected internal override void HandleIsSelectedChanged(bool oldValue, bool newValue)
        {
            UpdateVisualStates();
        }

#if MIGRATION
        protected override void OnMouseEnter(MouseEventArgs eventArgs)
#else
        protected override void OnPointerEntered(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseEnter(eventArgs);
#else
            base.OnPointerEntered(eventArgs);
#endif
            this.INTERNAL_ParentSelectorControl.NotifyItemMouseEnter(this);
            this.IsMouseOver = true;
            this.UpdateVisualStates();
        }

#if MIGRATION
        protected internal override void OnMouseLeave(MouseEventArgs eventArgs)
#else
        protected internal override void OnPointerExited(PointerRoutedEventArgs eventArgs)
#endif
        {
#if MIGRATION
            base.OnMouseLeave(eventArgs);
#else
            base.OnPointerExited(eventArgs);
#endif
            this.IsMouseOver = false;
            this.UpdateVisualStates();
        }

        internal override void UpdateVisualStates()
        {
            if (!IsEnabled)
            {
                GoToState(VisualStates.StateDisabled);
            }
            else if (IsMouseOver)
            {
                GoToState(VisualStates.StateMouseOver);
            }
            else
            {
                GoToState(VisualStates.StateNormal);
            }

            if (IsSelected)
            {
                GoToState(VisualStates.StateSelected);
            }
            else
            {
                GoToState(VisualStates.StateUnselected);
            }
            if (IsFocused)
            {
                GoToState(VisualStates.StateFocused);
            }
            else
            {
                GoToState(VisualStates.StateUnfocused);
            }
        }
    }
}
