
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
    /// This enum describes when updates (target-to-source data flow)
    /// happen in a given Binding.
    /// </summary>
    public enum UpdateSourceTrigger
    {
        /// <summary>
        /// Obtain trigger from target property default
        /// </summary>
        Default,

        /// <summary>
        /// Update whenever the target property changes
        /// </summary>
        PropertyChanged,

        //// <summary>
        //// Update only when target element loses focus, or when Binding deactivates
        //// </summary>
        //LostFocus,

        /// <summary>
        /// Update only by explicit call to BindingExpression.UpdateSource()
        /// </summary>
        Explicit
    }
}