
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
    /// Defines constants that indicate when a binding source is updated by its binding
    /// target in two-way binding.
    /// </summary>
    public enum UpdateSourceTrigger
    {
        /// <summary>
        /// The binding source is updated automatically when the binding target value changes.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The binding source is updated whenever the binding target value changes. If the
        /// binding target is a <see cref="Controls.TextBox"/>, it does not have to lose
        /// focus for the changes to be detected.
        /// </summary>
        PropertyChanged = 1,
        /// <summary>
        /// The binding source is updated only when you call the <see cref="BindingExpression.UpdateSource"/>
        /// method.
        /// </summary>
        Explicit = 3
    }
}