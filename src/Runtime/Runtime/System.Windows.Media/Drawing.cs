
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

namespace System.Windows.Media;

/// <summary>
/// Abstract class that describes a 2-D drawing. This class cannot be inherited by your code.
/// </summary>
[OpenSilver.NotImplemented]
public abstract class Drawing : DependencyObject
{
    internal Drawing() { }

    /// <summary>
    /// Gets the axis-aligned bounds of the drawing's contents.
    /// </summary>
    /// <returns>
    /// The axis-aligned bounds of the drawing's contents.
    /// </returns>
    public Rect Bounds => Rect.Empty;
}
