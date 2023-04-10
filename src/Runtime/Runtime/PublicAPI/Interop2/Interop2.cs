using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSHTML5;
using CSHTML5.Internal;
using CSHTML5.Types;
using static OpenSilver.Interop2Caller;

namespace OpenSilver
{
    public class Interop2
    {
        private class JsFuncInfo
        {
            public string TypeName;
            public IReadOnlyDictionary<int, (int ArgCount, InteropReturnType ReturnType)> Calls;
        }

        private class AsyncCallInfo
        {
            public string FuncName;
            public int Index;
            public string Javascript;

        }

        private static Dictionary<Type, JsFuncInfo> _types = new Dictionary<Type, JsFuncInfo>();
        private static List<AsyncCallInfo> _pendingAsyncCalls = new List<AsyncCallInfo>();

        // for debugging/testing only!
        public static bool DumpCreateFunction { get; set; } = false;
        public static bool DumpCalls { get; set; } =  false;

        public static Interop2OldImplementation Old { get; } = new Interop2OldImplementation();

        private static void CreateFunction<T>() where T : Enum
        {
            var type = typeof(T);
            var enums = type.GetMembers().OfType<FieldInfo>().Where(e => e.FieldType.BaseType == typeof(Enum)).ToList();
            Dictionary<int, string> subFunctions = new Dictionary<int, string>();
            var idx = 0;
            foreach (var e in enums)
            {
                var attr = e.GetCustomAttributes(typeof(JsCallAttribute), false);
                if (attr.Length != 1)
                    throw new Exception($"invalid JsCallAttribute for {e}");
                var jsForEnum = (attr[0] as JsCallAttribute).JS;

                var trueEnumValue = Enum.Parse(typeof(T), e.Name);
                var intIndex = (int)trueEnumValue;

                subFunctions.Add(intIndex, jsForEnum);
            }

            var enumName = typeof(T).ToString();
            try
            {
                var result = Interop2Caller.CreateFunction(enumName, subFunctions);
                if (!_types.ContainsKey(typeof(T)))
                    _types.Add(typeof(T), new JsFuncInfo { TypeName = enumName, Calls = result.ExtraInfo,  });

                if (DumpCreateFunction)
                    Console.WriteLine($"**** JS CREATED FUNC {enumName}\r\n{result.JS}");
            }
            catch (Exception e)
            {
                // in this case, something went wrong creating the function, very likely some syntax error
                // lets find out the exact enum value that caused the error
                ValidateFunctionEnums(enumName, subFunctions, e);
            }
        }

        private static void ValidateFunctionEnums(string funcName,IReadOnlyDictionary<int,string> functions, Exception originalException)
        {
            var full = Interop2Caller.GetJavascriptFunctionStringForEnum(functions).JS;
            var orderByIdx = functions.OrderBy(f => f.Key).ToList();
            for (int i = 1; i <= functions.Count; ++i)
            {
                var subList = orderByIdx.Take(i).ToList();
                var sub = subList. ToDictionary(kv => kv.Key, kv => kv.Value);
                try
                {
                    Interop2Caller.CreateFunction($"{funcName}_{i}", sub);
                    // if we get here, the function was successfully created
                }
                catch (Exception e)
                {
                    var index = subList.Last().Key;
                    var func = subList.Last().Value;
                    Console.WriteLine($"FATAL: can't create javascript function {funcName}:{index} -- js code:{func}\r\n\r\n" +
                                      $"FULL FUNCTION:\r\n{full}\r\n\r\nException:{e}");
                    return;
                }
            }

            // worst case scenario - the function was created successfully, but we still got an exception?
            Console.WriteLine($"FATAL: can't create javascript function {funcName}:\r\nFULL FUNCTION:\r\n{full}\r\n\r\nException: {originalException}");
        }

        private static object ExecuteJavaScript<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            ValidateArgs(func, subId, args.Length);

            return Interop2Caller.CallJavascriptFunction(func.TypeName, subId, args);
        }

        private static object ExecuteJavaScriptImpl<T>(T subId, IReadOnlyList<object> args) where T : Enum
        {
            // validation already done - also, for JsObjRef, I'm manually adding an argument 
            var func = _types[typeof(T)];
            return Interop2Caller.CallJavascriptFunction(func.TypeName, subId, args);
        }

        public static void VoidAsync<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            if (func.Calls[index].ReturnType != InteropReturnType.Void )
                throw new Exception($"JS function {func.TypeName}:{subId} does not return void. Prefix it with 'void;' in the enum's JsCall attribute");

            ValidateArgs(func, subId, args.Length);
            // the idea: compute the whole string now, so args[] can be released ASAP
            var all = Interop2Caller.ConvertArgsToString(args);
            var funcName = func.TypeName;

            AddAsyncCall(funcName, index, all);
        }

        // used by BulkInterop2
        internal static string CreateFunctionAndReturnName<T>() where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            return func.TypeName;
        }

        private static void ValidateArgs<T>(JsFuncInfo func, T subId, int argCount)
        {
            var index = (int)(object)subId;
            if (func.Calls[index].ArgCount != argCount)
                throw new Exception($"Function {func.TypeName}:{subId} has {func.Calls[index].ArgCount} arguments, but it's used with {argCount}");
        }

        // note: you won't be able to avoid specifying T type param here.
        // so either specify it, or call some of the predefined functions
        public static Result GetResult<Result, T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            ValidateArgs(func, subId, args.Length);
            var result = Interop2Caller.CallJavascriptFunction(func.TypeName, subId, args);

            if (result != null)
                return Interop.ConvertJavascriptResult<Result>(result);
            else
                return default;
        }
        private static Result TryExecuteJavaScriptGetResult<Result, T>(T subId, out bool nullOrUndefined, object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            nullOrUndefined = false;

            var func = _types[typeof(T)];
            ValidateArgs(func, subId, args.Length);

            var result = Interop2Caller.CallJavascriptFunction(func.TypeName, subId, args);
            if (result != null)
                try
                {
                    return Interop.ConvertJavascriptResult<Result>(result);
                }
                catch 
                {
                    // error at conversion -- assume invalid
                    nullOrUndefined = true;
                    return default;
                }
            else
            {
                nullOrUndefined = true;
                return default;
            }
        }

        public static void Void<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            if (func.Calls[index].ReturnType != InteropReturnType.Void)
                throw new Exception($"JS function {func.TypeName}:{subId} does not return void. Prefix it with 'void;' in the enum's JsCall attribute");

            ExecuteJavaScript(subId, args);
        }

        public static IDisposable JsObjRef<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            if (func.Calls[index].ReturnType != InteropReturnType.ReferenceId)
                throw new Exception($"JS function {func.TypeName}:{subId} does not return jsObjRefId. Prefix it with 'ref;' in the enum's JsCall attribute");

            ValidateArgs(func, subId, args.Length);
            var copy = args.ToList();
            var refId = INTERNAL_InteropImplementation.NewReferenceId().ToString();
            copy.Add(refId);
            var value = ExecuteJavaScriptImpl(subId, copy);
            return new INTERNAL_JSObjectReference(value, refId, "interop2");
        }

        public static IDisposable JsObjRefAsync<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            if (func.Calls[index].ReturnType != InteropReturnType.ReferenceId)
                throw new Exception($"JS function {func.TypeName}:{subId} does not return jsObjRefId. Prefix it with 'ref;' in the enum's JsCall attribute");

            ValidateArgs(func, subId, args.Length);
            var copy = args.ToList();
            var refId = INTERNAL_InteropImplementation.NewReferenceId().ToString();
            copy.Add(refId);

            // the idea: compute the whole string now, so args[] can be released ASAP
            var all = Interop2Caller.ConvertArgsToString(copy);
            var funcName = func.TypeName;
            AddAsyncCall(funcName, index, all);
            return new INTERNAL_JSObjectReference(null, refId, "interop2");
        }

        internal static void AddAsyncCall(string funcName, int index, string javascript)
        {
            // the idea:
            // when we have async calls, we need to have them executed sequentially
            // however, this gets complicated when we're trying to do this with both Interop. and Interop2. calls,
            // since Interop2. calls are executed one by one, while Interop. calls are appended to a byte-array and executed all at once
            //
            // so in order to maintain this sequential behavior, when we execute an Interop2 Async call, we flush the Interop. queue,
            // and vice versa
            INTERNAL_ExecuteJavaScript.JavaScriptRuntime.Flush();

            bool needsQueueAction = false;
            lock (_pendingAsyncCalls)
            {
                needsQueueAction = _pendingAsyncCalls.Count == 0;
                _pendingAsyncCalls.Add(new AsyncCallInfo
                {
                    FuncName = funcName, Index = index, Javascript = javascript,
                });
            }

            if (needsQueueAction)
                INTERNAL_DispatcherHelpers.QueueAction(Flush);
        }

        internal static void Flush()
        {
            lock (_pendingAsyncCalls)
            {
                foreach (var func in _pendingAsyncCalls)
                    Interop2Caller.CallJavascriptFunctionWithAllArgs(func.FuncName, func.Index, func.Javascript);
                _pendingAsyncCalls.Clear();
            }
        }

        public static bool Boolean<T>(T subId, params object[] args) where T : Enum
        {
            return GetResult<bool, T>(subId, args);
        }
        public static int Int<T>(T subId, params object[] args) where T : Enum
        {
            return GetResult<int, T>(subId, args);
        }
        public static long Long<T>(T subId, params object[] args) where T : Enum
        {
            return GetResult<long, T>(subId, args);
        }
        public static double Double<T>(T subId, params object[] args) where T : Enum
        {
            return GetResult<double, T>(subId, args);
        }
        public static string String<T>(T subId, params object[] args) where T : Enum
        {
            return GetResult<string, T>(subId, args);
        }




        public static bool SafeBoolean<T>(T subId, params object[] args) where T : Enum
        {
            return TryExecuteJavaScriptGetResult<bool, T>(subId, out var ignore, args);
        }
        // returns MinValue on null-or-undefined
        public static int SafeInt<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<int, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? int.MinValue : result;
        }
        // returns MinValue on null-or-undefined
        public static long SafeLong<T>(T subId, params object[] args) where T : Enum
        {
            var result =TryExecuteJavaScriptGetResult<long, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? long.MinValue : result;
        }
        // returns NaN on null-or-undefined
        public static double SafeDouble<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<double, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? double.NaN : result;
        }
        // returns "" on null-or-undefined
        public static string SafeString<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<string, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? "" : result;
        }

    }
}
