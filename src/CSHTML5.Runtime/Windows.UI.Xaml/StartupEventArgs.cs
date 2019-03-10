
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


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
    public sealed class StartupEventArgs : EventArgs
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
        public IDictionary<string, string> InitParams
        {
            get { return new Dictionary<string, string>(); }
        }
        #endregion
#endif
    }
}
