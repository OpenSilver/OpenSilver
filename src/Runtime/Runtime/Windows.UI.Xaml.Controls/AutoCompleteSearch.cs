

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal static class AutoCompleteSearch
    {
        public static AutoCompleteFilterPredicate<string> GetFilter(
          AutoCompleteFilterMode filterMode)
        {
            switch (filterMode)
            {
                case AutoCompleteFilterMode.StartsWith:
                    return StartsWith;
                case AutoCompleteFilterMode.StartsWithCaseSensitive:
                    return StartsWithCaseSensitive;
                case AutoCompleteFilterMode.StartsWithOrdinal:
                    return StartsWithOrdinal;
                case AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive:
                    return StartsWithOrdinalCaseSensitive;
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
                default:
                    return null;
            }
        }

        private static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static bool StartsWith(string text, string value) => value.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);

        public static bool StartsWithCaseSensitive(string text, string value) => value.StartsWith(text, StringComparison.CurrentCulture);

        public static bool StartsWithOrdinal(string text, string value) => value.StartsWith(text, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithOrdinalCaseSensitive(string text, string value) => value.StartsWith(text, StringComparison.Ordinal);

        public static bool Contains(string text, string value) => value.Contains(text, StringComparison.CurrentCultureIgnoreCase);

        public static bool ContainsCaseSensitive(string text, string value) => value.Contains(text, StringComparison.CurrentCulture);

        public static bool ContainsOrdinal(string text, string value) => value.Contains(text, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsOrdinalCaseSensitive(string text, string value) => value.Contains(text, StringComparison.Ordinal);

        public static bool Equals(string text, string value) => value.Equals(text, StringComparison.CurrentCultureIgnoreCase);

        public static bool EqualsCaseSensitive(string text, string value) => value.Equals(text, StringComparison.CurrentCulture);

        public static bool EqualsOrdinal(string text, string value) => value.Equals(text, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsOrdinalCaseSensitive(string text, string value) => value.Equals(text, StringComparison.Ordinal);
    }
}
