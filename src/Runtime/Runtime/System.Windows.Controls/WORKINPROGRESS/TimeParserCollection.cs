

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
using System.Collections.ObjectModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a collection of TimeParser objects.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class TimeParserCollection : Collection<TimeParser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeParserCollection"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimeParserCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeParserCollection"/> class.
        /// </summary>
        /// <param name="parsers">A sequence of TimeParser objects that will
        /// be copied into this collection.</param>
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
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
