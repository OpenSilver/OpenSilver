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

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Internal.Controls
{
#if MIGRATION
    public partial class DataPager : Control
#else
    internal partial class DataPager : Control
#endif
    {
        [OpenSilver.NotImplemented]
        public bool AutoEllipsis { get; set; }
    }
}
