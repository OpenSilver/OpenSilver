// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Controls;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// A predefined set of filter functions for the known, built-in 
    /// AutoCompleteFilterMode enumeration values.
    /// </summary>
    internal static class AutoCompleteSearch
    {
        /// <summary>
        /// Index function that retrieves the filter for the provided 
        /// AutoCompleteFilterMode.
        /// </summary>
        /// <param name="FilterMode">The built-in search mode.</param>
        /// <returns>Returns the string-based comparison function.</returns>
        public static AutoCompleteFilterPredicate<string> GetFilter(AutoCompleteFilterMode FilterMode)
        {
            switch (FilterMode)
            {
                case AutoCompleteFilterMode.Contains:
                    return Contains;

                case AutoCompleteFilterMode.ContainsCaseSensitive:
                    return ContainsCaseSensitive;

                case AutoCompleteFilterMode.ContainsOrdinal:
                    return ContainsOrdinal;

                case AutoCompleteFilterMode.ContainsOrdinalCaseSensitive:
                    return ContainsOrdinalCaseSensitive;

                case AutoCompleteFilterMode.Equals:
                    return Equals;

                case AutoCompleteFilterMode.EqualsCaseSensitive:
                    return EqualsCaseSensitive;

                case AutoCompleteFilterMode.EqualsOrdinal:
                    return EqualsOrdinal;

                case AutoCompleteFilterMode.EqualsOrdinalCaseSensitive:
                    return EqualsOrdinalCaseSensitive;

                case AutoCompleteFilterMode.StartsWith:
                    return StartsWith;

                case AutoCompleteFilterMode.StartsWithCaseSensitive:
                    return StartsWithCaseSensitive;

                case AutoCompleteFilterMode.StartsWithOrdinal:
                    return StartsWithOrdinal;

                case AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive:
                    return StartsWithOrdinalCaseSensitive;

                case AutoCompleteFilterMode.None:
                case AutoCompleteFilterMode.Custom:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWith(string text, string value)
        {
            return value.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWithCaseSensitive(string text, string value)
        {
            return value.StartsWith(text, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWithOrdinal(string text, string value)
        {
            return value.StartsWith(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWithOrdinalCaseSensitive(string text, string value)
        {
            return value.StartsWith(text, StringComparison.Ordinal);
        }

        /// <summary>
        /// Check if the prefix is contained in the string value. The current 
        /// culture's case insensitive string comparison operator is used.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool Contains(string text, string value)
        {
            return value.Contains(text, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Check if the prefix is contained in the string value.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool ContainsCaseSensitive(string text, string value)
        {
            return value.Contains(text, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Check if the prefix is contained in the string value.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool ContainsOrdinal(string text, string value)
        {
            return value.Contains(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the prefix is contained in the string value.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool ContainsOrdinalCaseSensitive(string text, string value)
        {
            return value.Contains(text, StringComparison.Ordinal);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool Equals(string text, string value)
        {
            return value.Equals(text, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool EqualsCaseSensitive(string text, string value)
        {
            return value.Equals(text, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool EqualsOrdinal(string text, string value)
        {
            return value.Equals(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool EqualsOrdinalCaseSensitive(string text, string value)
        {
            return value.Equals(text, StringComparison.Ordinal);
        }
    }
}