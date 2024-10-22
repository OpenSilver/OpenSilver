using DotNetForHtml5.Core;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenSilver.Maui;

public partial class WebViewManagerService : IWebViewManagerService
{
    public partial async Task InitializeWebViewAsync(IBlazorWebView blazorWebView, Action startAppAction)
    {
        ArgumentNullException.ThrowIfNull(blazorWebView);

        var webView = blazorWebView.Handler?.PlatformView as WebView2;
        if (webView == null) return;

        await webView.EnsureCoreWebView2Async();

        webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
        webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
        webView.CoreWebView2.Settings.AreHostObjectsAllowed = true;
        webView.CoreWebView2.Settings.IsScriptEnabled = true;

        var tcs = new TaskCompletionSource();

        webView.CoreWebView2.DOMContentLoaded += (_, _) =>
        {
            var dispatcher = Dispatcher.GetForCurrentThread();
            dispatcher?.Dispatch(() => OnLoaded(blazorWebView, tcs));
        };

        await tcs.Task;

        _ = Task.Run(startAppAction);
    }

    private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    private void OnLoaded(IBlazorWebView blazorWebView, TaskCompletionSource tcs)
    {
        try
        {
            var webView = blazorWebView.Handler?.PlatformView as WebView2;
            if (webView == null)
            {
                tcs.SetCanceled();
                return;
            }

            webView.WebMessageReceived += (_, e) =>
            {
                if (e.WebMessageAsJson.StartsWith("\"__bwv")) return;

                var message = JsonConvert.DeserializeObject<WebMessage>(e.TryGetWebMessageAsString(), jsonSettings);
                if (message == null) return;

                var callBack = new CSHTML5.Internal.OnCallbackSimulator();
                callBack.OnCallbackFromJavaScript(
                    message.CallbackId,
                    message.IdWhereCallbackArgsAreStored,
                    message.CallbackArgsObject,
                    message.ReturnValue
                );
            };

            Func<string, Task<string>> executeScriptAsyncAction = async (javaScript) =>
            {
                var cwv2 = webView?.CoreWebView2;
                if (cwv2 is null) return null;

                var result = await cwv2.ExecuteScriptAsync(javaScript);

                if (result is not null)
                {
                    result = result.Replace("\\\"", "\"");
                }

                return result;
            };

            var dispatcher = Dispatcher.GetForCurrentThread();

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
    }
}