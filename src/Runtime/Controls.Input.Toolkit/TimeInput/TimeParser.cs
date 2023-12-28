// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

namespace System.Windows.Controls
{
    /// <summary>
    /// Converts the specified string representation of a time to its DateTime 
    /// equivalent.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class TimeParser
    {
        /// <summary>
        /// Converts the specified string representation of a time to its DateTime 
        /// equivalent and returns a value that indicates whether the conversion 
        /// succeeded.
        /// </summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>True if the parse was successful, false if it was not.</returns>
        public virtual bool TryParse(string text, CultureInfo culture, out DateTime? result)
        {
            result = null;
            return false;
        }
    }
}