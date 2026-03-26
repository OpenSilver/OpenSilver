
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
using OpenSilver.Internal.Data;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows;

/// <summary>
/// Serves as the base class for <see cref="Trigger"/>, <see cref="DataTrigger"/>,
/// <see cref="MultiTrigger"/>, <see cref="MultiDataTrigger"/>, and <see cref="EventTrigger"/>.
/// </summary>
public abstract class TriggerBase : DependencyObject
{
    private TriggerActionCollection _enterActions;
    private TriggerActionCollection _exitActions;

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
                _enterActions = [];
                if (IsSealed)
                {
                    _enterActions.Seal();
                }
            }
            return _enterActions;
        }
    }

    internal bool HasEnterActions => _enterActions is not null && _enterActions.InternalCount > 0;

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
                _exitActions = [];
                if (IsSealed)
                {
                    _exitActions.Seal();
                }
            }
            return _exitActions;
        }
    }

    internal bool HasExitActions => _exitActions is not null && _exitActions.InternalCount > 0;

    internal override void Seal()
    {
        base.Seal();

        _enterActions?.Seal();
        _exitActions?.Seal();
    }

    internal void CheckSealed()
    {
        if (IsSealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, GetType().Name));
        }
    }

    internal static void ProcessSettersCollection(SetterBaseCollection setters)
    {
        if (setters is null)
        {
            return;
        }

        setters.Seal();

        foreach (SetterBase setterBase in setters.InternalItems)
        {
            if (setterBase is not Setter setter)
            {
                throw new InvalidOperationException(
                    string.Format(Strings.VisualTriggerSettersIncludeUnsupportedSetterType, setterBase.GetType().Name));
            }

            DependencyProperty dp = setter.Property;
            string target = setter.TargetName;

            if (target is null)
            {
                // Not allowed to use Style to affect the StyleProperty.
                if (dp == FrameworkElement.StyleProperty)
                {
                    throw new ArgumentException(Strings.StylePropertyInStyleNotAllowed);
                }
            }
            else
            {
                if (target.Length == 0)
                {
                    throw new ArgumentException(Strings.ChildNameMustBeNonEmpty);
                }
            }
        }
    }
}
