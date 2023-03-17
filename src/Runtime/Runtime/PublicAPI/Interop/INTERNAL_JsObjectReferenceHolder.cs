using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSHTML5.Types;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    // purpose -- keep references to all c# code that uses JsObjectReference
    internal class INTERNAL_JsObjectReferenceHolder
    {
        private INTERNAL_JsObjectReferenceHolder(){}
        public static INTERNAL_JsObjectReferenceHolder Instance { get; } = new INTERNAL_JsObjectReferenceHolder();

        private class Info {
            private const int MAX_DUMP_JS_LEN = 100;

            public string Javascript;
            public INTERNAL_JSObjectReference JsObject;
            public IReadOnlyList<FunctionDetails> StackTrace;
            public DateTime CreatedAt = DateTime.Now;

            private string TrimJs() {
                string js ;
                if (Javascript.Length <= MAX_DUMP_JS_LEN)
                    js = Javascript;
                else
                    js = Javascript.Substring(0, MAX_DUMP_JS_LEN) ;

                js = js.Replace("\r", "\\r").Replace("\n", "\\r");
                if (js.Length > MAX_DUMP_JS_LEN)
                    js = js.Substring(0, MAX_DUMP_JS_LEN);

                if (Javascript.Length > MAX_DUMP_JS_LEN)
                    js = js.Substring(0, js.Length - 3) + "...";

                return js;
            }

            private string StackTraceStr() {
                
                return "                " + string.Join("\r\n                ", StackTrace.Take(global::OpenSilver.Interop.DumpAllJavascriptObjectsStackTraceCount)
                    .Select(st => st.FriendlyStr()));
            }

            public string Summary() => $"       {CreatedAt:HH:mm:ss} [{JsObject.ReferenceId}] '{TrimJs()}' From {StackTrace[0].FunctionName}";
            public string Details() => $"{Summary()}\r\n{StackTraceStr()}";
        }

        private Dictionary<string, Info> _objects = new Dictionary<string, Info>();

        private HashSet<string> _added = new HashSet<string>();
        private HashSet<string> _removed = new HashSet<string>();

        public async void StartTracking(int ms) {
            while (true) {
                await Task.Delay(ms);
                DumpDiffs();
            }
        }

        private static string Key(INTERNAL_JSObjectReference obj) {
            var arrayIndex = obj.IsArray ? obj.ArrayIndex : -1;
            return $"{obj.ReferenceId}-{arrayIndex}";
        }

        private static string[] _ignoreFunctionNames = new[] {
            ".INTERNAL_JsObjectReferenceHolder.Add", 
            ".INTERNAL_JSObjectReference..ctor",
            ".ExecuteJavaScript_Implementation",
            ".ExecuteJavaScript_GetJSObject",
            ".ExecuteJavaScript",
            ".ExecuteJavaScriptVoid",
        };
        private IReadOnlyList<FunctionDetails> NormalizeStackTrace(IReadOnlyList<FunctionDetails> stackTrace) {
            return stackTrace.Where(fd => _ignoreFunctionNames.All(l => !fd.FunctionName.EndsWith(l))).ToList();
        }

        public void Add(INTERNAL_JSObjectReference obj, string javascript) {
            var key = Key(obj);
            var stackTrace = StackTraceProvider.StackTrace();
            lock (this) {
                _objects.Add(key, new Info {
                    JsObject = obj,
                    StackTrace = NormalizeStackTrace(stackTrace), 
                    Javascript = javascript,
                });
                _added.Add(key);
            }
        }
        public void Remove(INTERNAL_JSObjectReference obj) {
            var key = Key(obj);
            lock (this) {
                _objects.Remove(key);
                if(!_added.Remove(key))
                    _removed.Add(key);
            }
        }

        private void DumpDiffs() {
            // important: only last 20 calls from the stack trace
            List<Info> added;
            List<string> removed;
            int count = 0;
            lock (this) {
                if (_added.Count < 1 && _removed.Count < 1)
                    return;
                count = _objects.Count;
                removed = _removed.ToList();
                added = _added.Select(id => _objects[id]).ToList();

                _removed.Clear();
                _added.Clear();
            }
            var trueRefCount = global::OpenSilver.Interop.ExecuteJavaScript("Object.keys(document.jsObjRef).length");
            var trueRefCountAsInt = Convert.ToInt32(trueRefCount);
            trueRefCount.Dispose();

            StringBuilder diffs = new StringBuilder("[");
            foreach (var r in removed)
                diffs.Append($"-{r}, ");
            foreach (var a in added)
                diffs.Append($"+{a.JsObject.ReferenceId}, ");
            diffs.Append("]");
            var countStr = count != trueRefCountAsInt ? $"{count} (true={trueRefCount})" : $"{count}";

            var filterCount = added.Count(i => global::OpenSilver.Interop.DumpAllJavascriptObjectsFilter(i.StackTrace.First().FunctionName, i.Javascript));
            if (filterCount != added.Count)
                countStr += $" (filtered={filterCount}/{added.Count})";
            else
                countStr += $" (filtered={filterCount})";
            global::OpenSilver.Interop.DumpAllJavascriptObjectsLogger($"****** Javascript references: {countStr} {diffs}");
            foreach (var info in added.Where(i => global::OpenSilver.Interop.DumpAllJavascriptObjectsFilter(i.StackTrace.First().FunctionName, i.Javascript))) {
                var str = (global::OpenSilver.Interop.DumpAllJavascriptObjectsVerbose ? info.Details() : info.Summary());
                global::OpenSilver.Interop.DumpAllJavascriptObjectsLogger(str);
            }
            // the idea - the above could be really huge, so dump this again, make it easier to read without scrolling back
            if(global::OpenSilver.Interop.DumpAllJavascriptObjectsVerbose && filterCount > 0)
                global::OpenSilver.Interop.DumpAllJavascriptObjectsLogger($"****** Javascript references: {countStr} {diffs}");
        }
    }
}
