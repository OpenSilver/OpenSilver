
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



#if BRIDGE
using CSHTML5;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class INTERNAL_BridgeWorkarounds
{
    public static string GetAssemblyNameWithoutCallingGetNameMethod(Assembly assembly)
    {
        string fullName = assembly.FullName;
        int indexOfComma = fullName.IndexOf(',');
        if (indexOfComma > -1)
            return fullName.Substring(0, indexOfComma);
        else
            return fullName;
    }

    public static TimeSpan TimeSpanParse(string timeSpanAsString, bool canBeNegative = true)
    {
        //-----------------------------------------------------
        // Note: initially Bridge.NET did not support TimeSpan.Parse at all, so we created the workaround below.
        // Then Bridge.NET added implementation but only for the default representation: https://github.com/bridgedotnet/Bridge/pull/3544
        // So there are still issues with the built-in Bridge.NET implementation. For example, the following code fails: TimeSpan ts = TimeSpan.Parse("0");
        // To reproduce: in the app of the client linked to CSHTML5 ZenDesk ticket #828: Go to "Children" -> "Child Profile" window. An exception appears because TimeSpan.Parse("0") fails.
        // Therefore we still use the code below for now.
        //-----------------------------------------------------

        TimeSpan timeSpan;
        int days = 0;
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        int fractionalSeconds = 0;
        //string's format:
        //[ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws]
        //we get rid of the white spaces at the beginning and at the end:
        string timeAsString = timeSpanAsString.Trim();
        int signKeeper = 1;
        if (timeAsString[0] == '-')
        {
            signKeeper = -1;
            timeAsString = timeAsString.Substring(1);
        }
        string[] splittedTime = timeAsString.Split(':');
        // We can't parse timeSpanAsString to a TimeSpan in this case
        if (splittedTime.Length > 3)
        {
            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\".");
        }
        else if (splittedTime.Length == 1)
        {
            days = int.Parse(splittedTime[0]);
            if (days < 0)
            {
                throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
            }
        }
        else
        {
            for (int i = 0; i < splittedTime.Length; i++)
            {
                if (i == 0)
                {
                    // we check if the number of days is specified
                    string[] daysAndHours = splittedTime[0].Split('.');
                    if (daysAndHours.Length == 1) // number of days is not specified, so it is 0.
                    {
                        hours = int.Parse(daysAndHours[0]);
                    }
                    else
                    {
                        days = int.Parse(daysAndHours[0]);
                        if (days < 0)
                        {
                            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                        }
                        hours = int.Parse(daysAndHours[1]);
                    }
                    if (hours < 0 || hours > 23)
                    {
                        throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                    }
                }
                else if (i == 1) // In this case we try to get the minutes, so we just avec to parse splittedTime[i] to an int.
                {
                    minutes = int.Parse(splittedTime[i]);
                    if (minutes < 0 || minutes > 59)
                    {
                        throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                    }
                }
                else if (i == 2) // Here we want to get the seconds and milliseconds if specified.
                {
                    string[] secondsAndFractionalSeconds = splittedTime[i].Split('.');
                    if (secondsAndFractionalSeconds.Length == 1)
                    {
                        seconds = int.Parse(secondsAndFractionalSeconds[0]);
                    }
                    else
                    {
                        seconds = int.Parse((secondsAndFractionalSeconds[0] == string.Empty ? "0" : secondsAndFractionalSeconds[0])); // we need to check this because we can have to parse a string with the following format : "00:00:.5".
                        fractionalSeconds = int.Parse(secondsAndFractionalSeconds[1]);
                        if (fractionalSeconds < 0 || fractionalSeconds > 9999999)
                        {
                            throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                        }
                    }
                    if (seconds < 0 || seconds > 59)
                    {
                        throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\". At least one of the numeric components is out of range or contains too many digits.");
                    }
                }
            }
        }
        //we're done parsing, we can create the TimeSpan:
        long ticks = days * 24; //24 hours a day
        ticks += hours;
        ticks *= 60; //60 minutes an hour
        ticks += minutes;
        ticks *= 60; //60 seconds a minute
        ticks += seconds;
        ticks *= 10000000; // 10 000 000 ticks per second
        ticks += fractionalSeconds;

        if(signKeeper == -1)
        {
            if (canBeNegative)
            {
                timeSpan = new TimeSpan(ticks * signKeeper);
            }
            else
            {
                timeSpan = new TimeSpan();
            }
        }
        else
        {
            timeSpan = new TimeSpan(ticks);
        }
        return timeSpan;
    }

    public static IEnumerator<KeyValuePair<TKey, TValue>> GetDictionaryEnumerator<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        //Note: we do not simply return "dict.GetEnumerator()" because of Bridge issue #3004: https://github.com/bridgedotnet/Bridge/issues/3004
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)dict).GetEnumerator();
    }

    public static IEnumerable<TKey> GetDictionaryKeys_SimulatorCompatible<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        // When running in the Simulator, "Bridge.dll" is replaced by "Mscorlib.dll". This causes an issue with the "Dictionary<,>.Keys" method because, at the time of writing (Dec 2018), the Bridge version returns ICollection<> whereas the Mscorlib version returns a KeyCollection.
        //todo: remove this whole method as soon as the Bridge version of Dictionary<,>.Keys has the same signature as the Mscorlib version. Progress can be tracked at: https://github.com/bridgedotnet/Bridge/issues/3004
        if (IsRunningInTheSimulator())
            return dict.Select(keyValuePair => keyValuePair.Key);
        else
            return dict.Keys;
    }

    public static IEnumerable<TValue> GetDictionaryValues_SimulatorCompatible<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        // When running in the Simulator, "Bridge.dll" is replaced by "Mscorlib.dll". This causes an issue with the "Dictionary<,>.Values" method because, at the time of writing (Dec 2018), the Bridge version returns ICollection<> whereas the Mscorlib version returns a ValueCollection.
        //todo: remove this whole method as soon as the Bridge version of Dictionary<,>.Values has the same signature as the Mscorlib version. Progress can be tracked at: https://github.com/bridgedotnet/Bridge/issues/3004
        if (IsRunningInTheSimulator())
            return dict.Select(keyValuePair => keyValuePair.Value);
        else
            return dict.Values;
    }

    public static MethodInfo[] TypeGetMethods_SimulatorCompatible(Type type, BindingFlags bindingFlags)
    {
        if (Interop.IsRunningInTheSimulator)
            return ((dynamic)type).GetMethods(bindingFlags) as MethodInfo[];
        else
            return TypeGetMethods_BrowserOnly(type, bindingFlags);
    }

    private static MethodInfo[] TypeGetMethods_BrowserOnly(Type type, BindingFlags bindingFlags)
    {
        return type.GetMethods(bindingFlags);
    }

    public static bool MethodInfoIsStatic_SimulatorCompatible(MethodInfo methodInfo)
    {
        if (Interop.IsRunningInTheSimulator)
            //Note: we cast to "dynamic" in order to support the Simulator, because in Bridge, as of Feb 2019, the "IsStatic" method belongs to the "MemberInfo" class, whereas in .NET it belongs to the "MethodBase" class.
            return (bool)(((dynamic)methodInfo).IsStatic);
        else
            return MethodInfoIsStatic_BrowserOnly(methodInfo);
    }

    private static bool MethodInfoIsStatic_BrowserOnly(MethodInfo methodInfo)
    {
        return methodInfo.IsStatic;
    }

    public static bool ConstructorInfoIsStatic_SimulatorCompatible(ConstructorInfo constructorInfo)
    {
        if (Interop.IsRunningInTheSimulator)
            //Note: we cast to "dynamic" in order to support the Simulator, because in Bridge, as of Feb 2019, the "IsStatic" method belongs to the "MemberInfo" class, whereas in .NET it belongs to the "MethodBase" class.
            return (bool)(((dynamic)constructorInfo).IsStatic);
        else
            return ConstructorInfoIsStatic_BrowserOnly(constructorInfo);
    }

    private static bool ConstructorInfoIsStatic_BrowserOnly(ConstructorInfo constructorInfo)
    {
        return constructorInfo.IsStatic;
    }

    public static bool ConstructorInfoIsPublic_SimulatorCompatible(ConstructorInfo constructorInfo)
    {
        if (Interop.IsRunningInTheSimulator)
            //Note: we cast to "dynamic" in order to support the Simulator, because in Bridge, as of Feb 2019, the "IsPublic" method belongs to the "MemberInfo" class, whereas in .NET it belongs to the "MethodBase" class.
            return (bool)(((dynamic)constructorInfo).IsPublic);
        else
            return ConstructorInfoIsPublic_BrowserOnly(constructorInfo);
    }

    private static bool ConstructorInfoIsPublic_BrowserOnly(ConstructorInfo constructorInfo)
    {
        return constructorInfo.IsPublic;
    }

    public static bool FieldInfoIsStatic_SimulatorCompatible(FieldInfo fieldInfo)
    {
        if (Interop.IsRunningInTheSimulator)
            //Note: we cast to "dynamic" in order to support the Simulator, because in Bridge, as of Feb 2019, the "IsStatic" method belongs to the "MemberInfo" class, whereas in .NET it belongs to the "FieldInfo" class.
            return (bool)(((dynamic)fieldInfo).IsStatic);
        else
            return FieldInfoIsStatic_BrowserOnly(fieldInfo);
    }

    private static bool FieldInfoIsStatic_BrowserOnly(FieldInfo fieldInfo)
    {
        return fieldInfo.IsStatic;
    }

    public static bool FieldInfoIsPublic_SimulatorCompatible(FieldInfo fieldInfo)
    {
        if (Interop.IsRunningInTheSimulator)
            //Note: we cast to "dynamic" in order to support the Simulator, because in Bridge, as of Feb 2019, the "IsPublic" method belongs to the "MemberInfo" class, whereas in .NET it belongs to the "FieldInfo" class.
            return (bool)(((dynamic)fieldInfo).IsPublic);
        else
            return FieldInfoIsPublic_BrowserOnly(fieldInfo);
    }

    private static bool FieldInfoIsPublic_BrowserOnly(FieldInfo fieldInfo)
    {
        return fieldInfo.IsPublic;
    }

    public static int HexToInt_SimulatorOnly(string hexString)
    {
        // The following line is simpler, but it cannot be used because, at the time of writing (Dec 2018), Bridge does not support "System.Globalization.NumberStyles":
        // return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        // The following alternative cannot be used either, because it works in Bridge but not in the Simulator due to the fact that the Mscorlib version of int.Parse lacks the overload that takes the radix as argument:
        // return colorAsInt = int.Parse(colorcode, radix: 16);

        if (INTERNAL_Simulator.SimulatorProxy != null)
        {
            return (int)INTERNAL_Simulator.SimulatorProxy.HexToInt(hexString);
        }
        else
        {
            return 0; //We only arrive here during compilation because the Simulatorproxy is never set.
        }
    }

    [Bridge.Template("false")]
    static bool IsRunningInTheSimulator()
    {
        return true;
    }

    public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
        {
            throw new ArgumentNullException(nameof(propertyInfo));
        }

        MethodInfo getMethod = propertyInfo.GetMethod;
        if (getMethod != null && getMethod.IsPublic)
        {
            return getMethod;
        }
        
        return null;
    }

    public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
        {
            throw new ArgumentNullException(nameof(propertyInfo));
        }

        MethodInfo setMethod = propertyInfo.SetMethod;
        if (setMethod != null && setMethod.IsPublic)
        {
            return setMethod;
        }

        return null;
    }

    public static Type EventHandlerType(this EventInfo eventInfo)
    {
        if (eventInfo == null)
        {
            throw new ArgumentNullException(nameof(eventInfo));
        }

        return eventInfo.AddMethod.GetParameters()[0].ParameterType;
    }
}
#endif
