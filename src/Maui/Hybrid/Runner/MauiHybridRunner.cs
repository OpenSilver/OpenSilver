
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
using DotNetForHtml5.Core;
using Microsoft.JSInterop;
using OpenSilver.MauiHybrid.JavaScript;
using OpenSilver.MauiHybrid.Threading;
using System.Diagnostics;
using System.Text.Json;

namespace OpenSilver.MauiHybrid.Runner
{
    public class MauiHybridRunner(IJSRuntime jsRuntime) : IMauiHybridRunner
    {
        private const string InvokeJsMethod = "_hybridRuntime.invokeJS";
        private const string StartAsyncJsMethod = "_hybridRuntime.startAsync";
        private static bool _isRunApplicationCalled;

        private static readonly Lazy<OnCallbackSimulator> _onCallbackSimulator =
            new Lazy<OnCallbackSimulator>(() => new OnCallbackSimulator());
        private static OnCallbackSimulator OnCallbackSimulator => _onCallbackSimulator.Value;

        #region Interface implementation
        public async Task<T> RunApplicationAsync<T>(Func<Task<T>> createAppDelegate) where T : System.Windows.Application
        {
            ArgumentNullException.ThrowIfNull(createAppDelegate);

            if (!MainThread.IsMainThread)
            {
                throw new InvalidOperationException("RunApplicationAsync must be called on the main thread.");
            }

            if (_isRunApplicationCalled)
            {
                throw new InvalidOperationException("RunApplicationAsync can only be called once.");
            }
            _isRunApplicationCalled = true;

            await StartJsAsync();
            var context = InitializeOpenSilver();
            var tcs = new TaskCompletionSource<T>();

            context.Post(async (s) => {
                try
                {
                    var app = await createAppDelegate();
                    tcs.SetResult(app);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            var result = await tcs.Task;
            return result;
        }

        public Task<T> RunApplicationAsync<T>(Func<T> createAppDelegate) where T : System.Windows.Application
        {
            return RunApplicationAsync(() => Task.FromResult(createAppDelegate()));
        }

        public Task<T> RunApplicationAsync<T>() where T : System.Windows.Application, new()
            => RunApplicationAsync(() => new T());

        #endregion

        private static object? GetValueFromJsonElement(JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Object or JsonValueKind.Array => jsonElement,
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.GetDouble(),
                JsonValueKind.True or JsonValueKind.False => jsonElement.GetBoolean(),
                JsonValueKind.Undefined or JsonValueKind.Null or _ => null,
            };
        }

        private async Task<object?> ExecuteJavaScriptAsync(string javaScript)
        {
            try
            {
                var result = await jsRuntime.InvokeAsync<JsonElement>(InvokeJsMethod, javaScript);
                return GetValueFromJsonElement(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
                return null;
            }
        }

        private async Task StartJsAsync()
        {
            var res = await jsRuntime.InvokeAsync<bool>(StartAsyncJsMethod);
            if (!res)
            {
                throw new InvalidOperationException("An unexpected error occurred. Please check the browser console for more details.");
            }
        }

        private BackgroundThreadSynchronizationContext InitializeOpenSilver()
        {
            var dispatcher = Dispatcher.GetForCurrentThread() ??
                throw new InvalidOperationException("This method must be invoked on the thread associated with the UI Dispatcher.");

            var handler = new MauiJavaScriptExecutionHandler(
                ExecuteJavaScriptAsync,
                dispatcher.Dispatch,
                dispatcher.DispatchAsync,
                () => MainThread.IsMainThread
            );

            INTERNAL_Simulator.JavaScriptExecutionHandler = handler;
            INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround = true;

            var context = new BackgroundThreadSynchronizationContext();

            var thread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(context);

                context.RunMessageLoop();
            })
            {
                IsBackground = true
            };
            thread.Start();

            INTERNAL_Simulator.OpenSilverDispatcherBeginInvoke = (method) => context.Post((s) => method(), null);
            INTERNAL_Simulator.OpenSilverDispatcherInvoke = (method, _) => {
                if (context.CheckAccess())
                {
                    method();
                }
                else
                {
                    context.Send((s) => method(), null);
                }
            };
            INTERNAL_Simulator.OpenSilverDispatcherCheckAccess = context.CheckAccess;

            return context;
        }

        [JSInvokable]
        public static void InkoveFromJs(int callbackId, string idWhereCallbackArgsAreStored,
            JsonElement[] callbackArgsObject)
        {
            OnCallbackSimulator.OnCallbackFromJavaScript(
                callbackId,
                idWhereCallbackArgsAreStored,
                callbackArgsObject.Select(GetValueFromJsonElement).ToArray(),
                false
            );
        }

        [JSInvokable]
        public static void ErrorFromJs(string idWhereCallbackArgsAreStored)
        {
            OnCallbackSimulator.OnCallbackFromJavaScriptError(
                idWhereCallbackArgsAreStored
            );
        }
    }
}
