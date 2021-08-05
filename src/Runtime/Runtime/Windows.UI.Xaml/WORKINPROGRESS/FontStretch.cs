﻿

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


using System.ComponentModel;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not supported yet
    [TypeConverter(typeof(FontStretchConverter))]
    [OpenSilver.NotImplemented]
    public partial struct FontStretch : IFormattable
    {
		[OpenSilver.NotImplemented]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        internal object ToOpenTypeStretch()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
#endif
}
