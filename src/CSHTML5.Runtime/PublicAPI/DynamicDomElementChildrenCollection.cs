

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
using CSHTML5.Internal;

public static partial class CSharpXamlForHtml5
{
    public static partial class DomManagement
    {
        public static partial class Types
        {
#if !BRIDGE
            [JSIgnore]
#else
            [External]
#endif
            public class DynamicDomElementChildrenCollection
            {
                INTERNAL_HtmlDomElementReference _parentDomElementRef;

                internal DynamicDomElementChildrenCollection(INTERNAL_HtmlDomElementReference parentDomElementRef)
                {
                    _parentDomElementRef = parentDomElementRef;
                }

                public DynamicDomElement this[int index]
                {
                    get
                    {
                        return new DynamicDomElement(INTERNAL_HtmlDomManager.GetChildDomElementAt(_parentDomElementRef, index));
                    }
                }
            }

        }
    }
}
