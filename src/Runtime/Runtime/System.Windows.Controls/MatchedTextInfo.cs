// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Holds the information regarding the matched text from text search
//

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class MatchedTextInfo
    {
        internal MatchedTextInfo(int matchedItemIndex, string matchedText, int matchedPrefixLength, int textExcludingPrefixLength)
        {
            MatchedItemIndex = matchedItemIndex;
            MatchedText = matchedText;
            MatchedPrefixLength = matchedPrefixLength;
            TextExcludingPrefixLength = textExcludingPrefixLength;
        }

        /// <summary>
        /// No match from text search
        /// </summary>
        internal static MatchedTextInfo NoMatch { get; } = new MatchedTextInfo(-1, null, 0, 0);

        /// <summary>
        /// Matched text from text search
        /// </summary>
        internal string MatchedText { get; }

        /// <summary>
        /// Index of the matched item
        /// </summary>
        internal int MatchedItemIndex { get; }

        /// <summary>
        /// Length of the matched prefix
        /// </summary>
        internal int MatchedPrefixLength { get; }

        /// <summary>
        /// Length of the text excluding prefix
        /// </summary>
        internal int TextExcludingPrefixLength { get; }
    }
}
