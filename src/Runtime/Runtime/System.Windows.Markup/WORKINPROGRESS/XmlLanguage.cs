

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


#if WORKINPROGRESS
using System.ComponentModel;

namespace System.Windows.Markup
{
    [TypeConverter(typeof(XmlLanguageConverter))]
    [OpenSilver.NotImplemented]
    public sealed partial class XmlLanguage
    {
		[OpenSilver.NotImplemented]
        public string IetfLanguageTag { get; }

		[OpenSilver.NotImplemented]
        public static XmlLanguage GetLanguage(string ietfLanguageTag)
        {
            return null;
        }
    }
}

#endif