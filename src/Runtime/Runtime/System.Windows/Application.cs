
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

using CSHTML5.Internal;
using OpenSilver;
using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;
using OpenSilver.Theming;
using System.ApplicationModel.Activation;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using System.Xaml.Markup;

namespace System.Windows
{
    /// <summary>
    /// Encapsulates the app and its available services.
    /// </summary>
    public partial class Application : DispatcherObject, IResourceDictionaryOwner
    {
        private static readonly Dictionary<string, string> _resourcesCache = new(StringComparer.OrdinalIgnoreCase);
        private static Application _current;

        private readonly HtmlElementReference _rootDiv;
        private readonly ApplicationLifetimeObjectsCollection _lifetimeObjects = [];

        private Window _mainWindow;
        private ResourceDictionary _resources;
        private Dictionary<object, object> _implicitResourcesCache;
        private Host _host;
        private Theme _theme;
        private Uri _startupUri;

        /// <summary>
        /// Gets the Application object for the current application.
        /// </summary>
        public static Application Current => _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// More than one instance of the <see cref="Application"/> class is created per <see cref="AppDomain"/>.
        /// </exception>
        public Application()
            : this("opensilver-root")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="rootDivId"/> is null or the empty string.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// More than one instance of the <see cref="Application"/> class is created per <see cref="AppDomain"/>.
        /// </exception>
        public Application(string rootDivId)
        {
            ArgumentException.ThrowIfNullOrEmpty(rootDivId);

            if (Interlocked.CompareExchange(ref _current, this, null) is not null)
            {
                throw new InvalidOperationException(Strings.MultiSingleton);
            }

            _rootDiv = new(rootDivId);

            TextMeasurementService = new TextMeasurementService(rootDivId);

            // Initialize Deployment
            _ = Deployment.Current;
            // Ensure InputManager is created and register the root element for input events
            InputManager.Current.RegisterRoot(_rootDiv);

            AppParams = GetAppParams();

            DOMEvents.Window.AddEventListener("beforeunload", OnExitNative);

            // In case of a redirection from Microsoft AAD, when running in the Simulator, we re-instantiate the application. We need to reload the JavaScript files because they are no longer in the HTML DOM due to the AAD redirection:
            OpenSilver.Interop.ResetLoadedFilesDictionaries();

            // we change the resource manager for every resource registered
            ClientSideResourceRegister.Startup();

            // Keep a reference to the startup assembly:
            StartupAssemblyInfo.StartupAssembly = GetType().Assembly;

            // We call the "Startup" event and the "OnLaunched" method using the Dispatcher, because usually the user registers the "Startup" event in the constructor of the "App.cs" class, which is derived from "Application.cs", and therefore when we arrive here the event is not yet registered. Executing the code in the Dispatcher ensures that the constructor of the "App.cs" class has finished before running the code.
            Dispatcher.CurrentDispatcher.InvokeAsync(DoStartup);
        }

        // only for xaml.io designer
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Application(XamlDesignerConstructorStub _)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="OpenSilver.Theming.Theme"/> used across this <see cref="Application"/>.
        /// </summary>
        public Theme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme == value) return;

                _theme?.RemoveOwner(this);

                if (value is not null)
                {
                    value.Seal();
                    value.AddOwner(this);
                }

                _theme = value;
            }
        }

        /// <summary>
        /// Gets a collection of the <see cref="Window"/> instances that have been created.
        /// </summary>
        /// <returns>
        /// A collection of the windows used by the application.
        /// </returns>
        public WindowCollection Windows { get; } = [];

        /// <summary>
        /// Gets the application extension services that have been registered for this application.
        /// </summary>
        /// <returns>
        /// The registered services.
        /// </returns>
        public IList ApplicationLifetimeObjects => _lifetimeObjects;

        private void DoStartup()
        {
            StartServices();
            NotifyLifetimeAwareServicesStarting();
            DoStartupInternal();
            NotifyLifetimeAwareServicesStarted();

            OpenSilverCompatibilityPreferences.Seal();
        }

        private void DoStartupInternal()
        {
            OnStartup(new StartupEventArgs());

#pragma warning disable CS0618
            OnLaunched(new LaunchActivatedEventArgs());
#pragma warning restore CS0618

            if (StartupUri is null)
            {
                return;
            }

            if (LoadComponent(StartupUri) is not FrameworkElement component)
            {
                throw new InvalidOperationException(Strings.ApplicationRootMustBeFrameworkElement);
            }

            MainWindow = component switch
            {
                Window window => window,
                _ => new Window { Content = component },
            };
        }

        private void StartServices()
        {
            _lifetimeObjects.Close();

            for (int i = 0; i < _lifetimeObjects.Count;)
            {
                IApplicationService service = (IApplicationService)_lifetimeObjects[i];

                try
                {
                    service.StartService(new ApplicationServiceContext());
                    i++;
                }
                catch (Exception ex)
                {
                    _lifetimeObjects.RemoveServiceAt(i);
                    HandleException(ex);
                }
            }
        }

        private void NotifyLifetimeAwareServicesStarting()
        {
            foreach (IApplicationService service in _lifetimeObjects)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Starting();
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
            }
        }

        private void NotifyLifetimeAwareServicesStarted()
        {
            foreach (IApplicationService service in _lifetimeObjects)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Started();
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
            }
        }

        internal IDictionary<string, string> AppParams { get; }

        private struct HTMLParam
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private IDictionary<string, string> GetAppParams()
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            HTMLParam[] paramsArray;
            try
            {
                paramsArray = JsonSerializer.Deserialize<HTMLParam[]>(
                    OpenSilver.Interop.ExecuteJavaScriptString($"osjs.getAppParams('{_rootDiv.Uid}')"));
            }
            catch
            {
                paramsArray = [];
            }

            foreach (HTMLParam p in paramsArray)
            {
                if (p.Name != null)
                {
                    parameters[p.Name] = p.Value;
                }
            }

            return new ReadOnlyDictionary<string, string>(parameters);
        }

        /// <summary>
        /// Gets a collection of application-scoped resources, such as styles, templates,
        /// and brushes.
        /// </summary>
        [Ambient]
        public ResourceDictionary Resources
        {
            get
            {
                if (_resources is null)
                {
                    _resources = new ResourceDictionary();
                    _resources.AddOwner(this);
                }
                return _resources;
            }
            set
            {
                if (_resources == value) return;

                ResourceDictionary oldValue = _resources;
                _resources = value;

                // This app is no longer an owner for the old ResourceDictionary
                oldValue?.RemoveOwner(this);

                if (value != null)
                {
                    if (!value.ContainsOwner(this))
                    {
                        // This app is an owner for the new ResourceDictionary
                        value.AddOwner(this);
                    }
                }

                // this notify all window in the app that Application resources changed
                InvalidateResources(new ResourcesChangeInfo(oldValue, value));
            }
        }

        internal bool HasResources => _resources is not null && !_resources.IsEmpty;

        // Says if App.Resources has any implicit styles
        internal bool HasImplicitStylesInResources { get; set; }

        void IResourceDictionaryOwner.SetResources(ResourceDictionary resourceDictionary)
        {
            // Propagate the HasImplicitStyles flag to the new owner
            if (resourceDictionary.HasImplicitStyles)
            {
                HasImplicitStylesInResources = true;
            }
        }

        void IResourceDictionaryOwner.OnResourcesChange(ResourcesChangeInfo info, bool shouldInvalidate, bool hasImplicitStyles)
        {
            // Set the HasImplicitStyles flag on the owner
            if (hasImplicitStyles)
            {
                HasImplicitStylesInResources = true;
            }

            if (shouldInvalidate)
            {
                InvalidateResources(info);
            }
        }

        internal object FindResourceInternal(object resourceKey)
        {
            if (_resources is not null && _resources.TryGetResource(resourceKey, out object value))
            {
                return value;
            }

            if (_theme is Theme theme && theme.TryGetResource(resourceKey, out value))
            {
                return value;
            }

            return null;
        }

        internal object FindImplicitResource(object resourceKey)
        {
            if (_implicitResourcesCache is not null && _implicitResourcesCache.TryGetValue(resourceKey, out object resource))
            {
                return resource;
            }

            return null;
        }

        private void InvalidateResources(ResourcesChangeInfo info)
        {
            InvalidateImplicitResourcesCache(info);
            InvalidateResourceReferences(info);
        }

        private void InvalidateImplicitResourcesCache(ResourcesChangeInfo info)
        {
            if (info.Key is not null)
            {
                switch (info.Key)
                {
                    case Type:
                    case DataTemplateKey:
                        object resource = Resources[info.Key];
                        if (resource is null)
                        {
                            _implicitResourcesCache?.Remove(info.Key);
                        }
                        else
                        {
                            _implicitResourcesCache ??= [];
                            _implicitResourcesCache[info.Key] = resource;
                        }
                        break;
                }
            }
            else if (info.IsCatastrophicDictionaryChange || info.IsImplicitResourcesChange())
            {
                _implicitResourcesCache = HasResources && ResourceDictionary.Helpers.HasImplicitResources(Resources) ?
                    ResourceDictionary.Helpers.BuildImplicitResourcesCache(Resources) :
                    null;
            }
        }

        internal void InvalidateResourceReferences(ResourcesChangeInfo info)
        {
            foreach (Window window in Windows)
            {
                TreeWalkHelper.InvalidateOnResourcesChange(window, info);
            }
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform
        /// application initialization and to display initial content in the associated
        /// Window.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use OnStartup(StartupEventArgs) instead.")]
        protected virtual void OnLaunched(LaunchActivatedEventArgs args)
        {
        }

        /// <summary>
        /// Occurs when an application is started.
        /// </summary>
        public event StartupEventHandler Startup;

        /// <summary>
        /// Raises the <see cref="Startup"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="StartupEventArgs"/> that contains the event data.
        /// </param>
        /// <remarks>
        /// <see cref="OnStartup"/> raises the <see cref="Startup"/> event.
        /// A type that derives from <see cref="Application"/> may override <see cref="OnStartup"/>. The overridden method 
        /// must call <see cref="OnStartup"/> in the base class if the <see cref="Startup"/> event needs to be raised.
        /// </remarks>
        protected virtual void OnStartup(StartupEventArgs e) => Startup?.Invoke(this, e);

        /// <summary>
        /// Occurs when an exception that is raised is not handled.
        /// </summary>
        public event EventHandler<ApplicationUnhandledExceptionEventArgs> UnhandledException;

        internal static bool CallHandleException(Exception exception) => Current is Application app && app.HandleException(exception);

        internal bool HandleException(Exception exception)
        {
            if (UnhandledException is EventHandler<ApplicationUnhandledExceptionEventArgs> handler)
            {
                var args = new ApplicationUnhandledExceptionEventArgs(exception, false);
                handler(this, args);
                return args.Handled;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the main application UI. This is an alias for the 
        /// <see cref="Window.Content"/> of this application's <see cref="MainWindow"/>.
        /// </summary>
        public UIElement RootVisual
        {
            get => MainWindow?.Content;
            set
            {
                if (value is not FrameworkElement rootVisual)
                {
                    throw new ArgumentException(Strings.ApplicationRootMustBeFrameworkElement, nameof(value));
                }

                (MainWindow ??= new Window()).Content = rootVisual;
            }
        }

        /// <summary>
        /// Gets or sets a UI that is automatically shown when an application starts.
        /// </summary>
        /// <returns>
        /// A <see cref="Uri"/> that refers to the UI that automatically opens when an application starts.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// StartupUri is set with a value of null.
        /// </exception>
        public Uri StartupUri
        {
            get => _startupUri;
            set => _startupUri = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the main application window.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// MainWindow is set with a value of null.
        /// </exception>
        public Window MainWindow
        {
            get => _mainWindow;
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                if (value == _mainWindow)
                {
                    return;
                }

                Window oldMainWindow = _mainWindow;

                Window.Current = _mainWindow = value;

                if (!_mainWindow.HasOverlayInfrastructure)
                {
                    // Window hasn't been shown yet — show it now.
                    _mainWindow.Show();
                }

                MainWindowReady?.Invoke(this, EventArgs.Empty);
            }
        }

        internal event EventHandler MainWindowReady;

        internal HtmlElementReference GetRootDiv() => _rootDiv;

        internal TextMeasurementService TextMeasurementService { get; }

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
            if (_resourcesCache.TryGetValue(uriResource.OriginalString, out string content))
            {
                return Task.FromResult(content);
            }

            string uriAsString = uriResource.OriginalString;
            string extension = uriAsString.Substring(uriAsString.LastIndexOf('.'));

            if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".config", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, ".clientconfig", StringComparison.OrdinalIgnoreCase))
            {
                var tcs = new TaskCompletionSource<string>();

                var uris = new List<string>(1)
                {
                    uriAsString + ".g.js"
                };

                if (string.Equals(uriResource.OriginalString, "ms-appx://app.config", StringComparison.OrdinalIgnoreCase))
                {
                    OpenSilver.Interop.LoadJavaScriptFilesAsync(
                        uris,
                        () => tcs.SetResult(OpenSilver.Interop.ExecuteJavaScriptString("window.AppConfig")));
                }
                else if (string.Equals(uriResource.OriginalString, "ms-appx://servicereferences.clientconfig", StringComparison.OrdinalIgnoreCase))
                {
                    OpenSilver.Interop.LoadJavaScriptFilesAsync(
                        uris,
                        () => tcs.SetResult(OpenSilver.Interop.ExecuteJavaScriptString("window.ServiceReferencesClientConfig")));
                }
                else
                {
                    OpenSilver.Interop.LoadJavaScriptFilesAsync(
                        uris,
                        () =>
                        {
                            string result = OpenSilver.Interop.ExecuteJavaScriptString("window.FileContent");
                            _resourcesCache.Add(uriResource.OriginalString.ToLower(), result);
                            tcs.SetResult(result);
                        });
                }

                return tcs.Task;
            }

            return Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Searches for a user interface (UI) resource, such as a <see cref="Style"/> or <see cref="Brush"/>, with the specified key,
        /// and throws an exception if the requested resource is not found (see XAML Resources).
        /// </summary>
        /// <param name="resourceKey">
        /// The name of the resource to find.
        /// </param>
        /// <returns>
        /// The requested resource object. If the requested resource is not found, a <see cref="ResourceReferenceKeyNotFoundException"/>
        /// is thrown.
        /// </returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException">
        /// The resource cannot be found.
        /// </exception>
        public object FindResource(object resourceKey)
        {
            if (TryFindResource(resourceKey) is object resource)
            {
                return resource;
            }

            throw new ResourceReferenceKeyNotFoundException(Strings.MarkupExtensionResourceNotFound, resourceKey);
        }

        /// <summary>
        /// Searches for the specified resource.
        /// </summary>
        /// <param name="resourceKey">
        /// The name of the resource to find.
        /// </param>
        /// <returns>
        /// The requested resource object. If the requested resource is not found, a null reference is returned.
        /// </returns>
        public object TryFindResource(object resourceKey)
        {
            if (HasResources && Resources.TryGetResource(resourceKey, out object resource))
            {
                return resource;
            }

            if (XamlResources.FindResourceInGenericXaml(resourceKey) is object genericResource)
            {
                return genericResource;
            }

            return XamlResources.FindBuiltInResource(resourceKey);
        }

        /// <summary>
        /// Loads a XAML file that is located at the specified uniform resource identifier (URI) and 
        /// converts it to an instance of the object that is specified by the root element of the XAML
        /// file.
        /// </summary>
        /// <param name="component">
        /// An object of the same type as the root element of the XAML file.
        /// </param>
        /// <param name="resourceLocator">
        /// A <see cref="Uri"/> that maps to a relative XAML file.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="component"/> or <paramref name="resourceLocator"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="resourceLocator"/> is an absolute URI.
        /// </exception>
        /// <exception cref="Exception">
        /// <paramref name="component"/> is of a type that does not match the root element of the XAML file.
        /// </exception>
        public static void LoadComponent(object component, Uri resourceLocator)
        {
            ArgumentNullException.ThrowIfNull(component);
            ArgumentNullException.ThrowIfNull(resourceLocator);

            if (GetXamlComponentLoader(resourceLocator) is IXamlComponentLoader loader)
            {
                loader.LoadComponent(component);
            }
        }

        /// <summary>
        /// Loads a XAML file that is located at the specified uniform resource identifier (URI) and 
        /// converts it to an instance of the object that is specified by the root element of the XAML
        /// file.
        /// </summary>
        /// <param name="component">
        /// An object of the same type as the root element of the XAML file.
        /// </param>
        /// <param name="loader">
        /// A <see cref="Uri"/> that maps to a relative XAML file.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="component"/> or <paramref name="loader"/> is null.
        /// </exception>
        /// <exception cref="Exception">
        /// <paramref name="component"/> is of a type that does not match the root element of the XAML file.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void LoadComponent(object component, IXamlComponentLoader loader)
        {
            ArgumentNullException.ThrowIfNull(component);
            ArgumentNullException.ThrowIfNull(loader);

            loader.LoadComponent(component);
        }

        /// <summary>
        /// Loads a XAML file that is located at the specified uniform resource identifier (URI), and 
        /// converts it to an instance of the object that is specified by the root element of the XAML 
        /// file.
        /// </summary>
        /// <param name="resourceLocator">
        /// A System.Uri that maps to a relative XAML file.
        /// </param>
        /// <returns>
        /// An instance of the root element specified by the XAML file loaded.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceLocator"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The resourceLocator is an absolute URI.
        /// </exception>
        /// <exception cref="Exception">
        /// The file is not a XAML file.
        /// </exception>
        public static object LoadComponent(Uri resourceLocator)
        {
            ArgumentNullException.ThrowIfNull(resourceLocator);

            if (GetXamlComponentLoader(resourceLocator) is not IXamlComponentFactory factory)
            {
                throw new InvalidOperationException(string.Format(Strings.UnableToLocateResource, resourceLocator));
            }

            return factory.CreateComponent();
        }

        private static object GetXamlComponentLoader(Uri resourceLocator)
        {
            Debug.Assert(resourceLocator is not null);

            if (resourceLocator.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.AbsoluteUriNotAllowed);
            }

            if (GetXamlComponentLoaderType(resourceLocator.ToString()) is Type loaderType)
            {
                return Activator.CreateInstance(loaderType);
            }

            return null;
        }

        internal static Type GetXamlComponentLoaderType(string componentUri)
        {
            if (AppResourcesManager.TryResolveUri(componentUri, out string resolvedUri))
            {
                string className = XamlResourcesHelper.GenerateClassNameFromComponentUri(resolvedUri);
                string assemblyName = AppResourcesManager.ExtractAssemblyNameFromComponentUri(resolvedUri);

                return Type.GetType($"{className}, {assemblyName}");
            }

            return null;
        }

        /// <summary>
        /// Returns a resource file from a location in the application package.
        /// </summary>
        /// <param name="uriResource">
        /// A relative URI that identifies the resource file to be loaded. The URI is relative
        /// to the application package and does not need a leading forward slash.
        /// </param>
        /// <returns>
        /// A <see cref="StreamResourceInfo"/> that contains the stream for the desired resource 
        /// file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// uriResource is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// uriResource is an absolute URI.
        /// </exception>
        public static Task<StreamResourceInfo> GetResourceStream(Uri uriResource)
        {
            ArgumentNullException.ThrowIfNull(uriResource);

            if (uriResource.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.AbsoluteUriNotAllowed);
            }

            if (AppResourcesManager.GetResourceStream(uriResource.ToString()) is Stream stream)
            {
                return Task.FromResult(new StreamResourceInfo(stream, null));
            }

            return Task.FromResult<StreamResourceInfo>(null);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler INTERNAL_Reloaded;

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void INTERNAL_RaiseReloadedEvent()
        {
            EventHandler handler = INTERNAL_Reloaded;
            if (handler != null)
            {
                handler(null, null);
            }
        }


        #region Exit event

        /// <summary>
        /// Occurs just before an application shuts down.
        /// </summary>
        public event ExitEventHandler Exit;

        /// <summary>
        /// Raises the <see cref="Exit"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="ExitEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnExit(ExitEventArgs e) => Exit?.Invoke(this, e);

        private void OnExitNative(object jsEventArg)
        {
            var e = new ExitEventArgs(0);

            try
            {
                OnExit(e);

                foreach (Window window in Windows)
                {
                    if (window.InvokeOnClosing(true))
                    {
                        e.Handled = true;
                    }
                }
            }
            finally
            {
                Environment.ExitCode = e.ExitCode;

                if (e.Handled)
                {
                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        $"{OpenSilver.Interop.GetVariableStringForJS(jsEventArg)}.preventDefault()");
                }
                else
                {
                    StopApplicationServices();
                }
            }
        }

        private void StopApplicationServices()
        {
            // Note: Silverlight invokes the Exiting method before firing the Exit event. However, since
            // we allow cancellation of the Exit event, we need to call this method after the Exit event,
            // because we only want to stop the services if the event was not cancelled.
            foreach (IApplicationService service in _lifetimeObjects)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Exiting();
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
            }

            foreach (IApplicationService service in _lifetimeObjects)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Exited();
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }
            }

            // Note: Silverlight stops the services in reverse order of their registration.
            for (int i = _lifetimeObjects.Count - 1; i >= 0; i--)
            {
                IApplicationService service = (IApplicationService)_lifetimeObjects[i];

                try
                {
                    service.StopService();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets various details about the application's host.
        /// </summary>
        public Host Host => _host ??= new Host(this);

        /// <summary>
        /// The entry point of the application needs to be wrapped by this method
        /// to ensure correction functioning of the application.
        /// </summary>
        /// <param name="entryPoint"></param>
        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RunApplication(Action entryPoint) => entryPoint();
    }
}
