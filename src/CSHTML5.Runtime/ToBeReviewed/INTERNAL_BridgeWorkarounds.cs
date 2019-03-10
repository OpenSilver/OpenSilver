
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


#if BRIDGE
using CSHTML5;
using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class INTERNAL_BridgeWorkarounds
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

    public static TimeSpan TimeSpanParse(string timeSpanAsString)
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

        //timeAsString's format:
        //[-]{ d | [d.]hh:mm[:ss[.ff]] }
        //We remember the sign of the TimeSpan:
        int signKeeper = 1;
        if (timeAsString.StartsWith("-"))
        {
            signKeeper = -1;
        }
        //note: we implement this as if the timeSpan could be negative but in the specific case of KeyTime, it cannot, thus the next test on signKeeper.
        if (signKeeper != -1)
        {
            if (signKeeper == -1 || timeAsString.StartsWith("+")) //not sure whether there can be a '+' sign or not.
            {
                //we remove the sign at the start of the string:
                timeAsString = timeAsString.Substring(1);
            }

            //timeAsString's format:
            //{ d | [d.]hh:mm[:ss[.ff]] } (this means: d OR [d.]hh:mm[:ss[.ff]]
            if (!timeAsString.Contains(':'))
            {
                //if we arrive here, it means we are in the format that only contains the days:
                days = int.Parse(timeAsString);
            }
            else
            {
                //timeAsString's format:
                //[d.]hh:mm[:ss[.ff]]
                string[] splittedTime = timeAsString.Split(':');

                if (splittedTime.Length == 1 || splittedTime.Length > 3) //splittedTime.Length == 1  means that we have something like hh: and >3 does not make sense
                {
                    throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\".");
                }

                //we get the days and hours:
                string hoursAsString = splittedTime[0];
                //hoursAsString's format:
                //[d.]hh
                int i = hoursAsString.IndexOf('.');
                if (i > 0)
                {
                    //hoursAsString's format:
                    //d.hh
                    days = int.Parse(hoursAsString.Substring(0, i));
                    hoursAsString = hoursAsString.Substring(i + 1);
                }
                else if (i == 0)
                {
                    throw new FormatException("Could not create KeyTime from \"" + timeSpanAsString + "\".");
                }

                //hoursAsString's format:
                //hh
                hours = int.Parse(hoursAsString);

                //we get the minutes:
                minutes = int.Parse(splittedTime[1]); //the minutes should always be there if there are hours.

                //we get the seconds and fractional seconds
                if (splittedTime.Length == 3)
                {
                    string secondsAsString = splittedTime[2];
                    //secondsAsString's format:
                    //ss[.ff]
                    //note: ff can be from 1 to 7 digits (it's the ticks and there are 10 000 000 ticks per second)

                    i = secondsAsString.IndexOf('.');
                    if (i > 0)
                    {
                        string fractionalSecondsAsString = secondsAsString.Substring(i + 1);
                        fractionalSeconds = int.Parse(fractionalSecondsAsString) * (int)Math.Pow(10, 6 - fractionalSecondsAsString.Length); //we make sure the fractionalSeconds are in 7 digits (if we had 10.21, we don't want to consider it as if it was 10.0000021 and no way to know once it's an int)
                        secondsAsString = secondsAsString.Substring(0, i);
                    }
                    seconds = int.Parse(secondsAsString);
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
            ticks *= signKeeper; //if it was a negative TimeSpan, we put it.
            timeSpan = new TimeSpan(ticks);
        }
        else
        {
            timeSpan = new TimeSpan();
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

        return (int)INTERNAL_Simulator.SimulatorProxy.HexToInt(hexString);
    }

    [Bridge.Template("false")]
    static bool IsRunningInTheSimulator()
    {
        return true;
    }
}
#endif
