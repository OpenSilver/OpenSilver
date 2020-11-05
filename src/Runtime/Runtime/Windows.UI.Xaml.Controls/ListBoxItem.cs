

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
    public partial class ListBoxItem : SelectorItem
    {
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
                            if (!INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(parent.SelectedItems, this.INTERNAL_CorrespondingItem))
                            {
                                INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Add(parent.SelectedItems, this.INTERNAL_CorrespondingItem);
                            }
                        }
                        else
                        {
                            if (INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Contains(parent.SelectedItems, this.INTERNAL_CorrespondingItem))
                            {
                                INTERNAL_WorkaroundObservableCollectionBugWithJSIL.Remove(parent.SelectedItems, this.INTERNAL_CorrespondingItem);
                            }
                        }

                    }
                }
            }
        }
    }
}
