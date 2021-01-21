// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents spin directions that could be initiated by the end-user.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum SpinDirection
    {
        /// <summary>
        /// Represents a spin initiated by the end-user in order to Increase a value.
        /// </summary>
        Increase = 0,

        /// <summary>
        /// Represents a spin initiated by the end-user in order to Decrease a value.
        /// </summary>
        Decrease = 1
    }
}