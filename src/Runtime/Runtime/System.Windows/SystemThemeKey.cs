
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

using System.Diagnostics;
using System.Reflection;

namespace System.Windows;

/// <summary>
///     Implements ResourceKey to create unique keys for our system resources.
///     Keys will be exposed publicly only with the ResourceKey API.
/// </summary>
internal sealed class SystemThemeKey : ResourceKey
{
    /// <summary>
    ///     Constructs a new instance of the key with the given ID.
    /// </summary>
    /// <param name="id">The internal, unique ID of the system resource.</param>
    internal SystemThemeKey(SystemResourceKeyID id)
    {
        ResourceKey = id;
        Debug.Assert(id > SystemResourceKeyID.InternalSystemThemeStylesStart && id < SystemResourceKeyID.InternalSystemThemeStylesEnd);
    }

    internal SystemResourceKeyID ResourceKey { get; }

    public override Assembly Assembly { get; } = typeof(FrameworkElement).Assembly;

    public override bool Equals(object o) => o is SystemThemeKey key && key.ResourceKey == ResourceKey;

    public override int GetHashCode() => (int)ResourceKey;

    public override string ToString() => ResourceKey.ToString();
}
