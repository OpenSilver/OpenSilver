using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Controls.DataVisualization
{
    internal class ResourceDictionaryDispensedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ResourceDictionaryDispensedEventArgs class.
        /// </summary>
        /// <param name="index">The index of the ResourceDictionary dispensed.</param>
        /// <param name="resourceDictionary">The ResourceDictionary dispensed.</param>
        public ResourceDictionaryDispensedEventArgs(int index, ResourceDictionary resourceDictionary)
        {
            this.ResourceDictionary = resourceDictionary;
            this.Index = index;
        }

        /// <summary>Gets the index of the ResourceDictionary dispensed.</summary>
        public int Index { get; private set; }

        /// <summary>Gets the ResourceDictionary dispensed.</summary>
        public ResourceDictionary ResourceDictionary { get; private set; }

        /// <summary>
        /// Returns a value indicating whether two objects are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>A value indicating whether the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>Returns a hash code.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>Units of measure.</summary>
    public enum Unit
    {
        Pixels,
        Degrees,
    }
    /// <summary>A value in units.</summary>
    public struct UnitValue : IComparable
    {
        /// <summary>Returns a UnitValue representing an invalid value.</summary>
        /// <returns>UnitValue instance.</returns>
        public static UnitValue NaN()
        {
            return new UnitValue() { Value = double.NaN };
        }

        /// <summary>Instantiates a new instance of the UnitValue struct.</summary>
        /// <param name="value">The value associated with the units.</param>
        /// <param name="unit">The units associated with the value.</param>
        public UnitValue(double value, Unit unit)
        {
            this = new UnitValue();
            this.Value = value;
            this.Unit = unit;
        }

        /// <summary>Gets the value associated with the units.</summary>
        public double Value { get; private set; }

        /// <summary>Gets the units associated with the value.</summary>
        public Unit Unit { get; private set; }

        /// <summary>
        /// Compares two unit values to determine if they are equal or not.
        /// </summary>
        /// <param name="obj">The object being compared.</param>
        /// <returns>A number smaller than zero if the obj is larger than this
        /// object.  A number equal to 0 if they are equal.  A number greater
        /// than zero if this unit value is greater than obj.</returns>
        public int CompareTo(object obj)
        {
            UnitValue unitValue = (UnitValue)obj;
            if (unitValue.Unit != this.Unit)
                throw new InvalidOperationException("Cannot compare two unit values with different units.");
            return this.Value.CompareTo(unitValue.Value);
        }

        /// <summary>Determines if two values are equal.</summary>
        /// <param name="obj">The other value.</param>
        /// <returns>A value indicating whether values are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is UnitValue))
                return false;
            UnitValue unitValue = (UnitValue)obj;
            return (object.ReferenceEquals((object)unitValue.Value, (object)this.Value) || object.Equals((object)unitValue.Value, (object)this.Value)) && unitValue.Unit == this.Unit;
        }

        /// <summary>Determines whether two unit value objects are equal.</summary>
        /// <param name="left">The left unit value.</param>
        /// <param name="right">The right unit value.</param>
        /// <returns>A value indicating  whether two unit value objects are
        /// equal.</returns>
        public static bool operator ==(UnitValue left, UnitValue right)
        {
            return left.Equals((object)right);
        }

        /// <summary>
        /// Determines whether two unit value objects are not equal.
        /// </summary>
        /// <param name="left">The left unit value.</param>
        /// <param name="right">The right unit value.</param>
        /// <returns>A value indicating whether two unit value objects are not
        /// equal.</returns>
        public static bool operator !=(UnitValue left, UnitValue right)
        {
            return !left.Equals((object)right);
        }

        /// <summary>
        /// Determines whether the left value is smaller than the right.
        /// </summary>
        /// <param name="left">The left unit value.</param>
        /// <param name="right">The right unit value.</param>
        /// <returns>A value indicating whether the left value is smaller than
        /// the right.</returns>
        public static bool operator <(UnitValue left, UnitValue right)
        {
            return left.CompareTo((object)right) < 0;
        }

        /// <summary>
        /// Determines whether the left value is larger than the right.
        /// </summary>
        /// <param name="left">The left unit value.</param>
        /// <param name="right">The right unit value.</param>
        /// <returns>A value indicating whether the left value is larger than
        /// the right.</returns>
        public static bool operator >(UnitValue left, UnitValue right)
        {
            return left.CompareTo((object)right) > 0;
        }

        /// <summary>Returns the hash code of the unit value object.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (int)(this.Value.GetHashCode() + this.Unit);
        }
    }
}
