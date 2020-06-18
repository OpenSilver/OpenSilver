
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



//extern alias custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OPENSILVER
using System.Net;
#endif

#if OPENSILVER
namespace OpenSilver.Compatibility
#else
namespace System.Net
#endif
{
    /// <summary>
    /// Provides data for the System.Net.WebClient.DownloadStringCompleted event.
    /// </summary>
    public partial class DownloadStringCompletedEventArgs// : AsyncCompletedEventArgs //todo: AsyncCompletedEventArgs() is obsolete, see what we should do (it might be that it is only obsolete for use by the user, but not for us)
    {
        public DownloadStringCompletedEventArgs() { }
        internal DownloadStringCompletedEventArgs(System.Net.DownloadStringCompletedEventArgs e)
        {
            Result = e.Result;
            UserState = e.UserState;
            Error = e.Error;
            Cancelled = e.Cancelled;
        }

        internal DownloadStringCompletedEventArgs(INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            Result = e.Result;
            UserState = e.UserState;
            Error = e.Error;
            Cancelled = e.Cancelled;
        }

        /// <summary>
        /// Gets the data that is downloaded by a Overload:System.Net.WebClient.DownloadStringAsync
        /// method.
        /// </summary>
        public string Result { get; set; }

        //below: should be in AsyncCompletedEventArgs but since we don't want to implement it, it's here.
        
        /// <summary>
        /// Gets the unique identifier for the asynchronous task.
        /// </summary>
        public object UserState { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating whether an asynchronous operation has been canceled.
        /// </summary>
        public bool Cancelled { get; internal set; }
        
        /// <summary>
        /// Gets a value indicating which error occurred during an asynchronous operation.
        /// </summary>
        public Exception Error { get; set; }
    }
}
