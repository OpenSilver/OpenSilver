

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
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI;
#endif


namespace CSHTML5.Internal
{
    internal static class INTERNAL_PropertyStore
    {
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
                                                        string.Format("{0} is not an inherited property.", dependencyProperty.Name));

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

        [Obsolete("use TryGetStorage")]
        public static INTERNAL_PropertyStorage GetStorageIfExists(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            INTERNAL_PropertyStorage storage;
            TryGetStorage(dependencyObject, dependencyProperty, false /*don't create*/, out storage);
            return storage;
        }

        [Obsolete("use TryGetStorage")]
        public static INTERNAL_PropertyStorage GetStorageOrCreateNewIfNotExists(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            INTERNAL_PropertyStorage storage;
            TryGetStorage(dependencyObject, dependencyProperty, true /*create*/, out storage);
            return storage;
        }

        [Obsolete("use TryGetInheritedPropertyStorage")]
        internal static INTERNAL_PropertyStorage GetInheritedPropertyStorageOrCreateNewIfNotFound(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            // Create the Storage if it does not already exist
            INTERNAL_PropertyStorage storage;
            if (!dependencyObject.INTERNAL_AllInheritedProperties.TryGetValue(dependencyProperty, out storage))
            {
                // Get the storage or create a new one:
                if (!dependencyObject.INTERNAL_PropertyStorageDictionary.TryGetValue(dependencyProperty, out storage))
                {
                    // Get the type metadata (if any):
                    PropertyMetadata typeMetadata = dependencyProperty.GetTypeMetaData(dependencyObject.GetType());

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
                }
                dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);                
            }

            return storage;


#region OldStuff
            /*
            // This method ensures that the two dictionaries ("INTERNAL_PropertyStorageDictionary" and "INTERNAL_AllInheritedProperties") always share the same instances of Storages (for properties that are in common):
            // This is how it works:
            // - If both in INTERNAL_PropertyStorageDictionary and in INTERNAL_AllInheritedProperties, return any one
            // - If in INTERNAL_PropertyStorageDictionary but not in INTERNAL_AllInheritedProperties, copy it to INTERNAL_AllInheritedProperties and return it
            // - If in INTERNAL_AllInheritedProperties but not in INTERNAL_PropertyStorageDictionary, AND the property belongs to the object (or one of its ancestors), copy it to INTERNAL_PropertyStorageDictionary and return it
            // - If in neither INTERNAL_PropertyStorageDictionary nor INTERNAL_AllInheritedProperties, create it in INTERNAL_AllInheritedProperties, and, if the property belongs to the object (or one of its ancestors), copy it to INTERNAL_PropertyStorageDictionary and return it

            if (dependencyObject.INTERNAL_PropertyStorageDictionary != null && dependencyObject.INTERNAL_PropertyStorageDictionary.ContainsKey(dependencyProperty))
            {
                if (dependencyObject.INTERNAL_AllInheritedProperties != null && dependencyObject.INTERNAL_AllInheritedProperties.ContainsKey(dependencyProperty))
                {
                    //--------------------------------------------
                    // The property storage is in both INTERNAL_PropertyStorageDictionary and INTERNAL_AllInheritedProperties, so we return any one:
                    //--------------------------------------------
                    return dependencyObject.INTERNAL_AllInheritedProperties[dependencyProperty];
                }
                else
                {
                    //--------------------------------------------
                    // The property storage is in INTERNAL_PropertyStorageDictionary but not in INTERNAL_AllInheritedProperties, so we copy it to INTERNAL_AllInheritedProperties and return it:
                    //--------------------------------------------
                    var storage = dependencyObject.INTERNAL_PropertyStorageDictionary[dependencyProperty];
                    if (createAndSaveNewStorageIfNotExists)
                    {
                        if (dependencyObject.INTERNAL_AllInheritedProperties == null)
                            dependencyObject.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty,INTERNAL_PropertyStorage>();
                        dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);
                    }
                    return storage;
                }
            }
            else
            {
                if (dependencyObject.INTERNAL_AllInheritedProperties != null && dependencyObject.INTERNAL_AllInheritedProperties.ContainsKey(dependencyProperty))
                {
                    //--------------------------------------------
                    // The property storage is in INTERNAL_AllInheritedProperties but not in INTERNAL_PropertyStorageDictionary, therefore, if the property belongs to the object (or one of its ancestors), we copy it to INTERNAL_PropertyStorageDictionary and we return it
                    //--------------------------------------------
                    var storage = dependencyObject.INTERNAL_AllInheritedProperties[dependencyProperty];
                    if (createAndSaveNewStorageIfNotExists && dependencyProperty.OwnerType.IsAssignableFrom(dependencyObject.GetType())) // The second condition checks if the property belongs to the object (or one of its ancestors)
                    {
                        if (dependencyObject.INTERNAL_PropertyStorageDictionary == null)
                            dependencyObject.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty,INTERNAL_PropertyStorage>();
                        dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);
                    }
                    return storage;
                }
                else
                {
                    //--------------------------------------------
                    // The property storage is in neither INTERNAL_PropertyStorageDictionary nor INTERNAL_AllInheritedProperties, therefore, we create it in INTERNAL_AllInheritedProperties, and, if the property belongs to the object (or one of its ancestors), we copy it to INTERNAL_PropertyStorageDictionary and return it:
                    //--------------------------------------------
                    var storage = new INTERNAL_PropertyStorage(dependencyObject, dependencyProperty);
                    if (createAndSaveNewStorageIfNotExists)
                    {
                        if (dependencyObject.INTERNAL_AllInheritedProperties == null)
                                dependencyObject.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty,INTERNAL_PropertyStorage>();
                        dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);

                        if (dependencyProperty.OwnerType.IsAssignableFrom(dependencyObject.GetType())) // This checks if the property belongs to the object (or one of its ancestors)
                        {
                            if (dependencyObject.INTERNAL_PropertyStorageDictionary == null)
                                dependencyObject.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty,INTERNAL_PropertyStorage>();
                            dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);
                        }
                    }
                    return storage;
                }
            }
            */
#endregion
        }

        public static object GetValue(INTERNAL_PropertyStorage storage, PropertyMetadata typeMetadata)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }
            return storage.ActualValue;
        }

        public static object ComputeActualValue(INTERNAL_PropertyStorage storage, PropertyMetadata typeMetadata, bool ignoreCoercedValue)
        {
            object actualValue;

            if (storage._isIsEnabledOrIsHitTestVisibleProperty)
            {
                if ((actualValue = storage.InheritedValue) != INTERNAL_NoValue.NoValue && actualValue != null && ((bool)actualValue) == false)
                {
                    return false;
                }
            }

            if ((!ignoreCoercedValue && (actualValue = storage.CoercedValue) != INTERNAL_NoValue.NoValue)
            || ((actualValue = storage.VisualStateValue) != INTERNAL_NoValue.NoValue)
            || (storage.ActiveLocalValue.ActiveValue == KindOfValue.Local && (actualValue = storage.Local) != INTERNAL_NoValue.NoValue)
            || (storage.ActiveLocalValue.ActiveValue == KindOfValue.Animated && (actualValue = storage.AnimationValue) != INTERNAL_NoValue.NoValue)
            || ((actualValue = storage.LocalStyleValue) != INTERNAL_NoValue.NoValue)
            || ((actualValue = storage.ImplicitStyleValue) != INTERNAL_NoValue.NoValue)
            || ((actualValue = storage.InheritedValue) != INTERNAL_NoValue.NoValue))
            {
                return actualValue;
            }
            else
            {
                // Return the default value:
                return (typeMetadata != null) ? typeMetadata.DefaultValue : null;
            }
        }

        internal static void SetSpecificValue(INTERNAL_PropertyStorage storage, KindOfValue kindOfValueToSet, object newValue)
        {
            PropertyMetadata typeMetadata = storage.TypeMetadata;
            //-----------------------
            // SET THE VALUE IN THE STORAGE:
            //-----------------------
            object oldValue;
            if (storage.SetValue(newValue, kindOfValueToSet, out oldValue)) // is greather priority
            {
                // If we are removing the value by setting it to "NoValue", we need to recompute the ActualValue:
                if (newValue == INTERNAL_NoValue.NoValue)
                {
                    // Update the ActualValue to the one with the highest priority that is not set to "INTERNAL_NoValue.NoValue"
                    newValue = ComputeActualValue(storage, typeMetadata, true);
                }

                //if the new Value changes the actual value and is coerced, we compute the coerced value and set it.
                if (typeMetadata.CoerceValueCallback != null && kindOfValueToSet != KindOfValue.Coerced)
                {
                    // Compute the coerced value
                    newValue = typeMetadata.CoerceValueCallback(storage.Owner, newValue);

                    // Remember the coerced value
                    storage.CoercedValue = newValue;
                    storage.ActualValue = newValue;
                }
                else
                {
                    storage.ActualValue = newValue;
                }

                //-----------------------
                // Reset old value inheritance context
                //-----------------------
                if (oldValue is DependencyObject oldValueDO && !(oldValueDO is FrameworkElement))
                {
                    oldValueDO.SetInheritanceContext(null);
                }

                //-----------------------
                // Set new value inheritance context
                //-----------------------
                if (newValue is DependencyObject newValueDO && !(newValueDO is FrameworkElement))
                {
                    newValueDO.SetInheritanceContext(storage.Owner);
                }

                //-----------------------
                // HANDLE SOME SPECIAL Cases where we don't want to propagate the changes
                //-----------------------
                if (storage._isIsEnabledOrIsHitTestVisibleProperty)
                {
                    object value;
                    if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue && value != null && (((bool)value) == false))
                    {
                        return;
                    }
                }

                if (!ArePropertiesEqual(oldValue, newValue, storage.Property.PropertyType))
                {
                    //-----------------------
                    // RAISE THE "PROPERTYCHANGED" EVENT:
                    //-----------------------
                    if (!typeMetadata.Inherits || ShouldRaisePropertyChanged(storage))
                    {
                        OnPropertyChanged(storage, oldValue, newValue);
                    }

                    //-----------------------
                    // PROPAGATE TO CHILDREN (INHERITED PROPERTIES ONLY):
                    //-----------------------
                    if (typeMetadata.Inherits)
                    {
                        CascadeInheritedPropertyToChildren(storage, newValue);
                    }
                }
            }
        }

        internal static void SetInheritedValue(INTERNAL_PropertyStorage storage, object newValue, bool recursively)
        {
            PropertyMetadata typeMetadata = storage.TypeMetadata;
            //-----------------------
            // CHECK THAT A LOCAL OR STYLE VALUE DOES NOT EXIST 
            // (otherwise the local/style values superset the inherited value, 
            // and we don't want to raise PropertyChanged, nor continue the recursion)
            //-----------------------
            object oldValue;
            if (storage.SetInheritedValue(newValue, out oldValue)) // Actual value is impacted
            {
                newValue = ComputeActualValue(storage, typeMetadata, true);
                if (typeMetadata != null && typeMetadata.CoerceValueCallback != null)
                {
                    // Compute the coerced value:
                    newValue = typeMetadata.CoerceValueCallback(storage.Owner, newValue);

                    // Remember the coerced value:
                    storage.CoercedValue = newValue;
                    storage.ActualValue = newValue;
                }
                else
                {
                    storage.ActualValue = newValue;
                }

                //-----------------------
                // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
                //-----------------------
                //we only do the following inside the "if" because otherwise, the children's inherithed property would not change anyway
                if (!ArePropertiesEqual(oldValue, newValue, storage.Property.PropertyType))
                {
                    if (ShouldRaisePropertyChanged(storage))
                    {
                        OnPropertyChanged(storage, oldValue, newValue);
                    }
                    if (recursively)
                    {
                        CascadeInheritedPropertyToChildren(storage, newValue);
                    }
                }
            }
            else if (storage._isIsEnabledOrIsHitTestVisibleProperty) //todo: if we decide to make coercion possible on IsHitTestVisible or IsEnabled, change the "newValue" below into "coercedNewValue" (probably)
            {
                if ((bool)newValue)
                {
                    storage.ActualValueIsDirty = true; //The actual value is Dirty because it didn't take into consideration the values that usually have priority over the inherited value (i.e: local) and since the newValue is "true", it now needs to take them into consideration.
                    INTERNAL_PropertyStorage otherStorage;
                    if (TryGetStorage(storage.Owner, storage.Property, false/*don't create*/, out otherStorage))
                    {
                        newValue = otherStorage.ActualValue;
                    }
                    else
                    {
                        newValue = typeMetadata.DefaultValue;
                    }
                }
                if (!object.Equals(oldValue, newValue)) // note: No need to call 'ArePropertiesEqual' since we know we are dealing with Booleans.
                {
                    storage.ActualValue = newValue; // Make sure Storage.ActualValue is up to date.
                    if (ShouldRaisePropertyChanged(storage))
                    {
                        OnPropertyChanged(storage, oldValue, newValue);
                    }
                    if (recursively)
                    {
                        CascadeInheritedPropertyToChildren(storage, newValue);
                    }
                }
            }
        }
        
        internal static bool ArePropertiesEqual(object obj1, object obj2, Type type)
        {
            // Note: In Silverlight, a DependencyProperty callback is only called if one of the following condition is met :
            // - The Property type is a value type or a string and the old and new value are not equal (by value)
            // - The Property type is a reference and the old value and new value are not the same object
            // - The Property is the DataContext DependencyProperty (in this case the event 'DataContextChanged' is always raised) (Not handled in this method)
            if (type.IsValueType || type == typeof(string))
            {
                return object.Equals(obj1, obj2);
            }
            return object.ReferenceEquals(obj1, obj2);
        }

        internal static bool ShouldRaisePropertyChanged(INTERNAL_PropertyStorage storage)
        {
            // Note: we only want to call "OnPropertyChanged" when the property is used by the current DependencyObject or if it is the DataContext property.
            if (!storage.Property.IsAttached)
            {
                return storage.Property.OwnerType.IsAssignableFrom(storage.Owner.GetType()) || storage.Property == FrameworkElement.DataContextProperty;
            }
            return true;
        }

        internal static void SetLocalStyleValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            SetSpecificValue(storage, KindOfValue.LocalStyle, newValue);
        }

        internal static void CascadeInheritedPropertyToChildren(INTERNAL_PropertyStorage storage, object newValue)
        {
            DependencyObject dependencyObject = storage.Owner;

            // Set Inherited Value on the children:
            if (dependencyObject is UIElement)
            {
                UIElement parent = (UIElement)dependencyObject;
                if (parent.INTERNAL_VisualChildrenInformation != null)
                {
                    foreach (UIElement child in parent.INTERNAL_VisualChildrenInformation.Keys) //all the children should in there
                    {
                        child.SetInheritedValue(storage.Property, newValue, true);
                    }
                }
            }
        }

        internal static void ResetInheritedValue(INTERNAL_PropertyStorage storage)
        {
            storage.InheritedValue = INTERNAL_NoValue.NoValue; //this only occurs when we detach an item so there is no need to care about the propertyChanged event.
            storage.ActualValueIsDirty = true;
        }

        internal static void ResetLocalStyleValue(INTERNAL_PropertyStorage storage)
        {
            ResetLocalStyleValue(storage, false);
        }

        internal static void ResetLocalStyleValue(INTERNAL_PropertyStorage storage, bool dontRefresh)
        {
            if (dontRefresh)
            {
                storage.LocalStyleValue = INTERNAL_NoValue.NoValue;
                storage.ActualValueIsDirty = true;
            }
            else
            {
                // we need to refresh the actual value if we switch from an old style to a new one. for instance:
                // old style :
                // <Style>
                //   <Style.Setters>
                //     <Setter Property="Property1" Value="Value1">
                //   <Style.Setters>
                // <Style>
                // new style :
                // <Style>
                //   <Style.Setters>
                //     <Setter Property="Property2" Value="Value2">
                //   <Style.Setters>
                // <Style>
                // if we switch from old style to new style we need to force the refresh or the Property1 property would still have
                // its value set to Value1.
                SetLocalStyleValue(storage, INTERNAL_NoValue.NoValue);
            }
        }


        internal static void OnPropertyChanged(INTERNAL_PropertyStorage storage, object oldValue, object newValue) // "raiseEvenIfNewValueIsSameAsOldValue" is used to force refresh.
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
                foreach (IPropertyChangedListener listener in storage.PropertyListeners)
                {
                    listener.OnPropertyChanged(sender, new DependencyPropertyChangedEventArgs(oldValue, newValue, storage.Property));
                }
            }
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

        static void ApplyPropertyChanged(DependencyObject sender, CSSEquivalent cssEquivalent, object oldValue, object newValue)
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
                listeners = storage.PropertyListeners = new List<IPropertyChangedListener>();
            }

            PropertyChangedListener listener = new PropertyChangedListener(storage, updateSourceCallback);

            listeners.Add(listener);
            return listener;
        }

        internal static void CoerceCurrentValue(INTERNAL_PropertyStorage storage)
        {
            var typeMetadata = storage.TypeMetadata;
            if (typeMetadata == null || typeMetadata.CoerceValueCallback == null)
            {
                return; //we should not arrive here in this case but we make sure to not do anything should that happen.
            }
            object oldValue = storage.Local == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.Local;
            object currentValue = ComputeActualValue(storage, typeMetadata, true); //Note: we do not need to know where this value comes from (Local, VisualState, etc.) since calling this method means that it has not been modified and we only need to update the coerced value.
            object coercedNewValue = typeMetadata.CoerceValueCallback(storage.Owner, currentValue);
            SetSpecificValue(storage, KindOfValue.Coerced, coercedNewValue);
        }
    }
}
