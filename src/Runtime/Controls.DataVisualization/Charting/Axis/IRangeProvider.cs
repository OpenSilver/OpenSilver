// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// Provides information to a RangeConsumer.
    /// </summary>
    public interface IRangeProvider
    {
        /// <summary>
        /// Returns the range of values.
        /// </summary>
        /// <param name="rangeConsumer">The range consumer requesting the data 
        /// range.</param>
        /// <returns>A data range.</returns>
        Range<IComparable> GetRange(IRangeConsumer rangeConsumer);
    }
}
