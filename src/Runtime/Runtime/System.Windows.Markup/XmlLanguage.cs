
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

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using OpenSilver.Internal;

namespace System.Windows.Markup;

/// <summary>
/// Represents culture information for use in XML and XAML markup.
/// </summary>
[TypeConverter(typeof(XmlLanguageConverter))]
public sealed class XmlLanguage
{
    private static readonly ConcurrentDictionary<string, XmlLanguage> _cache = new();
    private static XmlLanguage _empty;

    private CultureInfo _equivalentCulture;
    private CultureInfo _specificCulture;
    private CultureInfo _compatibleCulture;
    private bool _equivalentCultureFailed;  // only consult after checking _equivalentCulture == null

    private XmlLanguage(string ietfLanguageTag)
    {
        IetfLanguageTag = ietfLanguageTag;
    }

    /// <summary>
    /// Gets a static <see cref="XmlLanguage"/> instance as would be created by <see cref="GetLanguage(string)"/>
    /// with the language tag as an empty attribute string.
    /// </summary>
    public static XmlLanguage Empty => _empty ??= GetLanguage(string.Empty);

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
        ArgumentNullException.ThrowIfNull(ietfLanguageTag);

        string lowercase = AsciiToLower(ietfLanguageTag); // throws on non-ascii
        return _cache.GetOrAdd(lowercase, CreateLanguage);
    }

    /// <summary>
    /// Gets the string representation of the language tag.
    /// </summary>
    public string IetfLanguageTag { get; }

    /// <summary>
    /// Returns the appropriate equivalent <see cref="CultureInfo"/> for this <see cref="XmlLanguage"/>, if and only if such 
    /// a <see cref="CultureInfo"/> is registered for the <see cref="IetfLanguageTag"/> value of this <see cref="XmlLanguage"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="CultureInfo"/> that can be used for localization-globalization API calls that take that type as an argument.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// No registered <see cref="CultureInfo"/> for the provided <see cref="XmlLanguage"/> exists, as determined by a call to 
    /// <see cref="CultureInfo.GetCultureInfo(string)"/>.
    /// </exception>
    public CultureInfo GetEquivalentCulture()
    {
        if (_equivalentCulture is null)
        {
            string lowerCaseTag = IetfLanguageTag;

            // xml:lang="und"
            // see http://www.w3.org/International/questions/qa-no-language
            //
            // Just treat it the same as xml:lang=""
            if (string.CompareOrdinal(lowerCaseTag, "und") == 0)
            {
                lowerCaseTag = string.Empty;
            }

            try
            {
                // Even if we previously failed to find an EquivalentCulture, we retry, if only to
                // capture inner exception.
                _equivalentCulture = CultureInfo.GetCultureInfo(lowerCaseTag);
            }
            catch (ArgumentException e)
            {
                _equivalentCultureFailed = true;
                throw new InvalidOperationException(string.Format(Strings.XmlLangGetCultureFailure, lowerCaseTag), e);
            }
        }

        return _equivalentCulture;
    }

    /// <summary>
    /// Returns the most-closely-related non-neutral <see cref="CultureInfo"/> for this <see cref="XmlLanguage"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="CultureInfo"/> that can be used for localization-globalization API calls that take that type as an argument.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// No related non-neutral <see cref="CultureInfo"/> is registered for the current <see cref="IetfLanguageTag"/>.
    /// </exception>
    public CultureInfo GetSpecificCulture()
    {
        if (_specificCulture is null)
        {
            if (IetfLanguageTag.Length == 0 || string.CompareOrdinal(IetfLanguageTag, "und") == 0)
            {
                _specificCulture = GetEquivalentCulture();
            }
            else
            {
                CultureInfo culture = GetCompatibleCulture();

                if (culture.IetfLanguageTag.Length == 0)
                {
                    throw new InvalidOperationException(string.Format(Strings.XmlLangGetSpecificCulture, IetfLanguageTag));
                }

                if (!culture.IsNeutralCulture)
                {
                    _specificCulture = culture;
                }
                else
                {
                    try
                    {
                        // note that it's important that we use culture.Name, not culture.IetfLanguageTag, here
                        culture = CultureInfo.CreateSpecificCulture(culture.Name);
                        _specificCulture = CultureInfo.GetCultureInfo(culture.IetfLanguageTag);
                    }
                    catch (ArgumentException e)
                    {
                        throw new InvalidOperationException(string.Format(Strings.XmlLangGetSpecificCulture, IetfLanguageTag), e);
                    }
                }
            }
        }

        return _specificCulture;
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
    public override bool Equals(object obj) => this == obj as XmlLanguage;

    /// <summary>
    /// Serves as a hash function for the <see cref="XmlLanguage"/> class.
    /// </summary>
    /// <returns>
    /// An integer hash value.
    /// </returns>
    public override int GetHashCode() => IetfLanguageTag.GetHashCode();

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

        return xmlLanguage1 is not null && xmlLanguage2 is not null && xmlLanguage1.IetfLanguageTag == xmlLanguage2.IetfLanguageTag;
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
    public static bool operator !=(XmlLanguage xmlLanguage1, XmlLanguage xmlLanguage2) => !(xmlLanguage1 == xmlLanguage2);

    public CultureInfo GetCompatibleCulture()
    {
        if (_compatibleCulture is null)
        {
            if (!TryGetEquivalentCulture(out CultureInfo culture))
            {
                string languageTag = IetfLanguageTag;

                do
                {
                    languageTag = Shorten(languageTag);
                    if (languageTag is null)
                    {
                        // Should never happen, because GetCultureInfo("") should return InvariantCulture!
                        culture = CultureInfo.InvariantCulture;
                    }
                    else
                    {
                        try
                        {
                            culture = CultureInfo.GetCultureInfo(languageTag);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }

                }
                while (culture is null);
            }
            _compatibleCulture = culture;
        }
        return _compatibleCulture;
    }

    private bool TryGetEquivalentCulture(out CultureInfo culture)
    {
        if (_equivalentCulture is null && !_equivalentCultureFailed)
        {
            try
            {
                GetEquivalentCulture();
            }
            catch (InvalidOperationException)
            {
            }
        }

        culture = _equivalentCulture;

        return culture is not null;
    }

    /// <summary>
    ///     Shorten a well-formed RFC 3066 string by one subtag.
    /// </summary>
    /// <remarks>
    ///     Shortens "" into null.
    /// </remarks>
    private static string Shorten(string languageTag)
    {
        if (languageTag.Length == 0)
        {
            return null;
        }

        int i = languageTag.Length - 1;

        while (languageTag[i] != '-' && i > 0)
        {
            i -= 1;
        }

        // i now contains of index of first character to be omitted from smaller tag
        return languageTag.Substring(0, i);
    }

    private static XmlLanguage CreateLanguage(string ietfLanguageTag)
    {
        ValidateLowerCaseTag(ietfLanguageTag); // throws on RFC 3066 validation failure
        return new XmlLanguage(ietfLanguageTag);
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
        ArgumentNullException.ThrowIfNull(ietfLanguageTag);

        if (ietfLanguageTag.Length > 0)
        {
            using (var reader = new StringReader(ietfLanguageTag))
            {
                int i = ParseSubtag(ietfLanguageTag, reader, /* isPrimary */ true);
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
        const int maxCharsPerSubtag = 8;

        int c = reader.Read();

        bool ok = IsLowerAlpha(c);
        if (!ok && !isPrimary)
        {
            ok = IsDigit(c);
        }

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

    private static bool IsLowerAlpha(int c) => c >= 'a' && c <= 'z';

    private static bool IsDigit(int c) => c >= '0' && c <= '9';

    private static void ThrowParseException(string ietfLanguageTag) =>
        throw new ArgumentException(string.Format(Strings.XmlLangMalformed, ietfLanguageTag), nameof(ietfLanguageTag));

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
