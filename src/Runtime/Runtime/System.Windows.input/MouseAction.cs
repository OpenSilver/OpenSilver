
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
/// Specifies constants that define actions performed by the mouse.
/// </summary>
[TypeConverter(typeof(MouseActionConverter))]
public enum MouseAction : byte
{
    /// <summary>
    /// No action.
    /// </summary>
    None,

    /// <summary>
    /// A left mouse button click.
    /// </summary>
    LeftClick,

    /// <summary>
    /// A right mouse button click.
    /// </summary>
    RightClick,

    /// <summary>
    /// A middle mouse button click.
    /// </summary>
    MiddleClick,

    /// <summary>
    /// A mouse wheel rotation.
    /// </summary>
    WheelClick,

    /// <summary>
    /// A left mouse button double-click.
    /// </summary>
    LeftDoubleClick,

    /// <summary>
    /// A right mouse button double-click.
    /// </summary>
    RightDoubleClick,

    /// <summary>
    /// A middle mouse button double-click.
    /// </summary>
    MiddleDoubleClick,
}