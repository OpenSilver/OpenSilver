
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

namespace System.Windows.Media;

/// <summary>
/// Represents a segment of a <see cref="PathFigure"/> object.
/// </summary>
public abstract class PathSegment : DependencyObject
{
    private Geometry _parentGeometry;

    internal PathSegment() { }

    /// <summary>
    /// Identifies the <see cref="IsStroked"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty IsStrokedProperty =
        DependencyProperty.Register(
            nameof(IsStroked),
            typeof(bool),
            typeof(PathSegment),
            new UIPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the segment is stroked.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the segment is stroked when a Pen is used to render the segment;
    /// otherwise, <see langword="false"/>. The default is <see langword="true"/>.
    /// </value>
    [OpenSilver.NotImplemented]
    public bool IsStroked
    {
        get => (bool)GetValue(IsStrokedProperty);
        set => SetValueInternal(IsStrokedProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsSmoothJoin"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty IsSmoothJoinProperty =
        DependencyProperty.Register(
            nameof(IsSmoothJoin),
            typeof(bool),
            typeof(PathSegment),
            new UIPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value that indicates whether the join between this <see cref="PathSegment"/>
    /// and the previous <see cref="PathSegment"/> is treated as a corner when it is stroked with a 
    /// <see cref="Pen"/>.
    /// </summary>
    /// <returns>
    /// true if the join between this <see cref="PathSegment"/> and the previous <see cref="PathSegment"/> 
    /// is not to be treated as a corner; otherwise, false. The default is false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public bool IsSmoothJoin
    {
        get => (bool)GetValue(IsSmoothJoinProperty);
        set => SetValueInternal(IsSmoothJoinProperty, value);
    }

    internal void SetParentGeometry(Geometry geometry) => _parentGeometry = geometry;

    internal static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PathSegment)d).InvalidateParentGeometry();
    }

    internal void InvalidateParentGeometry() => _parentGeometry?.RaisePathChanged();

    internal virtual void SerializeData(StreamGeometryContext context, Matrix transform, ref Point current)
    {
        throw new NotSupportedException($"SerializeToContext() not supported on {GetType().Name}");
    }

    internal abstract bool IsCurved();
}
