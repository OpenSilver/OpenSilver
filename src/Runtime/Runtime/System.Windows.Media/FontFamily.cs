

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


using System.Collections.Generic;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a family of related fonts.
    /// </summary>
    [TypeConverter(typeof(FontFamilyConverter))]
    public partial class FontFamily
    {
        // Parameters:
        //   familyName:
        //     The family name of the font to represent. This can include a hashed suffix.
        /// <summary>
        /// Initializes a new instance of the FontFamily class from the specified font
        /// family string.
        /// </summary>
        /// <param name="familyName">The family name of the font to represent. This can include a hashed suffix.</param>
        public FontFamily(string familyName) { Source = familyName; }

        /// <summary>
        /// Gets the font family name that is used to construct the FontFamily object.
        /// </summary>
        public string Source { get; }

        internal string INTERNAL_ToHtmlString()
        {
            return Source;
        }

        public override string ToString()
        {
            return Source;
        }

        public override bool Equals(object obj)
        {
            return obj is FontFamily family && Source == family.Source;
        }

        public override int GetHashCode()
        {
            return 924162744 + EqualityComparer<string>.Default.GetHashCode(Source);
        }
    }
}
