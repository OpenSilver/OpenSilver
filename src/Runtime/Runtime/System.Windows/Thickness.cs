
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
using System.Globalization;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Describes the thickness of a frame around a rectangle. Four <see cref="double"/> values
    /// describe the <see cref="Left"/>, <see cref="Top"/>, <see cref="Right"/>, and 
    /// <see cref="Bottom"/> sides of the rectangle, respectively.
    /// </summary>
    public struct Thickness
    {
        /// <summary>
        /// Initializes a <see cref="Thickness"/> structure that has the specified uniform
        /// length on each side.
        /// </summary>
        /// <param name="uniformLength">
        /// The uniform length applied to all four sides of the bounding rectangle.
        /// </param>
        public Thickness(double uniformLength)
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        /// <summary>
        /// Initializes a <see cref="Thickness"/> structure that has specific lengths (supplied
        /// as a <see cref="double"/>) applied to each side of the rectangle.
        /// </summary>
        /// <param name="left">
        /// The thickness for the left side of the rectangle.
        /// </param>
        /// <param name="top">
        /// The thickness for the upper side of the rectangle.
        /// </param>
        /// <param name="right">
        /// The thickness for the right side of the rectangle
        /// </param>
        /// <param name="bottom">
        /// The thickness for the lower side of the rectangle.
        /// </param>
        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the width, in pixels, of the lower side of the
        /// bounding rectangle for this instance of <see cref="Thickness"/>. The default is 0.
        /// </returns>
        public double Bottom { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the left side of the bounding rectangle.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the width, in pixels, of the left side of the
        /// bounding rectangle for this instance of <see cref="Thickness"/>. The default is 0.
        /// </returns>
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the right side of the bounding rectangle.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the width, in pixels, of the right side of the
        /// bounding rectangle for this instance of <see cref="Thickness"/>. The default is 0.
        /// </returns>
        public double Right { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the width, in pixels, of the upper side of the
        /// bounding rectangle for this instance of <see cref="Thickness"/>. The default is 0.
        /// </returns>
        public double Top { get; set; }

        /// <summary>
        /// Compares this <see cref="Thickness"/> structure to another <see cref="object"/> for equality.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// true if the two objects are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
            => obj is Thickness thickness && this == thickness;

        /// <summary>
        /// Compares this <see cref="Thickness"/> structure to another <see cref="Thickness"/>
        /// structure for equality.
        /// </summary>
        /// <param name="thickness">
        /// An instance of <see cref="Thickness"/> to compare for equality.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="Thickness"/> are equal; otherwise, false.
        /// </returns>
        public bool Equals(Thickness thickness) => this == thickness;

        /// <summary>
        /// Returns the hash code of the structure.
        /// </summary>
        /// <returns>
        /// A hash code for this instance of <see cref="Thickness"/>.
        /// </returns>
        public override int GetHashCode()
            => Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();

        /// <summary>
        /// Returns the string representation of the <see cref="Thickness"/> structure.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the <see cref="Thickness"/> value.
        /// </returns>
        public override string ToString() => ThicknessConverter.ToString(this, CultureInfo.InvariantCulture);

        /// <summary>
        /// Compares the value of two <see cref="Thickness"/> structures for equality.
        /// </summary>
        /// <param name="t1">
        /// The first structure to compare.
        /// </param>
        /// <param name="t2">
        /// The other structure to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="Thickness"/> are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(Thickness t1, Thickness t2)
            => t1.Left == t2.Left && t1.Top == t2.Top && t1.Bottom == t2.Bottom && t1.Right == t2.Right;

        /// <summary>
        /// Compares two <see cref="Thickness"/> structures for inequality.
        /// </summary>
        /// <param name="t1">
        /// The first structure to compare.
        /// </param>
        /// <param name="t2">
        /// The other structure to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="Thickness"/> are not equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator !=(Thickness t1, Thickness t2) => !(t1 == t2);
    }
}