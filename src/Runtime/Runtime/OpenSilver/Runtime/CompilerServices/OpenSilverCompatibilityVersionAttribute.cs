
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
using System.ComponentModel;

namespace OpenSilver.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class OpenSilverCompatibilityVersionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSilverAssemblyAttribute"/> class.
    /// </summary>
    public OpenSilverCompatibilityVersionAttribute(uint version)
    {
        Version = version;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public OpenSilverCompatibilityVersionAttribute(string version)
    {
        Version = uint.Parse(version);
    }

    public uint Version { get; }
}
