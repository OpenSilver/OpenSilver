// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
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
        /// <summary>
        /// The characters that are allowed inside a format.
        /// </summary>
        private readonly char[] TimeChars = new[] { 'h', 'm', 's', 'H', 't' };

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
                if (Culture != null)
                {
                    return Culture;
                }
                if (Thread.CurrentThread.CurrentCulture != null && !Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
                {
                    return Thread.CurrentThread.CurrentCulture;
                }
                if (Thread.CurrentThread.CurrentUICulture != null && !Thread.CurrentThread.CurrentUICulture.IsNeutralCulture)
                {
                    return Thread.CurrentThread.CurrentUICulture;
                }

                return new CultureInfo("en-US");
            }
        }

        /// <summary>
        /// Gets the characters that may be used to separate components of time,
        /// that is, hours, minutes and seconds.
        /// </summary>
        public virtual IList<char> TimeSeparators
        {
            get { return new[] { '.', ':' }.ToList(); }
        }

        /// <summary>
        /// Gets the string designator for hours that are "ante meridiem" 
        /// (before noon).
        /// </summary>
        /// <value>The AM designator.</value>
        public virtual string AMDesignator
        {
            get { return ActualCulture.DateTimeFormat.AMDesignator; }
        }

        /// <summary>
        /// Gets the string designator for hours that are "post meridiem" 
        /// (after noon).
        /// </summary>
        /// <value>The PM designator.</value>
        public virtual string PMDesignator
        {
            get { return ActualCulture.DateTimeFormat.PMDesignator; }
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
            // make sure everything that we do not understand is not part of the
            // format.
            // This method was discussed with BCL team.
            string transformed = new string(format.ToCharArray()
                                            .Where(c => Char.IsWhiteSpace(c) ||
                                                        TimeSeparators.Contains(c) ||
                                                        TimeChars.Contains(c))
                                            .Select(c => c).ToArray()).Trim();

            // feature: DateTime will not parse single characters.
            // the documented workaround is to add a space.
            if (transformed.Length == 1)
            {
                transformed = new string(new[] { transformed[0], ' ' });
            }

            return transformed;
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
            return input.ToString(CultureInfo.InvariantCulture)[0];
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
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unmap", Justification = "Unmapping is the opposite of mapping.")]
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
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Final implementation will surely use instance data.")]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Can be set from xaml.")]
        public virtual IEnumerable<TimeParser> GetActualTimeParsers(IEnumerable<TimeParser> parsers)
        {
            List<TimeParser> actualParsers = parsers == null ? new List<TimeParser>() : new List<TimeParser>(parsers);
            actualParsers.Add(new CatchallTimeParser());
            return actualParsers;
        }

        /// <summary>
        /// Formats the specified DateTime to a time string representation.
        /// </summary>
        /// <param name="value">The DateTime that should be formatted.</param>
        /// <param name="timeFormat">The time format used to describe how value
        /// should be formatted.</param>
        /// <param name="timeCharacters">The allowed characters in the format. 
        /// Leave empty to indicate that all characters are allowed. See remarks.</param>
        /// <returns>
        /// A string that represents the time part of a DateTime.
        /// </returns>
        /// <remarks>The TimeFormat will contain TimeCharacters in a certain 
        /// order, like hh:mm:ss. By passing specific TimeCharacters, these
        /// will get filtered and the method only returns part of the formatted
        /// string. Example: pass 'h', 't', 'H' to get back 4 AM, if the culture
        /// was set to en-US.</remarks>
        public virtual string FormatTime(DateTime? value, ITimeFormat timeFormat, params char[] timeCharacters)
        {
            if (value.HasValue)
            {
                if (timeFormat == null)
                {
                    throw new ArgumentNullException("timeFormat");
                }

                if (timeCharacters.Count() > 0)
                {
                    // if timeCharacters is used, only allow those characters.
                    string filtered = new string(timeFormat.GetTimeDisplayFormat(ActualCulture)
                                                     .ToCharArray()
                                                     .Where(c => timeCharacters.Contains(c))
                                                     .Select(c => c)
                                                     .ToArray());
                    // empty timeformat defaults to long datestring, 
                    // not filtering is a better default.
                    if (!string.IsNullOrEmpty(filtered))
                    {
                        timeFormat = new CustomTimeFormat(filtered);
                    }
                }

                string formatted = value.Value.ToString(GetTransformedFormat(timeFormat.GetTimeDisplayFormat(ActualCulture)), ActualCulture);
                // after formatting, allow globalization step to map digits.
                return new string(formatted.ToCharArray()
                    .Select(c => Char.IsDigit(c) ? MapDigitToCharacter(Int32.Parse(c.ToString(CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture)) : c)
                    .ToArray())
                    .Trim();
            }
            return string.Empty;
        }

        /// <summary>
        /// Parses a string into a DateTime using the specified ITimeFormat instance 
        /// and TimeParsers.
        /// </summary>
        /// <param name="mappedText">The text that was entered by the user.</param>
        /// <param name="timeFormat">The TimeFormat instance used to supply
        /// formats.</param>
        /// <param name="timeParsers">The time parsers.</param>
        /// <returns>
        /// A DateTime with a correctly set time part.
        /// </returns>
        /// <remarks>The date part of the DateTime is irrelevant and will be
        /// overwritten by the current date.
        /// </remarks>
        public DateTime? ParseTime(string mappedText, ITimeFormat timeFormat, IEnumerable<TimeParser> timeParsers)
        {
            // will perform same logic as TryParse, but will possibly throw.
            if (timeFormat == null)
            {
                throw new ArgumentNullException("timeFormat");
            }

            DateTime? result;
            if (TryParseTime(mappedText, timeFormat, timeParsers, out result))
            {
                return result;
            }

            // throw exception
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Cannot parse text '{0}'",
                mappedText);
            throw new ArgumentException(message, "mappedText");
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
        public bool TryParseTime(string mappedText, ITimeFormat timeFormat, IEnumerable<TimeParser> timeParsers, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrEmpty(mappedText))
            {
                return true;
            }
            string value = new string(mappedText.ToCharArray().Select(c => TimeSeparators.Contains(c) ? c : MapCharacterToDigit(c)).ToArray());
            if (timeFormat != null)
            {
                DateTime realResult;
                // try using formats.
                if (DateTime.TryParseExact(
                    value,
                    timeFormat.GetTimeParseFormats(ActualCulture).Select(s => GetTransformedFormat(s)).ToArray(),
                    ActualCulture,
                    DateTimeStyles.None,
                    out realResult))
                {
                    result = realResult;
                    return true;
                }
            }

            // try using custom collection of parsers.
            TimeParserCollection timeParserCollection = new TimeParserCollection(GetActualTimeParsers(timeParsers));
            DateTime? parsedResult;
            if (timeParserCollection.TryParse(mappedText, ActualCulture, out parsedResult))
            {
                result = parsedResult;
                return true;
            }

            return false;
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
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Although complex, breaking up the method would break mindflow.")]
        public virtual TimeSpan GetTimeUnitAtTextPosition(string text, int textPosition, ITimeFormat timeFormat)
        {
            // validate
            if (string.IsNullOrEmpty(text) || textPosition < 0 || textPosition > text.Length)
            {
                return new TimeSpan();
            }

            if (timeFormat == null)
            {
                throw new ArgumentNullException("timeFormat");
            }

            // the position that is taken into account is always the 
            // character that comes after the caret. If we are at the
            // end of the text, we will want to parse the character
            // before.

            // if the caret is currently at a timeseperator (:, or .)
            // also use the previous character.
            if (textPosition == text.Length || TimeSeparators.Contains(text[textPosition]))
            {
                // act on character before
                return GetTimeUnitAtTextPosition(text, (textPosition - 1), timeFormat);
            }

            // if the caret is at a whitespace, look around for the first real character
            if (Char.IsWhiteSpace(text[textPosition]))
            {
                int offset = 1;
                while (textPosition + offset < text.Length || textPosition - offset >= 0)
                {
                    if (textPosition - offset >= 0 && !Char.IsWhiteSpace(text[textPosition - offset]))
                    {
                        return GetTimeUnitAtTextPosition(text, textPosition - offset, timeFormat);
                    }
                    if (textPosition + offset < text.Length && !Char.IsWhiteSpace(text[textPosition + offset]))
                    {
                        return GetTimeUnitAtTextPosition(text, textPosition + offset, timeFormat);
                    }

                    offset++;
                }
            }

            #region handle am/pm flip and return
            // find out information about usage of AM/PM
            int designatorStartIndex = GetDesignatorTextPositionStart(text);
            int designatorEndIndex = GetDesignatorTextPositionEnd(text, designatorStartIndex);

            if (textPosition >= designatorStartIndex && textPosition < designatorEndIndex)
            {
                return TimeSpan.FromHours(12);
            }
            #endregion

            // find out the timespan that the spin has effect on
            // by clearing all the numbers (set to 0) and only setting to 1
            // at the caretposition. The remaining timespan can be used to
            // determine how to increment.
            StringBuilder timeSpanBuilder = new StringBuilder(text.Length);

            #region Determine timespan
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // fill with 0
                timeSpanBuilder.Append('0');

                // set a 1
                timeSpanBuilder[i] = i == textPosition ? '1' : '0';

                // copy over separator
                if (TimeSeparators.Contains(c))
                {
                    timeSpanBuilder[i] = c;
                }

                // copy over designator
                if (i >= designatorStartIndex && i < designatorEndIndex)
                {
                    timeSpanBuilder[i] = c;
                }

                // retain white space
                if (char.IsWhiteSpace(c))
                {
                    timeSpanBuilder[i] = c;
                }
            }
            #endregion

            string NulledTimeAM = timeSpanBuilder.ToString();
            if (!string.IsNullOrEmpty(PMDesignator))
            {
                NulledTimeAM = NulledTimeAM.Replace(PMDesignator, AMDesignator);
            }
            DateTime? spinnedTime;
            if (TryParseTime(NulledTimeAM, timeFormat, null, out spinnedTime) && spinnedTime.HasValue)
            {
                // behavior is special for hours.
                // we do not do contextual spinning on the hours, since this
                // turns out to be too confusing.
                if (spinnedTime.Value.TimeOfDay.Hours == 10)
                {
                    return TimeSpan.FromHours(1);
                }
                return spinnedTime.Value.TimeOfDay;
            }
            else
            {
                return new TimeSpan();
            }
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
        public virtual int GetTextPositionForTimeUnit(string text, TimeSpan timeSpan, ITimeFormat timeFormat)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            int designatorStartIndex = GetDesignatorTextPositionStart(text);
            int designatorEndIndex = GetDesignatorTextPositionEnd(text, designatorStartIndex);
            if (timeSpan == TimeSpan.FromHours(12))
            {
                return designatorStartIndex;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (i > designatorStartIndex && i < designatorEndIndex)
                {
                    continue;
                }

                char c = text[i];
                if (Char.IsWhiteSpace(c) || TimeSeparators.Contains(c))
                {
                    continue;
                }

                if (timeSpan == GetTimeUnitAtTextPosition(text, i, timeFormat))
                {
                    return i;
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
            // special case: value is at DateTime.MaxValue
            if (DateTime.MaxValue.Date == value.Date && value.TimeOfDay.Add(timeSpan) > TimeSpan.FromDays(1))
            {
                return value.AddDays(-1).Add(timeSpan);
            }

            return value.Add(timeSpan);
        }

        /// <summary>
        /// Subtracts a time span from a date time in a global context.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The DateTime after decrementing by TimeSpan.</returns>
        public virtual DateTime OnDecrement(DateTime value, TimeSpan timeSpan)
        {
            // special case: value is at DateTime.MinValue
            if (DateTime.MinValue.Date == value.Date && value.TimeOfDay < timeSpan)
            {
                return value.AddDays(1).Subtract(timeSpan);
            }

            return value.Subtract(timeSpan);
        }

        /// <summary>
        /// Gets the caret position at the start of the designator.
        /// </summary>
        /// <param name="text">The text that might include a designator.</param>
        /// <returns>Caret position for the end of the designator,
        /// or -1 if none found.</returns>
        private int GetDesignatorTextPositionStart(string text)
        {
            int designatorStartIndex = text.IndexOf(AMDesignator, StringComparison.OrdinalIgnoreCase);
            if (designatorStartIndex == -1)
            {
                designatorStartIndex = text.IndexOf(PMDesignator, StringComparison.OrdinalIgnoreCase);
            }
            return designatorStartIndex;
        }

        /// <summary>
        /// Gets the caret position at the end of the designator.
        /// </summary>
        /// <param name="text">The text that might include a designator.</param>
        /// <param name="designatorStartIndex">Start index of the designator.</param>
        /// <returns>
        /// Caret position for the end of the designator,
        /// or -1 if none found.
        /// </returns>
        private int GetDesignatorTextPositionEnd(string text, int designatorStartIndex)
        {
            if (text.Contains(AMDesignator, StringComparison.OrdinalIgnoreCase))
            {
                return designatorStartIndex + AMDesignator.Length;
            }

            if (text.Contains(PMDesignator, StringComparison.OrdinalIgnoreCase))
            {
                return designatorStartIndex + PMDesignator.Length;
            }

            return -1;
        }
    }
}
