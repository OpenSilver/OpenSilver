
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

using System.ComponentModel;
using System.Threading.Tasks;
using OpenSilver.Internal.Media;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a family of related fonts.
    /// </summary>
    public class FontFamily
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamily"/> class from
        /// the specified font family string.
        /// </summary>
        /// <param name="familyName">
        /// The family name or names that comprise the new <see cref="FontFamily"/>.
        /// </param>
        public FontFamily(string familyName)
        {
            Source = familyName;
        }

        internal static FontFamily Default { get; } = new FontFamily("Portable User Interface");

        /// <summary>
        /// Gets the font family name that is used to construct the <see cref="FontFamily"/> object.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets a value that indicates whether the current font family object and the specified
        /// font family object are the same.
        /// </summary>
        /// <param name="o">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// true if o is equal to the current <see cref="FontFamily"/> object; otherwise,
        /// false. If o is not a <see cref="FontFamily"/> object, false is returned.
        /// </returns>
        public override bool Equals(object o)
        {
            if (o is FontFamily ff)
            {
                return ReferenceEquals(this, ff) ||
                    (Source is not null && ff.Source is not null &&
                     string.Equals(Source, ff.Source, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for <see cref="FontFamily"/>.
        /// </summary>
        /// <returns>
        /// An integer hash value.
        /// </returns>
        public override int GetHashCode() => Source?.ToLower().GetHashCode() ?? base.GetHashCode();

        /// <summary>
        /// Returns a string representation of this <see cref="FontFamily"/>.
        /// </summary>
        /// <returns>
        /// The input font family string.
        /// </returns>
        public override string ToString() => Source ?? string.Empty;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Task<bool> LoadFontAsync(FontFamily font)
        {
            if (font is null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            return LoadFontAsync(font.Source);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Task<bool> LoadFontAsync(string fontSource)
        {
            if (string.IsNullOrEmpty(fontSource))
            {
                return Task.FromResult(true);
            }

            return FontFace.GetFontFace(fontSource, null).LoadAsync();
        }

        internal FontFace GetFontFace(UIElement relativeTo) => _face ??= FontFace.GetFontFace(Source, relativeTo);

        private FontFace _face;
    }
}
