// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
using SW = Microsoft.Windows;
#else
using Windows.Foundation;
using SW = System.Windows;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
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
        #region public Point IconPosition
        /// <summary>
        /// Gets or sets the icon position.
        /// </summary>
        public Point IconPosition
        {
            get { return (Point)GetValue(IconPositionProperty); }
            set { SetValue(IconPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the icon position dependency property.
        /// </summary>
        public static readonly DependencyProperty IconPositionProperty =
            DependencyProperty.Register(
                "IconPosition",
                typeof(Point),
                typeof(DragDecorator),
                new PropertyMetadata(new Point(0.0, 0.0)));

        #endregion public Point IconPosition

        #region public SW.DragDropEffects Effects
        /// <summary>
        /// Gets or sets the drag drop effects.
        /// </summary>
        public SW.DragDropEffects Effects
        {
            get { return (SW.DragDropEffects)GetValue(EffectsProperty); }
            set { SetValue(EffectsProperty, value); }
        }

        /// <summary>
        /// Identifies the Effects dependency property.
        /// </summary>
        public static readonly DependencyProperty EffectsProperty =
            DependencyProperty.Register(
                "Effects",
                typeof(SW.DragDropEffects),
                typeof(DragDecorator),
                new PropertyMetadata(SW.DragDropEffects.Copy | SW.DragDropEffects.Move | SW.DragDropEffects.Scroll | SW.DragDropEffects.Link, OnEffectsPropertyChanged));

        /// <summary>
        /// EffectsProperty property changed handler.
        /// </summary>
        /// <param name="d">DragContainer that changed its Effects.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnEffectsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragDecorator source = d as DragDecorator;
            source.OnEffectsPropertyChanged();
        }

        /// <summary>
        /// Changes the appearance of the DragDecorator when the effects 
        /// property is changed.
        /// </summary>
        private void OnEffectsPropertyChanged()
        {
            Update();
        }
        #endregion public SW.DragDropEffects Effects

        /// <summary>
        /// Updates the visual state of the DragContainer.
        /// </summary>
        private void Update()
        {
            if ((Effects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
            {
                VisualStateManager.GoToState(this, SW.DragDropEffects.Move.ToString(), false);
            }
            else if ((Effects & SW.DragDropEffects.Copy) == SW.DragDropEffects.Copy)
            {
                VisualStateManager.GoToState(this, SW.DragDropEffects.Copy.ToString(), false);
            }
            else if ((Effects & SW.DragDropEffects.Link) == SW.DragDropEffects.Link)
            {
                VisualStateManager.GoToState(this, SW.DragDropEffects.Link.ToString(), false);
            }
            else if ((Effects & SW.DragDropEffects.Scroll) == SW.DragDropEffects.Scroll)
            {
                VisualStateManager.GoToState(this, SW.DragDropEffects.Scroll.ToString(), false);
            }
            else if ((Effects & SW.DragDropEffects.None) == SW.DragDropEffects.None)
            {
                VisualStateManager.GoToState(this, SW.DragDropEffects.None.ToString(), false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DragDecorator.
        /// </summary>
        public DragDecorator()
        {
            this.DefaultStyleKey = typeof(DragDecorator);
            this.Loaded +=
                delegate
                {
                    Update();
                };
        }
    }
}