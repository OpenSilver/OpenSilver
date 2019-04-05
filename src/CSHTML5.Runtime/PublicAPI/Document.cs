
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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