
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


//extern alias custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// Provides data for the System.Net.WebClient.DownloadStringCompleted event.
    /// </summary>
    internal class INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs// : AsyncCompletedEventArgs //todo: AsyncCompletedEventArgs() is obsolete, see what we should do (it might be that it is only obsolete for use by the user, but not for us)
    {
        public INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs() { }
        internal INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs(System.Net.INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
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
