
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
    /// [SECURITY CRITICAL] Describes the characteristics of a rounded corner, such
    /// as can be applied to a Windows.UI.Xaml.Controls.Border.
    /// </summary>
    public partial struct CornerRadius
    {
        private double _topLeftRadius;
        private double _bottomLeftRadius;
        private double _topRightRadius;
        private double _bottomRightRadius;

        /// <summary>
        /// [SECURITY CRITICAL] Initializes a new Windows.UI.Xaml.CornerRadius structure,
        /// applying the same uniform radius to all its corners.
        /// </summary>
        /// <param name="uniformRadius">
        /// A uniform radius applied to all four Windows.UI.Xaml.CornerRadius properties
        /// (Windows.UI.Xaml.CornerRadius.TopLeft, Windows.UI.Xaml.CornerRadius.TopRight,
        /// Windows.UI.Xaml.CornerRadius.BottomRight, Windows.UI.Xaml.CornerRadius.BottomLeft).
        /// </param>
        public CornerRadius(double uniformRadius)
        {
            _topLeftRadius = uniformRadius;
            _bottomLeftRadius = uniformRadius;
            _topRightRadius = uniformRadius;
            _bottomRightRadius = uniformRadius;
        }
     
        /// <summary>
        /// [SECURITY CRITICAL] Initializes a new instance of the Windows.UI.Xaml.CornerRadius
        /// structure, applying specific radius values to its corners.
        /// </summary>
        /// <param name="topLeft">Sets the initial Windows.UI.Xaml.CornerRadius.TopLeft.</param>
        /// <param name="topRight">Sets the initial Windows.UI.Xaml.CornerRadius.TopRight.</param>
        /// <param name="bottomRight">Sets the initial Windows.UI.Xaml.CornerRadius.BottomLeft.</param>
        /// <param name="bottomLeft">Sets the initial Windows.UI.Xaml.CornerRadius.BottomRight.</param>
        public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
        {
            _topLeftRadius = topLeft;
            _bottomLeftRadius = bottomLeft;
            _topRightRadius = topRight;
            _bottomRightRadius = bottomRight;
        }

        /// <summary>
        /// [SECURITY CRITICAL] Compares two Windows.UI.Xaml.CornerRadius structures
        /// for inequality.
        /// </summary>
        /// <param name="cr1">The first structure to compare.</param>
        /// <param name="cr2">The other structure to compare.</param>
        /// <returns>true if the two instances of Windows.UI.Xaml.CornerRadius are not equal; otherwise, false.</returns>
        public static bool operator !=(CornerRadius cr1, CornerRadius cr2)
        {
            return (cr1.TopLeft != cr2.TopLeft || cr1.TopRight != cr2.TopRight || cr1.BottomRight != cr2.BottomRight || cr1.BottomLeft != cr2.BottomLeft);
        }

        /// <summary>
        /// [SECURITY CRITICAL] Compares the value of two Windows.UI.Xaml.CornerRadius
        /// structures for equality.
        /// </summary>
        /// <param name="cr1">The first structure to compare.</param>
        /// <param name="cr2">The other structure to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.CornerRadius are equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(CornerRadius cr1, CornerRadius cr2)
        {
            return (cr1.TopLeft == cr2.TopLeft && cr1.TopRight == cr2.TopRight && cr1.BottomRight == cr2.BottomRight && cr1.BottomLeft == cr2.BottomLeft);
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the radius of rounding, in pixels, of the
        /// bottom left corner of the object where a Windows.UI.Xaml.CornerRadius is
        /// applied.
        /// </summary>
        public double BottomLeft
        {
            get { return _bottomLeftRadius; }
            set { _bottomLeftRadius = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the radius of rounding, in pixels, of the
        /// bottom right corner of the object where a Windows.UI.Xaml.CornerRadius is
        /// applied.
        /// </summary>
        public double BottomRight
        {
            get { return _bottomRightRadius; }
            set { _bottomRightRadius = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the radius of rounding, in pixels, of the
        /// top left corner of the object where a Windows.UI.Xaml.CornerRadius is applied.
        /// </summary>
        public double TopLeft
        {
            get { return _topLeftRadius; }
            set { _topLeftRadius = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the radius of rounding, in pixels, of the
        /// top right corner of the object where a Windows.UI.Xaml.CornerRadius is applied.
        /// </summary>
        public double TopRight
        {
            get { return _topRightRadius; }
            set { _topRightRadius = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.CornerRadius structure
        /// to another Windows.UI.Xaml.CornerRadius structure for equality.
        /// </summary>
        /// <param name="cornerRadius">An instance of Windows.UI.Xaml.CornerRadius to compare for equality.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.CornerRadius are equal; otherwise,
        /// false.
        /// </returns>
        public bool Equals(CornerRadius cornerRadius)
        {
            return this == cornerRadius;
        }
    
        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.CornerRadius structure
        /// to another object for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CornerRadius)
            {
                return ((CornerRadius)obj) == this;
            }
            return false;
        }
       
        /// <summary>
        /// [SECURITY CRITICAL] Returns the hash code of the structure.
        /// </summary>
        /// <returns>A hash code for this Windows.UI.Xaml.CornerRadius.</returns>
        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() ^ TopRight.GetHashCode() ^ BottomRight.GetHashCode() ^ BottomLeft.GetHashCode();
        }
      
        /// <summary>
        /// [SECURITY CRITICAL] Returns the string representation of the Windows.UI.Xaml.CornerRadius
        /// structure.
        /// </summary>
        /// <returns>A System.String that represents the Windows.UI.Xaml.CornerRadius value.</returns>
        public override string ToString()
        {
            return CornerRadiusConverter.ToString(this, null);
        }
    }
}