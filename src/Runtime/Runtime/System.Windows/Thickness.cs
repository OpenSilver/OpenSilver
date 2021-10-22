
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

using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// [SECURITY CRITICAL] Describes the thickness of a frame around a rectangle.
    /// Four System.Double values describe the Windows.UI.Xaml.Thickness.Left, Windows.UI.Xaml.Thickness.Top,
    /// Windows.UI.Xaml.Thickness.Right, and Windows.UI.Xaml.Thickness.Bottom sides
    /// of the rectangle, respectively.
    /// </summary>
    public partial struct Thickness
    {
        double _left, _top, _right, _bottom;

        /// <summary>
        /// [SECURITY CRITICAL] Initializes a Windows.UI.Xaml.Thickness structure that
        /// has the specified uniform length on each side.
        /// </summary>
        /// <param name="uniformLength">The uniform length applied to all four sides of the bounding rectangle.</param>
        public Thickness(double uniformLength)
        {
            _left = uniformLength;
            _top = uniformLength;
            _right = uniformLength;
            _bottom = uniformLength;
        }
       
        /// <summary>
        /// [SECURITY CRITICAL] Initializes a Windows.UI.Xaml.Thickness structure that
        /// has specific lengths (supplied as a System.Double) applied to each side of
        /// the rectangle.
        /// </summary>
        /// <param name="left">The thickness for the left side of the rectangle.</param>
        /// <param name="top">The thickness for the upper side of the rectangle.</param>
        /// <param name="right">The thickness for the right side of the rectangle</param>
        /// <param name="bottom">The thickness for the lower side of the rectangle.</param>
        public Thickness(double left, double top, double right, double bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Compares two Windows.UI.Xaml.Thickness structures for
        /// inequality.
        /// </summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are not equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator !=(Thickness t1, Thickness t2)
        {
            return (t1.Left != t2.Left || t1.Top != t2.Top || t1.Bottom != t2.Bottom || t1.Right != t2.Right);
        }
      
        /// <summary>
        /// [SECURITY CRITICAL] Compares the value of two Windows.UI.Xaml.Thickness structures
        /// for equality.</summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(Thickness t1, Thickness t2)
        {
            return (t1.Left == t2.Left && t1.Top == t2.Top && t1.Bottom == t2.Bottom && t1.Right == t2.Right);
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the lower side
        /// of the bounding rectangle.
        /// </summary>
        public double Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the left side of
        /// the bounding rectangle.
        /// </summary>
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the right side
        /// of the bounding rectangle.
        /// </summary>
        public double Right
        {
            get { return _right; }
            set { _right = value; }
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the upper side
        /// of the bounding rectangle.
        /// </summary>
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.Thickness structure to
        /// another System.Object for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Thickness && ((Thickness)obj) == this);
        }
       
        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.Thickness structure to
        /// another Windows.UI.Xaml.Thickness structure for equality.
        /// </summary>
        /// <param name="thickness">An instance of Windows.UI.Xaml.Thickness to compare for equality.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are equal; otherwise,
        /// false.
        /// </returns>
        public bool Equals(Thickness thickness)
        {
            return thickness == this;
        }

        /// <summary>
        /// [SECURITY CRITICAL] Returns the hash code of the structure.
        /// </summary>
        /// <returns>A hash code for this instance of Windows.UI.Xaml.Thickness.</returns>
        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        /// <summary>
        /// Returns the string representation of the <see cref="Thickness"/> structure.
        /// structure.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the <see cref="Thickness"/> value.
        /// </returns>
        public override string ToString()
        {
            return ThicknessConverter.ToString(this, null);
        }
    }
}