
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
using System;
using System.ComponentModel;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
    public class INTERNAL_HtmlDomElementReference : IJavaScriptConvertible
    {
        private string _jsCache;
        private INTERNAL_HtmlDomElementReference _parent;
        private INTERNAL_HtmlDomStyleReference _style;
#pragma warning disable CS0618 // Type or member is obsolete
        private INTERNAL_Html2dContextReference _context2d;
#pragma warning restore CS0618 // Type or member is obsolete

        public INTERNAL_HtmlDomElementReference(string uniqueIdentifier, INTERNAL_HtmlDomElementReference parent)
        {
            UniqueIdentifier = uniqueIdentifier;
#pragma warning disable CS0618 // Type or member is obsolete
            Parent = parent;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public string UniqueIdentifier { get; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public INTERNAL_HtmlDomElementReference Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent != null)
                    _parent.FirstChild = null;
                _parent = value;
                if (_parent != null)
                    _parent.FirstChild = this;
            }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public INTERNAL_HtmlDomElementReference FirstChild { get; internal set; }
        
        internal INTERNAL_HtmlDomStyleReference Style => _style ??= new INTERNAL_HtmlDomStyleReference(UniqueIdentifier);

#pragma warning disable CS0618 // Type or member is obsolete
        internal INTERNAL_Html2dContextReference Context2d => _context2d ??= new INTERNAL_Html2dContextReference(UniqueIdentifier);
#pragma warning restore CS0618 // Type or member is obsolete

        string IJavaScriptConvertible.ToJavaScriptString() => _jsCache ??= $"document.getElementByIdSafe(\"{UniqueIdentifier}\")";
    }
}
