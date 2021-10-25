//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Event arguments used to describe browser navigation events.
    /// </summary>
    public sealed class BrowserNavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uri">The current browser location URI value.</param>
        /// <param name="statePairs">A collection of name-value pairs in the Uri Fragment.</param>
        internal BrowserNavigationEventArgs(Uri uri, IDictionary<string, string> statePairs)
        {
            Guard.ArgumentNotNull(uri, "uri");
            Guard.ArgumentNotNull(statePairs, "statePairs");

            this.Uri = uri;
            this.UriFragmentStatePairs = statePairs;
        }

        /// <summary>
        /// Gets the current browser location URI value.
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection of name-value pairs in the Uri Fragment.
        /// </summary>
        public IDictionary<string, string> UriFragmentStatePairs
        {
            get;
            private set;
        }
    }
}
