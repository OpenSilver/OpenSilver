
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

using System;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Provides the static <see cref="SetBinding(DependencyObject, DependencyProperty, Binding)" />
    /// method.
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
            // the signature of this method is slightly different: it takes a "Binding" instead of "BindingBase", 
            // and it returns a "BindingExpression" instead of a "BindingExpressionBase".
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }
            if (binding is null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            // Create the BindingExpression from the Binding
            var expr = new BindingExpression(binding, dp)
            {
                IsUpdatingValue = true
            };

            try
            {
                target.SetValue(dp, expr);
            }
            finally
            {
                expr.IsUpdatingValue = false;
            }

            // Return the newly created BindingExpression
            return expr;
        }

        public static BindingExpression GetBindingExpression(DependencyObject target, DependencyProperty dp)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            return target.ReadLocalValue(dp) as BindingExpression;
        }
    }
}
