
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

using System;
using System.Windows;

namespace OpenSilver.Internal;

internal static class VisibilityBoxes
{
    internal static readonly object VisibleBox = Visibility.Visible; 
    internal static readonly object CollapsedBox = Visibility.Collapsed;

    internal static object Box(Visibility value) =>
        value switch
        {
            Visibility.Collapsed => CollapsedBox,
            _ => VisibleBox,
        };
}

internal static class BooleanBoxes
{
    internal static readonly object TrueBox = true;
    internal static readonly object FalseBox = false;

    internal static object Box(bool value) => value ? TrueBox : FalseBox;

    internal static object Box(bool? value) =>
        value switch
        {
            true => TrueBox,
            false => FalseBox,
            null => null,
        };
}
