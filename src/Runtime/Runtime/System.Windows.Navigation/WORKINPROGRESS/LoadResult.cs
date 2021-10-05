

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


#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    [OpenSilver.NotImplemented]
    public class LoadResult
    {
        public object LoadedContent { get; private set; }

        public Uri RedirectUri { get; private set; }

        [OpenSilver.NotImplemented]
        public LoadResult(object loadedContent)
        {
        }

        [OpenSilver.NotImplemented]
        public LoadResult(Uri redirectUri)
        {
        }
    }
}
