
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
    public partial struct FontStyle : IFormattable
    {
        private readonly int _style;

        internal FontStyle(int style)
        {
            _style = style;
        }

        public override bool Equals(object o)
        {
            if (o is FontStyle)
            {
                FontStyle fs = (FontStyle)o;
                return this == fs;
            }
            return false;
        }

        public bool Equals(FontStyle fontStyle)
        {
            return this == fontStyle;
        }

        public override int GetHashCode()
        {
            return this._style;
        }

        public static bool operator ==(FontStyle left, FontStyle right)
        {
            return left._style == right._style;
        }

        public static bool operator !=(FontStyle left, FontStyle right)
        {
            return left._style != right._style;
        }

        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ConvertToString(format, formatProvider);
        }

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