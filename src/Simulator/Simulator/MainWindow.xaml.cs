﻿extern alias opensilver;

using MahApps.Metro.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using OpenSilver;
using OpenSilver.Simulator;
using OpenSilver.Simulator.XamlInspection;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public const string TipToCopyToClipboard = "TIP: You can copy the content of this message box by pressing Ctrl+C now.";
        string _pathOfAssemblyThatContainsEntryPoint;
        JavaScriptExecutionHandler _javaScriptExecutionHandler;
        bool _htmlHasBeenLoaded = false;
        Assembly _entryPointAssembly;
        Action _appCreationDelegate;
        SimulatorLaunchParameters _simulatorLaunchParameters;
        string _outputRootPath;
        string _outputResourcesPath;
        WebView2 MainWebBrowser;
        string _browserUserDataDir;
        readonly Thread _openSilverRuntimeThread;
        Dispatcher _openSilverRuntimeDispatcher;
        string _lastExecutedJavaScript = "";

        const string NAME_FOR_STORING_COOKIES = "ms_cookies_for_user_application"; // This is an arbitrary name used to store the cookies in the registry
        const string NAME_OF_TEMP_CACHE_FOLDER = "simulator-temp-cache";

        //https is used because of XR# requirement to host on https.
        private const string DefaultSimulatorUrl = "https://simulator.opensilver/";
        private readonly string _simulatorUrl;

        private const string OutputRootPath = "wwwroot";
        private const string OutputResourcesPath = "resources";

        internal static MainWindow Instance { get; private set; }

        public MainWindow(Action appCreationDelegate, Assembly appAssembly, SimulatorLaunchParameters simulatorLaunchParameters)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;

            InitializeComponent();
            Instance = this;

            Icon = new BitmapImage(new Uri("pack://application:,,,/OpenSilver.Simulator;component/OpenSilverIcon.ico"));
            Title = "Simulator II - OpenSilver";

            _appCreationDelegate = appCreationDelegate ?? throw new ArgumentNullException(nameof(appCreationDelegate));
            _simulatorLaunchParameters = simulatorLaunchParameters;
            _entryPointAssembly = appAssembly;
            _pathOfAssemblyThatContainsEntryPoint = _entryPointAssembly.Location;
            _simulatorUrl = simulatorLaunchParameters?.SimulatorUrl ?? DefaultSimulatorUrl;

            MainWebBrowser = new WebView2
            {
                Width = 150,
                Height = 200
            };
            MainWebBrowser.SizeChanged += MainWebBrowser_SizeChanged;

            simulatorLaunchParameters?.BrowserCreatedCallback?.Invoke(MainWebBrowser);

            //Note: The following line was an attempt to persist the Microsoft login cookies (for use by user applications that required AAD login), but it is no longer necessary because we changed the DotNetBrowser "StorageType" from "MEMORY" to "DISK", so cookies are now automatically persisted.
            //CookiesHelper.LoadMicrosoftCookies(MainWebBrowser, NAME_FOR_STORING_COOKIES);

            BrowserContainer.Child = MainWebBrowser;

            CheckBoxCORS.IsChecked = CrossDomainCallsHelper.IsBypassCORSErrors;
            CheckBoxCORS.Checked += CheckBoxCORS_Checked;
            CheckBoxCORS.Unchecked += CheckBoxCORS_Unchecked;

            LoadDisplaySize();

            // Continue when the window is loaded:
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

            _openSilverRuntimeThread = new Thread(() =>
            {
                _openSilverRuntimeDispatcher = Dispatcher.CurrentDispatcher;
                Dispatcher.Run();
            })
            {
                Name = "OpenSilverRuntimeThread",
                Priority = ThreadPriority.Highest,
            };

            _openSilverRuntimeThread.SetApartmentState(ApartmentState.STA);
            _openSilverRuntimeThread.Start();
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            SimulatorProxy.ShowExceptionStatic(e.Exception);
            e.Handled = true;
        }

        async void MainWebBrowser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DisplaySize_Desktop.IsChecked == true
                && _htmlHasBeenLoaded)
            {
                // Apply the size in pixels to the root <div> inside the html page:
               await ReflectBrowserSizeOnRootElementSizeAsync();
            }
        }

        async Task InitializeWebViewAsync()
        {
            _browserUserDataDir = Path.GetFullPath(NAME_OF_TEMP_CACHE_FOLDER);
            Directory.CreateDirectory(_browserUserDataDir);

            List<string> chromiumSwitches = new List<string>();

            if (CrossDomainCallsHelper.IsBypassCORSErrors)
            {
                chromiumSwitches.Add(@"--disable-web-security");
            }

            chromiumSwitches.AddRange(new[] {
                @"--disable-web-security",
                @"--allow-file-access-from-files",
                @"--allow-file-access",
                @"--remote-debugging-port=9222"
            });

            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, _browserUserDataDir,
                new CoreWebView2EnvironmentOptions(string.Join(" ", chromiumSwitches)));

            MainWebBrowser.CoreWebView2InitializationCompleted +=
                (_, _) => CookiesHelper.SetCustomCookies(MainWebBrowser, _simulatorLaunchParameters?.CookiesData);

            await MainWebBrowser.EnsureCoreWebView2Async(environment);

            var devToolsHelper = MainWebBrowser.CoreWebView2.GetDevToolsProtocolHelper();
            await devToolsHelper.Emulation.SetTouchEmulationEnabledAsync(true);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.Closed -= MainWindow_Closed;
            _openSilverRuntimeDispatcher?.BeginInvokeShutdown(DispatcherPriority.Normal);
        }


        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            while (_openSilverRuntimeDispatcher == null)
            {
                await Task.Yield();
            }

            // Initialize the WebBrowser control:
            await InitializeWebViewAsync();

            if (InitializeApplication())
            {
                MainWebBrowser.CoreWebView2.DOMContentLoaded += (s1, e1) =>
                {
                    Dispatcher.BeginInvoke((Action)(async () =>
                    {
                        //todo: verify that we are not on an outside page (eg. Azure Active Directory login page)
                        await OnLoadedAsync();
                    }), DispatcherPriority.ApplicationIdle);
                };

                await MainWebBrowser.CoreWebView2.CallDevToolsProtocolMethodAsync("Log.enable", "{}");
                MainWebBrowser.CoreWebView2.GetDevToolsProtocolEventReceiver("Log.entryAdded").DevToolsProtocolEventReceived += OnConsoleMessageEvent;
            }

            LoadIndexFile();
        }

        private void OnConsoleMessageEvent(object sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs args)
        {
            try
            {
                Console.ConsoleMessageHolder messageHolder = JsonSerializer.Deserialize<Console.ConsoleMessageHolder>(args.ParameterObjectAsJson);
                if (messageHolder?.Message != null)
                {
                    Console.AddMessage(messageHolder.Message);
                }
            }
            catch { }
        }

        private string PrepareIndexFile()
        {
            string simulatorRootHtml = File.ReadAllText("simulator_root.html");

            string outputPathAbsolute = GetOutputPathAbsoluteAndReadAssemblyAttributes();

            // Read the "App.Config" file for future use by the ClientBase.
            string relativePathToAppConfigFolder = CombinePathsWhileEnsuringEndingBackslashAndMore(_outputResourcesPath, _entryPointAssembly.GetName().Name);
            string relativePathToAppConfig = Path.Combine(relativePathToAppConfigFolder, "app.config.g.js");
            if (File.Exists(Path.Combine(outputPathAbsolute, relativePathToAppConfig)))
            {
                string scriptToReadAppConfig = "<script type=\"application/javascript\" src=\"" + Path.Combine(outputPathAbsolute, relativePathToAppConfig) + "\"></script>";
                simulatorRootHtml = simulatorRootHtml.Replace("[SCRIPT_TO_READ_APPCONFIG_GOES_HERE]", scriptToReadAppConfig);
            }
            else
            {
                simulatorRootHtml = simulatorRootHtml.Replace("[SCRIPT_TO_READ_APPCONFIG_GOES_HERE]", string.Empty);
            }

            // Read the "ServiceReferences.ClientConfig" file for future use by the ClientBase:
            string relativePathToServiceReferencesClientConfig = Path.Combine(relativePathToAppConfigFolder, "servicereferences.clientconfig.g.js");
            if (File.Exists(Path.Combine(outputPathAbsolute, relativePathToServiceReferencesClientConfig)))
            {
                string scriptToReadServiceReferencesClientConfig = "<script type=\"application/javascript\" src=\"" + Path.Combine(outputPathAbsolute, relativePathToServiceReferencesClientConfig) + "\"></script>";
                simulatorRootHtml = simulatorRootHtml.Replace("[SCRIPT_TO_READ_SERVICEREFERENCESCLIENTCONFIG_GOES_HERE]", scriptToReadServiceReferencesClientConfig);

            }
            else
            {
                simulatorRootHtml = simulatorRootHtml.Replace("[SCRIPT_TO_READ_SERVICEREFERENCESCLIENTCONFIG_GOES_HERE]", string.Empty);
            }

            simulatorRootHtml = simulatorRootHtml.Replace("..", "[PARENT]");

            if (_simulatorLaunchParameters?.InitParams != null)
            {
                simulatorRootHtml = simulatorRootHtml.Replace(
                    "[PARAM_INITPARAMS_GOES_HERE]",
                    $"<param name=\"InitParams\" value=\"{_simulatorLaunchParameters.InitParams}\"");
            }
            else
            {
                simulatorRootHtml = simulatorRootHtml.Replace("[PARAM_INITPARAMS_GOES_HERE]", string.Empty);
            }

            return simulatorRootHtml;
        }

        /// <summary>
        /// This method combines two paths while ensuring that no forward slash is used and that the combined path ends with a backslash.
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns>The combined path</returns>
        private static string CombinePathsWhileEnsuringEndingBackslashAndMore(string path1, string path2)
        {
            var separator = Path.DirectorySeparatorChar;

            // Replace any incorrect path separators with the correct one:
            path1 = path1.Replace('/', separator).Replace('\\', separator);
            path2 = path2.Replace('/', separator).Replace('\\', separator);

            // Combine the paths:
            var combinedPath = Path.Combine(path1, path2);

            // Make sure that the combined path ends with the correct separator:
            if (!combinedPath.EndsWith(separator.ToString()))
            {
                combinedPath += separator;
            }

            return combinedPath;
        }

        private void LoadIndexFile()
        {
            // Load the page:
            MainWebBrowser.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            MainWebBrowser.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

            MainWebBrowser.CoreWebView2.Navigate(_simulatorUrl);
        }

        private void SyncXamlInspectorVisibility()
        {
            bool xamlInspectorVisible = Properties.Settings.Default.XamlInspectorVisible;
            if (xamlInspectorVisible &&
                _entryPointAssembly != null &&
                XamlInspectionTreeViewInstance.TryRefresh(_entryPointAssembly, XamlPropertiesPaneInstance))
            {
                MainGridSplitter.Visibility = Visibility.Visible;
                BorderForXamlInspection.Visibility = Visibility.Visible;
                ButtonViewXamlTree.Visibility = Visibility.Collapsed;
                ContainerForXamlInspectorToolbar.Visibility = Visibility.Visible;
                ButtonHideXamlTree.Visibility = Visibility.Visible;
            }
        }

        private string GetHeaders(string fileName)
        {
            return $"Content-Type:{MimeTypesMap.GetMimeType(fileName)}";
        }

        private static bool AreUrlsEqual(string url1, string url2)
        {
            Uri uri1 = new Uri(url1);
            Uri uri2 = new Uri(url2);

            return uri1.Scheme == uri2.Scheme &&
                   uri1.Host == uri2.Host &&
                   uri1.AbsolutePath.TrimEnd('/') == uri2.AbsolutePath.TrimEnd('/');
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var environment = MainWebBrowser.CoreWebView2.Environment;
            var uriString = e.Request?.Uri;

            if (string.IsNullOrEmpty(uriString))
            {
                return;
            }

            if (!uriString.StartsWith(_simulatorUrl))
            {
                return;
            }

            if (AreUrlsEqual(uriString, _simulatorUrl))
            {
                var response = environment.CreateWebResourceResponse(
                    new MemoryStream(Encoding.UTF8.GetBytes(PrepareIndexFile())),
                    200, "OK", GetHeaders("index.html"));
                e.Response = response;
                return;
            }

            // Attempts to parse the provided URL string into a Uri object
            // to extract the local path, excluding any query parameters.
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                var localPath = uri.LocalPath.Trim('/');

                if (localPath.StartsWith(OutputResourcesPath))
                {
                    localPath = Path.Combine(OutputRootPath, localPath);
                }

                if (File.Exists(localPath))
                {
                    var response = environment.CreateWebResourceResponse(
                        new MemoryStream(File.ReadAllBytes(localPath)),
                        200, "OK", GetHeaders(localPath));
                    e.Response = response;
                    return;
                }
            }

            var notFound = environment.CreateWebResourceResponse(
                new MemoryStream(Encoding.UTF8.GetBytes("" +
                "<html>" +
                    "<head><title>404 Not Found</title></head>" +
                    "<body><center><h1>404 Not Found</h1></center></body>" +
                "</html>")), 404, "Not Found", GetHeaders("index.html"));
            e.Response = notFound;
        }

        private async Task OnLoadedAsync()
        {
            if (!_htmlHasBeenLoaded)
            {
                _htmlHasBeenLoaded = true;
                await UpdateWebBrowserAndWebPageSizeBasedOnCurrentState();

                // Start the app:
                ShowLoadingMessage();

                //We check if the key used by the user is still valid:
                CheckKeysValidity();

                await WaitForDocumentToBeFullyLoadedAsync(); // Note: without this, we got errors when running rokjs (with localhost as base url) without any breakpoints.

                await SetupSimulatorHostObject();

                bool success = await _openSilverRuntimeDispatcher.InvokeAsync(() => StartApplication());

                await Dispatcher.BeginInvoke(async () =>
                {
                    if (success)
                    {
                        _simulatorLaunchParameters?.AppStartedCallback?.Invoke();
                    }

                    HideLoadingMessage();

                    SyncXamlInspectorVisibility();

                    await UpdateWebBrowserAndWebPageSizeBasedOnCurrentState();
                }, DispatcherPriority.ApplicationIdle); // We do so in order to give the time to the rendering engine to display the "Loading..." message.
            }
        }

        private async Task WaitForDocumentToBeFullyLoadedAsync()
        {
            int startTime = Environment.TickCount;
            bool loaded = false;

            string htmlDocument =await MainWebBrowser.ExecuteScriptAsync(@"document");

            while (!loaded && (Environment.TickCount - startTime < 40000)) // Wait is limited to max 4 seconds.
            {
                if (htmlDocument != null)
                {
                    string xamlRoot = null;
                    try
                    {
                        xamlRoot = (await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync(
                            "document.getElementById('opensilver-root')")).ResultAsJson;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Initialization: can not get the root. {ex.Message}");
                    }

                    if (xamlRoot != null && xamlRoot != "null")
                    {
                        loaded = true;
                        break;
                    }
                    else
                    {
                        const string ROOT_NAME = "opensilver-root";
                        Debug.WriteLine($"Initialization: {ROOT_NAME} was not ready on first try.");
                    }
                }
                else
                {
                    Debug.WriteLine("Initialization: htmlDocument was null on first try.");
                    htmlDocument = await MainWebBrowser.ExecuteScriptAsync(@"document");
                }

                Thread.Sleep(50);
            }

            if (!loaded)
            {
                Debug.WriteLine("Initialization: The document was still not loaded after timeout.");
            }
        }

        private void CheckKeysValidity()
        {
            Thread thread = new Thread(() =>
            {
                bool isAllOK = CheckFeatureValidity(Constants.ENTERPRISE_EDITION_FEATURE_ID, Constants.ENTERPRISE_EDITION_FRIENDLY_NAME);
                isAllOK = isAllOK && CheckFeatureValidity(Constants.SL_MIGRATION_EDITION_FEATURE_ID, Constants.SL_MIGRATION_EDITION_FRIENDLY_NAME);
                isAllOK = isAllOK && CheckFeatureValidity(Constants.PROFESSIONAL_EDITION_FEATURE_ID, Constants.PROFESSIONAL_EDITION_FRIENDLY_NAME);
                isAllOK = isAllOK && CheckFeatureValidity(Constants.COMMERCIAL_EDITION_S_FEATURE_ID, Constants.COMMERCIAL_EDITION_S_FRIENDLY_NAME);
                isAllOK = isAllOK && CheckFeatureValidity(Constants.COMMERCIAL_EDITION_L_FEATURE_ID, Constants.COMMERCIAL_EDITION_L_FRIENDLY_NAME);
                isAllOK = isAllOK && CheckFeatureValidity(Constants.PREMIUM_SUPPORT_EDITION_FEATURE_ID, Constants.PREMIUM_SUPPORT_EDITION_FRIENDLY_NAME);
            });
            thread.Start();
        }

        private bool CheckFeatureValidity(string featureId, string editionName)
        {
            return true;
        }

        private async void ButtonStats_Click(object sender, RoutedEventArgs e)
        {
            // Count the number of DOM elements:
            var count = await MainWebBrowser.ExecuteScriptAsync(@"document.getElementsByTagName(""*"").length");

            // Display the result
            MessageBox.Show("Number of DOM elements: " + count
                + Environment.NewLine
                + Environment.NewLine
                + "TIPS:"
                + Environment.NewLine
                + Environment.NewLine
                + "- For best performance on mobile devices, be sure to keep the number of DOM elements low by limiting the number of UI Elements in the Visual Tree, and by using only default control templates instead of custom control templates. Also note that scrolling performance is greatly improved when the scroll bar visibility of ScrollViewers is set to 'Visible' rather than 'Auto'."
                + Environment.NewLine
                + Environment.NewLine
                + @"- If a portion of your application requires to display thousands of UI Elements, such as in a custom Chart or Calendar control, or if you need very high performance graphics, for example for games, you may want to use the ""HtmlCanvas"" control. To learn about it, please read:"
                + Environment.NewLine
                + "  http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx"
                + Environment.NewLine
                + Environment.NewLine
                + "- To learn how to profile performance in order to pinpoint performance issues, please read:"
                + "  http://cshtml5.com/links/how-to-profile-performance.aspx"
                );
        }

        private async Task<string> getHtmlSnapshotAsync(bool osRootOnly = false, string htmlElementId = null, string xamlElementName = null)
        {
            CoreWebView2ExecuteScriptResult result;
            if (htmlElementId != null)
            {
                result = await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync($"document.getElementById('{htmlElementId}').outerHTML");
            }
            else if (xamlElementName != null)
            {
                result = await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync($"document.querySelectorAll('[dataid=\"{xamlElementName}\"]')[0].outerHTML");
            }
            else if (osRootOnly)
            {
                result = await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync("document.getElementById('opensilver-root').outerHTML");
            }
            else
            {
                result = await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync("document.documentElement.outerHTML");
            }

            if (result != null && result.Succeeded)
            {
                result.TryGetResultAsString(out string html, out int status);
                if (status == 1 && !string.IsNullOrEmpty(html))
                {
                    return html;
                }
            }

            return "";
        }

        private async void ButtonSeeHtml_Click(object sender, RoutedEventArgs e)
        {
            string html = await getHtmlSnapshotAsync();
            var msgBox = new MessageBoxScrollable()
            {
                Value = html,
                Title = "Snapshot of HTML displayed in the Simulator"
            };
            msgBox.Show();
        }

        private async void ButtonSaveHtml_Click(object sender, RoutedEventArgs e)
        {
            string html = await getHtmlSnapshotAsync();
            SaveFileDialog saveFileDialog = new SaveFileDialog() { FileName = "index.html" };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, html);
        }

        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            this.StartApplication();
        }

        private void ButtonViewJavaScriptLog_Click(object sender, RoutedEventArgs e)
        {
            string fullLog = _javaScriptExecutionHandler.FullLogOfExecutedJavaScriptCode;
            var msgBox = new MessageBoxScrollable()
            {
                Value = fullLog,
                Title = "All JS code executed so far by the Simulator"
            };
            msgBox.Show();
        }

        private void ButtonDebugJavaScriptLog_Click(object sender, RoutedEventArgs e)
        {
            string destinationFolderName = "TempDebugOpenSilver";
            string info =
$@"This feature lets you debug the JavaScript code executed by the Simulator so far, which corresponds to the content of the Interop.ExecuteJavaScript(...) calls as well as the JS/C# interop calls that are specific to the Simulator.

A folder named '{destinationFolderName}' will be created on your desktop. The folder will contain a file named 'index.html' and other files. Just open that file with a browser and use the Browser Developer Tools to debug the code. In particular, you can look for errors in the browser Console output, and you can enable the 'Pause on caught exceptions' option in the Developer Tools to step into the code when an error occurs.

Click OK to continue.";
            MessageBoxResult result = MessageBox.Show(info, "Information", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result != MessageBoxResult.Cancel)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destinationPath = Path.Combine(desktopPath, destinationFolderName);

                try
                {
                    // Create the destination folder if it does not already exist:
                    if (!Directory.Exists(destinationPath))
                        Directory.CreateDirectory(destinationPath);

                    // Copy the html file:
                    string simulatorExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    File.Copy(Path.Combine(simulatorExePath, "interop_debug_root.html"), Path.Combine(destinationPath, "index.html"), true);

                    string simulatorJsCssPath = Path.Combine(simulatorExePath, @"js_css");

                    File.Copy(Path.Combine(simulatorJsCssPath, "cshtml5.css"), Path.Combine(destinationPath, "cshtml5.css"), true);
                    File.Copy(Path.Combine(simulatorJsCssPath, "cshtml5.js"), Path.Combine(destinationPath, "cshtml5.js"), true);
                    File.Copy(Path.Combine(simulatorJsCssPath, "ResizeObserver.js"), Path.Combine(destinationPath, "ResizeObserver.js"), true);

                    // Create "interopcalls.js" which contains all the JS executed by the Simulator so far:
                    string fullLog = _javaScriptExecutionHandler.FullLogOfExecutedJavaScriptCode;
                    File.WriteAllText(Path.Combine(destinationPath, "interopcalls.js"), fullLog);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to generate the debug files in the folder:\r\n\r\n" + destinationPath + "\r\n\r\n" + ex.ToString());
                    return;
                }

                // Open the destination folder with Explorer:
                try
                {
                    System.Diagnostics.Process.Start(destinationPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n\r\n" + destinationPath);
                }
            }
        }

        bool InitializeApplication()
        {
            // In OpenSilver we already have the user application type passed to the constructor, so we do not need to retrieve it here
            try
            {
                // Create the JavaScriptExecutionHandler that will be called by the "Core" project to interact with the Emulator:

                _javaScriptExecutionHandler = new JavaScriptExecutionHandler(MainWebBrowser);

                InteropHelpers.InjectWebControlDispatcher(MainWebBrowser);
                InteropHelpers.InjectJavaScriptExecutionHandler(_javaScriptExecutionHandler);
                InteropHelpers.InjectWebClientFactory();
                InteropHelpers.InjectClipboardHandler();
                InteropHelpers.InjectSimulatorProxy(
                    new SimulatorProxy(MainWebBrowser,
                        Console,
                        MainWebBrowser.Dispatcher,
                        _openSilverRuntimeDispatcher,
                        _javaScriptExecutionHandler));

                // In the OpenSilver Version, we use this work-around to know if we're in the simulator
                InteropHelpers.InjectIsRunningInTheSimulator_WorkAround();

                // Inject the code to display the message box in the simulator:
                InteropHelpers.InjectCodeToDisplayTheMessageBox(
                    (message, title, showCancelButton) =>
                    {
                        if (Dispatcher.CheckAccess())
                        {
                            return MessageBox.Show(this,
                                message,
                                title,
                                showCancelButton ? MessageBoxButton.OKCancel : MessageBoxButton.OK)
                            == MessageBoxResult.OK;
                        }
                        else
                        {
                            return Dispatcher.Invoke(() =>
                            {
                                return MessageBox.Show(this,
                                    message,
                                    title,
                                    showCancelButton ? MessageBoxButton.OKCancel : MessageBoxButton.OK)
                                == MessageBoxResult.OK;
                            });
                        }
                    });

                InteropHelpers.InjectOpenSilverRuntimeDispatcher(_openSilverRuntimeDispatcher);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading the application: " + Environment.NewLine + Environment.NewLine + ex.Message);
                HideLoadingMessage();
                return false;
            }
        }

        private async Task SetupSimulatorHostObject()
        {
            const string name = "onCallBack";
            await Dispatcher.InvokeAsync(() =>
            {
                MainWebBrowser.CoreWebView2.AddHostObjectToScript(name, new opensilver::CSHTML5.Internal.OnCallbackSimulator());
                _javaScriptExecutionHandler.ExecuteJavaScript($"window.onCallBack = chrome.webview.hostObjects.{name};");

                MainWebBrowser.CoreWebView2.AddHostObjectToScript("XamlInspectorCallback", new XamlInspectorCallback());
            });
        }

        private bool StartApplication()
        {
            // Create a new instance of the application:
            try
            {
                _appCreationDelegate();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to start the application.\r\n\r\n" + ex.ToString());
                HideLoadingMessage();
                return false;
            }
        }

        /// <summary>
        /// This Method returns the absolute path to the Output folder and sets the _outputRootPath, _outputAppFilesPath, _outputLibrariesPath, _outputResourcesPath and _intermediateOutputAbsolutePath variables.
        /// </summary>
        /// <returns>The absolute path to the Output folder.</returns>
        string GetOutputPathAbsoluteAndReadAssemblyAttributes()
        {
            //--------------------------
            // Note: this method is similar to the one in the Compiler (PathsHelper).
            // IMPORTANT: If you update this method, make sure to update the other one as well.
            //--------------------------

            // Determine the output path by reading the "OutputRootPath" attribute that the compiler has injected into the entry assembly:
            if (_outputRootPath == null)
            {
                ReflectionInUserAssembliesHelper.GetOutputPathsByReadingAssemblyAttributes(_entryPointAssembly, out _outputRootPath, out _, out _, out _outputResourcesPath, out _);
            }

            string outputRootPathFixed = _outputRootPath.Replace('/', '\\');
            if (!outputRootPathFixed.EndsWith("\\") && outputRootPathFixed != "")
                outputRootPathFixed = outputRootPathFixed + '\\';

            // If the path is already ABSOLUTE, we return it directly, otherwise we concatenate it to the path of the assembly:
            string outputPathAbsolute;
            if (Path.IsPathRooted(outputRootPathFixed))
            {
                outputPathAbsolute = outputRootPathFixed;
            }
            else
            {
                outputPathAbsolute = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_pathOfAssemblyThatContainsEntryPoint)), outputRootPathFixed);

                outputPathAbsolute = outputPathAbsolute.Replace('/', '\\');

                if (!outputPathAbsolute.EndsWith("\\") && outputPathAbsolute != "")
                    outputPathAbsolute = outputPathAbsolute + '\\';
            }

            return outputPathAbsolute;
        }

        private void ButtonShowAdvancedTools_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }

        void ButtonClearCookiesAndCache_Click(object sender, RoutedEventArgs e)
        {
            CookiesHelper.ClearCookies(MainWebBrowser, NAME_FOR_STORING_COOKIES);
            try
            {
                if (!string.IsNullOrWhiteSpace(_browserUserDataDir)
                    && Directory.Exists(_browserUserDataDir))
                {
                    MessageBoxResult result
                        = MessageBox.Show("To fully clear the Simulator cache, please close the Simulator and manually delete the following folder:" + Environment.NewLine + Environment.NewLine + _browserUserDataDir + Environment.NewLine + Environment.NewLine + "Click OK to see this folder in Windows Explorer.", "Confirm?", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Process.Start(_browserUserDataDir);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void ShowLoadingMessage()
        {
            ContainerOfLoadingMessage.Visibility = Visibility.Visible;
        }

        void HideLoadingMessage()
        {
            ContainerOfLoadingMessage.Visibility = Visibility.Collapsed;
        }

        void SetWebBrowserSize(double width, double height)
        {
            try
            {
                // We take into account the "Font Size" (DPI) setting of Windows: //cf. http://answers.awesomium.com/questions/321/non-standard-dpi-rendering-is-broken-in-webcontrol.html
                double correctedWidth = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(width);
                double correctedHeight = ScreenCoordinatesHelper.ConvertHeightOrNaNToDpiAwareHeightOrNaN(height);
                MainWebBrowser.Width = correctedWidth;
                MainWebBrowser.Height = correctedHeight;


                Dispatcher.BeginInvoke((Action)(async() =>
                {
                    await ReflectBrowserSizeOnRootElementSizeAsync();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        async Task ReflectBrowserSizeOnRootElementSizeAsync()
        {
            try
            {
                string htmlDocument = await MainWebBrowser.ExecuteScriptAsync(@"document");

                if (htmlDocument != null)
                {
                    string cshtml5DomRootElement = null;
                    try
                    {
                        cshtml5DomRootElement = (await MainWebBrowser.CoreWebView2.ExecuteScriptWithResultAsync(
                            $"{htmlDocument}.getElementById('opensilver-root')")).ResultAsJson;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Can not get the root. {ex.Message}");
                    }

                    if (cshtml5DomRootElement != null) // It is not an object for example if the app has navigated to another page via "System.Windows.Browser.HtmlPage.Window.Navigate(url)"
                    {
                        double width = double.IsNaN(MainWebBrowser.Width) ? MainWebBrowser.ActualWidth : MainWebBrowser.Width;
                        double height = double.IsNaN(MainWebBrowser.Height) ? MainWebBrowser.ActualHeight : MainWebBrowser.Height;

                        // Take into account screen DPI:
                        width = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(width, invert: true); // Supports "NaN"
                        height = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(height, invert: true); // Supports "NaN"

                        if (!double.IsNaN(width))
                            await SetJSPropertyAsync(cshtml5DomRootElement, "style", width.ToString(CultureInfo.InvariantCulture) + "px");

                        if (!double.IsNaN(height))
                            await SetJSPropertyAsync(cshtml5DomRootElement, "style", height.ToString(CultureInfo.InvariantCulture) + "px");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        async Task SetJSPropertyAsync(string instance, string propertyName, object value)
        {
            await MainWebBrowser.ExecuteScriptAsync($"{instance}.{propertyName} = {value}");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //Note: The following line was an attempt to persist the Microsoft login cookies (for use by user applications that required AAD login), but it is no longer necessary because we changed the DotNetBrowser "StorageType" from "MEMORY" to "DISK", so cookies are now automatically persisted.
            //CookiesHelper.SaveMicrosoftCookies(MainWebBrowser, NAME_FOR_STORING_COOKIES);

            // Destroy the WebControl and its underlying view:
            _openSilverRuntimeDispatcher?.BeginInvokeShutdown(DispatcherPriority.Normal);

            _javaScriptExecutionHandler.MarkWebControlAsDisposed();
            MainWebBrowser.Dispose();

            // Kill the process to avoid having the Simulator process that remains open due to a MessageBox or something else:
            Application.Current.Shutdown();
        }

        void ButtonExecuteJS_Click(object sender, RoutedEventArgs e)
        {
            var inputBox = new InputBox()
            {
                Value = _lastExecutedJavaScript,
                Title = "Please enter JS to execute"
            };
            inputBox.Callback = (Action<bool>)(isOK =>
            {
                if (isOK)
                {
                    _javaScriptExecutionHandler.ExecuteJavaScript(inputBox.Value);
                    _lastExecutedJavaScript = inputBox.Value;
                }
            });
            inputBox.Show();
        }

        private async void DisplaySize_Click(object sender, RoutedEventArgs e)
        {
            SaveDisplaySize();
            await UpdateWebBrowserAndWebPageSizeBasedOnCurrentState();
        }

        private async Task UpdateWebBrowserAndWebPageSizeBasedOnCurrentState()
        {
            if (DisplaySize_Phone.IsChecked == true)
            {
                this.ResizeMode = ResizeMode.CanMinimize;
                this.SizeToContent = SizeToContent.WidthAndHeight;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 40; // Prevents the window from growing below the Windows task bar, cf. https://stackoverflow.com/questions/25790674/wpf-scrollbar-on-auto-and-sizetocontent-height-goes-under-windows7-toolbar
                OptionsForDisplaySize_Phone.Visibility = Visibility.Visible;
                OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                PhoneDecoration1.Visibility = Visibility.Visible;
                PhoneDecoration2.Visibility = Visibility.Visible;
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                MainBorder.HorizontalAlignment = HorizontalAlignment.Center;
                MainBorder.VerticalAlignment = VerticalAlignment.Top;
                MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                if (DisplaySize_Phone_Landscape.IsChecked == true)
                {
                    SetWebBrowserSize(480, 320);
                    ContainerForMainWebBrowserAndHighlightElement.Margin = new Thickness(60, 10, 60, 10);
                }
                else
                {
                    SetWebBrowserSize(320, 480);
                    ContainerForMainWebBrowserAndHighlightElement.Margin = new Thickness(10, 60, 10, 60);
                }

                await SetTouchEmulation(true);
            }
            else if (DisplaySize_Tablet.IsChecked == true)
            {
                this.ResizeMode = ResizeMode.CanMinimize;
                this.SizeToContent = SizeToContent.WidthAndHeight;
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 40; // Prevents the window from growing below the Windows task bar, cf. https://stackoverflow.com/questions/25790674/wpf-scrollbar-on-auto-and-sizetocontent-height-goes-under-windows7-toolbar
                OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                OptionsForDisplaySize_Tablet.Visibility = Visibility.Visible;
                PhoneDecoration1.Visibility = Visibility.Visible;
                PhoneDecoration2.Visibility = Visibility.Visible;
                MainBorder.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                MainBorder.HorizontalAlignment = HorizontalAlignment.Center;
                MainBorder.VerticalAlignment = VerticalAlignment.Top;
                MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                if (DisplaySize_Tablet_Landscape.IsChecked == true)
                {
                    SetWebBrowserSize(1024, 768);
                    ContainerForMainWebBrowserAndHighlightElement.Margin = new Thickness(60, 10, 60, 10);
                }
                else
                {
                    SetWebBrowserSize(768, 1024);
                    ContainerForMainWebBrowserAndHighlightElement.Margin = new Thickness(10, 60, 10, 60);
                }

                await SetTouchEmulation(true);
            }
            else if (DisplaySize_Desktop.IsChecked == true)
            {
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.SizeToContent = SizeToContent.Manual;
                this.MaxHeight = double.PositiveInfinity;
                OptionsForDisplaySize_Phone.Visibility = Visibility.Collapsed;
                OptionsForDisplaySize_Tablet.Visibility = Visibility.Collapsed;
                PhoneDecoration1.Visibility = Visibility.Collapsed;
                PhoneDecoration2.Visibility = Visibility.Collapsed;
                MainBorder.Background = new SolidColorBrush(Colors.Transparent);
                MainBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                MainBorder.VerticalAlignment = VerticalAlignment.Stretch;
                MainScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                MainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

                SetWebBrowserSize(double.NaN, double.NaN);
                ContainerForMainWebBrowserAndHighlightElement.Margin = new Thickness(0, 0, 0, 0);
                await Dispatcher.BeginInvoke(() =>
                {
                    Width = 1024;
                    Height = 768;
                });

                await SetTouchEmulation(false);
            }
            else
            {
                MessageBox.Show("Error: no display size selected. Please report this error to the authors.");
            }
        }

        private async Task SetTouchEmulation(bool enable)
        {
            var devToolsHelper = MainWebBrowser.CoreWebView2.GetDevToolsProtocolHelper();
            await devToolsHelper.Emulation.SetEmitTouchEventsForMouseAsync(enable);
        }

        private void ButtonViewXamlTree_Click(object sender, RoutedEventArgs e)
        {
            if (_entryPointAssembly != null
                && XamlInspectionTreeViewInstance.TryRefresh(_entryPointAssembly, XamlPropertiesPaneInstance))
            {
                MainGridSplitter.Visibility = Visibility.Visible;
                BorderForXamlInspection.Visibility = Visibility.Visible;
                ButtonViewXamlTree.Visibility = Visibility.Collapsed;
                ContainerForXamlInspectorToolbar.Visibility = Visibility.Visible;
                ButtonHideXamlTree.Visibility = Visibility.Visible;

                // Save opened state
                Properties.Settings.Default.XamlInspectorVisible = true;
                Properties.Settings.Default.Save();

                // We activate the element picker by default:
                StartElementPickerForInspection();
            }
            else
            {
                ButtonHideXamlTree_Click(sender, e);
                MessageBox.Show("The Visual Tree is not available.");
            }
        }

        private void ButtonOpenDevTools_Click(object sender, RoutedEventArgs e)
        {
            MainWebBrowser.CoreWebView2.OpenDevToolsWindow();
        }

        private void ButtonHideXamlTree_Click(object sender, RoutedEventArgs e)
        {
            MainGridSplitter.Visibility = Visibility.Collapsed;
            BorderForXamlInspection.Visibility = Visibility.Collapsed;
            ButtonViewXamlTree.Visibility = Visibility.Visible;
            ContainerForXamlInspectorToolbar.Visibility = Visibility.Collapsed;
            ButtonHideXamlTree.Visibility = Visibility.Collapsed;
            XamlPropertiesPaneInstance.Width = 0;

            // Reset columns in case they were modified by the GridSplitter:
            ColumnForLeftToolbar.Width = GridLength.Auto;
            ColumnForMainWebBrowser.Width = new GridLength(1, GridUnitType.Star);
            ColumnForGridSplitter.Width = GridLength.Auto;
            ColumnForXamlInspection.Width = GridLength.Auto;
            ColumnForXamlPropertiesPane.Width = GridLength.Auto;

            // Save closed state
            Properties.Settings.Default.XamlInspectorVisible = false;
            Properties.Settings.Default.Save();

            // Ensure that the element picker is not activated:
            StopElementPickerForInspection();
        }

        private void ButtonRefreshXamlTree_Click(object sender, RoutedEventArgs e)
        {
            ButtonViewXamlTree_Click(sender, e);
        }

        void ButtonXamlInspectorOptions_Click(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }

        void ButtonExpandAllNodes_Click(object sender, RoutedEventArgs e)
        {
            XamlInspectionTreeViewInstance.ExpandAllNodes();
        }

        void ButtonCollapseAllNodes_Click(object sender, RoutedEventArgs e)
        {
            XamlInspectionTreeViewInstance.CollapseAllNodes();
        }

        void SaveDisplaySize()
        {
            //-----------
            // Display size (Phone, Tablet, or Desktop)
            //-----------
            int displaySize = 0;
            if (DisplaySize_Phone.IsChecked == true)
                displaySize = 0;
            else if (DisplaySize_Tablet.IsChecked == true)
                displaySize = 1;
            else if (DisplaySize_Desktop.IsChecked == true)
                displaySize = 2;
            Properties.Settings.Default.DisplaySize = displaySize;

            //-----------
            // Phone orientation (Portrait or Landscape)
            //-----------
            int displaySize_Phone_Orientation = 0;
            if (DisplaySize_Phone_Portrait.IsChecked == true)
                displaySize_Phone_Orientation = 0;
            else if (DisplaySize_Phone_Landscape.IsChecked == true)
                displaySize_Phone_Orientation = 1;
            Properties.Settings.Default.DisplaySize_Phone_Orientation = displaySize_Phone_Orientation;

            //-----------
            // Tablet orientation (Portrait or Landscape)
            //-----------
            int displaySize_Tablet_Orientation = 0;
            if (DisplaySize_Tablet_Portrait.IsChecked == true)
                displaySize_Tablet_Orientation = 0;
            else if (DisplaySize_Tablet_Landscape.IsChecked == true)
                displaySize_Tablet_Orientation = 1;
            Properties.Settings.Default.DisplaySize_Tablet_Orientation = displaySize_Tablet_Orientation;

            // SAVE:
            Properties.Settings.Default.Save();
        }

        void LoadDisplaySize()
        {
            //-----------
            // Display size (Phone, Tablet, or Desktop)
            //-----------
            int displaySize = Properties.Settings.Default.DisplaySize;
            switch (displaySize)
            {
                case 0:
                    DisplaySize_Phone.IsChecked = true;
                    break;
                case 1:
                    DisplaySize_Tablet.IsChecked = true;
                    break;
                case 2:
                default:
                    DisplaySize_Desktop.IsChecked = true;
                    break;
            }

            //-----------
            // Phone orientation (Portrait or Landscape)
            //-----------
            int displaySize_Phone_Orientation = Properties.Settings.Default.DisplaySize_Phone_Orientation;
            switch (displaySize_Phone_Orientation)
            {
                case 1:
                    DisplaySize_Phone_Landscape.IsChecked = true;
                    break;
                case 0:
                default:
                    DisplaySize_Phone_Portrait.IsChecked = true;
                    break;
            }

            //-----------
            // Tablet orientation (Portrait or Landscape)
            //-----------
            int displaySize_Tablet_Orientation = Properties.Settings.Default.DisplaySize_Tablet_Orientation;
            switch (displaySize_Tablet_Orientation)
            {
                case 1:
                    DisplaySize_Tablet_Landscape.IsChecked = true;
                    break;
                case 0:
                default:
                    DisplaySize_Tablet_Portrait.IsChecked = true;
                    break;
            }
        }

        private class CustomResponseEventArgs : EventArgs
        {
            public string Url { get; private set; }

            public CustomResponseEventArgs(string url)
            {
                this.Url = url;
            }
        }
        private delegate void CustomResponseHandler(object sender, CustomResponseEventArgs e);

        #region Element Picker for XAML Inspection

        void StartElementPickerForInspection()
        {
            if (ButtonViewHideElementPickerForInspector.IsChecked != true)
                ButtonViewHideElementPickerForInspector.IsChecked = true;

            // Show the tutorial:
            InformationAboutHowThePickerWorks.Visibility = Visibility.Visible;

            XamlInspectionHelper.StartInspection();
        }

        void StopElementPickerForInspection()
        {
            // Make sure the ToggleButton is in the correct state:
            if (ButtonViewHideElementPickerForInspector.IsChecked == true)
                ButtonViewHideElementPickerForInspector.IsChecked = false;

            // Hide the tutorial:
            InformationAboutHowThePickerWorks.Visibility = Visibility.Collapsed;

            XamlInspectionHelper.StopInspection();
        }

        private void ButtonViewHideElementPickerForInspector_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonViewHideElementPickerForInspector.IsChecked == true)
                StartElementPickerForInspection();
            else
                StopElementPickerForInspection();
        }

        #endregion

        private void CheckBoxCORS_Checked(object sender, RoutedEventArgs e)
        {
            CrossDomainCallsHelper.IsBypassCORSErrors = true;
        }

        private void CheckBoxCORS_Unchecked(object sender, RoutedEventArgs e)
        {
            CrossDomainCallsHelper.IsBypassCORSErrors = false;
        }

        private void MetroWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F11 && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                if (BorderForXamlInspection.Visibility == Visibility.Visible)
                {
                    ButtonHideXamlTree_Click(sender, e);
                }
                else
                {
                    ButtonViewXamlTree_Click(sender, e);
                }
            }
        }
    }
}
