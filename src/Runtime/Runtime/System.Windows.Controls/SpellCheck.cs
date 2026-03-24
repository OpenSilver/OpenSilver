/*====================================================================================
* OpenSilver — WPF compatibility: <c>SpellCheck.IsEnabled</c> attached property.
\*====================================================================================*/

using System.ComponentModel;
using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Provides access to spell-checking attached properties (WPF compatibility).
/// </summary>
public static class SpellCheck
{
    /// <summary>
    /// Identifies the SpellCheck.IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(SpellCheck),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsEnabledChanged));

    /// <summary>
    /// Gets the value of the IsEnabled attached property for the specified element.
    /// </summary>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetIsEnabled(DependencyObject element) =>
        element != null && (bool)element.GetValue(IsEnabledProperty);

    /// <summary>
    /// Sets the value of the IsEnabled attached property for the specified element.
    /// </summary>
    public static void SetIsEnabled(DependencyObject element, bool value) =>
        element?.SetValueInternal(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        bool enabled = (bool)e.NewValue;
        switch (d)
        {
            case TextBox tb:
                tb.IsSpellCheckEnabled = enabled;
                break;
            case RichTextBox rtb:
                rtb.IsSpellCheckEnabled = enabled;
                break;
        }
    }
}
