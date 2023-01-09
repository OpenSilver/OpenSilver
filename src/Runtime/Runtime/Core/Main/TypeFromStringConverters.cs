
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
using OpenSilver.Internal;

//
// IMPORTANT: Be very careful when editing this class.
//
// This class is called by the XAML inspector of the Simulator using reflection.
// If you wish to remove, rename or modify it, make sure to make the appropriate
// changes in the Simulator.
//

namespace DotNetForHtml5.Core 
{
    /// <summary>
    /// A class used to convert elements written as strings in the XAML 
    /// code into the correct type for the properties setted
    /// </summary>
    /// <exclude/>
    public static class TypeFromStringConverters
    {
        private static readonly Type _objectType = typeof(object);

        /// <summary>
        /// Checks if it is possible to convert from string to a given type.
        /// </summary>
        /// <param name="type">
        /// Type to lookup.
        /// </param>
        /// <returns>
        /// True if a converter exists, false otherwise.
        /// </returns>
        public static bool CanTypeBeConverted(Type type)
        {
            if (type == _objectType)
            {
                return true;
            }

            TypeConverter converter = TypeConverterHelper.GetConverter(type);

            if (converter != null)
            {
                return converter.CanConvertFrom(typeof(string));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete(Helper.ObsoleteMemberMessage + " Use System.ComponentModel.TypeConverterAttribute instead.")]
        public static void RegisterConverter(Type type, Func<string, object> converter)
        {
        }

        /// <summary>
        /// Converts the given <see cref="string"/> into the specified <see cref="Type"/> 
        /// using the registered <see cref="TypeConverter"/> for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">
        /// The Type to which the conversion method is registered.
        /// </param>
        /// <param name="s">
        /// The string to convert.
        /// </param>
        public static object ConvertFromInvariantString(Type type, string s) 
        {
            if (type == _objectType)
            {
                return s;
            }

            TypeConverter converter = TypeConverterHelper.GetConverter(type);

            if (converter != null)
            {
                return converter.ConvertFromInvariantString(s);
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
