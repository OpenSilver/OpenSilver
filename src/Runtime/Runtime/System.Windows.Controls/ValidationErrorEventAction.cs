
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Describes the reason a System.Windows.FrameworkElement.BindingValidationError
    /// event has occurred.
    /// </summary>
    public enum ValidationErrorEventAction
    {
        /// <summary>
        /// A new System.Windows.Controls.ValidationError has occurred.
        /// </summary>
        Added = 0,

        /// <summary>
        /// An existing System.Windows.Controls.ValidationError has been removed.
        /// </summary>
        Removed = 1,
    }
}

