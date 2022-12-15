
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

#if MIGRATION

using System.Diagnostics;

namespace System.Windows
{
    /// <summary>
    /// Represents the style of a font face (for instance, as normal or italic).
    /// </summary>
    public struct FontStyle : IFormattable
    {
        private readonly int _style;

        internal FontStyle(int style)
        {
            _style = style;
        }

        /// <summary>
        /// Compares an object with the current <see cref="FontStyle"/> instance for equality.
        /// </summary>
        /// <param name="obj">
        /// An object that represents the <see cref="FontStyle"/> to compare for equality.
        /// </param>
        /// <returns>
        /// true to show the two instances are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
            => obj is FontStyle fontStyle && this == fontStyle;

        /// <summary>
        /// Compares this <see cref="FontStyle"/> structure to another <see cref="FontStyle"/>
        /// structure for equality.
        /// </summary>
        /// <param name="fontStyle">
        /// An instance of <see cref="FontStyle"/> to compare for equality.
        /// </param>
        /// <returns>
        /// true if the two instances of <see cref="FontStyle"/> are equal; otherwise, false.
        /// </returns>
        public bool Equals(FontStyle fontStyle) => this == fontStyle;

        /// <summary>
        /// Retrieves the hash code for this object.
        /// </summary>
        /// <returns>
        /// An integer hash value.
        /// </returns>
        public override int GetHashCode() => _style;

        /// <summary>
        /// Compares two instances of <see cref="FontStyle"/> for equality.
        /// </summary>
        /// <param name="left">
        /// The first instance of <see cref="FontStyle"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second instance of <see cref="FontStyle"/> to compare.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="FontStyle"/> objects are equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(FontStyle left, FontStyle right) => left._style == right._style;

        /// <summary>
        /// Evaluates two instances of <see cref="FontStyle"/> to determine inequality.
        /// </summary>
        /// <param name="left">
        /// The first instance of <see cref="FontStyle"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second instance of <see cref="FontStyle"/> to compare.
        /// </param>
        /// <returns>
        /// false to show left is equal to right; otherwise, true.
        /// </returns>
        public static bool operator !=(FontStyle left, FontStyle right) => !(left == right);

        /// <summary>
        /// Creates a string that represents the current <see cref="FontStyle"/> object.
        /// </summary>
        /// <returns>
        /// A string that represents the value of the <see cref="FontStyle"/> object.
        /// </returns>
        public override string ToString() => ConvertToString(null, null);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
            => ConvertToString(format, formatProvider);

        /// <summary>
        /// Creates a string representation of this object based on the format string 
        /// and IFormatProvider passed in.  
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        private string ConvertToString(string format, IFormatProvider provider)
        {
            if (_style == 0)
                return "Normal";
            if (_style == 1)
                return "Oblique";
            Debug.Assert(_style == 2);
            return "Italic";
        }
    }
}

#endif