
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
using System.Windows.Markup;

namespace System.Windows.Controls;

/// <summary>
/// Defines the element tree that is applied as the control template for a control.
/// </summary>
public sealed class ControlTemplate : FrameworkTemplate
{
    private Type _targetType;
    private TriggerCollection _triggers;

    /// <summary>
    /// Initializes a new instance of the ControlTemplate class.
    /// </summary>
    public ControlTemplate() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlTemplate"/> class with the specified 
    /// target type.
    /// </summary>
    /// <param name="targetType">
    /// The type this template is intended for.
    /// </param>
    public ControlTemplate(Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        _targetType = targetType;
    }

    /// <summary>
    /// Gets or sets the type to which the ControlTemplate is applied.
    /// </summary>
    [Ambient]
    public Type TargetType
    {
        get => _targetType;
        set { CheckSealed(); _targetType = value; }
    }

    /// <summary>
    /// Gets a collection of <see cref="TriggerBase"/> objects that apply property changes
    /// or perform actions based on specified conditions.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="TriggerBase"/> objects. The default is an empty collection.
    /// </returns>
    public TriggerCollection Triggers
    {
        get
        {
            if (_triggers is null)
            {
                _triggers = [];

                if (IsSealed())
                {
                    _triggers.Seal();
                }
            }
            return _triggers;
        }
    }

    /// <summary>
    /// Checks the templated parent against a set of rules.
    /// </summary>
    /// <param name="templatedParent">
    /// The element this template is applied to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="templatedParent"/> must not be null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// You must associate the <see cref="ControlTemplate"/> with a <see cref="Control"/>
    /// by setting the <see cref="Control.Template"/> property before using the 
    /// <see cref="ControlTemplate"/> on the <see cref="Control"/>.
    /// </exception>
    protected override void ValidateTemplatedParent(FrameworkElement templatedParent)
    {
        // Must have a non-null feTemplatedParent
        ArgumentNullException.ThrowIfNull(templatedParent);

        // The target type of a ControlTemplate must match the 
        // type of the Control that it is being applied to
        if (_targetType is not null && !_targetType.IsInstanceOfType(templatedParent))
        {
            throw new ArgumentException(
                string.Format(Strings.TemplateTargetTypeMismatch, _targetType.Name, templatedParent.GetType().Name));
        }

        // One cannot use a ControlTemplate to template a Control that isn't associated with it
        if (templatedParent.TemplateInternal != this)
        {
            throw new ArgumentException(Strings.MustNotTemplateUnassociatedControl);
        }
    }

    internal override Type TargetTypeInternal => TargetType;

    internal override TriggerCollection TriggersInternal => _triggers;
}
