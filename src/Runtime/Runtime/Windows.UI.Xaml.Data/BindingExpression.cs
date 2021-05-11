
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

using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Contains information about a single instance of a <see cref="Binding" />.
    /// </summary>
    public partial class BindingExpression : BindingExpressionBase, IPropertyPathWalkerListener
    {
        //we are not allowed to change the following in BindingExpression because it is used:
        //  - ParentBinding.Mode
        //  - ParentBinding.Converter
        //  - ParentBinding.ConverterLanguage
        //  - ParentBinding.ConverterParameter

        #region Private Data

        private static readonly Type NullableType = typeof(Nullable<>);

        // This boolean is set to true in OnAttached to force Validation at the next
        // UpdateSourceObject. Its purpose is to force the Validation only once to
        // avoid hindering performances.
        internal bool INTERNAL_ForceValidateOnNextSetValue = false;
        internal bool IsUpdating;
        private bool _isAttaching;
        private readonly PropertyPathWalker propertyPathWalker;
        private IPropertyChangedListener _propertyListener;

        private readonly string computedPath;
        private readonly bool isDataContextBound;
        private object _bindingSource;

        #endregion Private Data

        #region Constructors

        internal BindingExpression(Binding binding, DependencyObject target, DependencyProperty property)
            : this(binding, property)
        {
            this.Target = target;
        }

        internal BindingExpression(Binding binding, DependencyProperty property)
        {
            ParentBinding = binding;
            TargetProperty = property;

            if (binding.ElementName == null && binding.Source == null && binding.RelativeSource == null) //this means that it is bound to current DataContext.
            {
                this.isDataContextBound = true;
                //we change the Binding so that when it is bound to the DataContext, we handle it the same way as 

                string str = ParentBinding.Path.Path;
                if (!string.IsNullOrWhiteSpace(str) && !((str = str.Trim()) == "."))
                {
                    this.computedPath = "DataContext." + str;
                }
                else
                {
                    this.computedPath = "DataContext";
                }
            }
            else
            {
                this.computedPath = this.ParentBinding.Path != null ? this.ParentBinding.Path.Path : string.Empty;
            }

            //string path = (binding.INTERNAL_ComputedPath != null ? binding.INTERNAL_ComputedPath.Path : null);
            var walker = propertyPathWalker = new PropertyPathWalker(this.computedPath, false);
            if (binding.Mode != BindingMode.OneTime)
                walker.Listen(this);

        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// The binding target property of this binding expression.
        /// </summary>
        public DependencyProperty TargetProperty { get; private set; }

        /// <summary>
        /// The <see cref="Binding"/> object of the current <see cref="BindingExpression"/>.
        /// </summary>
        public Binding ParentBinding { get; private set; }

        /// <summary>
        /// Gets the binding source object that this <see cref="BindingExpression"/>
        /// uses.
        /// </summary>
        public object DataItem
        {
            get
            {
                if (ParentBinding.ElementName == null &&
                    ParentBinding.Source == null &&
                    ParentBinding.RelativeSource == null &&
                    this._bindingSource is FrameworkElement sourceFE)
                {
                    // Note: In the BindingExpression, we set the Source to the
                    // FrameworkElement and added the DataContext in the Path
                    return sourceFE.DataContext;
                }
                return this._bindingSource;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sends the current binding target value to the binding source property in
        /// <see cref="BindingMode.TwoWay"/> bindings.
        /// </summary>
        public void UpdateSource()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException("The Binding has been detached from its target.");
            }

            // found this info at: https://msdn.microsoft.com/fr-fr/library/windows/apps/windows.ui.xaml.data.bindingexpression.updatesource.aspx
            // in the remark.
            if (!IsUpdating && ParentBinding.Mode == BindingMode.TwoWay) 
            {
                UpdateSourceObject(this.Target.GetValue(this.TargetProperty));
            }
        }

        #endregion Public Methods

        #region Overriden Methods

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object value;

            if (propertyPathWalker.IsPathBroken)
            {
                //------------------------
                // BROKEN PATH
                //------------------------

                value = ParentBinding.FallbackValue; // Note: the "FallbackValue" is null by default.

                if (value != null)
                {
                    // Convert from String or other types to the destination type
                    // (eg. binding "ScrollBar.Value" to "TextBox.Text"):
                    value = ConvertValueIfNecessary(value, dp.PropertyType);
                }
                else
                {
                    var typeMetadata = dp.GetTypeMetaData(d.GetType());

                    // Apply the default value of the dependency property:
                    if (typeMetadata != null)
                    {
                        value = typeMetadata.DefaultValue;
                    }
                }

                return value;
            }
            else
            {
                //------------------------
                // NON-BROKEN PATH
                //------------------------

                value = propertyPathWalker.ValueInternal;

                // todo: if the value here is "Unset" (is it even possible?),
                // should we then use "FallbackValue" instead?

                if (ParentBinding.Converter != null)
                {
#if MIGRATION
                    value = ParentBinding.Converter.Convert(value, dp.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                    value = ParentBinding.Converter.Convert(value, dp.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                }

                // Convert from String or other types to the destination type
                // (eg. binding "ScrollBar.Value" to "TextBox.Text"):
                value = ConvertValueIfNecessary(value, dp.PropertyType);

                if (ParentBinding.StringFormat != null)
                {
                    value = String.Format(ParentBinding.StringFormat, value);
                }

                // If null, apply the "TargetNullValue":
                if (value == null)
                {
                    value = ParentBinding.TargetNullValue; // Note: the "TargetNullValue" is also null by default.
                }

                // Note: Observations from Silverlight:
                // setting a binding between a value and a property that are
                // of incompatible types can have two behaviors
                // - if there is a "convention" (for example, an integer can
                // be considered true or false depending on whether it is 0
                // (or > 0 I don't remember) or not)
                // then this "convention" is applied
                // - if there is no such "convention", the DEFAULT value of
                // the DependencyProperty is applied (and not the previous
                // value as I would have assumed)
                Type propertyType = dp.PropertyType;

                // special case: we want a string as the result --> the calling
                // method will make a toString so no need to do anything to the
                // value.
                if (propertyType != typeof(string))
                {
                    if (value == null)
                    {
                        if (propertyType.IsValueType && 
                            !(propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == NullableType))
                        {
                            value = dp.GetTypeMetaData(d.GetType()).DefaultValue;
                        }
                    }
                    else
                    {
                        Type nonNullableMemberType = propertyType;
                        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == NullableType)
                        {
                            nonNullableMemberType = Nullable.GetUnderlyingType(propertyType); //We know the value is not null here.
                        }
                        if (!AreNumericTypes(nonNullableMemberType, value))
                        {
                            Type valueType = value.GetType();
                            if (!(valueType == typeof(Array) && typeof(IEnumerable).IsAssignableFrom(nonNullableMemberType)))
                            {
                                // In the case where the value and the expected type are
                                // numeric, we keep the value as is since JSIL doesn't
                                // know the difference between a double and an int:
                                // the value cannot be set to the item so we get the
                                // DependencyProperty's default value
                                if (!nonNullableMemberType.IsAssignableFrom(valueType))
                                {
                                    // todo: Add a handling of the special cases of "conventions"
                                    // (see note "Observations from Silverlight" above).
                                    value = dp.GetTypeMetaData(d.GetType()).DefaultValue;
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        internal override bool CanSetValue(DependencyObject d, DependencyProperty dp)
        {
            return (ParentBinding.Mode == BindingMode.TwoWay);
        }

        internal override void SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            if (CanSetValue(d, dp))
            {
                TryUpdateSourceObject(value);
            }
        }

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            if (IsAttached)
                return;

            this._isAttaching = IsAttached = true;

            this.Target = d;

            this.FindSource();

            // FindSource should find the source now. Otherwise, the PropertyPathNodes
            // shoud do the work (their properties will change when the source will
            // become available)
            propertyPathWalker.Update(this._bindingSource);

            //Listen to changes on the Target if the Binding is TwoWay:
            if (ParentBinding.Mode == BindingMode.TwoWay)
            {
                _propertyListener = INTERNAL_PropertyStore.ListenToChanged(Target, TargetProperty, UpdateSourceCallback);

                // If the user wants to force the Validation of the value when the element
                // is added to the Visual tree, we set a boolean to do it as soon as possible:
                if (ParentBinding.ValidatesOnExceptions && ParentBinding.ValidatesOnLoad)
                {
                    INTERNAL_ForceValidateOnNextSetValue = true;
                }
            }

            this._isAttaching = false;
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            if (!IsAttached)
                return;

            this.IsAttached = false;

            if (_propertyListener != null)
            {
                _propertyListener.Detach();
                _propertyListener = null;
            }

            propertyPathWalker.Update(null);

            Target.InheritedContextChanged -= new EventHandler(this.OnTargetInheritedContextChanged);
            Target = null;
        }

        #endregion Overriden Methods

        #region IPropertyPathWalkerListener

        void IPropertyPathWalkerListener.IsBrokenChanged() { Refresh(); }

        void IPropertyPathWalkerListener.ValueChanged() { Refresh(); }

        #endregion IPropertyPathWalkerListener

        #region Internal Properties

        /// <summary>
        /// The element that is the binding target object of this binding expression.
        /// </summary>
        internal DependencyObject Target { get; private set; }

        #endregion Internal Properties

        #region Internal Methods

        /// <summary>
        /// This method is used to check whether the value is Valid if needed.
        /// </summary>
        /// <param name="initialValue"></param>
        internal void CheckInitialValueValidity(object initialValue)
        {
            if (ParentBinding.ValidatesOnExceptions && ParentBinding.ValidatesOnLoad)
            {
                if (!propertyPathWalker.IsPathBroken)
                {
                    INTERNAL_ForceValidateOnNextSetValue = false;
                    try
                    {
                        PropertyPathNode node = (PropertyPathNode)propertyPathWalker.FinalNode;
                        node.SetValue(node.Value); //we set the source property to its own value to check whether it causes an exception, in which case the value is not valid.
                    }
                    catch (Exception e) //todo: put the content of this catch in a method which will be called here and in UpdateSourceObject (OR put the whole try/catch in the method and put the Value to set as parameter).
                    {
                        //We get the new Error (which is the innermost exception as far as I know):
                        Exception currentException = e;

                        if (CSHTML5.Interop.IsRunningInTheSimulator) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
                        {
                            while (currentException.InnerException != null)
                                currentException = currentException.InnerException;
                        }

                        Validation.MarkInvalid(this, new ValidationError(this) { Exception = currentException, ErrorContent = currentException.Message });
                    }
                }
            }
        }

        internal void OnSourceAvailable()
        {
            this.FindSource();
            if (this._bindingSource != null)
            {
                propertyPathWalker.Update(this._bindingSource);
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
            //         public partial class TestClass
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

        internal void TryUpdateSourceObject(object value)
        {
            if (!IsUpdating && ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
            {
                UpdateSourceObject(value);
            }
        }

        internal void UpdateSourceObject(object value)
        {
            if (!propertyPathWalker.IsPathBroken)
            {
                PropertyPathNode node = (PropertyPathNode)propertyPathWalker.FinalNode;
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

                    bool typeAcceptsNullAsValue = !expectedType.IsValueType || 
                        (expectedType.IsGenericType && expectedType.GetGenericTypeDefinition() == NullableType);

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

                        if (CSHTML5.Interop.IsRunningInTheSimulator) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
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

        #endregion Internal Methods

        #region Private Methods

        private static HashSet2<Type> NumericTypes;

        private static bool AreNumericTypes(Type type, object obj)
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

        private static object ConvertValueIfNecessary(object value, Type targetType)
        {
            // Convert from String to the destination type:
            if (value is string && targetType != typeof(string)) //eg. binding "ScrollBar.Value" to "TextBox.Text"
            {
                try //this try/catch block is solely for the purpose of not raising an exception so that the GetValue finishes its thing (including handling the case where the conversion cannot be done).
                {
                    value = TypeFromStringConverters.ConvertFromInvariantString(targetType, (string)value);
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
            if (targetType == typeof(Color) && value is SolidColorBrush scb) //eg. binding "Border.Background" to "DropShadowEffect.Color"
            {
                value = scb.Color;
            }

            return value;
        }

        private void OnTargetInheritedContextChanged(object sender, EventArgs e)
        {
            this.Target.InheritedContextChanged -= new EventHandler(this.OnTargetInheritedContextChanged);
            this.OnSourceAvailable();
        }

        private void FindSource()
        {
            if (this.isDataContextBound)
            {
                if (this.Target is FrameworkElement targetFE)
                {
                    DependencyObject contextElement = targetFE;

                    // special cases:
                    // 1. if target property is DataContext, use the target's parent.
                    //      This enables <X DataContext="{Binding...}"/>
                    if (this.TargetProperty == FrameworkElement.DataContextProperty)
                    {
                        contextElement = targetFE.Parent;
                    }
                    this._bindingSource = contextElement;
                }
                else
                {
                    this.Target.InheritedContextChanged += new EventHandler(this.OnTargetInheritedContextChanged);
                    this._bindingSource = this.Target.InheritedParent;
                }
            }
            else if (ParentBinding.Source != null)
            {
                this._bindingSource = ParentBinding.Source;
            }
            else if (ParentBinding.ElementName != null)
            {
                // we should not arrive here
                // todo: fix this so that an ElementName can be set programmatically
                if (Target is FrameworkElement targetFE)
                {
                    this._bindingSource = targetFE.FindName(ParentBinding.ElementName);
                }
                else
                {
                    this._bindingSource = null;
                }
            }
            else if (ParentBinding.RelativeSource != null)
            {
                var relativeSource = ParentBinding.RelativeSource;
                switch (relativeSource.Mode)
                {
                    case RelativeSourceMode.Self:
                        this._bindingSource = Target;
                        break;
                    case RelativeSourceMode.TemplatedParent:
                        if (ParentBinding.TemplateOwner != null)
                        {
                            this._bindingSource = ParentBinding.TemplateOwner.TemplateOwner;
                        }
                        else
                        {
                            // todo: find out why we enter here in Client_TUI.
                            Debug.WriteLine("ERROR: ParentBinding.TemplateOwner is null.");
                            this._bindingSource = null;
                        }
                        break;
                    case RelativeSourceMode.FindAncestor:
                        this._bindingSource = FindAncestor(Target, relativeSource);
                        break;
                }
            }
        }

        private object FindAncestor(DependencyObject target, RelativeSource relativeSource)
        {
            // todo: support bindings in style setters and then remove the following test.
            // To reproduce the issue:
            // <Style x:Key="LegendItemControlStyle"
            //        TargetType="legend:LegendItemControl">
            //   <Setter Property="DefaultMarkerGeometry"
            //           Value="{Binding DefaultMarkerGeometry, RelativeSource={RelativeSource AncestorType=telerik:RadLegend}}"/>
            // </Style>
            if (!(target is UIElement uiE))
                return null;

            // make sure the target is in the visual tree:
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(uiE))
                return null;

            // get the AncestorLevel and AncestorType:
            int ancestorLevel = relativeSource.AncestorLevel;
            Type ancestorType = relativeSource.AncestorType;
            if (ancestorLevel < 1 || ancestorType == null)
                return null;

            // look for the target's ancestor:
            UIElement currentParent = (UIElement)uiE.INTERNAL_VisualParent;
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
                    UpdateSourceObject(this.Target.GetValue(this.TargetProperty));
            }
            catch (Exception err)
            {
                Console.WriteLine("[BINDING] UpdateSource: " + err.ToString());
            }
        }

        private void Refresh()
        {
            if (this._isAttaching)
                return;

            if (IsAttached)
            {
                bool oldIsUpdating = IsUpdating;
                IsUpdating = true;
                Target.ApplyExpression(TargetProperty, this, ParentBinding._isInStyle);

                IsUpdating = oldIsUpdating;
            }
        }

        #endregion Private Methods
    }
}
