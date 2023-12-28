// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.InteropServices;

namespace System.Windows.Controls
{
    /// <summary>
    /// Numeric utility methods used by controls.  These methods are similar in
    /// scope to the WPF DoubleUtil class.
    /// </summary>
    internal static class NumericExtensions
    {
        /// <summary>
        /// Check if a number is zero.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>True if the number is zero, false otherwise.</returns>
        public static bool IsZero(this double value)
        {
            // We actually consider anything within an order of magnitude of
            // epsilon to be zero
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        /// <summary>
        /// Determine if one number is greater than another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is greater than the second, false
        /// otherwise.
        /// </returns>
        public static bool IsGreaterThan(double left, double right)
        {
            return (left > right) && !AreClose(left, right);
        }

        /// <summary>
        /// Determine if one number is less than or close to another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is less than or close to the second, false
        /// otherwise.
        /// </returns>
        public static bool IsLessThanOrClose(double left, double right)
        {
            return (left < right) || AreClose(left, right);
        }

        /// <summary>
        /// Determine if two numbers are close in value.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is close in value to the second, false
        /// otherwise.
        /// </returns>
        public static bool AreClose(double left, double right)
        {
            if (left == right)
            {
                return true;
            }

            double a = (Math.Abs(left) + Math.Abs(right) + 10.0) * 2.2204460492503131E-16;
            double b = left - right;
            return (-a < b) && (a > b);
        }
    }
}