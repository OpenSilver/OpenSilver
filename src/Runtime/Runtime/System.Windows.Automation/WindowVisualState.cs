
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
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Contains values that specify the visual state of a window for the <see cref="IWindowProvider" /> pattern.
    /// </summary>
    public enum WindowVisualState
    {
        /// <summary>
        /// Specifies that the window is normal (restored).
        /// </summary>
        Normal,
        /// <summary>
        /// Specifies that the window is maximized.
        /// </summary>
        Maximized,
        /// <summary>
        /// Specifies that the window is minimized.
        /// </summary>
        Minimized,
    }
}
