
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal;

internal static class INTERNAL_PropertyStore
{
    /// <summary>
    /// Attempt to get a Property Storage
    /// </summary>
    /// <param name="dependencyObject"></param>
    /// <param name="dependencyProperty"></param>
    /// <param name="metadata"></param>
    /// <param name="createIfNotFoud">when set to true, it forces the creation of the storage if it does not exists yet.</param>
    /// <param name="storage"></param>
    /// <returns></returns>
    public static bool TryGetStorage(
        DependencyObject dependencyObject,
        DependencyProperty dependencyProperty,
        PropertyMetadata metadata,
        bool createIfNotFoud,
        out INTERNAL_PropertyStorage storage)
    {
        if (dependencyObject.INTERNAL_PropertyStorageDictionary.TryGetValue(dependencyProperty, out storage))
        {
            return true;
        }

        if (createIfNotFoud)
        {
            //----------------------
            // CREATE A NEW STORAGE:
            //----------------------

            metadata ??= dependencyProperty.GetMetadata(dependencyObject.DependencyObjectType);

            storage = INTERNAL_PropertyStorage.CreateDefaultValueEntry(metadata.GetDefaultValue(dependencyObject, dependencyProperty));

            dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);

            //-----------------------
            // CHECK IF THE PROPERTY IS INHERITABLE:
            //-----------------------
            if (metadata.Inherits)
            {
                //-----------------------
                // ADD THE STORAGE TO "INTERNAL_AllInheritedProperties" IF IT IS NOT ALREADY THERE:
                //-----------------------
                dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);
            }
        }

        return createIfNotFoud;
    }

    /// <summary>
    /// Attemp to get a Property Storage for an inherited property 
    /// (faster than generic accessor 'TryGetStorage')
    /// </summary>
    /// <param name="dependencyObject"></param>
    /// <param name="dependencyProperty"></param>
    /// <param name="metadata"></param>
    /// <param name="createIfNotFoud">when set to true, it forces the creation of the storage if it does not exists yet.</param>
    /// <param name="storage"></param>
    /// <returns></returns>
    internal static bool TryGetInheritedPropertyStorage(
        DependencyObject dependencyObject,
        DependencyProperty dependencyProperty,
        PropertyMetadata metadata,
        bool createIfNotFoud,
        out INTERNAL_PropertyStorage storage)
    {
        // Create the Storage if it does not already exist
        if (dependencyObject.INTERNAL_AllInheritedProperties.TryGetValue(dependencyProperty, out storage))
        {
            return true;
        }

        if (createIfNotFoud)
        {
            metadata ??= dependencyProperty.GetMetadata(dependencyObject.DependencyObjectType);

            Debug.Assert(metadata != null && metadata.Inherits,
                $"{dependencyProperty.Name} is not an inherited property.");

            // Create the storage:
            storage = INTERNAL_PropertyStorage.CreateDefaultValueEntry(metadata.GetDefaultValue(dependencyObject, dependencyProperty));

            //-----------------------
            // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
            //-----------------------
            dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);
            dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);
        }

        return createIfNotFoud;
    }

    internal static void SetValueCommon(
        INTERNAL_PropertyStorage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object newValue)
    {
        if (newValue == DependencyProperty.UnsetValue)
        {
            ClearValueCommon(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, newValue, true);

        EffectiveValueEntry oldEntry = storage.Entry;
        EffectiveValueEntry newEntry = null;

        var newExpr = newValue as Expression;

        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;

            if (oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.Local)
            {
                if (currentExpr == newExpr)
                {
                    Debug.Assert(newExpr.IsAttached);
                    RefreshExpressionCommon(storage, d, dp, metadata, newExpr);
                    return;
                }

                // if the current BindingExpression is a TwoWay binding, we don't want to remove the binding 
                // unless we are overriding it with a new Expression.
                if (newExpr == null && currentExpr.CanSetValue(d, dp))
                {
                    newEntry = new EffectiveValueEntry(BaseValueSourceInternal.Local);
                    newEntry.Value = currentExpr;
                    newEntry.SetExpressionValue(newValue);
                }
                else
                {
                    currentExpr.OnDetach(d, dp);
                }
            }
            else
            {
                Debug.Assert(oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.LocalStyle ||
                             oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.ThemeStyle);

                currentExpr.OnDetach(d, dp);
            }
        }

        if (newEntry == null)
        {
            // Set the new local value
            storage.LocalValue = newValue;

            newEntry = EvaluateEffectiveValue(d, dp, newValue, BaseValueSourceInternal.Local);
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
        INTERNAL_PropertyStorage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata)
    {
        // Reset local value
        storage.LocalValue = DependencyProperty.UnsetValue;

        EffectiveValueEntry oldEntry = storage.Entry;

        // Check for expression
        if (oldEntry.IsExpression)
        {
            var currentExpr = (Expression)oldEntry.ModifiedValue.BaseValue;
            currentExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) =
            ComputeEffectiveBaseValue(storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, effectiveValue, effectiveValueKind);

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
        INTERNAL_PropertyStorage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object value)
    {
        if (value == DependencyProperty.UnsetValue)
        {
            ClearAnimatedValue(storage, d, dp, metadata);
            return;
        }

        ValidateValue(dp, value, false);

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
        INTERNAL_PropertyStorage storage,
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
            EvaluateExpression(newEntry, d, dp, expression);
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
        INTERNAL_PropertyStorage storage,
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
            currentExpr.OnDetach(d, dp);
        }

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, newValue, BaseValueSourceInternal.LocalStyle);

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
        INTERNAL_PropertyStorage storage,
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
            currentExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, effectiveValue, effectiveValueKind);

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
        INTERNAL_PropertyStorage storage,
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
            oldExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, effectiveValue, effectiveValueKind);

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
        INTERNAL_PropertyStorage storage,
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
            oldExpr.OnDetach(d, dp);
        }

        (object effectiveValue, BaseValueSourceInternal effectiveValueKind) = ComputeEffectiveBaseValue(
            storage, d, dp, metadata);

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, effectiveValue, effectiveValueKind);

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
        INTERNAL_PropertyStorage storage,
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

        EffectiveValueEntry newEntry = EvaluateEffectiveValue(d, dp, newValue, BaseValueSourceInternal.Inherited);

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
        INTERNAL_PropertyStorage storage,
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
        INTERNAL_PropertyStorage storage,
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

        ValidateValue(dp, newValue, false);

        var oldEntry = storage.Entry;
        var newEntry = new EffectiveValueEntry(oldEntry);

        // Coerce to current value
        object baseValue = GetCoercionBaseValue(oldEntry);
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
        INTERNAL_PropertyStorage storage,
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
        INTERNAL_PropertyStorage storage,
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

        EvaluateExpression(newEntry, d, dp, expression);

        UpdateEffectiveValue(storage,
            d,
            dp,
            metadata,
            oldEntry,
            newEntry,
            false, // clearValue
            OperationType.Unknown);
    }

    internal static object GetEffectiveValue(EffectiveValueEntry entry)
    {
        if (entry.HasModifiers)
        {
            if (entry.IsCoercedWithCurrentValue || entry.IsCoerced)
            {
                return entry.ModifiedValue.CoercedValue;
            }
            else if (entry.IsAnimatedOverLocal && entry.IsAnimated)
            {
                return entry.ModifiedValue.AnimatedValue;
            }
            else if (entry.IsExpression)
            {
                return entry.ModifiedValue.ExpressionValue;
            }
            else
            {
                Debug.Assert(!entry.IsAnimatedOverLocal && entry.IsAnimated);
                return entry.ModifiedValue.BaseValue;
            }
        }
        else
        {
            return entry.Value;
        }
    }

    private static EffectiveValueEntry EvaluateEffectiveValue(
        DependencyObject d,
        DependencyProperty dp,
        object value,
        BaseValueSourceInternal valueSource)
    {
        var entry = new EffectiveValueEntry(valueSource);

        if (value is Expression expression)
        {
            Debug.Assert(valueSource > BaseValueSourceInternal.Inherited);

            if (expression.IsAttached)
            {
                throw new InvalidOperationException($"Cannot attach an instance of '{expression}' multiple times");
            }

            expression.OnAttach(d, dp);

            entry.Value = expression;
            EvaluateExpression(entry, d, dp, expression);
        }
        else
        {
            entry.Value = dp.IsStringType ? value?.ToString() : value;
        }

        return entry;
    }

    private static void EvaluateExpression(
        EffectiveValueEntry entry,
        DependencyObject d,
        DependencyProperty dp,
        Expression expression)
    {
        Debug.Assert(expression != null);
        Debug.Assert(entry.Value == expression || entry.ModifiedValue.BaseValue == expression);

        object exprValue = expression.GetValue(d, dp);
        if (dp.IsStringType)
        {
            exprValue = exprValue?.ToString();
        }

        entry.SetExpressionValue(exprValue);
    }

    private static (object effectiveValue, BaseValueSourceInternal kind) ComputeEffectiveBaseValue(
        INTERNAL_PropertyStorage storage,
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
        INTERNAL_PropertyStorage storage,
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        EffectiveValueEntry oldEntry,
        EffectiveValueEntry newEntry,
        bool clearValue,
        OperationType operationType)
    {
        object oldValue = GetEffectiveValue(oldEntry);

        // Coerce Value
        // We don't want to coerce the value if it's being reset to the property's default value
        if (metadata.CoerceValueCallback != null && !(clearValue && newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default))
        {
            object baseValue = GetCoercionBaseValue(newEntry);
            ProcessCoerceValue(d,
                dp,
                metadata,
                newEntry,
                null, // controlValue
                oldValue,
                baseValue,
                false);
        }

        object newValue = GetEffectiveValue(newEntry);

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

        storage.Entry = newEntry;

        bool valueChanged = storage.INTERNAL_IsVisualValueDirty || !Equals(dp, oldValue, newValue);
        if (valueChanged)
        {
            // Raise the PropertyChanged event
            OnPropertyChanged(d, dp, metadata, oldValue, newValue, operationType);
            storage.INTERNAL_IsVisualValueDirty = false;
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

    private static object GetCoercionBaseValue(EffectiveValueEntry entry)
    {
        object baseValue;
        if (!entry.HasModifiers)
        {
            baseValue = entry.Value;
        }
        else if (entry.IsCoerced)
        {
            if (entry.IsCoercedWithCurrentValue)
            {
                baseValue = entry.ModifiedValue.CoercedValue;
            }
            else if (entry.IsAnimatedOverLocal && entry.IsAnimated)
            {
                baseValue = entry.ModifiedValue.AnimatedValue;
            }
            else if (entry.IsExpression)
            {
                baseValue = entry.ModifiedValue.ExpressionValue;
            }
            else
            {
                // Only modifier is Coerced
                baseValue = entry.ModifiedValue.BaseValue;
            }
        }
        else if (entry.IsAnimatedOverLocal && entry.IsAnimated)
        {
            baseValue = entry.ModifiedValue.AnimatedValue;
        }
        else if (entry.IsExpression)
        {
            baseValue = entry.ModifiedValue.ExpressionValue;
        }
        else
        {
            Debug.Assert(!entry.IsAnimatedOverLocal && entry.IsAnimated);
            baseValue = entry.ModifiedValue.BaseValue;
        }
        return baseValue;
    }

    private static void OnPropertyChanged(
        DependencyObject d,
        DependencyProperty dp,
        PropertyMetadata metadata,
        object oldValue,
        object newValue,
        OperationType operationType)
    {
        //---------------------
        // Ensure tha the value knows in which properties it is used (this is useful for example so that a SolidColorBrush knows in which properties it is used):
        //---------------------

        if (oldValue is IHasAccessToPropertiesWhereItIsUsed2 hasAccessToProperties)
        {
            var key = new WeakDependencyObjectWrapper(d);
            var propertiesWhereUsed = hasAccessToProperties.PropertiesWhereUsed;
            if (propertiesWhereUsed.TryGetValue(key, out HashSet<DependencyProperty> val))
            {
                val.Remove(dp);
                // Remove key from dictionary if no properties left
                if (val.Count == 0)
                {
                    propertiesWhereUsed.Remove(key);
                }
            }
        }

        if ((hasAccessToProperties = newValue as IHasAccessToPropertiesWhereItIsUsed2) != null)
        {
            var key = new WeakDependencyObjectWrapper(d);
            var propertiesWhereUsed = hasAccessToProperties.PropertiesWhereUsed;
            if (propertiesWhereUsed.TryGetValue(key, out HashSet<DependencyProperty> val))
            {
                val.Add(dp);
            }
            else
            {
                propertiesWhereUsed.Add(key, new HashSet<DependencyProperty>() { dp });
            }
        }

        //---------------------
        // If the element is in the Visual Tree, update the DOM:
        //---------------------

        if (metadata != null)
        {
            ApplyCssChanges(oldValue, newValue, metadata, d); // Note: this we need to call regardless of whether the element is in the visual tree. In fact, for example, the SolidColorBrush.Color property can be used by multiple UIElements, some of which may be in the visual tree and others not.
            
            if (d is IInternalUIElement uiElement && uiElement.IsLoaded)
            {
                // Note: this we call only if the element is in the visual tree.
                metadata.MethodToUpdateDom?.Invoke(d, newValue);
                metadata.MethodToUpdateDom2?.Invoke(d, oldValue, newValue);
            }
        }

        //---------------------
        // Call the PropertyChangedCallback if any:
        //---------------------

        d.NotifyPropertyChange(new DependencyPropertyChangedEventArgs(oldValue, newValue, dp, metadata, operationType));
    }

    private static bool Equals(DependencyProperty dp, object obj1, object obj2)
    {
        if (dp.IsValueType || dp.IsStringType)
        {
            return Equals(obj1, obj2);
        }
        return ReferenceEquals(obj1, obj2);
    }

    private static void ValidateValue(DependencyProperty dp, object value, bool allowExpression)
    {
        bool isValidValue = dp.IsValidValue(value) || (allowExpression && value is Expression);

        if (!isValidValue)
        {
            throw new ArgumentException(
                $"'{value}' is not a valid value for property '{dp.Name}'.");
        }
    }

    internal static void DirtyVisualValue(INTERNAL_PropertyStorage storage)
    {
        storage.INTERNAL_IsVisualValueDirty = true;
    }

    internal static void ApplyCssChanges(object oldValue, object newValue, PropertyMetadata typeMetadata, DependencyObject sender)
    {
        if (typeMetadata.GetCSSEquivalent != null)
        {
            CSSEquivalent cssEquivalent = typeMetadata.GetCSSEquivalent(sender);
            if (cssEquivalent != null)
            {
                ApplyPropertyChanged(sender, typeMetadata, cssEquivalent, oldValue, newValue);
            }
        }

        if (typeMetadata.GetCSSEquivalents != null)
        {
            List<CSSEquivalent> cssEquivalents = typeMetadata.GetCSSEquivalents(sender);
            if (cssEquivalents != null)
            {
                foreach (CSSEquivalent cssEquivalent in cssEquivalents)
                {
                    ApplyPropertyChanged(sender, typeMetadata, cssEquivalent, oldValue, newValue);
                }
            }
        }
    }

    private static void ApplyPropertyChanged(
        DependencyObject sender,
        PropertyMetadata metadata,
        CSSEquivalent cssEquivalent,
        object oldValue,
        object newValue)
    {
        //if (cssEquivalent.ApplyWhenControlHasTemplate) //Note: this is to handle the case of a Control with a ControlTemplate (some properties must not be applied on the control itself)

        if (cssEquivalent.Name != null && cssEquivalent.Name.Count > 0 || cssEquivalent.CallbackMethod != null)
        {
            UIElement uiElement = cssEquivalent.UIElement ?? (sender as UIElement); // If no UIElement is specified, we assume that the property is intended to be applied to the instance on which the PropertyChanged has occurred.

            bool hasTemplate = (uiElement is Control) && ((Control)uiElement).HasTemplate;

            if (!hasTemplate || cssEquivalent.ApplyAlsoWhenThereIsAControlTemplate)
            {
                if (cssEquivalent.CallbackMethod != null)// && cssEquivalent.UIElement != null) //Note: I don't see when the commented part of this test could be false so I'm commenting it and we'll put it back if needed.
                {

                    //PropertyInfo propertyInfo = uiElement.GetType().GetProperty(cssEquivalent.DependencyProperty.Name);

                    //Type propertyType = propertyInfo.PropertyType;
                    //var castedValue = DynamicCast(newValue, propertyType); //Note: we put this line here because the Xaml could use a Color gotten from a StaticResource (which was therefore not converted to a SolidColorbrush by the compiler in the .g.cs file) and led to a wrong type set in a property (Color value in a property of type Brush).
                    //uiElement.SetVisualStateValue(cssEquivalent.DependencyProperty, castedValue);

                    cssEquivalent.CallbackMethod(
                        cssEquivalent.UIElement,
                        new DependencyPropertyChangedEventArgs(oldValue, newValue, cssEquivalent.DependencyProperty, metadata));
                }
                else
                {
                    if (cssEquivalent.DomElement == null && uiElement != null)
                    {
                        cssEquivalent.DomElement = uiElement.INTERNAL_OuterDomElement; // Default value
                    }
                    if (cssEquivalent.DomElement != null)
                    {
                        cssEquivalent.Value ??= static (finalInstance, value) => { return value ?? ""; }; // Default value

                        object cssValue = cssEquivalent.Value(sender, newValue);

                        if (!(cssValue is Dictionary<string, object>))
                        {
                            if (cssEquivalent.OnlyUseVelocity)
                            {
                                INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);
                            }
                            else
                            {
                                INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);
                            }
                        }
                        else
                        {
                            //Note: currently, only Color needs to set multiple values when using Velocity (which is why cssValue is a Dictionary), which is why it has a special treatment.
                            //todo: if more types arrive here, find a way to have a more generic way of handling it ?
                            if (newValue is Color)
                            {
                                Color newColor = (Color)newValue;
                                if (cssEquivalent.OnlyUseVelocity)
                                {
                                    INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(cssEquivalent.DomElement, cssEquivalent.Name, newColor.INTERNAL_ToHtmlStringForVelocity());
                                }
                                else
                                {
                                    INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, newColor.INTERNAL_ToHtmlString(1d));
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Please set the Name property of the CSSEquivalent class.");
        }
    }
}
