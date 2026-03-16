
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
/// Describes the appearance of a list item's bullet style.
/// </summary>
public enum TextMarkerStyle
{
    /// <summary>
    /// No marker.
    /// </summary>
    None = 0,
    /// <summary>
    /// A solid disc circle.
    /// </summary>
    Disc = 1,
    /// <summary>
    /// A hollow disc circle.
    /// </summary>
    Circle = 2,
    /// <summary>
    /// A hollow square shape.
    /// </summary>
    Square = 3,
    /// <summary>
    /// A solid square box.
    /// </summary>
    Box = 4,
    /// <summary>
    /// A lowercase Roman numeral starting with the numeral i. For example, i, ii, iii, and iv.
    /// The numeral is automatically incremented for each item added to the list.
    /// </summary>
    LowerRoman = 5,
    /// <summary>
    /// An uppercase Roman numeral starting with the numeral I. For example, I, II, III, and IV.
    /// The numeric value is automatically incremented for each item added to the list.
    /// </summary>
    UpperRoman = 6,
    /// <summary>
    /// A lowercase ASCII character starting with the letter a. For example, a, b, and c.
    /// The character value is automatically incremented for each item added to the list.
    /// </summary>
    LowerLatin = 7,
    /// <summary>
    /// An uppercase ASCII character starting with the letter A. For example, A, B, and C.
    /// The character value is automatically incremented for each item added to the list.
    /// </summary>
    UpperLatin = 8,
    /// <summary>
    /// A decimal starting with the number one. For example, 1, 2, and 3. The decimal value is 
    /// automatically incremented for each item added to the list.
    /// </summary>
    Decimal = 9
}