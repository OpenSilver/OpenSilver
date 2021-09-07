

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
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Converts the specified string representation of a time to its DateTime equivalent.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class TimeParser
    {
        /// <summary>
        /// Converts the specified string representation of a time to its DateTime 
        /// equivalent and returns a value that indicates whether the conversion 
        /// succeeded.
        /// </summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>True if the parse was successful, false if it was not.</returns>
        [OpenSilver.NotImplemented]
        public virtual bool TryParse(string text, CultureInfo culture, out DateTime? result)
        {
            result = null;
            return false;
        }
    }
}
