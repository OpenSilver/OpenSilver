
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
public sealed class OpenSilverResourceExposureAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSilverResourceExposureAttribute"/>
    /// class.
    /// </summary>
    public OpenSilverResourceExposureAttribute(bool exposeResourcesToExtractor)
    {
        ExposeResourcesToExtractor = exposeResourcesToExtractor;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public OpenSilverResourceExposureAttribute(string exposeResourcesToExtractor)
    {
        ExposeResourcesToExtractor = bool.Parse(exposeResourcesToExtractor);
    }

    public bool ExposeResourcesToExtractor { get; }
}
