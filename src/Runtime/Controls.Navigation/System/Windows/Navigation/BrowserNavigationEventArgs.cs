

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
