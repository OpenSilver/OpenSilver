
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

using System.ComponentModel;

namespace System.Windows;

/// <summary>
/// Serves as the base class for <see cref="Trigger"/>, <see cref="DataTrigger"/>,
/// <see cref="MultiTrigger"/>, <see cref="MultiDataTrigger"/>, and <see cref="EventTrigger"/>.
/// </summary>
public abstract class TriggerBase : DependencyObject
{
    private TriggerActionCollection _enterActions;
    private TriggerActionCollection _exitActions;
    private bool _sealed;

    internal TriggerBase() { }

    /// <summary>
    /// Gets a collection of <see cref="TriggerAction"/> objects to apply when the trigger 
    /// object becomes active. This property does not apply to the <see cref="EventTrigger"/> class.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public TriggerActionCollection EnterActions
    {
        get
        {
            if (_enterActions is null)
            {
                _enterActions = new TriggerActionCollection();
                if (_sealed)
                {
                    _enterActions.Seal();
                }
            }
            return _enterActions;
        }
    }

    internal bool HasEnterActions => _enterActions is not null && _enterActions.Count > 0;

    /// <summary>
    /// Gets a collection of <see cref="TriggerAction"/> objects to apply when the trigger 
    /// object becomes inactive. This property does not apply to the <see cref="EventTrigger"/> class.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public TriggerActionCollection ExitActions
    {
        get
        {
            if (_exitActions is null)
            {
                _exitActions = new TriggerActionCollection();
                if (_sealed)
                {
                    _exitActions.Seal();
                }
            }
            return _exitActions;
        }
    }

    internal bool HasExitActions => _exitActions is not null && _exitActions.Count > 0;

    /// <summary>
    /// Gets a value indicating whether this trigger is sealed and cannot be changed.
    /// </summary>
    public new bool IsSealed => _sealed;

    internal new virtual void Seal()
    {
        _sealed = true;
        _enterActions?.Seal();
        _exitActions?.Seal();
    }

    internal void CheckSealed()
    {
        if (_sealed)
        {
            throw new InvalidOperationException(string.Format(OpenSilver.Internal.Strings.CannotChangeAfterSealed, GetType().Name));
        }
    }
}
