
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
    public abstract class BindingExpressionBase : Expression
    {
        internal BindingExpressionBase() { }

        /// <summary>
        /// NoTarget DependencyProperty, a placeholder used by BindingExpressions with no target property
        /// </summary>
        internal static readonly DependencyProperty NoTargetProperty =
            DependencyProperty.RegisterAttached(
                "NoTarget",
                typeof(object),
                typeof(BindingExpressionBase),
                null);
    }
}
