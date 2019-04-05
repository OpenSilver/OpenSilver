
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
