
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
