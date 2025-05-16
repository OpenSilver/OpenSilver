using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Wpf;
using System.IO;
using System.Reflection;

namespace OpenSilver.Simulator.BlazorSupport
{
    /// <summary>
    /// Provides static initialization logic to hook up Blazor support into the OpenSilver simulator
    /// when hosted in a WebView2 control.
    /// It attempts to load the <c>OpenSilver.Blazor.dll</c> assembly, invoke its internal initializer,
    /// and wire up the necessary JavaScript bridge and services for Blazor rendering.
    /// </summary>
    internal static class OpenSilverBlazorInitializer
    {
        const string OpenSilverBlazorAsm = "OpenSilver.Blazor";
        const string OpenSilverBlazorDll = OpenSilverBlazorAsm + ".dll";
        const string InitializerTypeName = "OpenSilver.Blazor.Initializer";
        const string UseBlazorForOpenSilverMethodName = "UseBlazorForOpenSilver";

        /// <summary>
        /// If the OpenSilver.Blazor DLL is present on disk, loads it (if needed)
        /// and calls Initializer.UseBlazorForOpenSilver(configuration, dispatcher) via reflection.
        /// </summary>
        private static bool TryUseBlazorForOpenSilver(IJSComponentConfiguration configuration,
            Dispatcher dispatcher)
        {
            var asm = AppDomain.CurrentDomain
                              .GetAssemblies()
                              .FirstOrDefault(a =>
                                  string.Equals(a.GetName().Name, OpenSilverBlazorAsm, StringComparison.OrdinalIgnoreCase));

            if (asm == null && File.Exists(OpenSilverBlazorDll))
            {
                try
                {
                    asm = Assembly.Load(new AssemblyName(OpenSilverBlazorAsm));
                }
                catch (Exception)
                {
                    return false;
                }
            }

            if (asm == null)
            {
                return false;
            }

            var initializerType = asm.GetType(InitializerTypeName, throwOnError: false, ignoreCase: false);
            if (initializerType == null)
            {
                return false;
            }

            var method = initializerType.GetMethod(
                UseBlazorForOpenSilverMethodName,
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: [typeof(IJSComponentConfiguration), dispatcher.GetType()],
                modifiers: null);

            if (method == null)
            {
                return false;
            }

            try
            {
                method.Invoke(
                    obj: null,
                    parameters: [configuration, dispatcher]);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException ?? tie;
            }

            return true;
        }

        /// <summary>
        /// Configures and returns a <see cref="BlazorWebViewManager"/> if Blazor support is available;
        /// otherwise, returns <c>null</c>.
        /// </summary>
        public static BlazorWebViewManager TryInitialize(System.Windows.Threading.Dispatcher dispatcher, WebView2 browser, string simulatorUrl)
        {
            var blazorDispatcher = new BlazorDispatcher(dispatcher);
            var jsComponentConfiguration = new JSComponentConfiguration();
            if (!TryUseBlazorForOpenSilver(jsComponentConfiguration, blazorDispatcher))
            {
                return null;
            }

            browser.CoreWebView2.DOMContentLoaded += (s, e) =>
            {
                var currentUrl = browser.Source.ToString();
                if (currentUrl == simulatorUrl)
                {
                    const string js = @"
                        (function () {
                            window.external = window.external || {};
                            window.external.sendMessage = message => {
                                window.chrome.webview.postMessage(message);
                            };
                            window.external.receiveMessage = callback => {
                                window.chrome.webview.addEventListener('message', e => callback(e.data));
                            };
                            const script = document.createElement('script');
                            script.src = '_framework/blazor.webview.js';
                            document.head.appendChild(script);
                        })();
                    ";
                    browser.CoreWebView2.ExecuteScriptAsync(js);
                }
            };

            var services = new ServiceCollection();
            services.AddBlazorWebView();

            var executingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new BlazorWebViewManager(browser, services.BuildServiceProvider(), blazorDispatcher,
                new Uri(simulatorUrl), new PhysicalFileProvider(executingAssemblyLocation),
                jsComponentConfiguration.JSComponents, "/");
        }
    }
}
