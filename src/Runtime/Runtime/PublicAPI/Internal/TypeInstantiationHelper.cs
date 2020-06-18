

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
    public static class TypeInstantiationHelper
    {
        /// <summary>
        /// Invokes the parameterless constructor of the specified type.
        /// </summary>
        /// <param name="type">The Type to instantiate.</param>
        /// <returns>A new instance of the Type, initialized through the parameterless constructor.</returns>
        public static object Instantiate(Type type)
        {
            var constructor = type.GetConstructor(new Type[] { });
            if (constructor != null)
            {
                return constructor.Invoke(null);
            }
            else
            {
                throw new Exception("No parameterless constructor defined for the type: \"" + type.FullName + "\".");
            }
        }
    }
}
