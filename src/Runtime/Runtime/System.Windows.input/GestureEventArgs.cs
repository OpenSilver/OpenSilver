
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Provides event data for gesture events such as <see cref="UIElement.DoubleTap"/>.
/// </summary>
[OpenSilver.NotImplemented]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GestureEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
    /// </summary>
    public GestureEventArgs() { }

    /// <summary>
    /// Returns the x and y coordinates of the pointer position, optionally evaluated against a coordinate 
    /// origin of a supplied <see cref="UIElement"/>.
    /// </summary>
    /// <param name="relativeTo">
    /// Any <see cref="UIElement"/> derived object that is contained by the Silverlight plug-in and connected 
    /// to the object tree. To specify the object relative to the overall Silverlight coordinate system, pass null.
    /// </param>
    /// <returns>
    /// A <see cref="Point"/> that represents the current x and y coordinates of the mouse pointer position. If 
    /// null was passed as relativeTo, this coordinate is for the overall Silverlight plug-in content area. If a 
    /// non-null relativeTo was passed, this coordinate is relative to the object referenced by relativeTo.
    /// </returns>
    public Point GetPosition(UIElement relativeTo) => new Point(0, 0);
}
