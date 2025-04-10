
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

namespace System.Windows.Input;

/// <summary>
/// Specifies the set of modifier keys.
/// </summary>
[Flags]
[TypeConverter(typeof(ModifierKeysConverter))]
public enum ModifierKeys
{
    /// <summary>
    /// No modifiers are pressed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The CTRL key.
    /// </summary>
    Control = 1,

    /// <summary>
    /// The ALT key.
    /// </summary>
    Alt = 2,

    /// <summary>
    /// The SHIFT key.
    /// </summary>
    Shift = 4,

    /// <summary>
    /// The Windows logo key.
    /// </summary>
    Windows = 8,

    /// <summary>
    /// The Apple key (also known as the Open Apple key).
    /// </summary>
    Apple = Windows,
}