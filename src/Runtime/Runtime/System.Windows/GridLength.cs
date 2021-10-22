
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
    /// Represents the length of elements that explicitly support Windows.UI.Xaml.GridUnitType.Star
    /// unit types.
    /// </summary>
    public partial struct GridLength
    {
        /// <summary>
        /// Returns a new instance of Gridlength with the same properties values.
        /// </summary>
        /// <returns>A new instance of Gridlength with the same properties values.</returns>
        public GridLength Clone()
        {
            return new GridLength()
            {
                _type = this._type,
                _value = this._value
            };
        }

        private double _value;
        private GridUnitType _type;

        /// <summary>
        /// Initializes a new instance of the Windows.UI.Xaml.GridLength
        /// structure using the specified absolute value in pixels.
        /// </summary>
        /// <param name="pixels">The absolute count of pixels to establish as the value.</param>
        public GridLength(double pixels)
        {
            _value = pixels;
            _type = GridUnitType.Pixel;
        }

        /// <summary>
        /// Initializes a new instance of the Windows.UI.Xaml.GridLength
        /// structure and specifies what kind of value it holds.
        /// </summary>
        /// <param name="value">The initial value of this instance of Windows.UI.Xaml.GridLength.</param>
        /// <param name="type">The Windows.UI.Xaml.GridUnitType held by this instance of Windows.UI.Xaml.GridLength.</param>
        public GridLength(double value, GridUnitType type)
        {
            _value = (type == GridUnitType.Auto ? 0.0 : value);
            _type = type;
        }

        /// <summary>
        /// Gets an instance of Windows.UI.Xaml.GridLength that holds
        /// a value whose size is determined by the size properties of the content object.
        /// </summary>
        public static GridLength Auto
        {
            get
            {
                return new GridLength(1.0, GridUnitType.Auto);
            }
        }
        
        /// <summary>
        /// Gets the associated Windows.UI.Xaml.GridUnitType for
        /// the Windows.UI.Xaml.GridLength.
        /// </summary>
        public GridUnitType GridUnitType
        {
            get
            {
                return _type;
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether the Windows.UI.Xaml.GridLength
        /// holds a value that is expressed in pixels.
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return _type == GridUnitType.Pixel;
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether the Windows.UI.Xaml.GridLength
        /// holds a value whose size is determined by the size properties of the content
        /// object.
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return _type == GridUnitType.Auto;
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether the Windows.UI.Xaml.GridLength
        /// holds a value that is expressed as a weighted proportion of available space.
        /// </summary>
        public bool IsStar
        {
            get
            {
                return _type == GridUnitType.Star;
            }
        }
        
        /// <summary>
        /// Gets a System.Double that represents the value of the
        /// Windows.UI.Xaml.GridLength.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
        }
 
        /// <summary>
        /// Compares two Windows.UI.Xaml.GridLength structures to
        /// determine if they are not equal.
        /// </summary>
        /// <param name="gl1">The first instance of Windows.UI.Xaml.GridLength to compare.</param>
        /// <param name="gl2">The second instance of Windows.UI.Xaml.GridLength to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.GridLength do not have the same
        /// value and Windows.UI.Xaml.GridUnitType; otherwise, false.
        /// </returns>
        public static bool operator !=(GridLength gl1, GridLength gl2)
        {
            return (gl1.GridUnitType != gl2.GridUnitType || gl1.Value != gl2.Value);
        }
        
        /// <summary>
        /// Compares two Windows.UI.Xaml.GridLength structures for
        /// equality.
        /// </summary>
        /// <param name="gl1">The first instance of Windows.UI.Xaml.GridLength to compare.</param>
        /// <param name="gl2">The second instance of Windows.UI.Xaml.GridLength to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.GridLength have the same value
        /// and Windows.UI.Xaml.GridUnitType; otherwise, false.
        /// </returns>
        public static bool operator ==(GridLength gl1, GridLength gl2)
        {
            return (gl1.GridUnitType == gl2.GridUnitType && gl1.Value == gl2.Value);
        }

        /// <summary>
        /// Determines whether the specified Windows.UI.Xaml.GridLength
        /// is equal to the current Windows.UI.Xaml.GridLength.
        /// </summary>
        /// <param name="gridLength">The Windows.UI.Xaml.GridLength structure to compare with the current instance.</param>
        /// <returns>
        /// true if the specified Windows.UI.Xaml.GridLength has the same value and Windows.UI.Xaml.GridLength.GridUnitType
        /// as the current instance; otherwise, false.
        /// </returns>
        public bool Equals(GridLength gridLength)
        {
            return (this == gridLength);
        }
        
        /// <summary>
        /// Determines whether the specified object is equal to the
        /// current Windows.UI.Xaml.GridLength instance.
        /// </summary>
        /// <param name="oCompare">The object to compare with the current instance.</param>
        /// <returns>
        /// true if the specified object has the same value and Windows.UI.Xaml.GridUnitType
        /// as the current instance; otherwise, false.
        /// </returns>
        public override bool Equals(object oCompare)
        {
            if (oCompare is GridLength)
            {
                return (this == (GridLength)oCompare);
            }
            else
                return false;
        }
        
        /// <summary>
        /// Gets a hash code for the Windows.UI.Xaml.GridLength.
        /// </summary>
        /// <returns>A hash code for the Windows.UI.Xaml.GridLength.</returns>
        public override int GetHashCode()
        {
            return ((int)_value) * 7 + (int)_type;
        }
         
        /// <summary>
        /// Returns a System.String representation of the Windows.UI.Xaml.GridLength.
        /// </summary>
        /// <returns>
        /// A System.String representation of the current Windows.UI.Xaml.GridLength
        /// structure.
        /// </returns>
        public override string ToString()
        {
            return GridLengthConverter.ToString(this, CultureInfo.InvariantCulture);
        }
    }
}