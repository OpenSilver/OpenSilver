
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
using System.Collections.Concurrent;
using System.Threading;

namespace OpenSilver.Compiler;

internal sealed class NameProvider
{
    private readonly ConcurrentDictionary<string, UInt64Counter> _nextCounterPerPrefix = [];

    private sealed class UInt64Counter
    {
        private long _value;

        public ulong Next() => unchecked((ulong)Interlocked.Increment(ref _value));
    }

    public string GetName(string str)
    {
        ReadOnlySpan<char> prefix = str.AsSpan();

        if (prefix.Length > 30)
        {
            prefix = prefix.Slice(0, 30);
        }

        // Because of f# warning, makes the first letter lower case
        string typeName = $"{char.ToLower(prefix[0])}{prefix.Slice(1)}";

        ulong id = _nextCounterPerPrefix.GetOrAdd(typeName, static (_) => new UInt64Counter()).Next();

        return $"{typeName}{id}"; // Example: button1, button2, ...
    }
}
