// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// A label used to display numeric axis values.
    /// </summary>
    public class NumericAxisLabel : AxisLabel
    {
        static NumericAxisLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericAxisLabel), new PropertyMetadata(typeof(NumericAxisLabel)));
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="NumericAxisLabel"/> class.
        /// </summary>
        public NumericAxisLabel() { }
    }
}
