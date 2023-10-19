// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Interface describing the common value, maximum and minimum properties
    /// that a TimeInput control is expected to have.
    /// </summary>
    /// <remarks>Used for internal coercion of these properties.</remarks>
    /// <QualityBand>Preview</QualityBand>
    internal interface ITimeInput
    {
        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>The current time.</value>
        DateTime? Value { get; set; }

        /// <summary>
        /// Gets or sets the minimum time.
        /// </summary>
        /// <value>The minimum time.</value>
        DateTime? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum time.
        /// </summary>
        /// <value>The maximum time.</value>
        DateTime? Maximum { get; set; }
    }
}
