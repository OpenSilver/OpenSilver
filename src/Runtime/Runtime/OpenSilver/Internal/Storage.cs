
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

using System.Windows;
using OpenSilver.Internal.Media.Animation;

namespace OpenSilver.Internal;

internal sealed class Storage
{
    private Storage(DependencyProperty dp, bool inheritable, object value)
    {
        LocalValue = DependencyProperty.UnsetValue;
        LocalStyleValue = DependencyProperty.UnsetValue;
        ThemeStyleValue = DependencyProperty.UnsetValue;
        InheritedValue = DependencyProperty.UnsetValue;
        PropertyIndex = dp.GlobalIndex;
        Inheritable = inheritable;
        Entry = new EffectiveValueEntry(value);
    }

    internal static Storage CreateDefaultValueEntry(DependencyProperty dp, bool inheritable, object value) => new(dp, inheritable, value);

    internal int PropertyIndex { get; }

    internal bool Inheritable { get; }

    internal EffectiveValueEntry Entry { get; set; }

    internal TimelineClock.ClockHandle ClockHandle { get; set; }

    internal object LocalValue { get; set; }

    internal object LocalStyleValue { get; set; }

    internal object ThemeStyleValue { get; set; }

    internal object InheritedValue { get; set; }
}
