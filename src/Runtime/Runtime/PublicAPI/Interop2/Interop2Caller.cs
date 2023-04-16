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
        // for debugging/testing
        private static int _callCount = 0;
        public static int CallCount => _callCount;

        public (string VariableName, string VariableDefinition) JsReferenceObject(object o, int idx, int subIdx)
        {
            var js = INTERNAL_InteropImplementation.GetVariableStringForJS(o);
            var name = $"obj{idx}_{subIdx}";
            var vd = $"const {name} = {js};";
            return (name, vd);
        }

        private static string AsJavascriptNumber(string s) => $"parseInt({s})";
        private static string AsJavascriptDouble(string s) => $"parseFloat({s})";
        private static string AsJavascriptBool(string s) => $"({s} === 'true')";

        public enum InteropReturnType
        {
            Object, Void, ReferenceId,
        }

        // to specify number or string: append |n or |d
        // for $X -> use $X|n or $X|d -> ONLY ON FIRST USE
        // for {name} -> {name|n} or {name|d} or {name,n} or {name,d}  -> ONLY ON FIRST USE
        //
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
        private static (string Js, int ArgCount, InteropReturnType ReturnType) CreateJavascriptCase(string str, int caseIdx)
        {
            var originalStr = str;
            try
            {
                // the args - each of the arg document.getJsRefObject(name) => name.startsWith("document.jsObjRef") -> get that otherwise, the name
                var charIdx = 0;
                var constAssignments = "";

                // first, handle $X, like $0, $1, etc
                var maxDollarIdx = -1;
                for (int i = 20; i >= 0; --i)
                {
                    var escapedName = $"${i}";
                    var escapeNameIdx = str.IndexOf(escapedName);
                    if (str.IndexOf(escapedName) < 0)
                        continue;

                    var isNumber = escapeNameIdx == str.IndexOf(escapedName + "|n");
                    var isDouble = escapeNameIdx == str.IndexOf(escapedName + "|d");
                    var isBool = escapeNameIdx == str.IndexOf(escapedName + "|b");
                    if (isNumber)
                        str = str.Replace(escapedName + "|n", escapedName);
                    if (isDouble)
                        str = str.Replace(escapedName + "|d", escapedName);
                    if (isBool)
                        str = str.Replace(escapedName + "|b", escapedName);

                    if (maxDollarIdx < 0)
                        maxDollarIdx = i;
                    var replacedName = $"obj{caseIdx}_{i}";
                    var assignValue = $"document.argToJsObj(jsArgs[{i}])";
                    if (isNumber)
                        assignValue = AsJavascriptNumber(assignValue);
                    else if (isDouble)
                        assignValue = AsJavascriptDouble(assignValue);
                    else if (isBool)
                        assignValue = AsJavascriptBool(assignValue);
                    constAssignments += $" const {replacedName} = {assignValue};";

                    // console.log('$0') -> console.log(replacedName) - no need for quotes
                    str = str.Replace($"'{escapedName}'", replacedName);
                    str = str.Replace($"\"{escapedName}\"", replacedName);

                    //obj.{property} -> obj[{property}] -> the {property} is passed as an argument
                    str = str.Replace($".{escapedName}", $"[{replacedName}]");

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

                    if (Char.IsWhiteSpace(str[nextIdx + 1]))
                    {
                        charIdx = nextIdx + 1;
                        // "{ " - not an escaped name,
                        // Example: "void;window.mapInitialize = function () { $0(); }"
                        continue;
                    }

                    var endIdx = str.IndexOf('}', nextIdx);
                    var escapedName = str.Substring(nextIdx + 1, endIdx - nextIdx - 1);
                    var replacedName = $"obj{caseIdx}_{subConstIdx}";

                    var isNumber = false;
                    var isDouble = false;
                    var isBool = false;
                    if (escapedName.EndsWith("|n") || escapedName.EndsWith(",n")) 
                    {
                        escapedName = escapedName.Substring(0, escapedName.Length - 2);
                        str = str.Replace($"{{{escapedName}|n}}", $"{{{escapedName}}}");
                        str = str.Replace($"{{{escapedName},n}}", $"{{{escapedName}}}");
                        isNumber = true;
                    } else if (escapedName.EndsWith("|d") || escapedName.EndsWith(",d")) {
                        escapedName = escapedName.Substring(0, escapedName.Length - 2);
                        str = str.Replace($"{{{escapedName}|d}}", $"{{{escapedName}}}");
                        str = str.Replace($"{{{escapedName},d}}", $"{{{escapedName}}}");
                        isDouble = true;
                    } else if (escapedName.EndsWith("|b") || escapedName.EndsWith(",b")) {
                        escapedName = escapedName.Substring(0, escapedName.Length - 2);
                        str = str.Replace($"{{{escapedName}|b}}", $"{{{escapedName}}}");
                        str = str.Replace($"{{{escapedName},b}}", $"{{{escapedName}}}");
                        isBool = true;
                    }

                    var assignValue = $"document.argToJsObj(jsArgs[{subConstIdx}])";
                    if (isNumber)
                        assignValue = AsJavascriptNumber(assignValue);
                    else if (isDouble)
                        assignValue = AsJavascriptDouble(assignValue);
                    else if (isBool)
                        assignValue = AsJavascriptBool(assignValue);

                    constAssignments += $" const {replacedName} = {assignValue};";
                    ++subConstIdx;

                    // console.log('{eventName}') -> console.log(replacedName) - no need for quotes
                    str = str.Replace($"'{{{escapedName}}}'", replacedName);
                    str = str.Replace($"\"{{{escapedName}}}\"", replacedName);

                    //obj.{property} -> obj[{property}] -> the {property} is passed as an argument
                    str = str.Replace($".{{{escapedName}}}", $"[{replacedName}]");

                    str = str.Replace($"{{{escapedName}}}", replacedName);
                }

                str = str.Replace("{{", "{").Replace("}}", "}").Trim();
                if (!str.EndsWith(";"))
                    str += ";";

                // finally, creation of temporary variables : $temp0 to $temp9
                for (int i = 0; i < 10; ++i)
                {
                    var temp = $"$temp{i}";
                    if (str.IndexOf(temp) >= 0)
                    {
                        // look for a single line that is $tempX = ...;
                        var tempAssign = $"$temp{i} =";
                        var idxTemp = str.IndexOf(tempAssign);
                        var idxEnd = str.IndexOf('\r', idxTemp);
                        var assignment = str.Substring(idxTemp, idxEnd - idxTemp);
                        constAssignments += $" let temp{i}_{caseIdx} {assignment.Substring(6)}";
                        str = str.Substring(0, idxTemp) + str.Substring(idxEnd);
                        str = str.Replace(temp, $"temp{i}_{caseIdx}"); // remove the $ prefix
                    }
                }

                /* what does the function return ?
                 *
                 * by default, unless ANY of the below are met, it will return the result of the whole expression
                 *
                 * 1. if string starts with "void;" -> it returns void (undefined)
                 * 2. if string starts with return ... -> it returns the expression following return (until first ;)
                 * 3. if expression ends with return ..., I will take the expression from the last return, to the end.
                 *
                 * 4. if expression starts with "ref;" -> it returns a jsObjRef (a reference ID).
                 *    In this case -> anything that the function would normally return WILL BE TURNED INTO A REFERENCE
                 *    + If this also contains void;, I will just use the referenceId
                 *
                 * 5. If anywhere I find $result, I will assume $result will hold the result to return
                 */

                str = str.Replace("void;ref;", "ref;void;");

                // append ; if not found
                var returnStr = "";
                bool returnsVoid = false;
                bool returnsRefId = false;
                bool specificResult = false;
                if (str.StartsWith("ref;")) 
                {
                    str = str.Substring(4);
                    returnsRefId = true;
                } 

                if (str.StartsWith("void;"))
                {
                    str = str.Substring(5);
                    returnStr = "return undefined;";
                    returnsVoid = true;
                } 
                else if (str.Contains("$result")) {
                    specificResult = true;
                    returnStr = "return _result;";
                    str = str.Replace("$result", "_result");
                    constAssignments += $" let _result = undefined;";
                }
                else if (str.StartsWith("return ")) {
                    var idx = str.IndexOf(';');
                    ++idx;
                    returnStr = str.Substring(0, idx);
                    str = str.Substring(idx);
                } 
                else
                {
                    var idx = str.LastIndexOf("return ");
                    // if it's a ref-id function -> just execute it and return result
                    var isRefIdFunction = returnsRefId && (str.Trim().StartsWith("(function") || str.Trim().StartsWith("( function"));
                    if (idx >= 0 && !isRefIdFunction)
                    {
                        returnStr = str.Substring(idx);
                        str = str.Substring(0, idx);
                    } else
                    {
                        returnStr = $"return {str}";
                        str = "";
                    }
                }

                str = str.Trim();
                if (str.Length > 0 && !str.EndsWith(";"))
                    str += ";";

                if (returnsRefId) 
                {
                    if (returnsVoid) 
                    {
                        returnStr = str;
                        str = "";
                    } 
                    else 
                        // replace "return ..." with document.jsObjRef[...] = ...
                        returnStr = returnStr.Substring("return ".Length);
                    returnStr = $"document.jsObjRef[ jsArgs[jsArgs.length - 1] ] = {returnStr}";
                    if (!returnStr.EndsWith(";"))
                        returnStr += ";";

                    if (specificResult)
                        returnStr = "return _result;";
                    else if (!returnsVoid)
                        returnStr += $"return document.jsObjRef[ jsArgs[jsArgs.length - 1] ];";
                    else
                        returnStr += "return undefined;";
                }

                var js = $" case {caseIdx}: {{ {constAssignments} {str} {returnStr} }} break; ";

                return (js, subConstIdx, (returnsRefId ? InteropReturnType.ReferenceId : returnsVoid ? InteropReturnType.Void : InteropReturnType.Object));
            }
            catch (Exception e)
            {
                throw new Exception($"ERROR: Invalid case string {originalStr}:{caseIdx}\r\n{e.Message}");
            }
        }

        private static void CreateJavascriptFunctionImpl(string funcName, string javascript)
        {
            INTERNAL_ExecuteJavaScript.ExecuteJavaScriptFuncSync("createFunction", funcName, javascript);
        }

        private static object CallJavascriptFunctionImpl(string funcName, int subId, string args)
        {
            return INTERNAL_ExecuteJavaScript.ExecuteJavaScriptFuncSync("callFunction", funcName, subId, args);
        }


        internal static (string JS,IReadOnlyDictionary<int, (int ArgCount, InteropReturnType ReturnType)> ExtraInfo) GetJavascriptFunctionStringForEnum(IReadOnlyDictionary<int, string> subFunctions)
        {
            var js = "\"use strict\";\r\n" +
                     "const jsArgs = args.split('\\b');\r\n" +
                     "switch(subId) { \r\n";
            Dictionary<int, (int ArgCount, InteropReturnType ReturnType)> extraInfo = new Dictionary<int, (int ArgCount, InteropReturnType ReturnType)>();
            foreach (var func in subFunctions)
            {
                var result = CreateJavascriptCase(func.Value, func.Key);
                js += $"{result.Js}\r\n";
                extraInfo.Add(func.Key, (result.ArgCount, result.ReturnType));
            }
            // add this case just for testing - just call it with argument 111111 -> if it works, we're good
            // if we get a Javascript error, it means there's a syntax error in the JS code
            js += $"case 111111: break; }}";
            return (js, extraInfo);
        }

        public static (string JS,IReadOnlyDictionary<int, (int ArgCount, InteropReturnType ReturnType)> ExtraInfo) CreateFunction(string functionName, IReadOnlyDictionary<int, string> subFunctions)
        {
            var result = GetJavascriptFunctionStringForEnum(subFunctions);
            CreateJavascriptFunctionImpl(functionName, result.JS);
            return result;
        }

        internal static object ConvertObjToJs(object a)
        {
            if (a == null)
                return "null";

            if (a is IJavaScriptConvertible jsc)
                return jsc.ToJavaScriptString();
            if (a is string)
                return a;
            if (a is double db)
                return db.ToInvariantString();
            if (a is float f)
                return f.ToInvariantString();
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

        public static object CallJavascriptFunctionWithAllArgs<T>(string funcName, T subId, string all)
        {
            try
            {
                ++_callCount;
                var index = (int)(object)subId;

                if (Interop2.DumpCalls)
                {
                    var friendlyName = funcName.Contains('.') ? funcName.Substring(funcName.LastIndexOf('.') + 1) : funcName;
                    Console.WriteLine($" *** JS     {friendlyName}[{subId}]({all.Replace("\b",", ")})");
                }

                return CallJavascriptFunctionImpl(funcName, index, all);
            }
            catch (Exception e)
            {
                // the idea -- write the args passed to the function, so we can investigate
                Console.WriteLine($"FATAL error on CallJS: {funcName}:{subId}, args={all}\r\n{e}");
                return null;
            }
        }

        public static object CallJavascriptFunction(string funcName, int subId, params object[] args)
        {
            var all = string.Join("\b", args.Select(ConvertObjToJs));
            return CallJavascriptFunctionWithAllArgs(funcName, subId, all);
        }
        // the idea: here, on exception, you get the string corresponding to the enum in the error message
        public static object CallJavascriptFunction<T>(string funcName, T subId, IReadOnlyList<object> args)
        {
            var all = string.Join("\b", args.Select(ConvertObjToJs));
            return CallJavascriptFunctionWithAllArgs(funcName, subId, all);
        }

        public static string ConvertArgsToString(IReadOnlyList<object> args) => string.Join("\b", args.Select(ConvertObjToJs));

    }
}
