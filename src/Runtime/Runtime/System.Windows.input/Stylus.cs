/*====================================================================================
* OpenSilver — WPF compatibility attached properties for stylus markup.
\*====================================================================================*/

using System.Windows;
using OpenSilver.Internal;

namespace System.Windows.Input;

/// <summary>
/// Provides stylus-related attached properties for WPF markup compatibility.
/// </summary>
public static class Stylus
{
    /// <summary>
    /// Identifies the IsFlicksEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty IsFlicksEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsFlicksEnabled",
            typeof(bool),
            typeof(Stylus),
            new PropertyMetadata(BooleanBoxes.TrueBox));

    public static void SetIsFlicksEnabled(DependencyObject element, bool value) =>
        element?.SetValueInternal(IsFlicksEnabledProperty, value);

    public static bool GetIsFlicksEnabled(DependencyObject element) =>
        element != null && (bool)element.GetValue(IsFlicksEnabledProperty);
}
