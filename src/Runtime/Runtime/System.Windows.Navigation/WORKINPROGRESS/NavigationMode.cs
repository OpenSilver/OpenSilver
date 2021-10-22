

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

using System.Runtime.CompilerServices;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Specifies the type of navigation that is occurring.
    //[TypeForwardedFrom("System.Windows.Controls.Navigation, Version=2.0.5.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35")]
    public enum NavigationMode : byte
    {
        //
        // Summary:
        //     Navigating to new content. This value is used when the System.Windows.Navigation.NavigationService.Navigate(System.Uri)
        //     method is called or when the System.Windows.Navigation.NavigationService.Source
        //     property is set. It is also used for any navigation request that the user initiates
        //     from the Web browser (including the back or forward buttons in a Web browser).
        New = 0,
        //
        // Summary:
        //     Navigating to the most recent content in the back navigation history. This value
        //     is used when the System.Windows.Navigation.NavigationService.GoBack method is
        //     called.
        Back = 1,
        //
        // Summary:
        //     Navigating to the most recent content in the forward navigation history. This
        //     value is used when the System.Windows.Navigation.NavigationService.GoForward
        //     method is called.
        Forward = 2,
        //
        // Summary:
        //     Reloading the current content. This value is used when the System.Windows.Navigation.NavigationService.Refresh
        //     method is called.
        Refresh = 3
    }
}
