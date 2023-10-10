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
    /// Specifies date formats for a <see cref="DatePicker" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public enum DatePickerFormat
    {
        /// <summary>
        /// Specifies that the date should be displayed using unabbreviated days
        /// of the week and month names.
        /// </summary>
        Long = 0,

        /// <summary>
        /// Specifies that the date should be displayed using abbreviated days
        /// of the week and month names.
        /// </summary>
        Short = 1
    }
}