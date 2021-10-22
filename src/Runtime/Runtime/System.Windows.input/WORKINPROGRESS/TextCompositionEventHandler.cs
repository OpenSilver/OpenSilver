

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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.UIElement.TextInput
    //     routed event.
    //
    // Parameters:
    //   sender:
    //     The object where the event handler is attached.
    //
    //   e:
    //     Event data for the event.
    public delegate void TextCompositionEventHandler(object sender, TextCompositionEventArgs e);
}
