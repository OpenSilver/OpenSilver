

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    internal class INTERNAL_NoValue
    {
        internal static readonly INTERNAL_NoValue NoValue; // Singleton

        static INTERNAL_NoValue()
        {
            NoValue = new INTERNAL_NoValue();
        }

        private INTERNAL_NoValue() // Private constructor si that class cannot be constructed from elsewhere.
        {

        }

    }
}
