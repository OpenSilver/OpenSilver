
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
/// Animates the value of a <see cref="Matrix"/> property by using a <see cref="Media.PathGeometry"/> 
/// to generate the animated values. This animation can be used to move a visual object 
/// along a path.
/// </summary>
public class MatrixAnimationUsingPath : AnimationTimeline, IAnimation<Matrix>
{
    private WeakEventToken _weakPathChangedEventToken;
    private Geometry.PathGeometryData _pathGeometryData;
    private bool _isValid;

    /// <summary>
    /// If IsCumulative is set to true, these values represents the values
    /// that are accumulated with each repeat.  They are the end values of
    /// the path.
    /// </summary>
    private Vector _accumulatingOffset = new();
    private double _accumulatingAngle;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixAnimationUsingPath"/> class.
    /// </summary>
    public MatrixAnimationUsingPath() { }

    /// <inheritdoc />
    public override Type TargetPropertyType => typeof(Matrix);

    /// <summary>
    /// Identifies the <see cref="DoesRotateWithTangent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DoesRotateWithTangentProperty =
        DependencyProperty.Register(
            nameof(DoesRotateWithTangent),
            typeof(bool),
            typeof(MatrixAnimationUsingPath),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnPropertyChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the object rotates along the tangent of 
    /// the path.
    /// </summary>
    /// <returns>
    /// true if the object will rotate along the tangent of the path; otherwise, false.
    /// The default is false.
    /// </returns>
    public bool DoesRotateWithTangent
    {
        get => (bool)GetValue(DoesRotateWithTangentProperty);
        set => SetValueInternal(DoesRotateWithTangentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsAngleCumulative"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAngleCumulativeProperty =
        DependencyProperty.Register(
            nameof(IsAngleCumulative),
            typeof(bool),
            typeof(MatrixAnimationUsingPath),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnPropertyChanged));

    /// <summary>
    /// Gets or sets a value that specifies whether the rotation angle of the animated 
    /// matrix should accumulate over repetitions.
    /// </summary>
    /// <returns>
    /// true if the animation's rotation angle should accumulate over repetitions; 
    /// otherwise, false. The default is false.
    /// </returns>
    public bool IsAngleCumulative
    {
        get => (bool)GetValue(IsAngleCumulativeProperty);
        set => SetValueInternal(IsAngleCumulativeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsOffsetCumulative"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsOffsetCumulativeProperty =
        DependencyProperty.Register(
            nameof(IsOffsetCumulative),
            typeof(bool),
            typeof(MatrixAnimationUsingPath),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnPropertyChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the offset produced by the animated matrix 
    /// will accumulate over repetitions.
    /// </summary>
    /// <returns>
    /// true if the object will accumulate over repeats of the animation; otherwise, false.
    /// The default is false.
    /// </returns>
    public bool IsOffsetCumulative
    {
        get => (bool)GetValue(IsOffsetCumulativeProperty);
        set => SetValueInternal(IsOffsetCumulativeProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="PathGeometry"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PathGeometryProperty =
        DependencyProperty.Register(
            nameof(PathGeometry),
            typeof(PathGeometry),
            typeof(MatrixAnimationUsingPath),
            new PropertyMetadata(null, OnPathGeometryChanged));

    /// <summary>
    /// Gets or sets the geometry used to generate this animation's output values.
    /// </summary>
    /// <returns>
    /// The geometry used to generate this animation's output values. The default is null.
    /// </returns>
    public PathGeometry PathGeometry
    {
        get => (PathGeometry)GetValue(PathGeometryProperty);
        set => SetValueInternal(PathGeometryProperty, value);
    }

    private static void OnPathGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var animation = (MatrixAnimationUsingPath)d;
        animation._weakPathChangedEventToken?.Dispose();
        animation._weakPathChangedEventToken = null;

        if (e.NewValue is PathGeometry path)
        {
            animation._weakPathChangedEventToken = WeakEvent.Subscribe<MatrixAnimationUsingPath, PathGeometry, GeometryInvalidatedEventsArgs>(
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
    /// Gets or sets a value that indicates whether the target property's current value 
    /// should be added to this animation's starting value.
    /// </summary>
    /// <returns>
    /// true if the target property's current value should be added to this animation's 
    /// starting value; otherwise, false. The default is false.
    /// </returns>
    public bool IsAdditive
    {
        get => (bool)GetValue(IsAdditiveProperty);
        set => SetValueInternal(IsAdditiveProperty, value);
    }

    internal sealed override TimelineClock CreateClock() =>
        new AnimationClock<Matrix>(this, new Animator<Matrix>(this));

    Matrix IAnimation<Matrix>.GetCurrentValue(Matrix initialValue, DependencyProperty dp, TimelineClock clock)
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

        double angle = 0.0;

        if (DoesRotateWithTangent)
        {
            angle = DoubleAnimationUsingPath.CalculateAngleFromTangentVector(pathTangent.X, pathTangent.Y);
        }

        var matrix = new Matrix();

        double currentRepeat = (double)(clock.CurrentIteration - 1);

        if (currentRepeat > 0)
        {
            if (IsOffsetCumulative)
            {
                pathPoint += _accumulatingOffset * currentRepeat;
            }

            if (DoesRotateWithTangent && IsAngleCumulative)
            {
                angle += _accumulatingAngle * currentRepeat;
            }
        }

        matrix.Rotate(angle);
        matrix.Translate(pathPoint.X, pathPoint.Y);

        if (IsAdditive)
        {
            return Matrix.Multiply(matrix, initialValue);
        }
        else
        {
            return matrix;
        }
    }

    private void Validate()
    {
        Debug.Assert(!_isValid);

        _pathGeometryData = PathGeometry?.GetPathGeometryData() ?? Geometry.GetEmptyPathGeometryData();

        if (IsOffsetCumulative || IsAngleCumulative)
        {
            // Get values at the beginning of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 0.0, out Point startPoint, out Point startTangent);

            // Get values at the end of the path.
            PathGeometry.GetPointAtLengthFraction(_pathGeometryData, 1.0, out Point endPoint, out Point endTangent);

            // Calculate difference.
            _accumulatingAngle = DoubleAnimationUsingPath.CalculateAngleFromTangentVector(endTangent.X, endTangent.Y)
                                 - DoubleAnimationUsingPath.CalculateAngleFromTangentVector(startTangent.X, startTangent.Y);

            _accumulatingOffset.X = endPoint.X - startPoint.X;
            _accumulatingOffset.Y = endPoint.Y - startPoint.Y;
        }

        _isValid = true;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MatrixAnimationUsingPath)d).OnChanged();
    }
}