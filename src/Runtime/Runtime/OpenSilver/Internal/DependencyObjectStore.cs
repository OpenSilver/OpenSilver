
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

using System;
using System.Diagnostics;
using System.Windows;

namespace OpenSilver.Internal;

internal static class DependencyObjectStore
{
    internal static void SetValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue,
        bool isInternal)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            ClearValueCommon(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, newValue, true, isInternal);

        EffectiveValueEntry oldEntry = storage.Entry;
        EffectiveValueEntry newEntry = null;

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            var newExpr = newValue as Expression;

            if (currentExpr == newExpr)
            {
                Debug.Assert(newExpr.IsAttached);
                RefreshExpressionCommon(storage, d, dp, metadata, newExpr);
                return;
            }

            // if the current BindingExpression is a TwoWay binding, we don't want to remove the binding 
            // unless we are overriding it with a new Expression.
            if (newExpr is null && currentExpr.CanSetValue(d, dp))
            {
                newEntry = new EffectiveValueEntry(oldEntry.BaseValueSourceInternal);
                newEntry.Value = currentExpr;
                newEntry.SetExpressionValue(newValue);
            }
            else
            {
                currentExpr.MarkDetached();
                currentExpr.OnDetach(d, dp);
            }
        }

        if (newEntry is null)
        {
            // Set the new local value
            storage.LocalValue = newValue;

            newEntry = EvaluateEffectiveValue(d, dp, metadata, newValue, BaseValueSourceInternal.Local);
        }

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static void ClearValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        EffectiveValueEntry oldEntry = storage.Entry;

        object current = storage.LocalValue;

        // Reset local value
        storage.LocalValue = DependencyProperty.UnsetValue;

        if (oldEntry.IsExpression)
        {
            // Inform value expression of detachment, if applicable
            if (current is Expression currentExpr)
            {
                currentExpr.MarkDetached();
                currentExpr.OnDetach(d, dp);
            }
            else
            {
                Debug.Assert(
                    oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.LocalStyle ||
                    oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.ThemeStyle);

                RefreshExpressionCommon(storage, d, dp, metadata, (Expression)oldEntry.ModifiedValue.BaseValue);
                return;
            }
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) =
            ComputeEffectiveBaseValue(storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, effectiveValue, effectiveValueKind);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = true;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            true, // clearValue
            OperationType.Unknown);
    }

    internal static void SetAnimatedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object value)
    {
        ValidateValue(dp, value, false, true);

        EffectiveValueEntry oldEntry = storage.Entry;

        var newEntry = new EffectiveValueEntry(oldEntry);
        newEntry.SetAnimatedValue(value);
        newEntry.IsAnimatedOverLocal = true;

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static void ClearAnimatedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        var oldEntry = storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry.BaseValueSourceInternal);

        if (oldEntry.IsExpression)
        {
            var expression = (Expression)oldEntry.ModifiedValue.BaseValue;
            newEntry.Value = expression;
            EvaluateExpression(newEntry, d, dp, metadata, expression);
        }
        else
        {
            newEntry.Value = oldEntry.HasModifiers ? oldEntry.ModifiedValue.BaseValue : oldEntry.Value;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            true,
            OperationType.Unknown);
    }

    internal static void SetLocalStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            ClearLocalStyleValue(storage, d, dp, metadata);
            return;
        }

        storage.LocalStyleValue = newValue;

        EffectiveValueEntry oldEntry = storage.Entry;

        // Check for early exit if effective value is not impacted
        if (BaseValueSourceInternal.LocalStyle < oldEntry.BaseValueSourceInternal)
        {
            // value source remains the same.
            // Exit if the newly set value is of lower precedence than the effective value.
            return;
        }

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            currentExpr.MarkDetached();
            currentExpr.OnDetach(d, dp);
        }

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, newValue, BaseValueSourceInternal.LocalStyle);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false,
            OperationType.Unknown);
    }

    private static void ClearLocalStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        EffectiveValueEntry oldEntry = storage.Entry;

        storage.LocalStyleValue = DependencyProperty.UnsetValue;

        if (oldEntry.BaseValueSourceInternal > BaseValueSourceInternal.LocalStyle)
        {
            return;
        }

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            currentExpr.MarkDetached();
            currentExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, effectiveValue, effectiveValueKind);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            true,
            OperationType.Unknown);
    }

    internal static void SetThemeStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            ClearThemeStyleValue(storage, d, dp, metadata);
            return;
        }

        EffectiveValueEntry oldEntry = storage.Entry;

        storage.ThemeStyleValue = newValue;

        // Check for early exit if effective value is not impacted
        if (BaseValueSourceInternal.ThemeStyle < oldEntry.BaseValueSourceInternal)
        {
            // value source remains the same.
            // Exit if the newly set value is of lower precedence than the effective value.
            return;
        }

        if (oldEntry.IsExpression)
        {
            var oldExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            oldExpr.MarkDetached();
            oldExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, effectiveValue, effectiveValueKind);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false,
            OperationType.Unknown);
    }

    private static void ClearThemeStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        EffectiveValueEntry oldEntry = storage.Entry;

        storage.ThemeStyleValue = DependencyProperty.UnsetValue;

        if (oldEntry.BaseValueSourceInternal > BaseValueSourceInternal.ThemeStyle)
        {
            return;
        }

        if (oldEntry.IsExpression)
        {
            var oldExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            oldExpr.MarkDetached();
            oldExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, effectiveValue, effectiveValueKind);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            true,
            OperationType.Unknown);
    }

    internal static bool SetInheritedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue,
        bool propagateChanges)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            return ClearInheritedValue(storage, d, dp, metadata, propagateChanges);
        }

        storage.InheritedValue = newValue;

        EffectiveValueEntry oldEntry = storage.Entry;

        // Check for early exit if effective value is not impacted
        if (BaseValueSourceInternal.Inherited < oldEntry.BaseValueSourceInternal)
        {
            // value source remains the same.
            // Exit if the newly set value is of lower precedence than the effective value.
            return false;
        }

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, newValue, BaseValueSourceInternal.Inherited);

        return UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false,
            propagateChanges ? OperationType.Unknown : OperationType.Inherit);
    }

    private static bool ClearInheritedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        bool propagateChanges)
    {
        storage.InheritedValue = DependencyProperty.UnsetValue;

        EffectiveValueEntry oldEntry = storage.Entry;

        if (oldEntry.BaseValueSourceInternal > BaseValueSourceInternal.Inherited)
        {
            return false;
        }

        var newEntry = new EffectiveValueEntry(metadata.GetDefaultValue(d, dp));

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        return UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            true,
            propagateChanges ? OperationType.Inherit : OperationType.Unknown);
    }

    internal static void SetCurrentValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            Debug.Assert(false, "Don't call SetCurrentValue with UnsetValue");
            ClearValueCommon(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, newValue, false, true);

        var oldEntry = storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry);

        // Coerce to current value
        object baseValue = GetEffectiveValue(newEntry, RequestFlags.CoercionBaseValue);
        ProcessCoerceValue(d,
            dp,
            metadata,
            newEntry,
            newValue, // controlValue
            null, // old value is unused when coerceWithCurrentValue is true
            baseValue,
            true); // coerceWithCurrentValue

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false, // clearValue
            OperationType.Unknown); // propagateChanges
    }

    internal static void CoerceValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        EffectiveValueEntry oldEntry = storage.Entry;

        if (oldEntry.IsCoercedWithCurrentValue)
        {
            SetCurrentValueCommon(storage,
                d,
                dp,
                metadata,
                oldEntry.ModifiedValue.CoercedValue);
            return;
        }

        var newEntry = new EffectiveValueEntry(oldEntry);

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false,
            OperationType.Unknown);
    }

    internal static void RefreshExpressionCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        Expression expression)
    {
        Debug.Assert(expression != null, "Expression should not be null");
        Debug.Assert(storage.Entry.IsExpression, "Property base value is not a BindingExpression !");
        Debug.Assert(storage.Entry.ModifiedValue.BaseValue == expression, "Expression is not active !");

        var oldEntry = storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry);

        EvaluateExpression(newEntry, d, dp, metadata, expression);

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static object GetEffectiveValue(EffectiveValueEntry entry, RequestFlags requests)
    {
        if (entry.HasModifiers)
        {
            ModifiedValue mv = entry.ModifiedValue;

            // Note that the modified values have an order of precedence
            // 1. Coerced Value (including Current value)
            // 2. Animated Value
            // 3. Expression Value
            // Also note that we support any arbitrary combinations of these
            // modifiers and will yet the precedence metioned above.
            if (entry.IsCoerced && ((requests & RequestFlags.CoercionBaseValue) == 0 || entry.IsCoercedWithCurrentValue))
            {
                return mv.CoercedValue;
            }

            if (entry.IsAnimatedOverLocal && entry.IsAnimated && (requests & RequestFlags.AnimationBaseValue) == 0)
            {
                return mv.AnimatedValue;
            }

            if (entry.IsExpression)
            {
                return mv.ExpressionValue;
            }

            return mv.BaseValue;
        }

        return entry.Value;
    }

    private static EffectiveValueEntry EvaluateEffectiveValue(
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object value,
        BaseValueSourceInternal valueSource)
    {
        var entry = new EffectiveValueEntry(valueSource);

        if (value is Expression expression)
        {
            Debug.Assert(valueSource > BaseValueSourceInternal.Inherited);

            if (expression.IsAttached)
            {
                throw new InvalidOperationException(Strings.SharingNonSharableExpression);
            }

            expression.MarkAttached();
            expression.OnAttach(d, dp);

            entry.Value = expression;
            EvaluateExpression(entry, d, dp, metadata, expression);
        }
        else
        {
            entry.Value = value;
        }

        return entry;
    }

    private static void EvaluateExpression(
        EffectiveValueEntry entry,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        Expression expression)
    {
        Debug.Assert(expression != null);
        Debug.Assert(entry.Value == expression || entry.ModifiedValue.BaseValue == expression);

        object exprValue = expression.GetValue(d, dp);

        // if there is still no value, use the default
        if (exprValue == DependencyProperty.UnsetValue)
        {
            exprValue = metadata.GetDefaultValue(d, dp);
        }

        ValidateValue(dp, exprValue, false, false);

        entry.SetExpressionValue(exprValue);
    }

    private static (object effectiveValue, BaseValueSourceInternal kind) ComputeEffectiveBaseValue(
        Storage storage,
        DependencyObject owner,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        if (storage.LocalValue != DependencyProperty.UnsetValue)
        {
            return (storage.LocalValue, BaseValueSourceInternal.Local);
        }
        else if (storage.LocalStyleValue != DependencyProperty.UnsetValue)
        {
            return (storage.LocalStyleValue, BaseValueSourceInternal.LocalStyle);
        }
        else if (storage.ThemeStyleValue != DependencyProperty.UnsetValue)
        {
            return (storage.ThemeStyleValue, BaseValueSourceInternal.ThemeStyle);
        }
        else if (storage.InheritedValue != DependencyProperty.UnsetValue)
        {
            return (storage.InheritedValue, BaseValueSourceInternal.Inherited);
        }
        else // Property default value
        {
            return (metadata.GetDefaultValue(owner, dp), BaseValueSourceInternal.Default);
        }
    }

    private static bool UpdateEffectiveValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        EffectiveValueEntry oldEntry,
        EffectiveValueEntry newEntry,
        bool clearValue,
        OperationType operationType)
    {
        object oldValue = GetEffectiveValue(oldEntry, RequestFlags.FullyResolved);

        // Coerce Value
        // We don't want to coerce the value if it's being reset to the property's default value
        if (metadata.CoerceValueCallback != null && !(clearValue && newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default))
        {
            object baseValue = GetEffectiveValue(newEntry, RequestFlags.CoercionBaseValue);
            ProcessCoerceValue(d,
                dp,
                metadata,
                newEntry,
                null, // controlValue
                oldValue,
                baseValue,
                false);
        }

        object newValue = GetEffectiveValue(newEntry, RequestFlags.FullyResolved);

        // Reset old value inheritance context
        if (oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.Local)
        {
            // Notes:
            // - Inheritance context is only handled by local value
            // - We use null instead of the actual DependencyProperty
            // as the parameter is ignored in the current implentation.
            d.RemoveSelfAsInheritanceContext(oldValue, null);
        }

        // Set new value inheritance context
        if (newEntry.BaseValueSourceInternal == BaseValueSourceInternal.Local)
        {
            // Check above
            d.ProvideSelfAsInheritanceContext(newValue, null);
        }

        if (newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default)
        {
            d.RemoveStorage(storage);
        }
        else
        {
            storage.Entry = newEntry;
        }

        bool valueChanged = !Equals(dp, oldValue, newValue);
        if (valueChanged)
        {
            d.NotifyPropertyChange(
                new DependencyPropertyChangedEventArgs(
                    oldValue,
                    newValue,
                    dp,
                    metadata,
                    operationType));
        }

        return valueChanged;
    }

    private static void ProcessCoerceValue(
        DependencyObject target,
        DependencyProperty dp,
        PropertyMetadata metadata,
        EffectiveValueEntry newEntry,
        object controlValue,
        object oldValue,
        object baseValue,
        bool coerceWithCurrentValue)
    {
        object coercedValue = coerceWithCurrentValue ? controlValue : metadata.CoerceValueCallback(target, baseValue);

        if (!Equals(dp, coercedValue, baseValue))
        {
            // returning DependencyProperty.UnsetValue from a Coercion callback means "don't do the set" ...
            // or "use previous value"
            if (coercedValue == DependencyProperty.UnsetValue)
            {
                Debug.Assert(!coerceWithCurrentValue);
                coercedValue = oldValue;
            }

            newEntry.SetCoercedValue(coercedValue, coerceWithCurrentValue);
        }
    }

    private static bool Equals(DependencyProperty dp, object obj1, object obj2)
    {
        if (dp.IsValueType || dp.IsStringType)
        {
            return Equals(obj1, obj2);
        }
        return ReferenceEquals(obj1, obj2);
    }

    private static void ValidateValue(DependencyProperty dp, object value, bool allowExpression, bool isInternal)
    {
        bool isValidValue = isInternal ? dp.IsValidValueInternal(value) : dp.IsValidValue(value);

        if (!isValidValue)
        {
            isValidValue = allowExpression && value is Expression;
        }

        if (!isValidValue)
        {
            throw new ArgumentException(string.Format(Strings.InvalidPropertyValue, value, dp.Name));
        }
    }
}

internal enum RequestFlags
{
    FullyResolved = 0x00,
    AnimationBaseValue = 0x01,
    CoercionBaseValue = 0x02,
}
