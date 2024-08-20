
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

namespace System.Windows.Data;

/// <summary>
/// Provides static methods to manipulate bindings, including <see cref="Binding"/>,
/// <see cref="MultiBinding"/> objects.
/// </summary>
public static class BindingOperations
{
    /// <summary>
    /// Creates and associates a new instance of <see cref="BindingExpressionBase"/>
    /// with the specified binding target property.
    /// </summary>
    /// <param name="target">
    /// The binding target of the binding.
    /// </param>
    /// <param name="dp">
    /// The target property of the binding.
    /// </param>
    /// <param name="binding">
    /// The <see cref="BindingBase"/> object that describes the binding.
    /// </param>
    /// <returns>
    /// The instance of <see cref="BindingExpressionBase"/> created for and associated
    /// with the specified property. The <see cref="BindingExpressionBase"/> class is 
    /// the base class of <see cref="BindingExpression"/> and <see cref="MultiBindingExpression"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The binding parameter cannot be null.
    /// </exception>
    public static BindingExpressionBase SetBinding(DependencyObject target, DependencyProperty dp, BindingBase binding)
    {
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

        var expr = binding.CreateBindingExpression(target, dp, null);

        expr.IsInTransfer = true;
        try
        {
            target.SetValue(dp, expr);
        }
        finally
        {
            expr.IsInTransfer = false;
        }

        return expr;
    }

    /// <summary>
    /// Creates and associates a new <see cref="BindingExpression"/> with the specified binding target property.
    /// </summary>
    /// <param name="target">
    /// The binding target of the binding.
    /// </param>
    /// <param name="dp">
    /// The target property of the binding.
    /// </param>
    /// <param name="binding">
    /// The <see cref="Binding"/> object that describes the binding.
    /// </param>
    /// <returns>
    /// The instance of <see cref="BindingExpression"/> created for and associated with the specified property.
    /// </returns>
    public static BindingExpression SetBinding(DependencyObject target, DependencyProperty dp, Binding binding)
        => (BindingExpression)SetBinding(target, dp, (BindingBase)binding);

    /// <summary>
    /// Returns the <see cref="BindingExpression"/> object associated with the specified binding 
    /// target property on the specified object.
    /// </summary>
    /// <param name="target">
    /// The binding target object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="BindingExpression"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="BindingExpression"/> object associated with the given property or null if 
    /// none exists.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
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

    /// <summary>
    /// Returns the <see cref="MultiBindingExpression"/> object associated with the specified binding 
    /// target property on the specified object.
    /// </summary>
    /// <param name="target">
    /// The binding target object where dp is.
    /// </param>
    /// <param name="dp">
    /// The binding target property from which to retrieve the <see cref="MultiBindingExpression"/> object.
    /// </param>
    /// <returns>
    /// The <see cref="MultiBindingExpression"/> object associated with the given property or null if none 
    /// exists.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The target and dp parameters cannot be null.
    /// </exception>
    public static MultiBindingExpression GetMultiBindingExpression(DependencyObject target, DependencyProperty dp)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }
        if (dp is null)
        {
            throw new ArgumentNullException(nameof(dp));
        }

        return target.ReadLocalValue(dp) as MultiBindingExpression;
    }
}
