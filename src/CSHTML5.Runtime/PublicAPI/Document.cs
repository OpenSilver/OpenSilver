
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