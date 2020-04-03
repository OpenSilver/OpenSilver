

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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSHTML5.Internal;
#if !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif

#if MIGRATION
using System.Windows.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //[TemplatePart(Name = "NextButton", Type = typeof(ButtonBase))]
    //[TemplatePart(Name = "PrevButton", Type = typeof(ButtonBase))]
    /// <summary>
    /// Represents a control that supports navigation to and from pages.
    /// </summary>
    public partial class Frame : ContentControl, INavigate
    {
        bool _isBrowserJournal = true; //we use this so we don't have to determine all the time which journal we want to use.
        bool _isSourceChanging = false;
        bool _isGoingBackOrForward = false;
        bool _isTopLevelFrame = true; //this tells us if the frame is contained whithin a frame, which allows us to determine which Journal Ownership to apply.
        FrameCache _cache = new FrameCache();
        bool _subscribedToHashChangeEvent = false;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.Frame class.
        /// </summary>
        public Frame()
        {
            this.Loaded += Frame_Loaded;
            this.Unloaded += Frame_Unloaded;
        }

        void UpdateCanGoBack()
        {
            if (_isBrowserJournal)
                CanGoBack = CanGoBack_BrowserJournalVersion();
            else
                CanGoBack = _cache.CanGoBack;
        }
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back
        /// navigation history.
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            internal set { SetValue(CanGoBackProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.Frame.CanGoBack dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof(bool), typeof(Frame), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        void UpdateCanGoForward()
        {
            if (_isBrowserJournal)
                CanGoForward = CanGoForward_BrowserJournalVersion();
            else
                CanGoForward = _cache.CanGoForward;
        }
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward
        /// navigation history.
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            internal set { SetValue(CanGoForwardProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.Frame.CanGoForward dependency property.
        /// </summary>
        public static readonly DependencyProperty CanGoForwardProperty =
            DependencyProperty.Register("CanGoForward", typeof(bool), typeof(Frame), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) of the current content
        /// or the content that is being navigated to.
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.Frame.Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(Frame), new PropertyMetadata(null, Source_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                Uri source = (Uri)e.NewValue;
                Frame frame = (Frame)d;
                if (frame._isBrowserJournal)
                {
                    if (!frame._isNavigationDone)
                    {
                        frame.Navigate_BrowserJournalVersion(source, true);
                    }
                }
                else
                {
                    if (!frame._isGoingBackOrForward)
                    {
                        frame._isSourceChanging = true;
                        frame.Navigate((Uri)e.NewValue);
                        frame._isSourceChanging = false;
                    }
                }
            }
        }

        //Note: We do not throw an exception when multiple frames attempt to integrate with
        //      browser journal since it is not the behavior that I observed when testing.
        // Exceptions:
        //   System.InvalidOperationException:
        //     A nested frame or more than one frame attempts to integrate with browser
        //     journal.
        /// <summary>
        /// Gets or sets whether a frame is responsible for managing its own navigation
        /// history, or whether it integrates with the Web browser journal.
        /// </summary>
        public JournalOwnership JournalOwnership
        {
            get { return (JournalOwnership)GetValue(JournalOwnershipProperty); }
            set { SetValue(JournalOwnershipProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.Frame.JournalOwnership dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty JournalOwnershipProperty =
            DependencyProperty.Register("JournalOwnership", typeof(JournalOwnership), typeof(Frame), new PropertyMetadata(JournalOwnership.Automatic, JournalOwnership_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private static void JournalOwnership_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = (Frame)d;
            frame.UpdateFieldsBasedOnJournalOwnership();
        }

        void UpdateFieldsBasedOnJournalOwnership()
        {
            switch (JournalOwnership)
            {
                case JournalOwnership.Automatic:
                    if (_isTopLevelFrame)
                    {
                        if (!_isBrowserJournal)
                        {
                            _cache.Clear();
                            _isBrowserJournal = true;
                            SubscribeToHashChanged();
                        }
                    }
                    else
                    {
                        if (_isBrowserJournal)
                        {
                            _isBrowserJournal = false;
                            UnSubscribeFromHashChanged();
                        }
                    }
                    break;
                case JournalOwnership.OwnsJournal:
                    if (_isBrowserJournal)
                    {
                        _isBrowserJournal = false;
                        UnSubscribeFromHashChanged();
                    }
                    break;
                default: //this is the UsesParentJournal one so we will use the browser (which is what is intended)
                    if (_isTopLevelFrame)
                    {
                        if (!_isBrowserJournal)
                        {
                            _cache.Clear();
                            _isBrowserJournal = true;
                            SubscribeToHashChanged();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("A frame cannot use the browser's journal it it is not a top-level frame.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the object to manage converting a uniform resource identifier
        /// (URI) to another URI for this frame.
        /// </summary>
        public UriMapperBase UriMapper
        {
            get { return (UriMapperBase)GetValue(UriMapperProperty); }
            set { SetValue(UriMapperProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.Frame.UriMapper dependency property.
        /// </summary>
        public static readonly DependencyProperty UriMapperProperty =
            DependencyProperty.Register("UriMapper", typeof(UriMapperBase), typeof(Frame), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        // Exceptions:
        //   System.InvalidOperationException:
        //     There are no entries in the back navigation history.
        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or throws
        /// an exception if no entry exists in back navigation.
        /// </summary>
        public void GoBack()
        {
            if (_isBrowserJournal)
            {
                GoBack_BrowserJournalVersion();
            }
            else
            {
                _isGoingBackOrForward = true;
                Navigate(_cache.GoBack()._uri);
                _isGoingBackOrForward = false;
            }
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     There are no entries in the forward navigation history.
        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or
        /// throws an exception if no entry exists in forward navigation.
        /// </summary>
        public void GoForward()
        {
            if (_isBrowserJournal)
            {
                GoForward_BrowserJournalVersion();
            }
            else
            {
                _isGoingBackOrForward = true;
                Navigate(_cache.GoForward()._uri);
                _isGoingBackOrForward = false;
            }
        }

        /// <summary>
        /// Navigates to the content specified by the uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">The URI representing a page to display in the frame.</param>
        /// <returns>true if the navigation started successfully; otherwise, false.</returns>
        public bool Navigate(Uri source)
        {
            if (_isBrowserJournal)
            {
                return Navigate_BrowserJournalVersion(source, true);
            }
            else
            {
                return Navigate_OwnJournalVersion(source);
            }
        }



#region own ournal specific methods

        internal bool Navigate_OwnJournalVersion(Uri source)
        {
            try
            {
                if (_isGoingBackOrForward)
                {
                    this.Content = _cache._currentItem._value;
                }
                if (_isSourceChanging) //note: we will never have both _isGoingBackOrForward and _isSourceChanging set tot true.
                {
                    object page = GetPageFromUri(source);
                    if (page != null)
                    {
                        _cache.Add(page, source);
                        this.Content = page;

                        UpdateCanGoBack();
                        UpdateCanGoForward();
                        RaiseNavigatedEvent();
                    }
                    else
                    {
                        RaiseNavigationFailedEvent(source);
                    }
                }
                else if (source != Source)
                {
                    Source = source; //not sure this is the best way to do it but this will call Source_Changed, which will in turn call this method with _isSourceChanging to true. That way we only do the thing once, while still setting Source, no matter how this method was called.
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

#endregion

#region browser journal specific methods


        //--------------------------------------------------
        //------Case where we use the browser history:------
        //when navigating:
        //check the current history.state:
        //      if "", go to next and history.replaceState("firstLast", "")
        //      else,
        //          if current is "firstLast" --> "first"
        //          else if current is "last" --> "inter"
        //          go to next and history.replaceState("last", "")

        //when checking CanGoBack:
        //  if history.state == "first" OU firstLast --> return false.
        //  else --> return true;

        //when checking CanGoForward:
        //  if history.state == last || firstLast --> return false
        //  else --> return true.
        //--------------------------------------------------


        const string FIRST_AND_LAST_PAGE_STATE_NAME = "firstLast";
        const string FIRST_PAGE_STATE_NAME = "first";
        const string LAST_PAGE_STATE_NAME = "last";
        const string INTERMEDIARY_PAGE_STATE_NAME = "inter";
        bool _isNavigationDone = false;
        private bool Navigate_BrowserJournalVersion(Uri source, bool updateHistoryState)
        {
            string state = null;
            string stateAfterNavigating = LAST_PAGE_STATE_NAME;
            if (updateHistoryState)
            {
                state = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("history.state"));
                string stateBeforeNavigating = null;
                if (string.IsNullOrEmpty(state))
                {
                    //this is the first page we go to so it is the first and the last one at the moment:
                    stateBeforeNavigating = null;
                    stateAfterNavigating = FIRST_AND_LAST_PAGE_STATE_NAME;
                }
                else if (state == FIRST_AND_LAST_PAGE_STATE_NAME)
                {
                    //we had only one page in the history so we update the first from first and last to only first and set the new one as last:
                    stateBeforeNavigating = FIRST_PAGE_STATE_NAME;
                    stateAfterNavigating = LAST_PAGE_STATE_NAME;
                }
                else if (state == FIRST_PAGE_STATE_NAME)
                {
                    //the page we were on was the first so we simply add the new one as last:
                    stateAfterNavigating = LAST_PAGE_STATE_NAME;
                }
                else if (state == LAST_PAGE_STATE_NAME)
                {
                    //we were on the last page so we change it to intermediary page and set the new one as last:
                    stateBeforeNavigating = INTERMEDIARY_PAGE_STATE_NAME;
                    stateAfterNavigating = LAST_PAGE_STATE_NAME;
                }
                else
                {
                    //we were on a Intermediary page so we simply set the new one at last:
                    stateAfterNavigating = LAST_PAGE_STATE_NAME;
                }

                //update the hash of the previous page:
                if (stateBeforeNavigating != null)
                {
                    CSHTML5.Interop.ExecuteJavaScript("history.replaceState($0, '')", stateBeforeNavigating);
                }
            }

            //go to the page:
            if (TryGoToPage(source))
            {
                //set the new source:
                _isNavigationDone = true;
                Source = source;
                _isNavigationDone = false;

                if (updateHistoryState)
                {
                    //update the hash:
                    CSHTML5.Interop.ExecuteJavaScriptAsync("location.hash = $0", source != null ? source.OriginalString : "");
                    //set the state of the new page (this one is always set):
                    CSHTML5.Interop.ExecuteJavaScriptAsync("history.replaceState($0, '')", stateAfterNavigating);
                }

                UpdateCanGoBack();
                UpdateCanGoForward();
                RaiseNavigatedEvent();
                return true;
            }
            else
            {
                RaiseNavigationFailedEvent(source);
                return false;
            }
        }

        private bool CanGoBack_BrowserJournalVersion()
        {
            string state = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("history.state"));
            if (state == "" || state == FIRST_PAGE_STATE_NAME || state == FIRST_AND_LAST_PAGE_STATE_NAME)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CanGoForward_BrowserJournalVersion()
        {
            string state = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("history.state"));
            if (state == "" || state == LAST_PAGE_STATE_NAME || state == FIRST_AND_LAST_PAGE_STATE_NAME)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void GoBack_BrowserJournalVersion()
        {
            CSHTML5.Interop.ExecuteJavaScript("history.back()");
        }

        private void GoForward_BrowserJournalVersion()
        {
            CSHTML5.Interop.ExecuteJavaScript("history.forward()");
        }

        internal void LocationHashChanged()
        {
            string newHash = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("location.hash"));

            if (!string.IsNullOrWhiteSpace(newHash))
            {
                if (newHash.StartsWith("#"))
                {
                    newHash = newHash.Substring(1);
                }

                // Compare with the current URL and navigate only if it is different:
                if (Source == null || newHash != Source.ToString())
                {
                    Navigate_BrowserJournalVersion(new Uri(newHash, UriKind.RelativeOrAbsolute), false);
                }
            }
            else
            {
                // Compare with the current URL and navigate only if it is different:
                if (Source != null && !string.IsNullOrWhiteSpace(Source.ToString()))
                {
                    Navigate_BrowserJournalVersion(null, false);
                }
            }
        }


        private void SubscribeToHashChanged()
        {
            //subscribe to the window.hashchange event:
            if (!_subscribedToHashChangeEvent)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("window.addEventListener('hashchange', $0, false)", (Action)LocationHashChanged);
                _subscribedToHashChangeEvent = true;
            }
        }

        private void UnSubscribeFromHashChanged()
        {
            //unsubscribe from the window.hashchange event:
            if (_subscribedToHashChangeEvent)
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync("window.removeEventListener('hashchange', $0, false)", (Action)LocationHashChanged);
                _subscribedToHashChangeEvent = false;
            }
        }

#endregion

        private object GetPageFromUri(Uri source)
        {
            object page = null;
            if (source != null && !string.IsNullOrWhiteSpace(source.OriginalString))
            {
                //we map the uri:
                Uri mappedSource = source;
                if (UriMapper != null)
                {
                    mappedSource = UriMapper.MapUri(source);
                }

                //do the thing:
                page = FindPage(mappedSource);
            }
            return page;
        }

        private bool TryGoToPage(Uri source)
        {
            object page = GetPageFromUri(source);
            if (page != null)
            {
                this.Content = page;
                return true;
            }
            else
            {
                return false;
            }
        }

        static object FindPage(Uri uri)
        {
            string assemblyName;
            string instancierName;
            GetXamlFileAssociatedClassInstancierName(uri, out assemblyName, out instancierName); //this method should only accept .xaml files (imo), it returns the name of the class we generated during the compilation, built from the assembly(gotten from either the uri, or the startingAssembly) and the file name.
#if BRIDGE
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (INTERNAL_BridgeWorkarounds.GetAssemblyNameWithoutCallingGetNameMethod(assembly) == assemblyName)
                {
                    Type type = assembly.GetType(instancierName);
                    if (type != null)
                    {
                        MethodInfo methodInfo = type.GetMethod("Instantiate");
                        return methodInfo.Invoke(null, null);
                    }
                    break;
                }
            }
#else
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == assemblyName)
                    {
                        Type type = assembly.GetType(instancierName);
                        if (type != null)
                        {
                            MethodInfo methodInfo = type.GetMethod("Instantiate");
                            return methodInfo.Invoke(null, null);
                        }
                        break;
                    }
                }
            }
            else
            {
                object assembly = CSHTML5.Interop.ExecuteJavaScript(@"JSIL.GetAssembly($0, true)", assemblyName);
                object type = CSHTML5.Interop.ExecuteJavaScript(@"JSIL.GetTypeFromAssembly($0, $1)", assembly, instancierName);
                if (!IsNullOrUndefined(type))
                {
                    object method = CSHTML5.Interop.ExecuteJavaScript(@"$0.GetMethod('Instantiate')", type);
                    return CSHTML5.Interop.ExecuteJavaScript("$0.Invoke(null, null)", method);
                }
            }
#endif
            return null;
        }

        static bool IsNullOrUndefined(object jsObject)
        {
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                if (jsObject == null)
                    return true;
#if CSHTML5NETSTANDARD
                return false;
#else
                if (!(jsObject is JSValue))
                    return false;
                JSValue value = ((JSValue)jsObject);
                return value.IsNull() || value.IsUndefined();
#endif
            }
            else
                return Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("(typeof $0 === 'undefined' || $0 === null)", jsObject));
        }

        static void GetXamlFileAssociatedClassInstancierName(Uri uri, out string assemblyName, out string instancierName)
        {
            string uriAsString = uri.OriginalString;
            //we make sure we have an absolute uri:
            string absoluteSourceUri = "";
            assemblyName = "";
            string[] splittedUri = uriAsString.Split(';');
            if (splittedUri.Length == 1)
            {
                assemblyName = CSHTML5.Internal.StartupAssemblyInfo.StartupAssemblyShortName;
                if (uriAsString.StartsWith("/"))
                {
                    absoluteSourceUri = "/" + assemblyName + ";component" + uriAsString;
                }
                else
                {
                    absoluteSourceUri = "/" + assemblyName + ";component/" + uriAsString;
                }
            }
            else if (splittedUri.Length == 2)
            {
                assemblyName = splittedUri[0];
                if (assemblyName.StartsWith("/"))
                {
                    assemblyName = assemblyName.Substring(1);
                }
                absoluteSourceUri = uriAsString;
            }
            else
            {
                throw new Exception("Class name badly formatted.");
            }
            instancierName = GenerateClassNameFromAbsoluteUri_ForRuntimeAccess(absoluteSourceUri);
        }

        static string GenerateClassNameFromAbsoluteUri_ForRuntimeAccess(string absoluteSourceUri)
        {
            // Convert to TitleCase (so that when we remove the spaces, it is easily readable):
            string className = MakeTitleCase(absoluteSourceUri);

            // If file name contains invalid chars, remove them:
            className = Regex.Replace(className, @"\W", "ǀǀ"); //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            // If class name doesn't begin with a letter, insert an underscore:
            if (char.IsDigit(className, 0))
            {
                className = className.Insert(0, "_");
            }

            // Remove white space:
            className = className.Replace(" ", string.Empty);

            className += "ǀǀFactory"; //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            return className;
        }

        static string MakeTitleCase(string str)
        {
            string result = "";
            string lowerStr = str.ToLower();
            int length = str.Length;
            bool makeUpper = true;
            int lastCopiedIndex = -1;
            //****************************
            //HOW THIS WORKS:
            //
            //  We go through all the characters of the string.
            //  If any is not an alphanumerical character, we make the next alphanumerical character uppercase.
            //  To do so, we copy the string (on which we call toLower) bit by bit into a new variable,
            //  each bit being the part between two uppercase characters, and while inserting the uppercase version of the character between each bit.
            //  then we add the end of the string.
            //****************************

            for (int i = 0; i < length; ++i)
            {
                char ch = lowerStr[i];
                if (ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '0')
                {
                    if (makeUpper && ch >= 'a' && ch <= 'z') //if we have a letter, we make it uppercase. otherwise, it is a number so we let it as is.
                    {
                        if (!(lastCopiedIndex == -1 && i == 0)) //except this very specific case, we should never have makeUpper at true while i = lastCopiedindex + 1 (since we made lowerStr[lastCopiedindex] into an uppercase letter.
                        {
                            result += lowerStr.Substring(lastCopiedIndex + 1, i - lastCopiedIndex - 1); //i - lastCopied - 1 because we do not want to copy the current index since we want to make it uppercase:
                        }
                        result += (char)(ch - 32); //32 is the difference between the lower case and the upper case, meaning that (char)('a' - 32) --> 'A'.
                        lastCopiedIndex = i;
                    }
                    makeUpper = false;
                }
                else
                {
                    makeUpper = true;
                }
            }
            //we copy the rest of the string:
            if (lastCopiedIndex < length - 1)
            {
                result += str.Substring(lastCopiedIndex + 1);
            }
            return result;


            //bool isFirst = true;
            //string[] spaceSplittedString = str.Split(' ');
            //foreach (string s in spaceSplittedString)
            //{
            //    if (isFirst)
            //    {
            //        isFirst = false;
            //    }
            //    else
            //    {
            //        result += " ";
            //    }
            //    result += MakeFirstCharUpperAndRestLower(s);
            //}
            //return result;
        }


        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is
        /// available.
        /// </summary>
        public event NavigatedEventHandler Navigated;

        void RaiseNavigatedEvent()
        {
            if (Navigated != null)
            {
                object content = (_isBrowserJournal ? this.Content : (_cache._currentItem != null ? _cache._currentItem._value : null));
                Uri uri = (_isBrowserJournal ? this.Source : (_cache._currentItem != null ? _cache._currentItem._uri : null));

                Navigated(this, new NavigationEventArgs(content, uri));
            }
        }

        /// <summary>
        /// Occurs when an error is encountered while navigating to the requested content.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed;

        void RaiseNavigationFailedEvent(Uri uri)
        {
            ArgumentException exception = new ArgumentException("Content for the URI cannot be loaded. The URI may be invalid.", "uri");

            // The commented code below shows how it is supposed to behave if we follow the same behavior as in Silverlight (ie. an exception is thrown every time that the URL is wrong)
            /*
            if (NavigationFailed != null)
            {
                var navigationFailedEventArgs = new NavigationFailedEventArgs(exception, false, uri);
                NavigationFailed(this, navigationFailedEventArgs);

                if (!navigationFailedEventArgs.Handled)
                    throw exception;
            }
            else
            {
                throw exception;
            }
            */

            // Instead, in our case, we want to avoid having too many error messages when the URI fragment is wrong, so we only throw an exception if the developer has registerd the NavigationFailed event and explicitly set the "Handled" property to False:
            if (NavigationFailed != null)
            {
                var navigationFailedEventArgs = new NavigationFailedEventArgs(exception, true, uri); // We set the "Handled" property to "true" by default.
                NavigationFailed(this, navigationFailedEventArgs);

                if (!navigationFailedEventArgs.Handled)
                    throw exception;
            }
        }

        void Frame_Loaded(object sender, RoutedEventArgs e) // Note: we use the "Loaded" event instead of the OnAttachedToVisualTree because it is better to not modify the visual tree when inside the OnAttached method.
        {
            _isTopLevelFrame = CheckIfThisIsATopLevelFrame();

            UpdateFieldsBasedOnJournalOwnership();
            if (_isBrowserJournal)
            {
                SubscribeToHashChanged();

                // We make it so that when launching the application at an URL that has a particular Hash, the frame displays the correct initial page:
                Dispatcher.BeginInvoke(() => // We use a dispatcher, so that the MainPage has the time to initialize itself (cf. Client_AS when launching directly at the "#/CategoryView" page)
                {
                    LocationHashChanged(); //to set the original page.
                }
                );
            }
        }

        private bool CheckIfThisIsATopLevelFrame()
        {
            //we go through the parents and if we meet a Frame, return false, else return true:
            DependencyObject currentElement = this.INTERNAL_VisualParent;
            while (currentElement != null)
            {
                if (currentElement is Frame)
                {
                    return false;
                }
                currentElement = ((UIElement)currentElement).INTERNAL_VisualParent; //I don't know what else we could have here.
            }
            return true;
        }


        void Frame_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_isBrowserJournal)
            {
                UnSubscribeFromHashChanged();
            }
        }


#region not implemented yet:

        //// Returns:
        ////     The number of pages that can be cached for the frame. The default value is
        ////     10.
        ///// <summary>
        ///// Gets or sets the number of pages that can be cached for the frame.
        ///// </summary>
        //public int CacheSize
        //{
        //    get { return (int)GetValue(CacheSizeProperty); }
        //    set { SetValue(CacheSizeProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the System.Windows.Controls.Frame.CacheSize dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CacheSizeProperty =
        //    DependencyProperty.Register("CacheSize", typeof(int), typeof(Frame), new PropertyMetadata(10, CacheSize_Changed));

        //private static void CacheSize_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Frame frame = (Frame)d;
        //    frame._cache._cacheSize = (int)e.NewValue;
        //    //todo find out how this works in normal silverlight.
        //}

        //// Returns:
        ////     The object responsible for providing the content that corresponds to a requested
        ////     URI. The default is a System.Windows.Navigation.PageResourceContentLoader
        ////     instance.
        ///// <summary>
        ///// Gets or sets the object responsible for providing the content that corresponds
        ///// to a requested URI.
        ///// </summary>
        //public INavigationContentLoader ContentLoader { get; set; }
        ///// <summary>
        ///// Identifies the System.Windows.Controls.Frame.ContentLoader dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ContentLoaderProperty;

        ///// <summary>
        ///// Gets the uniform resource identifier (URI) of the content that is currently
        ///// displayed.
        ///// </summary>
        //public Uri CurrentSource { get; }
        ///// <summary>
        ///// Identifies the System.Windows.Controls.Frame.CurrentSource dependency property.
        ///// </summary>
        //public static readonly DependencyProperty CurrentSourceProperty;

        ///// <summary>
        ///// Occurs when navigation to a content fragment begins.
        ///// </summary>
        //public event FragmentNavigationEventHandler FragmentNavigation;

        ///// <summary>
        ///// Occurs when a new navigation is requested.
        ///// </summary>
        //public event NavigatingCancelEventHandler Navigating;

        ///// <summary>
        ///// Occurs when a navigation is terminated by either calling the System.Windows.Controls.Frame.StopLoading()
        ///// method, or when a new navigation is requested while the current navigation
        ///// is in progress.
        ///// </summary>
        //public event NavigationStoppedEventHandler NavigationStopped;

        /// OnApplyTemplate
        ///// <summary>
        ///// Builds the visual tree for the System.Windows.Controls.Frame when a new template
        ///// is applied.
        ///// </summary>
        //public override void OnApplyTemplate();

        ///// <summary>
        ///// Returns a System.Windows.Automation.Peers.FrameAutomationPeer for use by
        ///// the Silverlight automation infrastructure.
        ///// </summary>
        ///// <returns>
        ///// A System.Windows.Automation.Peers.FrameAutomationPeer for the System.Windows.Controls.Frame
        ///// object.
        ///// </returns>
        //protected override AutomationPeer OnCreateAutomationPeer();

        ///// <summary>
        ///// Reloads the current page.
        ///// </summary>
        //public void Refresh();

        ///// <summary>
        ///// Stops asynchronous navigations that have not yet been processed.
        ///// </summary>
        //public void StopLoading();

#endregion

        //todo: see if there isn't a more efficient way to do this.
        internal partial class FrameCache //Note: this class serves as an intermediate to manage the cache, but we could easily have done the same directly in the Frame class.
        {
#region two-way chained list type cache
            //internal int _cacheSize = 10;
            int _currentAmount = 0;

            internal FrameCacheItem _currentItem = null;
            FrameCacheItem firstItem = null;

            internal bool CanGoBack
            {
                get
                {
                    if (_currentItem == null)
                    {
                        return false;
                    }
                    else
                    {
                        return _currentItem._previous != null;
                    }
                }
            }
            internal bool CanGoForward
            {
                get
                {
                    if (_currentItem == null)
                    {
                        return false;
                    }
                    else
                    {
                        return _currentItem._next != null;
                    }
                }
            }

            /// <summary>
            /// Adds the item to the cache, and handles the cases where:
            /// - it is the first item,
            /// - we add it to the end
            /// - we add it somewhere else.
            /// </summary>
            /// <param name="next"></param>
            /// <param name="uri"></param>
            internal void Add(object next, Uri uri)
            {
                FrameCacheItem nextFrameCacheItem = new FrameCacheItem(next, uri);
                if (_currentItem != null)
                {
                    _currentItem.RemoveNext(); //we remove any item that was after the current item.
                    //we set the new item as the next of the current one:
                    _currentItem._next = nextFrameCacheItem;
                    nextFrameCacheItem._previous = _currentItem;
                }
                else //this is the first item in the cache:
                {
                    firstItem = nextFrameCacheItem;
                }
                //we set the new item as the current one.
                _currentItem = nextFrameCacheItem;
            }

#region Add when I thought CacheSize defined the maximum amount of elements in the chained list.
            //note: I kept it because the algorithm might be useful when we will have figured out what CacheSize means.

            ///// <summary>
            ///// Adds the item to the cache, and handles the cases where:
            ///// - it is the first item,
            ///// - we add it to the end
            ///// - we add it somewhere else.
            ///// </summary>
            ///// <param name="next"></param>
            //internal void Add(object next, Uri uri)
            //{
            //    FrameCacheItem nextFrameCacheItem = new FrameCacheItem(next, uri);
            //    if (_currentItem != null)
            //    {
            //        int amountRemoved = _currentItem.RemoveNext();
            //        _currentItem._next = nextFrameCacheItem;
            //        nextFrameCacheItem._previous = _currentItem;
            //        _currentAmount -= amountRemoved - 1; //-1 because we added back the nextFrameCacheItem.
            //        if (_currentAmount > _cacheSize)
            //        {
            //            if (_cacheSize > 1) //would be stupid but not impossible
            //            {
            //                //we remove the first item of the list, and give its position as the first item to its next.
            //                FrameCacheItem oldFirst = firstItem;
            //                firstItem = firstItem._next;
            //                oldFirst._next = null;
            //                oldFirst._value = null;
            //                firstItem._previous = null;
            //            }
            //            else
            //            {
            //                firstItem = nextFrameCacheItem;
            //            }
            //            _currentAmount = _cacheSize;
            //        }
            //    }
            //    else //this is the first item in the cache:
            //    {
            //        firstItem = nextFrameCacheItem;
            //        ++_currentAmount;
            //    }
            //    _currentItem = nextFrameCacheItem;
            //}
#endregion

            internal FrameCacheItem GoBack()
            {
                if (CanGoBack)
                {
                    _currentItem = _currentItem._previous;
                    return _currentItem;
                }
                else
                {
                    throw new InvalidOperationException("There are no entries in the back navigation history.");
                }
            }

            internal FrameCacheItem GoForward()
            {
                if (CanGoForward)
                {
                    _currentItem = _currentItem._next;
                    return _currentItem;
                }
                else
                {
                    throw new InvalidOperationException("There are no entries in the forward navigation history.");
                }
            }

            internal void Clear()
            {
                firstItem.RemoveNext();
                firstItem = null;
            }
#endregion


#region list type cache
            //List<object> _cache = new List<object>();
            //int currentIndex = 0;

            //private int _cacheSize = 10;
            ///// <summary>
            ///// Gets or sets the maximum amount of objects the cache can remember.
            ///// </summary>
            //public int CacheSize
            //{
            //    get { return _cacheSize; }
            //    set { _cacheSize = value; }
            //}

            //internal bool CanGoBack { get { return currentIndex != 0; } }
            //internal bool CanGoForward { get { return currentIndex != _cache.Count - 1; } }


            ///// <summary>
            ///// Adds the object right after the current position, and forgets the possible elements that were after it.
            ///// </summary>
            ///// <param name="next">The object to add.</param>
            //internal void Add(object next)
            //{
            //    if (currentIndex == _cacheSize - 1) //we are adding one element too much (at the end) so we need to remove the first element of the list:
            //    {
            //        _cache.RemoveAt(0);
            //        --currentIndex;
            //    }
            //    else if (currentIndex < _cache.Count - 1) //we are putting an element before at least one element so we need to remeve everything that is after the currentIndex:
            //    {
            //        _cache.RemoveRange(currentIndex + 1, _cache.Count - currentIndex - 1); // - 1 because we start from currentIndex + 1 (we do not want to remove the currentindex.
            //    }
            //    _cache.Add(next);
            //    ++currentIndex;
            //}

            //internal object GoBack()
            //{
            //    if (CanGoBack)
            //    {
            //        --currentIndex;
            //        return _cache.ElementAt(currentIndex);
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("There are no entries in the back navigation history.");
            //    }
            //}

            //internal object GoForward()
            //{
            //    if (CanGoForward)
            //    {
            //        ++currentIndex;
            //        return _cache.ElementAt(currentIndex);
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("There are no entries in the forward navigation history.");
            //    }
            //}
#endregion
        }

        internal partial class FrameCacheItem
        {
            internal FrameCacheItem _previous = null;
            internal FrameCacheItem _next = null;
            internal Uri _uri = null;
            internal object _value = null;

            internal FrameCacheItem(object value, Uri uri)
            {
                _value = value;
                _uri = uri;
            }

            /// <summary>
            /// removes the next element(s) if any.
            /// </summary>
            internal void RemoveNext()
            {
                if (_next != null)
                {
                    FrameCacheItem next = _next;
                    _next = null;
                    next._previous = null;
                    next._value = null; //releasing the element just in case to avoid any risk of memory leak.
                    next._uri = null;
                    next.RemoveNext(); //we remove all the next elements recursively.
                }
            }
#region RemoveNext when I thought CacheSize defined the maximum amount of elements in the chained list.
            //internal int RemoveNext()
            //{
            //    if (_next != null)
            //    {
            //        FrameCacheItem next = _next;
            //        _next = null;
            //        next._previous = null;
            //        next._value = null; //releasing the element just in case to avoid any risk of memory leak.
            //        next._uri = null;
            //        return 1 + next.RemoveNext(); //we removed the next of the current element (thus the "1 +"), and recursively the other ones.
            //    }
            //    return 0;
            //}
#endregion
        }
    }
}
