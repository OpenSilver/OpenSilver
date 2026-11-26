
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

using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace System.Windows.Media;

/// <summary>
/// Enables you to apply a <see cref="Matrix3D"/> to an object.
/// </summary>
[ContentProperty(nameof(ProjectionMatrix))]
public sealed class Matrix3DProjection : Projection
{
    /// <summary>
    /// Initializes a new instance of a <see cref="Matrix3DProjection"/> class.
    /// </summary>
    public Matrix3DProjection() { }

    /// <summary>
    /// Identifies the <see cref="ProjectionMatrix"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProjectionMatrixProperty =
        DependencyProperty.Register(
            nameof(ProjectionMatrix),
            typeof(Matrix3D),
            typeof(Matrix3DProjection),
            new PropertyMetadata(Matrix3D.Identity, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the <see cref="Matrix3D"/> that is used for the projection that is applied to 
    /// the object.
    /// </summary>
    /// <returns>
    /// The <see cref="Matrix3D"/> that is used for the projection that is applied to the object.
    /// </returns>
    public Matrix3D ProjectionMatrix
    {
        get => (Matrix3D)GetValue(ProjectionMatrixProperty);
        set => SetValueInternal(ProjectionMatrixProperty, value);
    }

    internal override Matrix3D GetProjectionMatrix(Size renderSize) => ProjectionMatrix;
}
