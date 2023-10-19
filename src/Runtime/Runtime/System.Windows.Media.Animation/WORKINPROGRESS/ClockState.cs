
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
    /// This enumeration represents the different states that a clock can be in
    /// at any given time.
    /// </summary>
    public enum ClockState
    {
        /// <summary>
        /// The clock is currently active meaning that the current time of the
        /// clock changes relative to its parent time. 
        /// </summary>
        Active,

        /// <summary>
        /// The clock is currenty in a fill state meaning that its current time
        /// and progress do not change relative to the parent's time, but the 
        /// clock is not stopped.
        /// </summary>
        Filling,

        /// <summary>
        /// The clock is currently stopped which means that its current time and
        /// current progress property values are undefined. 
        /// </summary>
        Stopped
    }
}
