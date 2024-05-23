
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

namespace System.Windows.Media;

/// <summary>
/// Specifies a physical font face that corresponds to a font file on the disk.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class GlyphTypeface
{
    internal GlyphTypeface() { }

    /// <summary>
    /// Gets or sets the font file name for the <see cref="GlyphTypeface"/> object.
    /// </summary>
    /// <returns>
    /// The font file name for the <see cref="GlyphTypeface"/> object.
    /// </returns>
    public string FontFileName { get; }

    /// <summary>
    /// Gets the font face version interpreted from the font's 'NAME' table.
    /// </summary>
    /// <returns>
    /// A <see cref="double"/> value that represents the version.
    /// </returns>
    public double Version { get; }
}
