#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
	public static class DoubleExtensions
	{
        private const double Epsilon = 1e-10;
        public static bool IsClose(this double @this, double value)
        {
            // |a-b|/(|a|+|b|+1) < Epsilon
            return (@this == value) || @this.IsNaN() && value.IsNaN() || Math.Abs(@this - value) < Epsilon * (Math.Abs(@this) + Math.Abs(value) + 1);
        }
        public static bool IsNaN(this double @this)
        {
            return Double.IsNaN(@this);
        }
        public static double DefaultIfNaN(this double @this, double defaultValue)
        {
            return Double.IsNaN(@this) ? defaultValue : @this;
        }
        public static double Min(this double @this, double value)
        {
            return @this < value ? @this : value;
        }
        public static double Max(this double @this, double value)
        {
            return @this > value ? @this : value;
        }
        public static double Bounds(this double @this, double minimum, double maximum)
        {
            if (minimum > maximum)
            {
                throw new Exception($"Invalid bounds (minimum: {minimum}, maximum: {maximum})");
            }

            return @this.Max(minimum).Min(maximum);
        }
    }
}
#endif