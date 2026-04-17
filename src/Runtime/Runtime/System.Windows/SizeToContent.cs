
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
/// Specifies how a window will automatically size itself to fit the size of its content.
/// Used by the <see cref=" Window.SizeToContent"/> property.
/// </summary>
public enum SizeToContent
{
    /// <summary>
    /// Specifies that a window will not automatically set its size to fit the size of
    /// its content. Instead, the size of a window is determined by other properties,
    /// including <see cref="FrameworkElement.Width"/>, <see cref="FrameworkElement.Height"/>,
    /// <see cref="FrameworkElement.MaxWidth"/>, <see cref="FrameworkElement.MaxHeight"/>,
    /// <see cref="FrameworkElement.MinWidth"/>, and <see cref="FrameworkElement.MinHeight"/>.
    /// </summary>
    Manual = 0,

    /// <summary>
    /// Specifies that a window will automatically set its width to fit the width of its 
    /// content, but not the height.
    /// </summary>
    Width = 1,

    /// <summary>
    /// Specifies that a window will automatically set its height to fit the height of its 
    /// content, but not the width.
    /// </summary>
    Height = 2,

    /// <summary>
    /// Specifies that a window will automatically set both its width and height to fit the 
    /// width and height of its content.
    /// </summary>
    WidthAndHeight = 3,
}
