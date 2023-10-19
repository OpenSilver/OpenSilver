//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using OpenSilver.Internal.Navigation;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a content control that supports navigation.
    /// </summary>
    /// <seealso cref="Page"/>
    /// <QualityBand>Stable</QualityBand>
    [TemplatePart(Name = Frame.PART_FrameNextButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = Frame.PART_FramePreviousButton, Type = typeof(ButtonBase))]
    public class Frame : ContentControl, INavigate
    {
#region Static Fields and Constants

        private const string PART_FrameNextButton = "NextButton";
        private const string PART_FramePreviousButton = "PrevButton";
        private const int DefaultCacheSize = 10;

#endregion

#region Fields

        private ButtonBase _nextButton;
        private ButtonBase _previousButton;
        private NavigationService _navigationService;
        private bool _loaded;
        private bool _updatingSourceFromNavigationService;
        private Uri _deferredNavigation;

#endregion  Fields

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.Frame" /> class. 
        /// </summary>
        public Frame()
        {
            this.DefaultStyleKey = typeof(Frame);
            this.Loaded += new RoutedEventHandler(this.Frame_Loaded);
            this._navigationService = new NavigationService(this);
        }

#endregion Constructors

#region Events

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available.
        /// </summary>
        public event NavigatedEventHandler Navigated
        {
            add { this._navigationService.Navigated += value; }
            remove { this._navigationService.Navigated -= value; }
        }

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public event NavigatingCancelEventHandler Navigating
        {
            add { this._navigationService.Navigating += value; }
            remove { this._navigationService.Navigating -= value; }
        }

        /// <summary>
        /// Occurs when an error is encountered while navigating to the requested content.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed
        {
            add { this._navigationService.NavigationFailed += value; }
            remove { this._navigationService.NavigationFailed -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="M:System.Windows.Controls.Frame.StopLoading" /> method is 
        /// called, or when a new navigation is requested while the current navigation is in progress. 
        /// </summary>
        public event NavigationStoppedEventHandler NavigationStopped
        {
            add { this._navigationService.NavigationStopped += value; }
            remove { this._navigationService.NavigationStopped -= value; }
        }

        /// <summary>
        /// Occurs when navigation to a content fragment begins.
        /// </summary>
        public event FragmentNavigationEventHandler FragmentNavigation
        {
            add { this._navigationService.FragmentNavigation += value; }
            remove { this._navigationService.FragmentNavigation -= value; }
        }

#endregion Events

#region Dependency Properties

#region Source Dependency Property

        /// <summary>
        /// The DependencyProperty for the Source property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(Uri),
                typeof(Frame),
                new PropertyMetadata(SourcePropertyChanged));

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) of the current
        /// content or the content that is being navigated to.
        /// </summary>
        /// <remarks>
        /// This value may be different from CurrentSource if you set Source and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri Source
        {
            get { return this.GetValue(SourceProperty) as Uri; }
            set { this.SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Called when Source property is changed
        /// </summary>
        /// <param name="depObj">The dependency property</param>
        /// <param name="e">The event arguments</param>
        private static void SourcePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = depObj as Frame;

            // Verify frame reference is valid and we're not in design mode.
            if (frame != null &&
                !Frame.IsInDesignMode() &&
                frame._loaded &&
                frame._updatingSourceFromNavigationService == false)
            {
                frame.Navigate(e.NewValue as Uri);
            }

            if (Frame.IsInDesignMode())
            {
                if (e.NewValue != null)
                {
                    frame.Content = String.Format(CultureInfo.InvariantCulture, Resource.Frame_DefaultContent, e.NewValue.ToString());
                }
                else
                {
                    frame.Content = frame.GetType().Name;
                }
            }
        }

#endregion

#region JournalOwnership Dependency Property

        /// <summary>
        /// The DependencyProperty for the JournalOwnership property.
        /// </summary>
        public static readonly DependencyProperty JournalOwnershipProperty =
            DependencyProperty.Register(
                "JournalOwnership",
                typeof(JournalOwnership),
                typeof(Frame),
                new PropertyMetadata(JournalOwnership.Automatic, JournalOwnershipPropertyChanged));

        /// <summary>
        /// Gets or sets whether a frame is responsible for managing its own navigation history,
        /// or whether it integrates with the web browser journal.
        /// </summary>
        public JournalOwnership JournalOwnership
        {
            get { return (JournalOwnership)this.GetValue(JournalOwnershipProperty); }
            set { this.SetValue(JournalOwnershipProperty, value); }
        }

        private static void JournalOwnershipPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = depObj as Frame;
            if (depObj != null)
            {
                try
                {
                    frame.NavigationService.InitializeJournal();
                }
                catch (Exception)
                {
                    frame.JournalOwnership = (JournalOwnership)e.OldValue;
                    throw;
                }
            }
        }

#endregion

#region CanGoBack Dependency Property

        /// <summary>
        /// The DependencyProperty for the CanGoBack property.
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register(
                "CanGoBack",
                typeof(bool),
                typeof(Frame),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary> 
        public bool CanGoBack
        {
            get { return (bool)this.GetValue(CanGoBackProperty); }
            internal set { this.SetValueNoCallback(CanGoBackProperty, value); }
        }

#endregion

#region CanGoForward Dependency Property

        /// <summary>
        /// The DependencyProperty for the CanGoForward property.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty =
            DependencyProperty.Register(
                "CanGoForward",
                typeof(bool),
                typeof(Frame),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward navigation history.
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)this.GetValue(CanGoForwardProperty); }
            internal set { this.SetValueNoCallback(CanGoForwardProperty, value); }
        }

#endregion

#region CurrentSource Dependency Property

        /// <summary>
        /// The DependencyProperty for the CurrentSource property.
        /// </summary>
        public static readonly DependencyProperty CurrentSourceProperty =
            DependencyProperty.Register(
                "CurrentSource",
                typeof(Uri),
                typeof(Frame),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Gets the uniform resource identifier (URI) of the content that was last navigated to.
        /// </summary>
        /// <remarks>
        /// This value may be different from Source if you set Source and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri CurrentSource
        {
            get { return this.GetValue(CurrentSourceProperty) as Uri; }
            internal set { this.SetValueNoCallback(CurrentSourceProperty, value); }
        }

#endregion

#region UriMapper Dependency Property

        /// <summary>
        /// The DependencyProperty for the UriMapper property.
        /// </summary>
        public static readonly DependencyProperty UriMapperProperty =
            DependencyProperty.Register(
                "UriMapper",
                typeof(UriMapperBase),
                typeof(Frame),
                null);

        /// <summary>
        /// Gets or sets the object to manage converting a uniform resource identifier (URI) 
        /// to another URI for this frame.
        /// </summary>
        public UriMapperBase UriMapper
        {
            get { return this.GetValue(UriMapperProperty) as UriMapperBase; }
            set { this.SetValue(UriMapperProperty, value); }
        }

#endregion

#region ContentLoader Dependency Property

        /// <summary>
        /// The DependencyProperty for the ContentLoader property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty =
            DependencyProperty.Register(
                "ContentLoader",
                typeof(INavigationContentLoader),
                typeof(Frame),
                new PropertyMetadata(ContentLoaderPropertyChanged));

        /// <summary>
        /// Gets or sets whether a frame is responsible for managing its own navigation history,
        /// or whether it integrates with the web browser journal.
        /// </summary>
        public INavigationContentLoader ContentLoader
        {
            get { return (INavigationContentLoader)this.GetValue(ContentLoaderProperty); }
            set { this.SetValue(ContentLoaderProperty, value); }
        }

        private static void ContentLoaderPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = (Frame)depObj;
            INavigationContentLoader contentLoader = (INavigationContentLoader)e.NewValue;
            if (contentLoader == null)
            {
                throw new ArgumentNullException("ContentLoader");
            }

            if (frame.NavigationService.IsNavigating)
            {
                throw new InvalidOperationException(String.Format(Resource.Frame_CannotSetLoaderWhenLoading, "ContentLoader"));
            }
            else
            {
                frame.NavigationService.ContentLoader = contentLoader;
            }
        }

#endregion

#region CacheSize Dependency Property

        /// <summary>
        /// The DependencyProperty for the CacheSize property.
        /// </summary>
        public static readonly DependencyProperty CacheSizeProperty =
            DependencyProperty.Register(
                "CacheSize",
                typeof(int),
                typeof(Frame),
                new PropertyMetadata(DefaultCacheSize, new PropertyChangedCallback(CacheSizePropertyChanged)));

        /// <summary>
        /// Gets or sets the number of pages that can be cached for the frame.
        /// </summary> 
        public int CacheSize
        {
            get { return (int)this.GetValue(CacheSizeProperty); }
            set { this.SetValue(CacheSizeProperty, value); }
        }

        private static void CacheSizePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = (Frame)depObj;

            if (!frame.AreHandlersSuspended())
            {
                int newCacheSize = (int)e.NewValue;
                if (newCacheSize < 0)
                {
                    frame.SetValueNoCallback(CacheSizeProperty, e.OldValue);
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture,
                                      Resource.Frame_CacheSizeMustBeGreaterThanOrEqualToZero,
                                      "CacheSize"));
                }

                if (frame._navigationService != null && frame._navigationService.Cache != null)
                {
                    frame._navigationService.Cache.ChangeCacheSize(newCacheSize);
                }
            }
        }

#endregion

#endregion

#region Properties

        internal NavigationService NavigationService
        {
            get { return this._navigationService; }
        }

#endregion Properties

#region Methods

        internal static bool IsInDesignMode()
        {
            // Because VS 2008 doesn't support DesignerProperties.IsInDesignTool, we check the Application.Current object
            // first - if it's null or of type Application, we know we're in a designer.  Otherwise, defer to the newer
            // DesignerProperties.IsInDesignTool property for VS 2010 and Blend 3.
            return Application.Current == null ||
                   Application.Current.GetType() == typeof(Application) ||
                   DesignerProperties.IsInDesignTool;
        }

        /// <summary>
        /// This will check for deep link values in the URL if the Frame's 
        /// Journal is integrated with the browser.
        /// </summary>
        /// <returns>A value indicating whether or not deep links were found.</returns>
        internal bool ApplyDeepLinks()
        {
            return this.NavigationService.Journal.CheckForDeeplinks();
        }

        /// <summary>
        /// Stops further downloading of content for the current navigation request.
        /// </summary>
        public void StopLoading()
        {
            this._navigationService.StopLoading();
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, if one exists.
        /// </summary>
        public void GoBack()
        {
            this._navigationService.GoBack();
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, if one exists.
        /// </summary>
        public void GoForward()
        {
            this._navigationService.GoForward();
        }

        /// <summary>
        /// Navigates to the content specified by the uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">The URI representing a page to display in the frame.</param>
        /// <returns>Always returns true.</returns>
        public bool Navigate(Uri source)
        {
            if (this._loaded)
            {
                return this._navigationService.Navigate(source);
            }
            else
            {
                this._deferredNavigation = source;
                return true;
            }
        }

        /// <summary>
        /// Called when the template generation for the visual tree is created.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Unhook and rehook our Next button
            if (this._nextButton != null)
            {
                this._nextButton.Click -= new RoutedEventHandler(this.PART_nextButton_Click);
            }

            this._nextButton = GetTemplateChild(Frame.PART_FrameNextButton) as ButtonBase;
            if (this._nextButton != null)
            {
                this._nextButton.Click += new RoutedEventHandler(this.PART_nextButton_Click);
            }

            if (this._previousButton != null)
            {
                this._previousButton.Click -= new RoutedEventHandler(this.PART_previousButton_Click);
            }

            this._previousButton = GetTemplateChild(Frame.PART_FramePreviousButton) as ButtonBase;

            if (this._previousButton != null)
            {
                this._previousButton.Click += new RoutedEventHandler(this.PART_previousButton_Click);
            }
        }

        /// <summary>
        /// Refreshes the current page
        /// </summary>
        public void Refresh()
        {
            this._navigationService.Refresh();
        }

        /// <summary>
        /// Returns a <see cref="FrameAutomationPeer"/> for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A <see cref="FrameAutomationPeer"/> for the Frame object.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameAutomationPeer(this);
        }

        internal void UpdateSourceFromNavigationService(Uri newSource)
        {
            if (this.Source != newSource)
            {
                this._updatingSourceFromNavigationService = true;
                this.SetValue(SourceProperty, newSource);
                this._updatingSourceFromNavigationService = false;
            }
        }

        /// <summary>
        /// Called when a Read-Only dependency property is changed
        /// </summary>
        /// <param name="depObj">The dependency object</param>
        /// <param name="e">The event arguments</param>
        private static void OnReadOnlyPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = depObj as Frame;
            if (frame != null && !frame.AreHandlersSuspended())
            {
                frame.SetValueNoCallback(e.Property, e.OldValue);
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.UnderlyingPropertyIsReadOnly,
                        e.Property.ToString()));
            }
        }

        /// <summary>
        /// Called when the Frame.Loaded event fires.
        /// </summary>
        /// <param name="sender">The object raising the event</param>
        /// <param name="e">The event arguments</param>
        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            this._navigationService.InitializeJournal();

            if (this.ContentLoader == null)
            {
                this.ContentLoader = new PageResourceContentLoader();
            }

            this._navigationService.InitializeNavigationCache();

            // Set loaded flag
            this._loaded = true;

            // Don't attempt to load anything at design-time
            if (!Frame.IsInDesignMode())
            {
                UriMapperBase mapper = this.UriMapper;

                // If there's a deeplink, don't check Source as the deeplink overrides it
                if (this.ApplyDeepLinks() == false)
                {
                    if (this._deferredNavigation != null)
                    {
                        this.Navigate(this._deferredNavigation);
                        this._deferredNavigation = null;
                    }

                    // Check if source property was set
                    else if (this.Source != null)
                    {
                        this.Navigate(this.Source);
                    }

                    // If no Source was set, we may still be able to use UriMapper to convert this to a navigable Uri
                    else if (mapper != null)
                    {
                        Uri emptyUri = new Uri(String.Empty, UriKind.Relative);
                        Uri mappedUri = mapper.MapUri(emptyUri);
                        if (mappedUri != null && !String.IsNullOrEmpty(mappedUri.OriginalString))
                        {
                            this.Navigate(emptyUri);
                        }
                    }
                }
            }
            else
            {
                if (this.Source != null)
                {
                    this.Content = String.Format(CultureInfo.InvariantCulture, Resource.Frame_DefaultContent, this.Source.ToString());
                }
                else
                {
                    this.Content = typeof(Frame).Name;
                }
            }
        }

        /// <summary>
        /// Next button handler
        /// </summary>
        /// <param name="sender">The object raising the event</param>
        /// <param name="e">The event arguments</param>
        private void PART_nextButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoForward();
        }

        /// <summary>
        /// Previous button handler
        /// </summary>
        /// <param name="sender">The object raising the event</param>
        /// <param name="e">The event arguments</param>
        private void PART_previousButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoBack();
        }

#endregion Methods
    }
}
