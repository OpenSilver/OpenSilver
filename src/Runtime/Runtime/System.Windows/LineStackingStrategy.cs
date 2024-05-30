
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

namespace System.Windows;

/// <summary>
/// Describes the mechanism by which a line box is determined for each line.
/// </summary>
public enum LineStackingStrategy
{
    /// <summary>
    /// The stack height is the smallest value that contains the extended block progression
    /// dimension of all the inline elements on that line when those elements are properly
    /// aligned. This is the default.
    /// </summary>
    MaxHeight = 0,

    /// <summary>
    /// The stack height is determined by the block element line-height property value.
    /// </summary>
    BlockLineHeight = 1,

    /// <summary>
    /// The stack height is determined by adding LineHeight to the baseline of the previous
    /// line.
    /// </summary>
    BaselineToBaseline = 2,
}
