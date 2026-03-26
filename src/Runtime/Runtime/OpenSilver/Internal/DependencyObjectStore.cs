
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
        OperationType operationType,
        bool isInternal)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            ClearValueCommon(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, newValue, true, isInternal);

        ref EffectiveValueEntry oldEntry = ref storage.Entry;
        EffectiveValueEntry newEntry = default;

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

        if (newEntry.FullValueSource == 0)
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
            ref oldEntry,
            ref newEntry,
            false, // clearValue
            operationType);
    }

    internal static void ClearValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

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
                    oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.Style ||
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
            ref oldEntry,
            ref newEntry,
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

        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        var newEntry = new EffectiveValueEntry(oldEntry);
        newEntry.SetAnimatedValue(value);
        newEntry.IsAnimatedOverLocal = true;

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static void ClearAnimatedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        ref var oldEntry = ref storage.Entry;
        if (!oldEntry.IsAnimated)
        {
            return;
        }

        var newEntry = new EffectiveValueEntry(oldEntry.BaseValueSourceInternal);

        if (oldEntry.IsExpression)
        {
            var expression = (Expression)oldEntry.ModifiedValue.BaseValue;
            newEntry.Value = expression;
            EvaluateExpression(ref newEntry, d, dp, metadata, expression);
        }
        else
        {
            newEntry.Value = oldEntry.HasModifiers ? oldEntry.ModifiedValue.BaseValue : oldEntry.Value;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
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
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.StyleValue = newValue;

        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        // Check for early exit if effective value is not impacted
        if (BaseValueSourceInternal.Style < oldEntry.BaseValueSourceInternal)
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

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, newValue, BaseValueSourceInternal.Style);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
            false,
            OperationType.Unknown);
    }

    internal static void ClearLocalStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        storage.StyleValue = DependencyProperty.UnsetValue;

        if (oldEntry.BaseValueSourceInternal > BaseValueSourceInternal.Style)
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
            ref oldEntry,
            ref newEntry,
            true,
            OperationType.Unknown);
    }

    internal static void SetParentTemplateTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.ParentTemplateTriggerValue = newValue;

        //
        // In WPF, ParentTemplateTrigger has a lower precedence than Local. Since OpenSilver does not support
        // the ParentTemplate value source, it is not possible to replicate this behavior. As a workaround,
        // ParentTemplateTrigger and Local values are treated as if they have the same precedence, which means
        // that the most recently set value will be the active value.
        //
        //if (BaseValueSourceInternal.ParentTemplateTrigger < storage.Entry.BaseValueSourceInternal)
        //{
        //    return;
        //}

        SetTriggerValue(storage, d, dp, metadata, newValue, BaseValueSourceInternal.ParentTemplateTrigger);
    }

    internal static void SetStyleTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.StyleTriggerValue = newValue;

        if (BaseValueSourceInternal.StyleTrigger < storage.Entry.BaseValueSourceInternal)
        {
            return;
        }

        SetTriggerValue(storage, d, dp, metadata, newValue, BaseValueSourceInternal.StyleTrigger);
    }

    internal static void SetTemplateTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.TemplateTriggerValue = newValue;

        if (BaseValueSourceInternal.TemplateTrigger < storage.Entry.BaseValueSourceInternal)
        {
            return;
        }

        SetTriggerValue(storage, d, dp, metadata, newValue, BaseValueSourceInternal.TemplateTrigger);
    }

    internal static void SetThemeStyleTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.ThemeStyleTriggerValue = newValue;

        if (BaseValueSourceInternal.ThemeStyleTrigger < storage.Entry.BaseValueSourceInternal)
        {
            return;
        }

        SetTriggerValue(storage, d, dp, metadata, newValue, BaseValueSourceInternal.ThemeStyleTrigger);
    }

    private static void SetTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue,
        BaseValueSourceInternal valueSource)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            currentExpr.MarkDetached();
            currentExpr.OnDetach(d, dp);
        }

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, metadata, newValue, valueSource);

        if (oldEntry.IsAnimated)
        {
            newEntry.SetAnimatedValue(oldEntry.ModifiedValue.AnimatedValue);
            newEntry.IsAnimatedOverLocal = oldEntry.IsAnimatedOverLocal;
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
            false,
            OperationType.Unknown);
    }

    internal static void ClearParentTemplateTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        storage.ParentTemplateTriggerValue = DependencyProperty.UnsetValue;

        ClearTriggerValue(storage, d, dp, metadata, BaseValueSourceInternal.ParentTemplateTrigger);
    }

    internal static void ClearStyleTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        storage.StyleTriggerValue = DependencyProperty.UnsetValue;

        ClearTriggerValue(storage, d, dp, metadata, BaseValueSourceInternal.StyleTrigger);
    }

    internal static void ClearTemplateTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        storage.TemplateTriggerValue = DependencyProperty.UnsetValue;

        ClearTriggerValue(storage, d, dp, metadata, BaseValueSourceInternal.TemplateTrigger);
    }

    internal static void ClearThemeStyleTriggerValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        storage.ThemeStyleTriggerValue = DependencyProperty.UnsetValue;

        ClearTriggerValue(storage, d, dp, metadata, BaseValueSourceInternal.ThemeStyleTrigger);
    }

    private static void ClearTriggerValue(Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        BaseValueSourceInternal valueSource)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        if (oldEntry.BaseValueSourceInternal > valueSource)
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
            ref oldEntry,
            ref newEntry,
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
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        ref EffectiveValueEntry oldEntry = ref storage.Entry;

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
            ref oldEntry,
            ref newEntry,
            false,
            OperationType.Unknown);
    }

    internal static void ClearThemeStyleValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

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
            ref oldEntry,
            ref newEntry,
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
        Debug.Assert(newValue != DependencyProperty.UnsetValue);

        storage.InheritedValue = newValue;

        ref EffectiveValueEntry oldEntry = ref storage.Entry;

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
            ref oldEntry,
            ref newEntry,
            false,
            propagateChanges ? OperationType.Unknown : OperationType.Inherit);
    }

    internal static bool ClearInheritedValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        bool propagateChanges)
    {
        storage.InheritedValue = DependencyProperty.UnsetValue;

        ref EffectiveValueEntry oldEntry = ref storage.Entry;

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
            ref oldEntry,
            ref newEntry,
            true,
            propagateChanges ? OperationType.Inherit : OperationType.Unknown);
    }

    internal static void SetCurrentValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue,
        bool isInternal)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            Debug.Assert(false, "Don't call SetCurrentValue with UnsetValue");
            ClearValueCommon(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, newValue, false, isInternal);

        bool handled = false;

        ref var oldEntry = ref storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry);

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;

            // if the base value is a reflective expression, we want to set the expression value
            // instead of the coerced value.
            handled = currentExpr.CanSetValue(d, dp);

            if (handled)
            {
                newEntry.SetExpressionValue(newValue);
            }
        }

        if (!handled)
        {
            // Coerce to current value
            object baseValue = GetEffectiveValue(ref newEntry, RequestFlags.CoercionBaseValue);
            ProcessCoerceValue(d,
                dp,
                metadata,
                ref newEntry,
                newValue, // controlValue
                null, // old value is unused when coerceWithCurrentValue is true
                baseValue,
                true);
        }

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
            false, // clearValue
            OperationType.Unknown); // propagateChanges
    }

    internal static void CoerceValueCommon(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        ref EffectiveValueEntry oldEntry = ref storage.Entry;

        if (oldEntry.IsCoercedWithCurrentValue)
        {
            SetCurrentValueCommon(storage,
                d,
                dp,
                metadata,
                oldEntry.ModifiedValue.CoercedValue,
                true);
            return;
        }

        var newEntry = new EffectiveValueEntry(oldEntry);

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
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

        ref var oldEntry = ref storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry);

        EvaluateExpression(ref newEntry, d, dp, metadata, expression);

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            ref oldEntry,
            ref newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static object GetEffectiveValue(ref EffectiveValueEntry entry, RequestFlags requests)
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
            EvaluateExpression(ref entry, d, dp, metadata, expression);
        }
        else
        {
            entry.Value = value;
        }

        return entry;
    }

    private static void EvaluateExpression(
        ref EffectiveValueEntry entry,
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
        (object effectiveValue, BaseValueSourceInternal kind) = storage.GetValue();

        if (kind == BaseValueSourceInternal.Default)
        {
            // default value is not stored in Storage
            return (metadata.GetDefaultValue(owner, dp), BaseValueSourceInternal.Default);
        }

        return (effectiveValue, kind);
    }

    internal static bool UpdateEffectiveValue(
        Storage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        ref EffectiveValueEntry oldEntry,
        ref EffectiveValueEntry newEntry,
        bool clearValue,
        OperationType operationType)
    {
        object oldValue = GetEffectiveValue(ref oldEntry, RequestFlags.FullyResolved);

        // Coerce Value
        // We don't want to coerce the value if it's being reset to the property's default value
        if (metadata.CoerceValueCallback != null && !(clearValue && newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default))
        {
            object baseValue = GetEffectiveValue(ref newEntry, RequestFlags.CoercionBaseValue);
            ProcessCoerceValue(d,
                dp,
                metadata,
                ref newEntry,
                null, // controlValue
                oldValue,
                baseValue,
                false);
        }

        object newValue = GetEffectiveValue(ref newEntry, RequestFlags.FullyResolved);

        bool valueChanged = !Equals(dp, oldValue, newValue);

        // There are two cases in which we need to adjust inheritance contexts:
        //
        //     1.  The value pointed to this DP has changed, in which case
        //         we need to move the context from the old value to the
        //         new value.
        //
        //     2.  The value has not changed, but the ValueSource for the
        //         property has.  (For example, we've gone from being a local
        //         value to the result of a binding expression that just
        //         happens to return the same DO instance.)  In which case
        //         we may need to add or remove contexts even though we
        //         did not raise change notifications.
        //
        // WPF checks FullValueSource instead of BaseValueSourceInternal because
        // it does not want to provide an inheritance contexts if the entry is
        // animated, coerced, or an expression. Silverlight however does not
        // enforce this behavior for expressions and animated values, so we do
        // not enforce it either for now.

        BaseValueSourceInternal oldValueSource = oldEntry.BaseValueSourceInternal;
        BaseValueSourceInternal newValueSource = newEntry.BaseValueSourceInternal;
        bool oldEntryHadContext = oldValueSource == BaseValueSourceInternal.Local;
        bool newEntryNeedsContext = newValueSource == BaseValueSourceInternal.Local;

        if (valueChanged || oldEntryHadContext != newEntryNeedsContext)
        {
            if (oldEntryHadContext)
            {
                d.RemoveSelfAsInheritanceContext(oldValue, dp);
            }

            if (newEntryNeedsContext)
            {
                d.ProvideSelfAsInheritanceContext(newValue, dp);
            }
        }

        if (newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default)
        {
            d.RemoveStorage(storage);
        }
        else
        {
            storage.Entry = newEntry;
        }

        if (valueChanged || (operationType == OperationType.ChangeMutableDefaultValue && oldValueSource != newValueSource))
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
        ref EffectiveValueEntry newEntry,
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
