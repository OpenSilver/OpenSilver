//#define USEASSERT
using OpenSilver.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
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
        /// <param name="createIfNotFoud">when set to true, it forces the creation of the storage if it does not exists yet.</param>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static bool TryGetStorage(DependencyObject dependencyObject,
                                         DependencyProperty dependencyProperty,
                                         bool createIfNotFoud,
                                         out INTERNAL_PropertyStorage storage)
        {
            if (dependencyObject.INTERNAL_PropertyStorageDictionary.TryGetValue(dependencyProperty, out storage))
            {
                return true;
            }

            if (createIfNotFoud)
            {
                // Get the type metadata
                PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(dependencyObject.GetType());

                //----------------------
                // CREATE A NEW STORAGE:
                //----------------------

                storage = new INTERNAL_PropertyStorage(dependencyObject, dependencyProperty, typeMetadata);
                dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);

                //-----------------------
                // CHECK IF THE PROPERTY IS INHERITABLE:
                //-----------------------
                if (typeMetadata.Inherits)
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
        /// <param name="createIfNotFoud">when set to true, it forces the creation of the storage if it does not exists yet.</param>
        /// <param name="storage"></param>
        /// <returns></returns>
        internal static bool TryGetInheritedPropertyStorage(DependencyObject dependencyObject,
                                                            DependencyProperty dependencyProperty,
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
                // Get the type metadata (if any):
                PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(dependencyObject.GetType());

                global::System.Diagnostics.Debug.Assert(typeMetadata != null && typeMetadata.Inherits,
                                                        $"{dependencyProperty.Name} is not an inherited property.");

                // Create the storage:
                storage = new INTERNAL_PropertyStorage(dependencyObject, dependencyProperty, typeMetadata);

                //-----------------------
                // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
                //-----------------------
                //below: we check if the property is useful to the current DependencyObject, in which case we set it as its inheritedValue in "PropertyStorageDictionary"
                if (dependencyProperty.OwnerType.IsAssignableFrom(dependencyObject.GetType()))
                {
                    //-----------------------
                    // ADD THE STORAGE TO "INTERNAL_PropertyStorageDictionary"
                    //-----------------------
                    dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);
                }
                dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);
            }

            return createIfNotFoud;
        }

        internal static void SetValueCommon(INTERNAL_PropertyStorage storage,
                                            object newValue,
                                            bool coerceWithCurrentValue)
        {
            if (newValue == DependencyProperty.UnsetValue)
            {
                global::System.Diagnostics.Debug.Assert(!coerceWithCurrentValue, "Don't call SetCurrentValue with UnsetValue");
                ClearValueCommon(storage);
                return;
            }

            if (!coerceWithCurrentValue)
            {
                object newLocalValue = newValue;

                if (storage.BaseValueSourceInternal == BaseValueSourceInternal.Local)
                {
                    var currentExpr = storage.LocalValue as Expression;
                    if (currentExpr != null)
                    {
                        var newExpr = newValue as Expression;
                        if (currentExpr == newExpr)
                        {
                            global::System.Diagnostics.Debug.Assert(newExpr.IsAttached);
                            RefreshExpressionCommon(storage, newExpr, false);
                            return;
                        }

                        // if the current BindingExpression is a TwoWay binding, we don't want to remove the binding 
                        // unless we are overriding it with a new Expression.
                        if (newExpr != null || !currentExpr.CanSetValue(storage.Owner, storage.Property))
                        {
                            currentExpr.OnDetach(storage.Owner, storage.Property);
                        }
                        else
                        {
                            newLocalValue = currentExpr;
                        }
                    }
                }

                // Set the new local value
                storage.LocalValue = newLocalValue;
            }

            UpdateEffectiveValue(storage,
                                 newValue,
                                 BaseValueSourceInternal.Local,
                                 coerceWithCurrentValue, // coerceWithCurrentValue
                                 false, // coerceValue
                                 false, // clearValue
                                 true); // propagateChanges
        }

        internal static void RefreshExpressionCommon(INTERNAL_PropertyStorage storage, Expression expression, bool isInStyle)
        {
            global::System.Diagnostics.Debug.Assert(expression != null, "Expression should not be null");
            global::System.Diagnostics.Debug.Assert(storage.IsExpression || storage.IsExpressionFromStyle, "Property base value is not a BindingExpression !");

            UpdateEffectiveValue(storage,
                                 expression,
                                 isInStyle ? BaseValueSourceInternal.LocalStyle : BaseValueSourceInternal.Local,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 false, // clearValue
                                 true); // propagateChanges
        }

        internal static void ClearValueCommon(INTERNAL_PropertyStorage storage)
        {
            // Check for expression
            var currentExpr = storage.IsExpression
                              ? storage.LocalValue as Expression
                              : null;

            if (currentExpr != null)
            {
                currentExpr.OnDetach(storage.Owner, storage.Property);
            }

            // Reset local value
            storage.LocalValue = DependencyProperty.UnsetValue;

            UpdateEffectiveValue(storage,
                                 DependencyProperty.UnsetValue,
                                 BaseValueSourceInternal.Local,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 true, // clearValue
                                 true); // propagateChanges
        }

        internal static void CoerceValueCommon(INTERNAL_PropertyStorage storage)
        {
            if (storage.IsCoercedWithCurrentValue)
            {
                SetValueCommon(storage, storage.ModifiedValue.CoercedValue, true);
                return;
            }

            UpdateEffectiveValue(storage,
                                 null, //unused for coerce operation 
                                 BaseValueSourceInternal.Local,
                                 false, // coerceWithCurrentValue
                                 true, // coerceValue
                                 false, // clearValue
                                 true); // propagateChanges
        }

        internal static object GetEffectiveValue(INTERNAL_PropertyStorage storage)
        {
            if (storage.HasModifiers)
            {
                return (storage.IsCoercedWithCurrentValue || storage.IsCoerced)
                       ? storage.ModifiedValue.CoercedValue
                       : storage.ModifiedValue.ExpressionValue;
            }
            else
            {
                return storage.Value;
            }
        }

        internal static void SetAnimationValue(INTERNAL_PropertyStorage storage,
                                               object value)
        {
            if (storage.IsExpression || storage.IsExpressionFromStyle)
            {
                var currentExpr = storage.ModifiedValue.BaseValue as Expression;
                if (currentExpr != null)
                {
                    currentExpr.OnDetach(storage.Owner, storage.Property);
                }
            }

            storage.AnimatedValue = value;

            UpdateEffectiveValue(storage,
                                 value,
                                 BaseValueSourceInternal.Animated,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 value == DependencyProperty.UnsetValue, // clearValue
                                 true); // propagateChanges
        }

        #endregion

        #region Private Methods

        private static void ComputeEffectiveValue(INTERNAL_PropertyStorage storage,
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
            else if (storage.ImplicitReferenceValue != DependencyProperty.UnsetValue)
            {
                effectiveValue = storage.ImplicitReferenceValue;
                kind = BaseValueSourceInternal.ImplicitReference;
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
                effectiveValue = storage.TypeMetadata.DefaultValue;
                kind = BaseValueSourceInternal.Default;
            }
        }

        private static bool UpdateEffectiveValue(INTERNAL_PropertyStorage storage,
                                                 object newValue,
                                                 BaseValueSourceInternal newValueSource,
                                                 bool coerceWithCurrentValue,
                                                 bool coerceValue,
                                                 bool clearValue,
                                                 bool propagateChanges)
        {
            global::System.Diagnostics.Debug.Assert((coerceWithCurrentValue == coerceValue && !coerceValue) || coerceValue != coerceWithCurrentValue);

            bool isCoerceOperation = coerceValue || coerceWithCurrentValue;
            BaseValueSourceInternal oldBaseValueSource = storage.BaseValueSourceInternal;

            object oldValue;
            Expression currentExpr = null;

            // Compute new value
            object effectiveValue;
            BaseValueSourceInternal effectiveValueKind;
            if (isCoerceOperation)
            {
                // Source and base value are unchanged during coercion operation
                effectiveValue = newValue;
                effectiveValueKind = oldBaseValueSource;

                // Get old value before it gets overriden
                oldValue = GetEffectiveValue(storage);
            }
            else
            {
                ComputeEffectiveValue(storage, out effectiveValue, out effectiveValueKind);

                // Check for early exit if effective value is not impacted (if we are doing 
                // a coerce operation, we have to go through the update process)
                if (effectiveValueKind == oldBaseValueSource &&
                    newValueSource < effectiveValueKind)
                {
                    // value source remains the same.
                    // Exit if the newly set value is of lower precedence than the effective value.
                    return false;
                }

                // Get old value before it gets overriden
                oldValue = GetEffectiveValue(storage);

                currentExpr = (storage.IsExpression || storage.IsExpressionFromStyle) ? storage.ModifiedValue.BaseValue as Expression : null;

#if USEASSERT
                // If the current base value is an Expression, it should have been detached by now
                // Or is the same instance as 'effectiveValue' (this occurs when we update a property bound to a
                // BindingExpression)
                global::System.Diagnostics.Debug.Assert(currentExpr == null ||
                                                        !currentExpr.IsAttached ||
                                                        object.ReferenceEquals(currentExpr, effectiveValue), "Binding expression should be detached.");
#endif

                storage.ResetValue();

                // Update the base value source
                storage.BaseValueSourceInternal = effectiveValueKind;
            }

            object computedValue;

            if (!isCoerceOperation)
            {
                var newExpr = effectiveValue as Expression;
                if (newExpr == null)
                {
                    computedValue = storage.Property.PropertyType == typeof(string)
                                    ? effectiveValue?.ToString()
                                    : effectiveValue;
                    storage.Value = computedValue;
                }
                else
                {
#if USEASSERT
                    global::System.Diagnostics.Debug.Assert(effectiveValueKind == BaseValueSourceInternal.Local || effectiveValueKind == BaseValueSourceInternal.LocalStyle);
#endif

                    // If the new Expression is the same as the current one,
                    // the Expression is already attached
                    bool isNewBinding = !object.ReferenceEquals(currentExpr, newExpr);
                    if (isNewBinding)
                    {
                        if (newExpr.IsAttached)
                        {
                            throw new InvalidOperationException(string.Format("Cannot attach an instance of '{0}' multiple times", newExpr.GetType()));
                        }
                        newExpr.OnAttach(storage.Owner, storage.Property);
                        storage.Value = newExpr; // Set the new base value
                    }

                    if (effectiveValueKind == BaseValueSourceInternal.Local)
                    {
                        storage.SetExpressionValue(storage.TypeMetadata.DefaultValue, newExpr);
                    }
                    else
                    {
                        storage.SetExpressionFromStyleValue(storage.TypeMetadata.DefaultValue, newExpr);
                    }

                    // 1- 'isNewBinding == true' means that we are attaching a new Expression.
                    // 2- 'newValue is Expression == true' means that we are re-evaluating an Expression
                    // (usually by calling RefreshExpressionCommon)
                    // 3- Otherwise we are trying to change the value of a TwoWay binding.
                    // In that case we have to preserve the Expression (this is not the case if the first two 
                    // situations), hence the following line :
                    computedValue = isNewBinding || newValue is Expression ? newExpr.GetValue(storage.Owner, storage.Property)
                                                                           : newValue;
                    computedValue = storage.Property.PropertyType == typeof(string)
                                    ? computedValue?.ToString()
                                    : computedValue;
                    storage.ModifiedValue.ExpressionValue = computedValue;
                }
            }
            else
            {
                computedValue = coerceWithCurrentValue ? newValue : GetCoercionBaseValue(storage);
                if (coerceValue)
                {
                    storage.ResetCoercedValue();
                }
            }

            // Coerce to current value
            if (coerceWithCurrentValue)
            {
                object baseValue = GetCoercionBaseValue(storage);
                ProcessCoerceValue(storage,
                                   ref computedValue,
                                   oldValue,
                                   baseValue,
                                   true);
            }

            // Coerce Value
            // We don't want to coerce the value if it's being reset to the property's default value
            if (storage.TypeMetadata.CoerceValueCallback != null && !(clearValue && storage.FullValueSource == (FullValueSource)BaseValueSourceInternal.Default))
            {
                object baseValue = GetCoercionBaseValue(storage);
                ProcessCoerceValue(storage,
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
                storage.Owner.RemoveSelfAsInheritanceContext(oldValue, null/*storage.Property*/);
            }

            // Set new value inheritance context
            if (effectiveValueKind == BaseValueSourceInternal.Local)
            {
                // Check above
                storage.Owner.ProvideSelfAsInheritanceContext(computedValue, null/*storage.Property*/);
            }

            bool valueChanged;
            if (valueChanged = (storage.INTERNAL_IsVisualValueDirty || !ArePropertiesEqual(oldValue, computedValue, storage.Property.PropertyType)))
            {
                // Raise the PropertyChanged event
                if (!storage.TypeMetadata.Inherits || ShouldRaisePropertyChanged(storage))
                {
                    OnPropertyChanged(storage, oldValue, computedValue);
                }

                // Propagate to children if property is inherited
                if (storage.TypeMetadata.Inherits && propagateChanges)
                {
                    if (storage.Owner is FrameworkElement rootElement)
                    {
                        InheritablePropertyChangeInfo info = new InheritablePropertyChangeInfo(rootElement,
                            storage.Property,
                            oldValue, oldBaseValueSource,
                            computedValue, newValueSource);
                        TreeWalkHelper.InvalidateOnInheritablePropertyChange(rootElement, info);
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
                currentExpr.SetValue(storage.Owner, storage.Property, computedValue);
            }

			// Raise the InvalidateMeasure or InvalidateArrange
            storage.Owner.OnPropertyChanged(new DependencyPropertyChangedEventArgs(oldValue, newValue, storage.Property));
            return valueChanged;
        }


        internal static void DirtyVisualValue(INTERNAL_PropertyStorage storage)
        {
            storage.INTERNAL_IsVisualValueDirty = true;
        }

        private static void ProcessCoerceValue(INTERNAL_PropertyStorage storage,
                                               ref object newValue,
                                               object oldValue,
                                               object baseValue,
                                               bool coerceWithCurrentValue)
        {
            newValue = coerceWithCurrentValue ? newValue : storage.TypeMetadata.CoerceValueCallback(storage.Owner, newValue);

            if (!ArePropertiesEqual(newValue, baseValue, storage.Property.PropertyType))
            {
                // returning DependencyProperty.UnsetValue from a Coercion callback means "don't do the set" ...
                // or "use previous value"
                if (newValue == DependencyProperty.UnsetValue)
                {
                    newValue = oldValue;
                }

                storage.SetCoercedValue(newValue, baseValue, coerceWithCurrentValue);
            }
        }

        private static object GetCoercionBaseValue(INTERNAL_PropertyStorage storage)
        {
            object baseValue;
            if (!storage.HasModifiers)
            {
                baseValue = storage.Value;
            }
            else if (storage.IsCoerced)
            {
                if (storage.IsCoercedWithCurrentValue)
                {
                    baseValue = storage.ModifiedValue.CoercedValue;
                }
                else if (storage.IsExpression/* || storage.IsExpressionFromStyle*/)
                {
                    baseValue = storage.ModifiedValue.ExpressionValue;
                }
                else
                {
                    //global::System.Diagnostics.Debug.Assert(!storage.IsExpressionFromStyle);
                    // Only modifier is Coerced
                    baseValue = storage.ModifiedValue.BaseValue;
                }
            }
            else
            {
                global::System.Diagnostics.Debug.Assert(storage.IsExpression || storage.IsExpressionFromStyle);
                baseValue = storage.ModifiedValue.ExpressionValue;
            }
            return baseValue;
        }

        private static void OnPropertyChanged(INTERNAL_PropertyStorage storage, object oldValue, object newValue)
        {
            DependencyObject sender = storage.Owner;

            var typeMetadata = storage.TypeMetadata;

            //---------------------
            // Ensure tha the value knows in which properties it is used (this is useful for example so that a SolidColorBrush knows in which properties it is used):
            //---------------------

            if (oldValue is IHasAccessToPropertiesWhereItIsUsed)
            {
                ((IHasAccessToPropertiesWhereItIsUsed)oldValue).PropertiesWhereUsed.Remove(new KeyValuePair<DependencyObject, DependencyProperty>(sender, storage.Property));
            }

            if (newValue is IHasAccessToPropertiesWhereItIsUsed)
            {
                IHasAccessToPropertiesWhereItIsUsed newValueAsIHasAccessToPropertiesWhereItIsUsed = (IHasAccessToPropertiesWhereItIsUsed)newValue;
                // Note: it is not supposed to happen that the element is already in the list.
                newValueAsIHasAccessToPropertiesWhereItIsUsed.PropertiesWhereUsed.Add(new KeyValuePair<DependencyObject, DependencyProperty>(sender, storage.Property));
            }

            //---------------------
            // If the element is in the Visual Tree, update the DOM:
            //---------------------

            if (typeMetadata != null)
            {
                ApplyCssChanges(oldValue, newValue, typeMetadata, sender); // Note: this we need to call regardless of whether the element is in the visual tree. In fact, for example, the SolidColorBrush.Color property can be used by multiple UIElements, some of which may be in the visual tree and others not.

                if (sender is UIElement && ((UIElement)sender)._isLoaded)
                {
                    if (typeMetadata.MethodToUpdateDom != null)
                    {
                        typeMetadata.MethodToUpdateDom(sender, newValue); // Note: this we call only if the element is in the visual tree.
                    }
                }
            }

            //---------------------
            // Call the PropertyChangedCallback if any:
            //---------------------

            if (typeMetadata != null && typeMetadata.PropertyChangedCallback != null)
            {
                typeMetadata.PropertyChangedCallback(sender, new DependencyPropertyChangedEventArgs(oldValue, newValue, storage.Property));
            }

            //---------------------
            // Update bindings if any:
            //---------------------

            if (storage.PropertyListeners != null)
            {
                var listeners = storage.PropertyListeners.ToArray();
                foreach (var listener in listeners)
                {
                    listener.OnPropertyChanged(sender, new DependencyPropertyChangedEventArgs(oldValue, newValue, storage.Property));
                }
            }
        }

        private static bool ArePropertiesEqual(object obj1, object obj2, Type type)
        {
            // Note: In Silverlight (and WPF), a DependencyProperty callback is only called if one of the following condition is met :
            // - The Property type is a value type or a string and the old and new value are not equal (by value)
            // - The Property type is a reference and the old value and new value are not the same object
            // - The Property is the DataContext DependencyProperty (in this case the event 'DataContextChanged' is always raised) (Not handled in this method)
            if (type.IsValueType || type == typeof(string))
            {
                return object.Equals(obj1, obj2);
            }
            return object.ReferenceEquals(obj1, obj2);
        }

        private static bool ShouldRaisePropertyChanged(INTERNAL_PropertyStorage storage)
        {
            // Note: we only want to call "OnPropertyChanged" when the property is used by the current DependencyObject or if it is the DataContext property.
            if (!storage.Property.IsAttached)
            {
                return storage.Property.OwnerType.IsAssignableFrom(storage.Owner.GetType()) || storage.Property == FrameworkElement.DataContextProperty;
            }
            return true;
        }

#endregion

        internal static bool SetInheritedValue(INTERNAL_PropertyStorage storage, object newValue, bool recursively)
        {
            storage.InheritedValue = newValue;

            return UpdateEffectiveValue(storage,
                                        newValue,
                                        BaseValueSourceInternal.Inherited,
                                        false, // coerceWithCurrentValue
                                        false, // coerceValue
                                        newValue == DependencyProperty.UnsetValue, // clearValue
                                        recursively); // propagateChanges
        }

        internal static void SetLocalStyleValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            if (storage.BaseValueSourceInternal == BaseValueSourceInternal.LocalStyle)
            {
                var oldExpr = storage.IsExpressionFromStyle ? storage.LocalStyleValue as Expression : null;
                if (oldExpr != null)
                {
                    oldExpr.OnDetach(storage.Owner, storage.Property);
                }
            }

            storage.LocalStyleValue = newValue;

            UpdateEffectiveValue(storage,
                                 newValue,
                                 BaseValueSourceInternal.LocalStyle,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 newValue == DependencyProperty.UnsetValue, // clearValue
                                 true); // propagateChanges
        }

        internal static void SetImplicitReferenceValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            storage.ImplicitReferenceValue = newValue;

            UpdateEffectiveValue(storage,
                                 newValue,
                                 BaseValueSourceInternal.ImplicitReference,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 newValue == DependencyProperty.UnsetValue, // clearValue
                                 true); // propagateChanges
        }

        internal static void SetThemeStyleValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            if (storage.BaseValueSourceInternal == BaseValueSourceInternal.ThemeStyle)
            {
                var oldExpr = storage.IsExpressionFromStyle ? storage.ThemeStyleValue as Expression : null;
                if (oldExpr != null)
                {
                    oldExpr.OnDetach(storage.Owner, storage.Property);
                }
            }

            storage.ThemeStyleValue = newValue;

            UpdateEffectiveValue(storage,
                                 newValue,
                                 BaseValueSourceInternal.ThemeStyle,
                                 false, // coerceWithCurrentValue
                                 false, // coerceValue
                                 newValue == DependencyProperty.UnsetValue, // clearValue
                                 true); // propagateChanges
        }

        internal static void ApplyCssChanges(object oldValue, object newValue, PropertyMetadata typeMetadata, DependencyObject sender)
        {
            if (typeMetadata.GetCSSEquivalent != null)
            {
                CSSEquivalent cssEquivalent = typeMetadata.GetCSSEquivalent(sender);
                if (cssEquivalent != null)
                {
                    ApplyPropertyChanged(sender, cssEquivalent, oldValue, newValue);
                }
            }

            if (typeMetadata.GetCSSEquivalents != null)
            {
                List<CSSEquivalent> cssEquivalents = typeMetadata.GetCSSEquivalents(sender);
                if (cssEquivalents != null)
                {
                    foreach (CSSEquivalent cssEquivalent in cssEquivalents)
                    {
                        ApplyPropertyChanged(sender, cssEquivalent, oldValue, newValue);
                    }
                }
            }
        }

        private static void ApplyPropertyChanged(DependencyObject sender, CSSEquivalent cssEquivalent, object oldValue, object newValue)
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

                        cssEquivalent.CallbackMethod(cssEquivalent.UIElement, new DependencyPropertyChangedEventArgs(oldValue, newValue, cssEquivalent.DependencyProperty));
                    }
                    else
                    {
                        if (cssEquivalent.DomElement == null && uiElement != null)
                        {
                            cssEquivalent.DomElement = uiElement.INTERNAL_OuterDomElement; // Default value
                        }
                        if (cssEquivalent.DomElement != null)
                        {
                            if (newValue is ICanConvertToCSSValue)
                            {
                                cssEquivalent.Value = (finalInstance, value) => { return ((ICanConvertToCSSValue)value).ConvertToCSSValue(); };
                            }
                            if (newValue is ICanConvertToCSSValues)
                            {
                                cssEquivalent.Values = (finalInstance, value) => { return ((ICanConvertToCSSValues)value).ConvertToCSSValues(sender); };
                            }
                            if (cssEquivalent.Value == null)
                            {
                                cssEquivalent.Value = (finalInstance, value) => { return value ?? ""; }; // Default value
                            }
                            if (cssEquivalent.Values != null)
                            {
                                List<object> cssValues = cssEquivalent.Values(sender, newValue);

                                if (cssEquivalent.OnlyUseVelocity)
                                {
                                    foreach (object cssValue in cssValues)
                                    {
                                        INTERNAL_HtmlDomManager.SetDomElementStylePropertyUsingVelocity(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);
                                    }
                                }
                                else
                                {
                                    foreach (object cssValue in cssValues)
                                    {
                                        INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);
                                    }
                                }
                            }
                            else if (cssEquivalent.Value != null) //I guess we cannot have both defined
                            {
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
            }
            else
            {
                throw new InvalidOperationException("Please set the Name property of the CSSEquivalent class.");
            }
        }

        internal static IPropertyChangedListener ListenToChanged(DependencyObject target, DependencyProperty property, Action<object, IDependencyPropertyChangedEventArgs> updateSourceCallback)
        {
            INTERNAL_PropertyStorage storage;
            TryGetStorage(target, property, true/*create*/, out storage);
            List<IPropertyChangedListener> listeners = storage.PropertyListeners;
            if (listeners == null)
            {
                listeners = storage.PropertyListeners = new List<IPropertyChangedListener>(1);
            }

            PropertyChangedListener listener = new PropertyChangedListener(storage, updateSourceCallback);

            listeners.Add(listener);
            return listener;
        }
    }
}
