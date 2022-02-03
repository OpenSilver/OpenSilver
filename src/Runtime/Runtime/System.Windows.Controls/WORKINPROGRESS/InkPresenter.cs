#if MIGRATION
using System.Windows.Ink;

namespace System.Windows.Controls
{
    /// <summary>
    /// Implements a rectangular surface that displays ink strokes.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class InkPresenter : Canvas
    {
        /// <summary>
        /// Gets or sets the strokes that the <see cref="InkPresenter"/> displays.
        /// </summary>
        /// <returns>
        /// The collection of ink strokes that are displayed by the <see cref="InkPresenter"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public StrokeCollection Strokes
        {
            get { return (StrokeCollection)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register("Strokes", typeof(StrokeCollection), typeof(InkPresenter), new PropertyMetadata());
    }
}
#endif
