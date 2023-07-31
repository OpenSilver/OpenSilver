
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

namespace CSHTML5.Internal
{
    internal sealed class INTERNAL_VisualChildInformation
    {
        // Understanding the concept:
        // For some layouts like vertical stackpanels (implemented using table-like nested divs), the parent needs to create
        // one row for each child. Therefore, it needs to "wrap" the child into some DOM nodes that we call "ChildWrapper".
        // Each of those wrappers has an "outer" element that lets us remove the wrapper from the parent, and an "inner"
        // element that is the place where the real child element will be placed (in the previous example, the "inner" element is
        // a table cell).
        // Note: for structures that don't require the creation of "wrappers" around their children, those two fields must be left blank.

        public object INTERNAL_OptionalChildWrapper_OuterDomElement { get; set; }
    }
}
