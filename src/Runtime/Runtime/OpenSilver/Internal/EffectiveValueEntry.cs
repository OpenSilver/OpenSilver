
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

using System.Windows;

namespace OpenSilver.Internal;

internal sealed class EffectiveValueEntry
{
    public EffectiveValueEntry(object value)
    {
        FullValueSource = (FullValueSource)BaseValueSourceInternal.Default;
        Value = value;
    }

    public EffectiveValueEntry(BaseValueSourceInternal valueSource)
    {
        FullValueSource = (FullValueSource)valueSource;
        Value = DependencyProperty.UnsetValue;
    }

    public EffectiveValueEntry(EffectiveValueEntry copy)
    {
        const FullValueSource CopyMask = FullValueSource.ValueSourceMask |
                                         FullValueSource.IsExpression |
                                         FullValueSource.IsAnimated |
                                         FullValueSource.IsAnimatedOverLocal;

        FullValueSource = copy.FullValueSource & CopyMask;
        if (HasModifiers)
        {
            ModifiedValue mv = copy.ModifiedValue;
            Value = new ModifiedValue
            {
                BaseValue = mv.BaseValue,
                ExpressionValue = mv.ExpressionValue,
                AnimatedValue = mv.AnimatedValue,
            };
        }
        else
        {
            Value = copy.HasModifiers ? copy.ModifiedValue.BaseValue : copy.Value;
        }
    }

    internal object Value { get; set; }

    internal ModifiedValue ModifiedValue => Value as ModifiedValue;

    internal FullValueSource FullValueSource { get; private set; }

    internal BaseValueSourceInternal BaseValueSourceInternal
    {
        get => (BaseValueSourceInternal)(FullValueSource & FullValueSource.ValueSourceMask);
        set => FullValueSource = (FullValueSource & ~FullValueSource.ValueSourceMask) | (FullValueSource)value;
    }

    internal bool HasModifiers => (FullValueSource & FullValueSource.ModifiersMask) != 0;

    internal bool IsExpression
    {
        get => ReadPrivateFlag(FullValueSource.IsExpression);
        private set => WritePrivateFlag(FullValueSource.IsExpression, value);
    }

    internal bool IsAnimated
    {
        get => ReadPrivateFlag(FullValueSource.IsAnimated);
        private set => WritePrivateFlag(FullValueSource.IsAnimated, value);
    }

    internal bool IsCoerced
    {
        get => ReadPrivateFlag(FullValueSource.IsCoerced);
        private set => WritePrivateFlag(FullValueSource.IsCoerced, value);
    }

    internal bool IsCoercedWithCurrentValue
    {
        get => ReadPrivateFlag(FullValueSource.IsCoercedWithCurrentValue);
        private set => WritePrivateFlag(FullValueSource.IsCoercedWithCurrentValue, value);
    }

    internal bool IsAnimatedOverLocal
    {
        get => ReadPrivateFlag(FullValueSource.IsAnimatedOverLocal);
        set => WritePrivateFlag(FullValueSource.IsAnimatedOverLocal, value);
    }

    private ModifiedValue EnsureModifiedValue()
    {
        ModifiedValue modifiedValue;
        if (Value == null)
        {
            Value = modifiedValue = new ModifiedValue();
        }
        else
        {
            modifiedValue = Value as ModifiedValue;
            if (modifiedValue == null)
            {
                modifiedValue = new ModifiedValue();
                modifiedValue.BaseValue = Value;
                Value = modifiedValue;
            }
        }
        return modifiedValue;
    }

    internal void SetCoercedValue(object value, bool coerceWithCurrentValue)
    {
        ModifiedValue modifiedValue = EnsureModifiedValue();
        modifiedValue.CoercedValue = value;
        IsCoerced = true;
        IsCoercedWithCurrentValue = coerceWithCurrentValue;
    }

    internal void SetExpressionValue(object value)
    {
        ModifiedValue modifiedValue = EnsureModifiedValue();
        modifiedValue.ExpressionValue = value;
        IsExpression = true;
    }

    internal void SetAnimatedValue(object value)
    {
        ModifiedValue modifiedValue = EnsureModifiedValue();
        modifiedValue.AnimatedValue = value;
        IsAnimated = true;
    }

    private void WritePrivateFlag(FullValueSource bit, bool value)
    {
        if (value)
        {
            FullValueSource |= bit;
        }
        else
        {
            FullValueSource &= ~bit;
        }
    }

    private bool ReadPrivateFlag(FullValueSource bit) => (FullValueSource & bit) != 0;
}

internal sealed class ModifiedValue
{
    internal object BaseValue { get; set; }
    internal object ExpressionValue { get; set; }
    internal object AnimatedValue { get; set; }
    internal object CoercedValue { get; set; }
}

internal enum FullValueSource : short
{
    // Bit used to store BaseValueSourceInternal = 0x01
    // Bit used to store BaseValueSourceInternal = 0x02
    // Bit used to store BaseValueSourceInternal = 0x04
    // Bit used to store BaseValueSourceInternal = 0x08

    ValueSourceMask = 0x01 | 0x02 | 0x04 | 0x08, // 15
    ModifiersMask = IsExpression | IsAnimated | IsCoerced, // 112
    IsExpression = 0x0010, // 16
    IsAnimated = 0x0020, // 32
    IsCoerced = 0x0040, // 64
    IsCoercedWithCurrentValue = 0x0080, // 128
    IsAnimatedOverLocal = 0x0100, // 256
}

// Note that these enum values are arranged in the reverse order of
// precendence for these sources. Local value has highest
// precedence and Default value has the least.
internal enum BaseValueSourceInternal : short
{
    Default = 0,
    Inherited = 1,
    ThemeStyle = 2,
    LocalStyle = 3,
    Local = 4,
}
