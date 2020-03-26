

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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Defines functionality that enables transformations in a two-dimensional plane.
    /// </summary>
    public abstract partial class Transform : GeneralTransform
    {
        // Must be implemented by the concrete class:
        internal abstract void INTERNAL_ApplyTransform();
        internal abstract void INTERNAL_UnapplyTransform();
        internal abstract void INTERNAL_ApplyCSSChanges();
        internal abstract void INTERNAL_UnapplyCSSChanges();
    }
}
