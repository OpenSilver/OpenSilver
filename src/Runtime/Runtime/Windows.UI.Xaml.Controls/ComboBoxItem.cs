

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
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
            base.Loaded += (sender, e) =>
            {
                UpdateVisualStates();
            };

            base.DefaultStyleKey = typeof(ComboBoxItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateVisualStates();
        }

        protected internal override void HandleIsSelectedChanged(bool oldValue, bool newValue)
        {
            UpdateVisualStates();
        }

        protected override void OnMouseEnter(MouseEventArgs eventArgs)
        {
            base.OnMouseEnter(eventArgs);
            this.INTERNAL_ParentSelectorControl.NotifyItemMouseEnter(this);
            this.IsMouseOver = true;
            this.UpdateVisualStates();
        }

        protected internal override void OnMouseLeave(MouseEventArgs eventArgs)
        {
            base.OnMouseLeave(eventArgs);
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
