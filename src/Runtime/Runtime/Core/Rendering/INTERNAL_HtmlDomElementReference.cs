
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

using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
    public class INTERNAL_HtmlDomElementReference : IJavaScriptConvertible
    {
        private string _jsCache;
        private INTERNAL_HtmlDomStyleReference _style;

        public INTERNAL_HtmlDomElementReference(string uniqueIdentifier)
        {
            UniqueIdentifier = uniqueIdentifier;
        }

        public string UniqueIdentifier { get; }

        internal INTERNAL_HtmlDomStyleReference Style => _style ??= new INTERNAL_HtmlDomStyleReference(UniqueIdentifier);

        string IJavaScriptConvertible.ToJavaScriptString() => _jsCache ??= $"document.getElementByIdSafe(\"{UniqueIdentifier}\")";
    }
}
