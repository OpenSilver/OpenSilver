// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if MIGRATION
using System.Windows.Media;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting.Primitives
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting.Primitives
#endif
{
    /// <summary>
    /// Defines an area where you can arrange child elements either horizontally
    /// or vertically, relative to each other.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [OpenSilver.NotImplemented]
    public class EdgePanel : Panel
    {
        /// <summary>
        /// The maximum number of iterations.
        /// </summary>
        private const int MaximumIterations = 10;

        /// <summary>
        /// A flag that ignores a property change when set.
        /// </summary>
        private static bool _ignorePropertyChange;

        #region public attached Edge Edge
        /// <summary>
        /// Gets the value of the Edge attached property for a specified
        /// UIElement.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>The Edge property value for the element.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "EdgePanel only has UIElement children")]
        public static Edge GetEdge(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Edge)element.GetValue(EdgeProperty);
        }

        /// <summary>
        /// Sets the value of the Edge attached property to a specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the attached property is written.
        /// </param>
        /// <param name="edge">The needed Edge value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "EdgePanel only has UIElement children")]
        public static void SetEdge(UIElement element, Edge edge)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(EdgeProperty, edge);
        }

        /// <summary>
        /// Identifies the Edge dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeProperty =
            DependencyProperty.RegisterAttached(
                "Edge",
                typeof(Edge),
                typeof(EdgePanel),
                new PropertyMetadata(Edge.Center, OnEdgePropertyChanged));

        /// <summary>
        /// EdgeProperty property changed handler.
        /// </summary>
        /// <param name="d">UIElement that changed its Edge.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Almost always set from the attached property CLR setter.")]
        private static void OnEdgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Ignore the change if requested
            if (_ignorePropertyChange)
            {
                _ignorePropertyChange = false;
                return;
            }

            UIElement element = (UIElement)d;
            Edge value = (Edge)e.NewValue;

            // Validate the Edge property
            if ((value != Edge.Left) &&
                (value != Edge.Top) &&
                (value != Edge.Right) &&
                (value != Edge.Center) &&
                (value != Edge.Bottom))
            {
                // Reset the property to its original state before throwing
                _ignorePropertyChange = true;
                element.SetValue(EdgeProperty, (Edge)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    OpenSilver.Controls.DataVisualization.Properties.Resources.EdgePanel_OnEdgePropertyChanged,
                    value);

                throw new ArgumentException(message, "value");
            }

            // Cause the EdgePanel to update its layout when a child changes
            EdgePanel panel = VisualTreeHelper.GetParent(element) as EdgePanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }
        #endregion public attached Edge Edge

        /// <summary>
        /// Initializes a new instance of the EdgePanel class.
        /// </summary>
        public EdgePanel()
        {
            this.SizeChanged += new SizeChangedEventHandler(EdgePanelSizeChanged);
        }

        /// <summary>
        /// Invalidate measure when edge panel is resized.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void EdgePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        /// <summary>
        /// The left rectangle in which to render left elements.
        /// </summary>
        private Rect _leftRect;

        /// <summary>
        /// The right rectangle in which to render right elements.
        /// </summary>
        private Rect _rightRect;

        /// <summary>
        /// The top rectangle in which to render top elements.
        /// </summary>
        private Rect _topRect;

        /// <summary>
        /// The bottom rectangle in which to render bottom elements.
        /// </summary>
        private Rect _bottomRect;

        /// <summary>
        /// Measures the children of a EdgePanel in anticipation of arranging
        /// them during the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">A maximum Size to not exceed.</param>
        /// <returns>The desired size of the EdgePanel.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is by nature difficult to refactor into several methods.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting up method will make it more difficult to understand.")]
        [OpenSilver.NotImplemented]
        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        /// <summary>
        /// Arranges the content (child elements) of a EdgePanel element.
        /// </summary>
        /// <param name="arrangeSize">
        /// The Size the EdgePanel uses to arrange its child elements.
        /// </param>
        /// <returns>The arranged size of the EdgePanel.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting up method will make it more difficult to understand.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [OpenSilver.NotImplemented]
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return arrangeSize;
        }

        /// <summary>
        /// Creates a Rect safely by forcing width/height to be valid.
        /// </summary>
        /// <param name="left">Rect left parameter.</param>
        /// <param name="top">Rect top parameter.</param>
        /// <param name="width">Rect width parameter.</param>
        /// <param name="height">Rect height parameter.</param>
        /// <returns>New Rect struct.</returns>
        private static Rect SafeCreateRect(double left, double top, double width, double height)
        {
            return new Rect(left, top, Math.Max(0.0, width), Math.Max(0.0, height));
        }
    }
}