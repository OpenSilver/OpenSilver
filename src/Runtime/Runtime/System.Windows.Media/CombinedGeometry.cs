
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

namespace System.Windows.Media;

/// <summary>
/// Represents a 2-D geometric shape defined by the combination of two <see cref="Geometry"/> objects.
/// </summary>
public sealed class CombinedGeometry : Geometry
{
    private byte[] _data;
    private WeakEventToken _weakGeometry1InvalidatedToken;
    private WeakEventToken _weakGeometry2InvalidatedToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedGeometry"/> class.
    /// </summary>
    public CombinedGeometry() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedGeometry"/> class with the specified 
    /// <see cref="Geometry"/> objects.
    /// </summary>
    /// <param name="geometry1">
    /// The first <see cref="Geometry"/> to combine.
    /// </param>
    /// <param name="geometry2">
    /// The second <see cref="Geometry"/> to combine.
    /// </param>
    public CombinedGeometry(Geometry geometry1, Geometry geometry2)
    {
        Geometry1 = geometry1;
        Geometry2 = geometry2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedGeometry"/> class with the specified 
    /// <see cref="Geometry"/> objects and <see cref="GeometryCombineMode"/>.
    /// </summary>
    /// <param name="geometryCombineMode">
    /// The method by which geometry1 and geometry2 are combined.
    /// </param>
    /// <param name="geometry1">
    /// The first <see cref="Geometry"/> to combine.
    /// </param>
    /// <param name="geometry2">
    /// The second <see cref="Geometry"/> to combine.
    /// </param>
    public CombinedGeometry(GeometryCombineMode geometryCombineMode, Geometry geometry1, Geometry geometry2)
    {
        GeometryCombineMode = geometryCombineMode;
        Geometry1 = geometry1;
        Geometry2 = geometry2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedGeometry"/> class with the specified 
    /// <see cref="Geometry"/> objects, <see cref="GeometryCombineMode"/>, and <see cref="Transform"/>.
    /// </summary>
    /// <param name="geometryCombineMode">
    /// The method by which geometry1 and geometry2 are combined.
    /// </param>
    /// <param name="geometry1">
    /// The first <see cref="Geometry"/> to combine.
    /// </param>
    /// <param name="geometry2">
    /// The second <see cref="Geometry"/> to combine.
    /// </param>
    /// <param name="transform">
    /// The <see cref="Transform"/> applied to the <see cref="CombinedGeometry"/>.
    /// </param>
    public CombinedGeometry(GeometryCombineMode geometryCombineMode, Geometry geometry1, Geometry geometry2, Transform transform)
    {
        GeometryCombineMode = geometryCombineMode;
        Geometry1 = geometry1;
        Geometry2 = geometry2;
        Transform = transform;
    }

    /// <summary>
    /// Identifies the <see cref="Geometry1"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Geometry1Property =
        DependencyProperty.Register(
            nameof(Geometry1),
            typeof(Geometry),
            typeof(CombinedGeometry),
            new PropertyMetadata(null, OnGeometry1Changed));

    /// <summary>
    /// Gets or sets the first <see cref="Geometry"/> object of this <see cref="CombinedGeometry"/> object.
    /// </summary>
    /// <returns>
    /// The first <see cref="Geometry"/> object to combine.
    /// </returns>
    public Geometry Geometry1
    {
        get => (Geometry)GetValue(Geometry1Property);
        set => SetValueInternal(Geometry1Property, value);
    }

    private static void OnGeometry1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var cg = (CombinedGeometry)d;

        if (cg._weakGeometry1InvalidatedToken is not null)
        {
            cg._weakGeometry1InvalidatedToken.Dispose();
            cg._weakGeometry1InvalidatedToken = null;
        }

        if (e.NewValue is Geometry geometry)
        {
            cg._weakGeometry1InvalidatedToken = WeakEvent.Subscribe<CombinedGeometry, Geometry, GeometryInvalidatedEventsArgs>(
                cg,
                geometry,
                static (instance, sender, args) => instance.OnGeometryChanged(sender, args),
                static (handler, source) => source.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(handler),
                static (handler, source) => source.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(handler));
        }

        cg.ResetPathData();
    }

    /// <summary>
    /// Identifies the <see cref="Geometry2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty Geometry2Property =
        DependencyProperty.Register(
            nameof(Geometry2),
            typeof(Geometry),
            typeof(CombinedGeometry),
            new PropertyMetadata(null, OnGeometry2Changed));

    /// <summary>
    /// Gets or sets the second <see cref="Geometry"/> object of this <see cref="CombinedGeometry"/> object.
    /// </summary>
    /// <returns>
    /// The second <see cref="Geometry"/> object.
    /// </returns>
    public Geometry Geometry2
    {
        get => (Geometry)GetValue(Geometry2Property);
        set => SetValueInternal(Geometry2Property, value);
    }

    private static void OnGeometry2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var cg = (CombinedGeometry)d;

        if (cg._weakGeometry2InvalidatedToken is not null)
        {
            cg._weakGeometry2InvalidatedToken.Dispose();
            cg._weakGeometry2InvalidatedToken = null;
        }

        if (e.NewValue is Geometry geometry)
        {
            cg._weakGeometry2InvalidatedToken = WeakEvent.Subscribe<CombinedGeometry, Geometry, GeometryInvalidatedEventsArgs>(
                cg,
                geometry,
                static (instance, sender, args) => instance.OnGeometryChanged(sender, args),
                static (handler, source) => source.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(handler),
                static (handler, source) => source.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(handler));
        }

        cg.ResetPathData();
    }

    private void OnGeometryChanged(object sender, GeometryInvalidatedEventsArgs e) => ResetPathData();

    private void ResetPathData()
    {
        _data = null;
        RaisePathChanged();
    }

    /// <summary>
    /// Identifies the <see cref="GeometryCombineMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GeometryCombineModeProperty =
        DependencyProperty.Register(
            nameof(GeometryCombineMode),
            typeof(GeometryCombineMode),
            typeof(CombinedGeometry),
            new PropertyMetadata(GeometryCombineMode.Union, OnGeometryCombineModeChanged),
            ValidateEnums.IsGeometryCombineModeValid);

    /// <summary>
    /// Gets or sets the method by which the two geometries (specified by the <see cref="Geometry1"/> and 
    /// <see cref="Geometry2"/> properties) are combined.
    /// </summary>
    /// <returns>
    /// The method by which <see cref="Geometry1"/> and <see cref="Geometry2"/> are combined. The default value is 
    /// <see cref="GeometryCombineMode.Union"/>.
    /// </returns>
    public GeometryCombineMode GeometryCombineMode
    {
        get => (GeometryCombineMode)GetValue(GeometryCombineModeProperty);
        set => SetValueInternal(GeometryCombineModeProperty, value);
    }

    private static void OnGeometryCombineModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CombinedGeometry)d).ResetPathData();
    }

    /// <summary>
    /// Determines whether this <see cref="CombinedGeometry"/> object is empty.
    /// </summary>
    /// <returns>
    /// true if this <see cref="CombinedGeometry"/> is empty; otherwise, false.
    /// </returns>
    public override bool IsEmpty()
    {
        if (IsObviouslyEmpty())
        {
            return true;
        }

        return GetPathGeometryData().IsEmpty();
    }

    /// <summary>
    /// Determines whether this <see cref="CombinedGeometry"/> object may have curved segments.
    /// </summary>
    /// <returns>
    /// true if this <see cref="CombinedGeometry"/> object may have curved segments; otherwise, false.
    /// </returns>
    public override bool MayHaveCurves()
    {
        Geometry geometry1 = Geometry1;
        Geometry geometry2 = Geometry2;
        return (geometry1 is not null && geometry1.MayHaveCurves()) || (geometry2 is not null && geometry2.MayHaveCurves());
    }

    internal override void SerializeData(CapacityStreamGeometryContext context, Matrix transform)
    {
        Matrix matrix = GetCombinedMatrix(transform);
        PathGeometry.ParsePathGeometryData(GetPathGeometryData(), matrix, context);
    }

    internal override PathGeometryData GetPathGeometryData()
    {
        if (IsObviouslyEmpty())
        {
            return GetEmptyPathGeometryData();
        }

        if (_data is null)
        {
            var context = new ByteStreamGeometryContext();

            Geometry g1 = Geometry1 ?? new PathGeometry();
            Geometry g2 = Geometry2 ?? new PathGeometry();

            PathGeometry.InternalCombine(
                g1,
                g2,
                GeometryCombineMode,
                Matrix.Identity,
                StandardFlatteningTolerance,
                ToleranceType.Absolute,
                context);

            context.Close();

            _data = context.GetData() ?? GetEmptyPathGeometryData().SerializedData;
        }

        return new PathGeometryData
        {
            FillRule = GetFillRule(),
            Matrix = Transform.ToMatrix(Transform),
            SerializedData = _data,
        };
    }

    internal override bool IsObviouslyEmpty()
    {
        // See which operand is obviously empty
        Geometry geometry1 = Geometry1;
        Geometry geometry2 = Geometry2;
        bool empty1 = geometry1 is null || geometry1.IsObviouslyEmpty();
        bool empty2 = geometry2 is null || geometry2.IsObviouslyEmpty();

        // Depending on the operation -- 
        if (GeometryCombineMode == GeometryCombineMode.Intersect)
        {
            return empty1 || empty2;
        }
        else if (GeometryCombineMode == GeometryCombineMode.Exclude)
        {
            return empty1;
        }
        else
        {
            // Union or Xor
            return empty1 && empty2;
        }
    }
}
