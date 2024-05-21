
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
    /// Represents a two-dimensional skew.
    /// </summary>
    public sealed class SkewTransform : Transform
    {
        /// <summary>
        /// Identifies the <see cref="AngleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleXProperty =
            DependencyProperty.Register(
                nameof(AngleX),
                typeof(double),
                typeof(SkewTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-axis skew angle, which is measured in degrees counterclockwise
        /// from the y-axis.
        /// </summary>
        /// <returns>
        /// The skew angle, which is measured in degrees counterclockwise from the y-axis.
        /// The default is 0.
        /// </returns>
        public double AngleX
        {
            get => (double)GetValue(AngleXProperty);
            set => SetValueInternal(AngleXProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AngleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleYProperty =
            DependencyProperty.Register(
                nameof(AngleY),
                typeof(double),
                typeof(SkewTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-axis skew angle, which is measured in degrees counterclockwise
        /// from the x-axis.
        /// </summary>
        /// <returns>
        /// The skew angle, which is measured in degrees counterclockwise from the x-axis.
        /// The default is 0.
        /// </returns>
        public double AngleY
        {
            get => (double)GetValue(AngleYProperty);
            set => SetValueInternal(AngleYProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(
                nameof(CenterX),
                typeof(double),
                typeof(SkewTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-coordinate of the transform center.
        /// </summary>
        /// <returns>
        /// The x-coordinate of the transform center. The default is 0.
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
                typeof(SkewTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-coordinate of the transform center.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the transform center. The default is 0.
        /// </returns>
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValueInternal(CenterYProperty, value);
        }

        private protected override Matrix GetMatrixCore()
        {
            Matrix matrix = new Matrix();

            double angleX = AngleX;
            double angleY = AngleY;
            double centerX = CenterX;
            double centerY = CenterY;

            bool hasCenter = centerX != 0 || centerY != 0;

            if (hasCenter)
            {
                matrix.Translate(-centerX, -centerY);
            }

            matrix.Skew(angleX, angleY);

            if (hasCenter)
            {
                matrix.Translate(centerX, centerY);
            }

            return matrix;
        }

        internal override bool IsIdentity => AngleX == 0 && AngleY == 0;
    }
}
