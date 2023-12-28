// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Windows.Controls
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
        /// <summary>
        /// Expression used to parse.
        /// </summary>
        private static readonly Regex exp = new Regex(@"((?<hours>([0-1][\d]?))|(?<hours>(^2[0-3]?))|(?<hours>(^[3-9])))[^\d]?(?<minutes>\d{0,2})[^\d]?(?<seconds>\d{0,2})", RegexOptions.CultureInvariant);

        /// <summary>
        /// Tries to parse a string to a DateTime.
        /// </summary>
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
            Match match = exp.Match(text);

            if (match.Success)
            {
                bool pm = text.Contains("p", StringComparison.OrdinalIgnoreCase);

                result = null;

                int hours = int.Parse(match.Groups["hours"].Value, culture);
                if (hours > 23)
                {
                    return false;
                }
                int minutes = match.Groups["minutes"].Success && match.Groups["minutes"].Value.Length > 0 ? int.Parse(match.Groups["minutes"].Value, culture) : 0;
                if (minutes > 59)
                {
                    return false;
                }
                int seconds = match.Groups["seconds"].Success && match.Groups["seconds"].Value.Length > 0 ? int.Parse(match.Groups["seconds"].Value, culture) : 0;
                if (seconds > 59)
                {
                    return false;
                }

                result = DateTime.Now.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
                result = result.Value.AddHours(pm && hours < 12 ? 12 : 0);
                return true;
            }
            result = null;
            return false;
        }
    }
}
