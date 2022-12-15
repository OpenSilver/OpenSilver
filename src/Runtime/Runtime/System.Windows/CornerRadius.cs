
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
    /// Describes the characteristics of a rounded corner, such as can be applied to
    /// a <see cref="Controls.Border"/>.
    /// </summary>
    public partial struct CornerRadius
    {
        /// <summary>
        /// Initializes a new <see cref="CornerRadius"/> structure, applying the same uniform
        /// radius to all its corners.
        /// </summary>
        /// <param name="uniformRadius">
        /// A uniform radius applied to all four <see cref="CornerRadius"/> properties (<see cref="TopLeft"/>,
        /// <see cref="TopRight"/>, <see cref="BottomRight"/>, <see cref="BottomLeft"/>).
        /// </param>
        public CornerRadius(double uniformRadius)
        {
            TopLeft = BottomLeft = TopRight = BottomRight = uniformRadius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornerRadius"/> structure, applying
        /// specific radius values to its corners.
        /// </summary>
        /// <param name="topLeft">
        /// Sets the initial <see cref="TopLeft"/>.
        /// </param>
        /// <param name="topRight">
        /// Sets the initial <see cref="TopRight"/>.
        /// </param>
        /// <param name="bottomRight">
        /// Sets the initial <see cref="BottomLeft"/>.
        /// </param>
        /// <param name="bottomLeft">
        /// Sets the initial <see cref="BottomRight"/>.
        /// </param>
        public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
        {
            TopLeft = topLeft;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
        }

        /// <summary>
        /// Gets or sets the radius of rounding, in pixels, of the bottom left corner of
        /// the object where a <see cref="CornerRadius"/> is applied.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the radius of rounding, in pixels, of the bottom
        /// left corner of the object where a <see cref="CornerRadius"/> is applied. The
        /// default is 0.
        /// </returns>
        public double BottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the radius of rounding, in pixels, of the bottom right corner of
        /// the object where a <see cref="CornerRadius"/> is applied.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the radius of rounding, in pixels, of the bottom
        /// right corner of the object where a <see cref="CornerRadius"/> is applied. The
        /// default is 0.
        /// </returns>
        public double BottomRight { get; set; }

        /// <summary>
        /// Gets or sets the radius of rounding, in pixels, of the top left corner of the
        /// object where a <see cref="CornerRadius"/> is applied.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the radius of rounding, in pixels, of the top
        /// left corner of the object where a <see cref="CornerRadius"/> is applied. The
        /// default is 0.
        /// </returns>
        public double TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the radius of rounding, in pixels, of the top right corner of the
        /// object where a <see cref="CornerRadius"/> is applied.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the radius of rounding, in pixels, of the top
        /// right corner of the object where a <see cref="CornerRadius"/> is applied. The
        /// default is 0.
        /// </returns>
        public double TopRight { get; set; }

        /// <summary>
        /// Compares this <see cref="CornerRadius"/> structure to another object for equality.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// true if the two objects are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
            => obj is CornerRadius cornerRadius && this == cornerRadius;

        /// <summary>
        /// Compares this <see cref="CornerRadius"/> structure to another <see cref="CornerRadius"/>
        /// structure for equality.
        /// </summary>
        /// <param name="cornerRadius">
        /// An instance of <see cref="CornerRadius"/> to compare for equality.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="CornerRadius"/> are equal; otherwise, false.
        /// </returns>
        public bool Equals(CornerRadius cornerRadius) => this == cornerRadius;

        /// <summary>
        /// Returns the hash code of the structure.
        /// </summary>
        /// <returns>
        /// A hash code for this <see cref="CornerRadius"/>.
        /// </returns>
        public override int GetHashCode()
            => TopLeft.GetHashCode() ^ TopRight.GetHashCode() ^ BottomRight.GetHashCode() ^ BottomLeft.GetHashCode();

        /// <summary>
        /// Returns the string representation of the <see cref="CornerRadius"/> structure.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the <see cref="CornerRadius"/> value.
        /// </returns>
        public override string ToString() => CornerRadiusConverter.ToString(this, CultureInfo.InvariantCulture);

        /// <summary>
        /// Compares the value of two <see cref="CornerRadius"/> structures for equality.
        /// </summary>
        /// <param name="cr1">
        /// The first structure to compare.
        /// </param>
        /// <param name="cr2">
        /// The other structure to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="CornerRadius"/> are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(CornerRadius cr1, CornerRadius cr2)
            => cr1.TopLeft == cr2.TopLeft &&
               cr1.TopRight == cr2.TopRight &&
               cr1.BottomRight == cr2.BottomRight &&
               cr1.BottomLeft == cr2.BottomLeft;

        /// <summary>
        /// Compares two <see cref="CornerRadius"/> structures for inequality.
        /// </summary>
        /// <param name="cr1">
        /// The first structure to compare.
        /// </param>
        /// <param name="cr2">
        /// The other structure to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="CornerRadius"/> are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(CornerRadius cr1, CornerRadius cr2) => !(cr1 == cr2);
    }
}