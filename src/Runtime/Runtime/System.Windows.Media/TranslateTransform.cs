
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
    /// Translates (moves) an object in the two-dimensional x-y coordinate system.
    /// </summary>
    public sealed class TranslateTransform : Transform
    {
        /// <summary>
        /// Identifies the <see cref="X"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register(
                nameof(X),
                typeof(double),
                typeof(TranslateTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the distance to translate along the x-axis.
        /// </summary>
        /// <returns>
        /// The distance to translate (move) an object along the x-axis, in pixels. This
        /// property is read/write. The default is 0.
        /// </returns>
        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValueInternal(XProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Y"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register(
                nameof(Y),
                typeof(double),
                typeof(TranslateTransform),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the distance to translate (move) an object along the y-axis.
        /// </summary>
        /// <returns>
        /// The distance to translate (move) an object along the y-axis, in pixels.
        /// The default is 0.
        /// </returns>
        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValueInternal(YProperty, value);
        }

        private protected override Matrix GetMatrixCore()
        {
            Matrix matrix = Matrix.Identity;
            matrix.Translate(X, Y);
            return matrix;
        }

        internal override bool IsIdentity => X == 0 && Y == 0;
    }
}
