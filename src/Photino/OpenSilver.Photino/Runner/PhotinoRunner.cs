
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

using OpenSilver.Photino.JavaScript;
using OpenSilver.MauiHybrid.Threading;
using System.Text.Json;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using Photino.NET;
using System.Collections.Concurrent;
using OpenSilver.Internal.Xaml;

namespace OpenSilver.Photino.Runner
{
    internal sealed class PhotinoRunner(PhotinoWindow window)
    {
        private const string IdKey = "id";
        private const string TypeKey = "type";
        private const string ResultKey = "result";
        private const string CallbackIdKey = "callbackId";
        private const string IdWhereCallbackArgsAreStoredKey = "idWhereCallbackArgsAreStored";
        private const string CallbackArgsObject = "callbackArgsObject";

        private const string ResponseMessageType = "response";
        private const string InvokeDotNetMessageType = "invoke-net";
        private const string StartMessageType = "start";
        private const string JsErrorMessageType = "js-error";

        private static bool _isRunApplicationCalled;

        private int _idCounter = 0;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<object?>> _communication = new();

        private readonly TaskCompletionSource<bool> _jsStarted = new();

        private static readonly Lazy<OnCallbackSimulator> _onCallbackSimulator =
            new(() => new OnCallbackSimulator());
        private static OnCallbackSimulator OnCallbackSimulator => _onCallbackSimulator.Value;

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

        private async Task<object?> ExecuteJavaScriptAsync(string js)
        {
            try
            {
                var id = Interlocked.Increment(ref _idCounter);
                var tcs = new TaskCompletionSource<object?>();
                _communication[id] = tcs;
                var message = JsonSerializer.Serialize(new { id, js });
                window.SendWebMessage(message);
                var result = await tcs.Task;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private static BackgroundThreadSynchronizationContext InitializeOpenSilver()
        {
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
            INTERNAL_Simulator.OpenSilverDispatcherInvoke = (method, _) =>
            {
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

        private static void InkoveFromJs(int callbackId, string idWhereCallbackArgsAreStored,
            JsonElement[] callbackArgsObject)
        {
            OnCallbackSimulator.OnCallbackFromJavaScript(
                callbackId,
                idWhereCallbackArgsAreStored,
                callbackArgsObject.Select(GetValueFromJsonElement).ToArray(),
                false
            );
        }

        private static void ErrorFromJs(string idWhereCallbackArgsAreStored)
        {
            OnCallbackSimulator.OnCallbackFromJavaScriptError(
                idWhereCallbackArgsAreStored
            );
        }

        public async void RunApplicationAsync<T>(Func<Task<T>> createAppDelegate) where T : System.Windows.Application
        {
            ArgumentNullException.ThrowIfNull(createAppDelegate);

            if (_isRunApplicationCalled)
            {
                throw new InvalidOperationException("RunApplicationAsync can only be called once.");
            }
            _isRunApplicationCalled = true;

            window.RegisterWebMessageReceivedHandler((object? sender, string message) =>
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;

                var typeValue = root.GetProperty(TypeKey).GetString();

                if (typeValue == ResponseMessageType)
                {
                    var id = root.GetProperty(IdKey).GetInt32();
                    if (_communication.TryRemove(id, out var tcs))
                    {
                        var res = root.GetProperty(ResultKey);
                        if (res is JsonElement je)
                        {
                            tcs.SetResult(GetValueFromJsonElement(je));
                        }
                        else
                        {
                            tcs.SetResult(res);
                        }
                    }
                }
                else if (typeValue == InvokeDotNetMessageType)
                {
                    InkoveFromJs(root.GetProperty(CallbackIdKey).GetInt32(),
                        root.GetProperty(IdWhereCallbackArgsAreStoredKey).GetString() ?? "",
                        root.GetProperty(CallbackArgsObject).EnumerateArray().ToArray());
                }
                else if (typeValue == JsErrorMessageType)
                {
                    ErrorFromJs(root.GetProperty(IdWhereCallbackArgsAreStoredKey).GetString() ?? "");
                }
                else if (typeValue == StartMessageType)
                {
                    _jsStarted.SetResult(true);
                }
            });

            var context = InitializeOpenSilver();
            var handler = new PhotinoExecutionHandler(ExecuteJavaScriptAsync, a => window.Invoke(a));
            await _jsStarted.Task;

            var tcs = new TaskCompletionSource<T>();

            context.Post(async (s) =>
            {
                DotNetForHtml5.Cshtml5Initializer.Initialize(handler);

                try
                {
                    var app = await createAppDelegate();

                    if (app is IComponentConnector componentConnector)
                    {
                        componentConnector.InitializeComponent();
                    }

                    tcs.SetResult(app);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);
        }
    }
}
