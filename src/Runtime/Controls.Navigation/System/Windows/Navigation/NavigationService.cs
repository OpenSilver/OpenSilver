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
using System.Globalization;
using System.Windows.Browser;
using OpenSilver.Internal.Navigation;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Used to programmatically initiate navigation, primarily from within a <see cref="Page"/>.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public sealed class NavigationService
    {
#region Fields

        private bool _journalIsAddingHistoryPoint;
        private Frame _host;
        private Uri _currentSource;
        private Uri _currentSourceAfterMapping;
        private Uri _source;
        private Journal _journal;
        private INavigationContentLoader _contentLoader;
        private NavigationOperation _currentNavigation;
        private Dictionary<string, Page> _cacheRequiredPages = new Dictionary<string, Page>();

#endregion

#region Constructors

        /// <summary>
        /// Internal class used to host content and handles all navigations
        /// </summary>
        /// <param name="nav">
        /// Parent navigator that uses and owns this NavigationService.
        /// </param>
        internal NavigationService(Frame nav)
        {
            Guard.ArgumentNotNull(nav, "nav");
            this._host = nav;
        }

#endregion Constructors

#region Events

        /// <summary>
        /// Occurs when the an exception is raised during navigation.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// Occurs when the NavigationService is starting to navigate.
        /// </summary>
        /// <value></value>
        public event NavigatingCancelEventHandler Navigating;

        /// <summary>
        /// Occurs when the NavigationService has navigated.
        /// </summary>
        /// <value></value>
        public event NavigatedEventHandler Navigated;

        /// <summary>
        /// Occurs when a navigation operation has been cancelled.
        /// </summary>
        /// <value></value>
        public event NavigationStoppedEventHandler NavigationStopped;

        /// <summary>
        /// Occurs when a navigation occurs within a page.
        /// </summary>
        public event FragmentNavigationEventHandler FragmentNavigation;

#endregion

#region NavigationService Attached Property

        /// <summary>
        /// Attached DependencyProperty. It gives an element the NavigationService of the navigation container it's in.
        /// </summary>
        internal static readonly DependencyProperty NavigationServiceProperty =
                DependencyProperty.RegisterAttached(
                        "NavigationService",
                        typeof(NavigationService),
                        typeof(NavigationService),
                        new PropertyMetadata(null));

        /// <summary>
        /// Gets NavigationService of the navigation container the given dependencyObject is in.
        /// </summary>
        /// <param name="dependencyObject">The object to retrieve the attached <see cref="NavigationService"/> for</param>
        /// <returns>The <see cref="NavigationService"/> attached to the <paramref name="dependencyObject"/></returns>
        internal static NavigationService GetNavigationService(DependencyObject dependencyObject)
        {
            Guard.ArgumentNotNull(dependencyObject, "dependencyObject");

            return dependencyObject.GetValue(NavigationServiceProperty) as NavigationService;
        }

#endregion

#region Properties

        internal Journal Journal
        {
            get { return this._journal; }
        }

        internal INavigationContentLoader ContentLoader
        {
            get { return this._contentLoader; }
            set { this._contentLoader = value; }
        }

        internal Frame Host
        {
            get { return this._host; }
        }

        internal bool IsNavigating
        {
            get { return this._currentNavigation != null; }
        }

        internal NavigationCache Cache
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the Uri of the content currently hosted in the Frame.
        /// </summary>
        /// <remarks>
        /// This value may be different from CurrentSource if Source is set and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri Source
        {
            get
            {
                return this._source;
            }

            set
            {
                this._source = value;
                this.Navigate(value);
            }
        }

        /// <summary>
        /// Gets the Uri of the content currently hosted in the Frame.
        /// </summary>
        /// <remarks>
        /// This value may be different from Source if Source is set and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri CurrentSource
        {
            get { return this._currentSource; }
            internal set { this._currentSource = value; }
        }

        /// <summary>
        /// Gets a value used to determine if there are any entries on the forward stack
        /// </summary>
        /// <value></value>
        public bool CanGoForward
        {
            get { return this._journal.CanGoForward; }
        }

        /// <summary>
        /// Gets a value used to determine if there are any entries on the back stack
        /// </summary>
        /// <value></value>
        public bool CanGoBack
        {
            get { return this._journal.CanGoBack; }
        }

#endregion

#region Methods

        internal void InitializeJournal()
        {
            Journal originalJournal = this._journal;

            //Find the outer frame (if there is one)
            Frame outerFrame = null;
            DependencyObject walker = VisualTreeHelper.GetParent(this.Host);

            // Walk up tree to find a parent navigator.
            while (walker != null)
            {
                outerFrame = walker as Frame;
                if (outerFrame != null)
                {
                    break;
                }

                walker = VisualTreeHelper.GetParent(walker);
            }

            // Whenever we're in design mode, we should avoid using NavigationState as it's intended to interact with the
            // browser, which is not available at design-time.  Thus, we should always be in OwnsJournal mode when in a designer.
            if (this.Host.JournalOwnership == JournalOwnership.OwnsJournal ||
                Frame.IsInDesignMode())
            {
                this._journal = new Journal(false /* useNavigationState */);
            }
            // If we're set to use the parent journal, one of the following is true:
            // 1) We're a top-level Frame (i.e. not nested within another Frame) - in this case, integrate with the browser, as it is logically our "parent"
            // 2) We're not a top-level Frame, in which case browser integration is not supported, so throw an exception.  If graceful fall-back is desired, they can use JournalOwnership.Automatic instead.
            else if (this.Host.JournalOwnership == JournalOwnership.UsesParentJournal &&
                outerFrame != null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                                  Resource.NavigationService_JournalOwnership_UsesParentJournal_OnlyTopLevel,
                                  "JournalOwnership",
                                  "UsesParentJournal",
                                  "Frame"));
            }
            // If we're set to automatic (the default), then one of the following is true:
            // 1) We're a top-level Frame (i.e. not nested within another Frame) - in this case, integrate with the browser, as it is logically our "parent"
            // 2) We're not a top-level Frame, in which case browser integration is not supported, so fall-back to an internal journal silently.
            else if (this.Host.JournalOwnership == JournalOwnership.Automatic &&
                outerFrame != null)
            {
                this._journal = new Journal(false /* useNavigationState */);
            }
            else
            {
                // If we get this far, then we must be integrating with the browser:
                this._journal = new Journal(true /* useNavigationState */);
            }

            if (this._journal != originalJournal)
            {
                if (originalJournal != null)
                {
                    originalJournal.Navigated -= this.Journal_Navigated;
                }
                this._journal.Navigated += this.Journal_Navigated;
            }
        }

        internal void InitializeNavigationCache()
        {
            this.Cache = new NavigationCache(this.Host.CacheSize);
        }

        /// <summary>
        /// Navigate to source
        /// </summary>
        /// <param name="source">The Uri to begin navigating to</param>
        /// <returns>Always returns true.</returns>
        public bool Navigate(Uri source)
        {
            return this.NavigateCore(source, NavigationMode.New, false/*suppressJournalAdd*/, false/*isRedirect*/);
        }

        private void Journal_Navigated(object sender, JournalEventArgs args)
        {
            if (this._journalIsAddingHistoryPoint == false)
            {
                NavigationOperation navOp = this._currentNavigation;
                if (navOp == null || navOp.SuppressNotifications == false)
                {
                    this.NavigateCore(args.Uri, args.NavigationMode, true/*suppressJournalAdd*/, false/*isRedirect*/);
                }
            }
        }

        private bool NavigateCore(Uri uri, NavigationMode mode, bool suppressJournalAdd, bool isRedirect)
        {
            try
            {
                if (uri == null)
                {
                    throw new ArgumentNullException("uri", Resource.NavigationService_NavigationToANullUriIsNotSupported);
                }

                // Make sure we're on the UI thread because of the DependencyProperties we use.
                if (!this.Host.Dispatcher.CheckAccess())
                {
                    // Move to UI thread
                    this.Host.Dispatcher.BeginInvoke(() => this.NavigateCore(uri, mode, suppressJournalAdd, isRedirect));
                    return true;
                }

                Uri mappedUri = uri;
                // If the Uri is only a fragment, mapping does not take place
                if (!UriParsingHelper.InternalUriIsFragment(uri))
                {
                    UriMapperBase mapper = this.Host.UriMapper;
                    if (mapper != null)
                    {
                        Uri uriFromMapper = mapper.MapUri(uri);
                        if (uriFromMapper != null && !String.IsNullOrEmpty(uriFromMapper.OriginalString))
                        {
                            mappedUri = uriFromMapper;
                        }
                        else
                        {
                            mappedUri = uri;
                        }
                    }
                }

                Uri mergedUriAfterMapping = UriParsingHelper.InternalUriMerge(this._currentSourceAfterMapping, mappedUri) ?? mappedUri;
                Uri mergedUri = UriParsingHelper.InternalUriMerge(this._currentSource, uri) ?? uri;

                // If we're navigating to just a fragment (i.e. "#frag1") or to a page which differs only in the fragment
                // (i.e. "Page.xaml?id=123" to "Page.xaml?id=123#frag1") then complete navigation without involving the content loader
                bool isFragmentNavigationOnly = (mode != NavigationMode.Refresh) &&
                                                    (UriParsingHelper.InternalUriIsFragment(mappedUri) ||
                                                     UriParsingHelper.InternalUriGetAllButFragment(mergedUri) == UriParsingHelper.InternalUriGetAllButFragment(this._currentSource));

                // Check to see if anyone wants to cancel
                if (mode == NavigationMode.New || mode == NavigationMode.Refresh)
                {
                    if (this.RaiseNavigating(mergedUri, mode, isFragmentNavigationOnly) == true)
                    {
                        // Someone stopped us
                        this.RaiseNavigationStopped(null, mergedUri);
                        return true;
                    }
                }

                // If the ContentLoader cannot load the new URI, throw an ArgumentException
                if (!this.ContentLoader.CanLoad(mappedUri, _currentSourceAfterMapping))
                {
                    throw new ArgumentException(Resource.NavigationService_CannotLoadUri, "uri");
                }

                if (isFragmentNavigationOnly && this.Host.Content == null)
                {
                    // It doesn't make sense to fragment navigate when there's no content, so raise NavigationFailed
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                                                                      Resource.NavigationService_FragmentNavigationRequiresContent,
                                                                      "Frame"));
                }

                if (isRedirect && this._currentNavigation != null && 
                    this._currentNavigation.UriForJournal == this._currentSource)
                {
                    // Do not record navigation in the journal in case of a redirection 
                    // where the original target is the current URI.
                    suppressJournalAdd = true;
                }

                // Stop in-progress navigation
                this.StopLoadingCore(isRedirect);

                return this.NavigateCore_StartNavigation(uri, mode, suppressJournalAdd, mergedUriAfterMapping, mergedUri, isFragmentNavigationOnly);
            }
            catch (Exception ex)
            {
                if (this.RaiseNavigationFailed(uri, ex))
                {
                    throw;
                }
                return true;
            }
        }

        private bool NavigateCore_StartNavigation(Uri uri, NavigationMode mode, bool suppressJournalAdd, Uri mergedUriAfterMapping, Uri mergedUri, bool isFragmentNavigationOnly)
        {
            this._currentNavigation = new NavigationOperation(mergedUriAfterMapping, mergedUri, uri, mode, suppressJournalAdd);

            if (isFragmentNavigationOnly)
            {
                // If we're navigating only to a fragment (e.g. "#frag2") then the Uri to journal should be that merged with the base uri
                if (UriParsingHelper.InternalUriIsFragment(uri))
                {
                    this._currentNavigation.UriForJournal = mergedUri;
                }
                this.Host.Dispatcher.BeginInvoke(() => this.CompleteNavigation(null));
                return true;
            }

            this.UpdateNavigationCacheModeAlwaysPages();

            string uriAllButFragment = UriParsingHelper.InternalUriGetAllButFragment(uri);
            Page reusedPage = null;

            if (this._cacheRequiredPages.ContainsKey(uriAllButFragment))
            {
                reusedPage = this._cacheRequiredPages[uriAllButFragment];
            }
            else if (this.Cache.Contains(uriAllButFragment))
            {
                reusedPage = this.Cache[uriAllButFragment];
            }

            // If a page was found in either cache and that page hasn't yet changed its NavigationCacheMode to Disabled,
            // then navigation is done, otherwise open up new content
            if (reusedPage != null && reusedPage.NavigationCacheMode != NavigationCacheMode.Disabled)
            {
                this.Host.Dispatcher.BeginInvoke(() => this.CompleteNavigation(reusedPage));
            }
            else
            {
                this._currentNavigation.AsyncResult = this._contentLoader.BeginLoad(mergedUriAfterMapping, this._currentSourceAfterMapping, this.ContentLoader_BeginLoad_Callback, this._currentNavigation);
            }

            return true;
        }

        private void ContentLoader_BeginLoad_Callback(IAsyncResult result)
        {
            DependencyObject content = null;
            Uri uriBeingLoaded = null;

            try
            {
                NavigationOperation asyncNavigationOperationCompleted = result.AsyncState as NavigationOperation;

                NavigationOperation navOp = this._currentNavigation;
                if (navOp == null || navOp.Uri != asyncNavigationOperationCompleted.Uri)
                {
                    // We already fired NavigationStopped in NavigateCore(), so just return without doing anything
                    return;
                }

                uriBeingLoaded = navOp.UriBeforeMapping;

                LoadResult loadResult = this._contentLoader.EndLoad(result);
                if (loadResult == null)
                {
                    throw new InvalidOperationException(String.Format(Resource.NavigationService_InvalidLoadResult, "LoadResult", this._contentLoader.GetType()));
                }
                else if (loadResult.RedirectUri != null)
                {
                    // If we get a Redirect, navigate again without storing any context
                    this.NavigateCore(loadResult.RedirectUri, NavigationMode.New, false/*suppressJournalAdd*/, true/*isRedirect*/);
                }
                else
                {
                    content = loadResult.LoadedContent as DependencyObject;

                    // If the content is anything but a UserControl, we should throw.
                    // We support UserControls as they are a typical thing created in designers such as Blend, 
                    // but for a full experience one would use Page to get to things like NavigationContext,
                    // NavigationService, Title, etc.
                    if (!(content is UserControl))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                                                                          Resource.NavigationService_ContentIsNotAUserControl,
                                                                          content == null ? "null" : content.GetType().ToString(),
                                                                          "System.Windows.Controls.UserControl"));
                    }

                    // Content loader was successful, so complete navigation

                    // Create a new navigation context
                    JournalEntry.SetNavigationContext(content, new NavigationContext(UriParsingHelper.InternalUriParseQueryStringToDictionary(asyncNavigationOperationCompleted.Uri, true /* decodeResults */)));
                    content.SetValue(NavigationServiceProperty, this);

                    // Complete navigation operation
                    this.CompleteNavigation(content);
                }
            }
            catch (Exception ex)
            {
                if (this.RaiseNavigationFailed(uriBeingLoaded, ex))
                {
                    throw;
                }
            }
        }

        private void CompleteNavigation(DependencyObject content)
        {
            Uri uri = null;
            string pageTitle = null;
            Page existingContentPage = this._host.Content as Page;
            Page newContentPage = content as Page;

            pageTitle = JournalEntry.GetName(content ?? this._host.Content as DependencyObject);

            NavigationOperation navOp = this._currentNavigation;
            this._currentNavigation = null;
            if (navOp != null)
            {
                // Set uri
                uri = navOp.UriBeforeMapping;

                // Used to suppress navigation notifications.
                navOp.SuppressNotifications = true;

                if (this.CurrentSource == navOp.UriForJournal)
                {
                    // Do not record the navigation in the journal when moving to the same URI whether this
                    // is a redirection or not.
                    navOp.SuppressJournalAdd = true;
                }
                this.CurrentSource = navOp.UriForJournal;
                this._source = navOp.UriBeforeMapping;
                this._currentSourceAfterMapping = navOp.Uri;
                this.Host.UpdateSourceFromNavigationService(navOp.UriForJournal);
                this.Host.CurrentSource = this.CurrentSource;

                // Check if this is a 'New' operation
                if (navOp.Mode == NavigationMode.New && navOp.Uri != null && navOp.SuppressJournalAdd == false)
                {
                    try
                    {
                        this._journalIsAddingHistoryPoint = true;
                        JournalEntry je = new JournalEntry(pageTitle ?? uri.OriginalString, navOp.UriForJournal);
                        this.Journal.AddHistoryPoint(je);
                    }
                    finally
                    {
                        this._journalIsAddingHistoryPoint = false;
                    }
                }

                this.Host.CanGoBack = this.CanGoBack;
                this.Host.CanGoForward = this.CanGoForward;

                navOp.SuppressNotifications = false;
            }

            if (this.Journal.UseNavigationState && HtmlPage.IsEnabled)
            {
                HtmlPage.Document.SetProperty("title", pageTitle ?? (uri == null ? string.Empty : uri.OriginalString));
            }
            if (content == null)
            {
                // We're navigating to a fragment in the current page, so for WPF compatibility, fire FragmentNavigation THEN Navigated
                if (navOp != null)
                {
                    this.RaiseFragmentNavigation(UriParsingHelper.InternalUriGetFragment(navOp.Uri));
                    this.RaiseNavigated(content, uri, existingContentPage, newContentPage);
                }
            }
            else
            {
                // We're navigating to a fragment in the new content, so let the host load content, then for WPF compatibility,
                // fire Navigated THEN FragmentNavigation
                this.Host.Content = content;
                this.RaiseNavigated(content, uri, existingContentPage, newContentPage);
                string fragment = navOp == null ? null : UriParsingHelper.InternalUriGetFragment(navOp.Uri);
                if (!String.IsNullOrEmpty(fragment))
                {
                    this.RaiseFragmentNavigation(fragment);
                }
            }
        }

        private void UpdateNavigationCacheModeAlwaysPages()
        {
            Page currentPage = this.Host.Content as Page;
            if (currentPage != null)
            {
                string currentSourceWithoutFragment = UriParsingHelper.InternalUriGetAllButFragment(this.CurrentSource);

                if (currentPage.NavigationCacheMode == NavigationCacheMode.Required)
                {
                    // If this page is NavigationCacheMode == "Required" then put it in the dictionary to store a hard reference
                    // to it so it can be re-used by future navigations.
                    this._cacheRequiredPages[currentSourceWithoutFragment] = currentPage;
                }
                else
                {
                    // We must always try to remove, just in case this page used to be Required and is now Enabled or Disabled
                    this._cacheRequiredPages.Remove(currentSourceWithoutFragment);
                }


                if (currentPage.NavigationCacheMode == NavigationCacheMode.Enabled)
                {
                    // If this page is NavigationCacheMode == "Enabled" then put it in the cache
                    this.Cache.AddToCache(currentSourceWithoutFragment, currentPage);
                }
                else
                {
                    // We must always try to remove in case it went from Enabled to Disabled or Required
                    this.Cache.RemoveFromCache(currentSourceWithoutFragment);
                }
            }
        }

        /// <summary>
        /// Navigate to the next entry in the Journal
        /// </summary>
        /// <value></value>
        public void GoForward()
        {
            this.GoForwardBackCore(NavigationMode.Forward,
                                   this.CanGoForward,
                                   this.Journal.ForwardStack,
                                   string.Format(CultureInfo.InvariantCulture, Resource.CannotGoForward, "CanGoForward"));
        }

        /// <summary>
        /// Navigate to the previous entry in the Journal
        /// </summary>
        /// <value></value>
        public void GoBack()
        {
            this.GoForwardBackCore(NavigationMode.Back,
                                   this.CanGoBack,
                                   this.Journal.BackStack,
                                   string.Format(CultureInfo.InvariantCulture, Resource.CannotGoBack, "CanGoBack"));
        }

        /// <summary>
        /// Refreshes the current page
        /// </summary>
        public void Refresh()
        {
            this.NavigateCore(this.CurrentSource, NavigationMode.Refresh, true/*suppressJournalAdd*/, false/*isRedirect*/);
        }

        /// <summary>
        /// StopLoading aborts asynchronous navigations that haven't been processed yet.
        /// The <see cref="NavigationStopped"/> event is raised only if the navigation was actually aborted - if navigation is
        /// too far along to be canceled, then navigation may still complete and the <see cref="Navigated"/> event
        /// will be raised.
        /// </summary>
        /// <value></value>
        public void StopLoading()
        {
            StopLoadingCore(false/*fromRedirect*/);
        }

        private void StopLoadingCore(bool fromRedirect)
        {
            NavigationOperation navOp = this._currentNavigation;
            if (navOp != null)
            {
                if (!fromRedirect)
                {
                    // We don't want to call CancelLoad for redirects
                    this.ContentLoader.CancelLoad(navOp.AsyncResult);
                }

                this.RaiseNavigationStopped(null, navOp.Uri);

                // Release current context
                this._currentNavigation = null;
            }
        }

        private void GoForwardBackCore(NavigationMode mode, bool canDoIt, Stack<JournalEntry> entries, string onFailureText)
        {
            if (canDoIt)
            {
                JournalEntry entry = entries.Peek();

                bool isFragmentNavigationOnly =
                    UriParsingHelper.InternalUriIsFragment(entry.Source) ||
                    UriParsingHelper.InternalUriGetAllButFragment(entry.Source) == UriParsingHelper.InternalUriGetAllButFragment(this._currentSourceAfterMapping);

                if (this.RaiseNavigating(entry.Source, mode, isFragmentNavigationOnly) == false)
                {
                    if (mode == NavigationMode.Back)
                    {
                        this.Journal.GoBack();
                    }
                    else
                    {
                        this.Journal.GoForward();
                    }
                }
                else
                {
                    this.RaiseNavigationStopped(null, entry.Source);
                }
            }
            else
            {
                Exception ex = new InvalidOperationException(onFailureText);
                if (this.RaiseNavigationFailed(null, ex))
                {
                    throw ex;
                }
            }
        }

#region Event handlers

        /// <summary>
        /// Raises the Navigated event synchronously.
        /// </summary>
        /// <param name="content">A reference to the object content that is being navigated to.</param>
        /// <param name="uri">A URI value representing the navigation content.</param>
        /// <param name="existingContentPage">The existing content cast to a Page</param>
        /// <param name="newContentPage">The new content cast to a Page</param>
        private void RaiseNavigated(object content, Uri uri, Page existingContentPage, Page newContentPage)
        {
            NavigatedEventHandler eventHandler = this.Navigated;

            if (eventHandler != null)
            {
                NavigationEventArgs eventArgs = new NavigationEventArgs(content, uri);
                eventHandler(this, eventArgs);
            }

            if (existingContentPage != null && content != null)
            {
                existingContentPage.InternalOnNavigatedFrom(new NavigationEventArgs(content, uri));
            }

            if (newContentPage != null)
            {
                newContentPage.InternalOnNavigatedTo(new NavigationEventArgs(content, uri));
            }
        }

        /// <summary>
        /// Raises the Navigating event synchronously.
        /// </summary>
        /// <param name="uri">A URI value representing the navigation content.</param>
        /// <param name="mode">The mode of navigation being initiated (New, Forward or Back)</param>
        /// <param name="isFragmentNavigationOnly">True if this navigation is only a fragment navigation on the existing page, false if it is any other type of navigation</param>
        /// <returns>A value indicating whether or not to cancel the navigation operation.</returns>
        private bool RaiseNavigating(Uri uri, NavigationMode mode, bool isFragmentNavigationOnly)
        {
            NavigatingCancelEventHandler eventHandler = this.Navigating;
            bool canceled = false;

            if (eventHandler != null)
            {
                NavigatingCancelEventArgs eventArgs = new NavigatingCancelEventArgs(uri, mode);

                eventHandler(this, eventArgs);

                canceled = eventArgs.Cancel;
            }

            if (!isFragmentNavigationOnly)
            {
                Page p = this._host.Content as Page;
                if (p != null)
                {
                    NavigatingCancelEventArgs eventArgs = new NavigatingCancelEventArgs(uri, mode);
                    p.InternalOnNavigatingFrom(eventArgs);
                    canceled |= eventArgs.Cancel;
                }
            }

            return canceled;
        }

        /// <summary>
        /// Raises the Failed event synchronously.
        /// </summary>
        /// <param name="uri">A URI value representing the navigation content.</param>
        /// <param name="exception">The error that occurred</param>
        /// <returns>true if the the exception should be re-thrown, false if it was handled</returns>
        private bool RaiseNavigationFailed(Uri uri, Exception exception)
        {
            NavigationFailedEventHandler eventHandler = this.NavigationFailed;
            NavigationFailedEventArgs eventArgs = new NavigationFailedEventArgs(uri, exception);

            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }

            return !eventArgs.Handled;
        }

        /// <summary>
        /// Raises the Stopped event synchronously.
        /// </summary>
        /// <param name="content">A reference to the object content that is being navigated to.</param>
        /// <param name="uri">A URI value representing the navigation content.</param>
        private void RaiseNavigationStopped(object content, Uri uri)
        {
            NavigationStoppedEventHandler eventHandler = this.NavigationStopped;

            if (eventHandler != null)
            {
                NavigationEventArgs eventArgs = new NavigationEventArgs(content, uri);
                eventHandler(this, eventArgs);
            }
        }

        /// <summary>
        /// Raises the Fragment Navigation event synchronously
        /// </summary>
        /// <param name="fragment">The fragment that was navigated to</param>
        private void RaiseFragmentNavigation(string fragment)
        {
            FragmentNavigationEventHandler eventHandler = this.FragmentNavigation;

            if (eventHandler != null)
            {
                FragmentNavigationEventArgs eventArgs = new FragmentNavigationEventArgs(fragment);
                eventHandler(this, eventArgs);
            }

            Page p = this._host.Content as Page;
            if (p != null)
            {
                FragmentNavigationEventArgs eventArgs = new FragmentNavigationEventArgs(fragment);
                p.InternalOnFragmentNavigation(eventArgs);
            }
        }

#endregion

#endregion

#region Nested Classes, Structs

        /// <summary>
        /// Class used within the Frame to manage navigation operations.
        /// </summary>
        private class NavigationOperation
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="uri">The Uri after it was mapped</param>
            /// <param name="uriBeforeMapping">The Uri before it was mapped</param>
            /// <param name="uriForJournal">The Uri to use for the journal</param>
            /// <param name="mode">The mode (new, forward, or back) of this operation</param>
            /// <param name="suppressJournalUpdate">True if the journal shouldn't be updated by this operation, false otherwise</param>
            public NavigationOperation(Uri uri, Uri uriBeforeMapping, Uri uriForJournal, NavigationMode mode, bool suppressJournalUpdate)
            {
                this.Uri = uri;
                this.UriBeforeMapping = uriBeforeMapping;
                this.UriForJournal = uriForJournal;
                this.Mode = mode;
                this.SuppressJournalAdd = suppressJournalUpdate;
            }

            /// <summary>
            /// Gets or sets Uri used in the navigation operation, after passing through the UriMapper
            /// </summary>
            public Uri Uri
            {
                get;
                set;
            }

            public Uri UriBeforeMapping
            {
                get;
                set;
            }

            public Uri UriForJournal { get; set; }

            /// <summary>
            /// Gets or sets NavigationMode used in the current operation.
            /// </summary>
            public NavigationMode Mode
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the operation is altering the Source property.
            /// </summary>
            public bool SuppressNotifications
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the Journal should be updated based on this navigation operation
            /// </summary>
            public bool SuppressJournalAdd
            {
                get;
                set;
            }

            /// <summary>
            /// The IAsyncResult returned by the ContentLoader for this navigation operation.
            /// </summary>
            public IAsyncResult AsyncResult
            {
                get;
                set;
            }
        }

#endregion Nested Classes, Structs
    }
}
