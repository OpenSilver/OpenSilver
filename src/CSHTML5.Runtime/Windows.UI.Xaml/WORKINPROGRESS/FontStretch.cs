

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not supported yet
    [SupportsDirectContentViaTypeFromStringConverters]
    public partial struct FontStretch : IFormattable
    {
        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        static FontStretch()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontStretch), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string fontStretchAsString)
        {
            return new FontStretch();
        }
    }
    #endregion
#endif
}
