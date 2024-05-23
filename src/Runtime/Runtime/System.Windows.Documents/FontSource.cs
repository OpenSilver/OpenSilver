
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

using System.IO;
using System.Windows.Media;

namespace System.Windows.Documents;

/// <summary>
/// Represents one or more fonts created from a stream.
/// </summary>
[OpenSilver.NotImplemented]
public class FontSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontSource"/> class.
    /// </summary>
    /// <param name="stream">
    /// The stream that contains the font source.
    /// </param>
    public FontSource(Stream stream) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontSource"/> class.
    /// </summary>
    /// <param name="glyphtypeface">
    /// The <see cref="GlyphTypeface"/> object that contains the font file.
    /// </param>
    public FontSource(GlyphTypeface glyphtypeface) { }
}
