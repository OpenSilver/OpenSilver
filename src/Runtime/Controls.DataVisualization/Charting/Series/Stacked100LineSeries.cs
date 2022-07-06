﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Control that displays values as a 100% stacked line chart visualization.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class Stacked100LineSeries : StackedLineSeries
    {
        /// <summary>
        /// Initializes a new instance of the Stacked100LineSeries class.
        /// </summary>
        public Stacked100LineSeries()
        {
            IsStacked100 = true;
        }
    }
}
