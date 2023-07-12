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
    /// Specifies values for the different modes of operation of a <see cref="Calendar" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public enum CalendarMode
    {
        /// <summary>
        /// The <see cref="Calendar" /> displays a month at a time.
        /// </summary>
        Month = 0,

        /// <summary>
        /// The <see cref="Calendar" /> displays a year at a time.
        /// </summary>
        Year = 1,

        /// <summary>
        /// The <see cref="Calendar" /> displays a decade at a time.
        /// </summary>
        Decade = 2,
    }
}