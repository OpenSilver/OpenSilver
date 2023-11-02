

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenSilver.Compiler
{
    internal static class ProcessingHtmlPresenterNodes
    {
        //------------------------------------------------------------
        // This class process the "HtmlPresenter" nodes in order to
        // "escape" its content so that it is not processed during
        // the rest of the compilation, and it is instead considered
        // like plain text.
        //------------------------------------------------------------

        public static string Process(string xaml)
        {
            return global::System.Text.RegularExpressions.Regex.Replace(xaml, @"(?:(<native:HtmlPresenter[^>]*?\/>)|(<[^<:>\/]+:HtmlPresenter[\s\S]*?>)([\s\S]*?)(<\/[^<:>\/]+:HtmlPresenter>))",
                new global::System.Text.RegularExpressions.MatchEvaluator((global::System.Text.RegularExpressions.Match match) =>
                {
                    if (match.Groups.Count >= 3 && !string.IsNullOrEmpty(match.Groups[4].Value))
                    {
                        string htmlPresenterContent = match.Groups[3].Value;

                        // Replace characters:
                        string escapedContent = htmlPresenterContent
                            .Replace("&", "&amp;") // Note: it's important that this line be the first replacement, otherwise it replaces the replaced characters again that contain a "&".
                            .Replace("'", "&apos;")
                            .Replace("\"", "&quot;")
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;");
                        //.Replace(" ", "&#160;");

                        return match.Groups[2].Value + escapedContent + match.Groups[4].Value;
                    }
                    else
                    {
                        return match.Value;
                    }

                }));
        }
    }
}
