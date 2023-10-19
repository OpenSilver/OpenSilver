
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

namespace System.Windows
{
    public abstract partial class Expression
    {
        internal Expression() { }

        /// <summary>
        ///     Called to evaluate the Expression value
        /// </summary>
        /// <param name="d">DependencyObject being queried</param>
        /// <param name="dp">Property being queried</param>
        /// <returns>Computed value. Unset if unavailable.</returns>
        internal abstract object GetValue(DependencyObject d, DependencyProperty dp);

        /// <summary>
        ///     Check if this Expression can set values
        /// </summary>
        /// <param name="d">DependencyObject being set</param>
        /// <param name="dp">Property being set</param>
        /// <returns></returns>
        internal abstract bool CanSetValue(DependencyObject d, DependencyProperty dp);

        /// <summary>
        ///     Notification that the Expression has been set as a property's value
        /// </summary>
        /// <param name="d">DependencyObject being set</param>
        /// <param name="dp">Property being set</param>
        internal abstract void OnAttach(DependencyObject d, DependencyProperty dp);

        /// <summary>
        ///     Notification that the Expression has been removed as a property's value
        /// </summary>
        /// <param name="d">DependencyObject being cleared</param>
        /// <param name="dp">Property being cleared</param>
        internal abstract void OnDetach(DependencyObject d, DependencyProperty dp);

        internal bool IsAttached 
        { 
            get; 
            set; 
        }
    }
}
