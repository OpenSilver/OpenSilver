
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

using System.Windows.Markup;

namespace System.Windows.Media
{
    /// <summary>
    /// Describes the location and color of a transition point in a gradient.
    /// </summary>
    [ContentProperty(nameof(Color))]
    public sealed class GradientStop : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(GradientStop),
                new PropertyMetadata(Colors.Transparent, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the color of the gradient stop.
        /// </summary>
        /// <returns>
        /// The color of the gradient stop. The default is <see cref="Colors.Transparent"/>.
        /// </returns>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValueInternal(ColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Offset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(
                nameof(Offset),
                typeof(double),
                typeof(GradientStop),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets the location of the gradient stop within the gradient vector.
        /// </summary>
        public double Offset
        {
            get => (double)GetValue(OffsetProperty);
            set => SetValueInternal(OffsetProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GradientStop)d).RaiseChanged();
        }

        internal event EventHandler Changed;

        private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public object Clone() => new GradientStop { Color = Color, Offset = Offset };
    }
}
