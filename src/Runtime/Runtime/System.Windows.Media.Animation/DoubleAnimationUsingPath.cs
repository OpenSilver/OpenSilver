
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
/// Animates the value of a <see cref="double"/> property between two or more target 
/// values using a <see cref="Media.PathGeometry"/> to specify those values. This 
/// animation can be used to move a visual object along a path.
/// </summary>
public class DoubleAnimationUsingPath : AnimationTimeline, IAnimation<double>
{
    private WeakEventToken _weakPathChangedEventToken;
    private Geometry.PathGeometryData _pathGeometryData;
    private bool _isValid;

    /// <summary>
    /// If IsCumulative is set to true, this value represents the value that
    /// is accumulated with each repeat.  It is the end value of the path
    /// output value for the path.
    /// </summary>
    private double _accumulatingValue;

    static DoubleAnimationUsingPath()
    {
        IsCumulativeProperty.OverrideMetadata(typeof(DoubleAnimationUsingPath), new PropertyMetadata(BooleanBoxes.FalseBox, OnPropertyChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleAnimationUsingPath"/> class.
    /// </summary>
    public DoubleAnimationUsingPath() { }

    /// <inheritdoc />
    public sealed override Type TargetPropertyType => typeof(double);

    /// <summary>
    /// Identifies the <see cref="PathGeometry"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PathGeometryProperty =
        DependencyProperty.Register(
            nameof(PathGeometry),
            typeof(PathGeometry),
            typeof(DoubleAnimationUsingPath),
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
        var animation = (DoubleAnimationUsingPath)d;

        animation._weakPathChangedEventToken?.Dispose();
        animation._weakPathChangedEventToken = null;

        if (e.NewValue is PathGeometry path)
        {
            animation._weakPathChangedEventToken = WeakEvent.Subscribe<DoubleAnimationUsingPath, PathGeometry, GeometryInvalidatedEventsArgs>(
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
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(PathAnimationSource),
            typeof(DoubleAnimationUsingPath),
            new PropertyMetadata(PathAnimationSource.X, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the aspect of this animation's <see cref="PathGeometry"/> that 
    /// determines its output value.
    /// </summary>
    /// <returns>
    /// The aspect of this animation's <see cref="PathGeometry"/> that determines its 
    /// output value. The default value is <see cref="PathAnimationSource.X"/>.
    /// </returns>
    public PathAnimationSource Source
    {
        get => (PathAnimationSource)GetValue(SourceProperty);
        set => SetValueInternal(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the target property's current value 
    /// should be added to this animation's starting value.
    /// </summary>
    /// <returns>
    /// true if the target property's current value should be added to this animation's
    /// starting value; otherwise, false. The default value is false.
    /// </returns>
    public bool IsAdditive
    {
        get => (bool)GetValue(IsAdditiveProperty);
        set => SetValueInternal(IsAdditiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the animation's value accumulates 
    /// when it repeats.
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
        new AnimationClock<double>(this, new Animator<double>(this));

    double IAnimation<double>.GetCurrentValue(double initialValue, DependencyProperty dp, TimelineClock clock)
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

        PathGeometry.GetPointAtLengthFraction(_pathGeometryData, clock.CurrentProgress.Value, out Point pathPoint, out Point pathTangent);

        double pathValue = Source switch
        {
            PathAnimationSource.Angle => CalculateAngleFromTangentVector(pathTangent.X, pathTangent.Y),
            PathAnimationSource.X => pathPoint.X,
            PathAnimationSource.Y => pathPoint.Y,
            _ => 0.0,
        };

        double currentRepeat = (double)(clock.CurrentIteration - 1);

        if (IsCumulative && currentRepeat > 0)
        {
            pathValue += _accumulatingValue * currentRepeat;
        }

        if (IsAdditive)
        {
            return initialValue + pathValue;
        }
        else
        {
            return pathValue;
        }
    }

    /// <summary>
    /// The primary purpose of this method is to calculate the accumulating
    /// value if one of the properties changes.
    /// </summary>
    private void Validate()
    {
        Debug.Assert(!_isValid);

        _pathGeometryData = PathGeometry?.GetPathGeometryData() ?? Geometry.GetEmptyPathGeometryData();

        if (IsCumulative)
        {
            // Get values at the beginning of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 0.0, out Point startPoint, out Point startTangent);

            // Get values at the end of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 1.0, out Point endPoint, out Point endTangent);

            switch (Source)
            {
                case PathAnimationSource.Angle:
                    _accumulatingValue = CalculateAngleFromTangentVector(endTangent.X, endTangent.Y)
                                         - CalculateAngleFromTangentVector(startTangent.X, startTangent.Y);
                    break;

                case PathAnimationSource.X:
                    _accumulatingValue = endPoint.X - startPoint.X;
                    break;

                case PathAnimationSource.Y:
                    _accumulatingValue = endPoint.Y - startPoint.Y;
                    break;
            }
        }

        _isValid = true;
    }

    internal static double CalculateAngleFromTangentVector(double x, double y)
    {
        double angle = Math.Acos(x) * (180.0 / Math.PI);

        if (y < 0.0)
        {
            angle = 360 - angle;
        }

        return angle;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((DoubleAnimationUsingPath)d).OnChanged();
    }
}
