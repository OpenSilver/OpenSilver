
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
    }
}
