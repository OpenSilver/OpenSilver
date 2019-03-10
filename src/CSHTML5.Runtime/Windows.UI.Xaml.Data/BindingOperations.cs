
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
