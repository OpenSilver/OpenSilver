using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using Microsoft.Web.WebView2.Wpf;
using OpenSilver.Simulator.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenSilver.Simulator
{
    public class SimBrowser : WebView2
    {
        public IList<CookieData> Cookies;
        public string CacheFolderName { get { return "simulator-temp-cache"; } }
        public static SimBrowser Instance { get; }
        public Action OnNavigationCompleted { get; set; }
        public Action OnInitialized { get; set; }

        static SimBrowser() { Instance = new SimBrowser(); }

        private SimBrowser()
        {
            Loaded += SimBrowser_Loaded;
            CoreWebView2InitializationCompleted += SimBrowser_CoreWebView2InitializationCompleted;
        }

        private async void SimBrowser_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await EnsureCoreWebView2Async(CreateDefaultEnvironment());
        }

        private CoreWebView2Environment CreateDefaultEnvironment()
        {
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();

            options.AdditionalBrowserArguments = "--disable-web-security --user-data-dir=\"c:/ chromedev\" --disable-features=SameSiteByDefaultCookies";
            options.AdditionalBrowserArguments += " --allow-file-access-from-file";
            options.AdditionalBrowserArguments += " --allow-file-access";
            options.AdditionalBrowserArguments += " --remote-debugging-port=9222";

            CoreWebView2Environment environment = CoreWebView2Environment.CreateAsync(null, Path.GetFullPath(CacheFolderName), options).Result;
            return environment;
        }

        private async void SimBrowser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var coreWebView = CoreWebView2;

            coreWebView.AddWebResourceRequestedFilter("*.js", CoreWebView2WebResourceContext.Script);
            coreWebView.AddWebResourceRequestedFilter("*.css", CoreWebView2WebResourceContext.Stylesheet);

            coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
            coreWebView.NavigationStarting += CoreWebView_NavigationStarting;
            coreWebView.NavigationCompleted += CoreWebView_NavigationCompleted;
            coreWebView.ContextMenuRequested += CoreWebView_ContextMenuRequested;

            await coreWebView.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");

            coreWebView.SetVirtualHostNameToFolderMapping(RootPage.SimulatorHostName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CoreWebView2HostResourceAccessKind.Allow);

            //Attach to browser console logging 
            DevToolsProtocolHelper helper = coreWebView.GetDevToolsProtocolHelper();
            await helper.Runtime.EnableAsync();
            helper.Runtime.ConsoleAPICalled += OnConsoleMessage;

            coreWebView.Settings.AreBrowserAcceleratorKeysEnabled = false;
            if (Properties.Settings.Default.IsDevToolsOpened)
                coreWebView.OpenDevToolsWindow();

            CookiesHelper.SetCustomCookies(this, Cookies);

            if (OnInitialized != null)
                OnInitialized();
        }

        private void CoreWebView_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var uri = e.Request.Uri.Replace("file:///", "").Replace("[PARENT]", "..").Replace("%20", " ").Replace('/', '\\');

            if (File.Exists(uri))
            {
                string contentType = null;
                if (uri.EndsWith(".js"))
                    contentType = "application/javascript";
                else if (uri.EndsWith(".css"))
                    contentType = "text/css";

                if (contentType == null)
                    throw new Exception("unexpected resource in simulator-root");

                FileStream fs = File.OpenRead(uri);
                e.Response = CoreWebView2.Environment.CreateWebResourceResponse(fs, 200, "OK", $"Content-Type: {contentType}");
            }
        }

        private void CoreWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
        }

        private void CoreWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            AllowDenyContextMenu(false);

            OnNavigationCompleted();
        }

        private void OnConsoleMessage(object sender, Runtime.ConsoleAPICalledEventArgs e)
        {
            var logMsg = string.Join("\n", e.Args.Select(arg => arg.Value.ToString()));
            var callFrame = e.StackTrace.CallFrames[0];
            var fileSource = new FileSource(callFrame.Url, callFrame.FunctionName, callFrame.LineNumber);

            switch (e.Type)
            {
#if DEBUG
                case "debug":
#endif
                case "log":
                    MainWindow.Instance.Console.AddMessage(new ConsoleMessage(logMsg, ConsoleMessage.MessageLevel.Log, fileSource));
                    break;
                case "warning":
                    if (!string.IsNullOrEmpty(callFrame.Url))
                    {
                        MainWindow.Instance.Console.AddMessage(new ConsoleMessage(logMsg, ConsoleMessage.MessageLevel.Warning, fileSource));
                    }
                    else
                    {
                        MainWindow.Instance.Console.AddMessage(new ConsoleMessage(logMsg, ConsoleMessage.MessageLevel.Warning));
                    }
                    break;
                case "error":
                    if (!string.IsNullOrEmpty(callFrame.Url))
                    {
                        MainWindow.Instance.Console.AddMessage(new ConsoleMessage(logMsg, ConsoleMessage.MessageLevel.Error, fileSource));
                    }
                    else
                    {
                        MainWindow.Instance.Console.AddMessage(new ConsoleMessage(logMsg, ConsoleMessage.MessageLevel.Error));
                    }
                    break;
            }
        }

        private void CoreWebView_ContextMenuRequested(object sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            CoreWebView2Deferral deferral = e.GetDeferral();
            e.Handled = true;
            ContextMenu cMenu = new ContextMenu();
            cMenu.Closed += (s, ex) => deferral.Complete();

            var inspectDomElementItem = new MenuItem() { Header = "Inspect Dom Element" };
            inspectDomElementItem.Click += (ss, ee) =>
            {
                e.SelectedCommandId = e.MenuItems.Single(mi => mi.Name == "inspectElement").CommandId;
            };

            cMenu.Items.Add(inspectDomElementItem);
            cMenu.IsOpen = true;
        }

        public object ExecuteScriptWithResult(string javaScript)
        {
            if (Dispatcher.HasShutdownStarted)
                return null;

            string jsonString = null;

            if ((this as DispatcherObject).CheckAccess())
            {
                throw new NotSupportedException("Should not call ExecuteScript on the WebView2 thread");
            }
            else
            {
                var executeTask = Dispatcher.InvokeAsync(async () =>
                {
                    try
                    {
                        return await ExecuteScriptAsync(javaScript);
                    }
                    catch (TaskCanceledException)
                    {
                        return await Task.FromResult(null as string);
                    }
                }).Result;

                jsonString = executeTask.Result;
                if (jsonString == null)
                    return null;
            }

            var jsonDoc = JsonDocument.Parse(jsonString);

            if (jsonDoc != null)
            {
                switch (jsonDoc.RootElement.ValueKind)
                {
                    case JsonValueKind.String:
                        return jsonDoc.RootElement.GetString();
                    case JsonValueKind.Null:
                        return null;
                }
            }

            return jsonString;
        }

        public void AllowDenyContextMenu(bool allow)
        {
            if (allow)
            {
                ExecuteScriptAsync("document.body.oncontextmenu = null");
            }
            else
            {
                ExecuteScriptAsync("document.body.oncontextmenu = function() {return false;}");
            }
        }
    }
}
