
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

using System.Collections.Generic;
using System.IO;

namespace System.Windows.Markup
{
    /// <summary>
    /// Represents culture information for use in XML and XAML markup.
    /// </summary>
    public sealed class XmlLanguage
    {
        private static readonly HashSet<string> _cache = new HashSet<string>();

        private XmlLanguage(string ietfLanguageTag)
        {
            IetfLanguageTag = ietfLanguageTag;
        }

        /// <summary>
        /// Gets the string representation of the language tag.
        /// </summary>
        public string IetfLanguageTag { get; }

        /// <summary>
        /// Returns a <see cref="XmlLanguage"/> instance, based on a string representing
        /// the language per RFC 3066.
        /// </summary>
        /// <param name="ietfLanguageTag">
        /// An RFC 3066 language string, or an empty string ("").
        /// </param>
        /// <returns>
        /// A new <see cref="XmlLanguage"/> with the provided string as its <see cref="IetfLanguageTag"/>
        /// value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// ietfLanguageTag is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// ietfLanguageTag cannot be processed as a valid IETF language.
        /// </exception>
        public static XmlLanguage GetLanguage(string ietfLanguageTag)
        {
            if (ietfLanguageTag == null)
            {
                throw new ArgumentNullException(nameof(ietfLanguageTag));
            }

            string lowercase = AsciiToLower(ietfLanguageTag); // throws on non-ascii
            if (!_cache.Contains(lowercase))
            {
                ValidateLowerCaseTag(lowercase); // throws on RFC 3066 validation failure
                _cache.Add(lowercase);
            }

            return new XmlLanguage(lowercase);
        }

        /// <summary>
        /// Checks for equality of an object to the current object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to the calling object.
        /// </param>
        /// <returns>
        /// true if the objects are the same object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this == obj as XmlLanguage;
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="XmlLanguage"/> class.
        /// </summary>
        /// <returns>
        /// An integer hash value.
        /// </returns>
        public override int GetHashCode()
        {
            return IetfLanguageTag.GetHashCode();
        }

        /// <summary>
        /// Determines whether two specified <see cref="XmlLanguage"/> objects have
        /// the same value.
        /// </summary>
        /// <param name="xmlLanguage1">
        /// The first <see cref="XmlLanguage"/> to compare.
        /// </param>
        /// <param name="xmlLanguage2">
        /// The second <see cref="XmlLanguage"/> to compare.
        /// </param>
        /// <returns>
        /// true if the value of xmlLanguage1 is the same as the value of xmlLanguage2; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(XmlLanguage xmlLanguage1, XmlLanguage xmlLanguage2)
        {
            if (ReferenceEquals(xmlLanguage1, xmlLanguage2))
            {
                return true;
            }

            return !(xmlLanguage1 is null) && 
                !(xmlLanguage2 is null) && 
                xmlLanguage1.IetfLanguageTag == xmlLanguage2.IetfLanguageTag;
        }

        /// <summary>
        /// Determines whether two specified <see cref="XmlLanguage"/> objects have
        /// a different value.
        /// </summary>
        /// <param name="xmlLanguage1">
        /// The first <see cref="XmlLanguage"/> to compare.
        /// </param>
        /// <param name="xmlLanguage2">
        /// The second <see cref="XmlLanguage"/> to compare.
        /// </param>
        /// <returns>
        /// true if the value of xmlLanguage1 is different from the value of xmlLanguage2;
        /// otherwise, false.
        /// </returns>
        public static bool operator !=(XmlLanguage xmlLanguage1, XmlLanguage xmlLanguage2)
        {
            return !(xmlLanguage1 == xmlLanguage2);
        }

        /// <summary>
        ///     Throws an ArgumentException (or ArgumentNullException) is not the empty
        ///       string, and does not conform to RFC 3066.
        /// </summary>
        /// <remarks>
        ///     It is assumed that caller has already converted to lower-case.
        ///     The language string may be empty, or else must conform to RFC 3066 rules:
        ///     The first subtag must consist of only ASCII letters.
        ///     Additional subtags must consist ASCII letters or numerals.
        ///     Subtags are separated by a single hyphen character.
        ///     Every subtag must be 1 to 8 characters long.
        ///     No leading or trailing hyphens are permitted.
        /// </remarks>
        /// <param name="ietfLanguageTag"></param>
        /// <exception cref="ArgumentNullException">tag is NULL.</exception>
        /// <exception cref="ArgumentException">tag is non-empty, but does not conform to RFC 3066.</exception>
        private static void ValidateLowerCaseTag(string ietfLanguageTag)
        {
            if (ietfLanguageTag == null)
            {
                throw new ArgumentNullException("ietfLanguageTag");
            }

            if (ietfLanguageTag.Length > 0)
            {
                using (StringReader reader = new StringReader(ietfLanguageTag))
                {
                    int i;

                    i = ParseSubtag(ietfLanguageTag, reader, /* isPrimary */ true);
                    while (i != -1)
                    {
                        i = ParseSubtag(ietfLanguageTag, reader, /* isPrimary */ false);
                    }
                }
            }
        }

        // returns the character which terminated the subtag -- either '-' or -1 for
        //  end of string.
        // throws exception on improper formatting
        // It is assumed that caller has already converted to lower-case.
        private static int ParseSubtag(string ietfLanguageTag, StringReader reader, bool isPrimary)
        {
            int c;
            bool ok;
            const int maxCharsPerSubtag = 8;

            c = reader.Read();

            ok = IsLowerAlpha(c);
            if (!ok && !isPrimary)
                ok = IsDigit(c);

            if (!ok)
            {
                ThrowParseException(ietfLanguageTag);
            }

            int charsRead = 1;
            for (; ; )
            {
                c = reader.Read();
                charsRead++;

                ok = IsLowerAlpha(c);
                if (!ok && !isPrimary)
                {
                    ok = IsDigit(c);
                }

                if (!ok)
                {
                    if (c == -1 || c == '-')
                    {
                        return c;
                    }
                    else
                    {
                        ThrowParseException(ietfLanguageTag);
                    }
                }
                else
                {
                    if (charsRead > maxCharsPerSubtag)
                    {
                        ThrowParseException(ietfLanguageTag);
                    }
                }
            }
        }

        private static bool IsLowerAlpha(int c)
        {
            return (c >= 'a' && c <= 'z');
        }

        private static bool IsDigit(int c)
        {
            return c >= '0' && c <= '9';
        }

        private static void ThrowParseException(string ietfLanguageTag)
        {
            throw new ArgumentException($"'{ietfLanguageTag}' language tag must be empty or must conform to grammar defined in IETF RFC 3066.", nameof(ietfLanguageTag));
        }

        // throws if there is a non-7-bit ascii character
        private static string AsciiToLower(string tag)
        {
            int length = tag.Length;

            for (int i = 0; i < length; i++)
            {
                if (tag[i] > 127)
                {
                    ThrowParseException(tag);
                }
            }

            return tag.ToLowerInvariant();
        }
    }
}
