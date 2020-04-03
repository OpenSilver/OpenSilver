

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
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            // Create the BindingExpression from the Binding:
            BindingExpression newBindingExpression = new BindingExpression(binding, target, dp);

            // Apply the BindingExpression:
            target.ApplyBindingExpression(dp, newBindingExpression);

            // Return the newly created BindingExpression:
            return newBindingExpression;
        }
    }
}
