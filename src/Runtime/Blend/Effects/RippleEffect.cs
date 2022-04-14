
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

using System.Windows.Media.Effects;

#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Effect that simulates water ripples on the visual.
    /// </summary>
    public sealed class RippleEffect : ShaderEffect
    {
        /// <summary>
        /// Gets or sets the Center variable within the shader.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(
                nameof(Center),
                typeof(Point),
                typeof(RippleEffect),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>
        /// Gets or sets the Frequency variable within the shader.
        /// </summary>
        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register(
                nameof(Frequency),
                typeof(double),
                typeof(RippleEffect),
                new PropertyMetadata(40.0));

        /// <summary>
        /// Gets or sets the Amplitude variable within the shader.
        /// </summary>
        public static readonly DependencyProperty MagnitudeProperty =
            DependencyProperty.Register(
                nameof(Magnitude),
                typeof(double),
                typeof(RippleEffect),
                new PropertyMetadata(0.1));

        /// <summary>
        /// Gets or sets the Phase variable within the shader.
        /// </summary>
        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register(
                nameof(Phase),
                typeof(double),
                typeof(RippleEffect),
                new PropertyMetadata(10.0));

        /// <summary>
        /// Creates an instance and updates the shader's variables to the default values.
        /// </summary>
        public RippleEffect() { }

        /// <summary>
        /// Gets or sets the Center variable within the shader.
        /// </summary>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        /// <summary>
        /// Gets or sets the Frequency variable within the shader.
        /// </summary>
        public double Frequency
        {
            get => (double)GetValue(FrequencyProperty);
            set => SetValue(FrequencyProperty, value);
        }

        /// <summary>
        /// Gets or sets the Amplitude variable within the shader.
        /// </summary>
        public double Magnitude
        {
            get => (double)GetValue(MagnitudeProperty);
            set => SetValue(MagnitudeProperty, value);
        }

        /// <summary>
        /// Gets or sets the Phase variable within the shader.
        /// </summary>
        public double Phase
        {
            get => (double)GetValue(PhaseProperty);
            set => SetValue(PhaseProperty, value);
        }
    }
}