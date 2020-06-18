

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.Controls.WebBrowser.LoadCompleted
    //     event.
    //
    // Parameters:
    //   sender:
    //     The source of the event.
    //
    //   e:
    //     The event data.
    public delegate void LoadCompletedEventHandler(object sender, NavigationEventArgs e);
}

#endif
