

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    // Summary:
    //     Represents the method that will handle events raised when a System.Windows.DependencyProperty
    //     is changed on a particular System.Windows.DependencyObject implementation.
    //
    // Parameters:
    //   sender:
    //     The source of the event (typically the object where the property changed).
    //
    //   e:
    //     The event data.
    public delegate void DependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);
}