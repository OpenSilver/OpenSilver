

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
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a single time format used for parsing and formatting.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class CustomTimeFormat : ITimeFormat
    {
        /// <summary>
        /// Gets or sets the custom format that is used to parse or display
        /// a String or DateTime.
        /// </summary>
        [OpenSilver.NotImplemented]
        public string Format { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimeFormat"/> class.
        /// </summary>
        /// <param name="format">The format that is used to parse or display
        /// a String or DateTime.</param>
        [OpenSilver.NotImplemented]
        public CustomTimeFormat(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimeFormat"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public CustomTimeFormat()
        {
        }

        /// <summary>
        /// Gets the format to use to display a DateTime as a time value.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// A format to use during display of a DateTime.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string GetTimeDisplayFormat(CultureInfo culture)
        {
            return Format;
        }

        /// <summary>
        /// Gets the format to use to parse a string to a DateTime.
        /// </summary>
        /// <param name="culture">Culture used to determine formats.</param>
        /// <returns>
        /// An array of formats to be used during parsing.
        /// </returns>
        [OpenSilver.NotImplemented]
        public string[] GetTimeParseFormats(CultureInfo culture)
        {
            var formats = new List<string>(7) { Format };

            var info = culture.DateTimeFormat;
            if (info != null)
            {
                formats.Add(info.ShortTimePattern);
                formats.Add(info.LongTimePattern);
            }
            return formats.ToArray();
        }
    }
}
