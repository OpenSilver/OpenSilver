

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