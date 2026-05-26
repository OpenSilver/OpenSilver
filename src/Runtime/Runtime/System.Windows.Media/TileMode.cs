
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
/// Describes how a <see cref="TileBrush"/> paints tiles onto an output area.
/// </summary>
public enum TileMode
{
    /// <summary>
    /// The base tile is drawn but not repeated. The remaining area is transparent.
    /// </summary>
    None = 0,

    /// <summary>
    /// The same as <see cref="Tile"/> except that alternate columns of tiles are flipped horizontally.
    /// The base tile itself is not flipped.
    /// </summary>
    [OpenSilver.NotImplemented]
    FlipX = 1,

    /// <summary>
    /// The same as <see cref="Tile"/> except that alternate rows of tiles are flipped vertically.
    /// The base tile itself is not flipped.
    /// </summary>
    [OpenSilver.NotImplemented]
    FlipY = 2,

    /// <summary>
    /// The combination of <see cref="FlipX"/> and <see cref="FlipY"/>. The base tile itself is not flipped.
    /// </summary>
    [OpenSilver.NotImplemented]
    FlipXY = 3,

    /// <summary>
    /// The base tile is drawn and the remaining area is filled by repeating the base tile. The right edge of 
    /// one tile meets the left edge of the next, and similarly for the bottom and top edges.
    /// </summary>
    Tile = 4,
}
