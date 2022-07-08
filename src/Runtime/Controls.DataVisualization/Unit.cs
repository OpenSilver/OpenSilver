// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


#if MIGRATION
namespace System.Windows.Controls.DataVisualization
#else
namespace Windows.UI.Xaml.Controls.DataVisualization
#endif
{
    /// <summary>
    /// Units of measure.
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// The corresponding value is in pixels.
        /// </summary>
        Pixels,

        /// <summary>
        /// The corresponding value is in degrees.
        /// </summary>
        Degrees,
    }
}