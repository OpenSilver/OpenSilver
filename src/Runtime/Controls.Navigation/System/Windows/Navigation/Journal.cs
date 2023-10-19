//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Browser;
using System.Windows.Interop;
using OpenSilver.Internal.Navigation;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Journal used to manage a history list of JournalEntry items.
    /// </summary>
    internal class Journal
    {
#region Fields

        /// <summary>
        /// Synchronization lock object.
        /// </summary>
        private readonly object _syncLock = new object();

        private JournalEntry _currentEntry;

        private Stack<JournalEntry> _forwardStack = new Stack<JournalEntry>();

        private Stack<JournalEntry> _backStack = new Stack<JournalEntry>();

        /// <summary>
        /// Used to indicate whether or not to suppress navigation events.
        /// </summary>
        /// <remarks>
        /// This is used internally to avoid redundant browser navigation calls after deep link values are detected.
        /// </remarks>
        private bool _suppressNavigationEvent;

        /// <summary>
        /// Internal event handler reference used to sign up to the SilverlightHost.NavigationStateChanged event.
        /// </summary>
        /// <remarks>
        /// The event handler constructed here will use a weak reference to self in order to allow for this instance to be collected.
        /// </remarks>
        private EventHandler<NavigationStateChangedEventArgs> _weakRefEventHandler;
        
#endregion Fields

#region Constructors & Destructor

        internal Journal(bool useNavigationState)
        {
            this.UseNavigationState = useNavigationState;

            if (useNavigationState)
            {
                this.InitializeNavigationState();
            }
        }

        // 



        ~Journal()
        {
            if (this._weakRefEventHandler != null)
            {
                // This must be BeginInvoke'd because Application.Current.Host is accessible only on the UI thread
                Deployment.Current.Dispatcher.BeginInvoke(() => Application.Current.Host.NavigationStateChanged -= this._weakRefEventHandler);
            }
        }

#endregion

#region Events

        internal event EventHandler<JournalEventArgs> Navigated;

#endregion Events

#region Properties

        /// <summary>
        /// Gets a value indicating whether or not this journal instance is
        /// using the NavigationState property of the Application class.
        /// </summary>
        internal bool UseNavigationState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether or not the Journal instance
        /// can navigate backward.
        /// </summary>
        internal bool CanGoBack
        {
            get { return this._backStack.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Journal instance
        /// can navigate forward.
        /// </summary>
        internal bool CanGoForward
        {
            get { return (this._forwardStack.Count > 0); }
        }

        /// <summary>
        /// Gets the current JournalEntry or null if no history items exist.
        /// </summary>
        internal JournalEntry CurrentEntry
        {
            get
            {
                return this._currentEntry;
            }
        }

        /// <summary>
        /// Gets a stack of back entries in this journal
        /// </summary>
        internal Stack<JournalEntry> BackStack
        {
            get { return this._backStack; }
        }

        /// <summary>
        /// Gets a stack of forward entries in this journal
        /// </summary>
        internal Stack<JournalEntry> ForwardStack
        {
            get { return this._forwardStack; }
        }

#endregion Properties

#region Methods

        /// <summary>
        /// Adds a new JournalEntry to the history stack.
        /// </summary>
        /// <param name="journalEntry">A new JournalEntry to add to the history stack.</param>
        /// <remarks>
        /// Any JournalEntry items existing on the ForwardStack will be removed.
        /// </remarks>
        internal void AddHistoryPoint(JournalEntry journalEntry)
        {
            Guard.ArgumentNotNull(journalEntry, "journalEntry");

            lock (this._syncLock)
            {
                this._forwardStack.Clear();

                if (this._currentEntry != null)
                {
                    this._backStack.Push(this._currentEntry);
                }

                this._currentEntry = journalEntry;
            }

            this.UpdateObservables(journalEntry, NavigationMode.New);

            this.UpdateNavigationState(this.CurrentEntry);
        }

        /// <summary>
        /// Forces the Journal to check for deep-link values in 
        /// the browser address URI.
        /// </summary>
        /// <returns>
        /// A Boolean indicating whether or not a deep-link value was found.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deeplinks", Justification = "This is the correct spelling.")]
        internal bool CheckForDeeplinks()
        {
            if (this.UseNavigationState)
            {
                string currentState = UriParsingHelper.InternalUriFromExternalValue(Application.Current.Host.NavigationState);
                if (!String.IsNullOrEmpty(currentState))
                {
                    this.AddHistoryPointIfDifferent(currentState);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Navigates the Journal instance back to the previous 
        /// JournalEntry item in the history stack.
        /// </summary>
        /// <remarks>
        /// If CanGoBack is false, this method will throw an InvalidOperationException.
        /// </remarks>
        internal void GoBack()
        {
            if (this.CanGoBack == false)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                                  Resource.CannotGoBack,
                                  "CanGoBack"));
            }

            lock (this._syncLock)
            {
                this._forwardStack.Push(this._currentEntry);
                this._currentEntry = this._backStack.Pop();
            }

            this.UpdateObservables(this._currentEntry, NavigationMode.Back);

            this.UpdateNavigationState(this.CurrentEntry);
        }

        /// <summary>
        /// Navigates the Journal instance forward to the next 
        /// JournalEntry item in the history stack.
        /// </summary>
        /// <remarks>
        /// If CanGoForward is false, this method will throw an InvalidOperationException.
        /// </remarks>
        internal void GoForward()
        {
            if (this.CanGoForward == false)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                                  Resource.CannotGoForward,
                                  "CanGoForward"));
            }

            lock (this._syncLock)
            {
                this._backStack.Push(this._currentEntry);
                this._currentEntry = this._forwardStack.Pop();
            }

            this.UpdateObservables(this._currentEntry, NavigationMode.Forward);

            this.UpdateNavigationState(this.CurrentEntry);
        }

        /// <summary>
        /// Updates NavigationState to reflect the Journal state, if the Journal is using NavigationState.
        /// </summary>
        /// <param name="journalEntry">JournalEntry used to update the browser location.</param>
        private void UpdateNavigationState(JournalEntry journalEntry)
        {
            if (this.UseNavigationState)
            {
                if (this._suppressNavigationEvent == false)
                {
                    string state = journalEntry.Source == null
                                    ? string.Empty
                                    : UriParsingHelper.InternalUriToExternalValue(journalEntry.Source);

                    // Title updates only occur when DOM access is enabled, so check this first.
                    if (HtmlPage.IsEnabled)
                    {
                        // In older versions of IE (6, 7, and 8 in 7 compat mode) we use an
                        // iframe to cause journal updates.  But this requires that the title
                        // be set before the navigation for the dropdowns for back/forward
                        // to show the correct titles at the correct places in these lists.
                        //
                        // In newer versions of IE, and in all other supported browsers, the
                        // title should be set after the navigation for correct behavior.
                        if (UsingIFrame())
                        {
                            HtmlPage.Document.SetProperty("title", journalEntry.Name);
                            Application.Current.Host.NavigationState = state;
                        }
                        else
                        {
                            Application.Current.Host.NavigationState = state;
                            HtmlPage.Document.SetProperty("title", journalEntry.Name);
                        }
                    }
                    else
                    {
                        // We don't have DOM access, so just update NavigationState
                        // without a title update
                        Application.Current.Host.NavigationState = state;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if we're running in a version of IE that uses an iframe for NavigationState
        /// </summary>
        /// <remarks>
        /// This method should only be called when HtmlPage.IsEnabled is true.  It does
        /// not verify this, but will throw if that property is false.
        /// </remarks>
        /// <returns>True for IE6, IE7, and IE8 in 7 compat mode.  False otherwise</returns>
        private static bool UsingIFrame()
        {
            string userAgent = HtmlPage.BrowserInformation.UserAgent;

            if (userAgent.Contains("MSIE 6.0") ||
                userAgent.Contains("MSIE 7.0"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Occurs when the browser has navigated (usually due to the user hitting Back or Forward in the browser's UI).
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Empty event args.</param>
        private void Browser_Navigated(object sender, EventArgs eventArgs)
        {
            if (this.UseNavigationState)
            {
                this.AddHistoryPointIfDifferent(UriParsingHelper.InternalUriFromExternalValue(Application.Current.Host.NavigationState));
            }
        }

        /// <summary>
        /// Conditionally adds a new history point if the new state information differs from the current journal entry Uri value.
        /// </summary>
        /// <param name="newState">An updated state value to examine.</param>
        private void AddHistoryPointIfDifferent(string newState)
        {
            // Check if different from our current state
            string currentState = String.Empty;
            if (this.CurrentEntry != null && this.CurrentEntry.Source != null)
            {
                currentState = UriParsingHelper.InternalUriFromExternalValue(this.CurrentEntry.Source.OriginalString);
            }

            if (string.Equals(newState, currentState, StringComparison.Ordinal) == false)
            {
                this._suppressNavigationEvent = true;
                this.AddHistoryPoint(new JournalEntry(string.Empty, new Uri(newState, UriKind.RelativeOrAbsolute)));
                this._suppressNavigationEvent = false;
            }
        }

        /// <summary>
        /// Signs up for the Application.NavigationStateChanged event using a weak-reference based event handler.
        /// </summary>
        private void InitializeNavigationState()
        {
            WeakReference thisWeak = new WeakReference(this);
            this._weakRefEventHandler =
                (sender, args) =>
                {
                    var journal = thisWeak.Target as Journal;
                    if (journal != null)
                    {
                        journal.Browser_Navigated(sender, args);
                    }
                };

            try
            {
                // Signing up for this event can throw an exception if enableNavigation = "none" on the object tag.
                // There is currently no way to detect this from managed code (without accessing the DOM bridge), so
                // we wrap it in a try/catch, and upon exception, assume this means we cannot use NavigationState.
                Application.Current.Host.NavigationStateChanged += this._weakRefEventHandler;
            }
            catch (InvalidOperationException)
            {
                this.UseNavigationState = false;
            }
        }

        /// <summary>
        /// Raises the Navigated event.
        /// </summary>
        /// <param name="name">A value representing a journal entry name.</param>
        /// <param name="uri">A value representing a journal entry URI.</param>
        /// <param name="mode">A value representing a journal entry navigation mode.</param>
        protected void OnNavigated(string name, Uri uri, NavigationMode mode)
        {
            EventHandler<JournalEventArgs> eventHandler = this.Navigated;
            if (eventHandler != null)
            {
                JournalEventArgs args = new JournalEventArgs(name, uri, mode);
                eventHandler(this, args);
            }
        }

        /// <summary>
        /// Updates observable properties of the journal.
        /// </summary>
        /// <param name="currentEntry">The current journal entry.</param>
        /// <param name="mode">The mode of navigation that triggered the update.</param>
        private void UpdateObservables(JournalEntry currentEntry, NavigationMode mode)
        {
            this.OnNavigated(currentEntry.Name, currentEntry.Source, mode);
        }

#endregion Methods
    }
}
