
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

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Describes how a <see cref="Popup"/> control animates when it opens.
/// </summary>
public enum PopupAnimation
{
    /// <summary>
    /// The <see cref="Popup"/> control appears without animation.
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="Popup"/> control gradually appears, or fades in. This effect is created 
    /// by increasing the opacity of the <see cref="Popup"/> window over time.
    /// </summary>
    Fade = 1,

    /// <summary>
    /// The <see cref="Popup"/> control slides down or up into place. By default, a <see cref="Popup"/> 
    /// slides down. However, if the screen does not provide enough room for the <see cref="Popup"/>
    /// to slide down, it slides up instead.
    /// </summary>
    Slide = 2,

    /// <summary>
    /// The <see cref="Popup"/> control scrolls from the upper-left corner of its parent. If the 
    /// screen does not provide enough room to allow the <see cref="Popup"/> default behavior, the 
    /// <see cref="Popup"/> scrolls from the lower-right corner instead.
    /// </summary>
    Scroll = 3,
}
