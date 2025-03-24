// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents an item used by a Series in the Legend of a Chart.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class LegendItem : ContentControl
    {
        /// <summary>
        /// Gets or sets the owner of the LegendItem.
        /// </summary>
        public object Owner { get; set; }

        static LegendItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendItem), new PropertyMetadata(typeof(LegendItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegendItem"/> class.
        /// </summary>
        public LegendItem() { }
    }
}