#if MIGRATION
using System.Windows.Ink;
using System.Windows.Media;

namespace System.Windows.Controls
{
    [OpenSilver.NotImplemented]
    public class InkPresenter : Decorator
    {
        /// <summary>
        /// Gets or sets the strokes that the System.Windows.Controls.InkPresenter displays.
        /// </summary>
        [OpenSilver.NotImplemented]
        public StrokeCollection Strokes
        {
            get { return (StrokeCollection)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty =
            DependencyProperty.Register("Strokes", typeof(StrokeCollection), typeof(InkPresenter), new PropertyMetadata());

        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Calendar), new PropertyMetadata());
    }
}
#endif
