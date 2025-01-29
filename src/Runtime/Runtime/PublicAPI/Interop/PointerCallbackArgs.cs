
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

using System.Windows.Input;

namespace CSHTML5.Internal;

internal class PointerCallbackArgs
{
    public bool IsTouchEvent { get; set; }
    public double PageX { get; set; }
    public double PageY { get; set; }
    public ModifierKeys KeyModifiers { get; set; }
    public object UIEventArg { get; set; }
}
