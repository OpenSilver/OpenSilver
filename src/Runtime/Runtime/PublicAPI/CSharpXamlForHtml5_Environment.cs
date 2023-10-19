

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


using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class CSharpXamlForHtml5
{
    /// <summary>
    /// A class that allows to know the current Environnement (between Running in C# or in Javascript)
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// Gets a boolean saying if we are currently running in Javascript.
        /// </summary>
        public static bool IsRunningInJavaScript
        {
            get
            {
#if CSHTML5BLAZOR
                return false;
#else
                return !INTERNAL_InteropImplementation.IsRunningInTheSimulator();
#endif
            }
        }
    }

}
