
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

using System.Windows.Automation.Provider;

namespace System.Windows.Automation.Text
{
    /// <summary>
    /// Identifies text range endpoints for methods of  <see cref="ITextRangeProvider" />.
    /// </summary>
    public enum TextPatternRangeEndpoint
    {
        /// <summary>
        /// The start point of the range.
        /// </summary>
        Start,
        /// <summary>
        /// The endpoint of the range.
        /// </summary>
        End,
    }
}
