
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

//#define USEASSERT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_PropertyStore
    {
        #region Internal Methods

        /// <summary>
        /// Attempt to get a Property Storage
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="dependencyProperty"></param>
        /// <param name="metadata"></param>
        /// <param name="createIfNotFoud">when set to true, it forces the creation of the storage if it does not exists yet.</param>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static bool TryGetStorage(DependencyObject dependencyObject,
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
        internal static bool TryGetInheritedPropertyStorage(DependencyObject dependencyObject,
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

        internal static void SetValueCommon(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object newValue,
            bool coerceWithCurrentValue)
        {
            if (newValue == DependencyProperty.UnsetValue)
            {
                Debug.Assert(!coerceWithCurrentValue, "Don't call SetCurrentValue with UnsetValue");
                ClearValueCommon(storage, depObj, dp, metadata);
                return;
            }

            ValidateValue(dp, newValue);
            
            if (!coerceWithCurrentValue)
            {
                object newLocalValue = newValue;

                if (storage.Entry.IsExpression)
                {
                    if (storage.LocalValue is Expression currentExpr)
                    {
                        var newExpr = newValue as Expression;
                        if (currentExpr == newExpr)
                        {
                            Debug.Assert(newExpr.IsAttached);
                            RefreshExpressionCommon(storage, depObj, dp, metadata, newExpr, false);
                            return;
                        }

                        // if the current BindingExpression is a TwoWay binding, we don't want to remove the binding 
                        // unless we are overriding it with a new Expression.
                        if (newExpr != null || !currentExpr.CanSetValue(depObj, dp))
                        {
                            currentExpr.OnDetach(depObj, dp);
                        }
                        else
                        {
                            newLocalValue = currentExpr;
                        }
                    }
                }
                else if (storage.Entry.IsExpressionFromStyle)
                {
                    ((Expression)storage.Entry.ModifiedValue.BaseValue).OnDetach(depObj, dp);
                }

                // Set the new local value
                storage.LocalValue = newLocalValue;
            }

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                newValue,
                BaseValueSourceInternal.Local,
                coerceWithCurrentValue, // coerceWithCurrentValue
                false, // coerceValue
                false, // clearValue
                true); // propagateChanges
        }

        internal static void RefreshExpressionCommon(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            Expression expression,
            bool isInStyle)
        {
            Debug.Assert(expression != null, "Expression should not be null");
            Debug.Assert(storage.Entry.IsExpression || storage.Entry.IsExpressionFromStyle, "Property base value is not a BindingExpression !");

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                expression,
                isInStyle ? BaseValueSourceInternal.LocalStyle : BaseValueSourceInternal.Local,
                false, // coerceWithCurrentValue
                false, // coerceValue
                false, // clearValue
                true); // propagateChanges
        }

        internal static void ClearValueCommon(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata)
        {
            // Check for expression
            var currentExpr = storage.Entry.IsExpression
                              ? storage.LocalValue as Expression
                              : null;

            currentExpr?.OnDetach(depObj, dp);

            // Reset local value
            storage.LocalValue = DependencyProperty.UnsetValue;

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                DependencyProperty.UnsetValue,
                BaseValueSourceInternal.Local,
                false, // coerceWithCurrentValue
                false, // coerceValue
                true, // clearValue
                true); // propagateChanges
        }

        internal static void CoerceValueCommon(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata)
        {
            if (storage.Entry.IsCoercedWithCurrentValue)
            {
                SetValueCommon(storage,
                    depObj,
                    dp,
                    metadata,
                    storage.Entry.ModifiedValue.CoercedValue,
                    true);

                return;
            }

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                null, //unused for coerce operation 
                BaseValueSourceInternal.Local,
                false, // coerceWithCurrentValue
                true, // coerceValue
                false, // clearValue
                true); // propagateChanges
        }

        internal static object GetEffectiveValue(EffectiveValueEntry entry)
        {
            if (entry.HasModifiers)
            {
                return (entry.IsCoercedWithCurrentValue || entry.IsCoerced)
                       ? entry.ModifiedValue.CoercedValue
                       : entry.ModifiedValue.ExpressionValue;
            }
            else
            {
                return entry.Value;
            }
        }

        internal static void SetAnimationValue(
            INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object value)
        {
            bool clearValue = value == DependencyProperty.UnsetValue;

            if (!clearValue)
            {
                ValidateValue(dp, value);
            }

            if (storage.Entry.IsExpression || storage.Entry.IsExpressionFromStyle)
            {
                if (storage.Entry.ModifiedValue.BaseValue is Expression currentExpr)
                {
                    currentExpr.OnDetach(depObj, dp);
                }
            }

            storage.AnimatedValue = value;

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                value,
                BaseValueSourceInternal.Animated,
                false, // coerceWithCurrentValue
                false, // coerceValue
                clearValue, // clearValue
                true); // propagateChanges
        }

        #endregion

        #region Private Methods

        private static void ComputeEffectiveValue(INTERNAL_PropertyStorage storage,
            DependencyObject owner,
            DependencyProperty dp,
            PropertyMetadata metadata,
            out object effectiveValue,
            out BaseValueSourceInternal kind)
        {
            if (!storage.IsAnimatedOverLocal &&
                 storage.LocalValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.LocalValue;
                kind = BaseValueSourceInternal.Local;
            }
            else if (storage.IsAnimatedOverLocal &&
                     storage.AnimatedValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.AnimatedValue;
                kind = BaseValueSourceInternal.Animated;
            }
            else if (storage.LocalStyleValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.LocalStyleValue;
                kind = BaseValueSourceInternal.LocalStyle;
            }
            else if (storage.ThemeStyleValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.ThemeStyleValue;
                kind = BaseValueSourceInternal.ThemeStyle;
            }
            else if (storage.InheritedValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.InheritedValue;
                kind = BaseValueSourceInternal.Inherited;
            }
            else // Property default value
            {
                effectiveValue = metadata.GetDefaultValue(owner, dp);
                kind = BaseValueSourceInternal.Default;
            }
        }

        private static bool UpdateEffectiveValue(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object newValue,
            BaseValueSourceInternal newValueSource,
            bool coerceWithCurrentValue,
            bool coerceValue,
            bool clearValue,
            bool propagateChanges)
        {
            Debug.Assert((coerceWithCurrentValue == coerceValue && !coerceValue) || coerceValue != coerceWithCurrentValue);

            bool isCoerceOperation = coerceValue || coerceWithCurrentValue;
            EffectiveValueEntry newEntry;
            EffectiveValueEntry oldEntry = storage.Entry;
            BaseValueSourceInternal oldBaseValueSource = oldEntry.BaseValueSourceInternal;

            Expression currentExpr = null;

            // Compute new value
            object effectiveValue;
            BaseValueSourceInternal effectiveValueKind;
            if (isCoerceOperation)
            {
                // Source and base value are unchanged during coercion operation
                effectiveValue = newValue;
                effectiveValueKind = oldBaseValueSource;

                newEntry = new EffectiveValueEntry(oldEntry.BaseValueSourceInternal);
                if (!oldEntry.HasModifiers)
                {
                    newEntry.Value = oldEntry.Value;
                }
                else
                {
                    ModifiedValue modifiedValue = oldEntry.ModifiedValue;
                    object baseValue = modifiedValue.BaseValue;
                    newEntry.Value = baseValue;
                    if (oldEntry.IsExpression)
                    {
                        newEntry.SetExpressionValue(modifiedValue.ExpressionValue, baseValue);
                    }
                    else if (oldEntry.IsExpressionFromStyle)
                    {
                        newEntry.SetExpressionFromStyleValue(modifiedValue.ExpressionValue, baseValue);
                    }
                }
            }
            else
            {
                ComputeEffectiveValue(storage, depObj, dp, metadata, out effectiveValue, out effectiveValueKind);

                // Check for early exit if effective value is not impacted (if we are doing 
                // a coerce operation, we have to go through the update process)
                if (effectiveValueKind == oldBaseValueSource &&
                    (newValueSource < effectiveValueKind || clearValue))
                {
                    // value source remains the same.
                    // Exit if the newly set value is of lower precedence than the effective value.
                    return false;
                }

                currentExpr = (oldEntry.IsExpression || oldEntry.IsExpressionFromStyle) ? oldEntry.ModifiedValue.BaseValue as Expression : null;

#if USEASSERT
                // If the current base value is an Expression, it should have been detached by now
                // Or is the same instance as 'effectiveValue' (this occurs when we update a property bound to a
                // BindingExpression)
                Debug.Assert(currentExpr == null ||
                    !currentExpr.IsAttached ||
                    object.ReferenceEquals(currentExpr, effectiveValue), "Binding expression should be detached.");
#endif
                newEntry = new EffectiveValueEntry(effectiveValueKind);
            }

            object computedValue;
            object oldValue = GetEffectiveValue(oldEntry);

            if (!isCoerceOperation)
            {
                if (effectiveValue is not Expression newExpr)
                {
                    computedValue = dp.IsStringType ? effectiveValue?.ToString() : effectiveValue;
                    newEntry.Value = computedValue;
                }
                else
                {
#if USEASSERT
                    Debug.Assert(effectiveValueKind == BaseValueSourceInternal.Local || effectiveValueKind == BaseValueSourceInternal.LocalStyle);
#endif

                    // If the new Expression is the same as the current one,
                    // the Expression is already attached
                    bool isNewBinding = currentExpr != newExpr;
                    if (isNewBinding)
                    {
                        if (newExpr.IsAttached)
                        {
                            throw new InvalidOperationException(string.Format("Cannot attach an instance of '{0}' multiple times", newExpr.GetType()));
                        }

                        newExpr.OnAttach(depObj, dp);
                    }

                    newEntry.Value = newExpr; // Set the new base value

                    if (effectiveValueKind == BaseValueSourceInternal.Local)
                    {
                        newEntry.SetExpressionValue(null, newExpr);
                    }
                    else
                    {
                        newEntry.SetExpressionFromStyleValue(null, newExpr);
                    }

                    // 1- 'isNewBinding == true' means that we are attaching a new Expression.
                    // 2- 'newValue is Expression == true' means that we are re-evaluating an Expression
                    // (usually by calling RefreshExpressionCommon)
                    // 3- Otherwise we are trying to change the value of a TwoWay binding.
                    // In that case we have to preserve the Expression (this is not the case if the first two 
                    // situations), hence the following line :
                    computedValue = isNewBinding || newValue is Expression ? newExpr.GetValue(depObj, dp)
                                                                           : newValue;
                    computedValue = dp.IsStringType ? computedValue?.ToString() : computedValue;
                    newEntry.ModifiedValue.ExpressionValue = computedValue;
                }
            }
            else
            {
                computedValue = coerceWithCurrentValue ? newValue : GetCoercionBaseValue(newEntry);
                if (coerceValue)
                {
                    newEntry.ResetCoercedValue();
                }
            }

            // Coerce to current value
            if (coerceWithCurrentValue)
            {
                object baseValue = GetCoercionBaseValue(newEntry);
                ProcessCoerceValue(depObj,
                    dp,
                    metadata,
                    newEntry,
                    ref computedValue,
                    oldValue,
                    baseValue,
                    true);
            }

            // Coerce Value
            // We don't want to coerce the value if it's being reset to the property's default value
            if (metadata.CoerceValueCallback != null && !(clearValue && newEntry.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default))
            {
                object baseValue = GetCoercionBaseValue(newEntry);
                ProcessCoerceValue(depObj,
                    dp,
                    metadata,
                    newEntry,
                    ref computedValue,
                    oldValue,
                    baseValue,
                    false);
            }

            // Reset old value inheritance context
            if (oldBaseValueSource == BaseValueSourceInternal.Local)
            {
                // Notes:
                // - Inheritance context is only handled by local value
                // - We use null instead of the actual DependencyProperty
                // as the parameter is ignored in the current implentation.
                depObj.RemoveSelfAsInheritanceContext(oldValue, null/*storage.Property*/);
            }

            // Set new value inheritance context
            if (effectiveValueKind == BaseValueSourceInternal.Local)
            {
                // Check above
                depObj.ProvideSelfAsInheritanceContext(computedValue, null/*storage.Property*/);
            }

            storage.Entry = newEntry;

            bool valueChanged;
            if (valueChanged = (storage.INTERNAL_IsVisualValueDirty || !Equals(dp, oldValue, computedValue)))
            {
                // Raise the PropertyChanged event
                OnPropertyChanged(depObj, dp, metadata, oldValue, computedValue);

                // Propagate to children if property is inherited
                if (metadata.Inherits)
                {
                    if (depObj is IInternalFrameworkElement rootElement)
                    {
                        InheritablePropertyChangeInfo info = new InheritablePropertyChangeInfo(rootElement.AsDependencyObject(),
                            dp,
                            oldValue, oldBaseValueSource,
                            computedValue, newValueSource);

                        if (propagateChanges)
                        {
                            TreeWalkHelper.InvalidateOnInheritablePropertyChange(rootElement, info, true);
                        }

                        rootElement.OnInheritedPropertyChanged(info);
                    }
                }
                storage.INTERNAL_IsVisualValueDirty = false;
            }

            // Update the source of the Binding, in case the previous value
            // of a property was a Binding and the Mode was "TwoWay":
            // Note: we know that oldBindingExpression.IsUpdating is false
            // because oldBindingExpression is only set in that case (otherwise,
            // it is null).
            if (currentExpr != null) 
            {
                currentExpr.SetValue(depObj, dp, computedValue);
            }

            return valueChanged;
        }


        internal static void DirtyVisualValue(INTERNAL_PropertyStorage storage)
        {
            storage.INTERNAL_IsVisualValueDirty = true;
        }

        private static void ProcessCoerceValue(DependencyObject target,
            DependencyProperty dp,
            PropertyMetadata metadata,
            EffectiveValueEntry entry,
            ref object newValue,
            object oldValue,
            object baseValue,
            bool coerceWithCurrentValue)
        {
            newValue = coerceWithCurrentValue ? newValue : metadata.CoerceValueCallback(target, newValue);

            if (!Equals(dp, newValue, baseValue))
            {
                // returning DependencyProperty.UnsetValue from a Coercion callback means "don't do the set" ...
                // or "use previous value"
                if (newValue == DependencyProperty.UnsetValue)
                {
                    newValue = oldValue;
                }

                entry.SetCoercedValue(newValue, baseValue, coerceWithCurrentValue);
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
                else if (entry.IsExpression/* || entry.IsExpressionFromStyle*/)
                {
                    baseValue = entry.ModifiedValue.ExpressionValue;
                }
                else
                {
                    //global::System.Diagnostics.Debug.Assert(!entry.IsExpressionFromStyle);
                    // Only modifier is Coerced
                    baseValue = entry.ModifiedValue.BaseValue;
                }
            }
            else
            {
                Debug.Assert(entry.IsExpression || entry.IsExpressionFromStyle);
                baseValue = entry.ModifiedValue.ExpressionValue;
            }
            return baseValue;
        }

        private static void OnPropertyChanged(DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object oldValue,
            object newValue)
        {
            //---------------------
            // Ensure tha the value knows in which properties it is used (this is useful for example so that a SolidColorBrush knows in which properties it is used):
            //---------------------

            if (oldValue is IHasAccessToPropertiesWhereItIsUsed2 hasAccessToProperties)
            {
                var key = new WeakDependencyObjectWrapper(depObj);
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
                var key = new WeakDependencyObjectWrapper(depObj);
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
                ApplyCssChanges(oldValue, newValue, metadata, depObj); // Note: this we need to call regardless of whether the element is in the visual tree. In fact, for example, the SolidColorBrush.Color property can be used by multiple UIElements, some of which may be in the visual tree and others not.

                if (depObj is IInternalUIElement uiElement && uiElement.IsLoaded)
                {
                    // Note: this we call only if the element is in the visual tree.
                    metadata.MethodToUpdateDom?.Invoke(depObj, newValue); 
                    metadata.MethodToUpdateDom2?.Invoke(depObj, oldValue, newValue);
                }
            }

            //---------------------
            // Call the PropertyChangedCallback if any:
            //---------------------

            depObj.NotifyPropertyChange(new DependencyPropertyChangedEventArgs(oldValue, newValue, dp, metadata));
        }

        private static bool Equals(DependencyProperty dp, object obj1, object obj2)
        {
            if (dp.IsValueType || dp.IsStringType)
            {
                return Equals(obj1, obj2);
            }
            return ReferenceEquals(obj1, obj2);
        }

#endregion

        internal static bool SetInheritedValue(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object newValue,
            bool recursively)
        {
            storage.InheritedValue = newValue;

            return UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                newValue,
                BaseValueSourceInternal.Inherited,
                false, // coerceWithCurrentValue
                false, // coerceValue
                newValue == DependencyProperty.UnsetValue, // clearValue
                recursively); // propagateChanges
        }

        internal static void SetLocalStyleValue(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object newValue)
        {
            if (storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.LocalStyle)
            {
                var oldExpr = storage.Entry.IsExpressionFromStyle ? storage.LocalStyleValue as Expression : null;
                oldExpr?.OnDetach(depObj, dp);
            }

            storage.LocalStyleValue = newValue;

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                newValue,
                BaseValueSourceInternal.LocalStyle,
                false, // coerceWithCurrentValue
                false, // coerceValue
                newValue == DependencyProperty.UnsetValue, // clearValue
                true); // propagateChanges
        }

        internal static void SetThemeStyleValue(INTERNAL_PropertyStorage storage,
            DependencyObject depObj,
            DependencyProperty dp,
            PropertyMetadata metadata,
            object newValue)
        {
            if (storage.Entry.BaseValueSourceInternal == BaseValueSourceInternal.ThemeStyle)
            {
                var oldExpr = storage.Entry.IsExpressionFromStyle ? storage.ThemeStyleValue as Expression : null;
                oldExpr?.OnDetach(depObj, dp);
            }

            storage.ThemeStyleValue = newValue;

            UpdateEffectiveValue(storage,
                depObj,
                dp,
                metadata,
                newValue,
                BaseValueSourceInternal.ThemeStyle,
                false, // coerceWithCurrentValue
                false, // coerceValue
                newValue == DependencyProperty.UnsetValue, // clearValue
                true); // propagateChanges
        }

        private static void ValidateValue(DependencyProperty dp, object value)
        {
            bool isValidValue = dp.IsValidValue(value) || value is Expression;

            if (!isValidValue)
            {
                throw new ArgumentException(
                    string.Format("'{0}' is not a valid value for property '{1}'.", value, dp.Name));
            }
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
}
