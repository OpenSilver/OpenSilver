
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

using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace OpenSilver.Internal;

internal static class StringBuilderFactory
{
    private static readonly ObjectPool<StringBuilder> _pool;

    static StringBuilderFactory()
    {
        var provider = new DefaultObjectPoolProvider { };
        _pool = provider.CreateStringBuilderPool( initialCapacity: 16 * 1024, maximumRetainedCapacity: 32);
    }

    public static StringBuilder Get() {
        var sb = _pool.Get();
        sb.Clear();
        return sb;
    }

    // helper - wrap a string into a stringbuilder
    public static StringBuilder Get(string s) {
        var sb = _pool.Get();
        sb.Clear();
        sb.Append(s);
        return sb;
    }

    public static void Return(StringBuilder sb) => _pool.Return(sb);
}
