
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

namespace System.Windows;

/// <summary>
/// These EventArgs are used to pass additional information during a ResourcesChanged event
/// </summary>
internal sealed class ResourcesChangedEventArgs : EventArgs
{
    internal ResourcesChangedEventArgs(ResourcesChangeInfo info)
    {
        Info = info;
    }

    internal ResourcesChangeInfo Info { get; }
}
