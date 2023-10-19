// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a collection of TimeParser objects.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class TimeParserCollection : Collection<TimeParser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeParserCollection"/> class.
        /// </summary>
        public TimeParserCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeParserCollection"/> class.
        /// </summary>
        /// <param name="parsers">A sequence of TimeParser objects that will
        /// be copied into this collection.</param>
        public TimeParserCollection(IEnumerable<TimeParser> parsers)
        {
            if (parsers != null)
            {
                foreach (TimeParser parser in parsers)
                {
                    Add(parser);
                }
            }
        }

        /// <summary>
        /// Tries to parse a string to a DateTime.
        /// </summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>True if the parse was successful, false if it was not.</returns>
        public virtual bool TryParse(string text, CultureInfo culture, out DateTime? result)
        {
            result = null;
            foreach (TimeParser parser in this)
            {
                if (parser.TryParse(text, culture, out result))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
