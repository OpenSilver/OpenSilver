using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenSilver
{
    public class Interop2
    {
        private class JsFuncInfo
        {
            public string TypeName;
            public IReadOnlyDictionary<int, (int ArgCount, bool ReturnsVoid)> Calls;
        }

        private static Dictionary<Type, JsFuncInfo> _types = new Dictionary<Type, JsFuncInfo>();

        // for debugging/testing only!
        public static bool DumpCreateFunction { get; set; } = false;

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
            var result = Interop2Caller.CreateFunction(enumName, subFunctions);
            if (!_types.ContainsKey(typeof(T)))
                _types.Add(typeof(T), new JsFuncInfo { TypeName = enumName, Calls = result.ExtraInfo });

            if (DumpCreateFunction)
                Console.WriteLine($"**** JS CREATED FUNC {enumName}\r\n{result.JS}");
        }

        // FIXME later: validate the number of args you're sending to JS
        public static object ExecuteJavaScript<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            ValidateArgs(func, index, args.Length);
            return Interop2Caller.CallJavascriptFunction(func.TypeName, index, args);
        }

        // used by BulkInterop2
        internal static string CreateFunctionAndReturnName<T>() where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            return func.TypeName;
        }

        private static void ValidateArgs(JsFuncInfo func, int index, int argCount)
        {
            if (func.Calls[index].ArgCount != argCount)
                throw new Exception($"Function {func.TypeName}:{index} has {func.Calls[index].ArgCount} arguments, but it's used with {argCount}");
        }

        // note: you won't be able to avoid specifying T type param here.
        // so either specify it, or call some of the predefined functions
        public static Result ExecuteJavaScriptGetResult<Result, T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            ValidateArgs(func, index, args.Length);
            var result = Interop2Caller.CallJavascriptFunction(func.TypeName, index, args);
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
            var index = (int)(object)subId;
            ValidateArgs(func, index, args.Length);
            var result = Interop2Caller.CallJavascriptFunction(func.TypeName, index, args);
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

        public static void ExecuteJavaScriptVoid<T>(T subId, params object[] args) where T : Enum
        {
            if (!_types.ContainsKey(typeof(T)))
                CreateFunction<T>();

            var func = _types[typeof(T)];
            var index = (int)(object)subId;
            if (!func.Calls[index].ReturnsVoid)
                throw new Exception($"JS function {func.TypeName}:{subId} does not return void. Prefix it with 'void;' in the enum's JsCall attribute");

            ExecuteJavaScript(subId, args);
        }

        public static bool ExecuteJavaScriptBoolean<T>(T subId, params object[] args) where T : Enum
        {
            return ExecuteJavaScriptGetResult<bool, T>(subId, args);
        }
        public static int ExecuteJavaScriptInt<T>(T subId, params object[] args) where T : Enum
        {
            return ExecuteJavaScriptGetResult<int, T>(subId, args);
        }
        public static long ExecuteJavaScriptLong<T>(T subId, params object[] args) where T : Enum
        {
            return ExecuteJavaScriptGetResult<long, T>(subId, args);
        }
        public static double ExecuteJavaScriptDouble<T>(T subId, params object[] args) where T : Enum
        {
            return ExecuteJavaScriptGetResult<double, T>(subId, args);
        }
        public static string ExecuteJavaScriptString<T>(T subId, params object[] args) where T : Enum
        {
            return ExecuteJavaScriptGetResult<string, T>(subId, args);
        }




        public static bool SafeExecuteJavaScriptBoolean<T>(T subId, params object[] args) where T : Enum
        {
            return TryExecuteJavaScriptGetResult<bool, T>(subId, out var ignore, args);
        }
        // returns MinValue on null-or-undefined
        public static int SafeExecuteJavaScriptInt<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<int, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? int.MinValue : result;
        }
        // returns MinValue on null-or-undefined
        public static long SafeExecuteJavaScriptLong<T>(T subId, params object[] args) where T : Enum
        {
            var result =TryExecuteJavaScriptGetResult<long, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? long.MinValue : result;
        }
        // returns NaN on null-or-undefined
        public static double SafeExecuteJavaScriptDouble<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<double, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? Double.NaN : result;
        }
        // returns "" on null-or-undefined
        public static string SafeExecuteJavaScriptString<T>(T subId, params object[] args) where T : Enum
        {
            var result = TryExecuteJavaScriptGetResult<string, T>(subId, out var nullOrUndefined, args);
            return nullOrUndefined ? "" : result;
        }

    }
}
