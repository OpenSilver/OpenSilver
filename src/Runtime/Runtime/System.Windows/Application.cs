
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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Resources;
using OpenSilver.Internal;
using OpenSilver.Internal.Xaml;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Xaml.Markup;
using System.ApplicationModel.Activation;
using System.Windows.Input;
using System.Windows.Threading;
using CSHTML5.Internal;
using OpenSilver.Theming;

namespace System.Windows
{
    /// <summary>
    /// Encapsulates the app and its available services.
    /// </summary>
    public partial class Application : IResourceDictionaryOwner
    {
        private static readonly Dictionary<string, string> _resourcesCache = new(StringComparer.OrdinalIgnoreCase);

        private readonly Window _mainWindow;
        private readonly INTERNAL_HtmlDomElementReference _rootDiv;

        private ApplicationLifetimeObjectsCollection _lifetimeObjects;
        private ResourceDictionary _resources;
        private Dictionary<object, object> _implicitResourcesCache;
        private Host _host;
        private Theme _theme;

        /// <summary>
        /// Gets the Application object for the current application.
        /// </summary>
        public static Application Current { get; private set; }

        public Application()
            : this("opensilver-root")
        {
        }

        public Application(string rootDivId)
        {
            if (string.IsNullOrEmpty(rootDivId))
            {
                throw new ArgumentNullException(nameof(rootDivId));
            }

            _rootDiv = new(rootDivId);

            // Keep a reference to the app:
            Current = this;

            // Initialize Deployment
            _ = Deployment.Current;
            // Ensure InputManager is created
            _ = InputManager.Current;

            AppParams = GetAppParams();

            DOMEvents.Window.AddEventListener("beforeunload", OnExitNative);

            // In case of a redirection from Microsoft AAD, when running in the Simulator, we re-instantiate the application. We need to reload the JavaScript files because they are no longer in the HTML DOM due to the AAD redirection:
            OpenSilver.Interop.ResetLoadedFilesDictionaries();

            // we change the resource manager for every resource registered
            ClientSideResourceRegister.Startup();

            // Keep a reference to the startup assembly:
            StartupAssemblyInfo.StartupAssembly = GetType().Assembly;

            Window.Current = _mainWindow = new Window();
            _mainWindow.AttachToDomElement(_rootDiv);

            // We call the "Startup" event and the "OnLaunched" method using the Dispatcher, because usually the user registers the "Startup" event in the constructor of the "App.cs" class, which is derived from "Application.cs", and therefore when we arrive here the event is not yet registered. Executing the code in the Dispatcher ensures that the constructor of the "App.cs" class has finished before running the code.
            Dispatcher.CurrentDispatcher.InvokeAsync(DoStartup);
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
        public IList ApplicationLifetimeObjects => PrivateApplicationLifetimeObjects;

        private ApplicationLifetimeObjectsCollection PrivateApplicationLifetimeObjects => _lifetimeObjects ??= [];

        private void DoStartup()
        {
            ApplicationLifetimeObjectsCollection services = PrivateApplicationLifetimeObjects;
            services.Close();

            for (int i = 0; i < services.Count;)
            {
                IApplicationService service = (IApplicationService)services[i];

                try
                {
                    service.StartService(new ApplicationServiceContext());
                    i++;
                }
                catch (Exception ex)
                {
                    services.RemoveServiceAt(i);
                    OnUnhandledException(ex, false);
                }
            }

            foreach (IApplicationService service in services)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Starting();
                    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex, false);
                    }
                }
            }

            OnStartup(new StartupEventArgs());

            OnLaunched(new LaunchActivatedEventArgs());

            foreach (IApplicationService service in services)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Started();
                    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex, false);
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
                string sElement = OpenSilver.Interop.GetVariableStringForJS(_rootDiv);
                paramsArray = JsonSerializer.Deserialize<HTMLParam[]>(
                    OpenSilver.Interop.ExecuteJavaScriptString($"document.getAppParams({sElement})"));
            }
            catch
            {
                paramsArray = Array.Empty<HTMLParam>();
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
                            _implicitResourcesCache ??= new();
                            _implicitResourcesCache[info.Key] = resource;
                        }
                        break;
                }
            }
            else if (info.IsCatastrophicDictionaryChange ||
                (info.NewDictionary != null && ResourceDictionary.Helpers.HasImplicitResources(info.NewDictionary)) ||
                (info.OldDictionary != null && ResourceDictionary.Helpers.HasImplicitResources(info.OldDictionary)))
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
        /// Gets or sets the main application UI. This is an alias for the 
        /// <see cref="Window.Content"/> of this application's <see cref="MainWindow"/>.
        /// </summary>
        public UIElement RootVisual
        {
            get => _mainWindow.Content;
            set => _mainWindow.Content = value as FrameworkElement;
        }

        internal INTERNAL_HtmlDomElementReference GetRootDiv() => _rootDiv;

        /// <summary>
        /// Gets the application main window.
        /// </summary>
        public Window MainWindow => _mainWindow;

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
        /// Searches for the specified resource.
        /// </summary>
        /// <param name="resourceKey">The name of the resource to find.</param>
        /// <returns>
        /// The requested resource object. If the requested resource is not found, a
        /// null reference is returned.
        /// </returns>
        public object TryFindResource(object resourceKey)
        {
            if (resourceKey is Type typeKey && XamlResources.FindStyleResourceInGenericXaml(typeKey) is object resource)
            {
                return resource;
            }

            if (HasResources && Resources.TryGetResource(resourceKey, out resource))
            {
                return resource;
            }

            return XamlResources.FindBuiltInResource(resourceKey);
        }

        /// <summary>
        /// Create logic tree from given resource Locator, and associate this
        /// tree with the given component.
        /// </summary>
        /// <param name="component">Root Element</param>
        /// <param name="resourceLocator">Resource Locator</param>
        public static void LoadComponent(object component, Uri resourceLocator)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (resourceLocator is null)
            {
                throw new ArgumentNullException(nameof(resourceLocator));
            }

            if (resourceLocator.IsAbsoluteUri)
            {
                throw new ArgumentException("Uri must be relative.");
            }

            string resourceUri = resourceLocator.ToString();
            if (AppResourcesManager.IsComponentUri(resourceUri))
            {
                if (GetXamlComponentLoader(resourceUri) is IXamlComponentLoader loader)
                {
                    loader.LoadComponent(component);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void LoadComponent(object component, IXamlComponentLoader loader)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (loader is null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            loader.LoadComponent(component);
        }

        private static IXamlComponentLoader GetXamlComponentLoader(string componentUri)
        {
            string className = XamlResourcesHelper.GenerateClassNameFromComponentUri(componentUri);
            string assemblyName = AppResourcesManager.ExtractAssemblyNameFromComponentUri(componentUri);

            Type loaderType = Type.GetType($"{className}, {assemblyName}");
            if (loaderType != null)
            {
                return Activator.CreateInstance(loaderType) as IXamlComponentLoader;
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
            if (uriResource is null)
            {
                throw new ArgumentNullException(nameof(uriResource));
            }

            if (uriResource.IsAbsoluteUri)
            {
                throw new ArgumentException("Uri must be relative.");
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
            ApplicationLifetimeObjectsCollection services = PrivateApplicationLifetimeObjects;

            // Note: Silverlight invokes the Exiting method before firing the Exit event. However, since
            // we allow cancellation of the Exit event, we need to call this method after the Exit event,
            // because we only want to stop the services if the event was not cancelled.
            foreach (IApplicationService service in services)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Exiting();
                    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex, false);
                    }
                }
            }

            foreach (IApplicationService service in services)
            {
                if (service is IApplicationLifetimeAware lifetimeAwareService)
                {
                    try
                    {
                        lifetimeAwareService.Exited();
                    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex, false);
                    }
                }
            }

            // Note: Silverlight stops the services in reverse order of their registration.
            for (int i = services.Count - 1; i >= 0; i--)
            {
                IApplicationService service = (IApplicationService)services[i];

                try
                {
                    service.StopService();
                }
                catch (Exception ex)
                {
                    OnUnhandledException(ex, false);
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
