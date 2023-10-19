
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Specifies the repeat mode that a <see cref="RepeatBehavior"/>
    /// raw value represents.
    /// </summary>
    public enum RepeatBehaviorType
    {
        /// <summary>
        /// The <see cref="RepeatBehavior"/> represents a case where
        /// the timeline should repeat for a fixed number of complete runs.
        /// </summary>
        Count = 0,
        /// <summary>
        /// The <see cref="RepeatBehavior"/>  represents a case where
        /// the timeline should repeat for a time duration, which might result in an
        /// animation terminating part way through.
        /// </summary>
        Duration = 1,
        /// <summary>
        /// The <see cref="RepeatBehavior"/> represents a case where
        /// the timeline should repeat indefinitely.
        /// </summary>
        Forever = 2,
    }
}
