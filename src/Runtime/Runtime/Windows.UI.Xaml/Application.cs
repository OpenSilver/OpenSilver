

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


using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Resources;
using System.IO;
using System.Text;
using System.Reflection;
using DotNetForHtml5.Core;

#if MIGRATION
using System.ApplicationModel.Activation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
#else
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Encapsulates the app and its available services.
    /// </summary>
    public partial class Application
    {
        // Id of the application root html div.
        public const string ApplicationRootDomElementId = "cshtml5-root";

        static Dictionary<string, string> _resourcesCache = null;
        INTERNAL_XamlResourcesHandler _xamlResourcesHandler = new INTERNAL_XamlResourcesHandler();

        // Says if App.Resources has any implicit styles
        internal bool HasImplicitStylesInResources { get; set; }

        /// <summary>
        /// Gets the Application object for the current application.
        /// </summary>
        public static Application Current { get; set; }

        internal INTERNAL_XamlResourcesHandler XamlResourcesHandler { get { return _xamlResourcesHandler; } }

        Window _mainWindow;
        ResourceDictionary _resources;

#if WORKINPROGRESS
        public ITextMeasurementService TextMeasurementService { get; private set; }
#endif
        public Application()
        {
            // In case of a redirection from Microsoft AAD, when running in the Simulator, we re-instantiate the application. We need to reload the JavaScript files because they are no longer in the HTML DOM due to the AAD redirection:
            INTERNAL_InteropImplementation.ResetLoadedFilesDictionaries();

#if CSHTML5BLAZOR
            // we change the resource manager for every resource registered
            ClientSideResourceRegister.Startup();
#endif
            // Keep a reference to the startup assembly:
            StartupAssemblyInfo.StartupAssembly = this.GetType().Assembly;

            // Remember whether we are in "SL Migration" mode or not:
#if MIGRATION
            CSHTML5.Interop.ExecuteJavaScript(@"document.isSLMigration = true");
#else
            CSHTML5.Interop.ExecuteJavaScript(@"document.isSLMigration = false");
#endif

            //Interop.ExecuteJavaScript("document.raiseunhandledException = $0", (Action<object>)RaiseUnhandledException);


            // Inject the "DataContractSerializer" into the "XmlSerializer" (read note in the "XmlSerializer" implementation to understand why):
            if (!CSHTML5.Interop.IsRunningInTheSimulator) //Note: in case of the Simulator, we reference the .NET Framework version of "System.xml.dll", so we cannot inject stuff because the required members of XmlSerializer would be missing.
            {
                InjectDataContractSerializerIntoXmlSerializer();
            }

#if !CSHTML5NETSTANDARD
            // Fix the freezing of the Simulator when calling 'alert' using the "Interop.ExecuteJavaScript()" method by redirecting the JavaScript "alert" to the Simulator message box:
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                RedirectAlertToMessageBox_SimulatorOnly();
            }
#endif

            // Keep a reference to the app:
            Application.Current = this;

            // Get default font-family from css
            INTERNAL_FontsHelper.DefaultCssFontFamily = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("window.getComputedStyle(document.getElementsByTagName('body')[0]).getPropertyValue(\"font-family\")"));

#if WORKINPROGRESS
            TextMeasurementService = new TextMeasurementService();
#endif
            // Initialize the window:
            if (_mainWindow == null) // Note: it could be != null if the user clicks "Restart" from the Simulator advanced options.
            {
                _mainWindow = new Window();
                Window.Current = _mainWindow;
                object applicationRootDomElement = INTERNAL_HtmlDomManager.GetApplicationRootDomElement();
                _mainWindow.AttachToDomElement(applicationRootDomElement);

                // Listen to clicks anywhere in the window (this is used to close the popups that are not supposed to stay open):
#if MIGRATION
                _mainWindow.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow), true);
#else
                _mainWindow.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(INTERNAL_PopupsManager.OnClickOnPopupOrWindow), true);
#endif

#if !CSHTML5NETSTANDARD
                // Workaround an issue on Firefox where the UI disappears if the window is resized and on some other occasions:
                if (INTERNAL_HtmlDomManager.IsFirefox())
                {
                    _mainWindow.SizeChanged += MainWindow_SizeChanged;
                    _timerForWorkaroundFireFoxIssue.Interval = new TimeSpan(0, 0, 2);
                    _timerForWorkaroundFireFoxIssue.Tick += TimerForWorkaroundFireFoxIssue_Tick;
                }
#endif
            }

            // We call the "Startup" event and the "OnLaunched" method using the Dispatcher, because usually the user registers the "Startup" event in the constructor of the "App.cs" class, which is derived from "Application.cs", and therefore when we arrive here the event is not yet registered. Executing the code in the Dispatcher ensures that the constructor of the "App.cs" class has finished before running the code.
#if MIGRATION
            Dispatcher
#else
            CoreDispatcher
#endif
                .INTERNAL_GetCurrentDispatcher().BeginInvoke((Action)(() =>
            {
                // Raise the "Startup" event:
                if (this.Startup != null)
                    Startup(this, new StartupEventArgs());

                // Call the "OnLaunched" method:
                this.OnLaunched(new LaunchActivatedEventArgs());
            }));

        }

#region Work around an issue on Firefox where the UI disappears if the window is resized and on some other occasions:

#if !CSHTML5NETSTANDARD
        DispatcherTimer _timerForWorkaroundFireFoxIssue = new DispatcherTimer();
        ChildWindow _childWindowForWorkaroundFireFoxIssue;

        void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            INTERNAL_WorkAroundFirefoxDisappearingUI();
        }

        public void INTERNAL_WorkAroundFirefoxDisappearingUI() //todo: move this method to the "PublicAPI" folder?
        {
            // Work around an issue on Firefox where the UI disappears if the window is resized and on some other occasions:
            if (_childWindowForWorkaroundFireFoxIssue == null) // Avoids entering multiple times on a single resize.
            {
                _childWindowForWorkaroundFireFoxIssue = new ChildWindow() { Opacity = 0 };
                _childWindowForWorkaroundFireFoxIssue.Show();
                _timerForWorkaroundFireFoxIssue.Start();
            }
        }

#if MIGRATION
        void TimerForWorkaroundFireFoxIssue_Tick(object sender, EventArgs e)
#else
        void TimerForWorkaroundFireFoxIssue_Tick(object sender, object e)
#endif
        {
            // Work around an issue on Firefox where the UI disappears if the window is resized and on some other occasions:
            _childWindowForWorkaroundFireFoxIssue.Dispatcher.BeginInvoke(() =>
            {
                _timerForWorkaroundFireFoxIssue.Stop();
                _childWindowForWorkaroundFireFoxIssue.Close();
                _childWindowForWorkaroundFireFoxIssue = null;
            });
        }
#endif

#endregion

        /// <summary>
        /// Injects the "DataContractSerializer" into the "XmlSerializer" (read note in the "XmlSerializer" implementation to understand why).
        /// </summary>
        static void InjectDataContractSerializerIntoXmlSerializer()
        {

#if !FOR_DESIGN_TIME && !CSHTML5NETSTANDARD && !BRIDGE //Note: in case of "Design Time", we reference the .NET Framework version of "System.xml.dll", so the code below would not compile because the specified members of XmlSerializer would be missing.
            XmlSerializer.MethodToSerializeUsingDataContractSerializer = (obj, type) => { return (new DataContractSerializer(type, useXmlSerializerFormat: true)).SerializeToString(obj); };
            XmlSerializer.MethodToDeserializeUsingDataContractSerializer = (str, type) => { return (new DataContractSerializer(type, useXmlSerializerFormat: true)).DeserializeFromString(str); };
#endif
        }

#if !CSHTML5NETSTANDARD
#if !BRIDGE
        [JSIL.Meta.JSIgnore]
#else
        [Bridge.External]
#endif
        static void RedirectAlertToMessageBox_SimulatorOnly()
        {
            // Fix the freezing of the Simulator when calling 'alert' using the "Interop.ExecuteJavaScript()" method by redirecting the JavaScript "alert" to the Simulator message box:
            CSHTML5.Interop.ExecuteJavaScript(@"window.alert =  function(msg){ $0(msg); }", (Action<object>)((msg) => { MessageBox.Show(msg.ToString()); }));
        }
#endif

        /// <summary>
        /// Gets a collection of application-scoped resources, such as styles, templates,
        /// and brushes.
        /// </summary>
        public ResourceDictionary Resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = new ResourceDictionary();
                    _resources.AddOwner(this);
                }
                return _resources;
            }
            set
            {
                ResourceDictionary oldValue = _resources;
                _resources = value;

                if (oldValue != null)
                {
                    // This app is no longer an owner for the old ResourceDictionary
                    oldValue.RemoveOwner(this);
                }

                if (value != null)
                {
                    if (!value.ContainsOwner(this))
                    {
                        // This app is an owner for the new ResourceDictionary
                        value.AddOwner(this);
                    }
                }

                // todo: implement this.
                // this notify all window in the app that Application resources changed
                //if (oldValue != value)
                //{
                //    InvalidateResourceReferences(new ResourcesChangeInfo(oldValue, value));
                //}
            }
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform
        /// application initialization and to display initial content in the associated
        /// Window.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected virtual void OnLaunched(LaunchActivatedEventArgs args)
        {
        }

        /// <summary>
        /// Occurs when an application is started.
        /// </summary>
        public event StartupEventHandler Startup;

        //protected virtual void InitializeComponent()
        //{
        //}

        /// <summary>
        /// Gets or sets the main application UI. This is an alias for: Window.Current.Content
        /// </summary>
        public UIElement RootVisual
        {
            get
            {
#if WORKINPROGRESS
                return Window.Current.Content;
#else
                return (Window.Current.Content as UIElement);
#endif
            }
            set
            {
#if WORKINPROGRESS
                Window.Current.Content = value as FrameworkElement;
#else
                Window.Current.Content = value;
#endif
            }
        }

        /// <summary>
        /// Occurs just before an application shuts down and cannot be canceled.
        /// </summary>
        //public event EventHandler Exit;

        //returns the html window element
        internal object GetWindow()
        {
            return CSHTML5.Interop.ExecuteJavaScript(@"window");
        }

        /// <summary>
        /// Gets the application main window.
        /// </summary>
        public Window MainWindow
        {
            get
            {
                return Window.Current;
            }
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     is null.
        //
        //   System.ArgumentException:
        //     The System.Uri.OriginalString property of the System.Uri that is passed to
        //     System.Windows.Application.GetResourceStream(System.Uri) is null.
        //
        //   System.ArgumentException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     is either not relative, or is absolute but not in the pack://application:,,,/
        //     form.
        //
        //   System.IO.IOException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     cannot be found.
        /// <summary>
        /// Returns a string that contains the content of the file that is located at the
        /// specified System.Uri.
        /// </summary>
        /// <param name="uriResource">The System.Uri that maps to an embedded resource.</param>
        /// <returns>
        /// A string that contains the content of the file that is located at the specified System.Uri.
        /// </returns>
        public static Task<string> GetResourceString(Uri uriResource)
        {
            if (_resourcesCache == null)
            {
                _resourcesCache = new Dictionary<string, string>();
            }
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            if (_resourcesCache.ContainsKey(uriResource.OriginalString.ToLower()))
            {
                tcs.SetResult(_resourcesCache[uriResource.OriginalString.ToLower()]);
                return tcs.Task;
            }
            HashSet2<string> supportedExtensions = new HashSet2<string>(new string[] { ".txt", ".xml", ".config", ".json", ".clientconfig" });

            string uriAsString = uriResource.OriginalString;
            string extension = uriAsString.Substring(uriAsString.LastIndexOf('.'));
            if (!supportedExtensions.Contains(extension.ToLower())) //todo: when we will be able to handle more extensions, add them to supportedExtensions and do not forget to update GetResourceStream as well.
            {
                throw new NotSupportedException("Application.GetResourceString is currently not supported for files with an extension different than .txt, .xml, .json, .config, or .clientconfig.");
            }
            List<string> uris = new List<string>();
            uris.Add(uriAsString + ".g.js");
            if (uriResource.OriginalString.ToLower() == "ms-appx://app.config")
            {
                CSHTML5.Interop.LoadJavaScriptFilesAsync(
                    uris,
                    (Action)(() =>
                    {
                        tcs.SetResult(Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("window.AppConfig")));
                    })
                    );
            }
            else if (uriResource.OriginalString.ToLower() == "ms-appx://servicereferences.clientconfig")
            {
                CSHTML5.Interop.LoadJavaScriptFilesAsync(
                    uris,
                    (Action)(() =>
                    {
                        tcs.SetResult(Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("window.ServiceReferencesClientConfig")));
                    })
                    );
            }
            else
            {
                CSHTML5.Interop.LoadJavaScriptFilesAsync(
                    uris,
                    (Action)(() =>
                    {
                        string result = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("window.FileContent"));
                        _resourcesCache.Add(uriResource.OriginalString.ToLower(), result);
                        tcs.SetResult(result);
                    })
                    );
            }
            //return Convert.ToString(Interop.ExecuteJavaScript("window.FileContent"));
            return tcs.Task;
        }

        /// <summary>
        /// Searches for the specified resource.
        /// </summary>
        /// <param name="resourceKey">The name of the resource to find.</param>
        /// <returns>
        /// The requested resource object. If the requested resource is not found, a
        /// null reference is returned.
        /// </returns>
        public object TryFindResource(object resourceKey)
        {
            return _xamlResourcesHandler.TryFindResource(resourceKey);
        }



        // Exceptions:
        //   System.ArgumentNullException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     is null.
        //
        //   System.ArgumentException:
        //     The System.Uri.OriginalString property of the System.Uri that is passed to
        //     System.Windows.Application.GetResourceStream(System.Uri) is null.
        //
        //   System.ArgumentException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     is either not relative, or is absolute but not in the pack://application:,,,/
        //     form.
        //
        //   System.IO.IOException:
        //     The System.Uri that is passed to System.Windows.Application.GetResourceStream(System.Uri)
        //     cannot be found.
        /// <summary>
        /// Returns a resource stream for a resource data file that is located at the
        /// specified System.Uri (see WPF Application Resource, Content, and Data Files).
        /// </summary>
        /// <param name="uriResource">The System.Uri that maps to an embedded resource.</param>
        /// <returns>
        /// A System.Windows.Resources.StreamResourceInfo that contains a resource stream
        /// for resource data file that is located at the specified System.Uri.
        /// </returns>
        public static async Task<StreamResourceInfo> GetResourceStream(Uri uriResource)
        {
            string resourceString = await GetResourceString(uriResource);
            string uriAsString = uriResource.OriginalString;
            string extensionLowercase = uriAsString.Substring(uriAsString.LastIndexOf('.')).ToLower();

            byte[] byteArray = Encoding.ASCII.GetBytes(resourceString);
            MemoryStream stream = new MemoryStream(byteArray);

            string mimeType = "text/plain";
            if (extensionLowercase == ".xml" || extensionLowercase == ".config" || extensionLowercase == ".clientconfig")
            {
                mimeType = "application/xml";
            }
            else if (extensionLowercase == ".json")
            {
                mimeType = "application/json";
            } //todo: update this when more extensions will be handled

            StreamResourceInfo resourceInfo = new StreamResourceInfo(stream, mimeType);
            return resourceInfo;
        }

        public static event EventHandler INTERNAL_Reloaded;

        /// <summary>
        /// Intended to be called by the Simulator when navigating back from an external page.
        /// </summary>
        public static void INTERNAL_RaiseReloadedEvent()
        {
            EventHandler handler = INTERNAL_Reloaded;
            if (handler != null)
            {
                handler(null, null);
            }
        }


#region Exit event

        INTERNAL_EventManager<EventHandler, EventArgs> _ExitEventManager;
        INTERNAL_EventManager<EventHandler, EventArgs> ExitEventManager
        {
            get
            {
                if (_ExitEventManager == null)
                {
                    _ExitEventManager = new INTERNAL_EventManager<EventHandler, EventArgs>(GetWindow, "unload", ProcessOnExit);
                }
                return _ExitEventManager;
            }
        }

        /// <summary>
        /// Occurs when an otherwise unhandled Tap interaction occurs over the hit test
        /// area of this element.
        /// </summary>
        public event EventHandler Exit
        {
            add
            {
                ExitEventManager.Add(value);
            }
            remove
            {
                ExitEventManager.Remove(value);
            }
        }

        /// <summary>
        /// Raises the Exit event
        /// </summary>
        void ProcessOnExit(object jsEventArg)
        {
            var eventArgs = new EventArgs();
            OnExit(eventArgs);
        }

        /// <summary>
        /// Raises the Exit event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnExit(EventArgs eventArgs)
        {
            foreach (EventHandler handler in _ExitEventManager.Handlers)//.ToList<EventHandler>())
            {
                handler(this, eventArgs);
            }
        }

#endregion

        static Host _host;

        /// <summary>
        /// Gets various details about the application's host.
        /// </summary>
        public Host Host
        {
            get
            {
                if (_host == null)
                    _host = new Host();
                return _host;
            }
        }

#if CSHTML5NETSTANDARD
        /// <summary>
        /// The entry point of the application needs to be wrapped by this method
        /// to ensure correction functioning of the application.
        /// </summary>
        /// <param name="entryPoint"></param>
        public static void RunApplication(Action entryPoint)
        {
            // (See the comments in the definition of the following method for more information on the purpose)
            INTERNAL_SimulatorExecuteJavaScript.RunActionThenExecutePendingAsyncJSCodeExecutedDuringThatAction
                (
                entryPoint
                );
        }
#endif
    }
}
