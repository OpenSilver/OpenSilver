//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

#if  MIGRATION
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
