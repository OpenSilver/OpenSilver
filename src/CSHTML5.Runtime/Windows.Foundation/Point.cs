
//-----------------------------------------------------------------------------
//  CONFIDENTIALITY NOTICE:
//  This code is the sole property of Userware and is strictly confidential.
//  Unless you have a written agreement in effect with Userware that states
//  otherwise, you are not authorized to view, copy, modify, or compile this
//  source code, and you should destroy all the copies that you possess.
//  Any redistribution in source code form is strictly prohibited.
//  Redistribution in binary form is allowed provided that you have obtained
//  prior written consent from Userware. You can contact Userware at:
//  contact@userware-solutions.com - Copyright (c) 2016 Userware
//-----------------------------------------------------------------------------


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.ComponentModel;
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
    public struct Point //: IFormattable
    {
        //todo: Add the interface IFormattable

        double _x;
        double _y;

        /// <summary>
        /// Initializes a Windows.Foundation.Point structure that
        /// contains the specified values.
        /// </summary>
        /// <param name="x">The x-coordinate value of the Windows.Foundation.Point structure.</param>
        /// <param name="y">The y-coordinate value of the Windows.Foundation.Point structure.</param>
        public Point(double x, double y)
        {
            _x = 0d;
            _y = 0d;

            X = x;
            Y = y;
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
            TypeFromStringConverters.RegisterConverter(typeof(Point), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string pointAsString)
        {
            char separator;
            if (pointAsString.Contains(","))
            {
                separator = ',';
            }
            else
            {
                separator = ' ';
            }
            string[] splittedString = pointAsString.Trim().Split(separator);
            if (splittedString.Length == 2)
            {
                double x = 0d;
                double y = 0d;

                bool isParseOK = double.TryParse(splittedString[0], out x);
                isParseOK = isParseOK && double.TryParse(splittedString[1], out y);

                if (isParseOK)
                    return new Point(x, y);
            }
            
            throw new FormatException(pointAsString + " is not an eligible value for a Point");
        }
    }
}