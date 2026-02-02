
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
using System.Windows.Markup;

namespace System.Windows;

/// <summary>
/// Represents a trigger that applies property values or performs actions when a set
/// of conditions are satisfied.
/// </summary>
[ContentProperty(nameof(Setters))]
public sealed class MultiTrigger : TriggerBase
{
    private readonly ConditionCollection _conditions = new();
    private SetterBaseCollection _setters;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTrigger"/> class.
    /// </summary>
    public MultiTrigger() { }

    /// <summary>
    /// Gets a collection of <see cref="Condition"/> objects. Changes to property values
    /// are applied when all of the conditions in the collection are met.
    /// </summary>
    /// <returns>
    /// The default is an empty collection.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ConditionCollection Conditions => _conditions;

    /// <summary>
    /// Gets a collection of <see cref="Setter"/> objects, which describe the property values
    /// to apply when all of the conditions of the <see cref="MultiTrigger"/> are met.
    /// </summary>
    /// <returns>
    /// The default value is null.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SetterBaseCollection Setters => _setters ??= new SetterBaseCollection();

    /// <summary>
    /// Gets a value indicating whether this trigger has any setters.
    /// </summary>
    internal bool HasSetters => _setters is not null && _setters.InternalCount > 0;

    internal override void Seal()
    {
        if (IsSealed)
        {
            return;
        }

        // Seal conditions (passing true for isPropertyTrigger)
        if (_conditions.Count > 0)
        {
            _conditions.Seal(isPropertyTrigger: true);
        }

        // Seal the setters collection
        _setters?.Seal();

        base.Seal();
    }

    /// <summary>
    /// Evaluates whether all trigger conditions are currently satisfied for the given element.
    /// </summary>
    internal bool Evaluate(DependencyObject container)
    {
        if (_conditions.Count == 0)
        {
            return false;
        }

        foreach (Condition condition in _conditions)
        {
            if (condition.Property is null)
            {
                return false;
            }

            object currentValue = container.GetValue(condition.Property);
            if (!Trigger.Match(currentValue, condition.Value))
            {
                return false;
            }
        }

        return true;
    }
}
