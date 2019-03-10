
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
