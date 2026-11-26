
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

using System.Windows.Media.Media3D;

namespace System.Windows.Media;

/// <summary>
/// Provides a base class for projections, which describe how to transform an object
/// in 3-D space using perspective transforms.
/// </summary>
public abstract class Projection : DependencyObject
{
    internal Projection() { }

    /// <summary>
    /// Gets the matrix that maps the local coordinates of an element whose render size is
    /// <paramref name="renderSize"/> to its projected coordinates, still in the local space
    /// of the element.
    /// </summary>
    /// <remarks>
    /// Must be called during layout, when the visual transform of the element is computed.
    /// </remarks>
    internal abstract Matrix3D GetProjectionMatrix(Size renderSize);

    internal event EventHandler Changed;

    internal static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Projection)d).Changed?.Invoke(d, EventArgs.Empty);
    }
}
