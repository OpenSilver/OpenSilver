

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
#if !BRIDGE
    [JSIgnore]
#else
    [External]
#endif
    internal class INTERNAL_SimulatorJSExpression
    {
        public INTERNAL_SimulatorJSExpression(string expression)
        {
            this.Expression = expression;
        }

        public string Expression { get; set; }
    }
}
