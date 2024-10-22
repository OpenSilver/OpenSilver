using Microsoft.AspNetCore.Components.WebView.Maui;
using Android.Webkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Java.Interop;
using DotNetForHtml5.Core;

namespace OpenSilver.Maui;

public partial class WebViewManagerService : IWebViewManagerService
{
    public async partial Task InitializeWebViewAsync(IBlazorWebView blazorWebView, Action startAppAction)
    {
        ArgumentNullException.ThrowIfNull(blazorWebView);

        var webView = blazorWebView.Handler?.PlatformView as Android.Webkit.WebView;
        if (webView == null) return;

        webView.Settings.JavaScriptEnabled = true;
        webView.Settings.DomStorageEnabled = true;
        webView.Settings.DefaultTextEncodingName = "utf-8";

        webView.Settings.LoadWithOverviewMode = true;
        webView.Settings.UseWideViewPort = true;

        webView.Settings.BuiltInZoomControls = false;
        webView.Settings.DisplayZoomControls = false;
        webView.Settings.SetSupportZoom(false);

        webView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;

        webView.Settings.AllowFileAccess = true;
        webView.Settings.AllowUniversalAccessFromFileURLs = true;
        webView.Settings.AllowFileAccessFromFileURLs = true;
        webView.Settings.AllowContentAccess = true;

        var bridge = new JSBridge();
        webView.AddJavascriptInterface(bridge, "__openSilverBridge");

        var dispatcher = Dispatcher.GetForCurrentThread();
        var tcs = new TaskCompletionSource();

        bridge.DomContentLoaded += (_, _) =>
        {
            try
            {
                Func<string, Task<string>> executeScriptAsyncAction = (javaScript) =>
                {
                    var tcsScriptResult = new TaskCompletionSource<string>();

                    var valueCallback = new CustomValueCallback();
                    valueCallback.WebMessageReceived += (s, e) =>
                    {
                        var msg = e.Message;
                        if (msg is not null)
                        {
                            msg = msg.Replace("\\\"", "\"");
                        }

                        tcsScriptResult.SetResult(msg);
                    };

                    try
                    {
                        webView.EvaluateJavascript(javaScript, valueCallback);
                    }
                    catch (Exception ex)
                    {
                        tcsScriptResult.SetException(ex);
                    }

                    return tcsScriptResult.Task;
                };

                var handler = new MauiJavaScriptExecutionHandler(
                    executeScriptAsyncAction,
                    dispatcher.Dispatch,
                    dispatcher.DispatchAsync,
                    () => MainThread.IsMainThread
                );

                INTERNAL_Simulator.JavaScriptExecutionHandler = handler;
                INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround = true;

                Scheduler = new SingleThreadTaskScheduler();
                var factory = new TaskFactory(Scheduler);

                INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke = (method) => factory.StartNew(method);
                INTERNAL_Simulator.OpenSilverDispatcherInvoke = (method, timeout) => factory.StartNew(method);
                INTERNAL_Simulator.OpenSilverDispatcherCheckAccess = () => dispatcher.IsDispatchRequired;

                tcs.SetResult();
            }
            catch (Exception ex)
            { 
                tcs.SetException(ex);
            }
        };

        await tcs.Task;

        _ = Task.Run(startAppAction);
    }

    private class WebMessageReceivedEventArgs : EventArgs
    {
        public string? Message { get; set; }
    }

    private delegate void WebMessageReceivedEventHandler(object? sender, WebMessageReceivedEventArgs e);

    private class CustomValueCallback : Java.Lang.Object, IValueCallback
    {
        public WebMessageReceivedEventHandler? WebMessageReceived;

        public void OnReceiveValue(Java.Lang.Object? value)
        {
            WebMessageReceived?.Invoke(null, new WebMessageReceivedEventArgs { Message = (string)value });
        }
    }

    public class JSBridge : Java.Lang.Object
    {
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public EventHandler? DomContentLoaded;

        [JavascriptInterface]
        [Export("_domContentLoaded")]
        public void _domContentLoaded()
        {
            DomContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        [JavascriptInterface]
        [Export("postMessage")]
        public void PostMessage(string data)
        {
            if (data.StartsWith("\"__bwv")) return;

            var message = JsonConvert.DeserializeObject<WebMessage>(data, jsonSettings);
            if (message == null) return;

            var callBack = new CSHTML5.Internal.OnCallbackSimulator();
            callBack.OnCallbackFromJavaScript(
                message.CallbackId,
                message.IdWhereCallbackArgsAreStored,
                message.CallbackArgsObject,
                message.ReturnValue
            );
        }
    }
}