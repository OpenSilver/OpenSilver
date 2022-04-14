#if MIGRATION
using System.Windows.Input;

namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a collection of points that correspond to a stylus-down, move, and stylus-up sequence.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class Stroke : DependencyObject
    {
        /// <summary>
        /// Gets or sets the stylus points of the <see cref="Stroke"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="StylusPointCollection"/> that contains the stylus points that represent the current <see cref="Stroke"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public StylusPointCollection StylusPoints
        {
            get { return (StylusPointCollection)GetValue(StylusPointsProperty); }
            set { SetValue(StylusPointsProperty, value); }
        }

        public static readonly DependencyProperty StylusPointsProperty =
            DependencyProperty.Register("StylusPoints", typeof(StylusPointCollection), typeof(Stroke), new PropertyMetadata());
    }
}
#endif
