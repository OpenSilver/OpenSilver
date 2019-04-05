
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



#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
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