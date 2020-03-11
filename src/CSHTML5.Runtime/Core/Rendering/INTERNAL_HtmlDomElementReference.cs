

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
#if !BRIDGE
    [JSIgnore]
#else
    [External]
#endif
    public class INTERNAL_HtmlDomElementReference
    {
        public INTERNAL_HtmlDomElementReference(string uniqueIdentifier, INTERNAL_HtmlDomElementReference parent)
        {
            this.UniqueIdentifier = uniqueIdentifier;
            this.Parent = parent;
        }

        public string UniqueIdentifier { get; private set; }

        private INTERNAL_HtmlDomElementReference _parent;
        public INTERNAL_HtmlDomElementReference Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent != null)
                    _parent.FirstChild = null;
                _parent = value;
                if (_parent != null)
                    _parent.FirstChild = this; //what happens when we have multiple children? (is it even possible?)
            }
        }

        public INTERNAL_HtmlDomElementReference FirstChild { get; internal set; }
    }
}
