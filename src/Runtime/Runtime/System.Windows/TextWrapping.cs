
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

namespace System.Windows;

/// <summary>
/// Specifies whether text wraps when it reaches the edge of its container.
/// </summary>
public enum TextWrapping
{
    /// <summary>
    /// Line-breaking occurs if the line overflows beyond the available block width.
    /// However, a line may overflow beyond the block width if the line breaking algorithm
    /// cannot determine a line break opportunity, as in the case of a very long word
    /// constrained in a fixed-width container with no scrolling allowed.
    /// </summary>
    WrapWithOverflow = 0,

    /// <summary>
    /// No line wrapping is performed.
    /// </summary>
    NoWrap = 1,

    /// <summary>
    /// Line-breaking occurs if the line overflows beyond the available block width,
    /// even if the standard line breaking algorithm cannot determine any line break
    /// opportunity, as in the case of a very long word constrained in a fixed-width
    /// container with no scrolling allowed.
    /// </summary>
    Wrap = 2,
}