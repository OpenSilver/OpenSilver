

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


#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Event arguments used to describe journaling events.
    /// </summary>
    internal sealed class JournalEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">A value representing a journal event name.</param>
        /// <param name="uri">A value representing a journal event URI.</param>
        /// <param name="mode">A value representing a journal event navigation mode.</param>
        internal JournalEventArgs(string name, Uri uri, NavigationMode mode)
        {
            Guard.ArgumentNotNull(uri, "uri");

            this.Name = name;
            this.Uri = uri;
            this.NavigationMode = mode;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value containing the journal event name.
        /// </summary>
        internal string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value containing the journal event URI.
        /// </summary>
        internal Uri Uri
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value representing the journal event navigation mode.
        /// </summary>
        internal NavigationMode NavigationMode
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
