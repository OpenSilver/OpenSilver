﻿

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Text
#endif
{
    /// <summary>
    /// Provides a set of predefined font weights as static property values.
    /// </summary>
    public sealed partial class FontWeights
    {
        internal enum INTERNAL_FontweightsEnum : ushort
        {
            Black = 900,
            Bold = 700,
            DemiBold = 600,
            ExtraBlack = 900, //note: the value should be 950 but it is not supported in html5
            ExtraBold = 800,
            ExtraLight = 200,
            Heavy = 900,
            Light = 300,
            Medium = 500,
            Normal = 400,
            Regular = 400,
            SemiBold = 600,
            SemiLight = 300, //note: the value should be 350 (I think) but it is not supported in html5
            Thin = 100,
            UltraBlack = 900, //note: the value should be 950 but it is not supported in html5
            UltraBold = 800,
            UltraLight = 200
        }

        /// <summary>
        /// Specifies a "Black" font weight.
        /// </summary>
        public static FontWeight Black
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Black);
            }
        }
        /// <summary>
        /// Specifies a "Bold" font weight.
        /// </summary>
        public static FontWeight Bold
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Bold);
            }
        }

        /// <summary>
        /// Specifies a "Demi-bold" font weight.
        /// </summary>
        public static FontWeight DemiBold
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.DemiBold);
            }
        }

        /// <summary>
        /// Specifies an "ExtraBlack" font weight.
        /// </summary>
        public static FontWeight ExtraBlack
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.ExtraBlack);
            }
        }
        /// <summary>
        /// Specifies an "ExtraBold" font weight.
        /// </summary>
        public static FontWeight ExtraBold
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.ExtraBold);
            }
        }
        /// <summary>
        /// Specifies an "ExtraLight" font weight.
        /// </summary>
        public static FontWeight ExtraLight
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.ExtraLight);
            }
        }
        /// <summary>
        /// Specifies a "Heavy" font weight.
        /// </summary>
        public static FontWeight Heavy
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Heavy);
            }
        }
        /// <summary>
        /// Specifies a "Light" font weight.
        /// </summary>
        public static FontWeight Light
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Light);
            }
        }
        /// <summary>
        /// Specifies a "Medium" font weight.
        /// </summary>
        public static FontWeight Medium
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Medium);
            }
        }
        /// <summary>
        /// Specifies a "Normal" font weight.
        /// </summary>
        public static FontWeight Normal
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Normal);
            }
        }
        /// <summary>
        /// Specifies a "Regular" font weight.
        /// </summary>
        public static FontWeight Regular
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Regular);
            }
        }
        /// <summary>
        /// Specifies a "SemiBold" font weight.
        /// </summary>
        public static FontWeight SemiBold
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.SemiBold);
            }
        }
        /// <summary>
        /// Specifies a "SemiLight" font weight.
        /// </summary>
        public static FontWeight SemiLight
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.SemiLight);
            }
        }
        /// <summary>
        /// Specifies a "Thin" font weight.
        /// </summary>
        public static FontWeight Thin
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.Thin);
            }
        }
        /// <summary>
        /// Specifies an "Ultra-black" font weight.
        /// </summary>
        public static FontWeight UltraBlack
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.UltraBlack);
            }
        }

        /// <summary>
        /// Specifies an "Ultra-bold" font weight.
        /// </summary>
        public static FontWeight UltraBold
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.UltraBold);
            }
        }

        /// <summary>
        /// Specifies an "Ultra-light" font weight.
        /// </summary>
        public static FontWeight UltraLight
        {
            get
            {
                return FontWeight.INTERNAL_ConvertFromUshort((ushort)INTERNAL_FontweightsEnum.UltraLight);
            }
        }
    }
}