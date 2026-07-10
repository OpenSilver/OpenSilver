
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

using OpenSilver.Internal;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class Polyfills
    {
        extension(ArgumentNullException)
        {
            [DebuggerStepThrough]
            public static void ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
            {
                if (argument is null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
        }

        extension(ArgumentException)
        {
            [DebuggerStepThrough]
            public static void ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    ArgumentNullException.ThrowIfNull(argument, paramName);
                    throw new ArgumentException(Strings.Argument_EmptyString, paramName);
                }
            }
        }

        extension(ArgumentOutOfRangeException)
        {
            [DebuggerStepThrough]
            public static void ThrowIfNegative(double value, [CallerArgumentExpression(nameof(value))] string paramName = null)
            {
                if (value < 0)
                {
                    ThrowNegative(value, paramName);
                }
            }

            [DebuggerStepThrough]
            public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string paramName = null)
            {
                if (value < 0)
                {
                    ThrowNegative(value, paramName);
                }
            }

            [DebuggerStepThrough]
            public static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string paramName = null)
                where T : IComparable<T>
            {
                if (value.CompareTo(other) > 0)
                {
                    ThrowGreaterEqual(value, other, paramName);
                }
            }

            [DebuggerStepThrough]
            public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string paramName = null)
                where T : IComparable<T>
            {
                if (value.CompareTo(other) >= 0)
                {
                    ThrowGreaterEqual(value, other, paramName);
                }
            }

            private static void ThrowNegative<T>(T value, string paramName) =>
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(Strings.ArgumentOutOfRange_Generic_MustBeNonNegative, paramName, value));

            private static void ThrowGreater<T>(T value, T other, string paramName) =>
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(Strings.ArgumentOutOfRange_Generic_MustBeLessOrEqual, paramName, value, other));

            private static void ThrowGreaterEqual<T>(T value, T other, string paramName) =>
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(Strings.ArgumentOutOfRange_Generic_MustBeLess, paramName, value, other));
        }
    }
}
