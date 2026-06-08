
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
using OpenSilver.Internal.Media.Animation;
using System.Diagnostics;

namespace System.Windows.Media.Animation;

/// <summary>
/// Animates the value of a <see cref="Point"/> property between two or more target
/// values using a <see cref="Media.PathGeometry"/> to specify those values. This
/// animation can be used to move a visual object along a path.
/// </summary>
public class PointAnimationUsingPath : AnimationTimeline, IAnimation<Point>
{
    private WeakEventToken _weakPathChangedEventToken;
    private Geometry.PathGeometryData _pathGeometryData;
    private bool _isValid;

    /// <summary>
    /// If IsCumulative is set to true, this value represents the value that
    /// is accumulated with each repeat.  It is the end value of the path
    /// output value for the path.
    /// </summary>
    private Vector _accumulatingVector = new();

    static PointAnimationUsingPath()
    {
        IsCumulativeProperty.OverrideMetadata(typeof(PointAnimationUsingPath), new PropertyMetadata(BooleanBoxes.FalseBox, OnPropertyChanged));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PointAnimationUsingPath"/> class.
    /// </summary>
    public PointAnimationUsingPath() { }

    /// <inheritdoc />
    public override Type TargetPropertyType => typeof(Point);

    /// <summary>
    /// Identifies the <see cref="PathGeometry"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PathGeometryProperty =
        DependencyProperty.Register(
            nameof(PathGeometry),
            typeof(PathGeometry),
            typeof(PointAnimationUsingPath),
            new PropertyMetadata(null, OnPathGeometryChanged));

    /// <summary>
    /// Specifies the geometry used to generate this animation's output values.
    /// </summary>
    /// <returns>
    /// The path used to generate this animation's output values. The default value is null.
    /// </returns>
    public PathGeometry PathGeometry
    {
        get => (PathGeometry)GetValue(PathGeometryProperty);
        set => SetValueInternal(PathGeometryProperty, value);
    }

    private static void OnPathGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var animation = (PointAnimationUsingPath)d;

        animation._weakPathChangedEventToken?.Dispose();
        animation._weakPathChangedEventToken = null;

        if (e.NewValue is PathGeometry path)
        {
            animation._weakPathChangedEventToken = WeakEvent.Subscribe<PointAnimationUsingPath, PathGeometry, GeometryInvalidatedEventsArgs>(
                animation,
                path,
                static (instance, sender, args) => instance.OnPathChanged(sender, args),
                static (handler, source) => source.Invalidated -= new EventHandler<GeometryInvalidatedEventsArgs>(handler),
                static (handler, source) => source.Invalidated += new EventHandler<GeometryInvalidatedEventsArgs>(handler));
        }

        animation.OnChanged();
    }

    private void OnPathChanged(object sender, GeometryInvalidatedEventsArgs e) => OnChanged();

    private void OnChanged() => _isValid = false;

    /// <summary>
    /// Gets a value that specifies whether the animation's output value is added to the 
    /// base value of the property being animated.
    /// </summary>
    /// <returns>
    /// true if the animation adds its output value to the base value of the property
    /// being animated instead of replacing it; otherwise, false. The default value is false.
    /// </returns>
    public bool IsAdditive
    {
        get => (bool)GetValue(IsAdditiveProperty);
        set => SetValueInternal(IsAdditiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the animation's value accumulates when 
    /// it repeats.
    /// </summary>
    /// <returns>
    /// true if the animation accumulates its values when its <see cref="Timeline.RepeatBehavior"/>
    /// property causes it to repeat its simple duration. otherwise, false. The default
    /// value is false.
    /// </returns>
    public bool IsCumulative
    {
        get => (bool)GetValue(IsCumulativeProperty);
        set => SetValueInternal(IsCumulativeProperty, value);
    }

    internal sealed override TimelineClock CreateClock() =>
        new AnimationClock<Point>(this, new Animator<Point>(this));

    Point IAnimation<Point>.GetCurrentValue(Point initialValue, DependencyProperty dp, TimelineClock clock)
    {
        Debug.Assert(clock.CurrentState != ClockState.Stopped);

        if (!_isValid)
        {
            Validate();
        }

        if (_pathGeometryData.IsEmpty())
        {
            return initialValue;
        }

        PathGeometry.GetPointAtLengthFraction(_pathGeometryData, clock.CurrentProgress.Value, out Point pathPoint, out _);

        double currentRepeat = (double)(clock.CurrentIteration - 1);

        if (IsCumulative && currentRepeat > 0)
        {
            pathPoint += _accumulatingVector * currentRepeat;
        }

        if (IsAdditive)
        {
            return initialValue + (Vector)pathPoint;
        }
        else
        {
            return pathPoint;
        }
    }

    private void Validate()
    {
        Debug.Assert(!_isValid);

        _pathGeometryData = PathGeometry?.GetPathGeometryData() ?? Geometry.GetEmptyPathGeometryData();

        if (IsCumulative)
        {
            // Get values at the beginning of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 0.0, out Point startPoint, out _);

            // Get values at the end of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 1.0, out Point endPoint, out _);

            _accumulatingVector.X = endPoint.X - startPoint.X;
            _accumulatingVector.Y = endPoint.Y - startPoint.Y;
        }

        _isValid = true;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((PointAnimationUsingPath)d).OnChanged();
    }
}
