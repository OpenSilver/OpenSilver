
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
    /// Describes how the data propagates in a binding.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Updates the target property when the binding is created. Changes to the source object can also propagate to the target.
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Updates the target property when the binding is created.
        /// </summary>
        OneTime = 2,
        /// <summary>
        /// Updates either the target or the source object when either changes. When the binding is created, the target property is updated from the source.
        /// </summary>
        TwoWay = 3,

    }
}