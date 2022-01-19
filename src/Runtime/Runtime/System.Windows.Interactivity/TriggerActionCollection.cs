

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

#if MIGRATION

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents a collection of actions with a shared AssociatedObject and provides change notifications to its contents when that AssociatedObject changes.
    /// 
    /// </summary>
    public partial class TriggerActionCollection : AttachableCollection<TriggerAction>
    {
        //Note on this file: see commit 58c52131 of October 30th, 2019 for comments on the modifications from the original source.
        //Based on the code that can be found at https://github.com/jlaanstra/Windows.UI.Interactivity/tree/master/Windows.UI.Interactivity.

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.TriggerActionCollection"/> class.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Internal, because this should not be inherited outside this assembly.
        /// </remarks>
        internal TriggerActionCollection()
        {
        }

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            foreach (TriggerAction triggerAction in this)
            {
                triggerAction.Attach((FrameworkElement)this.AssociatedObject); //CSHTML5 added cast.
            }
        }

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (TriggerAction triggerAction in this)
            {
                triggerAction.Detach();
            }
        }

        /// <summary>
        /// Called when a new item is added to the collection.
        /// 
        /// </summary>
        /// <param name="item">The new item.</param>
        internal void ItemAdded(TriggerAction item) //This is supposed to be an override but we changed the heritage
        {
            if (item.IsHosted)
            {
                throw new InvalidOperationException("Cannot Host TriggerAction Multiple Times");
            }
            if (this.AssociatedObject != null)
            {
                item.Attach((FrameworkElement)this.AssociatedObject); //CSHTML5 added cast.
            }
            item.IsHosted = true;
        }

        /// <summary>
        /// Called when an item is removed from the collection.
        /// 
        /// </summary>
        /// <param name="item">The removed item.</param>
        internal void ItemRemoved(TriggerAction item) //This is supposed to be an override but we changed the heritage
        {
            if (((IAttachedObject)item).AssociatedObject != null)
            {
                item.Detach();
            }
            item.IsHosted = false;
        }
    }
}

#endif