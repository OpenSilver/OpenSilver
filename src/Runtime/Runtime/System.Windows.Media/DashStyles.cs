
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
/// Implements a set of predefined <see cref="DashStyle"/> objects.
/// </summary>
[OpenSilver.NotImplemented]
public static class DashStyles
{
    /// <summary>
    /// Gets a <see cref="DashStyle"/> with a <see cref="DashStyle.Dashes"/> property equal to 2, 2.
    /// </summary>
    /// <returns>
    /// A dash sequence of 2,2, which describes a sequence composed of a dash that is twice as long as the 
    /// pen <see cref="Pen.Thickness"/> followed by a space that is twice as long as the <see cref="Pen.Thickness"/>.
    /// </returns>
    public static DashStyle Dash => new([2, 2], 1);

    /// <summary>
    /// Gets a <see cref="DashStyle"/> with a <see cref="DashStyle.Dashes"/> property equal to 2, 2, 0, 2.
    /// </summary>
    /// <returns>
    /// A dash sequence of 2, 2, 0, 2.
    /// </returns>
    public static DashStyle DashDot => new([2, 2, 0, 2], 1);

    /// <summary>
    /// Gets a <see cref="DashStyle"/> with a <see cref="DashStyle.Dashes"/> property equal to 2, 2, 0, 2, 0, 2.
    /// </summary>
    /// <returns>
    /// A dash sequence of 2, 2, 0, 2, 0, 2.
    /// </returns>
    public static DashStyle DashDotDot => new([2, 2, 0, 2, 0, 2], 1);

    /// <summary>
    /// Gets a <see cref="DashStyle"/> with a <see cref="DashStyle.Dashes"/> property equal to 0, 2.
    /// </summary>
    /// <returns>
    /// A dash sequence of 0, 2.
    /// </returns>
    public static DashStyle Dot => new([0, 2], 0);

    /// <summary>
    /// Gets a <see cref="DashStyle"/> with an empty <see cref="DashStyle.Dashes"/> property.
    /// </summary>
    /// <returns>
    /// A dash sequence with no dashes.
    /// </returns>
    public static DashStyle Solid => new();
}
