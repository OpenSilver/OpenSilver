

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


using DotNetForHtml5.Core;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
#if WORKINPROGRESS
    [TypeConverter(typeof(FontFamilyTypeConverter))]
    public abstract partial class CacheMode : DependencyObject
    {
        static CacheMode()
        {
            TypeFromStringConverters.RegisterConverter(typeof(CacheMode), INTERNAL_ConvertFromString);            
        }

        internal static object INTERNAL_ConvertFromString(string cacheModeAsString)
        {
            string cacheModeAsStringToLower = cacheModeAsString.ToLower();
            switch (cacheModeAsStringToLower)
            {
                case "bitmapcache":
                    return new BitmapCache();
                default:
                    throw new Exception("\"" + cacheModeAsString + "\"" + " is not a supported type.");
            }
        }
    }
#endif
}
