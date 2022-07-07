using System.Windows;

#if MIGRATION
using Microsoft.Windows;
namespace System.Windows.Controls
#else
using Windows.Foundation;
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A content control that visually indicates what actions are available
    /// during a drag operation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class DragDecorator : ContentControl
    {
        /// <summary>Identifies the icon position dependency property.</summary>
        public static readonly DependencyProperty IconPositionProperty = DependencyProperty.Register(nameof(IconPosition), typeof(Point), typeof(DragDecorator), new PropertyMetadata((object)new Point(0.0, 0.0)));
        /// <summary>Identifies the Effects dependency property.</summary>
        public static readonly DependencyProperty EffectsProperty = DependencyProperty.Register(nameof(Effects), typeof(DragDropEffects), typeof(DragDecorator), new PropertyMetadata((object)(DragDropEffects.All | DragDropEffects.Link), new PropertyChangedCallback(DragDecorator.OnEffectsPropertyChanged)));

        /// <summary>Gets or sets the icon position.</summary>
        public Point IconPosition
        {
            get
            {
                return (Point)this.GetValue(DragDecorator.IconPositionProperty);
            }
            set
            {
                this.SetValue(DragDecorator.IconPositionProperty, (object)value);
            }
        }

        /// <summary>Gets or sets the drag drop effects.</summary>
        public DragDropEffects Effects
        {
            get
            {
                return (DragDropEffects)this.GetValue(DragDecorator.EffectsProperty);
            }
            set
            {
                this.SetValue(DragDecorator.EffectsProperty, (object)value);
            }
        }

        /// <summary>EffectsProperty property changed handler.</summary>
        /// <param name="d">DragContainer that changed its Effects.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnEffectsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DragDecorator).OnEffectsPropertyChanged();
        }

        /// <summary>
        /// Changes the appearance of the DragDecorator when the effects
        /// property is changed.
        /// </summary>
        private void OnEffectsPropertyChanged()
        {
            this.Update();
        }

        /// <summary>Updates the visual state of the DragContainer.</summary>
        private void Update()
        {
            if ((this.Effects & DragDropEffects.Move) == DragDropEffects.Move)
                VisualStateManager.GoToState((Control)this, DragDropEffects.Move.ToString(), false);
            else if ((this.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                VisualStateManager.GoToState((Control)this, DragDropEffects.Copy.ToString(), false);
            else if ((this.Effects & DragDropEffects.Link) == DragDropEffects.Link)
                VisualStateManager.GoToState((Control)this, DragDropEffects.Link.ToString(), false);
            else if ((this.Effects & DragDropEffects.Scroll) == DragDropEffects.Scroll)
            {
                VisualStateManager.GoToState((Control)this, DragDropEffects.Scroll.ToString(), false);
            }
            else
            {
                int effects = (int)this.Effects;
                if (false)
                    return;
                VisualStateManager.GoToState((Control)this, DragDropEffects.None.ToString(), false);
            }
        }

        /// <summary>Initializes a new instance of the DragDecorator.</summary>
        public DragDecorator()
        {
            this.DefaultStyleKey = (object)typeof(DragDecorator);
            this.Loaded += (RoutedEventHandler)delegate
            {
                this.Update();
            };
        }
    }
}
