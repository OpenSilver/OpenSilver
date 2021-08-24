// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class Range<T>
    {
        #region Data
        #endregion Data

        public Range(int lowerBound, int upperBound, T value)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Value = value;
        }

        #region Public Properties

        public int Count
        {
            get
            {
                return UpperBound - LowerBound + 1;
            }
        }

        public int LowerBound
        {
            get;
            set;
        }

        public int UpperBound
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        public bool ContainsIndex(int index)
        {
            return (LowerBound <= index) && (UpperBound >= index);
        }

        public bool ContainsValue(object value)
        {
            if (this.Value == null)
            {
                return value == null;
            }
            else
            {
                return this.Value.Equals(value);
            }
        }

        public Range<T> Copy()
        {
            return new Range<T>(LowerBound, UpperBound, Value);
        }

        #endregion Public Methods
    }
}
