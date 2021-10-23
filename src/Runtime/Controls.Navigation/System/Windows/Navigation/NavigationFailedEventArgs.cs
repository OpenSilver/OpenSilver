

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

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Event arguments used by the <see cref="System.Windows.Controls.Frame.NavigationFailed"/> and <see cref="NavigationService.NavigationFailed"/> events.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public sealed class NavigationFailedEventArgs : EventArgs
    {
        #region All Constructors

        /// <summary>
        /// Constructs a set of event arguments
        /// </summary>
        /// <param name="uri">The Uri to which navigation failed</param>
        /// <param name="error">The error that occurred</param>
        internal NavigationFailedEventArgs(Uri uri, Exception error)
        {
            this.Uri = uri;
            this.Exception = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Uri that failed to be navigated to
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the error that caused navigation to fail
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this failure event has been handled.
        /// If this is false after NavigationFailed completes, the exception will
        /// be thrown.
        /// </summary>
        public bool Handled { get; set; }

        #endregion
    }
}
