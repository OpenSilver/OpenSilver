
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

using System.Windows;

namespace OpenSilver.Internal;

internal sealed class TriggerStorage
{
    internal StyleTriggerStorage StyleTriggers { get; set; }

    internal StyleTriggerStorage ThemeStyleTriggers { get; set; }

    internal TemplateTriggerStorage TemplateTriggers { get; set; }

    internal void OnPropertyChanged(DependencyProperty dp)
    {
        StyleTriggers?.OnPropertyChanged(dp);
        TemplateTriggers?.OnPropertyChanged(dp);
        ThemeStyleTriggers?.OnPropertyChanged(dp);
    }
}
