//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Event arguments used by the <see cref="Frame.NavigationFailed"/> and <see cref="NavigationService.NavigationFailed"/> events.
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
