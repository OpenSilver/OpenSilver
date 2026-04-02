
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

namespace System.Windows.Controls;

/// <summary>
/// Provides real-time spell-checking functionality to text-editing controls, such as <see cref="TextBox"/>
/// and <see cref="RichTextBox"/>.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class SpellCheck
{
    private readonly DependencyObject _owner;

    internal SpellCheck(TextBox owner)
    {
        _owner = owner;
    }

    internal SpellCheck(RichTextBox owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Identifies the SpellCheck.IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            nameof(IsEnabled),
            typeof(bool),
            typeof(SpellCheck),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsEnabledChanged));

    /// <summary>
    /// Gets or sets a value that determines whether the spelling checker is enabled on this text-editing control,
    /// such as <see cref="TextBox"/> or <see cref="RichTextBox"/>.
    /// </summary>
    /// <returns>
    /// true if the spelling checker is enabled on the control; otherwise, false. The default value is false.
    /// </returns>
    public bool IsEnabled
    {
        get => (bool)_owner.GetValue(IsEnabledProperty);
        set => _owner.SetValueInternal(IsEnabledProperty, value);
    }

    /// <summary>
    /// Returns a value that indicates whether the spelling checker is enabled on the specified text-editing
    /// control.
    /// </summary>
    /// <param name="textBoxBase">
    /// The text-editing control to check.
    /// </param>
    /// <returns>
    /// true if the spelling checker is enabled on the text-editing control; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The textBoxBase is null.
    /// </exception>
    public static bool GetIsEnabled(TextBox textBoxBase)
    {
        ArgumentNullException.ThrowIfNull(textBoxBase);

        return (bool)textBoxBase.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Returns a value that indicates whether the spelling checker is enabled on the specified text-editing
    /// control.
    /// </summary>
    /// <param name="textBoxBase">
    /// The text-editing control to check.
    /// </param>
    /// <returns>
    /// true if the spelling checker is enabled on the text-editing control; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The textBoxBase is null.
    /// </exception>
    public static bool GetIsEnabled(RichTextBox textBoxBase)
    {
        ArgumentNullException.ThrowIfNull(textBoxBase);

        return (bool)textBoxBase.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Enables or disables the spelling checker on the specified text-editing control.
    /// </summary>
    /// <param name="textBoxBase">
    /// The text-editing control on which to enable or disable the spelling checker.
    /// </param>
    /// <param name="value">
    /// A Boolean value that specifies whether the spelling checker is enabled on the text-editing control.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The textBoxBase is null.
    /// </exception>
    public static void SetIsEnabled(TextBox textBoxBase, bool value)
    {
        ArgumentNullException.ThrowIfNull(textBoxBase);

        textBoxBase.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Enables or disables the spelling checker on the specified text-editing control.
    /// </summary>
    /// <param name="textBoxBase">
    /// The text-editing control on which to enable or disable the spelling checker.
    /// </param>
    /// <param name="value">
    /// A Boolean value that specifies whether the spelling checker is enabled on the text-editing control.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The textBoxBase is null.
    /// </exception>
    public static void SetIsEnabled(RichTextBox textBoxBase, bool value)
    {
        ArgumentNullException.ThrowIfNull(textBoxBase);

        textBoxBase.SetValue(IsEnabledProperty, value);
    }

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
