
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CSHTML5.Internal;
using System.Windows.Markup;
using DotNetForHtml5.Core;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(FontFamilyConverter))]
#endif
    /// <summary>
    /// Represents a family of related fonts.
    /// </summary>
    [SupportsDirectContentViaTypeFromStringConverters]
    public class FontFamily
    {
        static FontFamily()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontFamily), INTERNAL_ConvertFromString);
        }


        // Parameters:
        //   familyName:
        //     The family name of the font to represent. This can include a hashed suffix.
        /// <summary>
        /// Initializes a new instance of the FontFamily class from the specified font
        /// family string.
        /// </summary>
        /// <param name="familyName">The family name of the font to represent. This can include a hashed suffix.</param>
        public FontFamily(string familyName) { _source = familyName; }

        string _source;
        /// <summary>
        /// Gets the font family name that is used to construct the FontFamily object.
        /// </summary>
        public string Source { get { return _source; } }


        internal string INTERNAL_ToHtmlString()
        {
            return Source;
        }

        internal static object INTERNAL_ConvertFromString(string fontCode)
        {
            return new FontFamily(fontCode);
        }

        public override string ToString()
        {
            return _source;
        }
    }
}
