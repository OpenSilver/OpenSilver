
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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Transition effect that waves the current visual while introducing the new visual.
    /// </summary>
    public sealed class WaveTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Dependency property that modifies the Frequency variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register(
                nameof(Frequency),
                typeof(double),
                typeof(WaveTransitionEffect),
                new PropertyMetadata(20.0));

        /// <summary>
        /// Dependency property that modifies the Magnitude variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty MagnitudeProperty =
            DependencyProperty.Register(
                nameof(Magnitude),
                typeof(double),
                typeof(WaveTransitionEffect),
                new PropertyMetadata(0.1));

        /// <summary>
        /// Dependency property that modifies the Phase variable within the pixel shader.
        /// </summary>
        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register(
                nameof(Phase),
                typeof(double),
                typeof(WaveTransitionEffect),
                new PropertyMetadata(14.0));

        /// <summary>
        /// Creates an instance of the shader.
        /// </summary>
        public WaveTransitionEffect() { }

        /// <summary>
        /// Gets or sets the magnitude of the wave.
        /// </summary>
        public double Frequency
        {
            get => (double)GetValue(FrequencyProperty);
            set => SetValue(FrequencyProperty, value);
        }

        /// <summary>
        /// Gets or sets the magnitude of the wave.
        /// </summary>
        public double Magnitude
        {
            get => (double)GetValue(MagnitudeProperty);
            set => SetValue(MagnitudeProperty, value);
        }

        /// <summary>
        /// Gets or sets the phase of the wave.
        /// </summary>
        public double Phase
        {
            get => (double)GetValue(PhaseProperty);
            set => SetValue(PhaseProperty, value);
        }

        /// <summary>
        /// Makes a deep copy of the WaveTransitionEffect effect.
        /// </summary>
        /// <returns>
        /// A clone of the current instance of the WaveTransitionEffect effect.
        /// </returns>
        protected override TransitionEffect DeepCopy()
        {
            return new WaveTransitionEffect
            {
                Frequency = Frequency,
                Magnitude = Magnitude,
                Phase = Phase,
            };
        }
    }
}
