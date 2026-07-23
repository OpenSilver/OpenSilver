
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

namespace System.Windows.Controls;

/// <summary>
/// Specifies the case of characters typed manually into a <see cref="TextBox"/> control.
/// </summary>
public enum CharacterCasing
{
    /// <summary>
    /// Characters typed into a <see cref="TextBox"/> are not converted.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Characters typed into a <see cref="TextBox"/> are converted to lowercase.
    /// </summary>
    Lower = 1,

    /// <summary>
    /// Characters typed into a <see cref="TextBox"/> are converted to uppercase.
    /// </summary>
    Upper = 2,
}
