

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
using CSHTML5.Internal;

namespace CSHTML5.Native.Html
{
    public static class UriHelper
    {
        /// <summary>
        /// Converts a XAML-style URI (such as "ms-appx///") into a path that can be used in HTML.
        /// </summary>
        /// <param name="uri">A XAML-style URI such as ms-appx:///AssemblyName/Folder/Image.png or /AssemblyName;component/Folder/Image.png</param>
        /// <returns>A path that can be used in HTML</returns>
        public static string ConvertToHtmlPath(string uri)
        {
            return INTERNAL_UriHelper.ConvertToHtml5Path(uri, null);
        }
    }
}
