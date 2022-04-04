

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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// TimeParser that will allow very loose time to be entered. It will try
    /// to parse the first two numbers as hours and the second two numbers as
    /// minutes, and will not care about other characters, such as designators,
    /// separators or non-time related characters.
    /// If the first character is bigger than 2, it will parse only the first
    /// character as an hour and will use the second two characters for minutes.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class CatchallTimeParser : TimeParser
    {
        /// <summary>Expression used to parse.</summary>
        private static readonly Regex exp = new Regex("((?<hours>([0-1][\\d]?))|(?<hours>(^2[0-3]?))|(?<hours>(^[3-9])))[^\\d]?(?<minutes>\\d{0,2})[^\\d]?(?<seconds>\\d{0,2})", RegexOptions.CultureInvariant);

        /// <summary>Tries to parse a string to a DateTime.</summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>
        /// True if the parse was successful, false if it was not.
        /// </returns>
        /// <remarks>The parsing is culture insensitive. A user can type 8p to
        /// indicate 20:00:00, or 20.</remarks>
        public override bool TryParse(string text, CultureInfo culture, out DateTime? result)
        {
            Match match = CatchallTimeParser.exp.Match(text);
            if (match.Success)
            {
                bool flag = text.Contains("p"/*, StringComparison.OrdinalIgnoreCase*/);
                result = new DateTime?();
                int num1 = int.Parse(match.Groups["hours"].Value, (IFormatProvider)culture);
                if (num1 > 23)
                    return false;
                int num2 = !match.Groups["minutes"].Success || match.Groups["minutes"].Value.Length <= 0 ? 0 : int.Parse(match.Groups["minutes"].Value, (IFormatProvider)culture);
                if (num2 > 59)
                    return false;
                int num3 = !match.Groups["seconds"].Success || match.Groups["seconds"].Value.Length <= 0 ? 0 : int.Parse(match.Groups["seconds"].Value, (IFormatProvider)culture);
                if (num3 > 59)
                    return false;
                ref DateTime? local1 = ref result;
                DateTime dateTime = DateTime.Now.Date.AddHours((double)num1);
                dateTime = dateTime.AddMinutes((double)num2);
                DateTime? nullable1 = new DateTime?(dateTime.AddSeconds((double)num3));
                local1 = nullable1;
                ref DateTime? local2 = ref result;
                dateTime = result.Value;
                DateTime? nullable2 = new DateTime?(dateTime.AddHours(!flag || num1 >= 12 ? 0.0 : 12.0));
                local2 = nullable2;
                return true;
            }
            result = new DateTime?();
            return false;
        }
    }

    /// <summary>
    /// Strategy object that determines how controls interact with DateTime and
    /// CultureInfo.
    /// </summary>
    /// <remarks>TimeInput supports only the following formatting characters:
    /// 'h', 'm', 's', 'H', 't'. All other characters are filtered out:
    /// 'd', 'f', 'F', 'g', 'K', 'M', 'y', 'z'.</remarks>
    /// <QualityBand>Preview</QualityBand>
    public class TimeGlobalizationInfo
    {
        /// <summary>The characters that are allowed inside a format.</summary>
        private readonly char[] TimeChars = new char[5]
        {
      'h',
      'm',
      's',
      'H',
      't'
        };

        /// <summary>
        /// Gets or sets the culture used by the owning TimeInput control.
        /// </summary>
        internal CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the actual culture used by the TimeGlobalizationInfo for formatting
        /// and parsing.
        /// </summary>
        /// <value>The actual culture.</value>
        public CultureInfo ActualCulture
        {
            get
            {
                if (this.Culture != null)
                    return this.Culture;
                if (Thread.CurrentThread.CurrentCulture != null && !Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
                    return Thread.CurrentThread.CurrentCulture;
                return Thread.CurrentThread.CurrentUICulture != null && !Thread.CurrentThread.CurrentUICulture.IsNeutralCulture ? Thread.CurrentThread.CurrentUICulture : new CultureInfo("en-US");
            }
        }

        /// <summary>
        /// Gets the characters that may be used to separate components of time,
        /// that is, hours, minutes and seconds.
        /// </summary>
        public virtual IList<char> TimeSeparators
        {
            get
            {
                return (IList<char>)((IEnumerable<char>)new char[2]
                {
          '.',
          ':'
                }).ToList<char>();
            }
        }

        /// <summary>
        /// Gets the string designator for hours that are "ante meridiem"
        /// (before noon).
        /// </summary>
        /// <value>The AM designator.</value>
        public virtual string AMDesignator
        {
            get
            {
                return this.ActualCulture.DateTimeFormat.AMDesignator;
            }
        }

        /// <summary>
        /// Gets the string designator for hours that are "post meridiem"
        /// (after noon).
        /// </summary>
        /// <value>The PM designator.</value>
        public virtual string PMDesignator
        {
            get
            {
                return this.ActualCulture.DateTimeFormat.PMDesignator;
            }
        }

        /// <summary>
        /// Transforms a format to a format that only allows the characters
        /// h, m, s, t, H and the defined TimeSeparators (: and .).
        /// Also takes into account the rule that a single TimeCharacter should
        /// be followed by a space.
        /// </summary>
        /// <param name="format">The format that needs to be transformed.</param>
        /// <returns>A format containing only the expected characters.</returns>
        protected virtual string GetTransformedFormat(string format)
        {
            string str = new string(((IEnumerable<char>)format.ToCharArray()).Where<char>((Func<char, bool>)(c => char.IsWhiteSpace(c) || this.TimeSeparators.Contains(c) || ((IEnumerable<char>)this.TimeChars).Contains<char>(c))).Select<char, char>((Func<char, char>)(c => c)).ToArray<char>()).Trim();
            if (str.Length == 1)
                str = new string(new char[2] { str[0], ' ' });
            return str;
        }

        /// <summary>
        /// Returns the global representation of each integer formatted
        /// by the TimeGlobalizationInfo.
        /// </summary>
        /// <param name="input">Character that will be mapped to a different
        /// character.</param>
        /// <returns>The global version of a character that represents the input.</returns>
        protected virtual char MapDigitToCharacter(int input)
        {
            return input.ToString((IFormatProvider)CultureInfo.InvariantCulture)[0];
        }

        /// <summary>
        /// Returns the European number character of each global representation
        /// parsed by the TimeGlobalizationInfo.
        /// </summary>
        /// <param name="input">The global version of the character that needs
        /// to be mapped to a regular character.</param>
        /// <returns>The character that represents the global version of a character.</returns>
        /// <remarks>All characters pass through this method (whitespaces and
        /// TimeDesignators). Return the input character if no logical mapping
        /// could be made.</remarks>
        protected virtual char MapCharacterToDigit(char input)
        {
            return input;
        }

        /// <summary>
        /// Gets the actual TimeParsers that will be used for parsing.
        /// </summary>
        /// <param name="parsers">The parsers that are currently used by parent.</param>
        /// <returns>A new collection of parsers that represent the parsers
        /// this strategy object will use.</returns>
        public virtual IEnumerable<TimeParser> GetActualTimeParsers(
          IEnumerable<TimeParser> parsers)
        {
            List<TimeParser> timeParserList = parsers == null ? new List<TimeParser>() : new List<TimeParser>(parsers);
            timeParserList.Add((TimeParser)new CatchallTimeParser());
            return (IEnumerable<TimeParser>)timeParserList;
        }

        /// <summary>
        /// Formats the specified DateTime to a time string representation.
        /// </summary>
        /// <param name="value">The DateTime that should be formatted.</param>
        /// <param name="timeFormat">The time format used to describe how value
        /// should be formatted.</param>
        /// <param name="timeCharacters">The allowed characters in the format.
        /// Leave empty to indicate that all characters are allowed. See remarks.</param>
        /// <returns>A string that represents the time part of a DateTime.</returns>
        /// <remarks>The TimeFormat will contain TimeCharacters in a certain
        /// order, like hh:mm:ss. By passing specific TimeCharacters, these
        /// will get filtered and the method only returns part of the formatted
        /// string. Example: pass 'h', 't', 'H' to get back 4 AM, if the culture
        /// was set to en-US.</remarks>
        public virtual string FormatTime(
          DateTime? value,
          ITimeFormat timeFormat,
          params char[] timeCharacters)
        {
            if (!value.HasValue)
                return string.Empty;
            if (timeFormat == null)
                throw new ArgumentNullException(nameof(timeFormat));
            if (((IEnumerable<char>)timeCharacters).Count<char>() > 0)
            {
                string format = new string(((IEnumerable<char>)timeFormat.GetTimeDisplayFormat(this.ActualCulture).ToCharArray()).Where<char>((Func<char, bool>)(c => ((IEnumerable<char>)timeCharacters).Contains<char>(c))).Select<char, char>((Func<char, char>)(c => c)).ToArray<char>());
                if (!string.IsNullOrEmpty(format))
                    timeFormat = (ITimeFormat)new CustomTimeFormat(format);
            }
            return new string(((IEnumerable<char>)value.Value.ToString(this.GetTransformedFormat(timeFormat.GetTimeDisplayFormat(this.ActualCulture)), (IFormatProvider)this.ActualCulture).ToCharArray()).Select<char, char>((Func<char, char>)(c => char.IsDigit(c) ? this.MapDigitToCharacter(int.Parse(c.ToString((IFormatProvider)CultureInfo.InvariantCulture), NumberStyles.Number, (IFormatProvider)CultureInfo.InvariantCulture)) : c)).ToArray<char>()).Trim();
        }

        /// <summary>
        /// Parses a string into a DateTime using the specified ITimeFormat instance
        /// and TimeParsers.
        /// </summary>
        /// <param name="mappedText">The text that was entered by the user.</param>
        /// <param name="timeFormat">The TimeFormat instance used to supply
        /// formats.</param>
        /// <param name="timeParsers">The time parsers.</param>
        /// <returns>A DateTime with a correctly set time part.</returns>
        /// <remarks>The date part of the DateTime is irrelevant and will be
        /// overwritten by the current date.
        /// </remarks>
        public DateTime? ParseTime(
          string mappedText,
          ITimeFormat timeFormat,
          IEnumerable<TimeParser> timeParsers)
        {
            if (timeFormat == null)
                throw new ArgumentNullException(nameof(timeFormat));
            DateTime? result;
            if (this.TryParseTime(mappedText, timeFormat, timeParsers, out result))
                return result;
            throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "ParseTime Failed: {0} {1}", (object)mappedText), nameof(mappedText));
        }

        /// <summary>
        /// Parses a string into a DateTime using the specified ITimeFormat instance
        /// and TimeParsers and returns a value that indicates whether the conversion
        /// succeeded.
        /// </summary>
        /// <param name="mappedText">The text that was entered by the user.</param>
        /// <param name="timeFormat">The TimeFormat instance used to supply
        /// formats.</param>
        /// <param name="timeParsers">The time parsers.</param>
        /// <param name="result">A DateTime with a correctly set time part.</param>
        /// <returns>
        /// True, if the time was parsed correctly, false if the time was not
        /// parsed.
        /// </returns>
        /// <remarks>The date part of the DateTime is irrelevant and will be
        /// overwritten by the current date.
        /// </remarks>
        public bool TryParseTime(
          string mappedText,
          ITimeFormat timeFormat,
          IEnumerable<TimeParser> timeParsers,
          out DateTime? result)
        {
            result = new DateTime?();
            if (string.IsNullOrEmpty(mappedText))
                return true;
            string s1 = new string(((IEnumerable<char>)mappedText.ToCharArray()).Select<char, char>((Func<char, char>)(c => this.TimeSeparators.Contains(c) ? c : this.MapCharacterToDigit(c))).ToArray<char>());
            DateTime result1;
            if (timeFormat != null && DateTime.TryParseExact(s1, ((IEnumerable<string>)timeFormat.GetTimeParseFormats(this.ActualCulture)).Select<string, string>((Func<string, string>)(s => this.GetTransformedFormat(s))).ToArray<string>(), (IFormatProvider)this.ActualCulture, DateTimeStyles.None, out result1))
            {
                result = new DateTime?(result1);
                return true;
            }
            DateTime? result2;
            if (!new TimeParserCollection(this.GetActualTimeParsers(timeParsers)).TryParse(mappedText, this.ActualCulture, out result2))
                return false;
            result = result2;
            return true;
        }

        /// <summary>
        /// Gets the time unit that is represented by a text position.
        /// </summary>
        /// <param name="text">The text that represents a DateTime.</param>
        /// <param name="textPosition">The location in the text.</param>
        /// <param name="timeFormat">The time format describe how the text
        /// can be parsed to a DateTime.</param>
        /// <returns>
        /// The TimeSpan that is represented by the character at a
        /// specific caret position.
        /// </returns>
        public virtual TimeSpan GetTimeUnitAtTextPosition(
          string text,
          int textPosition,
          ITimeFormat timeFormat)
        {
            if (string.IsNullOrEmpty(text) || textPosition < 0 || textPosition > text.Length)
                return new TimeSpan();
            if (timeFormat == null)
                throw new ArgumentNullException(nameof(timeFormat));
            if (textPosition == text.Length || this.TimeSeparators.Contains(text[textPosition]))
                return this.GetTimeUnitAtTextPosition(text, textPosition - 1, timeFormat);
            if (char.IsWhiteSpace(text[textPosition]))
            {
                for (int index = 1; textPosition + index < text.Length || textPosition - index >= 0; ++index)
                {
                    if (textPosition - index >= 0 && !char.IsWhiteSpace(text[textPosition - index]))
                        return this.GetTimeUnitAtTextPosition(text, textPosition - index, timeFormat);
                    if (textPosition + index < text.Length && !char.IsWhiteSpace(text[textPosition + index]))
                        return this.GetTimeUnitAtTextPosition(text, textPosition + index, timeFormat);
                }
            }
            int textPositionStart = this.GetDesignatorTextPositionStart(text);
            int designatorTextPositionEnd = this.GetDesignatorTextPositionEnd(text, textPositionStart);
            if (textPosition >= textPositionStart && textPosition < designatorTextPositionEnd)
                return TimeSpan.FromHours(12.0);
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            for (int index = 0; index < text.Length; ++index)
            {
                char c = text[index];
                stringBuilder.Append('0');
                stringBuilder[index] = index == textPosition ? '1' : '0';
                if (this.TimeSeparators.Contains(c))
                    stringBuilder[index] = c;
                if (index >= textPositionStart && index < designatorTextPositionEnd)
                    stringBuilder[index] = c;
                if (char.IsWhiteSpace(c))
                    stringBuilder[index] = c;
            }
            string mappedText = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(this.PMDesignator))
                mappedText = mappedText.Replace(this.PMDesignator, this.AMDesignator);
            DateTime? result;
            if (!this.TryParseTime(mappedText, timeFormat, (IEnumerable<TimeParser>)null, out result) || !result.HasValue)
                return new TimeSpan();
            return result.Value.TimeOfDay.Hours == 10 ? TimeSpan.FromHours(1.0) : result.Value.TimeOfDay;
        }

        /// <summary>
        /// Gets the position for a time unit in a string that can be parsed by
        /// the specified ITimeFormat.
        /// </summary>
        /// <param name="text">The text that represents a DateTime.</param>
        /// <param name="timeSpan">The time span that is searched for.</param>
        /// <param name="timeFormat">The time format that describes how this text can be
        /// parsed to a DateTime.</param>
        /// <returns>
        /// The position in the text that corresponds to the TimeSpan or
        /// -1 if none was found.
        /// </returns>
        public virtual int GetTextPositionForTimeUnit(
          string text,
          TimeSpan timeSpan,
          ITimeFormat timeFormat)
        {
            if (string.IsNullOrEmpty(text))
                return -1;
            int textPositionStart = this.GetDesignatorTextPositionStart(text);
            int designatorTextPositionEnd = this.GetDesignatorTextPositionEnd(text, textPositionStart);
            if (timeSpan == TimeSpan.FromHours(12.0))
                return textPositionStart;
            for (int textPosition = 0; textPosition < text.Length; ++textPosition)
            {
                if (textPosition <= textPositionStart || textPosition >= designatorTextPositionEnd)
                {
                    char c = text[textPosition];
                    if (!char.IsWhiteSpace(c) && !this.TimeSeparators.Contains(c) && timeSpan == this.GetTimeUnitAtTextPosition(text, textPosition, timeFormat))
                        return textPosition;
                }
            }
            return -1;
        }

        /// <summary>
        /// Performs addition of a date time and a time span in a global context.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The DateTime after incrementing by TimeSpan.</returns>
        public virtual DateTime OnIncrement(DateTime value, TimeSpan timeSpan)
        {
            return DateTime.MaxValue.Date == value.Date && value.TimeOfDay.Add(timeSpan) > TimeSpan.FromDays(1.0) ? value.AddDays(-1.0).Add(timeSpan) : value.Add(timeSpan);
        }

        /// <summary>
        /// Subtracts a time span from a date time in a global context.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The DateTime after decrementing by TimeSpan.</returns>
        public virtual DateTime OnDecrement(DateTime value, TimeSpan timeSpan)
        {
            return DateTime.MinValue.Date == value.Date && value.TimeOfDay < timeSpan ? value.AddDays(1.0).Subtract(timeSpan) : value.Subtract(timeSpan);
        }

        /// <summary>
        /// Gets the caret position at the start of the designator.
        /// </summary>
        /// <param name="text">The text that might include a designator.</param>
        /// <returns>Caret position for the end of the designator,
        /// or -1 if none found.</returns>
        private int GetDesignatorTextPositionStart(string text)
        {
            int num = text.IndexOf(this.AMDesignator, StringComparison.OrdinalIgnoreCase);
            if (num == -1)
                num = text.IndexOf(this.PMDesignator, StringComparison.OrdinalIgnoreCase);
            return num;
        }

        /// <summary>Gets the caret position at the end of the designator.</summary>
        /// <param name="text">The text that might include a designator.</param>
        /// <param name="designatorStartIndex">Start index of the designator.</param>
        /// <returns>
        /// Caret position for the end of the designator,
        /// or -1 if none found.
        /// </returns>
        private int GetDesignatorTextPositionEnd(string text, int designatorStartIndex)
        {
            if (text.Contains(this.AMDesignator/*, StringComparison.OrdinalIgnoreCase*/))
                return designatorStartIndex + this.AMDesignator.Length;
            return text.Contains(this.PMDesignator/*, StringComparison.OrdinalIgnoreCase*/) ? designatorStartIndex + this.PMDesignator.Length : -1;
        }
    }
}
