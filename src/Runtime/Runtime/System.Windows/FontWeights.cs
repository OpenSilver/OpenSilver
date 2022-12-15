
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
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// Provides a set of predefined font weights as static property values.
    /// </summary>
    public static class FontWeights
    {
        /// <summary>
        /// Specifies a "Black" font weight.
        /// </summary>
        public static FontWeight Black => new FontWeight((int)FontWeightsCode.Black);

        /// <summary>
        /// Specifies a "Bold" font weight.
        /// </summary>
        public static FontWeight Bold => new FontWeight((int)FontWeightsCode.Bold);

        /// <summary>
        /// Specifies a "Demi-bold" font weight.
        /// </summary>
        public static FontWeight DemiBold => new FontWeight((int)FontWeightsCode.DemiBold);

        /// <summary>
        /// Specifies an "ExtraBlack" font weight.
        /// </summary>
        public static FontWeight ExtraBlack => new FontWeight((int)FontWeightsCode.ExtraBlack);

        /// <summary>
        /// Specifies an "ExtraBold" font weight.
        /// </summary>
        public static FontWeight ExtraBold => new FontWeight((int)FontWeightsCode.ExtraBold);

        /// <summary>
        /// Specifies an "ExtraLight" font weight.
        /// </summary>
        public static FontWeight ExtraLight => new FontWeight((int)FontWeightsCode.ExtraLight);

        /// <summary>
        /// Specifies a "Heavy" font weight.
        /// </summary>
        public static FontWeight Heavy => new FontWeight((int)FontWeightsCode.Heavy);

        /// <summary>
        /// Specifies a "Light" font weight.
        /// </summary>
        public static FontWeight Light => new FontWeight((int)FontWeightsCode.Light);

        /// <summary>
        /// Specifies a "Medium" font weight.
        /// </summary>
        public static FontWeight Medium => new FontWeight((int)FontWeightsCode.Medium);

        /// <summary>
        /// Specifies a "Normal" font weight.
        /// </summary>
        public static FontWeight Normal => new FontWeight((int)FontWeightsCode.Normal);

        /// <summary>
        /// Specifies a "Regular" font weight.
        /// </summary>
        public static FontWeight Regular => new FontWeight((int)FontWeightsCode.Regular);

        /// <summary>
        /// Specifies a "SemiBold" font weight.
        /// </summary>
        public static FontWeight SemiBold => new FontWeight((int)FontWeightsCode.SemiBold);

        /// <summary>
        /// Specifies a "SemiLight" font weight.
        /// </summary>
        public static FontWeight SemiLight => new FontWeight((int)FontWeightsCode.SemiLight);

        /// <summary>
        /// Specifies a "Thin" font weight.
        /// </summary>
        public static FontWeight Thin => new FontWeight((int)FontWeightsCode.Thin);

        /// <summary>
        /// Specifies an "Ultra-black" font weight.
        /// </summary>
        public static FontWeight UltraBlack => new FontWeight((int)FontWeightsCode.UltraBlack);

        /// <summary>
        /// Specifies an "Ultra-bold" font weight.
        /// </summary>
        public static FontWeight UltraBold => new FontWeight((int)FontWeightsCode.UltraBold);

        /// <summary>
        /// Specifies an "Ultra-light" font weight.
        /// </summary>
        public static FontWeight UltraLight => new FontWeight((int)FontWeightsCode.UltraLight);

        internal static bool FontWeightStringToKnownWeight(string s, IFormatProvider provider, ref FontWeight fontWeight)
        {
            switch (s.Length)
            {
                case 4:
                    if (s.Equals("Bold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Bold;
                        return true;
                    }
                    if (s.Equals("Thin", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Thin;
                        return true;
                    }
                    break;

                case 5:
                    if (s.Equals("Black", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Black;
                        return true;
                    }
                    if (s.Equals("Light", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Light;
                        return true;
                    }
                    if (s.Equals("Heavy", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Heavy;
                        return true;
                    }
                    break;

                case 6:
                    if (s.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Normal;
                        return true;
                    }
                    if (s.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Medium;
                        return true;
                    }
                    break;

                case 7:
                    if (s.Equals("Regular", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = Regular;
                        return true;
                    }
                    break;

                case 8:
                    if (s.Equals("SemiBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = SemiBold;
                        return true;
                    }
                    if (s.Equals("DemiBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = DemiBold;
                        return true;
                    }
                    break;

                case 9:
                    if (s.Equals("ExtraBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = ExtraBold;
                        return true;
                    }
                    if (s.Equals("UltraBold", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = UltraBold;
                        return true;
                    }
                    break;

                case 10:
                    if (s.Equals("ExtraLight", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = ExtraLight;
                        return true;
                    }
                    if (s.Equals("UltraLight", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = UltraLight;
                        return true;
                    }
                    if (s.Equals("ExtraBlack", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = ExtraBlack;
                        return true;
                    }
                    if (s.Equals("UltraBlack", StringComparison.OrdinalIgnoreCase))
                    {
                        fontWeight = UltraBlack;
                        return true;
                    }
                    break;
            }
            if (int.TryParse(s, NumberStyles.Integer, provider, out int weightValue))
            {
                fontWeight = FontWeight.FromOpenTypeWeight(weightValue);
                return true;
            }
            return false;
        }

        internal static bool FontWeightToString(int weight, out string convertedValue)
        {
            switch (weight)
            {
                case 100:
                    convertedValue = "Thin";
                    return true;
                case 200:
                    convertedValue = "ExtraLight";
                    return true;
                case 300:
                    convertedValue = "Light";
                    return true;
                case 400:
                    convertedValue = "Normal";
                    return true;
                case 500:
                    convertedValue = "Medium";
                    return true;
                case 600:
                    convertedValue = "SemiBold";
                    return true;
                case 700:
                    convertedValue = "Bold";
                    return true;
                case 800:
                    convertedValue = "ExtraBold";
                    return true;
                case 900:
                    convertedValue = "Black";
                    return true;
                case 950:
                    convertedValue = "ExtraBlack";
                    return true;
            }
            convertedValue = null;
            return false;
        }

        //
        // IMPORTANT: if you add or remove entries in this Enum, you must update
        // accordingly the file "ConvertingStringToValue.cs" in the Compiler project.
        //
        private enum FontWeightsCode : int
        {
            Thin = 100,
            ExtraLight = 200,
            UltraLight = 200,
            Light = 300,
            SemiLight = 350,
            Normal = 400,
            Regular = 400,
            Medium = 500,
            DemiBold = 600,
            SemiBold = 600,
            Bold = 700,
            ExtraBold = 800,
            UltraBold = 800,
            Black = 900,
            Heavy = 900,
            ExtraBlack = 950,
            UltraBlack = 950,
        }
    }
}