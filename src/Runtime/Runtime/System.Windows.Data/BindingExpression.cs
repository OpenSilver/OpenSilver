
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

using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows.Data
{
    /// <summary>
    /// Contains information about a single instance of a <see cref="Binding" />.
    /// </summary>
    public sealed class BindingExpression : BindingExpressionBase
    {
        private DynamicValueConverter _dynamicConverter;
        private object _dataItem;
        private IInternalFrameworkElement _mentor;
        private ValidationError _baseValidationError;
        private List<ValidationError> _notifyDataErrors;
        private object _effectiveTargetNullValue = DefaultValueObject;

        private PropertyChangeListener _dataContextListener;
        private PropertyChangeListener _cvsListener;
        private WeakEventToken _weakDataChangedEventToken;
        private WeakEventToken _weakSourceErrorsChangedEventToken;
        private WeakEventToken _weakValueErrorsChangedEventToken;
        private INotifyDataErrorInfo _dataErrorSource;
        private INotifyDataErrorInfo _dataErrorValue;

        private readonly PropertyPathWalker _propertyPathWalker;

        private BindingExpression(Binding binding, BindingExpressionBase owner)
            : base(binding, owner)
        {
            _propertyPathWalker = new PropertyPathWalker(this);
        }

        // Create a new BindingExpression from the given Bind description
        internal static BindingExpression CreateBindingExpression(DependencyObject d, DependencyProperty dp, Binding binding, BindingExpressionBase parent)
        {
            FrameworkPropertyMetadata fwMetaData = dp.GetMetadata(d.DependencyObjectType) as FrameworkPropertyMetadata;

            if ((fwMetaData is not null && !fwMetaData.IsDataBindingAllowed) || dp.ReadOnly)
            {
                throw new ArgumentException(string.Format(Strings.PropertyNotBindable, dp.Name), nameof(dp));
            }

            // create the BindingExpression
            var bindExpr = new BindingExpression(binding, parent);

            bindExpr.ResolvePropertyDefaultSettings(binding.GetMode(), binding.UpdateSourceTrigger, fwMetaData);

            bindExpr.SaveDefaultFlags();

            // Two-way Binding with an empty path makes no sense
            if (bindExpr.IsReflective && (string.IsNullOrEmpty(binding.Path.Path) || binding.Path.Path == "."))
            {
                throw new InvalidOperationException(Strings.TwoWayBindingNeedsPath);
            }

            return bindExpr;
        }

        private void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => Activate(args.NewValue);

        /// <summary>
        /// The binding target property of this binding expression.
        /// </summary>
        public new DependencyProperty TargetProperty => base.TargetProperty;

        /// <summary>
        /// The <see cref="Binding"/> object of the current <see cref="BindingExpression"/>.
        /// </summary>
        public Binding ParentBinding => Unsafe.As<Binding>(ParentBindingBase);

        /// <summary>
        /// Gets the binding source object that this <see cref="BindingExpression"/> uses.
        /// </summary>
        /// <returns>
        /// The binding source object that this <see cref="BindingExpression"/> uses.
        /// </returns>
        public object DataItem => _dataItem;

        /// <summary>
        /// Gets the binding source object for this <see cref="BindingExpression"/>.
        /// </summary>
        /// <returns>
        /// The binding source object for this <see cref="BindingExpression"/>.
        /// </returns>
        public object ResolvedSource => SourceItem;

        /// <summary>
        /// Gets the name of the binding source property for this <see cref="BindingExpression"/>.
        /// </summary>
        /// <returns>
        /// The name of the binding source property for this <see cref="BindingExpression"/>.
        /// </returns>
        public string ResolvedSourcePropertyName => SourcePropertyName;

        /// <summary>
        /// Sends the current binding target value to the binding source property in
        /// <see cref="BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BindingExpression"/> is detached from the binding target.
        /// </exception>
        public override void UpdateSource()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException(Strings.BindingExpressionIsDetached);
            }

            NeedsUpdate = true;
            Update();
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object value;

            if (_propertyPathWalker.IsPathBroken)
            {
                value = UseFallbackValue();
            }
            else
            {
                value = GetConvertedValue(_propertyPathWalker.FinalNode.Value);
            }

            return value;
        }

        private object GetConvertedValue(object rawValue)
        {
            Type targetType = GetEffectiveTargetType();

            object value = rawValue;

            if (value != DependencyProperty.UnsetValue)
            {
                if (ParentBinding.Converter != null)
                {
                    value = ParentBinding.Converter.Convert(value,
                        targetType,
                        ParentBinding.ConverterParameter,
                        GetCulture());

                    if (IsDetached)
                    {
                        // user code detached the binding.  Give up.
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            if (value == DependencyProperty.UnsetValue)
            {
                if (ParentBinding.FallbackValue != null)
                {
                    value = ParentBinding.FallbackValue;
                }
                else if (!IsInBindingExpressionCollection)
                {
                    value = DefaultValue;
                }
            }

            if (value != DependencyProperty.UnsetValue)
            {
                if (value is null)
                {
                    value = EffectiveTargetNullValue;
                }
                else
                {
                    value = ApplyStringFormat(value);

                    // chain in a default value converter if the returned value's type is not compatible with the targetType
                    if (value != null &&
                        value != DependencyProperty.UnsetValue &&
                        !targetType.IsAssignableFrom(value.GetType()))
                    {
                        value = ConvertHelper(value, targetType, Target, GetCulture());
                    }
                }
            }

            // if the value isn't acceptable to the target property, don't use it
            // (in MultiBinding, the value will go through the multi-converter, so
            // it's too early to make this judgment)
            if (!IsInMultiBindingExpression && value != DependencyProperty.UnsetValue && !TargetProperty.IsValidValue(value))
            {
                value = DependencyProperty.UnsetValue;

                if (Status == BindingStatus.Active)
                {
                    SetStatus(BindingStatus.UpdateTargetError);
                }
            }

            if (value == DependencyProperty.UnsetValue)
            {
                value = UseFallbackValue();
            }

            return value;
        }

        internal override bool CanSetValue(DependencyObject d, DependencyProperty dp) => IsReflective;

        internal void SetValue(object value)
        {
            if (IsReflective && !_propertyPathWalker.IsPathBroken)
            {
                _propertyPathWalker.FinalNode.Value = value;
            }
        }

        internal override void AttachOverride(DependencyObject d, DependencyProperty dp)
        {
            base.AttachOverride(d, dp);

            AttachToContext(false);
        }

        internal override void DetachOverride()
        {
            _dataContextListener?.Dispose();
            _dataContextListener = null;

            if (ValidatesOnNotifyDataErrors)
            {
                _weakSourceErrorsChangedEventToken?.Dispose();
                _weakSourceErrorsChangedEventToken = null;

                _weakValueErrorsChangedEventToken?.Dispose();
                _weakValueErrorsChangedEventToken = null;

                _dataErrorSource = null;
                _dataErrorValue = null;
            }

            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);

            SetMentor(null);

            _dataItem = null;
            _propertyPathWalker.DetachDataItem();
            SetStatus(BindingStatus.Inactive);

            UpdateValidationError(null);
            UpdateNotifyDataErrorValidationErrors(null);

            base.DetachOverride();
        }

        /// <summary>
        /// Invalidate the given child expression.
        /// </summary>
        internal override void InvalidateChild(BindingExpressionBase bindingExpression)
        {
            // BindingExpression does not support child bindings
        }

        internal void TransferValue(object newValue)
        {
            IsInTransfer = true;

            UpdateNotifyDataErrors(newValue);
            UpdateValidationError(GetBaseValidationError());

            RaiseValueChanged();

            if (ParentBindingExpressionBase != null)
            {
                ParentBindingExpressionBase.InvalidateChild(this);
            }
            else
            {
                Invalidate();
            }

            IsInTransfer = false;
        }

        // MultiBinding looks at this to find out what type its MultiValueConverter should
        // convert back to, when this BindingExpression is not using a user-specified converter.
        internal Type ConverterSourceType =>
            _propertyPathWalker.IsPathBroken ? TargetProperty.PropertyType : _propertyPathWalker.FinalNode.Type;

        private DynamicValueConverter DynamicConverter => _dynamicConverter ??= new DynamicValueConverter(IsReflective);

        private object EffectiveTargetNullValue
        {
            get
            {
                if (_effectiveTargetNullValue == DefaultValueObject)
                {
                    _effectiveTargetNullValue = ConvertValue(ParentBinding.TargetNullValue, TargetProperty);
                }
                return _effectiveTargetNullValue;
            }
        }

        // the item whose property changes when we UpdateSource
        private object SourceItem => _propertyPathWalker.FinalNode.Source;

        // the name of the property that changes when we UpdateSource
        private string SourcePropertyName => _propertyPathWalker.FinalNode.PropertyName;

        private void OnCollectionViewSourceViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => Activate(args.NewValue);

        private void OnDataChanged(object sender, EventArgs e) => Activate(((DataSourceProvider)sender).Data);

        private void UpdateNotifyDataErrors(object value)
        {
            if (!ValidatesOnNotifyDataErrors)
            {
                return;
            }

            UpdateNotifyDataErrors(SourceItem, SourcePropertyName, value);
        }

        private void UpdateNotifyDataErrors(object source, string propertyName, object value)
        {
            if (!ValidatesOnNotifyDataErrors || !IsAttached)
            {
                return;
            }

            if (source != _dataErrorSource)
            {
                _weakSourceErrorsChangedEventToken?.Dispose();
                _weakSourceErrorsChangedEventToken = null;

                _dataErrorSource = source as INotifyDataErrorInfo;

                if (_dataErrorSource != null)
                {
                    _weakSourceErrorsChangedEventToken = WeakEvent.Subscribe<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs>(
                        this,
                        _dataErrorSource,
                        static (instance, source, args) => instance.OnSourceErrorsChanged(source, args),
                        static (handler, source) => source.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(handler),
                        static (handler, source) => source.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(handler));
                }
            }

            if (value != _dataErrorValue)
            {
                _weakValueErrorsChangedEventToken?.Dispose();
                _weakValueErrorsChangedEventToken = null;

                _dataErrorValue = null;

                if (value != DependencyProperty.UnsetValue)
                {
                    _dataErrorValue = value as INotifyDataErrorInfo;

                    if (_dataErrorValue != null)
                    {
                        _weakValueErrorsChangedEventToken = WeakEvent.Subscribe<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs>(
                            this,
                            _dataErrorValue,
                            static (instance, source, args) => instance.OnValueErrorsChanged(source, args),
                            static (handler, source) => source.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(handler),
                            static (handler, source) => source.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(handler));
                    }
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
            if (e.PropertyName == SourcePropertyName)
            {
                UpdateNotifyDataErrors(_dataErrorSource, e.PropertyName, DependencyProperty.UnsetValue);
            }
        }

        private void OnValueErrorsChanged(object sender, DataErrorsChangedEventArgs e)
            => UpdateNotifyDataErrors(DependencyProperty.UnsetValue);

        internal void OnSourceAvailable(bool lastAttempt)
        {
            AttachToContext(lastAttempt);

            if (_dataItem is null && lastAttempt)
            {
                if (IsInBindingExpressionCollection && ParentBindingExpressionBase.Status == BindingStatus.Unattached)
                {
                    ParentBindingExpressionBase.InvalidateChild(this);
                }
            }
        }

        // Return the object from which the given value was obtained, if possible
        internal override object GetSourceItem(object newValue) => SourceItem;

        internal override void Update()
        {
            if (!NeedsUpdate || !IsReflective || IsInTransfer || _propertyPathWalker.IsPathBroken)
            {
                return;
            }

            ValidationError oldValidationError = _baseValidationError;

            if (Status == BindingStatus.UpdateSourceError)
            {
                SetStatus(BindingStatus.Active);
            }

            object rawValue = GetRawProposedValue();
            Type expectedType = _propertyPathWalker.FinalNode.Type;
            object convertedValue = rawValue;

            if (expectedType != null && ParentBinding.Converter != null)
            {
                convertedValue = ConvertBackHelper(ParentBinding.Converter,
                    convertedValue,
                    expectedType,
                    ParentBinding.ConverterParameter,
                    GetCulture());

                if (!Validate(convertedValue))
                {
                    return;
                }
            }

            if (!DependencyProperty.IsValidType(convertedValue, expectedType))
            {
                convertedValue = ConvertBackHelper(DynamicConverter,
                    convertedValue,
                    expectedType,
                    null,
                    GetCulture());
            }

            if (!Validate(convertedValue))
            {
                if (_baseValidationError == oldValidationError)
                {
                    UpdateValidationError(new ValidationError(this)
                    {
                        ErrorContent = string.Format(Strings.Validation_ConversionFailed, rawValue),
                    });
                }
                return;
            }

            convertedValue = UpdateSource(convertedValue);
            if (!Validate(convertedValue))
            {
                return;
            }

            if (_baseValidationError == oldValidationError)
            {
                UpdateValidationError(GetBaseValidationError());
            }
        }

        private object GetRawProposedValue() => Target.GetValue(TargetProperty);

        internal object UpdateSource(object value)
        {
            // If there is a failure to convert, then Update failed.
            if (value == DependencyProperty.UnsetValue)
            {
                SetStatus(BindingStatus.UpdateSourceError);
                return value;
            }

            BeginSourceUpdate();

            try
            {
                _propertyPathWalker.FinalNode.SetValue(value);
            }
            catch (Exception ex)
            {
                ex = CriticalExceptions.Unwrap(ex);
                if (CriticalExceptions.IsCriticalApplicationException(ex))
                {
                    throw;
                }

                ProcessException(ex, ValidatesOnExceptions);
                SetStatus(BindingStatus.UpdateSourceError);
                value = DependencyProperty.UnsetValue;
            }
            finally
            {
                EndSourceUpdate();
            }

            return value;
        }

        private object ConvertBackHelper(IValueConverter converter, object value, Type sourceType, object parameter, CultureInfo culture)
        {
            object convertedValue;
            try
            {
                convertedValue = converter.ConvertBack(value, sourceType, parameter, culture);
            }
            catch (Exception ex)
            {
                ex = CriticalExceptions.Unwrap(ex);
                if (CriticalExceptions.IsCriticalApplicationException(ex))
                {
                    throw;
                }

                ProcessException(ex, ValidatesOnExceptions);
                convertedValue = DependencyProperty.UnsetValue;
            }
            return convertedValue;
        }

        private void ProcessException(Exception ex, bool validate)
        {
            if (!validate)
            {
                return;
            }

            UpdateValidationError(new ValidationError(this)
            {
                Exception = ex,
                ErrorContent = ex.Message,
            });
        }

        private ValidationError GetBaseValidationError()
        {
            if (ValidatesOnDataErrors && SourceItem is IDataErrorInfo dataErrorInfo)
            {
                string name = SourcePropertyName;
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

        private object ConvertValue(object value, DependencyProperty dp)
        {
            object result;

            if (value == DependencyProperty.UnsetValue || dp.IsValidValue(value))
            {
                result = value;
            }
            else
            {
                result = ConvertHelper(value,
                    dp.PropertyType,
                    Target,
                    GetCulture());
            }

            return result;
        }

        private object ConvertHelper(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object convertedValue;
            try
            {
                convertedValue = DynamicConverter.Convert(value, targetType, parameter, culture);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                convertedValue = DependencyProperty.UnsetValue;
            }

            return convertedValue;
        }

        private object UseFallbackValue()
        {
            object value = DependencyProperty.UnsetValue;

            if (ParentBinding.FallbackValue != null)
            {
                value = ConvertValue(ParentBinding.FallbackValue, TargetProperty);
            }

            // OneWayToSource bindings should initialize to the Fallback/Default
            // value without error
            if (value == DependencyProperty.UnsetValue && IsOneWayToSource)
            {
                value = DefaultValue;
            }

            if (value == DependencyProperty.UnsetValue)
            {
                if (Status == BindingStatus.Active)
                {
                    SetStatus(BindingStatus.UpdateTargetError);
                }

                if (!IsInBindingExpressionCollection)
                {
                    value = DefaultValue;
                }
            }

            return value;
        }

        private object ApplyStringFormat(object value)
        {
            object result = value;

            string stringFormat = GetEffectiveStringFormat();
            if (stringFormat != null)
            {
                try
                {
                    result = string.Format(stringFormat, value);
                }
                catch (FormatException fe)
                {
                    HandleException(fe);
                    result = DependencyProperty.UnsetValue;
                }
            }

            return result;
        }

        private void OnTargetInheritedContextChanged(object sender, EventArgs e)
        {
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
            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);
            _dataContextListener?.Dispose();
            _dataContextListener = null;
            SetMentor(null);

            IInternalFrameworkElement mentor = FrameworkElement.FindMentor(Target);

            if (mentor is null)
            {
                if (lastAttempt)
                {
                    SetStatus(BindingStatus.PathError);
                    return;
                }

                Target.InheritedContextChanged += new EventHandler(OnTargetInheritedContextChanged);
                return;
            }

            Binding binding = ParentBinding;

            object source;
            bool useMentor;

            if (binding.Source is not null)
            {
                source = binding.Source;
                useMentor = false;
            }
            else if (binding.ElementName is not null)
            {
                source = FindName(mentor, binding.ElementName);
                useMentor = true;
            }
            else if (binding.RelativeSource is not null)
            {
                RelativeSource relativeSource = binding.RelativeSource;

                (source, useMentor) = relativeSource.Mode switch
                {
                    RelativeSourceMode.Self => (Target, false),
                    RelativeSourceMode.TemplatedParent => (mentor.TemplatedParent, true),
                    RelativeSourceMode.FindAncestor => (FindAncestorOftype(mentor, relativeSource.AncestorType, relativeSource.AncestorLevel), true),
                    _ => (null, false),
                };
            }
            else
            {
                DependencyObject contextElement = mentor.AsDependencyObject();

                // special cases:
                // 1. if target property is DataContext, use the target's parent.
                //      This enables <X DataContext="{Binding...}"/>
                // 2. if the target is ContentPresenter and the target property
                //      is Content, use the parent.  This enables
                //          <ContentPresenter Content="{Binding...}"/>
                if (TargetProperty == FrameworkElement.DataContextProperty ||
                    (TargetProperty == ContentPresenter.ContentProperty && Target is ContentPresenter))
                {
                    contextElement = LogicalTreeHelper.GetParent(contextElement) ?? VisualTreeHelper.GetParent(contextElement);
                }

                if (contextElement is IInternalFrameworkElement sourceFE)
                {
                    useMentor = false;

                    _dataContextListener = PropertyChangeListener.CreateListener(
                        sourceFE.AsDependencyObject(),
                        FrameworkElement.DataContextProperty,
                        OnDataContextChanged);

                    source = sourceFE.GetValue(FrameworkElement.DataContextProperty);
                }
                else
                {
                    useMentor = true;
                    source = null;
                }
            }

            if (source is null)
            {
                if (lastAttempt)
                {
                    SetStatus(BindingStatus.PathError);
                    return;
                }

                if (useMentor)
                {
                    SetMentor(mentor);
                    return;
                }

                return;
            }

            SetStatus(BindingStatus.Inactive);
            Activate(source);
        }

        private void Activate(object item)
        {
            if (!ParentBinding.BindsDirectlyToSource)
            {
                _cvsListener?.Dispose();
                _cvsListener = null;

                _weakDataChangedEventToken?.Dispose();
                _weakDataChangedEventToken = null;

                if (item is CollectionViewSource cvs)
                {
                    _cvsListener = PropertyChangeListener.CreateListener(
                        cvs,
                        CollectionViewSource.ViewProperty,
                        OnCollectionViewSourceViewChanged);

                    item = cvs.View;
                }
                else if (item is DataSourceProvider dsp)
                {
                    _weakDataChangedEventToken = WeakEvent.Subscribe<BindingExpression, DataSourceProvider, EventArgs>(
                        this,
                        dsp,
                        static (instance, source, args) => instance.OnDataChanged(source, args),
                        static (handler, source) => source.DataChanged -= new EventHandler(handler),
                        static (handler, source) => source.DataChanged += new EventHandler(handler));

                    item = dsp.Data;
                }
            }

            _dataItem = item;

            // mark the BindingExpression active
            SetStatus(BindingStatus.Active);

            // attach to data item (may set error status)
            _propertyPathWalker.AttachDataItem(item, !IsAttaching);
        }

        private void SetMentor(IInternalFrameworkElement mentor)
        {
            if (_mentor == mentor)
            {
                return;
            }

            _mentor?.Loaded -= new RoutedEventHandler(OnMentorLoaded);
            _mentor = mentor;
            _mentor?.Loaded += new RoutedEventHandler(OnMentorLoaded);
        }

        internal static object FindName(IInternalFrameworkElement mentor, string name)
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

        internal static object FindAncestorOftype(IInternalFrameworkElement mentor, Type type, int level)
        {
            Debug.Assert(mentor is not null);

            if (type is null || level < 1)
            {
                return null;
            }

            DependencyObject ancestor = GetAncestor(mentor.AsDependencyObject());

            while (ancestor is not null)
            {
                if (type.IsInstanceOfType(ancestor))
                {
                    if (--level <= 0)
                    {
                        break;
                    }
                }

                ancestor = GetAncestor(ancestor);
            }

            return ancestor;

            static DependencyObject GetAncestor(DependencyObject d)
            {
                if (d is null)
                {
                    return null;
                }

                return VisualTreeHelper.GetParent(d) ?? LogicalTreeHelper.GetParent(d) ?? d.InheritanceContext;
            }
        }
    }
}
