
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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CSHTML5.Internal;
using CSHTML5.Types;
using DotNetForHtml5;
using DotNetForHtml5.Core;
using OpenSilver.Buffers;
using OpenSilver.Internal;

namespace OpenSilver;

/// <summary>
/// Provides static methods for executing JavaScript code from within C#.
/// </summary>
public static partial class Interop
{
    private static readonly ReferenceIDGenerator _refIdGenerator = new();
    private static readonly SynchronyzedStore<string> _javascriptCallsStore = new();
    private static readonly CharArrayBuilder _buffer = new();

    private static int _dumpAllJavascriptObjectsEveryMs;
    private static bool _isDispatcherPending;

    // for debugging/testing only
    // if > 0, we're dumping All JS objects every X millis
    public static int DumpAllJavascriptObjectsEveryMs
    {
        get => _dumpAllJavascriptObjectsEveryMs;
        set
        {
            if (_dumpAllJavascriptObjectsEveryMs != value)
            {
                _dumpAllJavascriptObjectsEveryMs = value;
                if (_dumpAllJavascriptObjectsEveryMs > 0)
                {
                    JSObjectReferenceHolder.Instance.StartTracking(_dumpAllJavascriptObjectsEveryMs);
                }
            }
        }
    }

    // for debugging/testing only
    // if true, we dump stack trace when dumping the JS Ref objects
    public static bool DumpAllJavascriptObjectsVerbose { get; set; } = true;

    // how many functions (from the stack trace) to dump? (since the stack trace can end up being insanely huge)
    public static int DumpAllJavascriptObjectsStackTraceCount { get; set; } = 15;

    // filter when dumping added Javascript Objects -- perhaps you already know some objects that are persistent throughout most of the app
    // in this case, filter those out, so you can focus on the leaks
    //
    // function-name, javascript-code
    public static Func<string, string, bool> DumpAllJavascriptObjectsFilter { get; set; } = (a, b) => true;

    // the idea: in release AOT, Console.WriteLine doesn't work in OpenSilver, but it works on client code
    public static Action<string> DumpAllJavascriptObjectsLogger { get; set; } = Console.WriteLine;

    /// <summary>
    /// Returns True is the app is running in the simulator, and False otherwise. 
    /// </summary>
    public static bool IsRunningInTheSimulator => INTERNAL_Simulator.IsRunningInTheSimulator_WorkAround;

    /// <summary>
    /// Returns True if the app is running inside the Xaml Designer, and False otherwise.
    /// </summary>
    public static bool IsRunningInTheXamlDesigner { get; private set; }

    internal static IPendingJavascript JavaScriptRuntime { get; private set; }

    internal static void SetRuntime(IJavaScriptExecutionHandler jsRuntime)
    {
        Debug.Assert(jsRuntime is not null);

        if (jsRuntime is IWebAssemblyExecutionHandler wasmHandler)
        {
            JavaScriptRuntime = new PendingJavascript(wasmHandler, _buffer);
        }
        else
        {
            JavaScriptRuntime = new PendingJavascriptSimulator(jsRuntime, _buffer);
        }
    }

    internal static bool EnableInteropLogging { get; set; }

    public static T1 ExecuteJavaScriptGetResult<T1>(string javascript)
    {
        var result = ExecuteJavaScriptSync(javascript, -1, true, true);
        T1 t1 = ConvertJavascriptResult<T1>(result);
        return t1;
    }

    internal static void ExecuteJavaScriptVoid(string javascript, bool flushQueue)
        => ExecuteJavaScriptSync(javascript, -1, false, flushQueue);

    internal static void ExecuteJavaScriptVoid(string format, bool flushQueue, params object[] variables)
    {
        string javascript = FormatArguments(format, variables);
        ExecuteJavaScriptVoid(javascript, flushQueue);
    }

    public static void ExecuteJavaScriptVoid(string javascript, params object[] variables)
        => ExecuteJavaScriptVoid(javascript, false, variables);

    public static void ExecuteJavaScriptVoid(string javascript)
        => ExecuteJavaScriptVoid(javascript, true);

    public static void ExecuteJavaScriptVoidAsync(string javascript, params object[] variables)
    {
        javascript = FormatArguments(javascript, variables);
        ExecuteJavaScriptVoidAsync(javascript);
    }

    public static void ExecuteJavaScriptVoidAsync(string javascript)
    {
        JavaScriptRuntime.AcquireLock();
        _buffer.AppendLine(javascript);
        JavaScriptRuntime.ReleaseLock();
        BeginInvokeJavaScript();
    }

    internal static void ExecuteJavaScriptVoidAsync(ref AppendInterpolatedStringHandler _)
    {
        _buffer.AppendLine();
        JavaScriptRuntime.ReleaseLock();
        BeginInvokeJavaScript();
    }

    private static void BeginInvokeJavaScript()
    {
        if (!_isDispatcherPending)
        {
            _isDispatcherPending = true;
            Dispatcher.CurrentDispatcher.BeginInvoke(ExecutePendingJavaScriptCode);
        }
    }

    private static void ExecutePendingJavaScriptCode()
    {
        _isDispatcherPending = false;
        JavaScriptRuntime.Flush();
    }

    /// <summary>
    /// Allows calling JavaScript code from within C#.
    /// </summary>
    /// <param name="javascript">The JavaScript code to execute.</param>
    /// <returns>The result, if any, of the JavaScript call.</returns>
    public static IDisposable ExecuteJavaScript(string javascript)
        => ExecuteJavaScriptCore(javascript);

    /// <summary>
    /// Allows calling JavaScript code from within C#.
    /// </summary>
    /// <param name="javascript">The JavaScript code to execute.</param>
    /// <param name="variables">The objects to use inside the JavaScript call.</param>
    /// <returns>The result, if any, of the JavaScript call.</returns>
    public static IDisposable ExecuteJavaScript(string javascript, params object[] variables)
        => ExecuteJavaScriptCore(javascript, variables);

    private static JSObjectRef ExecuteJavaScriptCore(string format, params object[] variables)
    {
        int referenceId = _refIdGenerator.NewId();
        string javascript = FormatArguments(format, variables);

        object jsResult = ExecuteJavaScriptSync(javascript, referenceId, true, true);

        return new JSObjectRef(jsResult, referenceId.ToString(), javascript);
    }

    /// <summary>
    /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
    /// </summary>
    /// <param name="javascript">The JavaScript code to execute.</param>
    public static IDisposable ExecuteJavaScriptAsync(string javascript)
        => ExecuteJavaScriptAsyncCore(javascript);

    /// <summary>
    /// Allows calling JavaScript code from within C#. The call will be asynchronous when run in the Simulator.
    /// </summary>
    /// <param name="javascript">The JavaScript code to execute.</param>
    /// <param name="variables">The objects to use inside the JavaScript call.</param>
    public static IDisposable ExecuteJavaScriptAsync(string javascript, params object[] variables)
        => ExecuteJavaScriptAsyncCore(javascript, variables);

    private static JSObjectRef ExecuteJavaScriptAsyncCore(string format, params object[] variables)
    {
        int referenceId = _refIdGenerator.NewId();
        string javascript = FormatArguments(format, variables);

        ExecuteJavaScriptVoidAsync(WrapReferenceIdInJavascriptCall(javascript, referenceId));

        return new JSObjectRef(null, referenceId.ToString(), javascript);
    }

    internal static Task<object> ExecuteJavaScriptAsync(string javascript, int referenceId, bool wantsResult)
        => Task.Run(() => ExecuteJavaScriptSync(javascript, referenceId, wantsResult, false));

    internal static double ExecuteJavaScriptDouble(string javascript, bool flushQueue = true)
    {
        object value = ExecuteJavaScriptSync(javascript, -1, true, flushQueue);
        return ConvertJSResultToDouble(value);
    }

    internal static int ExecuteJavaScriptInt32(string javascript, bool flushQueue = true)
    {
        object value = ExecuteJavaScriptSync(javascript, -1, true, flushQueue);
        return ConvertJSResultToInt32(value);
    }

    internal static string ExecuteJavaScriptString(string javascript, bool flushQueue = true)
    {
        object value = ExecuteJavaScriptSync(javascript, -1, true, flushQueue);
        return ConvertJSResultToString(value);
    }

    internal static bool ExecuteJavaScriptBoolean(string javascript, bool flushQueue = true)
    {
        object value = ExecuteJavaScriptSync(javascript, -1, true, flushQueue);
        return ConvertJSResultToBoolean(value);
    }

    private static object ExecuteJavaScriptSync(string javascript, int referenceId, bool wantsResult, bool flush)
    {
        if (flush)
        {
            JavaScriptRuntime.Flush();
        }

        var result = JavaScriptRuntime.ExecuteJavaScript(javascript, referenceId, wantsResult);
        return result;
    }

    /// <summary>
    /// Register a .NET method to allow it to be invoked from JavaScript code.
    /// </summary>
    /// <returns>
    /// An <see cref="IDisposable"/> object that must be disposed after it has been used.
    /// If the object is not disposed, it will be leaked.
    /// </returns>
    public static IDisposable CreateJavascriptCallback(Delegate d) => JavaScriptCallback.Create(d);

    private static T ConvertJavascriptResult<T>(object value)
    {
        Type t = typeof(T);
        if (t == typeof(string))
        {
            string s = ConvertJSResultToString(value);
            return Unsafe.As<string, T>(ref s);
        }
        else if (t == typeof(double))
        {
            double d = ConvertJSResultToDouble(value);
            return Unsafe.As<double, T>(ref d);
        }
        else if (t == typeof(int))
        {
            int i = ConvertJSResultToInt32(value);
            return Unsafe.As<int, T>(ref i);
        }
        else if (t == typeof(bool))
        {
            bool b = ConvertJSResultToBoolean(value);
            return Unsafe.As<bool, T>(ref b);
        }
        else if (t == typeof(char))
        {
            char c = ConvertJSResultToChar(value);
            return Unsafe.As<char, T>(ref c);
        }
        else if (t == typeof(float))
        {
            float f = ConvertJSResultToSingle(value);
            return Unsafe.As<float, T>(ref f);
        }
        else if (t == typeof(byte))
        {
            byte b = ConvertJSResultToByte(value);
            return Unsafe.As<byte, T>(ref b);
        }
        else if (t == typeof(uint))
        {
            uint i = ConvertJSResultToUInt32(value);
            return Unsafe.As<uint, T>(ref i);
        }
        else if (t == typeof(long))
        {
            long l = ConvertJSResultToInt64(value);
            return Unsafe.As<long, T>(ref l);
        }
        else if (t == typeof(ulong))
        {
            ulong l = ConvertJSResultToUInt64(value);
            return Unsafe.As<ulong, T>(ref l);
        }
        else if (t == typeof(short))
        {
            short s = ConvertJSResultToInt16(value);
            return Unsafe.As<short, T>(ref s);
        }
        else if (t == typeof(decimal))
        {
            decimal d = ConvertJSResultToDecimal(value);
            return Unsafe.As<decimal, T>(ref d);
        }
        else if (t == typeof(DateTime))
        {
            DateTime d = ConvertJSResultToDateTime(value);
            return Unsafe.As<DateTime, T>(ref d);
        }
        else
        {
            throw new ArgumentException($"Type '{t.FullName}' is not supported.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ConvertJSResultToString(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToString(value) : Convert.ToString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double ConvertJSResultToDouble(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToDouble(value) : Convert.ToDouble(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ConvertJSResultToInt32(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToInt32(value) : Convert.ToInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ConvertJSResultToBoolean(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToBoolean(value) : Convert.ToBoolean(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char ConvertJSResultToChar(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToChar(value) : Convert.ToChar(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ConvertJSResultToSingle(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToSingle(value) : Convert.ToSingle(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ConvertJSResultToByte(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToByte(value) : Convert.ToByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ConvertJSResultToUInt32(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToUInt32(value) : Convert.ToUInt32(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ConvertJSResultToInt64(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToInt64(value) : Convert.ToInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ConvertJSResultToUInt64(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToUInt64(value) : Convert.ToUInt64(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static short ConvertJSResultToInt16(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToInt16(value) : Convert.ToInt16(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal ConvertJSResultToDecimal(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToDecimal(value) : Convert.ToDecimal(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateTime ConvertJSResultToDateTime(object value) =>
        IsRunningInTheSimulator ? JSObjectRef.ToDateTime(value) : Convert.ToDateTime(value);

    internal static string FormatArguments(string format, params object[] variables)
    {
        // If the javascript code has references to previously obtained JavaScript objects,
        // we replace those references with calls to the "document.jsObjRef"
        // dictionary.
        // Note: we iterate in reverse order because, when we replace ""$" + i.ToString()", we
        // need to replace "$10" before replacing "$1", otherwise it thinks that "$10" is "$1"
        // followed by the number "0". To reproduce the issue, call "ExecuteJavaScript" passing
        // 10 arguments and using "$10".
        for (int i = variables.Length - 1; i >= 0; i--)
        {
            format = format.Replace($"${i}", GetVariableStringForJS(variables[i]));
        }

        return format;
    }

    internal static string GetVariableStringForJS(object variable)
    {
        if (variable is Delegate d)
        {
            variable = JavaScriptCallback.Create(d);
        }

        if (variable is IJavaScriptConvertible jsConvertible)
        {
            return GetVariableStringForJS(jsConvertible);
        }
        else if (variable == null)
        {
            //--------------------
            // Null
            //--------------------

            return "null";
        }
        else
        {
            //--------------------
            // Simple value types or other objects
            // (note: this includes objects that
            // override the "ToString" method, such
            // as the class "Uri")
            //--------------------

            return INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(variable);
        }
    }

    internal static string GetVariableStringForJS(IJavaScriptConvertible jsObject)
    {
        Debug.Assert(jsObject is not null);
        return jsObject.ToJavaScriptString();
    }

    internal static string WrapReferenceIdInJavascriptCall(string javascript, int referenceId)
    {
        // Change the JS code to call ShowErrorMessage in case of error:
        string errorCallBackId = _javascriptCallsStore.Add(javascript).ToString();

        javascript = $"document.callScriptSafe(\"{referenceId}\",\"{INTERNAL_HtmlDomManager.EscapeStringForUseInJavaScript(javascript)}\",{errorCallBackId})";
        return javascript;
    }

    internal static string GetJavaScript(int id) => _javascriptCallsStore.Get(id);

    /// <summary>
    /// Returns the HTML Div that is associated to the specified FrameworkElement.
    /// Note: the FrameworkElement must be in the visual tree. Consider calling this
    /// method from the 'Loaded' event to ensure that the element is in the visual tree.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static object GetDiv(UIElement element)
    {
        return element.OuterDiv;
    }

    /// <summary>
    /// Check if the given jsnode is undefined
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsUndefined(object jsObject)
    {
        return ((JSObjectRef)jsObject).IsUndefined();
    }

    /// <summary>
    /// Check if the given jsnode is undefined
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNull(object jsObject)
    {
        return ((JSObjectRef)jsObject).IsNull();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    internal ref struct AppendInterpolatedStringHandler
    {
        public AppendInterpolatedStringHandler(int literalLength, int formattedCount)
        {
            JavaScriptRuntime.AcquireLock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(string value) => _buffer.Append(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted(string value) => _buffer.Append(value);

        public void AppendFormatted<T>(T value) => _buffer.Append(value.ToString());
    }
}
