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
	public partial struct Point : IFormattable
	{
		public static readonly Point Empty = new Point();
		public static readonly Point Zero = new Point(0, 0);

		/// <summary>
		/// Creates a System.String representation of this Windows.Foundation.Point.
		/// </summary>
		/// <param name="provider">Culture-specific formatting information.</param>
		/// <returns>
		/// A System.String containing the Windows.Foundation.Point.X and Windows.Foundation.Point.Y
		/// values of this Windows.Foundation.Point structure.
		/// </returns>
		[OpenSilver.NotImplemented]
		public string ToString(IFormatProvider provider)
		{
			return default(string);
		}
        public static Point operator +(Point point1, Point point2)
        {
            if (point1 == Point.Zero)
            {
                return point2;
            }

            if (point2 == Point.Zero)
            {
                return point1;
            }

            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point operator -(Point point1, Point point2)
        {
            if (point1 == Point.Zero)
            {
                return -point2;
            }

            if (point2 == Point.Zero)
            {
                return point1;
            }

            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }
        public static Point operator -(Point point)
        {
            if (point == Point.Zero)
            {
                return point;
            }

            return new Point(-point.X, -point.Y);
        }
    }
}
#endif