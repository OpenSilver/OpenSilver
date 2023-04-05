using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;

namespace OpenSilver
{
    public class BulkInterop2
    {
        private class SingleCall
        {
            public string Javascript;
            public int Index;
            public string FuncName;
        }

        private List<SingleCall> _cachedCalls ;

        // if we get to this size, we automatically execute
        // if < 0, we don't preallocate anything
        private int _maxCapacity;

        private int _index;
        private bool _postponedExecute;

        public static bool LogPerformance { get; set; } = false;

        private bool PreallocateCapacity => _maxCapacity > 0;

        public BulkInterop2( int maxCapacity = -1)
        {
            _maxCapacity = maxCapacity;
            _cachedCalls = PreallocateCapacity ? new List<SingleCall>(_maxCapacity) : new List<SingleCall>();
            if (PreallocateCapacity)
                for (int i = 0; i < _maxCapacity; ++i)
                    _cachedCalls.Add(new SingleCall());
        }

        public IDisposable AddJavascriptAsync<T>(T subId, params object[] args) where T : Enum
        {
            //FIXME not implemented
            Debug.Assert(false);
            //javascript = INTERNAL_InteropImplementation.ReplaceJSArgs(javascript, variables);
            //return AddJavascriptAsync(javascript);
            return null;
        }

        private static object CallJavascriptFunctionImpl(string funcName, int subId, string args)
        {
            return INTERNAL_ExecuteJavaScript.ExecuteJavaScriptFuncSync("callFunction", funcName, subId, args);
        }

        public void AddJavascript<T>(T subId, params object[] args) where T : Enum
        {
            var funcName = Interop2.CreateFunctionAndReturnName<T>();
            if (PreallocateCapacity)
            {
                _cachedCalls[_index].Javascript = Interop2Caller.ConvertArgsToString(args);
                _cachedCalls[_index].Index = (int)(object)subId;
                _cachedCalls[_index].FuncName = funcName;
            } 
            else
                _cachedCalls.Add(new SingleCall
                {
                    Javascript = Interop2Caller.ConvertArgsToString(args),
                    Index = (int)(object)subId,
                    FuncName = funcName,
                });

            ++_index;
            if (PreallocateCapacity && _index >= _maxCapacity)
                Execute();
        }

        public void Execute()
        {
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < _index; ++i)
            {
                var call = _cachedCalls[i];
                try
                {
                    CallJavascriptFunctionImpl(call.FuncName, call.Index, call.Javascript);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FATAL BulkInterop2 execute: {call.FuncName}:{call.Index}  {call.Javascript}\r\n{e}");
                }
                if (PreallocateCapacity)
                    call.Javascript = call.FuncName = null;
            }
            double ms = watch.ElapsedMilliseconds;
            if (LogPerformance)
                Console.WriteLine($"BULK2 INTEROP : calls: {_index} , took {ms} ({(ms / _index):F4}/call)");

            _index = 0;
            if (!PreallocateCapacity)
                _cachedCalls.Clear();
        }

        public void PostponeExecute(Dispatcher dispatcher)
        {
            if (_postponedExecute)
                return; // already postponed
            _postponedExecute = true;
            dispatcher.BeginInvoke(Execute);
        }
    }
}
