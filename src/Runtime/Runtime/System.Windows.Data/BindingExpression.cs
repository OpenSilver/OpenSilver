
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using OpenSilver.Internal;
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
        [Flags]
        private enum BindingStatus
        {
            None = 0,
            UpdatingValue = 1 << 0,
            UpdatingSource = 1 << 1,
            Attaching = 1 << 2,
        }

        private BindingStatus _status = BindingStatus.None;
        private bool _isUpdateOnLostFocus;
        private bool _needsUpdate; // True if this binding expression has a pending source update
        private DynamicValueConverter _dynamicConverter;
        private object _bindingSource;
        private IInternalFrameworkElement _mentor;
        private ValidationError _baseValidationError;
        private List<ValidationError> _notifyDataErrors;

        private DependencyPropertyChangedListener _targetPropertyListener;
        private DependencyPropertyChangedListener _dataContextListener;
        private DependencyPropertyChangedListener _cvsListener;
        private WeakEventListener<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs> _sourceErrorsChangedListener;
        private WeakEventListener<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs> _valueErrorsChangedListener;
        private INotifyDataErrorInfo _dataErrorSource;
        private INotifyDataErrorInfo _dataErrorValue;

        private readonly PropertyPathWalker _propertyPathWalker;

        internal bool IsUpdatingValue
        {
            get => ReadFlag(BindingStatus.UpdatingValue);
            set => WriteFlag(BindingStatus.UpdatingValue, value);
        }

        private bool IsUpdatingSource
        {
            get => ReadFlag(BindingStatus.UpdatingSource);
            set => WriteFlag(BindingStatus.UpdatingSource, value);
        }

        private bool IsAttaching
        {
            get => ReadFlag(BindingStatus.Attaching);
            set => WriteFlag(BindingStatus.Attaching, value);
        }

        private void WriteFlag(BindingStatus flag, bool value)
        {
            if (value)
            {
                _status |= flag;
            }
            else
            {
                _status &= ~flag;
            }
        }

        private bool ReadFlag(BindingStatus flag) => (flag & _status) != 0;

        internal BindingExpression(Binding binding, DependencyProperty property)
        {
            ParentBinding = binding;
            TargetProperty = property;

            _propertyPathWalker = new PropertyPathWalker(this);
        }

        private void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            BindingSource = args.NewValue;

            _propertyPathWalker.Update(BindingSource);
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
        public object DataItem => BindingSource;

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
            if (_status == BindingStatus.None && ParentBinding.Mode == BindingMode.TwoWay)
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

                if (_dataContextListener != null && _propertyPathWalker.FirstNode is SourcePropertyNode)
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

                value = GetConvertedValue(_propertyPathWalker.FinalNode.Value);
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

            value = ConvertValueImplicitly(value, TargetProperty);

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

        internal override void OnAttach(DependencyObject d, DependencyProperty dp)
        {
            if (IsAttached)
                return;

            ParentBinding.Seal();

            IsAttaching = IsAttached = true;

            Target = d;

            AttachToContext(false);

            // Listen to changes on the Target if the Binding is TwoWay:
            if (ParentBinding.Mode == BindingMode.TwoWay && ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit)
            {
                _isUpdateOnLostFocus = ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Default &&
                    ((d is TextBox && dp == TextBox.TextProperty) || (d is PasswordBox && dp == PasswordBox.PasswordProperty));
                if (_isUpdateOnLostFocus)
                {
                    ((IInternalFrameworkElement)Target).LostFocus += new RoutedEventHandler(OnTargetLostFocus);
                }

                _targetPropertyListener = new DependencyPropertyChangedListener(Target, TargetProperty, UpdateSourceCallback);
            }

            // FindSource should find the source now. Otherwise, the PropertyPathNodes
            // shoud do the work (their properties will change when the source will
            // become available)
            _propertyPathWalker.Update(BindingSource);

            IsAttaching = false;
        }

        internal override void OnDetach(DependencyObject d, DependencyProperty dp)
        {
            if (!IsAttached)
                return;

            IsAttached = false;

            if (_targetPropertyListener != null)
            {
                _targetPropertyListener.Dispose();
                _targetPropertyListener = null;
            }

            if (_dataContextListener != null)
            {
                _dataContextListener.Dispose();
                _dataContextListener = null;
            }

            if (ParentBinding.ValidatesOnNotifyDataErrors)
            {
                if (_sourceErrorsChangedListener != null)
                {
                    _sourceErrorsChangedListener.Detach();
                    _sourceErrorsChangedListener = null;
                }

                if (_valueErrorsChangedListener != null)
                {
                    _valueErrorsChangedListener.Detach();
                    _valueErrorsChangedListener = null;
                }

                _dataErrorSource = null;
                _dataErrorValue = null;
            }

            BindingSource = null;
            _propertyPathWalker.Update(null);

            UpdateValidationError(null);
            UpdateNotifyDataErrorValidationErrors(null);

            if (_isUpdateOnLostFocus)
            {
                _isUpdateOnLostFocus = false;
                ((IInternalFrameworkElement)Target).LostFocus -= new RoutedEventHandler(OnTargetLostFocus);
            }

            DetachMentor();

            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);
            Target = null;
        }

        internal void ValueChanged()
        {
            UpdateNotifyDataErrors(_propertyPathWalker.FinalNode.Value);
            UpdateValidationError(GetBaseValidationError());

            Refresh();
        }

        /// <summary>
        /// The element that is the binding target object of this binding expression.
        /// </summary>
        internal DependencyObject Target { get; private set; }

        private object BindingSource
        {
            get => _bindingSource;
            set
            {
                _bindingSource = value;
                if (!ParentBinding.BindsDirectlyToSource)
                {
                    if (_cvsListener != null)
                    {
                        _cvsListener.Dispose();
                        _cvsListener = null;
                    }

                    if (value is CollectionViewSource cvs)
                    {
                        _cvsListener = new DependencyPropertyChangedListener(cvs, CollectionViewSource.ViewProperty, OnCollectionViewSourceViewChanged);
                        _bindingSource = cvs.View;
                    }
                }
            }
        }

        private void OnCollectionViewSourceViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            _bindingSource = args.NewValue;

            _propertyPathWalker.Update(BindingSource);
        }

        private void UpdateNotifyDataErrors(object value)
        {
            if (!ParentBinding.ValidatesOnNotifyDataErrors)
            {
                return;
            }

            UpdateNotifyDataErrors(
                _propertyPathWalker.FinalNode.Source,
                _propertyPathWalker.FinalNode.PropertyName,
                value);
        }

        private void UpdateNotifyDataErrors(object source, string propertyName, object value)
        {
            if (!ParentBinding.ValidatesOnNotifyDataErrors || !IsAttached)
            {
                return;
            }

            if (source != _dataErrorSource)
            {
                if (_sourceErrorsChangedListener != null)
                {
                    _sourceErrorsChangedListener.Detach();
                    _sourceErrorsChangedListener = null;
                }

                _dataErrorSource = source as INotifyDataErrorInfo;

                if (_dataErrorSource != null)
                {
                    _sourceErrorsChangedListener = new(this, _dataErrorSource)
                    {
                        OnEventAction = static (instance, source, args) => instance.OnSourceErrorsChanged(source, args),
                        OnDetachAction = static (listener, source) => source.ErrorsChanged -= listener.OnEvent,
                    };
                    _dataErrorSource.ErrorsChanged += _sourceErrorsChangedListener.OnEvent;
                }
            }

            if (value != DependencyProperty.UnsetValue && value != _dataErrorValue)
            {
                if (_valueErrorsChangedListener != null)
                {
                    _valueErrorsChangedListener.Detach();
                    _valueErrorsChangedListener = null;
                }

                _dataErrorValue = value as INotifyDataErrorInfo;

                if (_dataErrorValue != null)
                {
                    _valueErrorsChangedListener = new(this, _dataErrorValue)
                    {
                        OnEventAction = static (instance, source, args) => instance.OnValueErrorsChanged(source, args),
                        OnDetachAction = static (listener, source) => source.ErrorsChanged -= listener.OnEvent,
                    };
                    _dataErrorValue.ErrorsChanged += _valueErrorsChangedListener.OnEvent;
                }
            }

            try
            {
                List<object> propertyErrors = GetDataErrors(_dataErrorSource, propertyName);
                List<object> valueErrors = GetDataErrors(_dataErrorValue, string.Empty);
                List<object> errors = MergeErrors(propertyErrors, valueErrors);

                UpdateNotifyDataErrorValidationErrors(errors);
            }
            catch (Exception ex)
            {
                if (CriticalExceptions.IsCriticalApplicationException(ex))
                {
                    throw;
                }
            }
        }

        // fetch errors for the given property
        private static List<object> GetDataErrors(INotifyDataErrorInfo indei, string propertyName)
        {
            const int RetryCount = 3;
            List<object> result = null;
            if (indei != null && indei.HasErrors)
            {
                // if a worker thread is updating the source's errors while we're trying to
                // read them, the enumerator will throw.   The interface doesn't provide
                // any way to work around this, so we'll just try it a few times hoping
                // for success.
                for (int i = RetryCount; i >= 0; --i)
                {
                    try
                    {
                        result = new List<object>();
                        IEnumerable ie = indei.GetErrors(propertyName);
                        if (ie != null)
                        {
                            foreach (object o in ie)
                            {
                                result.Add(o);
                            }
                        }
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        // on the last try, let the exception bubble up
                        if (i == 0)
                            throw;
                    }
                }
            }

            if (result != null && result.Count == 0)
                result = null;

            return result;
        }

        private List<object> MergeErrors(List<object> list1, List<object> list2)
        {
            if (list1 == null)
                return list2;
            if (list2 == null)
                return list1;

            foreach (object o in list2)
                list1.Add(o);
            return list1;
        }

        internal void UpdateValidationError(ValidationError validationError)
        {
            // the steps are carefully ordered to avoid going through a "no error"
            // state while replacing one error with another
            ValidationError oldValidationError = _baseValidationError;

            _baseValidationError = validationError;

            if (validationError != null)
            {
                AddValidationError(validationError);
            }

            if (oldValidationError != null)
            {
                RemoveValidationError(oldValidationError);
            }
        }

        private void UpdateNotifyDataErrorValidationErrors(List<object> errors)
        {
            List<object> toAdd;
            List<ValidationError> toRemove;

            GetValidationDelta(_notifyDataErrors, errors, out toAdd, out toRemove);

            // add the new errors, then remove the old ones - this avoid a transient
            // "no error" state
            if (toAdd != null && toAdd.Count > 0)
            {
                List<ValidationError> notifyDataErrors = _notifyDataErrors;

                if (notifyDataErrors == null)
                {
                    notifyDataErrors = new List<ValidationError>();
                    _notifyDataErrors = notifyDataErrors;
                }

                foreach (object o in toAdd)
                {
                    ValidationError veAdd = new ValidationError(this) { ErrorContent = o };
                    notifyDataErrors.Add(veAdd);
                    AddValidationError(veAdd);
                }
            }

            if (toRemove != null && toRemove.Count > 0)
            {
                List<ValidationError> notifyDataErrors = _notifyDataErrors;
                foreach (ValidationError veRemove in toRemove)
                {
                    notifyDataErrors.Remove(veRemove);
                    RemoveValidationError(veRemove);
                }

                if (notifyDataErrors.Count == 0)
                {
                    _notifyDataErrors = null;
                }
            }
        }

        private static void GetValidationDelta(List<ValidationError> previousErrors,
            List<object> errors,
            out List<object> toAdd,
            out List<ValidationError> toRemove)
        {
            // determine the errors to add and the validation results to remove,
            // taking duplicates into account
            if (previousErrors == null || previousErrors.Count == 0)
            {
                toAdd = errors;
                toRemove = null;
            }
            else if (errors == null || errors.Count == 0)
            {
                toAdd = null;
                toRemove = new List<ValidationError>(previousErrors);
            }
            else
            {
                toAdd = new List<object>();
                toRemove = new List<ValidationError>(previousErrors);

                for (int i = errors.Count - 1; i >= 0; --i)
                {
                    object errorContent = errors[i];

                    int j;
                    for (j = toRemove.Count - 1; j >= 0; --j)
                    {
                        if (ItemsControl.EqualsEx(toRemove[j].ErrorContent, errorContent))
                        {
                            // this error appears on both lists - remove it from toRemove
                            toRemove.RemoveAt(j);
                            break;
                        }
                    }

                    if (j < 0)
                    {
                        // this error didn't appear on toRemove - add it to toAdd
                        toAdd.Add(errorContent);
                    }
                }
            }
        }

        private void AddValidationError(ValidationError validationError)
        {
            // add the error to the target element
            Validation.AddValidationError(validationError, Target, ParentBinding.NotifyOnValidationError);
        }

        private void RemoveValidationError(ValidationError validationError)
        {
            // remove the error from the target element
            Validation.RemoveValidationError(validationError, Target, ParentBinding.NotifyOnValidationError);
        }

        private void OnSourceErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == _propertyPathWalker.FinalNode.PropertyName)
            {
                UpdateNotifyDataErrors(_dataErrorSource, e.PropertyName, DependencyProperty.UnsetValue);
            }
        }

        private void OnValueErrorsChanged(object sender, DataErrorsChangedEventArgs e)
            => UpdateNotifyDataErrors(DependencyProperty.UnsetValue);

        internal void OnSourceAvailable(bool lastAttempt)
        {
            AttachToContext(lastAttempt);
            if (BindingSource != null)
            {
                _propertyPathWalker.Update(BindingSource);
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

        internal void UpdateSourceObject(object value)
        {
            if (_propertyPathWalker.IsPathBroken)
            {
                return;
            }

            IsUpdatingSource = true;

            IPropertyPathNode node = _propertyPathWalker.FinalNode;

            object convertedValue = value;
            Type expectedType = node.Type;

            ValidationError vError = null;

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
                    {
                        return;
                    }
                }

                if (!DependencyProperty.IsValidType(convertedValue, expectedType))
                {
#if MIGRATION
                    convertedValue = DynamicConverter.Convert(convertedValue, expectedType, null, ParentBinding.ConverterCulture);
#else
                    convertedValue = DynamicConverter.Convert(convertedValue, expectedType, null, ParentBinding.ConverterLanguage);
#endif

                    if (convertedValue == DependencyProperty.UnsetValue)
                    {
                        return;
                    }
                }

                node.SetValue(convertedValue);
            }
            catch (Exception ex)
            {
                ex = CriticalExceptions.Unwrap(ex);
                if (CriticalExceptions.IsCriticalApplicationException(ex))
                {
                    throw;
                }

                if (ParentBinding.ValidatesOnExceptions)
                {
                    vError = new ValidationError(this)
                    {
                        Exception = ex,
                        ErrorContent = ex.Message,
                    };
                }
            }
            finally
            {
                IsUpdatingSource = false;
            }

            vError ??= GetBaseValidationError();
            UpdateValidationError(vError);
        }

        private ValidationError GetBaseValidationError()
        {
            if (ParentBinding.ValidatesOnDataErrors &&
                _propertyPathWalker.FinalNode.Source is IDataErrorInfo dataErrorInfo)
            {
                string name = _propertyPathWalker.FinalNode.PropertyName;
                string error;
                try
                {
                    error = dataErrorInfo[name];
                }
                catch (Exception ex)
                {
                    if (CriticalExceptions.IsCriticalApplicationException(ex))
                    {
                        throw;
                    }

                    error = null;
                }

                if (!string.IsNullOrEmpty(error))
                {
                    return new ValidationError(this)
                    {
                        ErrorContent = error,
                    };
                }
            }

            return null;
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

        private object ConvertValueImplicitly(object value, DependencyProperty dp)
        {
            if (dp.IsValidType(value))
            {
                return value;
            }

            return UseDynamicConverter(value, dp.PropertyType);
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
                value = ConvertValueImplicitly(ParentBinding.FallbackValue, TargetProperty);
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

        private object GetDefaultValue() => TargetProperty.GetDefaultValue(Target);

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
            OnSourceAvailable(false);
        }

        private void OnMentorLoaded(object sender, RoutedEventArgs e)
        {
            // Note: When the loaded event of this Binding's mentor is raised, a handler could
            // clear this binding. In that case we would still run this handler, even if we
            // detach it in 'OnDetach', so we need make sure the binding is still attached to
            // prevent any unexpected errors.
            if (!IsAttached)
            {
                return;
            }

            ((IInternalFrameworkElement)sender).Loaded -= new RoutedEventHandler(OnMentorLoaded);
            OnSourceAvailable(true);
        }

        private void AttachToContext(bool lastAttempt)
        {
            object source = null;
            IInternalFrameworkElement mentor = null;
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
                if (Target is IInternalFrameworkElement targetFE)
                {
                    DependencyObject contextElement = Target;

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
                        if (contextElement == null && !lastAttempt)
                        {
                            targetFE.Loaded += new RoutedEventHandler(OnMentorLoaded);
                        }
                    }

                    source = contextElement;
                }
                else
                {
                    useMentor = true;
                    source = mentor = FrameworkElement.FindMentor(Target);
                }

                if (_dataContextListener != null)
                {
                    _dataContextListener.Dispose();
                    _dataContextListener = null;
                }

                if (source is IInternalFrameworkElement sourceFE)
                {
                    _dataContextListener = new DependencyPropertyChangedListener(
                        sourceFE.AsDependencyObject(),
                        FrameworkElement.DataContextProperty,
                        OnDataContextChanged);

                    source = sourceFE.GetValue(FrameworkElement.DataContextProperty);
                }
                else
                {
                    Debug.Assert(source is null);
                }
            }

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

            BindingSource = source;
        }

        private void DetachMentor()
        {
            if (_mentor != null)
            {
                _mentor.Loaded -= new RoutedEventHandler(OnMentorLoaded);
                _mentor = null;
            }
        }

        private static object FindName(IInternalFrameworkElement mentor, string name)
        {
            object o = null;
            IInternalFrameworkElement fe = mentor is IUserControl
                ? (mentor.Parent ?? VisualTreeHelper.GetParent(mentor)) as IInternalFrameworkElement
                : mentor;

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
                        if ((fe.Parent ?? VisualTreeHelper.GetParent(fe)) is IPanel panel && panel.IsItemsHost)
                        {
                            dd = (DependencyObject)panel;
                        }
                    }

                    // Last, try inherited context
                    if (dd == null)
                    {
                        dd = fe.AsDependencyObject().InheritanceContext;
                    }

                    fe = FrameworkElement.FindMentor(dd);
                }
            }

            return o;
        }

        private static object FindAncestor(IInternalFrameworkElement mentor, RelativeSource relativeSource)
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
            if (!mentor.IsConnectedToLiveTree)
                return null;

            // get the AncestorLevel and AncestorType:
            int ancestorLevel = relativeSource.AncestorLevel;
            Type ancestorType = relativeSource.AncestorType;
            if (ancestorLevel < 1 || ancestorType == null)
                return null;

            // look for the target's ancestor:
            var currentParent = VisualTreeHelper.GetParent(mentor);
            if (currentParent == null)
                return null;

            while (!ancestorType.IsAssignableFrom(currentParent.GetType()) || --ancestorLevel > 0)
            {
                currentParent = VisualTreeHelper.GetParent(currentParent);
                if (currentParent == null)
                    return null;
            }
            if (ancestorLevel == 0)
                return currentParent;
            return null;
        }

        private void UpdateSourceCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (_status != BindingStatus.None)
            {
                return;
            }

            if (_isUpdateOnLostFocus && ReferenceEquals(FocusManager.GetFocusedElement(), Target))
            {
                _needsUpdate = true;
                return;
            }
            
            try
            {
                UpdateSourceObject(args.NewValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BINDING] UpdateSource: {ex}");
            }
        }

        private void Refresh()
        {
            if (IsAttaching || !IsAttached) return;

            bool oldIsUpdating = IsUpdatingValue;
            IsUpdatingValue = true;
            Target.ApplyExpression(TargetProperty, this, ParentBinding._isInStyle);
            IsUpdatingValue = oldIsUpdating;
        }
    }
}
