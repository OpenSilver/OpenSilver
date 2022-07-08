// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// An object that consumes data.
    /// </summary>
    public interface IDataConsumer
    {
        /// <summary>
        /// Supplies the consumer with data.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="data">The data used by the consumer.</param>
        void DataChanged(IDataProvider dataProvider, IEnumerable<object> data);
    }
}
