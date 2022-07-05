using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>A class used to calculate axis range.</summary>
    internal class ValueMarginCoordinateAndOverlap
    {
        /// <summary>Gets or sets the value margin object.</summary>
        public ValueMargin ValueMargin { get; set; }

        /// <summary>Gets or sets the coordinate.</summary>
        public double Coordinate { get; set; }

        /// <summary>Gets or sets the left overlap.</summary>
        public double LeftOverlap { get; set; }

        /// <summary>Gets or sets the right overlap.</summary>
        public double RightOverlap { get; set; }
    }
}
