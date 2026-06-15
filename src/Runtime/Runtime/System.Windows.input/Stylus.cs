
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

using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Provides access to general information about a tablet pen.
/// </summary>
[OpenSilver.NotImplemented]
public static class Stylus
{
    /// <summary>
    /// Gets the stylus that represents the stylus currently in use.
    /// </summary>
    /// <returns>
    /// The <see cref="StylusDevice"/> that represents the stylus currently in use.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static StylusDevice CurrentStylusDevice => null;

    /// <summary>
    /// Identifies the Stylus.IsFlicksEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty IsFlicksEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsFlicksEnabled",
            typeof(bool),
            typeof(Stylus),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets the value of the Stylus.IsFlicksEnabled attached property on the specified element.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> on which to enable flicks.
    /// </param>
    /// <param name="enabled">
    /// true to enable flicks; false to disable flicks.
    /// </param>
    public static void SetIsFlicksEnabled(DependencyObject element, bool enabled)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(IsFlicksEnabledProperty, enabled);
    }

    /// <summary>
    /// Gets the value of the Stylus.IsFlicksEnabled attached property on the specified element.
    /// </summary>
    /// <param name="element">
    /// A <see cref="UIElement"/> on which to determine whether flicks are enabled.
    /// </param>
    /// <returns>
    /// true if the specified element has flicks enabled; otherwise, false.
    /// </returns>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetIsFlicksEnabled(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(IsFlicksEnabledProperty);
    }
}
