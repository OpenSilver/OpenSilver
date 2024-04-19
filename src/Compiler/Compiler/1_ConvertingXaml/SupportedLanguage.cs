
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

namespace OpenSilver.Compiler
{
    public enum SupportedLanguage
    {
        CSharp,
        VBNet,
        FSharp,
        Unknown,
    }

    internal static class LanguageHelpers
    {
        public static SupportedLanguage GetLanguage(string language)
        {
            if (string.Equals(language, "c#", StringComparison.OrdinalIgnoreCase))
            {
                return SupportedLanguage.CSharp;
            }
            else if (string.Equals(language, "vb", StringComparison.OrdinalIgnoreCase))
            {
                return SupportedLanguage.VBNet;
            }
            else if (string.Equals(language, "f#", StringComparison.OrdinalIgnoreCase))
            {
                return SupportedLanguage.FSharp;
            }
            else
            {
                return SupportedLanguage.Unknown;
            }
        }
    }
}
