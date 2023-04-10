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
            // only for debugging/testing
            public string Enum;
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

        private Dispatcher _dispatcher;

        // for testing -- this will execute each JS instantly, so that it's easier to find javascript errors
        public bool ExecuteInstantly { get; set; } = false;

        public BulkInterop2(Dispatcher dispatcher, int maxCapacity = -1)
        {
            _dispatcher = dispatcher;
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
            // note: args is already pre-processed
            return Interop2Caller.CallJavascriptFunctionWithAllArgs(funcName, subId, args);
        }

        public void AddJavascript<T>(T subId, params object[] args) where T : Enum
        {
            var funcName = Interop2.CreateFunctionAndReturnName<T>();
            if (ExecuteInstantly)
            {
                var index = (int)(object)subId;
                var js = Interop2Caller.ConvertArgsToString(args);
                try
                {
                    CallJavascriptFunctionImpl(funcName, index, js);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FATAL BulkInterop2 execute: {funcName}:{subId}  {js}\r\n{e}");
                }
                return;
            }

            if (PreallocateCapacity)
            {
                _cachedCalls[_index].Javascript = Interop2Caller.ConvertArgsToString(args);
                _cachedCalls[_index].Index = (int)(object)subId;
                _cachedCalls[_index].Enum= subId.ToString();
                _cachedCalls[_index].FuncName = funcName;
            } 
            else
                _cachedCalls.Add(new SingleCall
                {
                    Javascript = Interop2Caller.ConvertArgsToString(args),
                    Index = (int)(object)subId,
                    Enum= subId.ToString(),
                    FuncName = funcName,
                });

            ++_index;
            if (PreallocateCapacity && _index >= _maxCapacity)
                Execute();
        }

        public void Execute()
        {
            if (_index < 1)
                return;

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
                    Console.WriteLine($"FATAL BulkInterop2 execute: {call.FuncName}:{call.Enum}  {call.Javascript}\r\n{e}");
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

        public void PostponeExecute()
        {
            if (_postponedExecute)
                return; // already postponed
            _postponedExecute = true;
            _dispatcher.BeginInvoke(Execute);
        }
    }
}
