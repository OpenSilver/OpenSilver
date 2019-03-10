
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
