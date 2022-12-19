// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// A collection of helper methods for working with double data types.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal static class DoubleUtil
        {
            /// <summary>
            /// Epsilon is the smallest value such that 1.0+epsilon != 1.0.  It
            /// can be used to determine the acceptable tolerance for rounding
            /// errors.
            /// </summary>
            /// <remarks>
            /// Epsilon is normally 2.2204460492503131E-16, but Silverlight 2
            /// uses floats so the effective epsilon is really 1.192093E-07.
            /// </remarks>
            private const double Epsilon = 1.192093E-07;

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            private const double ScalarAdjustment = 10.0;

            /// <summary>
            /// Determine if the two doubles are effectively equal within
            /// tolerances.
            /// </summary>
            /// <param name="value1">Inherited code: Requires comment.</param>
            /// <param name="value2">Inherited code: Requires comment 1.</param>
            /// <returns>Inherited code: Requires comment 2.</returns>
            public static bool AreClose(double value1, double value2)
            {
                if (value1 == value2)
                {
                    return true;
                }
                double num = ((Math.Abs(value1) + Math.Abs(value2)) + DoubleUtil.ScalarAdjustment) * DoubleUtil.Epsilon;
                double num2 = value1 - value2;
                return ((-num < num2) && (num > num2));
            }
        }
    }
}