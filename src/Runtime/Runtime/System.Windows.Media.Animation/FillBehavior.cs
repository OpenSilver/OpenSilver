
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
/// Specifies how a <see cref="Timeline"/> behaves when it is outside its active period 
/// but its parent is inside its active or hold period.
/// </summary>
public enum FillBehavior
{
    /// <summary>
    /// After it reaches the end of its active period, the timeline holds its progress
    /// until the end of its parent's active and hold periods.
    /// </summary>
    HoldEnd = 0,

    /// <summary>
    /// The timeline stops if it is outside its active period while its parent is inside
    /// its active period.
    /// </summary>
    Stop = 1,
}
