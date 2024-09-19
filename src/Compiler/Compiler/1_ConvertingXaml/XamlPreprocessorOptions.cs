
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

namespace OpenSilver.Compiler;

public enum XamlPreprocessorOptions
{
    Auto,
    Optimize,
}

internal static class XamlPreprocessorOptionsHelpers
{
    public static bool TryParse(string str, out XamlPreprocessorOptions options)
    {
        if (string.Equals(str, "auto", StringComparison.OrdinalIgnoreCase))
        {
            options = XamlPreprocessorOptions.Auto;
            return true;
        }
        else if (string.Equals(str, "optimize", StringComparison.OrdinalIgnoreCase))
        {
            options = XamlPreprocessorOptions.Optimize;
            return true;
        }
        else
        {
            options = default;
            return false;
        }
    }
}
