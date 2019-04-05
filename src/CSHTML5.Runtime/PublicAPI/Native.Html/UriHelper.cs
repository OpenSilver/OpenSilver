
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
