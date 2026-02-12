
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

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies visual states conditionally.
/// </summary>
public sealed class StateTrigger : StateTriggerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateTrigger"/> class.
    /// </summary>
    public StateTrigger() { }

    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(
            nameof(IsActive),
            typeof(bool),
            typeof(StateTrigger),
            new PropertyMetadata(BooleanBoxes.FalseBox, OnIsActiveChanged));

    /// <summary>
    /// Gets or sets a value that indicates whether the trigger should be applied.
    /// </summary>
    /// <returns>
    /// true if the system should apply the trigger; otherwise, false.
    /// </returns>
    public new bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValueInternal(IsActiveProperty, value);
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((StateTrigger)d).UpdateState((bool)e.NewValue);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateState(IsActive, true);
    }

    private void UpdateState(bool isActive, bool knownAttached = false)
    {
        if (!knownAttached && !IsAttached)
        {
            return;
        }

        SetActive(isActive);
    }
}
