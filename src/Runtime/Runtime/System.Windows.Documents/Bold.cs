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


#if MIGRATION
#else
using Windows.UI.Text;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Provides an inline-level content element that causes content to render with a bold font weight.
    /// </summary>
    public sealed partial class Bold : Span
    {
        /// <summary>
        /// Initializes a new instance of the Bold class.
        /// </summary>
        public Bold()
        {
            FontWeight = FontWeights.Bold;
        }
    }
}
