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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a collection of behaviors with a shared AssociatedObject and provides
    /// change notifications to its contents when that AssociatedObject changes.
    /// </summary>
    public sealed partial class BehaviorCollection : AttachableCollection<Behavior>
    {
        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached() { }

        
        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but
        /// before it has actually occurred.
        /// </summary>
        protected override void OnDetaching() { }
    }
}