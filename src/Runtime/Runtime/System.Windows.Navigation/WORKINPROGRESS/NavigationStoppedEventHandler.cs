

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
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.Navigation.NavigationService.NavigationStopped
    //     event.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     The data for the event.
    public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);
}
