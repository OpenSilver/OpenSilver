
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

namespace System.Windows.Media.Animation;

/// <summary>
/// Describes the potential states of an animation.
/// </summary>
public enum ClockState
{
    /// <summary>
    /// The current animation changes in direct relation to that of its parent.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The animation continues and does not change in relation to that of its parent.
    /// </summary>
    Filling = 1,

    /// <summary>
    /// The animation is stopped.
    /// </summary>
    Stopped = 2,
}
