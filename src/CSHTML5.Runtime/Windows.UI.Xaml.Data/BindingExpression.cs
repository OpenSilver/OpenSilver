﻿
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



using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections;
using DotNetForHtml5.Core;
#if !MIGRATION
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Collections.ObjectModel;
#else
using System.Windows.Controls;
using System.Windows.Media;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Contains information about a single instance of a System.Windows.Data.Binding.
    /// </summary>
    public class BindingExpression : IPropertyPathWalkerListener
    {

        //we are not allowed to change the following in BindingExpression because it is used:
        //  - ParentBinding.Mode
        //  - ParentBinding.Converter
        //  - ParentBinding.ConverterLanguage
        //  - ParentBinding.ConverterParameter

        internal bool IsUpdating;
        internal bool IsAttached;

        /// <summary>
        /// The binding target property of this binding expression.
        /// </summary>
        public DependencyProperty Property;
        /// <summary>
        /// The System.Windows.Data.Binding object of the current System.Windows.Data.BindingExpression.
        /// </summary>
        public Binding ParentBinding;
        /// <summary>
        /// The element that is the binding target object of this binding expression.
        /// </summary>
        public DependencyObject Target;

        private PropertyPathWalker PropertyPathWalker;
        private IPropertyChangedListener PropertyListener;

        private bool _isDataContextBound;
        private object _DataContext;


        internal BindingExpression(Binding binding, DependencyObject target, DependencyProperty property)
        {
            ParentBinding = binding;
            Target = target;
            Property = property;

            //setting _isDataContextBound to true if none of the following elements are set in the parentBinding: ElementName, Source and RelativeSource :
            _isDataContextBound = binding.ElementName == null && binding.Source == null && binding.RelativeSource == null;

            //we change the Binding so that when it is bound to the DataContext, we handle it the same way as 
            if (_isDataContextBound)
            {
                ParentBinding.INTERNAL_ComputedSource = target;
                string str = ParentBinding.Path.Path;
                if (!String.IsNullOrWhiteSpace(str) && !((str = str.Trim()) == "."))
                {

                    ParentBinding.INTERNAL_ComputedPath = new PropertyPath("DataContext." + str);
                }
                else
                {
                    ParentBinding.INTERNAL_ComputedPath = new PropertyPath("DataContext");
                }
                _isDataContextBound = false; //test to see if the problems with the DataContext are still there or not.
            }

            string path = (binding.INTERNAL_ComputedPath != null ? binding.INTERNAL_ComputedPath.Path : null);
            var walker = PropertyPathWalker = new PropertyPathWalker(path, _isDataContextBound);
            if (binding.Mode != BindingMode.OneTime)
                walker.Listen(this);

        }

        /// <summary>
        /// Gets the binding source object that this System.Windows.Data.BindingExpression
        /// uses.
        /// </summary>
        public object DataItem
        {
            get
            {
                object source = FindSource();
                if (ParentBinding.ElementName == null && ParentBinding.Source == null && ParentBinding.RelativeSource == null && source is FrameworkElement) //I think source is always a frameworkElement but we check anyway.
                {
                    return ((FrameworkElement)source).DataContext;//Note: In the BindingExpression, we set the Source to the FrameworkElement and added the DataContext in the Path
                }
                return source;
            }
        }

        internal void IsBrokenChanged() { Refresh(); }
        void IPropertyPathWalkerListener.IsBrokenChanged() { IsBrokenChanged(); } //this is so that we don't have a compilation error while still are able to give valueChanged() as internal (interfaces do not allow implementation with other access than public...)

        internal void ValueChanged() { Refresh(); }
        void IPropertyPathWalkerListener.ValueChanged() { ValueChanged(); } //this is so that we don't have a compilation error while still are able to give valueChanged() as internal (interfaces do not allow implementation with other access than public...)


        internal object GetValue(DependencyProperty property, Type OwnerType = null)
        {
            object value;

            if (PropertyPathWalker.IsPathBroken)
            {
                //------------------------
                // BROKEN PATH
                //------------------------

                PropertyMetadata typeMetadata = null;
                if (OwnerType == null)
                {
                    typeMetadata = property.GetTypeMetaData(property.OwnerType);
                }
                else
                {
                    typeMetadata = property.GetTypeMetaData(OwnerType);
                }
                //todo: IMPORTANT: add the type to the parameters of the method so that we can call GetPropertyMetadata while considering the right type. (we cannot use property.Ownertype

                value = ParentBinding.FallbackValue; // Note: the "FallbackValue" is null by default.

                if (value != null)
                {
                    // Convert from String or other types to the destination type (eg. binding "ScrollBar.Value" to "TextBox.Text"):
                    value = ConvertValueIfNecessary(value, property.PropertyType);
                }
                else
                {
                    // Apply the default value of the dependency property:
                    if (typeMetadata != null)
                    {
                        value = typeMetadata.DefaultValue; // This is useful for example to prevent "e.NewValue" in "Visibility_PropertyChanged" from being equal to "null" rather than "Visible" when changing the DataContext to null while Visibility was bound to a property of the DataContext. For details, see "AccountManagerApp" and see the commits of CSHTML5 around the date 2015.05.23.
                    }
                }

                return value;
            }
            else
            {
                //------------------------
                // NON-BROKEN PATH
                //------------------------

                value = PropertyPathWalker.ValueInternal;

                //todo: if the value here is "Unset" (is it even possible?), should we then use "FallbackValue" instead?

                if (ParentBinding.Converter != null)
                {
#if MIGRATION
                    value = ParentBinding.Converter.Convert(value, property.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                    value = ParentBinding.Converter.Convert(value, property.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                }

                // Convert from String or other types to the destination type (eg. binding "ScrollBar.Value" to "TextBox.Text"):
                value = ConvertValueIfNecessary(value, property.PropertyType);

                if (ParentBinding.StringFormat != null)
                {
                    value = String.Format(ParentBinding.StringFormat, value);
                }

                // If null, apply the "TargetNullValue":
                if (value == null)
                {
                    value = ParentBinding.TargetNullValue; // Note: the "TargetNullValue" is also null by default.
                }

                //Note: Observations from Silverlight:
                //      setting a binding between a value and a property that are of incompatible types can have two behaviors
                //          - if there is a "convention" (for example, an integer can be considered true or false depending on whether it is 0 (or > 0 I don't remember) or not)
                //              then this "convention" is applied
                //          - if there is no such "convention", the DEFAULT value of the DependencyProperty is applied (and not the previous value as I would have assumed)
                Type propertyType = property.PropertyType;
                if (propertyType != typeof(string)) //special case: we want a string as the result --> the calling method will make a toString so no need to do anything to the value.
                {
                    if (value == null)
                    {
                        if (propertyType.IsValueType && !propertyType.FullName.StartsWith("System.Nullable`1"))
                        {
                            value = property.GetTypeMetaData(OwnerType).DefaultValue;
                        }
                    }
                    else
                    {
                        Type nonNullableMemberType = propertyType;
                        if (propertyType.FullName.StartsWith("System.Nullable`1"))
                        {
                            nonNullableMemberType = Nullable.GetUnderlyingType(propertyType); //We know the value is not null here.
                        }
                        if (!AreNumericTypes(nonNullableMemberType, value))
                        {
                            Type valueType = value.GetType();
                            if (!(valueType.Name == "Array" && typeof(IEnumerable).IsAssignableFrom(nonNullableMemberType)))
                            {
                                //In the case where the value and the expected type are numeric, we keep the value as is since JSIL doesn't know the difference between a double and an int:
                                if (!nonNullableMemberType.IsAssignableFrom(valueType)) //the value cannot be set to the item so we get the DependencyProperty's default value
                                {
                                    //todo: Add a handling of the special cases of "conventions" (see note "Observations from Silverlight" above).
                                    value = property.GetTypeMetaData(OwnerType).DefaultValue;
                                }
                            }
                        }
                    }
                }

            }

            return value;
        }
        static HashSet2<Type> NumericTypes;

        static bool AreNumericTypes(Type type, object obj)
        {
            if (NumericTypes == null)
            {
                NumericTypes = new HashSet2<Type>
                    {
                        typeof(Byte),
                        typeof(SByte),
                        typeof(UInt16),
                        typeof(UInt32),
                        typeof(UInt64),
                        typeof(Int16),
                        typeof(Int32),
                        typeof(Int64),
                        typeof(Decimal),
                        typeof(Double),
                        typeof(Single),
                        typeof(Byte?),
                        typeof(SByte?),
                        typeof(UInt16?),
                        typeof(UInt32?),
                        typeof(UInt64?),
                        typeof(Int16?),
                        typeof(Int32?),
                        typeof(Int64?),
                        typeof(Decimal?),
                        typeof(Double?),
                        typeof(Single?),
                    };
            }
            if (type != null && NumericTypes.Contains(type) && obj != null && NumericTypes.Contains(obj.GetType()))
                return true;
            else
                return false;
        }

        static object ConvertValueIfNecessary(object value, Type targetType)
        {
            // Convert from String to the destination type:
            if (value is string && targetType != typeof(string)) //eg. binding "ScrollBar.Value" to "TextBox.Text"
            {
                try //this try/catch block is solely for the purpose of not raising an exception so that the GetValue finishes its thing (including handling the case where the conversion cannot be done).
                {
                    value = global::DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(targetType, (string)value);
                }
                catch (Exception ex)
                {
                    if (Application.Current.Host.Settings.EnableBindingErrorsLogging)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    if (Application.Current.Host.Settings.EnableBindingErrorsThrowing)
                    {
                        throw;
                    }
                }
            }

            // Some hard-coded conversions: //todo: generalize this system by implementing "TypeConverter" and "TypeConverterAttribute"
            if (value is SolidColorBrush && targetType == typeof(Color)) //eg. binding "Border.Background" to "DropShadowEffect.Color"
            {
                value = ((SolidColorBrush)value).Color;
            }

            return value;
        }

        internal bool INTERNAL_ForceValidateOnNextSetValue = false; //This boolean is set to true in OnAttached to force Validation at the next UpdateSourceObject. Its purpose is to force the Validation only once to avoid hindering performances.

        internal void OnAttached(DependencyObject target)
        {
            if (IsAttached)
            {
                return;
            }

            IsAttached = true;
            //get the DataContextProperty.
            string dataContextPropertyName = "DataContext";
            Type type = target.GetType();
            DependencyProperty dataContextDependencyProperty = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(type, dataContextPropertyName);
            if (dataContextDependencyProperty != null)
            {
                object dataContext = target.GetValue(dataContextDependencyProperty);
                this.OnDataContextChanged(dataContext);
            }

            var source = FindSource();

            PropertyPathWalker.Update(source); //FindSource should find the source now. Otherwise, the PropertyPathNodes shoud do the work (their properties will change when the source will become available)

            //Listen to changes on the Target if the Binding is TwoWay:
            if (ParentBinding.Mode == BindingMode.TwoWay)
            {
                PropertyListener = INTERNAL_PropertyStore.ListenToChanged(Target, Property, UpdateSourceCallback);

                //If the user wants to force the Validation of the value when the element is added to the Visual tree, we set a boolean to do it as soon as possible:
                if (ParentBinding.ValidatesOnExceptions && ParentBinding.ValidatesOnLoad)
                {
                    INTERNAL_ForceValidateOnNextSetValue = true;
                }
            }
        }

        /// <summary>
        /// This method is used to check whether the value is Valid if needed.
        /// </summary>
        /// <param name="initialValue"></param>
        internal void CheckInitialValueValidity(object initialValue)
        {
            if (ParentBinding.ValidatesOnExceptions && ParentBinding.ValidatesOnLoad)
            {
                if (!PropertyPathWalker.IsPathBroken)
                {
                    INTERNAL_ForceValidateOnNextSetValue = false;
                    try
                    {
                        PropertyPathNode node = (PropertyPathNode)PropertyPathWalker.FinalNode;
                        node.SetValue(node.Value); //we set the source property to its own value to check whether it causes an exception, in which case the value is not valid.
                    }
                    catch (Exception e) //todo: put the content of this catch in a method which will be called here and in UpdateSourceObject (OR put the whole try/catch in the method and put the Value to set as parameter).
                    {
                        //We get the new Error (which is the innermost exception as far as I know):
                        Exception currentException = e;

                        if (Interop.IsRunningInTheSimulator) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
                        {
                            while (currentException.InnerException != null)
                                currentException = currentException.InnerException;
                        }

                        Validation.MarkInvalid(this, new ValidationError(this) { Exception = currentException, ErrorContent = currentException.Message });
                    }
                }
            }
        }

        internal void OnDetached(DependencyObject element)
        {
            if (IsAttached)
            {
                this.IsAttached = false;
                this.OnDataContextChanged(null);

                if (PropertyListener != null)
                {
                    PropertyListener.Detach();
                    PropertyListener = null;
                }

                PropertyPathWalker.Update(null);

                Target = null;
            }
        }

        internal void OnSourceAvailable()
        {
            var source = FindSource();
            if (source != null)
            {
                PropertyPathWalker.Update(source);
            }

            //Target.SetValue(Property, this); // Read note below

            //--------------
            // Note: the line above was commented on 2017.06.21 because it caused the following issue:
            // TO REPRODUCE (it happened when the line above was not commented, until Beta 11.10 included):
            // 1) Add the following code:
            //      XAML:
            //         <TabControl>
            //             <TabItem Header="Tab1"></TabItem>
            //             <TabItem Header="Tab2">
            //                 <TextBox Text="{Binding TestProperty, Mode=TwoWay}"/>
            //             </TabItem>
            //         </TabControl>
            //      C#:
            //         public MainPage()
            //         {
            //             this.InitializeComponent();

            //             var x = new TestClass()
            //             {
            //                 TestProperty = "This is a test"
            //             };
            //             LayoutRoot.DataContext = x;
            //         }
            //         public class TestClass
            //         {
            //             public string TestProperty { get; set; }
            //         }
            // 2) Launch the app
            // 3) Click second tab
            // 4) Modify the TextBox
            // 5) Go back to first tab
            // 6) Go to the second tab
            // 7) Issue: the text you previously entered does not appear. Instead, the original text appears.
            //
            // Note: this is the comment that was near the line before it was commented: "todo: see if this SetValue is useful since we don't do it in OnAttached (and this method should basically do the same as what we do when attaching the BindingExpression)"
            //--------------
        }

        private object FindSource()
        {
            if (ParentBinding.INTERNAL_ComputedSource != null)
            {
                return ParentBinding.INTERNAL_ComputedSource;
            }
            else if (ParentBinding.ElementName != null)
            {
                //we should not arrive here
                //todo: fix this so that an ElementName can be set programmatically
                if (Target is FrameworkElement)
                {
                    return ((FrameworkElement)Target).FindName(ParentBinding.ElementName);
                }
            }
            else if (ParentBinding.RelativeSource != null)
            {
                var relativeSource = ParentBinding.RelativeSource;
                switch (relativeSource.Mode)
                {
                    case RelativeSourceMode.Self:
                        return Target;
                    case RelativeSourceMode.TemplatedParent:
                        return ParentBinding.TemplateOwner.TemplateOwner;
                    case RelativeSourceMode.FindAncestor:
                        return FindAncestor(Target, relativeSource);
                }
            }
            return this._DataContext;
        }

        private object FindAncestor(DependencyObject target, RelativeSource relativeSource)
        {
            //todo: support bindings in style setters and then remove the following test. To reproduce the issue: <Style x:Key="LegendItemControlStyle" TargetType="legend:LegendItemControl"><Setter Property="DefaultMarkerGeometry" Value="{Binding DefaultMarkerGeometry, RelativeSource={RelativeSource AncestorType=telerik:RadLegend}}"/></Style>
            if (!(target is UIElement))
                return null;

            //make sure the target is in the visual tree:
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree((UIElement)target))
                return null;

            //get the AncestorLevel and AncestorType:
            int ancestorLevel = relativeSource.AncestorLevel;
            Type ancestorType = relativeSource.AncestorType;
            if (ancestorLevel < 1 || ancestorType == null)
                return null;

            //look for the target's ancestor:
            UIElement currentParent = (UIElement)((UIElement)target).INTERNAL_VisualParent;
            while (!ancestorType.IsAssignableFrom(currentParent.GetType()) || --ancestorLevel > 0)
            {
                currentParent = (UIElement)currentParent.INTERNAL_VisualParent;
                if (currentParent == null)
                    return null;
            }
            if (ancestorLevel == 0)
                return currentParent;
            return null;
        }

        private void UpdateSourceCallback(object sender, IDependencyPropertyChangedEventArgs args)
        {
            try
            {
                if (!IsUpdating && ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
                    UpdateSourceObject(this.Target.GetValue(this.Property));
            }
            catch (Exception err)
            {
                Console.WriteLine("[BINDING] UpdateSource: " + err.ToString());
            }
        }

        internal void TryUpdateSourceObject(object value)
        {
            if (!IsUpdating && ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
            {
                UpdateSourceObject(value);
            }
        }

        internal void UpdateSourceObject(object value)
        {
            if (!PropertyPathWalker.IsPathBroken)
            {
                PropertyPathNode node = (PropertyPathNode)PropertyPathWalker.FinalNode;
                bool oldIsUpdating = IsUpdating;

                try
                {
                    Type expectedType = null;
                    object convertedValue = value;
                    if (ParentBinding.Converter != null)
                    {
                        if (node.DependencyProperty != null)
                        {
#if MIGRATION
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.DependencyProperty.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.DependencyProperty.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                            expectedType = node.DependencyProperty.PropertyType;
                        }
                        else if (node.PropertyInfo != null)
                        {
#if MIGRATION
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.PropertyInfo.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.PropertyInfo.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                            expectedType = node.PropertyInfo.PropertyType;
                        }
                        else if (node.FieldInfo != null)
                        {
#if MIGRATION
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.FieldInfo.FieldType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                            convertedValue = ParentBinding.Converter.ConvertBack(value, node.FieldInfo.FieldType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                            expectedType = node.FieldInfo.FieldType;
                        }
                    }
                    else //we only need to set expectedType:
                    {
                        if (node.DependencyProperty != null)
                        {
                            expectedType = node.DependencyProperty.PropertyType;
                        }
                        else if (node.PropertyInfo != null)
                        {
                            expectedType = node.PropertyInfo.PropertyType;
                        }
                        else if (node.FieldInfo != null)
                        {
                            expectedType = node.FieldInfo.FieldType;
                        }
                    }

                    bool typeAcceptsNullAsValue = !expectedType.IsValueType || expectedType.FullName.StartsWith("System.Nullable`1");
                    bool isNotNullOrIsNullAccepted = ((convertedValue != null) || typeAcceptsNullAsValue);
                    if (isNotNullOrIsNullAccepted) //Note: we put this test here to avoid making unneccessary tests but the point is that the new value is simply ignored since it doesn't fit the property (cannot set a non-nullable property to null).
                    {
                        //bool oldIsUpdating = IsUpdating;
                        IsUpdating = true;
                        Type[] AutoConvertTypes = { typeof(Int16), typeof(Int32), typeof(Int64), typeof(Double), typeof(Uri) };
                        bool typeFound = false;
                        if ((AutoConvertTypes.Contains(expectedType))
                            && convertedValue is string) //todo: find a way to do this more efficiently (and maybe mode generic ?)
                        {
                            typeFound = true;
                            if (expectedType == typeof(Int16))
                            {
                                convertedValue = Int16.Parse((string)convertedValue);
                            }
                            else if (expectedType == typeof(Int32))
                            {
                                convertedValue = Int32.Parse((string)convertedValue);
                            }
                            else if (expectedType == typeof(Int64))
                            {
                                convertedValue = Int64.Parse((string)convertedValue);
                            }
                            else if (expectedType == typeof(Double))
                            {
                                convertedValue = Double.Parse((string)convertedValue);
                            }
                            else if (expectedType == typeof(Uri))
                            {
                                convertedValue = new Uri((string)convertedValue);
                            }
                        }

                        //todo: partially merge this "if" and the previous one.
                        if ((!typeFound && TypeFromStringConverters.CanTypeBeConverted(expectedType))
                            && convertedValue is string)
                        {
                            typeFound = true;
                            convertedValue = TypeFromStringConverters.ConvertFromInvariantString(expectedType, (string)convertedValue);
                        }

                        if (!typeFound && convertedValue != null && (expectedType == typeof(string)))
                        {
                            convertedValue = convertedValue.ToString();
                        }

                        node.SetValue(convertedValue);
                        Validation.ClearInvalid(this);
                    }
                }
                catch (Exception e)
                {
                    //If we have ValidatesOnExceptions set to true, we display a popup with the error close to the element.
                    if (ParentBinding.ValidatesOnExceptions)
                    {
                        //We get the new Error (which is the innermost exception as far as I know):
                        Exception currentException = e;

                        if (Interop.IsRunningInTheSimulator) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
                        {
                            while (currentException.InnerException != null)
                                currentException = currentException.InnerException;
                        }

                        Validation.MarkInvalid(this, new ValidationError(this) { Exception = currentException, ErrorContent = currentException.Message });
                    }
                }
                finally
                {
                    IsUpdating = oldIsUpdating;
                }
            }
        }


        /// <summary>
        /// Sends the current binding target value to the binding source property in
        /// System.Windows.Data.BindingMode.TwoWay or System.Windows.Data.BindingMode.OneWayToSource
        /// bindings.
        /// </summary>
        public void UpdateSource()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException("The Binding has been detached from its target.");
            }
            if (!IsUpdating && ParentBinding.Mode == BindingMode.TwoWay) //found this info at: https://msdn.microsoft.com/fr-fr/library/windows/apps/windows.ui.xaml.data.bindingexpression.updatesource.aspx in the remark.
            {
                UpdateSourceObject(this.Target.GetValue(this.Property));
            }
        }

        internal void OnDataContextChanged(object newDataContext)
        {
            if (_DataContext != newDataContext)
            {
                _DataContext = newDataContext;
                if (_isDataContextBound)
                {
                    PropertyPathWalker.Update(newDataContext);
                    if (ParentBinding.Mode == BindingMode.OneTime)
                    {
                        Refresh();
                    }
                }
            }
        }

        private void Refresh()
        {
            if (IsAttached)
            {
                bool oldIsUpdating = IsUpdating;
                IsUpdating = true;
                Target.ApplyBindingExpression(Property, this);

                IsUpdating = oldIsUpdating;
            }
        }


    }
}
