

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

namespace System.Windows
{
    /// <summary>
    /// Represents the method that will handle the Application.Startup event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void StartupEventHandler(object sender, StartupEventArgs e);
}
