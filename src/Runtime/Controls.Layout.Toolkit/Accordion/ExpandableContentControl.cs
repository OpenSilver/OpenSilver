// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if OPENSILVER
using Properties = OpenSilver.Controls.Properties;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control with a single piece of content that expands or 
    /// collapses in a sliding motion to a specified desired size.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = ElementContentSiteName, Type = typeof(ContentPresenter))]
    public class ExpandableContentControl : ContentControl
    {
        /// <summary>
        /// The Geometry used to clip this control. The control will potentially
        /// have less available space than the content it is arranging. That
        /// part will be clipped.
        /// </summary>
        private readonly RectangleGeometry _clippingRectangle;

        #region TemplateParts
        /// <summary>
        /// The name of the ContentSite template part.
        /// </summary>
        private const string ElementContentSiteName = "ContentSite";

        /// <summary>
        /// BackingField for the ContentSite property.
        /// </summary>
        private ContentPresenter _contentSite;

        /// <summary>
        /// Gets or sets the ContentSite template part.
        /// </summary>
        private ContentPresenter ContentSite
        {
            get
            {
                return _contentSite;
            }
            set
            {
                if (_contentSite != null)
                {
                    _contentSite.SizeChanged -= OnContentSiteSizeChanged;
                }
                _contentSite = value;
                if (_contentSite != null)
                {
                    _contentSite.SizeChanged += OnContentSiteSizeChanged;
                }
            }
        }
        #endregion TemplateParts

        #region public ExpandDirection RevealMode
        /// <summary>
        /// Gets or sets the direction in which the ExpandableContentControl 
        /// content window opens.
        /// </summary>
        public ExpandDirection RevealMode
        {
            get { return (ExpandDirection)GetValue(RevealModeProperty); }
            set { SetValue(RevealModeProperty, value); }
        }

        /// <summary>
        /// Identifies the RevealMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RevealModeProperty =
            DependencyProperty.Register(
                "RevealMode",
                typeof(ExpandDirection),
                typeof(ExpandableContentControl),
                new PropertyMetadata(ExpandDirection.Down, OnRevealModePropertyChanged));

        /// <summary>
        /// RevealModeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandableContentControl that changed its RevealMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRevealModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            ExpandDirection value = (ExpandDirection)e.NewValue;

            if (value != ExpandDirection.Down &&
                value != ExpandDirection.Left &&
                value != ExpandDirection.Right &&
                value != ExpandDirection.Up)
            {
                // revert to old value
                source.RevealMode = (ExpandDirection)e.OldValue;

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Expander_OnExpandDirectionPropertyChanged_InvalidValue,
                    value);
                throw new ArgumentException(message, "e");
            }

            // set the non-reveal dimension
            source.SetNonRevealDimension();

            // calculate the reveal dimension
            source.SetRevealDimension();
        }

        /// <summary>
        /// Gets a value indicating whether the content should be revealed vertically.
        /// </summary>
        private bool IsVerticalRevealMode
        {
            get { return RevealMode == ExpandDirection.Down || RevealMode == ExpandDirection.Up; }
        }

        /// <summary>
        /// Gets a value indicating whether the content should be revealed horizontally.
        /// </summary>
        private bool IsHorizontalRevealMode
        {
            get { return RevealMode == ExpandDirection.Left || RevealMode == ExpandDirection.Right; }
        }
        #endregion public ExpandDirection RevealMode

        #region public double Percentage
        /// <summary>
        /// Gets or sets the relative percentage of the content that is 
        /// currently visible. A percentage of 1 corresponds to the complete
        /// TargetSize.
        /// </summary>
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        /// <summary>
        /// Identifies the Percentage dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(
                "Percentage",
                typeof(double),
                typeof(ExpandableContentControl),
                new PropertyMetadata(0.0, OnPercentagePropertyChanged));

        /// <summary>
        /// PercentageProperty property changed handler.
        /// </summary>
        /// <param name="d">Page that changed its Percentage.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPercentagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            source.SetRevealDimension();
        }
        #endregion public double Percentage

        #region public Size TargetSize
        /// <summary>
        /// Gets or sets the desired size of the ExpandableContentControl content.
        /// </summary>
        /// <remarks>Use the percentage property to animate to this size.</remarks>
        public Size TargetSize
        {
            get { return (Size)GetValue(TargetSizeProperty); }
            set { SetValue(TargetSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the TargetSize dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetSizeProperty =
            DependencyProperty.Register(
                "TargetSize",
                typeof(Size),
                typeof(ExpandableContentControl),
                new PropertyMetadata(new Size(double.NaN, double.NaN), OnTargetSizePropertyChanged));

        /// <summary>
        /// TargetSizeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandableContentControl that changed its TargetSize.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTargetSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            Size value = (Size)e.NewValue;
            // recalculate percentage based on this new targetsize.
            // for instance, percentage was at 1 and width was 100. Now width was changed to 200, this means
            // that the percentage needs to be set to 0.5 so that a reveal animation can be started again.

            source.RecalculatePercentage(value);
        }
        #endregion public Size TargetSize

        /// <summary>
        /// Occurs when the content changed its size.
        /// </summary>
        public event SizeChangedEventHandler ContentSizeChanged;

        /// <summary>
        /// Does a measure pass of the control and its content. The content will
        /// get measured according to the TargetSize and is clipped.
        /// </summary>
        /// <param name="availableSize">The available size that this object can 
        /// give to child objects. Infinity can be specified as a value to 
        /// indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based 
        /// on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // this control will always follow the TargetSize
            // and allow its content to take all the space it needs

            if (ContentSite != null)
            {
                // we will adhere to the available size, to allow scrollbars
                // to appear                
                Size desiredSize = availableSize;
                if (Percentage != 1)
                {
                    // we shall use the targetsize, to allow the content
                    // to adjust to the final size it should be.
                    desiredSize = CalculateDesiredContentSize();
                }
                MeasureContent(desiredSize);
                return ContentSite.DesiredSize;
            }
            return new Size(0, 0);
        }

        /// <summary>
        /// Measures the content with a specific size.
        /// </summary>
        /// <param name="desiredSize">The size passed to the content.</param>
        internal void MeasureContent(Size desiredSize)
        {
            if (ContentSite != null)
            {
                ContentSite.Measure(desiredSize);
            }
        }

        /// <summary>
        /// Interprets TargetSize.
        /// </summary>
        /// <returns>A size that can be safely used to measure content.</returns>
        internal Size CalculateDesiredContentSize()
        {
            Size desiredSize = TargetSize;
            if (desiredSize.Width.Equals(Double.NaN))
            {
                desiredSize.Width = Double.PositiveInfinity;
            }
            if (desiredSize.Height.Equals(Double.NaN))
            {
                desiredSize.Height = Double.PositiveInfinity;
            }
            return desiredSize;
        }

        /// <summary>
        /// Arranges the control and its content. Content is arranged according
        /// to the TargetSize and clipped.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this 
        /// object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ContentSite == null)
            {
                return finalSize;
            }

            // arrange content as if filling the targetSize
            Size desiredSize = TargetSize;

            // if a direction is not set, use given size.
            if (desiredSize.Width.Equals(Double.NaN))
            {
                desiredSize.Width = finalSize.Width;
            }
            if (desiredSize.Height.Equals(Double.NaN))
            {
                desiredSize.Height = finalSize.Height;
            }

            Rect finalRect = new Rect(new Point(0, 0), desiredSize);
            ContentSite.Arrange(finalRect);

            Size actualSize = new Size(
                    (IsHorizontalRevealMode ? Width : finalSize.Width),
                    (IsVerticalRevealMode ? Height : finalSize.Height));

            if (Double.IsNaN(actualSize.Width))
            {
                actualSize.Width = finalSize.Width;
            }
            if (Double.IsNaN(actualSize.Height))
            {
                actualSize.Height = finalSize.Height;
            }

            UpdateClip(actualSize);

            return actualSize;
        }

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        /// <param name="arrangeSize">Size of the visible part of the control.</param>
        private void UpdateClip(Size arrangeSize)
        {
            if (Clip != _clippingRectangle)
            {
                Clip = _clippingRectangle;
            }
            _clippingRectangle.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
        }

        /// <summary>
        /// Recalculates the percentage based on a new size.
        /// </summary>
        /// <param name="value">The new size used to base percentages on.</param>
        internal void RecalculatePercentage(Size value)
        {
            if (ContentSite != null)
            {
                if (IsVerticalRevealMode)
                {
                    Percentage = ActualHeight / (double.IsNaN(value.Height) ? ContentSite.DesiredSize.Height : value.Height);
                }
                else if (IsHorizontalRevealMode)
                {
                    Percentage = ActualWidth / (double.IsNaN(value.Width) ? ContentSite.DesiredSize.Width : value.Width);
                }
            }
            else
            {
                Percentage = 0.0;
            }
        }

        /// <summary>
        /// Sets the dimensions according to the current percentage.
        /// </summary>
        private void SetRevealDimension()
        {
            if (ContentSite == null)
            {
                return;
            }

            if (IsHorizontalRevealMode)
            {
                double targetWidth = TargetSize.Width;
                if (Double.IsNaN(targetWidth))
                {
                    // NaN has the same meaning as autosize, which in this context means the desired size
                    targetWidth = ContentSite.ActualWidth;
                }

                Width = Percentage * targetWidth;
            }

            if (IsVerticalRevealMode)
            {
                double targetHeight = TargetSize.Height;
                if (Double.IsNaN(targetHeight))
                {
                    // NaN has the same meaning as autosize, which in this context means the desired size
                    targetHeight = ContentSite.ActualHeight;
                }

                Height = Percentage * targetHeight;
            }
        }

        /// <summary>
        /// Sets the opposite dimension.
        /// </summary>
        private void SetNonRevealDimension()
        {
            if (IsHorizontalRevealMode)
            {
                // reset height to target size height. This can be double.NaN (auto size)
                Height = TargetSize.Height;
            }

            if (IsVerticalRevealMode)
            {
                // reset width to target size width. This can be double.NaN (auto size)
                Width = TargetSize.Width;
            }
        }

        /// <summary>
        /// Gets the content current visible size.
        /// </summary>
        internal Size RelevantContentSize
        {
            get
            {
                return new Size(
                    (IsHorizontalRevealMode ? Width : 0),
                    (IsVerticalRevealMode ? Height : 0));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableContentControl"/> class.
        /// </summary>
        public ExpandableContentControl()
        {
            DefaultStyleKey = typeof(ExpandableContentControl);
            _clippingRectangle = new RectangleGeometry();
            Clip = _clippingRectangle;
        }

        /// <summary>
        /// Builds the visual tree for the ExpandableContentControl control when a 
        /// new template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            ContentSite = GetTemplateChild(ElementContentSiteName) as ContentPresenter;

            SetRevealDimension();

            SetNonRevealDimension();
        }

        #region Reactive
        /// <summary>
        /// Raises the ContentSizeChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnContentSiteSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChangedEventHandler handler = ContentSizeChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
