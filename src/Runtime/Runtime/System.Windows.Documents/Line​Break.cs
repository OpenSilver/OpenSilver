﻿

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
    public sealed partial class LineBreak : Inline
    {
        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var linebreak = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("br", parentRef, this);
            domElementWhereToPlaceChildren = linebreak;
            return linebreak;
        }
    }
}
