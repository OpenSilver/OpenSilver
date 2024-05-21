
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

        private protected override Matrix GetMatrixCore()
        {
            Matrix m = new Matrix();
            m.ScaleAt(ScaleX, ScaleY, CenterX, CenterY);
            return m;
        }

        internal override bool IsIdentity => ScaleX == 1 && ScaleY == 1;
    }
}
