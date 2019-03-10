
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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Represents an inline element that causes a new line to begin in content when rendered in a text container.
    /// </summary>
    public sealed class LineBreak : Inline
    {
        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            dynamic linebreak = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("br", parentRef, this);
            domElementWhereToPlaceChildren = linebreak;
            return linebreak;
        }
    }
}
