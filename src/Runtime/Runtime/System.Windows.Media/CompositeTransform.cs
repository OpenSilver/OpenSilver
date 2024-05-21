
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
    /// This class lets you apply multiple different transforms to an object.
    /// </summary>
    public sealed class CompositeTransform : Transform
    {
        /// <summary>
        /// Identifies the <see cref="ScaleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(
                nameof(ScaleX),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-axis scale factor. You can use this property to stretch or
        /// shrink an object horizontally.
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
                typeof(CompositeTransform),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-axis scale factor. You can use this property to stretch or
        /// shrink an object vertically.
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
        /// Identifies the <see cref="SkewX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewXProperty =
            DependencyProperty.Register(
                nameof(SkewX),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-axis skew angle, which is measured in degrees counterclockwise
        /// from the y-axis. A skew transform can be useful for creating the illusion of
        /// three-dimensional depth in a two-dimensional object.
        /// </summary>
        /// <returns>
        /// The skew angle, which is measured in degrees counterclockwise from the y-axis.
        /// The default is 0.
        /// </returns>
        public double SkewX
        {
            get => (double)GetValue(SkewXProperty);
            set => SetValueInternal(SkewXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SkewY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkewYProperty =
            DependencyProperty.Register(
                nameof(SkewY),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-axis skew angle, which is measured in degrees counterclockwise
        /// from the x-axis. A skew transform can be useful for creating the illusion of
        /// three-dimensional depth in a two-dimensional object.
        /// </summary>
        /// <returns>
        /// The skew angle, which is measured in degrees counterclockwise from the x-axis.
        /// The default is 0.
        /// </returns>
        public double SkewY
        {
            get => (double)GetValue(SkewYProperty);
            set => SetValueInternal(SkewYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Rotation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register(
                nameof(Rotation),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the angle, in degrees, of clockwise rotation.
        /// </summary>
        /// <returns>
        /// The angle, in degrees, of clockwise rotation. The default is 0.
        /// </returns>
        public double Rotation
        {
            get => (double)GetValue(RotationProperty);
            set => SetValueInternal(RotationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TranslateX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register(
                nameof(TranslateX),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the distance to translate along the x-axis.
        /// </summary>
        /// <returns>
        /// The distance to translate (move) an object along the x-axis, in pixels. This
        /// property is read/write. The default is 0.
        /// </returns>
        public double TranslateX
        {
            get => (double)GetValue(TranslateXProperty);
            set => SetValueInternal(TranslateXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TranslateY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register(
                nameof(TranslateY),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the distance to translate (move) an object along the y-axis.
        /// </summary>
        /// <returns>
        /// The distance to translate (move) an object along the y-axis, in pixels. The default is 0.
        /// </returns>
        public double TranslateY
        {
            get => (double)GetValue(TranslateYProperty);
            set => SetValueInternal(TranslateYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(
                nameof(CenterX),
                typeof(double),
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-coordinate of the center point for all transforms specified
        /// by the <see cref="CompositeTransform"/>.
        /// </summary>
        /// <returns>
        /// The x-coordinate of the center point for all transforms specified by the <see cref="CompositeTransform"/>.
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
                typeof(CompositeTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-coordinate of the center point for all transforms specified
        /// by the <see cref="CompositeTransform"/>.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the center point for all transforms specified by the <see cref="CompositeTransform"/>.
        /// </returns>
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValueInternal(CenterYProperty, value);
        }

        private protected override Matrix GetMatrixCore()
        {
            double centerX = CenterX;
            double centerY = CenterY;
            bool hasCenter = centerX != 0 || centerY != 0;

            // 1. Scale
            Matrix transform = new Matrix();
            transform.ScaleAt(ScaleX, ScaleY, centerX, centerY);

            // 2. Skew
            if (hasCenter)
            {
                transform.Translate(-centerX, -centerY);
            }

            transform.Skew(SkewX, SkewY);

            if (hasCenter)
            {
                transform.Translate(centerX, centerY);
            }

            // 3. Rotate
            transform.RotateAt(Rotation, centerX, centerY);

            // 4. Translate
            transform.Translate(TranslateX, TranslateY);

            return transform;
        }

        internal override bool IsIdentity =>
            ScaleX == 1 && ScaleY == 1 &&
            SkewX == 0 && SkewY == 0 &&
            Rotation == 0 &&
            TranslateX == 0 && TranslateY == 0;
    }
}
