

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
#if MIGRATION
using System.Windows.Controls.Primitives;
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
    public partial class ListBoxItem : SelectorItem
    {
        public ListBoxItem()
        {
            this.DefaultStyleKey = typeof(ListBoxItem);
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
            base.HandleIsSelectedChanged(oldValue, newValue);
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //todo: remove what's inside the "if" above once we will have defined a default style for the ItemsControl, that does about the same thing.
                if (INTERNAL_ParentSelectorControl != null)
                {
                    if (INTERNAL_ParentSelectorControl is MultiSelector)
                    {
                        var parent = (MultiSelector)INTERNAL_ParentSelectorControl;
                        if (newValue)
                        {
                            if (!parent.SelectedItems.Contains(this.INTERNAL_CorrespondingItem))
                            {
                                parent.SelectedItems.Add(this.INTERNAL_CorrespondingItem);
                            }
                        }
                        else
                        {
                            if (parent.SelectedItems.Contains(this.INTERNAL_CorrespondingItem))
                            {
                                parent.SelectedItems.Remove(this.INTERNAL_CorrespondingItem);
                            }
                        }

                    }
                }
            }
        }
    }
}
