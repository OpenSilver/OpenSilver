
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
using CSHTML5.Internal;
using OpenSilver.Internal.Data;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
    public class BindingExpression : BindingExpressionBase
    {
        // we are not allowed to change the following in BindingExpression because it is used:
        //  - ParentBinding.Mode
        //  - ParentBinding.Converter
        //  - ParentBinding.ConverterLanguage
        //  - ParentBinding.ConverterParameter

        private static readonly Type NullableType = typeof(Nullable<>);

        // This boolean is set to true in OnAttached to force Validation at the next
        // UpdateSourceObject. Its purpose is to force the Validation only once to
        // avoid hindering performances.
        internal bool INTERNAL_ForceValidateOnNextSetValue = false;
        internal bool IsUpdating;
        private bool _isAttaching;
        private IPropertyChangedListener _propertyListener;
        private DynamicValueConverter _dynamicConverter;
        private object _bindingSource;
        private bool _isUpdateOnLostFocus; // True if this binding expression updates on LostFocus
        private bool _needsUpdate; // True if this binding expression has a pending source update
        private FrameworkElement _mentor;

        private readonly PropertyPathWalker _propertyPathWalker;

        internal BindingExpression(Binding binding, DependencyObject target, DependencyProperty property)
            : this(binding, property)
        {
            Target = target;
        }

        internal BindingExpression(Binding binding, DependencyProperty property)
        {
            ParentBinding = binding;
            TargetProperty = property;

            _propertyPathWalker = new PropertyPathWalker(this);
        }

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
                    _bindingSource is FrameworkElement sourceFE)
                {
                    // Note: In the BindingExpression, we set the Source to the
                    // FrameworkElement and added the DataContext in the Path
                    return sourceFE.DataContext;
                }

                return _bindingSource;
            }
        }

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
                UpdateSourceObject(Target.GetValue(TargetProperty));
            }
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object value;

            if (_propertyPathWalker.IsPathBroken)
            {
                //------------------------
                // BROKEN PATH
                //------------------------

                if (_propertyPathWalker.IsDataContextBound && _propertyPathWalker.FinalNode is DataContextNode)
                {
                    value = UseTargetNullValue();
                }
                else
                {
                    value = UseFallbackValue();
                }
            }
            else
            {
                //------------------------
                // NON-BROKEN PATH
                //------------------------

                value = GetConvertedValue(_propertyPathWalker.ValueInternal);
            }

            return value;
        }

        private object GetConvertedValue(object rawValue)
        {
            object value = rawValue;

            if (ParentBinding.Converter != null)
            {
#if MIGRATION
                value = ParentBinding.Converter.Convert(value, TargetProperty.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                value = ParentBinding.Converter.Convert(value, TargetProperty.PropertyType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
                if (value == DependencyProperty.UnsetValue)
                {
                    value = ParentBinding.FallbackValue ?? GetDefaultValue();
                }
            }

            if (value == null)
            {
                value = ParentBinding.TargetNullValue;
            }
            else
            {
                value = ApplyStringFormat(value);
            }

            value = ConvertValueImplicitly(value, TargetProperty.PropertyType);

            if (value == DependencyProperty.UnsetValue)
            {
                value = UseFallbackValue();
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

            ParentBinding.Seal();

            _isAttaching = IsAttached = true;

            Target = d;

            _isUpdateOnLostFocus = ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Default &&
                (d is TextBox && dp == TextBox.TextProperty || 
                 d is PasswordBox && dp == PasswordBox.PasswordProperty);
            if (_isUpdateOnLostFocus)
            {
                ((FrameworkElement)Target).LostFocus += new RoutedEventHandler(OnTargetLostFocus);
            }

            AttachToContext(false);

            if (_bindingSource is FrameworkElement fe)
            {
                if (ParentBinding.XamlPath == "ActualWidth" || ParentBinding.XamlPath == "ActualHeight")
                {
                    fe.SubsribeToSizeChanged();
                }
            }

            // FindSource should find the source now. Otherwise, the PropertyPathNodes
            // shoud do the work (their properties will change when the source will
            // become available)
            _propertyPathWalker.Update(_bindingSource);

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

            _isAttaching = false;
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            if (!IsAttached)
                return;

            IsAttached = false;

            if (_propertyListener != null)
            {
                _propertyListener.Detach();
                _propertyListener = null;
            }

            _propertyPathWalker.Update(null);

            if (_isUpdateOnLostFocus)
            {
                _isUpdateOnLostFocus = false;
                ((FrameworkElement)Target).LostFocus -= new RoutedEventHandler(OnTargetLostFocus);
            }

            DetachMentor();

            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);
            Target = null;
        }

        internal void ValueChanged() { Refresh(); }

        /// <summary>
        /// The element that is the binding target object of this binding expression.
        /// </summary>
        internal DependencyObject Target { get; private set; }

        /// <summary>
        /// This method is used to check whether the value is Valid if needed.
        /// </summary>
        /// <param name="initialValue"></param>
        internal void CheckInitialValueValidity(object initialValue)
        {
            if (ParentBinding.ValidatesOnExceptions && ParentBinding.ValidatesOnLoad)
            {
                if (!_propertyPathWalker.IsPathBroken)
                {
                    INTERNAL_ForceValidateOnNextSetValue = false;
                    try
                    {
                        PropertyPathNode node = _propertyPathWalker.FinalNode;
                        node.SetValue(node.Value); //we set the source property to its own value to check whether it causes an exception, in which case the value is not valid.
                    }
                    catch (Exception e) //todo: put the content of this catch in a method which will be called here and in UpdateSourceObject (OR put the whole try/catch in the method and put the Value to set as parameter).
                    {
                        //We get the new Error (which is the innermost exception as far as I know):
                        Exception currentException = e;
#if OPENSILVER
                        if (true) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
#elif BRIDGE
                        if (CSHTML5.Interop.IsRunningInTheSimulator) // Note: "InnerException" is only supported in the Simulator as of July 27, 2017.
#endif
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
            AttachToContext(true);
            if (_bindingSource != null)
            {
                _propertyPathWalker.Update(_bindingSource);
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
            if (IsUpdating || ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
            {
                return;
            }

            if (_isUpdateOnLostFocus && ReferenceEquals(FocusManager.GetFocusedElement(), Target))
            {
                _needsUpdate = true;
                return;
            }

            UpdateSourceObject(value);
        }

        private static bool IsValueValidForSourceUpdate(object value, Type type)
        {
            if (value == null)
            {
                return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == NullableType);
            }
            
            return type.IsAssignableFrom(value.GetType());
        }

        internal void UpdateSourceObject(object value)
        {
            if (_propertyPathWalker.IsPathBroken)
                return;

            PropertyPathNode node = _propertyPathWalker.FinalNode;
            bool oldIsUpdating = IsUpdating;

            object convertedValue = value;
            Type expectedType = node.Type;

            try
            {
                if (expectedType != null && ParentBinding.Converter != null)
                {
#if MIGRATION
                    convertedValue = ParentBinding.Converter.ConvertBack(value, expectedType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                    convertedValue = ParentBinding.Converter.ConvertBack(value, expectedType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif

                    if (convertedValue == DependencyProperty.UnsetValue)
                        return;
                }

                if (!IsValueValidForSourceUpdate(convertedValue, expectedType))
                {
                    IsUpdating = true;

#if MIGRATION
                    convertedValue = DynamicConverter.Convert(convertedValue, expectedType, null, ParentBinding.ConverterCulture);
#else
                    convertedValue = DynamicConverter.Convert(convertedValue, expectedType, null, ParentBinding.ConverterLanguage);
#endif

                    if (convertedValue == DependencyProperty.UnsetValue)
                        return;                    
                }

                node.SetValue(convertedValue);
                Validation.ClearInvalid(this);
            }
            catch (Exception e)
            {
                //If we have ValidatesOnExceptions set to true, we display a popup with the error close to the element.
                if (ParentBinding.ValidatesOnExceptions)
                {
                    //We get the new Error (which is the innermost exception as far as I know):
                    Exception currentException = e;

                    while (currentException.InnerException != null)
                    {
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

        private void OnTargetLostFocus(object sender, RoutedEventArgs e)
        {
            if (_needsUpdate)
            {
                _needsUpdate = false;
                UpdateSourceObject(Target.GetValue(TargetProperty));
            }
        }

        /// <summary>
        /// Create a format that is suitable for String.Format
        /// </summary>
        /// <param name="stringFormat"></param>
        /// <returns></returns>
        internal static string GetEffectiveStringFormat(string stringFormat)
        {
            if (stringFormat.IndexOf('{') < 0)
            {
                stringFormat = @"{0:" + stringFormat + @"}";
            }
            return stringFormat;
        }

        private DynamicValueConverter DynamicConverter
        {
            get
            {
                if (_dynamicConverter == null)
                {
                    _dynamicConverter = new DynamicValueConverter(ParentBinding.Mode == BindingMode.TwoWay);
                }

                return _dynamicConverter;
            }
        } 

        private object ConvertValueImplicitly(object value, Type targetType)
        {
            if (IsValidValue(value, targetType))
            {
                return value;
            }

            return UseDynamicConverter(value, targetType);
        }

        private static bool IsValidValue(object value, Type targetType)
        {
            if (value != null)
            {
                return targetType.IsAssignableFrom(value.GetType());
            }

            return false;
        }

        private object UseDynamicConverter(object value, Type targetType)
        {
            object convertedValue;
            try
            {
#if MIGRATION
                convertedValue = DynamicConverter.Convert(value, targetType, ParentBinding.ConverterParameter, ParentBinding.ConverterCulture);
#else
                convertedValue = DynamicConverter.Convert(value, targetType, ParentBinding.ConverterParameter, ParentBinding.ConverterLanguage);
#endif
            }
            catch (Exception ex)
            {
                HandleException(ex);
                convertedValue = DependencyProperty.UnsetValue;
            }

            return convertedValue;
        }

        private object UseTargetNullValue()
        {
            object value;

            if (ParentBinding.TargetNullValue != null)
            {
                value = GetConvertedValue(null);
            }
            else
            {
                value = GetDefaultValue();
            }

            return value;
        }

        private object UseFallbackValue()
        {
            object value = DependencyProperty.UnsetValue;

            if (ParentBinding.FallbackValue != null)
            {
                value = ConvertValueImplicitly(ParentBinding.FallbackValue, TargetProperty.PropertyType);
            }
            
            if (value == DependencyProperty.UnsetValue)
            {
                value = GetDefaultValue();
            }

            return value;
        }

        private object ApplyStringFormat(object value)
        {
            object result = value;

            string format = ParentBinding.StringFormat;
            if (format != null)
            {
                try
                {
                    string stringFormat = GetEffectiveStringFormat(format);
                    result = string.Format(stringFormat, value);
                }
                catch (FormatException fe)
                {
                    HandleException(fe);
                    result = UseFallbackValue();
                }
            }

            return result;
        }

        private object GetDefaultValue() => TargetProperty.GetMetadata(Target.GetType()).DefaultValue;

        private static void HandleException(Exception ex)
        {
            if (Application.Current.Host.Settings.EnableBindingErrorsLogging)
            {
                Debug.WriteLine(ex.ToString());
            }
            if (Application.Current.Host.Settings.EnableBindingErrorsThrowing)
            {
                throw ex;
            }
        }

        private void OnTargetInheritedContextChanged(object sender, EventArgs e)
        {
            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);
            OnSourceAvailable();
        }

        private void OnMentorLoaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= new RoutedEventHandler(OnMentorLoaded);
            OnSourceAvailable();
        }

        private void AttachToContext(bool lastAttempt)
        {
            object source = null;
            FrameworkElement mentor = null;
            bool useMentor = false;

            if (ParentBinding.Source != null)
            {
                source = ParentBinding.Source;
            }
            else if (ParentBinding.ElementName != null)
            {
                useMentor = true;
                mentor = FrameworkElement.FindMentor(Target);
                if (mentor != null)
                {
                    source = FindName(mentor, ParentBinding.ElementName);
                    if (source == null && !lastAttempt)
                    {
                        mentor.Loaded += new RoutedEventHandler(OnMentorLoaded);
                    }
                }
            }
            else if (ParentBinding.RelativeSource != null)
            {
                switch (ParentBinding.RelativeSource.Mode)
                {
                    case RelativeSourceMode.Self:
                        source = Target;
                        break;

                    case RelativeSourceMode.TemplatedParent:
                        useMentor = true;
                        mentor = FrameworkElement.FindMentor(Target);
                        source = mentor?.TemplatedParent;
                        break;

                    case RelativeSourceMode.FindAncestor:
                        useMentor = true;
                        mentor = FrameworkElement.FindMentor(Target);
                        if (mentor != null)
                        {
                            source = FindAncestor(mentor, ParentBinding.RelativeSource);
                            if (source == null && !lastAttempt)
                            {
                                mentor.Loaded += new RoutedEventHandler(OnMentorLoaded);
                            }
                        }
                        break;

                    case RelativeSourceMode.None:
                    default:
                        source = null;
                        break;
                }
            }
            else
            {
                if (Target is FrameworkElement targetFE)
                {
                    DependencyObject contextElement = targetFE;

                    // special cases:
                    // 1. if target property is DataContext, use the target's parent.
                    //      This enables <X DataContext="{Binding...}"/>
                    // 2. if the target is ContentPresenter and the target property
                    //      is Content, use the parent.  This enables
                    //          <ContentPresenter Content="{Binding...}"/>
                    if (TargetProperty == FrameworkElement.DataContextProperty ||
                        TargetProperty == ContentPresenter.ContentProperty)
                    {
                        contextElement = targetFE.Parent ?? VisualTreeHelper.GetParent(targetFE);
                    }

                    source = contextElement;
                }
                else
                {
                    useMentor = true;
                    source = mentor = FrameworkElement.FindMentor(Target);
                }
            }

            _bindingSource = source;

            if (useMentor)
            {
                if (_mentor != mentor)
                {
                    DetachMentor();
                }

                _mentor = mentor;

                if (source == null && mentor == null)
                {
                    Target.InheritedContextChanged += new EventHandler(OnTargetInheritedContextChanged);
                }
            }
        }

        private void DetachMentor()
        {
            if (_mentor != null)
            {
                _mentor.Loaded -= new RoutedEventHandler(OnMentorLoaded);
                _mentor = null;
            }
        }

        private static object FindName(FrameworkElement mentor, string name)
        {
            object o = null;
            FrameworkElement fe = mentor;

            while (o == null && fe != null)
            {
                o = fe.FindName(name);

                if (o == null)
                {
                    // move to the next outer namescope.
                    // First try TemplatedParent of the scope owner.
                    DependencyObject dd = fe.TemplatedParent;

                    // if that doesn't work, we could be at the top of
                    // generated content for an ItemsControl.  If so, use
                    // the (visual) parent - a panel.
                    if (dd == null)
                    {
                        Panel panel = fe as Panel;
                        if (panel != null && panel.IsItemsHost)
                        {
                            dd = panel;
                        }
                    }

                    // Last, try inherited context
                    if (dd == null)
                    {
                        dd = fe.InheritanceContext;
                    }

                    fe = FrameworkElement.FindMentor(dd);
                }
            }

            return o;
        }

        private static object FindAncestor(FrameworkElement mentor, RelativeSource relativeSource)
        {
            // todo: support bindings in style setters and then remove the following test.
            // To reproduce the issue:
            // <Style x:Key="LegendItemControlStyle"
            //        TargetType="legend:LegendItemControl">
            //   <Setter Property="DefaultMarkerGeometry"
            //           Value="{Binding DefaultMarkerGeometry, RelativeSource={RelativeSource AncestorType=telerik:RadLegend}}"/>
            // </Style>
            if (mentor == null)
                return null;

            // make sure the target is in the visual tree:
            if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(mentor))
                return null;

            // get the AncestorLevel and AncestorType:
            int ancestorLevel = relativeSource.AncestorLevel;
            Type ancestorType = relativeSource.AncestorType;
            if (ancestorLevel < 1 || ancestorType == null)
                return null;

            // look for the target's ancestor:
            UIElement currentParent = (UIElement)VisualTreeHelper.GetParent(mentor);
            if (currentParent == null)
                return null;

            while (!ancestorType.IsAssignableFrom(currentParent.GetType()) || --ancestorLevel > 0)
            {
                currentParent = (UIElement)VisualTreeHelper.GetParent(currentParent);
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
                TryUpdateSourceObject(Target.GetValue(TargetProperty));
            }
            catch (Exception err)
            {
                Console.WriteLine($"[BINDING] UpdateSource: {err}");
            }
        }

        private void Refresh()
        {
            if (_isAttaching)
                return;

            if (IsAttached)
            {
                bool oldIsUpdating = IsUpdating;
                IsUpdating = true;
                Target.ApplyExpression(TargetProperty, this, ParentBinding._isInStyle);

                IsUpdating = oldIsUpdating;
            }
        }
    }
}
