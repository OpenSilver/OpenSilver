// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the control that shows a preview of the GridSplitter's
    /// redistribution of space between columns or rows of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = PreviewControl.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PreviewControl.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    internal partial class PreviewControl : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementVerticalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Is Null until the PreviewControl is bound to a GridSplitter.
        /// </summary>
        private GridSplitter.GridResizeDirection _currentGridResizeDirection;

        /// <summary>
        /// Tracks the bound GridSplitter's location for calculating the
        /// PreviewControl's offset.
        /// </summary>
        private Point _gridSplitterOrigin;

        /// <summary>
        /// Instantiate the PreviewControl.
        /// </summary>
        public PreviewControl()
        {
            _gridSplitterOrigin = new Point();
        }

        /// <summary>
        /// Called when template should be applied to the control.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(PreviewControl.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(PreviewControl.ElementVerticalTemplateName) as FrameworkElement;

            if (_currentGridResizeDirection == GridSplitter.GridResizeDirection.Columns)
            {
                if (ElementHorizontalTemplateFrameworkElement != null)
                {
                    ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
                if (ElementVerticalTemplateFrameworkElement != null)
                {
                    ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (ElementHorizontalTemplateFrameworkElement != null)
                {
                    ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                }
                if (ElementVerticalTemplateFrameworkElement != null)
                {
                    ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Bind the the dimensions of the preview control to the associated
        /// grid splitter.
        /// </summary>
        /// <param name="gridSplitter">GridSplitter instance to target.</param>
        public void Bind(GridSplitter gridSplitter)
        {
            Debug.Assert(gridSplitter != null, "gridSplitter should not be null!");
            Debug.Assert(gridSplitter.Parent != null, "gridSplitter.Parent should not be null!");

            this.Style = gridSplitter.PreviewStyle;
            this.Height = gridSplitter.ActualHeight;
            this.Width = gridSplitter.ActualWidth;

            if (gridSplitter.ResizeDataInternal != null)
            {
                _currentGridResizeDirection = gridSplitter.ResizeDataInternal.ResizeDirection;
            }

            GeneralTransform gt = gridSplitter.TransformToVisual((UIElement)gridSplitter.Parent);
            Point p = new Point(0, 0);
            p = gt.Transform(p);

            _gridSplitterOrigin.X = p.X;
            _gridSplitterOrigin.Y = p.Y;

            SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X);
            SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y);
        }

        /// <summary>
        /// Gets or sets the x-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(Canvas.LeftProperty) - _gridSplitterOrigin.X; }
            set { SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X + value); }
        }

        /// <summary>
        /// Gets or sets the y-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(Canvas.TopProperty) - _gridSplitterOrigin.Y; }
            set { SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y + value); }
        }
    }
}