
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

using System.Collections.Generic;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides methods for encoding and decoding HTML and URL strings.
    /// </summary>
    public static class HttpUtility
    {
        private static readonly Dictionary<string, string> codeNamesToChars;

        static HttpUtility()
        {
            codeNamesToChars = new Dictionary<string, string>()
            {
                { "&nbsp;", " " },
                {"&quot;", "\""},
                {"&amp;", "&"},
                {"&apos;", "'"},
                {"&lt;", "<"},
                {"&gt;", ">"},
                {"&iexcl;", "¡"},
                {"&cent;", "¢"},
                {"&pound;", "£"},
                {"&curren;", "¤"},
                {"&yen;", "¥"},
                {"&brvbar;", "¦"},
                {"&sect;", "§"},
                {"&uml;", "¨"},
                {"&copy;", "©"},
                {"&ordf;", "ª"},
                {"&laquo;", "«"},
                {"&not;", "¬"},
                {"&shy;", "­"},
                {"&reg;", "®"},
                {"&macr;", "¯"},
                {"&deg;", "°"},
                {"&plusmn;", "±"},
                {"&sup2;", "²"},
                {"&sup3;", "³"},
                {"&acute;", "´"},
                {"&micro;", "µ­"},
                {"&para;", "¶"},
                {"&middot;", "·"},
                {"&cedil;", "¸"},
                {"&sup1;", "¹"},
                {"&ordm;", "º"},
                {"&raquo;", "»"},
                {"&frac14;", "¼"},
                {"&frac12;", "½"},
                {"&frac34;", "¾"},
                {"&iquest;", "¿"},
                {"&Agrave;", "À"},
                {"&Aacute;", "Á"},
                {"&Acirc;", "Â"},
                {"&Atilde;", "Ã"},
                {"&Auml;", "Ä"},
                {"&Aring;", "Å"},
                {"&AElig;", "Æ"},
                {"&Ccedil;", "Ç"},
                {"&Egrave;", "È"},
                {"&Eacute;", "É"},
                {"&Ecirc;", "Ê"},
                {"&Euml;", "Ë"},
                {"&Igrave;", "Ì"},
                {"&Iacute;", "Í"},
                {"&Icirc;", "Î"},
                {"&Iuml;", "Ï"},
                {"&ETH;", "Ð"},
                {"&Ntilde;", "Ñ"},
                {"&Ograve;", "Ò"},
                {"&Oacute;", "Ó"},
                {"&Ocirc;", "Ô"},
                {"&Otilde;", "Õ"},
                {"&Ouml;", "Ö"},
                {"&times;", "×"},
                {"&Oslash;", "Ø"},
                {"&Ugrave;", "Ù"},
                {"&Uacute;", "Ú"},
                {"&Ucirc;", "Û"},
                {"&Uuml;", "Ü"},
                {"&Yacute;", "Ý"},
                {"&THORN;", "Þ"},
                {"&szlig;", "ß"},
                {"&agrave;", "à"},
                {"&aacute;", "á"},
                {"&acirc;", "â"},
                {"&atilde;", "ã"},
                {"&auml;", "ä"},
                {"&aring;", "å"},
                {"&aelig;", "æ"},
                {"&ccedil;", "ç"},
                {"&egrave;", "è"},
                {"&eacute;", "é"},
                {"&ecirc;", "ê"},
                {"&euml;", "ë"},
                {"&igrave;", "ì"},
                {"&iacute;", "í"},
                {"&icirc;", "î"},
                {"&iuml;", "ï"},
                {"&eth;", "ð"},
                {"&ntilde;", "ñ"},
                {"&ograve;", "ò"},
                {"&oacute;", "ó"},
                {"&ocirc;", "ô"},
                {"&otilde;", "õ"},
                {"&ouml;", "ö"},
                {"&divide;", "÷"},
                {"&oslash;", "ø"},
                {"&ugrave;", "ù"},
                {"&uacute;", "ú"},
                {"&ucirc;", "û"},
                {"&uuml;", "ü"},
                {"&yacute;", "ý"},
                {"&thorn;", "þ"},
                {"&yuml;", "ÿ"},

                {"&bull;", "•"},
                {"&infin;", "∞"},
                {"&permil;", "‰"},
                {"&sdot;", "⋅"},
                {"&dagger;", "†"},
                {"&mdash;", "—"},
                {"&perp;", "⊥"},
                {"&par;", "∥"},

                {"&euro;", "€"},
                {"&trade;", "™"},

                {"&alpha;", "α"},
                {"&beta;", "β"},
                {"&gamma;", "γ"},
                {"&delta;", "δ"},
                {"&epsilon;", "ε"},
                {"&zeta;", "ζ"},
                {"&eta;", "η"},
                {"&theta;", "θ"},
                {"&iota;", "ι"},
                {"&kappa;", "κ"},
                {"&lambda;", "λ"},
                {"&mu;", "μ"},
                {"&nu;", "ν"},
                {"&xi;", "ξ"},
                {"&omicron;", "ο"},
                {"&pi;", "π"},
                {"&rho;", "ρ"},
                {"&sigma;", "σ"},
                {"&tau;", "τ"},
                {"&upsilon;", "υ"},
                {"&phi;", "φ"},
                {"&chi;", "χ"},
                {"&psi;", "ψ"},
                {"&omega;", "ω"},
                {"&Alpha;", "Α"},
                {"&Beta;", "Β"},
                {"&Gamma;", "Γ"},
                {"&Delta;", "Δ"},
                {"&Epsilon;", "Ε"},
                {"&Zeta;", "Ζ"},
                {"&Eta;", "Η"},
                {"&Theta;", "Θ"},
                {"&Iota;", "Ι"},
                {"&Kappa;", "Κ"},
                {"&Lambda;", "Λ"},
                {"&Mu;", "Μ"},
                {"&Nu;", "Ν"},
                {"&Xi;", "Ξ"},
                {"&Omicron;", "Ο"},
                {"&Pi;", "Π"},
                {"&Rho;", "Ρ"},
                {"&Sigma;", "Σ"},
                {"&Tau;", "Τ"},
                {"&Upsilon;", "Υ"},
                {"&Phi;", "Φ"},
                {"&Chi;", "Χ"},
                {"&Psi;", "Ψ"},
                {"&Omega;", "Ω"}
            };
        }


        /// <summary>
        /// Converts a string that has been HTML-encoded (for HTTP transmission) into
        /// a decoded string.
        /// </summary>
        /// <param name="html">An HTML-encoded string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string HtmlDecode(string html)
        {
            //safety step: we prevent "&#38;" to be replaced with "&" in the first step. This way, we won't have surprises such as &#38;amp&#59; turn into "&amp;" which will then be transformed in "&" when decoding the name codes.
            //             we replace it with &>#38;, which should never exist in an encoded html string (since '&' should be transformed into "&amp;" or "&#38;")
            //             Note: we will throw an exception if &!#38; already exists (still safety measure)
            if (html.Contains("&>#38;"))
                throw new FormatException("The html string to decode is in an incorrect format.");

            string decodedHtml = html.Replace("&#38;", "&>#38;");

            //same thing for "&#x26;" which is the equivalent hexadecimal-style version of "&#38;":
            if (decodedHtml.Contains("&>#x26;"))
                throw new FormatException("The html string to decode is in an incorrect format.");

            decodedHtml = decodedHtml.Replace("&#x26;", "&>#x26;");


            //Initial decoding:
            decodedHtml = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript(@"$0.replace(/&#\d+;/gim, function(i) { return String.fromCharCode(i.substring(2,i.length-1));} )", decodedHtml));

            //Decoding hexadecimal-style encoding:
            decodedHtml = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript(@"$0.replace(/&#x[a-fA-F0-9]+;/gim, function(i) { return String.fromCharCode(parseInt(i.substring(3,i.length-1), 16));} )", decodedHtml));
        #region line above explained:
            //decodedHtml.replace(/&#x[a-fA-F0-9]+;/gim, //regex that accepts anything that starts with &#x followed by an hexadecimal number, followed by ';'
            //    function(i) //the function to apply on each match of the regex and that returns the string with which to replace it
            //    {
            //        return String.fromCharCode(       //gets the character corresponding to the char code with the given decimal value (so with 38 as parameter, it returns '&')
            //          parseInt(                       // with 16 as the second parameter, returns the decimal value of the string entered as 1st parameter considered as hexadecimal
            //            i.substring(3,i.length-1),    // cleans the match by removing the first 3 characters ("&#x") and the ';' at the end, so only the hexa number is left in the string.
            //            16));
            //    }
            //)
        #endregion

            //Now we want to decode the elements that have been encoded with their named codes:
            int i = 0;
            while (i != -1 && i < decodedHtml.Length)
            {
                i = decodedHtml.IndexOf('&', i);
                if (i != -1)
                {
                    int j = decodedHtml.IndexOf(';', i + 1);
                    if (j != -1)
                    {
                        string sub = decodedHtml.Substring(i, j - i + 1);
                        if (codeNamesToChars.ContainsKey(sub))
                        {
                            decodedHtml = decodedHtml.Replace(sub, codeNamesToChars[sub]);
                        }
                        ++i;
                    }
                    else
                    {
                        //there is no ';' left so we can consider the decoding finished.
                        i = -1;
                    }
                }
            }


            //We now undo the safety step and replace it with '&':
            decodedHtml = decodedHtml.Replace("&>#38;", "&");

            return decodedHtml;
        }

        /// <summary>
        /// Converts a text string into an HTML-encoded string.
        /// </summary>
        /// <param name="html">The text to HTML-encode.</param>
        /// <returns>An HTML-encoded string.</returns>
        public static string HtmlEncode(string html)
        {
            //brutal way but dealing with each case separately seems like it might be a lot:
            // Here, we will replace all non-alphanumeric characters with their html character (so '&' => "&#38;" for example)
            //Regex regex = new Regex("[^a-zA-Z0-9]");
            string encodedHtml = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript(@"$0.replace(/[^a-zA-Z0-9]/gim, function(i) { return '&#' + i.charCodeAt(0) + ';'} )", html));
            return encodedHtml;

            ////very limited way (for html, a lot more cases need to be handled than for xml apparently: https://owasp.org/www-project-cheat-sheets/cheatsheets/Cross_Site_Scripting_Prevention_Cheat_Sheet.html)
            //string escapedHtml = INTERNAL_EscapeHelpers.EscapeXml(html);
            //escapedHtml = escapedHtml.Replace("/", "&#x27;")
            //                         .Replace("&apos;", "&#x2F;"); //apparently, &apos; is not in the HTML spec so we replace it.
            //return escapedHtml;
            ////return Interop.ExecuteJavaScript("encodeURIComponent($0)", html).ToString();
        }

        /// <summary>
        /// Converts a string that has been encoded for transmission in a URL into a
        /// decoded string.
        /// </summary>
        /// <param name="url">A URL-encoded string to decode.</param>
        /// <returns>A decoded string.</returns>
#if BRIDGE
        [Bridge.Template("{url} == null ? {url} : decodeURIComponent({url})")]
#endif
        public static string UrlDecode(string url)
        {
            if (url == null)
            {
                return null;
            }

            return CSHTML5.Interop.ExecuteJavaScript("decodeURIComponent($0)", url).ToString();
        }

        /// <summary>
        /// Converts a text string into a URL-encoded string.
        /// </summary>
        /// <param name="url">The text to URL-encode.</param>
        /// <returns>A URL-encoded string.</returns>
#if BRIDGE
        [Bridge.Template("{url} == null ? {url} : encodeURIComponent({url})")]
#endif
        public static string UrlEncode(string url)
        {
            if (url == null)
            {
                return null;
            }

            return CSHTML5.Interop.ExecuteJavaScript("encodeURIComponent($0)", url).ToString();
        }
    }
}
