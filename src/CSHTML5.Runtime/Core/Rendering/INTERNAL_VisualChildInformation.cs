
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal class INTERNAL_VisualChildInformation
    {
        // Understanding the concept:
        // For some layouts like vertical stackpanels (implemented using table-like nested divs), the parent needs to create
        // one row for each child. Therefore, it needs to "wrap" the child into some DOM nodes that we call "ChildWrapper".
        // Each of those wrappers has an "outer" element that lets us remove the wrapper from the parent, and an "inner"
        // element that is the place where the real child element will be placed (in the previous example, the "inner" element is
        // a table cell).
        // Note: for structures that don't require the creation of "wrappers" around their children, those two fields must be left blank.

        public UIElement INTERNAL_UIElement { get; set; }
        public dynamic INTERNAL_OptionalChildWrapper_OuterDomElement { get; set; } // This is used to remove the child (and its parent-specific wrapper) from the DOM.
        public dynamic INTERNAL_OptionalChildWrapper_ChildWrapperInnerDomElement { get; set; } // This is used to place grand-children inside.
    }

}
