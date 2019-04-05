
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public static INTERNAL_PropertyStorage GetStorage(DependencyObject dependencyObject, DependencyProperty dependencyProperty, bool createAndSaveNewStorageIfNotExists = false)
        {
            // Create the dictionary of it does not already exist:
            if (dependencyObject.INTERNAL_PropertyStorageDictionary == null)
                dependencyObject.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();

            // Create the Storage if it does not already exist, and if "createAndSaveNewStorageIfNotExists" is True:
            INTERNAL_PropertyStorage storage;
            if (!dependencyObject.INTERNAL_PropertyStorageDictionary.TryGetValue(dependencyProperty, out storage))
            {
                storage = new INTERNAL_PropertyStorage(dependencyObject, dependencyProperty);
                if (createAndSaveNewStorageIfNotExists)
                {
                    dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);

                    //-----------------------
                    // CHECK IF THE PROPERTY IS INHERITABLE:
                    //-----------------------
                    PropertyMetadata typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
                    if (typeMetadata != null && typeMetadata.Inherits)
                    {
                        //-----------------------
                        // ADD THE STORAGE TO "INTERNAL_AllInheritedProperties" IF IT IS NOT ALREADY THERE:
                        //-----------------------
                        if (dependencyObject.INTERNAL_AllInheritedProperties == null)
                            dependencyObject.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();

                        if (!dependencyObject.INTERNAL_AllInheritedProperties.ContainsKey(dependencyProperty))
                            dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);
                    }
                }
            }

            return storage;
        }

        internal static INTERNAL_PropertyStorage GetInheritedPropertyStorage(DependencyObject dependencyObject, DependencyProperty dependencyProperty, bool createAndSaveNewStorageIfNotExists)
        {
            // Create the dictionary of it does not already exist:
            if (dependencyObject.INTERNAL_AllInheritedProperties == null)
                dependencyObject.INTERNAL_AllInheritedProperties = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();

            // Create the Storage if it does not already exist, and if "createAndSaveNewStorageIfNotExists" is True:
            INTERNAL_PropertyStorage storage;
            if (!dependencyObject.INTERNAL_AllInheritedProperties.TryGetValue(dependencyProperty, out storage))
            {
                if (dependencyObject.INTERNAL_PropertyStorageDictionary.ContainsKey(dependencyProperty))
                {
                    storage = dependencyObject.INTERNAL_PropertyStorageDictionary[dependencyProperty];
                }
                else
                {
                    storage = new INTERNAL_PropertyStorage(dependencyObject, dependencyProperty);
                }
                if (createAndSaveNewStorageIfNotExists)
                {
                    dependencyObject.INTERNAL_AllInheritedProperties.Add(dependencyProperty, storage);

                    //-----------------------
                    // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
                    //-----------------------
                    //below: we check if the property is useful to the current DependencyObject, in which case we set it as its inheritedValue in "PropertyStorageDictionary"
                    if (dependencyProperty.OwnerType.IsAssignableFrom(dependencyObject.GetType()))
                    {
                        //-----------------------
                        // ADD THE STORAGE TO "INTERNAL_PropertyStorageDictionary" IF IT IS NOT ALREADY THERE:
                        //-----------------------
                        if (dependencyObject.INTERNAL_PropertyStorageDictionary == null)
                            dependencyObject.INTERNAL_PropertyStorageDictionary = new Dictionary<DependencyProperty, INTERNAL_PropertyStorage>();

                        if (!dependencyObject.INTERNAL_PropertyStorageDictionary.ContainsKey(dependencyProperty))
                            dependencyObject.INTERNAL_PropertyStorageDictionary.Add(dependencyProperty, storage);
                    }
                }
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

        public static void SetVisualStateValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            SetSpecificValue(storage, KindOfValue.VisualState, newValue, null);
        }

        public static void SetLocalValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            SetSpecificValue(storage, KindOfValue.Local, newValue, null);
        }

        static void RaisePropertyChangedAndCascadeToChildren(INTERNAL_PropertyStorage storage, object oldValue, object newValue, PropertyMetadata typeMetadata)
        {
            //-----------------------
            // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
            //-----------------------
            if (ShouldRaisePropertyChanged(storage))
            {
                // Raise Property Changed:
                OnPropertyChanged(storage, oldValue, newValue, typeMetadata: typeMetadata);
            }

            //-----------------------
            // CHECK IF THE PROPERTY IS INHERITABLE:
            //-----------------------
            if (typeMetadata == null)
                typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
            if (typeMetadata != null && typeMetadata.Inherits)
            {
                //-----------------------
                // PROPAGATE TO CHILDREN:
                //-----------------------
                CascadeInheritedPropertyToChildren(storage, newValue);

                HandleSpecialPropertiesThatShouldInheritDataContext(storage, newValue);
            }
        }

        //about the todo below, make sure we don't want a specific value to purposely set it at null
        public static object GetValue(INTERNAL_PropertyStorage storage, PropertyMetadata typeMetadata = null) //todo: remove the "(value = storage.Local) != null" because the user might purposely set it at null OR define a specific value to purposely set something at null
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            object value;
            if (storage._isIsEnabledOrIsHitTestVisibleProperty)
            {
                if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue && value != null && ((bool)value) == false)
                {
#if PERFSTAT
                    Performance.Counter("INTERNAL_PropertyStore.GetValue", t);
#endif
                    return false;
                }
            }

            //todo: remove all comparisons with "null" to leave only the comparisons with "NoValue":
            if ((value = storage.CoercedValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.VisualStateValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.Local) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.LocalStyleValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.ImplicitStyleValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue) { }
            else
            {
                if (typeMetadata == null)
                    typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
#if PERFSTAT
                Performance.Counter("INTERNAL_PropertyStore.GetValue", t);
#endif
                return (typeMetadata != null ? typeMetadata.DefaultValue : null);
            }

#if PERFSTAT
            Performance.Counter("INTERNAL_PropertyStore.GetValue", t);
#endif
            return value;
        }

        public static object GetValueWithoutCoerce(INTERNAL_PropertyStorage storage, PropertyMetadata typeMetadata) //todo: remove the "(value = storage.Local) != null" because the user might purposely set it at null OR define a specific value to purposely set something at null
        {
            object value;
#if PERFSTAT
            var t = Performance.now();
#endif
            if (storage._isIsEnabledOrIsHitTestVisibleProperty)
            {
                if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue && value != null && ((bool)value) == false)
                {
#if PERFSTAT
                    Performance.Counter("INTERNAL_PropertyStore.GetValueWithoutCoerce", t);
#endif
                    return false;
                }
            }

            //todo: remove all comparisons with "null" to leave only the comparisons with "NoValue":
            if ((value = storage.VisualStateValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.Local) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.LocalStyleValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.ImplicitStyleValue) != INTERNAL_NoValue.NoValue) { }
            else if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue) { }
            else
            {
                if (typeMetadata == null)
                    typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
#if PERFSTAT
                Performance.Counter("INTERNAL_PropertyStore.GetValueWithoutCoerce", t);
#endif
                return (typeMetadata != null ? typeMetadata.DefaultValue : null);
            }
#if PERFSTAT
            Performance.Counter("INTERNAL_PropertyStore.GetValueWithoutCoerce", t);
#endif
            return value;
        }

        static bool DoesSpecificValueImpactActualValue(INTERNAL_PropertyStorage storage, KindOfValue kind)
        {
            object value;
            //Note: in KindOfValue, the value attributed to the enum values corresponds to their priority rank so the lower, the more priority.
            if ((kind <= KindOfValue.VisualState || ((value = storage.VisualStateValue) == INTERNAL_NoValue.NoValue)) //means "kind has a higher priority tha VisualState or there is no VisualState value
                && (kind <= KindOfValue.Local || ((value = storage.Local) == INTERNAL_NoValue.NoValue))
                && (kind <= KindOfValue.LocalStyle || ((value = storage.LocalStyleValue) == INTERNAL_NoValue.NoValue))
                && (kind <= KindOfValue.ImplicitStyle || ((value = storage.ImplicitStyleValue) == INTERNAL_NoValue.NoValue))
                && (kind <= KindOfValue.Inherited || ((value = storage.InheritedValue) == INTERNAL_NoValue.NoValue)))
            {
                return true;
            }
            return false;
        }

        static void SetSpecificValue(INTERNAL_PropertyStorage storage, KindOfValue kindOfValueToSet, object newValue, PropertyMetadata typeMetadata)
        {
#if PERFSTAT
            var t = Performance.now();
#endif
            if (typeMetadata == null)
                typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
            object oldValue = GetValue(storage, typeMetadata);
            bool raisePropertyChangedAndCascadetoChildren = true;

            bool impactsActualValue = DoesSpecificValueImpactActualValue(storage, kindOfValueToSet);
            bool coerces = typeMetadata != null && typeMetadata.CoerceValueCallback != null;

            switch (kindOfValueToSet)
            {
                case KindOfValue.Coerced:
                    storage.CoercedValue = newValue;
                    break;
                case KindOfValue.VisualState:
                    //oldValue = (storage.VisualStateValue == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.VisualStateValue);
                    storage.VisualStateValue = newValue;
                    break;
                case KindOfValue.Local:
                    //oldValue = (storage.Local == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.Local);
                    storage.Local = newValue;
                    break;
                case KindOfValue.LocalStyle:
                    //oldValue = (storage.LocalStyleValue == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.LocalStyleValue);
                    storage.LocalStyleValue= newValue;
                    break;
                case KindOfValue.ImplicitStyle:
                    //oldValue = (storage.ImplicitStyleValue == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.ImplicitStyleValue);
                    storage.ImplicitStyleValue = newValue;
                    break;
                case KindOfValue.Inherited:
                    //inherited is a special case
                    break;
                default:
                    break;
            }

            if (newValue == INTERNAL_NoValue.NoValue) //we consider that the new value is the one with the highest priority since we removed the former one.
            {
                newValue = GetValueWithoutCoerce(storage, typeMetadata);
            }

            //if the new Value changes the actual value and is coerced, we compute the coerced value and set it.
            if (coerces && impactsActualValue && kindOfValueToSet != KindOfValue.Coerced)
            {
                // Compute the coerced value:
                newValue = typeMetadata.CoerceValueCallback(storage.Owner, newValue);

                // A coerced value has the highest priority so it does impact the actual value:
                impactsActualValue = true;

                // Remember the coerced value:
                storage.CoercedValue = newValue;
            }

            //we make sure that we don't raise PropertyChanged for IsEnabled if its inherited value is false, because a "false" inherited value has priority over any local value.
            if (storage._isIsEnabledOrIsHitTestVisibleProperty)
            {
                object value;
                if ((value = storage.InheritedValue) != INTERNAL_NoValue.NoValue && value != null && (((bool)value) == false))
                {
                    raisePropertyChangedAndCascadetoChildren = false; //we don't want to call RaisePropertyChanged... because the inherited value overrides the local value if it is false.
                }
            }

            if (raisePropertyChangedAndCascadetoChildren && impactsActualValue)
            {
#if PERFSTAT
                Performance.Counter("INTERNAL_PropertyStore.SetSpecificValue", t);
#endif
                RaisePropertyChangedAndCascadeToChildren(storage, oldValue, newValue, typeMetadata); //Note: in the case of a value that has no CoerceCallBack, this is equal to newValue.
            }
        }

        internal static void SetInheritedValue(INTERNAL_PropertyStorage storage, object newValue, bool recursively) //the "firePropertyChangedEvent" parameter allows us to only throw the onPropertyChangedEvent when the dependencyObject actually uses the property that is changed.
        {
            bool impactsActualValue = DoesSpecificValueImpactActualValue(storage, KindOfValue.Inherited);
            PropertyMetadata typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());
            object oldValue = GetValue(storage, typeMetadata);

            storage.InheritedValue = newValue;

            //-----------------------
            // CHECK THAT A LOCAL OR STYLE VALUE DOES NOT EXIST (otherwise the local/style values superset the inherited value, and we don't want to raise PropertyChanged, nor continue the recursion)
            //-----------------------
            if (impactsActualValue)
            {
                object actualNewValue = GetValueWithoutCoerce(storage, typeMetadata);
                object coercedNewValue = ((typeMetadata != null && typeMetadata.CoerceValueCallback != null) ? typeMetadata.CoerceValueCallback(storage.Owner, actualNewValue) : actualNewValue);
                //-----------------------
                // CHECK IF THE PROPERTY BELONGS TO THE OBJECT (OR TO ONE OF ITS ANCESTORS):
                //-----------------------
                //we only do the following inside the "if" because otherwise, the children's inherithed property would not change anyway
                if (oldValue != coercedNewValue)
                {
                    if (ShouldRaisePropertyChanged(storage))
                    {
                        OnPropertyChanged(storage, oldValue, coercedNewValue, typeMetadata: typeMetadata);
                    }
                    if (recursively)
                        CascadeInheritedPropertyToChildren(storage, coercedNewValue);

                    HandleSpecialPropertiesThatShouldInheritDataContext(storage, coercedNewValue);
                }
            }
            else if (storage._isIsEnabledOrIsHitTestVisibleProperty) //todo: if we decide to make coercion possible on IsHitTestVisible or IsEnabled, change the "newValue" below into "coercedNewValue" (probably)
            {
                if (((bool)newValue == true))
                {
                    newValue = GetValue(GetStorage(storage.Owner, storage.Property)); //we need newValue to be the value that will be active afterwards.
                }
                if (oldValue != newValue)
                {
                    if (ShouldRaisePropertyChanged(storage))
                    {
                        OnPropertyChanged(storage, oldValue, newValue, typeMetadata: typeMetadata);
                    }
                    if (recursively)
                    {
                        CascadeInheritedPropertyToChildren(storage, newValue);
                    }
                }
            }
        }

        internal static void HandleSpecialPropertiesThatShouldInheritDataContext(INTERNAL_PropertyStorage storage, object newValue)
        {
            // Support inheriting the DataContext in other properties such as "RenderTransform", which does not happen automatically because Transforms are not FrameworkElements so they are not in the visual tree (they are not in the "VisualChildren" internal collection). This ensures that, for example, <RotateTransform Angle="{Binding=Angle}"/> work properly:
            if ((storage.Owner is UIElement)
                && (storage.Owner as UIElement).RenderTransform != null
                && storage.Property == FrameworkElement.DataContextProperty)
            {
                (storage.Owner as UIElement).RenderTransform.SetInheritedValue(FrameworkElement.DataContextProperty, newValue, recursively: false);
            }
        }

        internal static bool ShouldRaisePropertyChanged(INTERNAL_PropertyStorage storage)
        {
            // Note: we only want to call "OnPropertyChanged" when the property is used by the current DependencyObject or if it is the DataContext property.
            //todo: handle attached properties
            return storage.Property.OwnerType.IsAssignableFrom(storage.Owner.GetType()) || storage.Property == FrameworkElement.DataContextProperty;
        }

        internal static void SetLocalStyleValue(INTERNAL_PropertyStorage storage, object newValue)
        {
            SetSpecificValue(storage, KindOfValue.LocalStyle, newValue, null);
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
        }

        internal static void ResetLocalStyleValue(INTERNAL_PropertyStorage storage)
        {
            storage.LocalStyleValue = INTERNAL_NoValue.NoValue; //this only occurs when we detach an item so there is no need to care about the propertyChanged event.
        }


        internal static void OnPropertyChanged(INTERNAL_PropertyStorage storage, object oldValue, object newValue, PropertyMetadata typeMetadata = null) // "raiseEvenIfNewValueIsSameAsOldValue" is used to force refresh.
        {
            DependencyObject sender = storage.Owner;

            if (typeMetadata == null)
                typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());

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
                if (newValue != oldValue)
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
            var storage = GetStorage(target, property, createAndSaveNewStorageIfNotExists: true);
            List<IPropertyChangedListener> listeners = storage.PropertyListeners;
            if (listeners == null)
            {
                listeners = storage.PropertyListeners = new List<IPropertyChangedListener>();
            }

            PropertyChangedListener listener = new PropertyChangedListener(storage, updateSourceCallback);

            listeners.Add(listener);
            return listener;
        }

        internal static void CoerceCurrentValue(INTERNAL_PropertyStorage storage, PropertyMetadata typeMetadata)
        {
            if (typeMetadata == null)
                typeMetadata = storage.Property.GetTypeMetaData(storage.Owner.GetType());

            if (typeMetadata == null || typeMetadata.CoerceValueCallback == null)
            {
                return; //we should not arrive here in this case but we make sure to not do anything should that happen.
            }
            object oldValue = storage.Local == INTERNAL_NoValue.NoValue ? (typeMetadata != null ? typeMetadata.DefaultValue : null) : storage.Local;

            object currentValue = GetValueWithoutCoerce(storage, typeMetadata); //Note: we do not need to know where this value comes from (Local, VisualState, etc.) since calling this method means that it has not been modified and we only need to update the coerced value.
            object coercedNewValue = typeMetadata.CoerceValueCallback(storage.Owner, currentValue);
            SetSpecificValue(storage, KindOfValue.Coerced, coercedNewValue, typeMetadata);
        }
    }
}
