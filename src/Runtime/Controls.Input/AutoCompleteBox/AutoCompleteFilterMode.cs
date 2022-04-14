﻿// (c) Copyright Microsoft Corporation.
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
    // When adding to this enum, please update the OnFilterModePropertyChanged
    // in the AutoCompleteBox class that is used for validation.

    /// <summary>
    /// Specifies how text in the text box portion of the
    /// <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control is used
    /// to filter items specified by the
    /// <see cref="P:System.Windows.Controls.AutoCompleteBox.ItemsSource" />
    /// property for display in the drop-down.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public enum AutoCompleteFilterMode
    {
        /// <summary>
        /// Specifies that no filter is used. All items are returned.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the
        /// returned items start with the specified text. The filter uses the
        /// <see cref="M:System.String.StartsWith(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.CurrentCultureIgnoreCase" /> as
        /// the string comparison criteria.
        /// </summary>
        StartsWith = 1,

        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the
        /// returned items start with the specified text. The filter uses the
        /// <see cref="M:System.String.StartsWith(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.CurrentCulture" /> as the string
        /// comparison criteria.
        /// </summary>
        StartsWithCaseSensitive = 2,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned
        /// items start with the specified text. The filter uses the
        /// <see cref="M:System.String.StartsWith(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.OrdinalIgnoreCase" /> as the
        /// string comparison criteria.
        /// </summary>
        StartsWithOrdinal = 3,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items
        /// start with the specified text. The filter uses the
        /// <see cref="M:System.String.StartsWith(System.String,System.StringComparison)" />
        /// method, specifying <see cref="P:System.StringComparer.Ordinal" /> as
        /// the string comparison criteria.
        /// </summary>
        StartsWithOrdinalCaseSensitive = 4,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the
        /// returned items contain the specified text.
        /// </summary>
        Contains = 5,

        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the
        /// returned items contain the specified text.
        /// </summary>
        ContainsCaseSensitive = 6,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned
        /// items contain the specified text.
        /// </summary>
        ContainsOrdinal = 7,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items
        /// contain the specified text.
        /// </summary>
        ContainsOrdinalCaseSensitive = 8,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the
        /// returned items equal the specified text. The filter uses the
        /// <see cref="M:System.String.Equals(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.CurrentCultureIgnoreCase" /> as
        /// the search comparison criteria.
        /// </summary>
        Equals = 9,

        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the
        /// returned items equal the specified text. The filter uses the
        /// <see cref="M:System.String.Equals(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.CurrentCulture" /> as the string
        /// comparison criteria.
        /// </summary>
        EqualsCaseSensitive = 10,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned
        /// items equal the specified text. The filter uses the
        /// <see cref="M:System.String.Equals(System.String,System.StringComparison)" />
        /// method, specifying
        /// <see cref="P:System.StringComparer.OrdinalIgnoreCase" /> as the
        /// string comparison criteria.
        /// </summary>
        EqualsOrdinal = 11,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items
        /// equal the specified text. The filter uses the
        /// <see cref="M:System.String.Equals(System.String,System.StringComparison)" />
        /// method, specifying <see cref="P:System.StringComparer.Ordinal" /> as
        /// the string comparison criteria.
        /// </summary>
        EqualsOrdinalCaseSensitive = 12,

        /// <summary>
        /// Specifies that a custom filter is used. This mode is used when the
        /// <see cref="P:System.Windows.Controls.AutoCompleteBox.TextFilter" />
        /// or
        /// <see cref="P:System.Windows.Controls.AutoCompleteBox.ItemFilter" />
        /// properties are set.
        /// </summary>
        Custom = 13,
    }
}