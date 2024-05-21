
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
    /// Rotates an object clockwise about a specified point in a two-dimensional
    /// x-y coordinate system.
    /// </summary>
    public sealed class RotateTransform : Transform
    {
        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                nameof(Angle),
                typeof(double),
                typeof(RotateTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the angle, in degrees, of clockwise rotation.
        /// </summary>
        /// <returns>
        /// The angle, in degrees, of clockwise rotation. The default is 0.
        /// </returns>
        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValueInternal(AngleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CenterX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register(
                nameof(CenterX),
                typeof(double),
                typeof(RotateTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the x-coordinate of the rotation center point.
        /// </summary>
        /// <returns>
        /// The x-coordinate of the center of rotation. The default is 0.
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
                typeof(RotateTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the y-coordinate of the rotation center point.
        /// </summary>
        /// <returns>
        /// The y-coordinate of the center of rotation. The default is 0.
        /// </returns>
        public double CenterY
        {
            get => (double)GetValue(CenterYProperty);
            set => SetValueInternal(CenterYProperty, value);
        }

        private protected override Matrix GetMatrixCore()
        {
            Matrix m = new Matrix();
            m.RotateAt(Angle, CenterX, CenterY);
            return m;
        }

        internal override bool IsIdentity => Angle == 0;
    }
}