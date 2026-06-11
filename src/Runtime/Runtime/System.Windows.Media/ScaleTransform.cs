
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

namespace System.Windows.Media
{
    /// <summary>
    /// Scales an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed class ScaleTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransform"/> class.
        /// </summary>
        public ScaleTransform() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransform"/> class with the specified x- and y- scale 
        /// factors. The scale operation is centered on (0,0).
        /// </summary>
        /// <param name="scaleX">
        /// The x-axis scale factor.
        /// </param>
        /// <param name="scaleY">
        /// The y-axis scale factor.
        /// </param>
        public ScaleTransform(double scaleX, double scaleY)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransform"/> class that has the specified scale factors 
        /// and center point.
        /// </summary>
        /// <param name="scaleX">
        /// The x-axis scale factor. For more information, see the <see cref="ScaleX"/> property.
        /// </param>
        /// <param name="scaleY">
        /// The y-axis scale factor. For more information, see the <see cref="ScaleY"/> property.
        /// </param>
        /// <param name="centerX">
        /// The x-coordinate of the center of this <see cref="ScaleTransform"/>. For more information, see the 
        /// <see cref="CenterX"/> property.
        /// </param>
        /// <param name="centerY">
        /// The y-coordinate of the center of this <see cref="ScaleTransform"/>. For more information, see the 
        /// <see cref="CenterY"/> property.
        /// </param>
        public ScaleTransform(double scaleX, double scaleY, double centerX, double centerY)
            : this(scaleX, scaleY)
        {
            CenterX = centerX;
            CenterY = centerY;
        }

        /// <summary>
        /// Identifies the <see cref="ScaleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(
                nameof(ScaleX),
                typeof(double),
                typeof(ScaleTransform),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-axis scale factor.
        /// </summary>
        /// <returns>
        /// The scale factor along the x-axis. The default is 1.
        /// </returns>
        public double ScaleX
        {
            get => (double)GetValue(ScaleXProperty);
            set => SetValueInternal(ScaleXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ScaleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(
                nameof(ScaleY),
                typeof(double),
                typeof(ScaleTransform),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-axis scale factor.
        /// </summary>
        /// <returns>
        /// The scale factor along the y-axis. The default is 1.
        /// </returns>
        public double ScaleY
        {
            get => (double)GetValue(ScaleYProperty);
            set => SetValueInternal(ScaleYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(
                nameof(CenterX),
                typeof(double),
                typeof(ScaleTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// </summary>
        /// <returns>
        /// The x-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// The default is 0.
        /// </returns>
        public double CenterX
        {
            get => (double)GetValue(CenterXProperty);
            set => SetValueInternal(CenterXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register(
                nameof(CenterY),
                typeof(double),
                typeof(ScaleTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the center point of this <see cref="ScaleTransform"/>.
        /// The default is 0.
        /// </returns>
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValueInternal(CenterYProperty, value);
        }

        /// <inheritdoc />
        public override Matrix Value => Matrix.CreateScaling(ScaleX, ScaleY, CenterX, CenterY);

        internal override bool IsIdentity => ScaleX == 1 && ScaleY == 1;

        /// <summary>
        /// Returns a live inverse transform that updates whenever this <see cref="ScaleTransform"/> changes.
        /// This ensures that bindings which capture the inverse (e.g. via UnscaleTransformConverter)
        /// continue to reflect the correct inverse even after ScaleX/ScaleY are mutated at runtime.
        /// </summary>
        public override GeneralTransform Inverse
        {
            get
            {
                double sx = ScaleX;
                double sy = ScaleY;
                if (sx == 0 || sy == 0)
                {
                    return null;
                }
                return new LiveInverseScaleTransform(this);
            }
        }

        /// <summary>
        /// A Transform that mirrors the inverse of a source <see cref="ScaleTransform"/>.
        /// It subscribes to the source's Changed event so that its matrix stays current.
        /// </summary>
        private sealed class LiveInverseScaleTransform : Transform
        {
            private readonly ScaleTransform _source;
            private WeakEventToken _changedToken;

            internal LiveInverseScaleTransform(ScaleTransform source)
            {
                _source = source;
                _changedToken = WeakEvent.Subscribe<LiveInverseScaleTransform, ScaleTransform, EventArgs>(
                    this,
                    source,
                    static (instance, sender, args) => instance.OnSourceChanged(),
                    static (handler, src) => src.Changed -= new EventHandler(handler),
                    static (handler, src) => src.Changed += new EventHandler(handler));
            }

            private void OnSourceChanged() => OnTransformChanged();

            public override Matrix Value
            {
                get
                {
                    double sx = _source.ScaleX;
                    double sy = _source.ScaleY;
                    if (sx == 0 || sy == 0)
                    {
                        return Matrix.Identity;
                    }
                    return Matrix.CreateScaling(1.0 / sx, 1.0 / sy, _source.CenterX, _source.CenterY);
                }
            }

            internal override bool IsIdentity => _source.ScaleX == 1 && _source.ScaleY == 1;

            public override GeneralTransform Inverse => _source;
        }
    }
}
