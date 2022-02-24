// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

#if MIGRATION
namespace System.Windows.Controls.DataVisualization.Charting
#else
namespace Windows.UI.Xaml.Controls.DataVisualization.Charting
#endif
{
    /// <summary>
    /// Specifies the orientation of an axis.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum AxisOrientation
    {
        /// <summary>
        /// Orientation is automatically set.
        /// </summary>
        None,

        /// <summary>
        /// Indicates the axis plots along the X axis.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X", Justification = "X is the expected terminology.")]
        X,

        /// <summary>
        /// Indicates the axis plots along the Y axis.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y", Justification = "Y is the expected terminology.")]
        Y,
    }
}