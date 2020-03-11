

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


#if PUBLIC_API_THAT_REQUIRES_SUPPORT_OF_DYNAMIC
using System;
using CSHTML5.Internal;
using JSIL.Meta;
using System.Dynamic;
using System.Collections.Generic;

public static partial class CSharpXamlForHtml5
{
    public static partial class DomManagement
    {
        public static partial class Types
        {
            // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
            [JSIgnore]
            public class Document
            {
                public dynamic createElement(string domElementTag)
                {
                    return new DynamicDomElement(INTERNAL_HtmlDomManager.CreateDomElementAndAppendItToTempLocation_ForUseByPublicAPIOnly_SimulatorOnly(domElementTag));
                }
            }
        }
    }
}
#endif