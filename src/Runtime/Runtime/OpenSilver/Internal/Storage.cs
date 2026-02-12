
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

using System.Diagnostics;
using System.Windows;
using OpenSilver.Internal.Media.Animation;

namespace OpenSilver.Internal;

internal sealed class Storage
{
    private UncommonFields _uncommonFields;
    private EffectiveValueEntry _entry;

    internal Storage(DependencyProperty dp, bool inheritable)
    {
        LocalValue = DependencyProperty.UnsetValue;
        PropertyIndex = dp.GlobalIndex;
        Inheritable = inheritable;
    }

    internal Storage(int propertyIndex)
    {
        Debug.Assert(DependencyProperty.RegisteredPropertyList[propertyIndex] is null);

        LocalValue = DependencyProperty.UnsetValue;
        PropertyIndex = propertyIndex;
        Inheritable = false;
    }

    // CRITICAL: DependencyObject uses a custom collection to store Storage and DependentList that requires
    // these objects to override GetHashCode() to return a DependencyProperty index. This method cannot be
    // removed or changed.
    public sealed override int GetHashCode() => PropertyIndex;

    public sealed override bool Equals(object obj) => base.Equals(obj);

    internal int PropertyIndex { get; }

    internal bool Inheritable { get; }

    internal ref EffectiveValueEntry Entry => ref _entry;

    internal object LocalValue { get; set; }

    internal TimelineClock Clock
    {
        get => _uncommonFields?.Clock;
        set => (_uncommonFields ??= new()).Clock = value;
    }

    internal object LocalStyleValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.LocalStyleValue;
        set => (_uncommonFields ??= new()).LocalStyleValue = value;
    }

    internal object StyleTriggerValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.StyleTriggerValue;
        set => (_uncommonFields ??= new()).StyleTriggerValue = value;
    }

    internal object ParentTemplateTriggerValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.ParentTemplateTriggerValue;
        set => (_uncommonFields ??= new()).ParentTemplateTriggerValue = value;
    }

    internal object ThemeStyleTriggerValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.ThemeStyleTriggerValue;
        set => (_uncommonFields ??= new()).ThemeStyleTriggerValue = value;
    }

    internal object ThemeStyleValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.ThemeStyleValue;
        set => (_uncommonFields ??= new()).ThemeStyleValue = value;
    }

    internal object InheritedValue
    {
        get => _uncommonFields is null ? DependencyProperty.UnsetValue : _uncommonFields.InheritedValue;
        set => (_uncommonFields ??= new()).InheritedValue = value;
    }

    internal (object effectiveValue, BaseValueSourceInternal kind) GetValue()
    {
        // Check ParentTemplateTrigger first (highest precedence)
        if (_uncommonFields is not null && _uncommonFields.ParentTemplateTriggerValue != DependencyProperty.UnsetValue)
        {
            return (_uncommonFields.ParentTemplateTriggerValue, BaseValueSourceInternal.ParentTemplateTrigger);
        }

        if (LocalValue != DependencyProperty.UnsetValue)
        {
            return (LocalValue, BaseValueSourceInternal.Local);
        }

        if (_uncommonFields is not null)
        {
            // Style trigger values have higher precedence than regular style values
            if (_uncommonFields.StyleTriggerValue != DependencyProperty.UnsetValue)
            {
                return (_uncommonFields.StyleTriggerValue, BaseValueSourceInternal.Style);
            }
            else if (_uncommonFields.LocalStyleValue != DependencyProperty.UnsetValue)
            {
                return (_uncommonFields.LocalStyleValue, BaseValueSourceInternal.Style);
            }
            else if (_uncommonFields.ThemeStyleTriggerValue != DependencyProperty.UnsetValue)
            {
                return (_uncommonFields.ThemeStyleTriggerValue, BaseValueSourceInternal.ThemeStyleTrigger);
            }
            else if (_uncommonFields.ThemeStyleValue != DependencyProperty.UnsetValue)
            {
                return (_uncommonFields.ThemeStyleValue, BaseValueSourceInternal.ThemeStyle);
            }
            else if (_uncommonFields.InheritedValue != DependencyProperty.UnsetValue)
            {
                return (_uncommonFields.InheritedValue, BaseValueSourceInternal.Inherited);
            }
        }

        return (DependencyProperty.UnsetValue, BaseValueSourceInternal.Default);
    }

    private sealed class UncommonFields
    {
        public UncommonFields()
        {
            LocalStyleValue = DependencyProperty.UnsetValue;
            StyleTriggerValue = DependencyProperty.UnsetValue;
            ParentTemplateTriggerValue = DependencyProperty.UnsetValue;
            ThemeStyleTriggerValue = DependencyProperty.UnsetValue;
            ThemeStyleValue = DependencyProperty.UnsetValue;
            InheritedValue = DependencyProperty.UnsetValue;
        }

        internal TimelineClock Clock;
        internal object LocalStyleValue;
        internal object StyleTriggerValue;
        internal object ParentTemplateTriggerValue;
        internal object ThemeStyleTriggerValue;
        internal object ThemeStyleValue;
        internal object InheritedValue;
    }
}
