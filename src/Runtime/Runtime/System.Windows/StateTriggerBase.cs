
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

namespace System.Windows;

/// <summary>
/// Represents the base class for state triggers.
/// </summary>
public abstract class StateTriggerBase : DependencyObject
{
    private bool _isActive;

    /// <summary>
    /// Occurs when the value of the <see cref="IsActive"/> property changes.
    /// </summary>
    public event EventHandler IsActiveChanged;

    /// <summary>
    /// Gets a value that indicates wether the state trigger is currently attached to its owner.
    /// </summary>
    /// <returns>
    /// true if the state trigger is attached to its owner; otherwise, false.
    /// </returns>
    public bool IsAttached { get; private set; }

    /// <summary>
    /// Gets a value that indicates whether the state trigger is active.
    /// </summary>
    /// <returns>
    /// true if the state trigger is active; otherwise, false.
    /// </returns>
    public bool IsActive
    {
        get => _isActive;
        private set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Sets the value that indicates whether the state trigger is active.
    /// </summary>
    /// <param name="isActive">
    /// true if the system should apply the trigger; otherwise, false.
    /// </param>
    protected void SetActive(bool isActive)
    {
        if (IsActive == isActive)
        {
            return;
        }

        IsActive = isActive;

        VisualState?.VisualStateGroup?.UpdateStateTriggers();
    }

    /// <summary>
    /// Invoked when the <see cref="StateTriggerBase" /> is attached to its owner.
    /// </summary>
    protected virtual void OnAttached() { }

    /// <summary>
    /// Invoked when the <see cref="StateTriggerBase" /> is detached from its owner.
    /// </summary>
    protected virtual void OnDetached() { }

    internal void SendAttached()
    {
        if (IsAttached)
        {
            return;
        }

        OnAttached();
        IsAttached = true;
    }

    internal void SendDetached()
    {
        if (!IsAttached)
        {
            return;
        }

        OnDetached();
        IsAttached = false;
    }

    internal VisualState VisualState { get; set; }
}
