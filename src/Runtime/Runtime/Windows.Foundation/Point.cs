

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Represents an x- and y-coordinate pair in two-dimensional
    /// space. Can also represent a logical point for certain property usages.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(PointConverter))]
#endif
    [SupportsDirectContentViaTypeFromStringConverters]
#if WORKINPROGRESS
    public partial struct Point : IFormattable
#else
    public partial struct Point //: IFormattable
#endif
    {
        //todo: Add the interface IFormattable

        internal double _x;
        internal double _y;

		public static readonly Point Empty = new Point();
		public static readonly Point Zero = new Point(0, 0);

        /// <summary>
        /// Initializes a Windows.Foundation.Point structure that
        /// contains the specified values.
        /// </summary>
        /// <param name="x">The x-coordinate value of the Windows.Foundation.Point structure.</param>
        /// <param name="y">The y-coordinate value of the Windows.Foundation.Point structure.</param>
        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Compares two Windows.Foundation.Point structures for inequality
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// true if point1 and point2 have different Windows.Foundation.Point.X or Windows.Foundation.Point.Y
        /// values; false if point1 and point2 have the same Windows.Foundation.Point.X
        /// and Windows.Foundation.Point.Y values.
        /// </returns>
        public static bool operator !=(Point point1, Point point2)
        {
            return (point1.X != point2.X || point1.Y != point2.Y);
        }
       

        /// <summary>
        /// Compares two Windows.Foundation.Point structures for equality
        /// </summary>
        /// <param name="point1">The first point to compare.</param>
        /// <param name="point2">The second point to compare.</param>
        /// <returns>
        /// true if both the Windows.Foundation.Point.X and Windows.Foundation.Point.Y
        /// values of point1 and point2 are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(Point point1, Point point2)
        {
            return (point1.X == point2.X && point1.Y == point2.Y);

        }

        /// <summary>
        /// Gets or sets the Windows.Foundation.Point.X-coordinate
        /// value of this Windows.Foundation.Point structure.
        /// </summary>
        public double X {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Windows.Foundation.Point.Y-coordinate
        /// value of this Windows.Foundation.Point.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

       
        /// <summary>
        /// Determines whether the specified object is a Windows.Foundation.Point
        /// and whether it contains the same values as this Windows.Foundation.Point.
        /// </summary>
        /// <param name="o">The object to compare.</param>
        /// <returns>
        /// true if obj is a Windows.Foundation.Point and contains the same Windows.Foundation.Point.X
        /// and Windows.Foundation.Point.Y values as this Windows.Foundation.Point; otherwise,
        /// false.
        /// </returns>
        public override bool Equals(object o)
        {
            return (o is Point && ((Point)o) == this);
        }

        /// <summary>
        /// Compares two Windows.Foundation.Point structures for
        /// equality.
        /// </summary>
        /// <param name="value">The point to compare to this instance.</param>
        /// <returns>
        /// true if both Windows.Foundation.Point structures contain the same Windows.Foundation.Point.X
        /// and Windows.Foundation.Point.Y values; otherwise, false.
        /// </returns>
        public bool Equals(Point value)
        {
            return value == this;
        }

        /// <summary>
        /// Returns the hash code for this Windows.Foundation.Point.
        /// </summary>
        /// <returns>The hash code for this Windows.Foundation.Point.</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode());
        }

     
        /// <summary>
        /// Creates a System.String representation of this Windows.Foundation.Point.
        /// </summary>
        /// <returns>
        /// A System.String containing the Windows.Foundation.Point.X and Windows.Foundation.Point.Y
        /// values of this Windows.Foundation.Point structure.
        /// </returns>
        public override string ToString()
        {
            return X + "," + Y;
        }
        
        //// <summary>
        //// Creates a System.String representation of this Windows.Foundation.Point.
        //// </summary>
        //// <param name="provider">Culture-specific formatting information.</param>
        //// <returns>
        //// A System.String containing the Windows.Foundation.Point.X and Windows.Foundation.Point.Y
        //// values of this Windows.Foundation.Point structure.
        //// </returns>
        //public string ToString(IFormatProvider provider)
        //{
        //    throw new NotImplementedException();
        //    //var provider.GetFormat(typeof(Point));
        //    //todo: I don't know how a FormatProvider should be used 
        //    return X + "," + Y;
        //}

        static Point()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Point), s => Parse(s));
        }

        public static Point Parse(string pointAsString)
        {
            string[] splittedString = pointAsString.Split(new[]{',', ' '}, StringSplitOptions.RemoveEmptyEntries);

            if (splittedString.Length == 2)
            {
                double x, y;
#if OPENSILVER
                if (double.TryParse(splittedString[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) && 
                    double.TryParse(splittedString[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
#else
                if (double.TryParse(splittedString[0], out x) &&
                    double.TryParse(splittedString[1], out y))
#endif
                    return new Point(x, y);
            }
            
            throw new FormatException(pointAsString + " is not an eligible value for a Point");
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return null;
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