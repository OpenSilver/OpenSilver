
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



using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Describes the width, height, and point origin of a rectangle.
    /// </summary>
    [SupportsDirectContentViaTypeFromStringConverters]
    public struct Rect// : IFormattable
    {
        //todo: Add the interface IFormattable


        double _x;
        double _y;
        double _width;
        double _height;


        //public Rect()
        //{
        //    //setting the elements of the struct to their default value 
        //    _x = 0;
        //    _y = 0;
        //    _width = 0;
        //    _height = 0;
        //}

        /// <summary>
        /// Initializes a Windows.Foundation.Rect structure that
        /// is exactly large enough to contain the two specified points.
        /// </summary>
        /// <param name="point1">The first point that the new rectangle must contain.</param>
        /// <param name="point2">The second point that the new rectangle must contain.</param>
        public Rect(Point point1, Point point2)
        {
            //setting the elements of the struct to their default value (we don't set it to the given value because it could be an anothorized value)
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;


            double x1 = (point1.X < point2.X) ? point1.X : point2.X;
            double x2 = (point1.X >= point2.X) ? point1.X : point2.X;

            X = point1.X;
            Width = point2.X - point1.X;


            double y1 = (point1.Y < point2.Y) ? point1.Y : point2.Y;
            double y2 = (point1.Y >= point2.Y) ? point1.Y : point2.Y;

            Y = point1.Y;
            Height = point2.Y - point1.Y;
        }

        /// <summary>
        /// Initializes a Windows.Foundation.Rect structure based
        /// on an origin and size.
        /// </summary>
        /// <param name="location">The origin of the new Windows.Foundation.Rect.</param>
        /// <param name="size">The size of the new Windows.Foundation.Rect.</param>
        public Rect(Point location, Size size)
        {
            //setting the elements of the struct to their default value (we don't set it to the given value because it could be an anothorized value)
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;

            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Initializes a Windows.Foundation.Rect structure that
        /// has the specified x-coordinate, y-coordinate, width, and height.
        /// </summary>
        /// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rect(double x, double y, double width, double height)
        {
            //setting the elements of the struct to their default value (we don't set it to the given value because it could be an anothorized value)
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        /// <summary>
        /// Compares two Windows.Foundation.Rect structures for inequality.
        /// </summary>
        /// <param name="rect1">The first rectangle to compare.</param>
        /// <param name="rect2">The second rectangle to compare.</param>
        /// <returns>
        /// true if the Windows.Foundation.Rect structures do not have the same x,y,width,height
        /// property values; otherwise, false.
        /// </returns>
        public static bool operator !=(Rect rect1, Rect rect2)
        {
            return (rect1.X != rect2.X || rect1.Y != rect2.Y ||
                rect1.Width != rect2.Width || rect1.Height != rect2.Height);
        }

        /// <summary>
        /// Compares two Windows.Foundation.Rect structures for equality.
        /// </summary>
        /// <param name="rect1">The first rectangle to compare.</param>
        /// <param name="rect2">The second rectangle to compare.</param>
        /// <returns>
        /// true if the Windows.Foundation.Rect structures have the same x,y,width,height
        /// property values; otherwise, false.
        /// </returns>
        public static bool operator ==(Rect rect1, Rect rect2)
        {
            return (rect1.X == rect2.X && rect1.Y == rect2.Y &&
                rect1.Width == rect2.Width && rect1.Height == rect2.Height);
        }

        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle.
        /// </summary>
        public double Bottom
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return Y + Height;
            }
        }

        /// <summary>
        /// Gets a special value that represents a rectangle with
        /// no position or area.
        /// </summary>
        public static Rect Empty
        {
            get
            {
                Rect rect = new Rect();
                rect.Width = double.NegativeInfinity;
                rect.Height = double.NegativeInfinity;
                rect.X = double.PositiveInfinity;
                rect.Y = double.PositiveInfinity;
                return rect;
            }
        }
        
        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (!IsEmpty && value < 0)
                {
                    throw new ArgumentException("Height cannot be lower than 0");
                }
                else
                {
                    _height = value;
                }
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether the rectangle is
        /// the Windows.Foundation.Rect.Empty rectangle.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Width < 0 && Height < 0; // (double.IsNegativeInfinity(Width) && double.IsNegativeInfinity(Height));
                //todo: revert the line above when JSIL will support the "IsNegativeInfinity" method.
            }
        }

       
        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle.
        /// </summary>
        public double Left
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return X;
            }
        }

        
        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        public double Right
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return X + Width;
            }
        }

        
        /// <summary>
        /// Gets the y-axis position of the top of the rectangle.
        /// </summary>
        public double Top
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return Y;
            }
        }

        
        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (!IsEmpty && value < 0)
                {
                    throw new ArgumentException("Width cannot be lower than 0");
                }
                else
                {
                    _width = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the x-axis value of the left side of the
        /// rectangle.
        /// </summary>
        public double X
        {
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
        /// Gets or sets the y-axis value of the top side of the
        /// rectangle.
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
        /// Indicates whether the rectangle described by the Windows.Foundation.Rect
        /// contains the specified point.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>
        /// true if the rectangle described by the Windows.Foundation.Rect contains the
        /// specified point; otherwise, false.
        /// </returns>
        public bool Contains(Point point)
        {
            bool ret = X <= point.X && X + Width >= point.X; //we check if the point is contained in the rectangle considering only the X-axis
            ret = ret && Y <= point.Y && Y + Height >= point.Y; //if the point is contained in the X-axis, we check if the point is contained in the rectangle considering only the Y-axis
            return ret;
        }

        /// <summary>
        /// Indicates whether the specified object is equal to the
        /// current Windows.Foundation.Rect.
        /// </summary>
        /// <param name="o">The object to compare to the current rectangle.</param>
        /// <returns>
        /// true if o is a Windows.Foundation.Rect and has the same x,y,width,height
        /// values as the current Windows.Foundation.Rect; otherwise, false.
        /// </returns>
        public override bool Equals(object o)
        {
            if (o is Rect)
            {
                return ((Rect)o) == this;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified Windows.Foundation.Rect
        /// is equal to the current Windows.Foundation.Rect.
        /// </summary>
        /// <param name="value">The rectangle to compare to the current rectangle.</param>
        /// <returns>
        /// true if the specified Windows.Foundation.Rect has the same x,y,width,height
        /// property values as the current Windows.Foundation.Rect; otherwise, false.
        /// </returns>
        public bool Equals(Rect value)
        {
            return value == this;
        }

        /// <summary>
        /// Creates a hash code for the Windows.Foundation.Rect.
        /// </summary>
        /// <returns>A hash code for the current Windows.Foundation.Rect structure.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

#if WORKINPROGRESS
        public void Intersect(Rect rect)
        {

        }
#endif

        public static Rect Parse(string rectAsString)
        {
            Rect rect = new Rect();
            string[] rectAsStringSplittedOverBlanks = rectAsString.Split(' ');
            List<string> rectAsStringSplittedOverBlanksWithoutWhiteSpaces = new List<string>();
            foreach (string s in rectAsStringSplittedOverBlanks)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    rectAsStringSplittedOverBlanksWithoutWhiteSpaces.Add(s);
                }
            }
            if (rectAsStringSplittedOverBlanksWithoutWhiteSpaces.Count == 4)
            {
                rect._x = double.Parse(rectAsStringSplittedOverBlanksWithoutWhiteSpaces[0]);
                rect._y = double.Parse(rectAsStringSplittedOverBlanksWithoutWhiteSpaces[1]);
                rect._width = double.Parse(rectAsStringSplittedOverBlanksWithoutWhiteSpaces[2]);
                rect._height = double.Parse(rectAsStringSplittedOverBlanksWithoutWhiteSpaces[3]);
            }
            else
            {
                throw new FormatException(rectAsString + "is not an eligible value for Rect"); 
            }
            return rect;
        }
      
        //// <summary>
        //// Finds the intersection of the rectangle represented by
        //// the current Windows.Foundation.Rect and the rectangle represented by the
        //// specified Windows.Foundation.Rect, and stores the result as the current Windows.Foundation.Rect.
        //// </summary>
        //// <param name="rect">The rectangle to intersect with the current rectangle.</param>
        //// <exclude/>
        //public void Intersect(Rect rect)
        //{
        //    //todo: finish this

        //    //Rect intersection = Rect.Empty;
        //    //if (rect.X <= X && rect.Width + rect.X >= X)
        //    //{
        //    //    intersection.X = X;
        //    //}
        //    //if (rect.Y <= Y && rect.Height + rect.Y >= Y)
        //    //{
        //    //    intersection.Y = Y;
        //    //}

        //    //if (X <= rect.X && Width + X >= rect.X)
        //    //{
        //    //    intersection.X = rect.X;
        //    //}
        //    //if (Y <= rect.Y && Height + Y >= rect.Y)
        //    //{
        //    //    intersection.Y = rect.Y;
        //    //}
        //}

        /// <summary>
        /// Returns a string representation of the Windows.Foundation.Rect
        /// structure.
        /// </summary>
        /// <returns>
        /// A string representation of the current Windows.Foundation.Rect structure.
        /// The string has the following form: "Windows.Foundation.Rect.X,Windows.Foundation.Rect.Y,Windows.Foundation.Rect.Width,Windows.Foundation.Rect.Height".
        /// </returns>
        public override string ToString()
        {
            return X + "," + Y + "," + Width + "," + Height;
        }

        // Summary:
        //     Returns a string representation of the rectangle by using
        //     the specified format provider.
        //
        // Parameters:
        //   provider:
        //     Culture-specific formatting information.
        //
        // Returns:
        //     A string representation of the current rectangle that is determined by the
        //     specified format provider.
        //public string ToString(IFormatProvider provider);
        //todo

        //
        // Summary:
        //     Expands the rectangle represented by the current Windows.Foundation.Rect
        //     exactly enough to contain the specified point.
        //
        // Parameters:
        //   point:
        //     The point to include.
        /// <summary>
        /// Expands the rectangle represented by the current Windows.Foundation.Rect
        /// exactly enough to contain the specified point.
        /// </summary>
        /// <param name="point">The point to include.</param>
        public void Union(Point point)
        {
            if (point.X < X)
            {
                Width = Width + X - point.X;
                X = point.X;
            }
            else if (point.X > X+Width)
            {
                Width = Width + point.X - X;
            }

            if (point.Y < Y)
            {
                Height = Height + Y - point.Y;
                Y = point.Y;
            }
            else if (point.Y > Y + Height)
            {
                Height = Height + point.Y - Y;
            }
        }
        
        /// <summary>
        /// Expands the rectangle represented by the current Windows.Foundation.Rect
        /// exactly enough to contain the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to include.</param>
        public void Union(Rect rect)
        {
            if (rect.X < X)
            {
                Width = Width + X - rect.X;
                X = rect.X;
            }
            else if (rect.X + rect.Width > X + Width)
            {
                Width = Width + rect.X + rect.Width - X;
            }

            if (rect.Y < Y)
            {
                Height = Height + Y - rect.Y;
                Y = rect.Y;
            }
            else if (rect.Y + rect.Height > Y + Height)
            {
                Height = Height + rect.Y + rect.Height - Y;
            }
        }

        static Rect()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Rect), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string rectAsString)
        {
            return Parse(rectAsString);
        }
    }
}