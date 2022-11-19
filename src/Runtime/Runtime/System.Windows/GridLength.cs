
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
    /// Represents the length of elements that explicitly support <see cref="GridUnitType.Star"/>
    /// unit types.
    /// </summary>
    public struct GridLength
    {
        private double _unitValue;      //  unit value storage
        private GridUnitType _unitType; //  unit type storage

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLength"/> structure using the
        /// specified absolute value in pixels.
        /// </summary>
        /// <param name="pixels">
        /// The absolute count of pixels to establish as the value.
        /// </param>
        public GridLength(double pixels)
            : this(pixels, GridUnitType.Pixel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLength"/> structure and specifies
        /// what kind of value it holds.
        /// </summary>
        /// <param name="value">
        /// The initial value of this instance of <see cref="GridLength"/>.
        /// </param>
        /// <param name="type">
        /// The <see cref="GridUnitType"/> held by this instance of <see cref="GridLength"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// value is less than 0 or is not a number.- or -type is not a valid <see cref="GridUnitType"/>.
        /// </exception>
        public GridLength(double value, GridUnitType type)
        {
            if (double.IsNaN(value))
            {
                throw new ArgumentException($"'{value}' parameter cannot be NaN.", nameof(value));
            }
            if (double.IsInfinity(value))
            {
                throw new ArgumentException($"'{value}' parameter cannot be Infinity.", nameof(value));
            }
            if (value < 0.0)
            {
                throw new ArgumentException($"'{value}' parameter cannot be negative.", nameof(value));
            }
            if (type != GridUnitType.Auto
                && type != GridUnitType.Pixel
                && type != GridUnitType.Star)
            {
                throw new ArgumentException(
                    $"'{type}' parameter is not valid. Valid values are GridUnitType.Auto, GridUnitType.Pixel, or GridUnitType.Star.",
                    nameof(type));
            }

            _unitValue = type == GridUnitType.Auto ? 1.0 : value;
            _unitType = type;
        }

        /// <summary>
        /// Gets an instance of <see cref="GridLength"/> that holds a value whose size is
        /// determined by the size properties of the content object.
        /// </summary>
        /// <returns>
        /// A instance of <see cref="GridLength"/> whose <see cref="GridUnitType"/>
        /// property is set to <see cref="GridUnitType.Auto"/>.
        /// </returns>
        public static GridLength Auto { get; } = new GridLength(1.0, GridUnitType.Auto);

        /// <summary>
        /// Gets the associated <see cref="GridUnitType"/> for the <see cref="GridLength"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="GridUnitType"/> values. The default is <see cref="GridUnitType.Auto"/>.
        /// </returns>
        public GridUnitType GridUnitType => _unitType;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="GridLength"/> holds a value
        /// that is expressed in pixels.
        /// </summary>
        /// <returns>
        /// true if the <see cref="GridUnitType"/> property is <see cref="GridUnitType.Pixel"/>;
        /// otherwise, false.
        /// </returns>
        public bool IsAbsolute => _unitType == GridUnitType.Pixel;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="GridLength"/> holds a value
        /// whose size is determined by the size properties of the content object.
        /// </summary>
        /// <returns>
        /// true if the <see cref="GridUnitType"/> property is <see cref="GridUnitType.Auto"/>;
        /// otherwise, false.
        /// </returns>
        public bool IsAuto => _unitType == GridUnitType.Auto;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="GridLength"/> holds a value
        /// that is expressed as a weighted proportion of available space.
        /// </summary>
        /// <returns>
        /// true if the <see cref="GridUnitType"/> property is <see cref="GridUnitType.Star"/>;
        /// otherwise, false.
        /// </returns>
        public bool IsStar => _unitType == GridUnitType.Star;

        /// <summary>
        /// Gets a <see cref="double"/> that represents the value of the <see cref="GridLength"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the value of the current instance.
        /// </returns>
        public double Value => _unitType == GridUnitType.Auto ? 1.0 : _unitValue;

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="GridLength"/>
        /// instance.
        /// </summary>
        /// <param name="oCompare">
        /// The object to compare with the current instance.
        /// </param>
        /// <returns>
        /// true if the specified object has the same value and <see cref="GridUnitType"/>
        /// as the current instance; otherwise, false.
        /// </returns>
        public override bool Equals(object oCompare)
            => oCompare is GridLength gridLength && this == gridLength;

        /// <summary>
        /// Determines whether the specified <see cref="GridLength"/> is equal to the current
        /// <see cref="GridLength"/>.
        /// </summary>
        /// <param name="gridLength">
        /// The <see cref="GridLength"/> structure to compare with the current instance.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="GridLength"/> has the same value and <see cref="GridUnitType"/>
        /// as the current instance; otherwise, false.
        /// </returns>
        public bool Equals(GridLength gridLength) => this == gridLength;

        /// <summary>
        /// Gets a hash code for the <see cref="GridLength"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the <see cref="GridLength"/>.
        /// </returns>
        public override int GetHashCode() => (int)_unitValue + (int)_unitType;

        /// <summary>
        /// Returns a <see cref="string"/> representation of the <see cref="GridLength"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representation of the current <see cref="GridLength"/> structure.
        /// </returns>
        public override string ToString()
            => GridLengthConverter.ToString(this, CultureInfo.InvariantCulture);

        /// <summary>
        /// Compares two <see cref="GridLength"/> structures for equality.
        /// </summary>
        /// <param name="gl1">
        /// The first instance of <see cref="GridLength"/> to compare.
        /// </param>
        /// <param name="gl2">
        /// The second instance of <see cref="GridLength"/> to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="GridLength"/> have the same value and
        /// <see cref="GridUnitType"/>; otherwise, false. 
        /// </returns>
        public static bool operator ==(GridLength gl1, GridLength gl2)
            => gl1.GridUnitType == gl2.GridUnitType && gl1.Value == gl2.Value;

        /// <summary>
        /// Compares two <see cref="GridLength"/> structures to determine if they are not
        /// equal.
        /// </summary>
        /// <param name="gl1">
        /// The first instance of <see cref="GridLength"/> to compare.
        /// </param>
        /// <param name="gl2">
        /// The second instance of <see cref="GridLength"/> to compare.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="GridLength"/> do not have the same value
        /// and <see cref="GridUnitType"/>; otherwise, false.
        /// </returns>
        public static bool operator !=(GridLength gl1, GridLength gl2) => !(gl1 == gl2);

        /// <summary>
        /// Get a copy of the current instance of <see cref="GridLength"/>.
        /// </summary>
        public GridLength Clone() => new GridLength() { _unitType = _unitType, _unitValue = _unitValue };
    }
}