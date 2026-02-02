
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
/// Represents a trigger that applies property values or performs actions when the
/// bound data meet a set of conditions.
/// </summary>
[ContentProperty(nameof(Setters))]
public sealed class MultiDataTrigger : TriggerBase
{
    private readonly ConditionCollection _conditions = new();
    private SetterBaseCollection _setters;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiDataTrigger"/> class.
    /// </summary>
    public MultiDataTrigger() { }

    /// <summary>
    /// Gets a collection of <see cref="Condition"/> objects. Changes to property values
    /// are applied when all the conditions in the collection are met.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="Condition"/> objects. The default is an empty collection.
    /// </returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ConditionCollection Conditions => _conditions;

    /// <summary>
    /// Gets a collection of <see cref="Setter"/> objects that describe the property values
    /// to apply when all the conditions of the <see cref="MultiDataTrigger"/> are met.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="Setter"/> objects. The default value is an empty collection.
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

        // Seal conditions (passing false for isPropertyTrigger since this is a data trigger)
        if (_conditions.Count > 0)
        {
            _conditions.Seal(isPropertyTrigger: false);
        }

        // Seal the setters collection
        _setters?.Seal();

        base.Seal();
    }
}
