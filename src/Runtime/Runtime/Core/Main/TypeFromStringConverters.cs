

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHTML5;
using CSHTML5.Internal;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace DotNetForHtml5.Core // Important: DO NOT RENAME. This namespace is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
{
    /// <summary>
    /// A class used to convert elements written as strings in the XAML code into the correct type for the properties setted
    /// </summary>
    /// <exclude/>
    public static class TypeFromStringConverters // Important: DO NOT REMOVE OR RENAME. This class is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
    {
        static bool IsBaseTypesConvertersRegistered; // Note: we use this variable instead of a static constructor that makes the call because of a bug in JSIL (2014.04.30).
        static Dictionary<Type, Func<string, object>> Converters = new Dictionary<Type, Func<string, object>>();
        const string AdviseToFixTheError = "To fix the issue, please locate the error by looking for the first XAML file in the Stack Trace.";

        static void RegisterBaseTypesConverters()
        {
            // Register converters for base system types:
            RegisterConverter(typeof(bool), ConvertBoolFromString);
            RegisterConverter(typeof(Nullable<bool>), ConvertNullableBoolFromString);
            RegisterConverter(typeof(int), ConvertIntFromString);
            RegisterConverter(typeof(double), ConvertDoubleFromString);
            RegisterConverter(typeof(Uri), ConvertUriFromString);
            RegisterConverter(typeof(object), ConvertObjectFromString);
            RegisterConverter(typeof(byte), ConvertByteFromString);
#if !MIGRATION
            RegisterConverter(typeof(TextDecorations), ConvertTextDecorationsFromString);
            RegisterConverter(typeof(Nullable<TextDecorations>), ConvertNullableTextDecorationsFromString);
#endif
            RegisterConverter(typeof(TimeSpan), ConvertTimeSpanFromString);
            RegisterConverter(typeof(Nullable<TimeSpan>), ConvertTimeSpanFromString);
        }

        /// <summary>
        /// Checks if it is possible to convert from string to a given type.
        /// </summary>
        /// <param name="type">Type to lookup.</param>
        /// <returns>True if a converter exists, false otherwise.</returns>
        public static bool CanTypeBeConverted(Type type) // Important: DO NOT REMOVE OR RENAME. This method is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
        {
            Func<string, object> converter;
            return TryGetConverter(type, out converter);
        }

        /// <summary>
        /// Registers a type and its associated method to convert it from a string. This method takes part in allowing the definition of an object of the said type directly in the xaml.
        /// </summary>
        /// <param name="type">The type for which a converter will be defined</param>
        /// <param name="converter">The method that will convert the XAML string into an instance of the type.</param>
        public static void RegisterConverter(Type type, Func<string, object> converter)
        {
            if (!Converters.ContainsKey(type))
                Converters.Add(type, converter);
        }

        /// <summary>
        /// Converts the given string into the specified Type using the registered converter for the Type.
        /// </summary>
        /// <param name="type">The Type to which the conversion method is registered.</param>
        /// <param name="s">The string to convert.</param>
        /// <returns>The instance of the specified type converted from the string.</returns>
        public static object ConvertFromInvariantString(Type type, string s)  // Important: DO NOT REMOVE OR RENAME. This method is called by the XAML inspector of the Simulator using reflection. If you wish to remove or rename it, make sure to make the appropriate changes in the Simulator.
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif

            //if the type is Nullable and the value is Null, we don't need to get the type, just set the value to null:
            if (type.Name == "Nullable`1" && s == "x:Null") //todo: verify this is correct and appropriate (cf. changeset 1548)
            {
#if PERFSTAT
                Performance.Counter("XAML: ConvertFromInvariantString: " + type.Name, t0);
#endif

                return null;
            }
            //if the type is an Enum, we can simply convert the string into the enum value:
            else if (type.IsEnum)
            {
                object enumValue = Enum.Parse(type, s, true);
                return enumValue;
            }
            else
            {
#if WORKINPROGRESS && !CSHTML5NETSTANDARD
                if (Interop.IsRunningInTheSimulator) // this is usefull if we need to call a static constructor of a type that is defined outside of core assembly, so that it registers the type's converter.
                {
                    INTERNAL_Simulator.SimulatorProxy.RunClassConstructor(type);
                }
#endif
                Func<string, object> converter;
                if (TryGetConverter(type, out converter))
                {
                    var convertedValue = converter(s);

#if PERFSTAT
                    Performance.Counter("XAML: ConvertFromInvariantString: " + type.Name, t0);
#endif
                    return convertedValue;
                }
                else
                {
#if WORKINPROGRESS
                    Debug.WriteLine("Unable to find a converter from \"String\" to \"" + type.ToString() + "\"");
                    if(type.IsValueType)
                    {
                        return Activator.CreateInstance(type);
                    }
                    else 
                    {
                        return null;
                    }
#else
                    throw new Exception("Unable to find a converter from \"String\" to \"" + type.ToString() + "\"");
#endif
                }
            }
        }

        static bool TryGetConverter(Type type, out Func<string, object> converter)
        {
            // Register converters for base system types if not already done:
            if (!IsBaseTypesConvertersRegistered)
            {
                RegisterBaseTypesConverters();
                IsBaseTypesConvertersRegistered = true;
            }

            Type nonNullableType = type;
            if (type.Name == "Nullable`1")
            {
                if (Converters.ContainsKey(type))
                {
                    converter = Converters[type];
                    return true;
                }
                else
                {
                    //BRIDGETODO
                    //verify the conditons matchs below with or without bridge
#if !BRIDGE
                if (type.IsGenericType && type.GenericTypeArguments.Length > 0)
#else
                    if (type.IsGenericType && type.GetGenericArguments().Length > 0)
#endif
                    {
#if !BRIDGE
                        nonNullableType = type.GenericTypeArguments[0];
#else
                        nonNullableType = type.GetGenericArguments()[0];
#endif
                    }
                }
            }

            if (Converters.ContainsKey(nonNullableType))
            {
                converter = Converters[nonNullableType];
                return true;
            }
            else
            {
                converter = null;
                return false;
            }
        }

#region Converters for base types

        static object ConvertBoolFromString(string str)
        {
            string lowerStr = str.ToLower();
            if (lowerStr == "true")
            {
                return true;
            }
            else if (lowerStr == "false")
            {
                return false;
            }
            else
            {
                throw new Exception("Xaml exception: cannot convert \"" + str + "\" to bool. " + AdviseToFixTheError);
            }
        }

        static object ConvertNullableBoolFromString(string str)
        {
            string lowerStr = str.ToLower();
            if (lowerStr == "true")
            {
                return true;
            }
            else if (lowerStr == "false")
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        static object ConvertIntFromString(string str)
        {
            int returnValue;
            if (!int.TryParse(str, out returnValue))
                throw new Exception("Xaml exception: cannot convert \"" + str + "\" to int. " + AdviseToFixTheError);
            return returnValue;
        }

        static object ConvertDoubleFromString(string str)
        {
            double returnValue;
            if (str != null && (str.ToLower() == "auto" || str.ToLower() == "nan"))
            {
                returnValue = double.NaN;
            }
            else if (str.ToLower() == "infinity")
            {
                returnValue = double.PositiveInfinity;
            }
            else if (str.ToLower() == "-infinity")
            {
                returnValue = double.NegativeInfinity;
            }
            else if (!double.TryParse(str, out returnValue))
            {
                throw new Exception("Xaml exception: cannot convert \"" + str + "\" to double. " + AdviseToFixTheError);
            }
            return returnValue;
        }

        static object ConvertByteFromString(string str)
        {
            byte returnValue;
            if (!byte.TryParse(str, out returnValue))
                throw new Exception("Xaml exception: cannot convert \"" + str + "\" to byte. " + AdviseToFixTheError);
            return returnValue;
        }

        internal static object ConvertUriFromString(string str)
        {
            UriKind uriKind;
            if (str.Contains(":"))
            {
                // cf. https://stackoverflow.com/questions/1737575/are-colons-allowed-in-urls
                string textBeforeColon = str.Substring(0, str.IndexOf(":"));
                if (!textBeforeColon.Contains(@"\") && !textBeforeColon.Contains(@"/"))
                {
                    uriKind = UriKind.Absolute;
                }
                else
                {
                    uriKind = UriKind.Relative;
                }
            }
            else
            {
                uriKind = UriKind.Relative;
            }

            Uri returnValue = new Uri(str, uriKind);

            return returnValue;
        }

        static object ConvertObjectFromString(string str)
        {
            return (object)str;
        }

#if !MIGRATION
        static object ConvertTextDecorationsFromString(string textDecorations)
        {
            TextDecorations? returnValue;
            if (string.IsNullOrEmpty(textDecorations) || !TryParseTextDecorations(textDecorations, out returnValue))
                throw new Exception("Xaml exception: cannot convert \"" + (textDecorations ?? "") + "\" to TextDecorations enum. " + AdviseToFixTheError);
            return (TextDecorations)returnValue;
        }

        static object ConvertNullableTextDecorationsFromString(string textDecorations)
        {
            TextDecorations? returnValue;
            if (string.IsNullOrEmpty(textDecorations))
                return null;
            else if (!TryParseTextDecorations(textDecorations, out returnValue))
                throw new Exception("Xaml exception: cannot convert \"" + textDecorations + "\" to TextDecorations enum. " + AdviseToFixTheError);
            else
                return returnValue;
        }

        static bool TryParseTextDecorations(string textDecorationsString, out TextDecorations? textDecorations)
        {
            foreach (TextDecorations value in Enum.GetValues(typeof(TextDecorations)))
            {
                if (textDecorationsString.ToLower() == value.ToString().ToLower())
                {
                    textDecorations = value;
                    return true;
                }
            }
            textDecorations = null;
            return false;
        }
#endif

        static object ConvertTimeSpanFromString(string str)
        {
            try
            {
#if BRIDGE
                TimeSpan returnValue = INTERNAL_BridgeWorkarounds.TimeSpanParse(str);
#else
                TimeSpan returnValue = TimeSpan.Parse(str);
#endif
                return returnValue;
            }
            catch
            {
                throw new Exception("Xaml exception: cannot convert \"" + str + "\" to TimeSpan. " + AdviseToFixTheError);
            }
        }

#endregion
    }
}
