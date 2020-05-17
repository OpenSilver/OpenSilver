
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
    // Summary:
    //     
    /// <summary>
    /// Provides data for the System.Net.WebClient.UploadStringCompleted event.
    /// </summary>
    public partial class UploadStringCompletedEventArgs //: AsyncCompletedEventArgs //todo: AsyncCompletedEventArgs() is obsolete, see what we should do (it might be that it is only obsolete for use by the user, but not for us)
    {
        
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.AsyncCompletedEventArgs
        /// class.
        /// </summary>
        public UploadStringCompletedEventArgs() { }
        
        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.AsyncCompletedEventArgs
        /// class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled">A value indicating whether the asynchronous operation was canceled.</param>
        /// <param name="userState">
        /// The optional user-supplied state object passed to the System.ComponentModel.BackgroundWorker.RunWorkerAsync(System.Object)
        /// method.
        /// </param>
        public UploadStringCompletedEventArgs(Exception error, bool cancelled, object userState)
        {
            Error = error;
            Cancelled = cancelled;
            UserState = userState;
        }
        internal UploadStringCompletedEventArgs(System.Net.DownloadStringCompletedEventArgs e) //it is normal that the parameter is of type DownloadStringCompletedEventArgs instead of UploadStringCompletedEventArgs.
        {
            Result = e.Result;
            UserState = e.UserState;
            Error = e.Error;
            Cancelled = e.Cancelled;
        }
        internal UploadStringCompletedEventArgs(INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            Result = e.Result;
            UserState = e.UserState;
            Error = e.Error;
            Cancelled = e.Cancelled;
        }

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation has been canceled.
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// Gets a value indicating which error occurred during an asynchronous operation.
        /// </summary>
        public Exception Error { get; private set; }
        
        // Returns:
        //     An object reference that uniquely identifies the asynchronous task; otherwise,
        //     null if no value has been set.
        /// <summary>
        /// Gets the unique identifier for the asynchronous task.
        /// </summary>
        public object UserState { get; private set; }

        /// <summary>
        /// Gets the server reply to a string upload operation that is started by calling
        /// an Overload:System.Net.WebClient.UploadStringAsync method.
        /// </summary>
        public string Result { get; internal set; }
    }
}