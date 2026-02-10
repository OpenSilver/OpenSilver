
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

using OpenSilver.Internal;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class Polyfills
    {
        extension(ArgumentNullException)
        {
            [DebuggerStepThrough]
            public static void ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
            {
                if (argument is null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
        }

        extension(ArgumentException)
        {
            [DebuggerStepThrough]
            public static void ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    ArgumentNullException.ThrowIfNull(argument, paramName);
                    throw new ArgumentException(Strings.Argument_EmptyString, paramName);
                }
            }
        }
    }
}
