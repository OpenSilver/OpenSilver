using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSHTML5;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace OpenSilver
{
    internal class Interop2Caller
    {
        // jsObjRef,id,arrayindex

        public (string VariableName, string VariableDefinition) JsReferenceObject(object o, int idx, int subIdx)
        {
            var js = INTERNAL_InteropImplementation.GetVariableStringForJS(o);
            var name = $"obj{idx}_{subIdx}";
            var vd = $"const {name} = {js};";
            return (name, vd);
        }

        // IMPORTANT: $0..$20, if present, are handled first
        //
        // Examples:
        //
        // $"{sEvent}.shiftKey || false || $0 || $1"
        // $"{sEvent}.shiftKey || false"
        // $"{sEvent}.changedTouches[0].pageX + '|' + {sEvent}.changedTouches[0].pageY"
        // $"({sElement}.getBoundingClientRect().left - document.body.getBoundingClientRect().left) + '|' + ({sElement}.getBoundingClientRect().top - document.body.getBoundingClientRect().top)"
        // $"{sEvent}.doNotReroute = true"
        // 
        // $"{sRequest}.setRequestHeader({sKey}, {sHeader})"
        // $"{sRequest}.open({sMethod}, {sAddress}, {sAsync})"
        // $"{sRequest}.timeout = {sTimeout}"
        // $"{sRequest}.onload = {sCallback}"
        //
        // $"(function(cvs) {{ const ctx = cvs.getContext('2d'); ctx.moveTo({_lastPos.X.ToInvariantString()}, {_lastPos.Y.ToInvariantString()}); ctx.lineTo({_mousePos.X.ToInvariantString()}, {_mousePos.Y.ToInvariantString()}); ctx.stroke(); }})({sCanvas})"
        private static (string Js, int ArgCount, bool ReturnsVoid) CreateJavascriptCase(string str, int caseIdx)
        {
            // the args - each of the arg document.getJsRefObject(name) => name.startsWith("document.jsObjRef") -> get that otherwise, the name
            var charIdx = 0;
            var constAssignments = "";

            // first, handle $X, like $0, $1, etc
            var maxDollarIdx = -1;
            for (int i = 20; i >= 0; --i) 
            {
                var escapedName = $"${i}";
                if (str.IndexOf(escapedName) < 0)
                    continue;
                if (maxDollarIdx < 0)
                    maxDollarIdx = i;
                var replacedName = $"obj{caseIdx}_{i}";
                constAssignments += $" const {replacedName} = document.argToJsObj(jsArgs[{i}]);";

                str = str.Replace($"{escapedName}", replacedName);
            }

            var subConstIdx = maxDollarIdx + 1;

            while (true)
            {
                var nextIdx = str.IndexOf('{', charIdx);
                if (nextIdx < 0)
                    break;
                if (str[nextIdx + 1] == '{')
                {
                    // {{ -> escaped {
                    charIdx = nextIdx + 2;
                    continue;
                }
                if (str[nextIdx + 1] == ' ') 
                {
                    charIdx = nextIdx + 1;
                    // "{ " - not an escaped name,
                    // Example: "void;window.mapInitialize = function () { $0(); }"
                    continue;
                }

                var endIdx = str.IndexOf('}', nextIdx);
                var escapedName = str.Substring(nextIdx + 1, endIdx - nextIdx - 1);
                var replacedName = $"obj{caseIdx}_{subConstIdx}";
                constAssignments += $" const {replacedName} = document.argToJsObj(jsArgs[{subConstIdx}]);";
                ++subConstIdx;

                str = str.Replace($"{{{escapedName}}}", replacedName);
            }

            str = str.Replace("{{", "{").Replace("}}", "}").Trim();
            if (!str.EndsWith(";"))
                str += ";";

            /* what does the function return ?
             *
             * by default, unless ANY of the below are met, it will return the result of the whole expression
             *
             * 1. if string starts with "void;" -> it returns void (undefined)
             * 2. if string starts with return ... -> it returns the expression following return (until first ;)
             * 3. if expression ends with return ..., I will take the expression from the last return, to the end.
             *
             * TOTHINK
             * 4a. if expression starts with "ref;" or "refId;" -> the result will kept in a referenceId + the argument will name will hold the referenceid
             * 4b. if in the code i have ref[...] -> turn into document.jsObRefId[...]
             */

            // append ; if not found
            var returnStr = "";
            bool returnsVoid = false;
            if (str.StartsWith("void;"))
            {
                str = str.Substring(5);
                returnStr = "return undefined;";
                returnsVoid = true;
            }
            else if (str.StartsWith("return "))
            {
                var idx = str.IndexOf(';');
                ++idx;
                returnStr = str.Substring(0, idx);
                str = str.Substring(idx);
            }
            else
            {
                var idx = str.LastIndexOf("return ");
                if (idx >= 0)
                {
                    returnStr = str.Substring(idx);
                    str = str.Substring(0, idx);
                }
                else
                {
                    returnStr = $"return {str}";
                    str = "";
                }
            }

            str = str.Trim();
            if (str.Length > 0 && !str.EndsWith(";"))
                str += ";";

            var js = $" case {caseIdx}: {constAssignments} {str} {returnStr} break; ";
            return (js, subConstIdx, returnsVoid);
        }

        private static void CreateJavascriptFunctionImpl(string funcName, string javascript)
        {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptFuncSync("createFunction", funcName, javascript);
        }

        private static object CallJavascriptFunctionImpl(string funcName, int subId, string args)
        {
            return INTERNAL_ExecuteJavaScript.ExecuteJavaScriptFuncSync("callFunction", funcName, subId, args);
        }


        private static (string JS,IReadOnlyDictionary<int, (int ArgCount, bool ReturnsVoid)> ExtraInfo) CreateFunction(IReadOnlyDictionary<int, string> subFunctions)
        {
            var js = "\"use strict\";\r\n" +
                     "const jsArgs = args.split('\\b');\r\n" +
                     "switch(subId) { \r\n";
            Dictionary<int, (int ArgCount, bool ReturnsVoid)> extraInfo = new Dictionary<int, (int, bool)>();
            foreach (var func in subFunctions)
            {
                var result = CreateJavascriptCase(func.Value, func.Key);
                js += $"{result.Js}\r\n";
                extraInfo.Add(func.Key, (result.ArgCount, result.ReturnsVoid));
            }

            js += "}";
            return (js, extraInfo);
        }

        public static (string JS,IReadOnlyDictionary<int, (int ArgCount, bool ReturnsVoid)> ExtraInfo) CreateFunction(string functionName, IReadOnlyDictionary<int, string> subFunctions)
        {
            var result = CreateFunction(subFunctions);
            try
            {
                CreateJavascriptFunctionImpl(functionName, result.JS);
            }
            catch (Exception e)
            {
                Console.WriteLine($"FATAL: could not create function {functionName}:\r\n{e}");
            }
            return result;
        }

        private static object ConvertObjToJs(object a)
        {
            if (a == null)
                return "null";

            if (a is IJavaScriptConvertible jsc)
                return jsc.ToJavaScriptString();
            if (a is string)
                return a;
            if (a is bool b)
                return (b ? "true" : "false");
            if (a is IFormattable formattable)
                return formattable.ToInvariantString();
            if (a is Delegate d)
            {
                // always create it synchronously
                var cb = JavaScriptCallback.Create(d, sync: true);
                return INTERNAL_InteropImplementation.GetVariableStringForJS(cb);
            }
            // pass it unchanged, it will invoke .ToString()
            return a;
        }

        public static object CallJavascriptFunction(string funcName, int subId, params object[] args)
        {
            var all = string.Join("\b", args.Select(ConvertObjToJs));
            try
            {
                return CallJavascriptFunctionImpl(funcName, subId, all);
            }
            catch (Exception e)
            {
                // the idea -- write the args passed to the function, so we can investigate
                Console.WriteLine($"FATAL error on CallJS: {funcName}, args={all}\r\n{e}");
                return null;
            }
        }

        public static string ConvertArgsToString(object[] args) => string.Join("\b", args.Select(ConvertObjToJs));

    }
}
