
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

namespace OpenSilver.Internal;

internal sealed class INTERNAL_PropertyStorage
{
    internal INTERNAL_PropertyStorage()
    {
        LocalValue = DependencyProperty.UnsetValue;
        LocalStyleValue = DependencyProperty.UnsetValue;
        ThemeStyleValue = DependencyProperty.UnsetValue;
        InheritedValue = DependencyProperty.UnsetValue;
    }

    internal static INTERNAL_PropertyStorage CreateDefaultValueEntry(object value) =>
        new INTERNAL_PropertyStorage { Entry = new EffectiveValueEntry(value) };

    internal EffectiveValueEntry Entry { get; set; }

    /// <summary>
    /// This value should always be false, except when the value as seen in the Visual tree does not match the value in C#. It happens during animations that use Velocity.
    /// If it is set to true, it means that the next time the property's value value is set, we will have to force it to go through the Property changed callbacks so we can be sure the visuals fit the C# value.
    /// Note: In Silverlight, these animations update the C# value throughout the animation, but we do not do that to reduce the impact on performance.
    /// </summary>
    internal bool INTERNAL_IsVisualValueDirty;

    internal object LocalValue { get; set; }

    internal object LocalStyleValue { get; set; }

    internal object ThemeStyleValue { get; set; }

    internal object InheritedValue { get; set; }
}
