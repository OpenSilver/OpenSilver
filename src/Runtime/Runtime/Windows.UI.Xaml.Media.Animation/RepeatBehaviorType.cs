

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
    /// <summary>
    /// Specifies the repeat mode that a Windows.UI.Xaml.Media.Animation.RepeatBehavior
    /// raw value represents.
    /// </summary>
    public enum RepeatBehaviorType
    {
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat for a fixed number of complete runs.
        /// </summary>
        Count = 0,
        //// <summary>
        //// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        //// the timeline should repeat for a time duration, which might result in an
        //// animation terminating part way through.
        //// </summary>
        //Duration = 1, //todo: implement the duration for repeatBehavior.
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat indefinitely.
        /// </summary>
        Forever = 2,
    }
}
