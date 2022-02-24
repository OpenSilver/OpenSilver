// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if MIGRATION
using System.Windows.Shapes;
#else
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// An axis that has a range.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class RangeAxis : DisplayAxis //, IRangeAxis, IValueMarginConsumer
    {
        #region public Style MinorTickMarkStyle
        /// <summary>
        /// Gets or sets the minor tick mark style.
        /// </summary>
        public Style MinorTickMarkStyle
        {
            get { return GetValue(MinorTickMarkStyleProperty) as Style; }
            set { SetValue(MinorTickMarkStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the MinorTickMarkStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorTickMarkStyleProperty =
            DependencyProperty.Register(
                "MinorTickMarkStyle",
                typeof(Style),
                typeof(RangeAxis),
                new PropertyMetadata(null));

        #endregion public Style MinorTickMarkStyle

        /// <summary>
        /// The maximum value displayed in the range axis.
        /// </summary>
        private IComparable _protectedMaximum;

        /// <summary>
        /// Gets or sets the maximum value displayed in the range axis.
        /// </summary>
        [OpenSilver.NotImplemented]
        protected IComparable ProtectedMaximum
        {
            get
            {
                return _protectedMaximum;
            }
            set
            {
                //if (value != null && ProtectedMinimum != null && ValueHelper.Compare(ProtectedMinimum, value) > 0)
                //{
                //    throw new InvalidOperationException(Properties.Resources.RangeAxis_MaximumValueMustBeLargerThanOrEqualToMinimumValue);
                //}
                if (!object.ReferenceEquals(_protectedMaximum, value) && !object.Equals(_protectedMaximum, value))
                {
                    _protectedMaximum = value;
                    UpdateActualRange();
                }
            }
        }

        /// <summary>
        /// The minimum value displayed in the range axis.
        /// </summary>
        private IComparable _protectedMinimum;

        /// <summary>
        /// Gets or sets the minimum value displayed in the range axis.
        /// </summary>
        [OpenSilver.NotImplemented]
        protected IComparable ProtectedMinimum
        {
            get
            {
                return _protectedMinimum;
            }
            set
            {
                //if (value != null && ProtectedMaximum != null && ValueHelper.Compare(value, ProtectedMaximum) > 0)
                //{
                //    throw new InvalidOperationException(Properties.Resources.RangeAxis_MinimumValueMustBeLargerThanOrEqualToMaximumValue);
                //}
                if (!object.ReferenceEquals(_protectedMinimum, value) && !object.Equals(_protectedMinimum, value))
                {
                    _protectedMinimum = value;
                    UpdateActualRange();
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the RangeAxis class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static RangeAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeAxis), new FrameworkPropertyMetadata(typeof(RangeAxis)));
        }

#endif
        /// <summary>
        /// Instantiates a new instance of the RangeAxis class.
        /// </summary>
        [OpenSilver.NotImplemented]
        protected RangeAxis()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(RangeAxis);
#endif
            //this._labelPool = new ObjectPool<Control>(() => CreateAxisLabel());
            //this._majorTickMarkPool = new ObjectPool<Line>(() => CreateMajorTickMark());
            //this._minorTickMarkPool = new ObjectPool<Line>(() => CreateMinorTickMark());

            // Update actual range when size changes for the first time.  This
            // is necessary because the value margins may have changed after
            // the first layout pass.
            SizeChangedEventHandler handler = null;
            handler = delegate
            {
                SizeChanged -= handler;
                UpdateActualRange();
            };
            SizeChanged += handler;
        }

        /// <summary>
        /// Creates a minor axis tick mark.
        /// </summary>
        /// <returns>A line to used to render a tick mark.</returns>
        protected Line CreateMinorTickMark()
        {
            return CreateTickMark(MinorTickMarkStyle);
        }

        /// <summary>
        /// Updates the actual range displayed on the axis.
        /// </summary>
        [OpenSilver.NotImplemented]
        private void UpdateActualRange()
        {
        }

        /// <summary>
        /// Renders the axis as an oriented axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        [OpenSilver.NotImplemented]
        private void RenderOriented(Size availableSize)
        {
        }

        /// <summary>
        /// Renders the axis labels, tick marks, and other visual elements.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        protected override void Render(Size availableSize)
        {
            RenderOriented(availableSize);
        }

        /// <summary>
        /// Returns a sequence of the values at which to plot major grid lines.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of the values at which to plot major grid lines.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "This is the expected capitalization.")]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may do a lot of work and is therefore not a suitable candidate for a property.")]
        protected virtual IEnumerable<IComparable> GetMajorGridLineValues(Size availableSize)
        {
            return GetMajorTickMarkValues(availableSize);
        }

        /// <summary>
        /// Returns a sequence of values to plot on the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may do a lot of work and is therefore not a suitable candidate for a property.")]
        protected abstract IEnumerable<IComparable> GetMajorTickMarkValues(Size availableSize);

        /// <summary>
        /// Returns a sequence of values to plot on the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may do a lot of work and is therefore not a suitable candidate for a property.")]
        protected virtual IEnumerable<IComparable> GetMinorTickMarkValues(Size availableSize)
        {
            yield break;
        }

        /// <summary>
        /// Returns a sequence of values to plot on the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>A sequence of values to plot on the axis.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method may do a lot of work and is therefore not a suitable candidate for a property.")]
        protected abstract IEnumerable<IComparable> GetLabelValues(Size availableSize);

        /// <summary>
        /// Gets the origin value on the axis.
        /// </summary>
        protected abstract IComparable Origin { get; }
    }
}