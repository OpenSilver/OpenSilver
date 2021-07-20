

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
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows
{
    [TypeConverter(typeof(FontStyleConverter))]
    public partial struct FontStyle : IFormattable
    {
        private readonly int Style;

        internal FontStyle(int style)
        {
            Style = style;
        }

        public override bool Equals(object obj)
        {
            return obj is FontStyle style && Style == style.Style;
        }

        public bool Equals(FontStyle fontStyle)
        {
            return this == fontStyle;
        }

        public override int GetHashCode()
        {
            return -1755968282 + Style.GetHashCode();
        }

        public static bool operator ==(FontStyle left, FontStyle right)
        {
            return left.Style == right.Style;
        }

        public static bool operator !=(FontStyle left, FontStyle right)
        {
            return left.Style != right.Style;
        }

        public override string ToString()
        {
            switch (Style)
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Oblique";
                case 2:
                    return "Italic";
                default:
                    return string.Empty; //should not be possible
            }
        }

        internal int GetStyleForInternalConstruction()
        {
            Debug.Assert(0 <= Style && Style <= 2);
            return Style;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Style == 0)
                return "Normal";
            if (Style == 1)
                return "Oblique";
            Debug.Assert(Style == 2);
            return "Italic";
        }
    }
}
#endif