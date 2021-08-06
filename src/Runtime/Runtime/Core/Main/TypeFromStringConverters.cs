

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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

#if !MIGRATION
using Windows.UI.Text;
#endif

namespace DotNetForHtml5.Core // Important: DO NOT RENAME. This namespace is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
{
    /// <summary>
    /// A class used to convert elements written as strings in the XAML code into the correct type for the properties setted
    /// </summary>
    /// <exclude/>
    public static class TypeFromStringConverters // Important: DO NOT REMOVE OR RENAME. This class is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
    {
        /// <summary>
        /// Checks if it is possible to convert from string to a given type.
        /// </summary>
        /// <param name="type">Type to lookup.</param>
        /// <returns>True if a converter exists, false otherwise.</returns>
        public static bool CanTypeBeConverted(Type type) // Important: DO NOT REMOVE OR RENAME. This method is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter == null)
            {
                return false;
            }

            return converter.CanConvertFrom(typeof(string));
        }

        /// <summary>
        /// Converts the given string into the specified Type using the registered converter for the Type.
        /// </summary>
        /// <param name="type">The Type to which the conversion method is registered.</param>
        /// <param name="expression">The string to convert.</param>
        /// <returns>The instance of the specified type converted from the string.</returns>
        public static object ConvertFromInvariantString(Type type, string expression)  // Important: DO NOT REMOVE OR RENAME. This method is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
        {
            var converter = TypeDescriptor.GetConverter(type);

            if (converter != null &&
                converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromInvariantString(expression);
            }

            if (type.IsAssignableFrom(typeof(string)))
            {
                return expression;
            }

#if WORKINPROGRESS
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
#else
            throw new Exception("Unable to find a converter from \"String\" to \"" + type.ToString() + "\"");
#endif
        }

        /// <summary>
        /// Registers a type and its associated method to convert it from a string. This method takes part in allowing the definition of an object of the said type directly in the xaml.
        /// </summary>
        /// <param name="type">The type for which a converter will be defined</param>
        /// <param name="converter">The method that will convert the XAML string into an instance of the type.</param>
        [Obsolete("This does not do anything. Use TypeConverter instead")]
        public static void RegisterConverter(Type type, Func<string, object> converter)
        {
        }
    }
}
