
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
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes a method to support UI automation access to controls that initiate or perform 
    /// a single, unambiguous action and do not maintain state when activated. 
    /// </summary>
    public interface IInvokeProvider
    {
        /// <summary>
        /// Sends a request to activate a control and initiate its single, unambiguous action.
        /// </summary>
        void Invoke();
    }
}
