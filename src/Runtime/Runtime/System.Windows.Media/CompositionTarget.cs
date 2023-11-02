
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

using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media;

/// <summary>
/// Represents the display surface of a Silverlight-based application.
/// </summary>
public static class CompositionTarget
{
    /// <summary>
    /// Occurs when the core Silverlight rendering process renders a frame.
    /// </summary>
    public static event EventHandler Rendering
    {
        add => AnimationManager.Current.RequestAnimationFrame += value;
        remove => AnimationManager.Current.RequestAnimationFrame -= value;
    }
}
