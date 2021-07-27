

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


#if BRIDGE
using System;
#endif
using System.ComponentModel;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Describes the width, height, and point origin of a rectangle.
    /// </summary>
    [TypeConverter(typeof(RectConverter))]
    public partial struct Rect
    {
        internal double _x;
        internal double _y;
        internal double _width;
        internal double _height;

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
        /// Initializes a new instance of the System.Windows.Rect structure that is of the
        /// specified size and is located at (0,0).
        /// </summary>
        /// <param name="size">The size of the new Windows.Foundation.Rect.</param>
        public Rect(Size size)
        {
            //setting the elements of the struct to their default value (we don't set it to the given value because it could be an anothorized value)
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;

            Width = size.Width;
            Height = size.Height;
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
            return rect1.X != rect2.X || rect1.Y != rect2.Y ||
                rect1.Width != rect2.Width || rect1.Height != rect2.Height;
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
            return rect1.X == rect2.X && rect1.Y == rect2.Y &&
                rect1.Width == rect2.Width && rect1.Height == rect2.Height;
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
        /// Gets the position of the bottom-left corner of the rectangle
        /// </summary>
        public Point BottomLeft
        {
            get
            {
                return new Point(Left, Bottom);
            }
        }

        /// <summary>
        /// Gets the position of the bottom-right corner of the rectangle.
        /// </summary>
        public Point BottomRight
        {
            get
            {
                return new Point(Right, Bottom);
            }
        }

        /// <summary>
        /// Gets a special value that represents a rectangle with
        /// no position or area.
        /// </summary>
        public static Rect Empty { get; }

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
                if (value < 0)
                    throw new ArgumentException("Height cannot be lower than 0");
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty Rect.");

                _height = value;
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
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return X;
            }
        }

        /// <summary>
        /// Gets or sets the position of the top-left corner of the rectangle.
        /// </summary>
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty Rect.");

                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle.
        /// </summary>
        public double Right
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return X + Width;
            }
        }

        /// <summary>
        /// Gets or sets the width and height of the rectangle.
        /// </summary>
        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty Rect.");

                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets the y-axis position of the top of the rectangle.
        /// </summary>
        public double Top
        {
            get
            {
                if (IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return Y;
            }
        }

        /// <summary>
        /// Gets the position of the top-left corner of the rectangle
        /// </summary>
        public Point TopLeft
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        /// <summary>
        /// Gets the position of the top-right corner of the rectangle.
        /// </summary>
        public Point TopRight
        {
            get
            {
                return new Point(Right, Top);
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
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty Rect.");

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
                if (IsEmpty)
                    throw new InvalidOperationException("Cannot modify empty Rect.");

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

        /// <summary>
        /// Finds the intersection of the rectangle represented by the current <see cref="Rect" />
        /// and the rectangle represented by the specified <see cref="Rect" />,
        /// and stores the result as the current <see cref="Rect"/>.
        /// </summary>
        /// <param name="rect">The rectangle to intersect with the current rectangle.</param>
        public void Intersect(Rect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Rect.Empty;
            }
            else
            {
                var num1 = Math.Max(this._x, rect.X);
                var num2 = Math.Max(this._y, rect.Y);
                this._width = Math.Max(Math.Min(this._x + this._width, rect.X + rect.Width) - num1, 0.0);
                this._height = Math.Max(Math.Min(this._y + this._height, rect.Y + rect.Height) - num2, 0.0);
                this._x = num1;
                this._y = num2;
            }
        }

        /// <summary>
        /// Checks if the current <see cref="Rect" /> intersects with the specified <see cref="Rect" />.
        /// </summary>
        /// <param name="rect">The rectangle to intersect with the current rectangle.</param>
        /// <returns><c>true</c> if the current rectangle intersects with the specified rectangle, <c>false</c> otherwise</returns>
        private bool IntersectsWith(Rect rect)
        {
            return this._width >= 0.0 && rect.Width >= 0.0
                                      && rect.X <= this._x + this._width
                                      && rect.X + rect.Width >= this._x
                                      && rect.Y <= this._y + this._height
                                      && rect.Y + rect.Height >= this._y;
        }

#if WORKINPROGRESS
        /// <summary>
        /// Returns a string representation of the rectangle by using
        /// the specified format provider.
        /// </summary>
        /// <param name="provider">Culture-specific formatting information.</param>
		[OpenSilver.NotImplemented]
        public string ToString(IFormatProvider provider)
        {
            return default(string);
        }
#endif

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
            return string.Concat(X, ", ", Y, ", ", Width, ", ", Height);
        }

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
            Empty = new Rect
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity,
                _x = double.PositiveInfinity,
                _y = double.PositiveInfinity
            };
        }
    }
}