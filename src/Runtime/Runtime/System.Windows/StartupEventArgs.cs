﻿

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
using System.Security;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains the event data for the Application.Startup event.
    /// </summary>
    public sealed partial class StartupEventArgs : EventArgs
    {
#if WORKINPROGRESS
        #region Not supported yet
        // Summary:
        //     Gets the initialization parameters that were passed as part of HTML initialization
        //     of a Silverlight plug-in.
        //
        // Returns:
        //     The set of initialization parameters, as a dictionary with key strings and
        //     value strings.
        [OpenSilver.NotImplemented]
        public IDictionary<string, string> InitParams
        {
            get { return new Dictionary<string, string>(); }
        }
        #endregion
#endif
    }
}
