
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

namespace System.Windows
{
    /// <summary>
    /// Provides a set of predefined font stretches as static property values.
    /// </summary>
    public static class FontStretches
	{
        /// <summary>
        /// Specifies an ultra-condensed font stretch.
        /// </summary>
        public static FontStretch UltraCondensed => new FontStretch(1);

        /// <summary>
        /// Specifies an extra-condensed font stretch.
        /// </summary>
        public static FontStretch ExtraCondensed => new FontStretch(2);

        /// <summary>
        /// Specifies a condensed font stretch.
        /// </summary>
        public static FontStretch Condensed => new FontStretch(3);

        /// <summary>
        /// Specifies a semi-condensed font stretch.
        /// </summary>
        public static FontStretch SemiCondensed => new FontStretch(4);

        /// <summary>
        /// Specifies a normal font stretch.
        /// </summary>
        public static FontStretch Normal => new FontStretch(5);

        /// <summary>
        /// Specifies a semi-expanded font stretch.
        /// </summary>
        public static FontStretch SemiExpanded => new FontStretch(6);

        /// <summary>
        /// Specifies an expanded font stretch.
        /// </summary>
        public static FontStretch Expanded => new FontStretch(7);

        /// <summary>
        /// Specifies an extra-expanded font stretch.
        /// </summary>
        public static FontStretch ExtraExpanded => new FontStretch(8);

        /// <summary>
        /// Specifies an ultra-expanded font stretch.
        /// </summary>
        public static FontStretch UltraExpanded => new FontStretch(9);

        internal static bool FontStretchStringToKnownStretch(string s, IFormatProvider provider, ref FontStretch fontStretch)
        {
            switch (s.Length)
            {
                case 6:
                    if (s.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = Normal;
                        return true;
                    }
                    break;

                case 8:
                    if (s.Equals("Expanded", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = Expanded;
                        return true;
                    }
                    break;

                case 9:
                    if (s.Equals("Condensed", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = Condensed;
                        return true;
                    }
                    break;

                case 12:
                    if (s.Equals("SemiExpanded", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = SemiExpanded;
                        return true;
                    }
                    break;

                case 13:
                    if (s.Equals("SemiCondensed", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = SemiCondensed;
                        return true;
                    }
                    if (s.Equals("ExtraExpanded", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = ExtraExpanded;
                        return true;
                    }
                    if (s.Equals("UltraExpanded", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = UltraExpanded;
                        return true;
                    }
                    break;

                case 14:
                    if (s.Equals("UltraCondensed", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = UltraCondensed;
                        return true;
                    }
                    if (s.Equals("ExtraCondensed", StringComparison.OrdinalIgnoreCase))
                    {
                        fontStretch = ExtraCondensed;
                        return true;
                    }
                    break;
            }
            if (int.TryParse(s, NumberStyles.Integer, provider, out int stretchValue))
            {
                fontStretch = FontStretch.FromOpenTypeStretch(stretchValue);
                return true;
            }
            return false;
        }

        internal static bool FontStretchToString(int stretch, out string convertedValue)
        {
            switch (stretch)
            {
                case 1:
                    convertedValue = "UltraCondensed";
                    return true;
                case 2:
                    convertedValue = "ExtraCondensed";
                    return true;
                case 3:
                    convertedValue = "Condensed";
                    return true;
                case 4:
                    convertedValue = "SemiCondensed";
                    return true;
                case 5:
                    convertedValue = "Normal";
                    return true;
                case 6:
                    convertedValue = "SemiExpanded";
                    return true;
                case 7:
                    convertedValue = "Expanded";
                    return true;
                case 8:
                    convertedValue = "ExtraExpanded";
                    return true;
                case 9:
                    convertedValue = "UltraExpanded";
                    return true;
            }

            convertedValue = null;
            return false;
        }
    }
}
