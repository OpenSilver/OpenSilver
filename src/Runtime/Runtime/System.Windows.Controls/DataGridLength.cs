

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


using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    // Summary:
    //     Represents the lengths of elements within the System.Windows.Controls.DataGrid
    //     control.
    /// <summary>
    /// Represents the lengths of elements within the System.Windows.Controls.DataGrid
    /// control.
    /// </summary>
    [TypeConverter(typeof(DataGridLengthConverter))]
    public partial struct DataGridLength : IEquatable<DataGridLength>
    {
        //todo: add SizeToCell and SizeToHeader everywhere when we'll know how to handle them (idea: add in the grid's cell something like INTERNAL_IsRelevantToAuto (depends on how auto works)

        private double _value;
        private DataGridLengthUnitType _type;

        // Exceptions:
        //   System.ArgumentException:
        //     pixels is System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity.
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.DataGridLength
        /// class with an absolute value in pixels.
        /// </summary>
        /// <param name="pixels">The absolute pixel value (96 pixels-per-inch) to initialize the length to.</param>
        public DataGridLength(double pixels)
        {
            _value = pixels;
            _type = DataGridLengthUnitType.Pixel;
        }

        // Exceptions:
        //   System.ArgumentException:
        //     value is System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity.-or-type
        //     is not System.Windows.Controls.DataGridLengthUnitType.Auto, System.Windows.Controls.DataGridLengthUnitType.Pixel,
        //     System.Windows.Controls.DataGridLengthUnitType.Star, System.Windows.Controls.DataGridLengthUnitType.SizeToCells,
        //     or System.Windows.Controls.DataGridLengthUnitType.SizeToHeader.
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.DataGridLength
        /// class with a specified value and unit.
        /// </summary>
        /// <param name="value">The requested size of the element.</param>
        /// <param name="type">The type that is used to determine how the size of the element is calculated.</param>
        public DataGridLength(double value, DataGridLengthUnitType type)
        {
            _value = (type == DataGridLengthUnitType.Auto ? 0.0 : value);
            _type = type;
        }

        ////
        //// Summary:
        ////     Initializes a new instance of the System.Windows.Controls.DataGridLength
        ////     class with the specified value, unit, desired value, and display value.
        ////
        //// Parameters:
        ////   value:
        ////     The requested size of the element.
        ////
        ////   type:
        ////     The type that is used to determine how the size of the element is calculated.
        ////
        ////   desiredValue:
        ////     The calculated size needed for the element.
        ////
        ////   displayValue:
        ////     The allocated size for the element.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     value is System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity.-or-type
        ////     is not System.Windows.Controls.DataGridLengthUnitType.Auto, System.Windows.Controls.DataGridLengthUnitType.Pixel,
        ////     System.Windows.Controls.DataGridLengthUnitType.Star, System.Windows.Controls.DataGridLengthUnitType.SizeToCells,
        ////     or System.Windows.Controls.DataGridLengthUnitType.SizeToHeader.-or-desiredValue
        ////     is System.Double.NegativeInfinity or System.Double.PositiveInfinity.-or-displayValue
        ////     is System.Double.NegativeInfinity or System.Double.PositiveInfinity.
        //public DataGridLength(double value, DataGridLengthUnitType type, double desiredValue, double displayValue);

       
        /// <summary>
        /// Compares two System.Windows.Controls.DataGridLength structures to determine
        /// whether they are not equal.
        /// </summary>
        /// <param name="gl1">The first System.Windows.Controls.DataGridLength instance to compare.</param>
        /// <param name="gl2">The second System.Windows.Controls.DataGridLength instance to compare.</param>
        /// <returns>
        /// true if the two System.Windows.Controls.DataGridLength instances do not have
        /// the same value or sizing mode; otherwise, false.
        /// </returns>
        public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
        {
            return (gl1.UnitType != gl2.UnitType || gl1.Value != gl2.Value);
        }

        /// <summary>
        /// Compares two System.Windows.Controls.DataGridLength structures for equality.
        /// </summary>
        /// <param name="gl1">The first System.Windows.Controls.DataGridLength instance to compare.</param>
        /// <param name="gl2">The second System.Windows.Controls.DataGridLength instance to compare.</param>
        /// <returns>
        /// true if the two System.Windows.Controls.DataGridLength instances have the
        /// same value or sizing mode; otherwise, false.
        /// </returns>
        public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
        {
            return (gl1.UnitType == gl2.UnitType && gl1.Value == gl2.Value);

        }

        /// <summary>
        /// Converts a System.Double to an instance of the System.Windows.Controls.DataGridLength
        /// class.
        /// </summary>
        /// <param name="value">The absolute pixel value (96 pixels-per-inch) to initialize the length to.</param>
        /// <returns>An object that represents the specified length.</returns>
        public static implicit operator DataGridLength(double value)
        {
            return new DataGridLength(value);
        }

       
        /// <summary>
        /// Gets a System.Windows.Controls.DataGridLength structure that represents the
        /// standard automatic sizing mode.
        /// </summary>
        public static DataGridLength Auto {
            get
            {
                return new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
        }

        //// Summary:
        ////     Gets the calculated pixel value needed for the element.
        ////
        //// Returns:
        ////     The number of pixels calculated for the size of the element.
        //public double DesiredValue { get; }

        //// Summary:
        ////     Gets the pixel value allocated for the size of the element.
        ////
        //// Returns:
        ////     The number of pixels allocated for the element.
        //public double DisplayValue { get; }

        /// <summary>
        /// Gets a value that indicates whether this instance sizes elements based on
        /// a fixed pixel value.
        /// </summary>
        public bool IsAbsolute
        {
            get
            {
                return _type == DataGridLengthUnitType.Pixel;
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether this instance automatically sizes elements
        /// based on both the content of cells and the column headers.
        /// </summary>
        public bool IsAuto
        {
            get
            {
                return _type == DataGridLengthUnitType.Auto;
            }
        }
        ////
        //// Summary:
        ////     Gets a value that indicates whether this instance automatically sizes elements
        ////     based on the content of the cells.
        ////
        //// Returns:
        ////     true if the System.Windows.Controls.DataGridLength.UnitType property is set
        ////     to System.Windows.Controls.DataGridLengthUnitType.SizeToCells; otherwise,
        ////     false.
        //public bool IsSizeToCells
        //{
        //    get
        //    {
        //        return _type == DataGridLengthUnitType.SizeToCells;
        //    }
        //}
        ////
        //// Summary:
        ////     Gets a value that indicates whether this instance automatically sizes elements
        ////     based on the header.
        ////
        //// Returns:
        ////     true if the System.Windows.Controls.DataGridLength.UnitType property is set
        ////     to System.Windows.Controls.DataGridLengthUnitType.SizeToHeader; otherwise,
        ////     false.
        //public bool IsSizeToHeader
        //{
        //    get
        //    {
        //        return _type == DataGridLengthUnitType.SizeToHeader;
        //    }
        //}
        
        /// <summary>
        /// Gets a value that indicates whether this instance automatically sizes elements
        /// based on a weighted proportion of available space.
        /// </summary>
        public bool IsStar
        {
            get
            {
                return _type == DataGridLengthUnitType.Star;
            }
        }
        ////
        //// Summary:
        ////     Gets a System.Windows.Controls.DataGridLength structure that represents the
        ////     cell-based automatic sizing mode.
        ////
        //// Returns:
        ////     A System.Windows.Controls.DataGridLength structure that represents the cell-based
        ////     automatic sizing mode.
        //public static DataGridLength SizeToCells
        //{
        //    get
        //    {
        //        return new DataGridLength(0.0,DataGridLengthUnitType.SizeToCells);
        //    }
        //}
        ////
        //// Summary:
        ////     Gets a System.Windows.Controls.DataGridLength structure that represents the
        ////     header-based automatic sizing mode.
        ////
        //// Returns:
        ////     A System.Windows.Controls.DataGridLength structure that represents the header-based
        ////     automatic sizing mode.
        //public static DataGridLength SizeToHeader
        //{
        //    get
        //    {
        //        return new DataGridLength(0.0, DataGridLengthUnitType.SizeToHeader);
        //    }
        //}
        
        /// <summary>
        /// Gets the type that is used to determine how the size of the element is calculated.
        /// </summary>
        public DataGridLengthUnitType UnitType
        {
            get
            {
                return _type;
            }
        }
        
        
        /// <summary>
        /// Gets the absolute value of the System.Windows.Controls.DataGridLength in
        /// pixels, or 1.0 if the System.Windows.Controls.DataGridLength.UnitType property is
        /// set to System.Windows.Controls.DataGridLengthUnitType.Auto.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Determines whether the specified System.Windows.Controls.DataGridLength is
        /// equal to the current System.Windows.Controls.DataGridLength.
        /// </summary>
        /// <param name="other">The System.Windows.Controls.DataGridLength to compare to the current instance.</param>
        /// <returns>
        /// true if the specified object is a System.Windows.Controls.DataGridLength
        /// with the same value or sizing mode as the current System.Windows.Controls.DataGridLength;
        /// otherwise, false.
        /// </returns>
        public bool Equals(DataGridLength other)
        {
            return this == other;
        }
        
        /// <summary>
        /// Determines whether the specified object is equal to the current System.Windows.Controls.DataGridLength.
        /// </summary>
        /// <param name="obj">The object to compare to the current instance.</param>
        /// <returns>
        /// true if the specified object is a System.Windows.Controls.DataGridLength
        /// with the same value or sizing mode as the current System.Windows.Controls.DataGridLength;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ((obj is DataGridLength) && (this == (DataGridLength)obj));
        }
        
        /// <summary>
        /// Gets a hash code for the System.Windows.Controls.DataGridLength.
        /// </summary>
        /// <returns>A hash code for the current System.Windows.Controls.DataGridLength.</returns>
        public override int GetHashCode()
        {
            return (int)_value * 7 + (int)_type;
        }
        
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represent the current object.</returns>
        public override string ToString()
        {
            if (_type == DataGridLengthUnitType.Auto)
            {
                return "Auto";
            }
            //else if (_type == DataGridLengthUnitType.SizeToCells)
            //{
            //    return "SizeToCells";
            //}
            //else if (_type == DataGridLengthUnitType.SizeToHeader)
            //{
            //    return "SizeToHeader";
            //}
            else if (_type == DataGridLengthUnitType.Star)
            {
                return (_value == 1 ? "*" : _value.ToString() + "*");
            }
            else
            {
                return _value.ToString();
            }
        }
    }
}
