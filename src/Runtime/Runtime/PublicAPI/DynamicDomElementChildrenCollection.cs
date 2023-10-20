
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

using CSHTML5.Internal;

public static partial class CSharpXamlForHtml5
{
    public static partial class DomManagement
    {
        public static partial class Types
        {
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
