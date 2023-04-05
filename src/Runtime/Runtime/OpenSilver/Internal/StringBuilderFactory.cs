
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
        var provider = new DefaultObjectPoolProvider { MaximumRetained = 10 };
        _pool = provider.CreateStringBuilderPool();
    }

    public static StringBuilder Get() => _pool.Get();

    public static void Return(StringBuilder sb) => _pool.Return(sb);
}
