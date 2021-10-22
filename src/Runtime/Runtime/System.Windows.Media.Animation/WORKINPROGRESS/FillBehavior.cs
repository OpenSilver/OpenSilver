

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
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    //
    // Summary:
    //     Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
    //     its active period but its parent is inside its active or hold period.
    public enum FillBehavior
    {
        //
        // Summary:
        //     After it reaches the end of its active period, the timeline holds its progress
        //     until the end of its parent's active and hold periods.
        HoldEnd = 0,
        //
        // Summary:
        //     The timeline stops if it is outside its active period while its parent is inside
        //     its active period.
        Stop = 1
    }
}
