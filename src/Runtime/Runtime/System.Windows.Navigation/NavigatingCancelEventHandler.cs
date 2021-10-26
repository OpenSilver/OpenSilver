
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
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Represents the method that will handle the <c>NavigationService.Navigating</c>
    /// event, which is a cancelable event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The data for the event.</param>
    public delegate void NavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e);
}
