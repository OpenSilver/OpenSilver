using System.Collections.Generic;
using System.Globalization;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>A range of values.</summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public struct Range<T> where T : IComparable
    {
        /// <summary>
        /// A flag that determines whether the range is empty or not.
        /// </summary>
        private bool _hasData;
        /// <summary>The maximum value in the range.</summary>
        private T _maximum;
        /// <summary>The minimum value in the range.</summary>
        private T _minimum;

        /// <summary>
        /// Gets a value indicating whether the range is empty or not.
        /// </summary>
        public bool HasData
        {
            get
            {
                return this._hasData;
            }
        }

        /// <summary>Gets the maximum value in the range.</summary>
        public T Maximum
        {
            get
            {
                if (!this.HasData)
                    throw new InvalidOperationException("Range.Maximum: Cannot Read The Maximum Of An Empty Range");
                return this._maximum;
            }
        }

        /// <summary>Gets the minimum value in the range.</summary>
        public T Minimum
        {
            get
            {
                if (!this.HasData)
                    throw new InvalidOperationException("Range.Minimum: Cannot Read The Minimum Of An Empty Range");
                return this._minimum;
            }
        }

        /// <summary>Initializes a new instance of the Range class.</summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        public Range(T minimum, T maximum)
        {
            if ((object)minimum == null)
                throw new ArgumentNullException(nameof(minimum));
            if ((object)maximum == null)
                throw new ArgumentNullException(nameof(maximum));
            this._hasData = true;
            this._minimum = minimum;
            this._maximum = maximum;
            if (ValueHelper.Compare((IComparable)minimum, (IComparable)maximum) == 1)
                throw new InvalidOperationException("Range: Maximum Value Must Be Larger Than Or Equal To Minimum Value");
        }

        /// <summary>
        /// Compare two ranges and return a value indicating whether they are
        /// equal.
        /// </summary>
        /// <param name="leftRange">Left-hand side range.</param>
        /// <param name="rightRange">Right-hand side range.</param>
        /// <returns>A value indicating whether the ranges are equal.</returns>
        public static bool operator ==(Range<T> leftRange, Range<T> rightRange)
        {
            if (!leftRange.HasData)
                return !rightRange.HasData;
            if (!rightRange.HasData)
                return !leftRange.HasData;
            T obj = leftRange.Minimum;
            int num;
            if (obj.Equals((object)rightRange.Minimum))
            {
                obj = leftRange.Maximum;
                num = obj.Equals((object)rightRange.Maximum) ? 1 : 0;
            }
            else
                num = 0;
            return num != 0;
        }

        /// <summary>
        /// Compare two ranges and return a value indicating whether they are
        /// not equal.
        /// </summary>
        /// <param name="leftRange">Left-hand side range.</param>
        /// <param name="rightRange">Right-hand side range.</param>
        /// <returns>A value indicating whether the ranges are not equal.</returns>
        public static bool operator !=(Range<T> leftRange, Range<T> rightRange)
        {
            return !(leftRange == rightRange);
        }

        /// <summary>Adds a range to the current range.</summary>
        /// <param name="range">A range to add to the current range.</param>
        /// <returns>A new range that encompasses the instance range and the
        /// range parameter.</returns>
        public Range<T> Add(Range<T> range)
        {
            if (!this.HasData)
                return range;
            if (!range.HasData)
                return this;
            return new Range<T>(ValueHelper.Compare((IComparable)this.Minimum, (IComparable)range.Minimum) == -1 ? this.Minimum : range.Minimum, ValueHelper.Compare((IComparable)this.Maximum, (IComparable)range.Maximum) == 1 ? this.Maximum : range.Maximum);
        }

        /// <summary>Compares the range to another range.</summary>
        /// <param name="range">A different range.</param>
        /// <returns>A value indicating whether the ranges are equal.</returns>
        public bool Equals(Range<T> range)
        {
            return this == range;
        }

        /// <summary>Compares the range to an object.</summary>
        /// <param name="obj">Another object.</param>
        /// <returns>A value indicating whether the other object is a range,
        /// and if so, whether that range is equal to the instance range.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this == (Range<T>)obj;
        }

        /// <summary>
        /// Returns a value indicating whether a value is within a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Whether the value is within the range.</returns>
        public bool Contains(T value)
        {
            return ValueHelper.Compare((IComparable)this.Minimum, (IComparable)value) <= 0 && ValueHelper.Compare((IComparable)value, (IComparable)this.Maximum) <= 0;
        }

        /// <summary>
        /// Returns a value indicating whether two ranges intersect.
        /// </summary>
        /// <param name="range">The range to compare against this range.</param>
        /// <returns>A value indicating whether the ranges intersect.</returns>
        public bool IntersectsWith(Range<T> range)
        {
            if (!this.HasData || !range.HasData)
                return false;
            Func<Range<T>, Range<T>, bool> func = (Func<Range<T>, Range<T>, bool>)((leftRange, rightRange) => ValueHelper.Compare((IComparable)rightRange.Minimum, (IComparable)leftRange.Maximum) <= 0 && ValueHelper.Compare((IComparable)rightRange.Minimum, (IComparable)leftRange.Minimum) >= 0 || ValueHelper.Compare((IComparable)leftRange.Minimum, (IComparable)rightRange.Maximum) <= 0 && ValueHelper.Compare((IComparable)leftRange.Minimum, (IComparable)rightRange.Minimum) >= 0);
            return func(this, range) || func(range, this);
        }

        /// <summary>Computes a hash code value.</summary>
        /// <returns>A hash code value.</returns>
        public override int GetHashCode()
        {
            if (!this.HasData)
                return 0;
            return EqualityComparer<T>.Default.GetHashCode(this.Minimum) + EqualityComparer<T>.Default.GetHashCode(this.Maximum);
        }

        /// <summary>Returns the string representation of the range.</summary>
        /// <returns>The string representation of the range.</returns>
        public override string ToString()
        {
            if (!this.HasData)
                return "{0}, {1}";
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0}, {1}", new object[2]
            {
        (object) this.Minimum,
        (object) this.Maximum
            });
        }
    }
}
