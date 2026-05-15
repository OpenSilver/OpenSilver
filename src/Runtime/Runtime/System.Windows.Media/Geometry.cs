
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

using OpenSilver.Internal;
using OpenSilver.Internal.Media;
using OpenSilver.Internal.Media.Geometry.Core;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Shapes;

namespace System.Windows.Media;

/// <summary>
/// Provides a base class for objects that define geometric shapes. <see cref="Geometry"/>
/// objects can be used for clipping regions and as geometry definitions for rendering
/// two-dimensional graphic data as a <see cref="Path"/>.
/// </summary>
[TypeConverter(typeof(GeometryConverter))]
public abstract class Geometry : DependencyObject
{
    private static readonly PathGeometryData _emptyPathGeometryData = MakeEmptyPathGeometryData();

    internal Geometry() { }

    /// <summary>
    /// Creates a new <see cref="Geometry"/> instance from the specified string using the current culture.
    /// </summary>
    /// <param name="source">
    /// A string that describes the geometry to be created.
    /// </param>
    /// <returns>
    /// A new <see cref="Geometry"/> instance created from the specified string.
    /// </returns>
    public static Geometry Parse(string source) => Parsers.ParseGeometry(source, CultureInfo.InvariantCulture);

    /// <summary>
    /// Combines the two geometries using the specified <see cref="GeometryCombineMode"/> and tolerance 
    /// factor, and applies the specified transform to the resulting geometry.
    /// </summary>
    /// <param name="geometry1">
    /// The first geometry to combine.
    /// </param>
    /// <param name="geometry2">
    /// The second geometry to combine.
    /// </param>
    /// <param name="mode">
    /// One of the enumeration values that specifies how the geometries are combined.
    /// </param>
    /// <param name="transform">
    /// A transformation to apply to the combined geometry, or null.
    /// </param>
    /// <param name="tolerance">
    /// The maximum bounds on the distance between points in the polygonal approximation of the geometries.
    /// Smaller values produce more accurate results but cause slower execution. If tolerance is less than 
    /// .000001, .000001 is used instead.
    /// </param>
    /// <param name="type">
    /// One of the <see cref="ToleranceType"/> values that specifies whether the tolerance factor is an 
    /// absolute value or relative to the area of the geometry.
    /// </param>
    /// <returns>
    /// The combined geometry.
    /// </returns>
    public static PathGeometry Combine(
        Geometry geometry1,
        Geometry geometry2,
        GeometryCombineMode mode,
        Transform transform,
        double tolerance,
        ToleranceType type)
    {
        return PathGeometry.InternalCombine(
            geometry1,
            geometry2,
            mode,
            transform,
            tolerance,
            type);
    }

    /// <summary>
    /// Combines the two geometries using the specified <see cref="GeometryCombineMode"/> and applies 
    /// the specified transform to the resulting geometry.
    /// </summary>
    /// <param name="geometry1">
    /// The first geometry to combine.
    /// </param>
    /// <param name="geometry2">
    /// The second geometry to combine.
    /// </param>
    /// <param name="mode">
    /// One of the enumeration values that specifies how the geometries are combined.
    /// </param>
    /// <param name="transform">
    /// A transformation to apply to the combined geometry, or null.
    /// </param>
    /// <returns>
    /// The combined geometry.
    /// </returns>
    public static PathGeometry Combine(
        Geometry geometry1,
        Geometry geometry2,
        GeometryCombineMode mode,
        Transform transform)
    {
        return PathGeometry.InternalCombine(
            geometry1,
            geometry2,
            mode,
            transform,
            StandardFlatteningTolerance,
            ToleranceType.Absolute);
    }

    /// <summary>
    /// Gets an empty geometry object.
    /// </summary>
    /// <returns>
    /// The empty geometry object.
    /// </returns>
    public static Geometry Empty => new StreamGeometry();

    /// <summary>
    /// Identifies the <see cref="Transform"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TransformProperty =
        DependencyProperty.Register(
            nameof(Transform),
            typeof(Transform),
            typeof(Geometry),
            new PropertyMetadata(Transform.Identity));

    /// <summary>
    /// Gets or sets the <see cref="Media.Transform"/> object applied to a <see cref="Geometry"/>.
    /// </summary>
    /// <returns>
    /// The transformation applied to the <see cref="Geometry"/>. Note that this
    /// value may be a single <see cref="Media.Transform"/> or a <see cref="TransformCollection"/>
    /// cast as a <see cref="Media.Transform"/>.
    /// </returns>
    public Transform Transform
    {
        get => (Transform)GetValue(TransformProperty);
        set => SetValueInternal(TransformProperty, value);
    }

    /// <summary>
    /// Gets the standard tolerance used for polygonal approximation.
    /// </summary>
    /// <returns>
    /// The standard tolerance. The default value is 0.25.
    /// </returns>
    public static double StandardFlatteningTolerance { get; } = Utils.DEFAULT_FLATTENING_TOLERANCE;

    /// <summary>
    /// Gets a <see cref="Rect"/> that specifies the axis-aligned bounding box of the
    /// <see cref="Geometry"/>.
    /// </summary>
    /// <returns>
    /// The axis-aligned bounding box of the <see cref="Geometry"/>.
    /// </returns>
    public Rect Bounds => GetBoundsInternal();

    /// <summary>
    /// Determines whether the object is empty.
    /// </summary>
    /// <returns>
    /// true if the geometry is empty; otherwise, false.
    /// </returns>
    public abstract bool IsEmpty();

    /// <summary>
    /// Determines whether the object might have curved segments.
    /// </summary>
    /// <returns>
    /// true if the geometry object might have curved segments; otherwise, false.
    /// </returns>
    public abstract bool MayHaveCurves();

    internal static PathGeometryData GetEmptyPathGeometryData() => _emptyPathGeometryData;

    // This method is used for eliminating unnecessary work when the geometry is obviously empty.
    // For most Geometry types the definite IsEmpty() query is just as cheap.  The exceptions will 
    // be CombinedGeometry and GeometryGroup.
    internal virtual bool IsObviouslyEmpty() => IsEmpty();

    internal virtual Rect GetBoundsInternal()
    {
        // Do not skip non-fillable figures
        return GetPathBounds(GetPathGeometryData(), Matrix.Identity, false);
    }

    /// <summary>
    /// Gets the bounds of this PathGeometry as an axis-aligned bounding box with pen and/or transform
    /// </summary>
    internal static Rect GetPathBounds(PathGeometryData pathData, Matrix worldMatrix, bool skipHollows)
    {
        if (pathData.IsEmpty())
        {
            return Rect.Empty;
        }

        var shape = new PathGeometryWrapper(pathData.SerializedData, pathData.FillRule, pathData.Matrix);

        Rect bounds = shape.GetTightBounds(worldMatrix, skipHollows);

        if (double.IsNaN(bounds.X) || double.IsNaN(bounds.Y) || double.IsNaN(bounds.Width) || double.IsNaN(bounds.Height))
        {
            return Rect.Empty;
        }

        return bounds;
    }

    internal event EventHandler<GeometryInvalidatedEventsArgs> Invalidated;

    internal static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Geometry geometry = (Geometry)d;
        geometry.RaisePathChanged();
    }

    internal void RaisePathChanged() => Invalidated?.Invoke(this, GeometryInvalidatedEventsArgs.AffectsMeasureArgs);

    internal static void OnFillRuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Geometry geometry = (Geometry)d;
        geometry.RaiseFillRuleChanged();
    }

    private void RaiseFillRuleChanged() => Invalidated?.Invoke(this, GeometryInvalidatedEventsArgs.AffectsFillRuleArgs);

    internal string ToPathData(IFormatProvider formatProvider)
    {
        using var context = new StringStreamGeometryContext(formatProvider);
        SerializeData(context, Matrix.Identity);
        return context.ToString();
    }

    internal abstract void SerializeData(CapacityStreamGeometryContext context, Matrix transform);

    /// <summary>
    /// GetPathGeometryData - returns a struct which contains this Geometry represented
    /// as a path geometry's serialized format.
    /// </summary>
    internal virtual PathGeometryData GetPathGeometryData()
    {
        if (IsObviouslyEmpty())
        {
            return GetEmptyPathGeometryData();
        }

        var data = new PathGeometryData
        {
            FillRule = GetFillRule(),
            Matrix = Matrix.Identity,
        };

        var context = new ByteStreamGeometryContext();
        SerializeData(context, Matrix.Identity);
        context.Close();

        data.SerializedData = context.GetData();

        return data;
    }

    internal virtual FillRule GetFillRule() => FillRule.EvenOdd;

    internal Matrix GetCombinedMatrix(Matrix transform)
    {
        Matrix matrix = Matrix.Identity;

        if (Transform is Transform internalTransform && !Transform.IsIdentityTransform(internalTransform))
        {
            matrix = internalTransform.Matrix;

            if (!transform.IsIdentity)
            {
                matrix *= transform;
            }
        }
        else if (!transform.IsIdentity)
        {
            matrix = transform;
        }

        return matrix;
    }

    private static PathGeometryData MakeEmptyPathGeometryData()
    {
        int size = Unsafe.SizeOf<MIL_PATHGEOMETRY>();

        var data = new PathGeometryData
        {
            FillRule = FillRule.EvenOdd,
            Matrix = Matrix.Identity,
            SerializedData = new byte[size],
        };

        // implicitly set pPathGeometry.Flags = 0;
        var pPathGeometry = new MIL_PATHGEOMETRY
        {
            FigureCount = 0,
            Size = (uint)size,
        };

        MemoryMarshal.Write(data.SerializedData, ref pPathGeometry);

        return data;
    }

    internal struct PathGeometryData
    {
        internal readonly bool IsEmpty()
        {
            if (SerializedData is null || SerializedData.Length <= 0)
            {
                return true;
            }

            MIL_PATHGEOMETRY pPathGeometry = MemoryMarshal.Read<MIL_PATHGEOMETRY>(SerializedData);
            return pPathGeometry.FigureCount <= 0;
        }

        internal FillRule FillRule;
        internal Matrix Matrix;
        internal byte[] SerializedData;

        internal readonly uint Size
        {
            get
            {
                if (SerializedData is null || SerializedData.Length <= 0)
                {
                    return 0;
                }

                MIL_PATHGEOMETRY pPathGeometryData = MemoryMarshal.Read<MIL_PATHGEOMETRY>(SerializedData);
                uint size = pPathGeometryData.Size;

                Debug.Assert(size <= (uint)SerializedData.Length);

                return size;
            }
        }
    }
}

internal class GeometryInvalidatedEventsArgs : EventArgs
{
    public GeometryInvalidatedEventsArgs(bool affectsMeasure, bool affectsFillRule)
    {
        AffectsMeasure = affectsMeasure;
        AffectsFillRule = affectsFillRule;
    }

    public static GeometryInvalidatedEventsArgs AffectsMeasureArgs { get; } = new(true, false);

    public static GeometryInvalidatedEventsArgs AffectsFillRuleArgs { get; } = new(false, true);

    public bool AffectsMeasure { get; }

    public bool AffectsFillRule { get; }
}
