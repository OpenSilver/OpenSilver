// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Positions child elements sequentially from left to right or top to
    /// bottom.  When elements extend beyond the panel edge, elements are
    /// positioned in the next row or column.
    /// </summary>
    public class WrapPanel : Panel
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignorePropertyChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanel"/> class.
        /// </summary>
        public WrapPanel()
        {
        }

        /// <summary>
        /// Gets or sets the direction in which child elements are arranged.
        /// </summary>
        /// <value>
        /// One of the <see cref="Orientation" /> values. The default is
        /// <see cref="Orientation.Horizontal" />.
        /// </value>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Orientation" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="Orientation" /> dependency property.
        /// </value>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(WrapPanel),
                new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        /// <summary>
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="d">WrapPanel that changed its Orientation.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WrapPanel source = (WrapPanel)d;
            Orientation value = (Orientation)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Validate the Orientation
            if ((value != Orientation.Horizontal) &&
                (value != Orientation.Vertical))
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(OrientationProperty, (Orientation)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid Orientation value '{0}'.",
                    value);
                throw new ArgumentException(message, "value");
            }

            // Orientation affects measuring.
            source.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets the height of the layout area for each item that is contained 
        /// in a <see cref="WrapPanel" />.
        /// </summary>
        /// <value>
        /// The height applied to the layout area of each item that is contained within 
        /// a <see cref="WrapPanel" />. The default value is <see cref="double.NaN" />.
        /// </value>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ItemHeight" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="ItemHeight" /> dependency property.
        /// </value>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                nameof(ItemHeight),
                typeof(double),
                typeof(WrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        /// <summary>
        /// Gets or sets the width of the layout area for each item that is contained 
        /// in a <see cref="WrapPanel" />.
        /// </summary>
        /// <value>
        /// The width that applies to the layout area of each item that is contained 
        /// in a <see cref="WrapPanel" />. The default value is <see cref="double.NaN" />.
        /// </value>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ItemWidth" /> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="ItemWidth" /> dependency property.
        /// </value>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(
                nameof(ItemWidth),
                typeof(double),
                typeof(WrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        /// <summary>
        /// Property changed handler for ItemHeight and ItemWidth.
        /// </summary>
        /// <param name="d">
        /// WrapPanel that changed its ItemHeight or ItemWidth.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemHeightOrWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WrapPanel source = (WrapPanel)d;
            double value = (double)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Validate the length (which must either be NaN or a positive,
            // finite number)
            if (!double.IsNaN(value) && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, (double)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid length value '{0}'.",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        /// <summary>
        /// Measures the child elements of a <see cref="WrapPanel" /> in anticipation
        /// of arranging them during the <see cref="ArrangeOverride(Size)" /> pass.
        /// </summary>
        /// <param name="constraint">
        /// The size available to child elements of the wrap panel.
        /// </param>
        /// <returns>
        /// The size required by the
        /// <see cref="WrapPanel" /> and its elements.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // Variables tracking the size of the current line, the total size
            // measured so far, and the maximum size available to fill.  Note
            // that the line might represent a row or a column depending on the
            // orientation.
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize totalSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, constraint.Width, constraint.Height);

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            Size itemSize = new Size(
                hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);

            // Measure each of the Children
            foreach (UIElement element in Children)
            {
                // Determine the size of the element
                element.Measure(itemSize);
                OrientedSize elementSize = new OrientedSize(
                    o,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (NumericExtensions.IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct))
                {
                    // Update the total size with the direct and indirect growth
                    // for the current line
                    totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
                    totalSize.Indirect += lineSize.Indirect;

                    // Move the element to a new line
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size,
                    // place it on a line by itself
                    if (NumericExtensions.IsGreaterThan(elementSize.Direct, maximumSize.Direct))
                    {
                        // Update the total size for the line occupied by this
                        // single element
                        totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
                        totalSize.Indirect += elementSize.Indirect;

                        // Move to a new line
                        lineSize = new OrientedSize(o);
                    }
                }
                else
                {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Update the total size with the elements on the last line
            totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            totalSize.Indirect += lineSize.Indirect;

            // Return the total size required as an un-oriented quantity
            return new Size(totalSize.Width, totalSize.Height);
        }

        /// <summary>
        /// Arranges and sizes the
        /// <see cref="WrapPanel" /> control and its child elements.
        /// </summary>
        /// <param name="finalSize">
        /// The area within the parent that the
        /// <see cref="WrapPanel" /> should use  arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="WrapPanel" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Variables tracking the size of the current line, and the maximum
            // size available to fill.  Note that the line might represent a row
            // or a column depending on the orientation.
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, finalSize.Width, finalSize.Height);

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            double indirectOffset = 0;
            double? directDelta = (o == Orientation.Horizontal) ?
                (hasFixedWidth ? itemWidth : null) :
                (hasFixedHeight ? itemHeight : null);

            // Measure each of the Children.  We will process the elements one
            // line at a time, just like during measure, but we will wait until
            // we've completed an entire line of elements before arranging them.
            // The lineStart and lineEnd variables track the size of the
            // currently arranged line.
            UIElementCollection children = Children;
            int count = children.Count;
            int lineStart = 0;
            for (int lineEnd = 0; lineEnd < count; lineEnd++)
            {
                UIElement element = children[lineEnd];

                // Get the size of the element
                OrientedSize elementSize = new OrientedSize(
                    o,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                // If this element falls of the edge of the line
                if (NumericExtensions.IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct))
                {
                    // Then we just completed a line and we should arrange it
                    ArrangeLine(lineStart, lineEnd, directDelta, indirectOffset, lineSize.Indirect);

                    // Move the current element to a new line
                    indirectOffset += lineSize.Indirect;
                    lineSize = elementSize;

                    // If the current element is larger than the maximum size
                    if (NumericExtensions.IsGreaterThan(elementSize.Direct, maximumSize.Direct))
                    {
                        // Arrange the element as a single line
                        ArrangeLine(lineEnd, ++lineEnd, directDelta, indirectOffset, elementSize.Indirect);

                        // Move to a new line
                        indirectOffset += lineSize.Indirect;
                        lineSize = new OrientedSize(o);
                    }

                    // Advance the start index to a new line after arranging
                    lineStart = lineEnd;
                }
                else
                {
                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            // Arrange any elements on the last line
            if (lineStart < count)
            {
                ArrangeLine(lineStart, count, directDelta, indirectOffset, lineSize.Indirect);
            }

            return finalSize;
        }

        /// <summary>
        /// Arrange a sequence of elements in a single line.
        /// </summary>
        /// <param name="lineStart">
        /// Index of the first element in the sequence to arrange.
        /// </param>
        /// <param name="lineEnd">
        /// Index of the last element in the sequence to arrange.
        /// </param>
        /// <param name="directDelta">
        /// Optional fixed growth in the primary direction.
        /// </param>
        /// <param name="indirectOffset">
        /// Offset of the line in the indirect direction.
        /// </param>
        /// <param name="indirectGrowth">
        /// Shared indirect growth of the elements on this line.
        /// </param>
        private void ArrangeLine(int lineStart, int lineEnd, double? directDelta, double indirectOffset, double indirectGrowth)
        {
            double directOffset = 0.0;

            Orientation o = Orientation;
            bool isHorizontal = o == Orientation.Horizontal;

            UIElementCollection children = Children;
            for (int index = lineStart; index < lineEnd; index++)
            {
                // Get the size of the element
                UIElement element = children[index];
                OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                // Determine if we should use the element's desired size or the
                // fixed item width or height
                double directGrowth = directDelta != null ?
                    directDelta.Value :
                    elementSize.Direct;

                // Arrange the element
                Rect bounds = isHorizontal ?
                    new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                element.Arrange(bounds);

                directOffset += directGrowth;
            }
        }

        /// <summary>
        /// The OrientedSize structure is used to abstract the growth direction from
        /// the layout algorithms of WrapPanel.  When the growth direction is
        /// oriented horizontally (ex: the next element is arranged on the side of
        /// the previous element), then the Width grows directly with the placement
        /// of elements and Height grows indirectly with the size of the largest
        /// element in the row.  When the orientation is reversed, so is the
        /// directional growth with respect to Width and Height.
        /// </summary>
        private struct OrientedSize
        {
            private readonly Orientation _orientation;

            /// <summary>
            /// Gets or sets the size dimension that grows directly with layout
            /// placement.
            /// </summary>
            internal double Direct;

            /// <summary>
            /// Gets or sets the size dimension that grows indirectly with the
            /// maximum value of the layout row or column.
            /// </summary>
            internal double Indirect;

            /// <summary>
            /// Initializes a new OrientedSize structure.
            /// </summary>
            /// <param name="orientation">Orientation of the structure.</param>
            public OrientedSize(Orientation orientation)
            {
                _orientation = orientation;
                Direct = Indirect = 0.0;
            }

            /// <summary>
            /// Initializes a new OrientedSize structure.
            /// </summary>
            /// <param name="orientation">Orientation of the structure.</param>
            /// <param name="width">Un-oriented width of the structure.</param>
            /// <param name="height">Un-oriented height of the structure.</param>
            public OrientedSize(Orientation orientation, double width, double height)
            {
                _orientation = orientation;
                Direct = Indirect = 0.0;

                Width = width;
                Height = height;
            }

            /// <summary>
            /// Gets or sets the width of the size.
            /// </summary>
            public double Width
            {
                get { return _orientation == Orientation.Horizontal ? Direct : Indirect; }
                set { if (_orientation == Orientation.Horizontal) Direct = value; else Indirect = value; }
            }

            /// <summary>
            /// Gets or sets the height of the size.
            /// </summary>
            public double Height
            {
                get { return _orientation != Orientation.Horizontal ? Direct : Indirect; }
                set { if (_orientation != Orientation.Horizontal) Direct = value; else Indirect = value; }
            }
        }
    }
}
