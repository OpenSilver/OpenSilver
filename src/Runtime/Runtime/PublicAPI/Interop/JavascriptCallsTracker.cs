using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSilver;
using OpenSilver.Internal;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    internal static class ReadOnlyListExtensions {
        public static int FindLastIndex<T>(this IReadOnlyList<T> self, Predicate<T> match) => FindLastIndex(self, self.Count - 1, self.Count, match);

        public static int FindLastIndex<T>(this IReadOnlyList<T> self, int startIndex, int count, Predicate<T> match) {
            if (self.Count == 0)
                return -1;

            if (startIndex >= self.Count)
                return -1;

            if (count < 0 || startIndex - count + 1 < 0)
                return -1;

            int endIndex = startIndex - count;
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(self[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal class JavascriptCallsTracker
    {
        private JavascriptCallsTracker(){ }
        public static JavascriptCallsTracker Instance { get; } = new JavascriptCallsTracker();

        private bool _isTracking = false;
        
        private int _bigCount = 50;

        // how many calls to dump
        private int _dumTopCount = 15;

        private static int _maxJavascriptSize = 128;

        private class CallInfo
        {
            public FunctionDetails FuncInfo;
            public string Javascript;
            public DateTime LastCall { get; private set; } = DateTime.Now;
            public int CallCount { get; private set; } = 1;

            public void AddCall()
            {
                ++CallCount;
                LastCall = DateTime.Now;
            }
        }

        private class CallArrayInfo
        {
            public Dictionary<string, CallInfo> All = new Dictionary<string, CallInfo>();
            public int PrevCallCount = 0;
            public int PrevCallStringSizeSum = 0;

            public int CallCount = 0;
            public int CallStringSizeSum = 0;

            public void Clear()
            {
                All.Clear();
                CallCount = 0;
                CallStringSizeSum = 0;
                PrevCallCount = 0;
                PrevCallStringSizeSum = 0;
            }

            public void AddCall(FunctionDetails funcInfo, string js)
            {
                ++CallCount;
                CallStringSizeSum += js.Length;
                var funcName = funcInfo.FunctionName;
                if (All.TryGetValue(funcName, out var call))
                    call.AddCall();
                else
                {
                    var jsInfo = js.Length < _maxJavascriptSize ? js : js.Substring(0, _maxJavascriptSize) + "...";
                    All.Add(funcName, new CallInfo { FuncInfo = funcInfo, Javascript = jsInfo });
                }
            }
        }

        private CallArrayInfo _all = new CallArrayInfo();
        private CallArrayInfo _shortTimeLast = new CallArrayInfo();
        private CallArrayInfo _bigTimeLast = new CallArrayInfo();

        public async void StackTracking(int ms)
        {
            _isTracking = true;

            var secs = (double)ms / 1000d;
            int bigTimeIndex = 0;
            int interop2CallCount = Interop2Caller.CallCount;
            while (true)
            {
                await Task.Delay(ms);
                DumpCallInfos(_shortTimeLast, $"NOW-{secs:F2}");
                _shortTimeLast.Clear();
                DumpCallInfos(_bigTimeLast, $"LAST-{(secs * _bigCount):F2}");
                var needsResetPrevCallCount = (bigTimeIndex++ % _bigCount) == 0;
                if (needsResetPrevCallCount)
                    _bigTimeLast.Clear();
                DumpCallInfos(_all, "ALL");
                var interop2Diff = Interop2Caller.CallCount - interop2CallCount;
                if (interop2Diff > 0)
                    Console.WriteLine($"*** INTEROP2: +{interop2Diff} ({Interop2Caller.CallCount})");
                interop2CallCount = Interop2Caller.CallCount;
            }
        }

        private void DumpCallInfos(CallArrayInfo ci, string callsType)
        {
            if (ci.PrevCallCount == ci.CallCount)
                return; // no new calls

            Console.WriteLine($"**** JS CALLS {callsType} - calls {ci.CallCount} ; size= {ci.CallStringSizeSum} ");
            var top = ci.All.Values.OrderByDescending(c => c.CallCount).Take(_dumTopCount).ToList();
            Console.WriteLine($"     {string.Join("\r\n     ", top.Select(c => $"calls={c.CallCount} {c.FuncInfo.FunctionName} : '{c.Javascript}'"))}");

            ci.PrevCallCount = ci.CallCount;
            ci.PrevCallStringSizeSum = ci.CallStringSizeSum;
        }

        public void AddJavascriptCall(string js)
        {
            if (!_isTracking)
                return;

            var stackTrace = StackTraceProvider.StackTrace().ToList();
            // look for .Interop.ExecuteJavaScript...
            // why the last? just in case there are calls to the obsolete API (Cshtml5...)
            var prevCallIdx = stackTrace.FindLastIndex(st => st.FunctionName.Contains(".Interop.ExecuteJavaScript"));
            if (prevCallIdx < 0 || prevCallIdx + 1 >= stackTrace.Count)
            {
                Console.WriteLine($"FATAL: invalid Javascript call {string.Join("\r\n", stackTrace.Select(st => st.FunctionName))}");
                return;
            }

            var callInfo = stackTrace[prevCallIdx + 1];
            _all.AddCall(callInfo, js);
            _shortTimeLast.AddCall(callInfo, js);
            _bigTimeLast.AddCall(callInfo, js);
        }
    }
}
