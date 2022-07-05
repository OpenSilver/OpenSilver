using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>Provides information to a RangeConsumer.</summary>
    public interface IRangeProvider
    {
        /// <summary>Returns the range of values.</summary>
        /// <param name="rangeConsumer">The range consumer requesting the data
        /// range.</param>
        /// <returns>A data range.</returns>
        Range<IComparable> GetRange(IRangeConsumer rangeConsumer);
    }

    /// <summary>An object that consumes a range.</summary>
    public interface IRangeConsumer
    {
        /// <summary>
        /// Informs a range consumer that a provider's range has changed.
        /// </summary>
        /// <param name="provider">The range provider.</param>
        /// <param name="range">The range of data.</param>
        void RangeChanged(IRangeProvider provider, Range<IComparable> range);
    }

    /// <summary>An axis with a range.</summary>
    public interface IRangeAxis : IAxis, IRangeConsumer
    {
        /// <summary>Gets the range of values displayed on the axis.</summary>
        System.Windows.Controls.DataVisualization.Range<IComparable> Range { get; }

        /// <summary>The plot area coordinate of a value.</summary>
        /// <param name="position">The position at which to retrieve the plot
        /// area coordinate.</param>
        /// <returns>The plot area coordinate.</returns>
        IComparable GetValueAtPosition(UnitValue position);

        /// <summary>Gets the origin value on the axis.</summary>
        IComparable Origin { get; }
    }


    /// <summary>
    /// Range axes look for this interface on series to determine whether to
    /// anchor the origin to the bottom or top of the screen where possible.
    /// </summary>
    /// <remarks>
    /// Implementing this interface ensures that value margins will not cause
    /// an origin to float above the bottom or top of the screen if no
    /// data exists below or above.
    /// </remarks>
    public interface IAnchoredToOrigin
    {
        /// <summary>Gets the axis to which the data is anchored.</summary>
        IRangeAxis AnchoredAxis { get; }
    }


    /// <summary>
    /// Defines methods on classes that contain a global index.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IRequireGlobalSeriesIndex
    {
        /// <summary>Occurs when a global series index changes.</summary>
        /// <param name="globalIndex">The global index that has changed.</param>
        void GlobalSeriesIndexChanged(int? globalIndex);
    }

    /// <summary>Provides information to a category axis.</summary>
    public interface IDataProvider
    {
        /// <summary>Retrieves the data to be plotted on the axis.</summary>
        /// <param name="axis">The axis to retrieve the data for.</param>
        /// <returns>The data to plot on the axis.</returns>
        IEnumerable<object> GetData(IDataConsumer axis);
    }

    /// <summary>An object that consumes data.</summary>
    public interface IDataConsumer
    {
        /// <summary>Supplies the consumer with data.</summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="data">The data used by the consumer.</param>
        void DataChanged(IDataProvider dataProvider, IEnumerable<object> data);
    }


    /// <summary>An axis that is arranged by category.</summary>
    public interface ICategoryAxis : IAxis, IDataConsumer
    {
        /// <summary>
        /// Accepts a category and returns the coordinate range of that category
        /// on the axis.
        /// </summary>
        /// <param name="category">A category for which to retrieve the
        /// coordinate location.</param>
        /// <returns>The coordinate range of the category on the axis.</returns>
        Range<UnitValue> GetPlotAreaCoordinateRange(object category);

        /// <summary>Returns the category at a given coordinate.</summary>
        /// <param name="position">The plot are coordinate.</param>
        /// <returns>The category at the given plot area coordinate.</returns>
        object GetCategoryAtPosition(UnitValue position);
    }
}
