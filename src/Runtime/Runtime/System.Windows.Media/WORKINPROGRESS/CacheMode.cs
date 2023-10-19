
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

namespace System.Windows.Media
{
    [OpenSilver.NotImplemented]
    public abstract class CacheMode : DependencyObject
    {
        /// <summary>
        /// Parse - this method is called by the type converter to parse a CacheMode's string 
        /// (provided in "value").
        /// </summary>
        /// <returns>
        /// A CacheMode which was created by parsing the "value" argument.
        /// </returns>
        /// <param name="value"> String representation of a CacheMode. Cannot be null/empty. </param>
        internal static CacheMode Parse(string value)
        {
            if (value != null &&
                value.Equals("BitmapCache", StringComparison.OrdinalIgnoreCase))
            {
                return new BitmapCache();
            }
            
            throw new FormatException("Token is not valid.");
        }
    }
}
