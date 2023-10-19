
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

namespace System.Windows.Data
{
    /// <summary>
    /// Defines constants that describe the location of the binding source relative to the position of the binding target.
    /// </summary>
    public enum RelativeSourceMode
    {
        /// <summary>
        /// Don't use this value of RelativeSourceMode; always use either Self or TemplatedParent.
        /// </summary>
        None = 0,

        /// <summary>
        /// Refers to the element to which the template (in which the data-bound element
        /// exists) is applied. This is similar to setting a TemplateBinding Markup Extension
        /// and is only applicable if the Binding is within a template.
        /// </summary>
        TemplatedParent = 1,

        /// <summary>
        /// Refers to the element on which you are setting the binding and allows you
        /// to bind one property of that element to another property on the same element.
        /// </summary>
        Self = 2,
      
        /// <summary>
        /// Refers to the ancestor in the parent chain of the data-bound element. You
        /// can use this to bind to an ancestor of a specific type or its subclasses.
        /// This is the mode you use if you want to specify System.Windows.Data.RelativeSource.AncestorType
        /// and/or System.Windows.Data.RelativeSource.AncestorLevel.
        /// </summary>
        FindAncestor = 3,
    }
}