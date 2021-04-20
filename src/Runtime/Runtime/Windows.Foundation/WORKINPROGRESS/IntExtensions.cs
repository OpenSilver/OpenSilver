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
	public static class IntExtensions
    {
        public static int Min(this int @this, int value)
        {
            return @this < value ? @this : value;
        }
        public static int Max(this int @this, int value)
        {
            return @this > value ? @this : value;
        }
        public static int Bounds(this int @this, int minimum, int maximum)
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