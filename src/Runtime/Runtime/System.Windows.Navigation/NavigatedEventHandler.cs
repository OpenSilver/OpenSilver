
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

namespace System.Windows.Navigation
{
    /// <summary>
    /// Represents the method that will handle the <c>NavigationService.Navigated</c>
    /// event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The data for the event.</param>
    public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);
}