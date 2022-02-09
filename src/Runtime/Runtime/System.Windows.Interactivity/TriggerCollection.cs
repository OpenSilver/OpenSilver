

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
using System.Windows.Interactivity;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents a collection of triggers with a shared AssociatedObject and provides change notifications to its contents when that AssociatedObject changes.
    /// </summary>
    public sealed partial class TriggerCollection : AttachableCollection<TriggerBase>
    {
        //Note on this file: see commit 58c52131 of October 30th, 2019 for comments on the modifications from the original source.
        //Based on the code that can be found at https://github.com/jlaanstra/Windows.UI.Interactivity/tree/master/Windows.UI.Interactivity.

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Interactivity.TriggerCollection"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Internal, because this should not be inherited outside this assembly.
        /// </remarks>
        internal TriggerCollection() { }

        /// <summary>
		/// Called immediately after the collection is attached to an AssociatedObject.
		/// </summary>
		protected override void OnAttached()
        {
            foreach (TriggerBase trigger in this)
            {
                trigger.Attach(this.AssociatedObject);
            }
        }

        /// <summary>
		/// Called when the collection is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		protected override void OnDetaching()
        {
            foreach (TriggerBase trigger in this)
            {
                trigger.Detach();
            }
        }
    }
}