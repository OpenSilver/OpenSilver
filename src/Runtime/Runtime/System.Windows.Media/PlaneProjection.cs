
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
using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Represents a perspective transform (a 3-D-like effect) on an object.
/// </summary>
public sealed class PlaneProjection : Projection
{
    // Fixed camera at distance 999: x' = x * 999 / (999 - z), y' = y * 999 / (999 - z).
    // The plane z = 0 is left unchanged. Silverlight magnifies everything by an extra
    // 1000 / 999, except when the projection is identity.
    private static readonly Matrix3D s_camera = new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, -1.0 / 999,
        0, 0, 0, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaneProjection"/> class.
    /// </summary>
    public PlaneProjection() { }

    /// <summary>
    /// Identifies the <see cref="RotationX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationXProperty =
        DependencyProperty.Register(
            nameof(RotationX),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the number of degrees to rotate the object around the x-axis of rotation.
    /// </summary>
    /// <returns>
    /// The number of degrees to rotate the object around the x-axis of rotation. The default is 0.
    /// </returns>
    public double RotationX
    {
        get => (double)GetValue(RotationXProperty);
        set => SetValueInternal(RotationXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RotationY" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationYProperty =
        DependencyProperty.Register(
            nameof(RotationY),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the number of degrees to rotate the object around the y-axis of rotation.
    /// </summary>
    /// <returns>
    /// The number of degrees to rotate the object around the y-axis of rotation. The default is 0.
    /// </returns>
    public double RotationY
    {
        get => (double)GetValue(RotationYProperty);
        set => SetValueInternal(RotationYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="RotationZ" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotationZProperty =
        DependencyProperty.Register(
            nameof(RotationZ),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the number of degrees to rotate the object around the z-axis of rotation.
    /// </summary>
    /// <returns>
    /// The number of degrees to rotate the object around the z-axis of rotation. The default is 0.
    /// </returns>
    public double RotationZ
    {
        get => (double)GetValue(RotationZProperty);
        set => SetValueInternal(RotationZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CenterOfRotationX" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterOfRotationXProperty =
        DependencyProperty.Register(
            nameof(CenterOfRotationX),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.5, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the x-coordinate of the center of rotation of the object you rotate.
    /// </summary>
    /// <returns>
    /// The x-coordinate of the center of rotation of the object you rotate. Typical
    /// values are between 0 and 1 with a value of 0 corresponding to one edge of the
    /// object and 1 to the opposite edge. Values outside this range are allowed and
    /// move the center of rotation accordingly. The default is 0.5 (the center of object).
    /// </returns>
    public double CenterOfRotationX
    {
        get => (double)GetValue(CenterOfRotationXProperty);
        set => SetValueInternal(CenterOfRotationXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CenterOfRotationY" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterOfRotationYProperty =
        DependencyProperty.Register(
            nameof(CenterOfRotationY),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.5, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the y-coordinate of the center of rotation of the object you rotate.
    /// </summary>
    /// <returns>
    /// The y-coordinate of the center of rotation of the object you rotate. Typical
    /// values are between 0 and 1 with a value of 0 corresponding to one edge of the
    /// object and 1 to the opposite edge. Values outside this range are allowed and
    /// move the center of rotation accordingly. The default is 0.5 (the center of object).
    /// </returns>
    public double CenterOfRotationY
    {
        get => (double)GetValue(CenterOfRotationYProperty);
        set => SetValueInternal(CenterOfRotationYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CenterOfRotationZ" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterOfRotationZProperty =
        DependencyProperty.Register(
            nameof(CenterOfRotationZ),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the z-coordinate of the center of rotation of the object you rotate.
    /// </summary>
    /// <returns>
    /// The z-coordinate of the center of rotation of the object you rotate. The default
    /// is 0. Values greater than 0 correspond to coordinates out from the plane of the
    /// object, and negative values correspond to coordinates behind the plane of the
    /// object.
    /// </returns>
    public double CenterOfRotationZ
    {
        get => (double)GetValue(CenterOfRotationZProperty);
        set => SetValueInternal(CenterOfRotationZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LocalOffsetX" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LocalOffsetXProperty =
        DependencyProperty.Register(
            nameof(LocalOffsetX),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the x-axis of the plane
    /// of the object.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the x-axis of the plane of the object.
    /// </returns>
    public double LocalOffsetX
    {
        get => (double)GetValue(LocalOffsetXProperty);
        set => SetValueInternal(LocalOffsetXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LocalOffsetY" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LocalOffsetYProperty =
        DependencyProperty.Register(
            nameof(LocalOffsetY),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the y-axis of the plane
    /// of the object.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the y-axis of the plane of the object.
    /// </returns>
    public double LocalOffsetY
    {
        get => (double)GetValue(LocalOffsetYProperty);
        set => SetValueInternal(LocalOffsetYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LocalOffsetZ" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty LocalOffsetZProperty =
        DependencyProperty.Register(
            nameof(LocalOffsetZ),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the z-axis of the plane
    /// of the object.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the z-axis of the plane of the object.
    /// </returns>
    public double LocalOffsetZ
    {
        get => (double)GetValue(LocalOffsetZProperty);
        set => SetValueInternal(LocalOffsetZProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="GlobalOffsetX" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty GlobalOffsetXProperty =
        DependencyProperty.Register(
            nameof(GlobalOffsetX),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the x-axis of the screen.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the x-axis of the screen.
    /// </returns>
    public double GlobalOffsetX
    {
        get => (double)GetValue(GlobalOffsetXProperty);
        set => SetValueInternal(GlobalOffsetXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="GlobalOffsetY" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty GlobalOffsetYProperty =
        DependencyProperty.Register(
            nameof(GlobalOffsetY),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the y-axis of the screen.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the y-axis of the screen.
    /// </returns>
    public double GlobalOffsetY
    {
        get => (double)GetValue(GlobalOffsetYProperty);
        set => SetValueInternal(GlobalOffsetYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="GlobalOffsetZ" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty GlobalOffsetZProperty =
        DependencyProperty.Register(
            nameof(GlobalOffsetZ),
            typeof(double),
            typeof(PlaneProjection),
            new PropertyMetadata(0.0, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the distance the object is translated along the z-axis of the screen.
    /// </summary>
    /// <returns>
    /// The distance the object is translated along the z-axis of the screen.
    /// </returns>
    public double GlobalOffsetZ
    {
        get => (double)GetValue(GlobalOffsetZProperty);
        set => SetValueInternal(GlobalOffsetZProperty, value);
    }

    private static readonly DependencyPropertyKey ProjectionMatrixPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ProjectionMatrix),
            typeof(Matrix3D),
            typeof(PlaneProjection),
            new PropertyMetadata(Matrix3D.Identity));

    /// <summary>
    /// Identifies the <see cref="ProjectionMatrix" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProjectionMatrixProperty = ProjectionMatrixPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the projection matrix on the <see cref="PlaneProjection"/>.
    /// </summary>
    /// <returns>
    /// The projection matrix of the <see cref="PlaneProjection"/>. The default value is null.
    /// </returns>
    public Matrix3D ProjectionMatrix
    {
        get => (Matrix3D)GetValue(ProjectionMatrixProperty);
        private set => SetValueInternal(ProjectionMatrixPropertyKey, value);
    }

    internal override Matrix3D GetProjectionMatrix(Size renderSize)
    {
        Matrix3D matrix = GetPlaneMatrix(renderSize);
        ProjectionMatrix = matrix;

        double halfWidth = renderSize.Width / 2;
        double halfHeight = renderSize.Height / 2;

        return Matrix3DUtil.CreateTranslation(-halfWidth, -halfHeight, 0)
            * matrix
            * s_camera
            * Matrix3DUtil.CreateTranslation(halfWidth, halfHeight, 0);
    }

    /// <summary>
    /// Gets the matrix of the rotations and offsets, in a space whose origin is the center
    /// of the element (y-axis downward, z-axis toward the viewer).
    /// </summary>
    private Matrix3D GetPlaneMatrix(Size renderSize)
    {
        double cx = (CenterOfRotationX - 0.5) * renderSize.Width;
        double cy = (CenterOfRotationY - 0.5) * renderSize.Height;
        double cz = CenterOfRotationZ;

        return Matrix3DUtil.CreateTranslation(LocalOffsetX - cx, LocalOffsetY - cy, LocalOffsetZ - cz)
            * Matrix3DUtil.CreateRotationX(ToRadians(RotationX))
            * Matrix3DUtil.CreateRotationY(ToRadians(RotationY))
            * Matrix3DUtil.CreateRotationZ(ToRadians(RotationZ))
            * Matrix3DUtil.CreateTranslation(cx + GlobalOffsetX, cy + GlobalOffsetY, cz + GlobalOffsetZ);
    }

    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
}
