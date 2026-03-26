
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
using System.Windows.Data;

namespace System.Windows;

/// <summary>
/// Identifies the property system source of a particular dependency property value.
/// </summary>
public enum BaseValueSource
{
    /// <summary>
    /// Source is the default value, as defined by property metadata.
    /// </summary>
    Default = BaseValueSourceInternal.Default,

    /// <summary>
    /// Source is a value through property value inheritance.
    /// </summary>
    Inherited = BaseValueSourceInternal.Inherited,

    /// <summary>
    /// Source is from a setter in the default style. The default style comes from the current theme.
    /// </summary>
    DefaultStyle = BaseValueSourceInternal.ThemeStyle,

    /// <summary>
    /// Source is from a trigger in the default style. The default style comes from the current theme.
    /// </summary>
    DefaultStyleTrigger = BaseValueSourceInternal.ThemeStyleTrigger,

    /// <summary>
    /// Source is from a style setter of a non-theme style.
    /// </summary>
    Style = BaseValueSourceInternal.Style,

    /// <summary>
    /// Source is a trigger-based value in a template that is from a non-theme style.
    /// </summary>
    TemplateTrigger = BaseValueSourceInternal.TemplateTrigger,

    /// <summary>
    /// Source is a trigger-based value of a non-theme style.
    /// </summary>
    StyleTrigger = BaseValueSourceInternal.StyleTrigger,

    /// <summary>
    /// Source is a trigger-based value from a parent template that created the element.
    /// </summary>
    ParentTemplateTrigger = BaseValueSourceInternal.ParentTemplateTrigger,

    /// <summary>
    /// Source is a locally set value.
    /// </summary>
    Local = BaseValueSourceInternal.Local,
}

/// <summary>
/// Reports the information returned from <see cref="DependencyPropertyHelper.GetValueSource(DependencyObject, DependencyProperty)"/>.
/// </summary>
public readonly struct ValueSource
{
    internal ValueSource(BaseValueSourceInternal source, bool isExpression, bool isAnimated, bool isCoerced, bool isCurrent)
    {
        // this cast is justified because the public BaseValueSource enum
        // values agree with the internal BaseValueSourceInternal enum values.
        BaseValueSource = (BaseValueSource)source;

        IsExpression = isExpression;
        IsAnimated = isAnimated;
        IsCoerced = isCoerced;
        IsCurrent = isCurrent;
    }

    /// <summary>
    /// Gets a value of the <see cref="Windows.BaseValueSource"/> enumeration, which reports the source that provided 
    /// the dependency property system with a value.
    /// </summary>
    /// <returns>
    /// A value of the enumeration.
    /// </returns>
    public BaseValueSource BaseValueSource { get; }

    /// <summary>
    /// Gets a value that declares whether this value resulted from an evaluated expression. This might be a 
    /// <see cref="BindingExpression"/> supporting a binding, or an internal expression such as those that support the 
    /// DynamicResource Markup Extension.
    /// </summary>
    /// <returns>
    /// true if the value came from an evaluated expression; otherwise, false.
    /// </returns>
    public bool IsExpression { get; }

    /// <summary>
    /// Gets a value that declares whether the property is being animated.
    /// </summary>
    /// <returns>
    /// true if the property is animated; otherwise, false.
    /// </returns>
    public bool IsAnimated { get; }

    /// <summary>
    /// Gets a value that declares whether this value resulted from a <see cref="CoerceValueCallback"/> implementation applied 
    /// to a dependency property.
    /// </summary>
    /// <returns>
    /// true if the value resulted from a <see cref="CoerceValueCallback"/> implementation applied to a dependency property;
    /// otherwise, false.
    /// </returns>
    public bool IsCoerced { get; }

    /// <summary>
    /// Gets whether the value was set by the <see cref="DependencyObject.SetCurrentValue(DependencyProperty, object)"/> method.
    /// </summary>
    /// <returns>
    /// true if the value was set by the <see cref="DependencyObject.SetCurrentValue(DependencyProperty, object)"/> method; 
    /// otherwise, false.
    /// </returns>
    public bool IsCurrent { get; }

    /// <summary>
    /// Returns the hash code for this <see cref="ValueSource"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit unsigned integer hash code.
    /// </returns>
    public override int GetHashCode() => BaseValueSource.GetHashCode();

    /// <summary>
    /// Returns a value indicating whether this <see cref="ValueSource"/> is equal to a specified object.
    /// </summary>
    /// <param name="o">
    /// The object to compare with this <see cref="ValueSource"/>.
    /// </param>
    /// <returns>
    /// true if the provided object is equivalent to the current <see cref="ValueSource"/>; otherwise, false.
    /// </returns>
    public override bool Equals(object o) => o is ValueSource other && Equals(other);

    public bool Equals(ValueSource other) =>
        BaseValueSource == other.BaseValueSource &&
        IsExpression == other.IsExpression &&
        IsAnimated == other.IsAnimated &&
        IsCoerced == other.IsCoerced;

    /// <summary>
    /// Determines whether two <see cref="ValueSource"/> instances have the same value.
    /// </summary>
    /// <param name="vs1">
    /// The first <see cref="ValueSource"/> to compare.
    /// </param>
    /// <param name="vs2">
    /// The second <see cref="ValueSource"/> to compare.
    /// </param>
    /// <returns>
    /// true if the two <see cref="ValueSource"/> instances are equivalent; otherwise, false.
    /// </returns>
    public static bool operator ==(ValueSource vs1, ValueSource vs2) => vs1.Equals(vs2);

    /// <summary>
    /// Determines whether two <see cref="ValueSource"/> instances do not have the same value.
    /// </summary>
    /// <param name="vs1">
    /// The first <see cref="ValueSource"/> to compare.
    /// </param>
    /// <param name="vs2">
    /// The second <see cref="ValueSource"/> to compare.
    /// </param>
    /// <returns>
    /// true if the two <see cref="ValueSource"/> instances are not equivalent; otherwise, false.
    /// </returns>
    public static bool operator !=(ValueSource vs1, ValueSource vs2) => !vs1.Equals(vs2);
}

/// <summary>
/// Provides a single helper method (<see cref="GetValueSource(DependencyObject, DependencyProperty)"/>) that reports 
/// the property system source for the effective value of a dependency property.
/// </summary>
public static class DependencyPropertyHelper
{
    /// <summary>
    /// Returns a structure that reports various metadata and property system characteristics of a specified dependency 
    /// property on a particular <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="dependencyObject">
    /// The element that contains the dependencyProperty to report information for.
    /// </param>
    /// <param name="dependencyProperty">
    /// The identifier for the dependency property to report information for.
    /// </param>
    /// <returns>
    /// A <see cref="ValueSource"/> structure that reports the specific information.
    /// </returns>
    public static ValueSource GetValueSource(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
        ArgumentNullException.ThrowIfNull(dependencyObject);
        ArgumentNullException.ThrowIfNull(dependencyProperty);

        if (dependencyObject.GetStorage(dependencyProperty.GlobalIndex) is Storage storage)
        {
            EffectiveValueEntry entry = storage.Entry;

            return new ValueSource(
                entry.BaseValueSourceInternal,
                entry.IsExpression,
                entry.IsAnimated,
                entry.IsCoerced,
                entry.IsCoercedWithCurrentValue);
        }

        if (dependencyProperty.ReadOnly)
        {
            PropertyMetadata metadata = dependencyProperty.GetMetadata(dependencyObject.DependencyObjectType);

            if (metadata.GetReadOnlyValueCallback is not null)
            {
                return new ValueSource(BaseValueSourceInternal.Local, false, false, false, false);
            }
        }

        return new ValueSource(BaseValueSourceInternal.Default, false, false, false, false);
    }
}