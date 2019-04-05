
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



using System;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Provides the static BindingOperations.SetBinding(DependencyObject,DependencyProperty,BindingBase) method.
    /// </summary>
    public static class BindingOperations
    {
        /// <summary>
        /// Creates and associates a new BindingExpression with the specified binding target property.
        /// </summary>
        /// <param name="target">The target to set the binding to.</param>
        /// <param name="dp">The property on the target to bind.</param>
        /// <param name="binding">The binding to assign to the target property.</param>
        /// <returns>An object that contains information about the binding.</returns>
        public static BindingExpression SetBinding(DependencyObject target, DependencyProperty dp, Binding binding)
        {
            //todo: the signature of this method is slightly different: it takes a "Binding" instead of "BindingBase", and it returns a "BindingExpression" instead of a "BindingExpressionBase".

            return target.SetBinding(dp, binding);
        }
    }
}
