using System;
using System.Collections.Generic;
using System.Text;
using CSHTML5;
using CSHTML5.Internal;
using CSHTML5.Types;

namespace OpenSilver
{
    public class Interop2OldImplementation
    {
        public Interop2OldImplementation()
        {
        }

        public void Void<T>(T subId, string javascript, params object[] vars) where T : Enum
        {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, vars);
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }

            Interop.ExecuteJavaScriptVoid(javascript);
        }

        public void VoidAsync<T>(T subId,string javascript) where T : Enum
        {
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }

            Interop.ExecuteJavaScriptFastAsync(javascript);
        }

        public IDisposable Async<T>(T subId,string javascript, params object[] vars) where T : Enum
        {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, vars);
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }

            return Interop.ExecuteJavaScriptAsync(javascript);
        }

        public string String<T>(T subId, string javascript) where T : Enum
        {
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }
            return Interop.ExecuteJavaScriptString(javascript);
        }
        public int Int<T>(T subId, string javascript) where T : Enum
        {
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }
            return Interop.ExecuteJavaScriptInt32(javascript);
        }
        public IDisposable JsObjRef<T>(T subId, string javascript, params object[] vars) where T : Enum
        {
            javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, vars);
            if (Interop2.DumpCalls)
            {
                var funcName = Interop2.CreateFunctionAndReturnName<T>();
                var friendlyName = funcName.Contains(".") ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;

                Console.WriteLine($" *** JS OLD {friendlyName}[{subId}] {javascript.Replace("\b",", ")}");
            }
            return Interop.ExecuteJavaScript(javascript);
        }
    }
}
