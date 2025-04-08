
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace OpenSilver.Internal.Controls.Primitives;

internal static class PopupService
{
    /// <summary>
    /// Place the Popup relative to this point 
    /// </summary>
    internal static Point MousePosition { get; private set; }

    internal static void TrackMousePosition(Window owner)
    {
        Debug.Assert(owner is not null);
        owner.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
    }

    internal static void OnMouseEvent(MouseEventArgs e) => MousePosition = e.GetPosition(null);

    private static void OnMouseMove(object sender, MouseEventArgs e) => OnMouseEvent(e);
}
