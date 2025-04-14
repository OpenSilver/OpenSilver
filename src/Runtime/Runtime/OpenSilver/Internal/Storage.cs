
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
using OpenSilver.Internal.Media.Animation;

namespace OpenSilver.Internal;

internal sealed class Storage
{
    internal Storage(DependencyProperty dp, bool inheritable, object value)
    {
        LocalValue = DependencyProperty.UnsetValue;
        LocalStyleValue = DependencyProperty.UnsetValue;
        ThemeStyleValue = DependencyProperty.UnsetValue;
        InheritedValue = DependencyProperty.UnsetValue;
        PropertyIndex = dp.GlobalIndex;
        Inheritable = inheritable;
        Entry = new EffectiveValueEntry(value);
    }

    internal Storage(int propertyIndex)
    {
        Debug.Assert(DependencyProperty.RegisteredPropertyList[propertyIndex] is null);

        LocalValue = DependencyProperty.UnsetValue;
        PropertyIndex = propertyIndex;
        Inheritable = false;
    }

    internal int PropertyIndex { get; }

    internal bool Inheritable { get; }

    internal EffectiveValueEntry Entry { get; set; }

    internal TimelineClock Clock { get; set; }

    internal object LocalValue { get; set; }

    internal object LocalStyleValue { get; set; }

    internal object ThemeStyleValue { get; set; }

    internal object InheritedValue { get; set; }
}
