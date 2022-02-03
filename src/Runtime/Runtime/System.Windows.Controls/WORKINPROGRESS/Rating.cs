
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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A control that has a rating.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class Rating : ItemsControl, IUpdateVisualState
    {
        [OpenSilver.NotImplemented]
        public void UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }
    }
}
