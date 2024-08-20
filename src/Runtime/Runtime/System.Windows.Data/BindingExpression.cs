
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
        private object _bindingSource;
        private IInternalFrameworkElement _mentor;
        private ValidationError _baseValidationError;
        private List<ValidationError> _notifyDataErrors;

        private DependencyPropertyChangedListener _dataContextListener;
        private DependencyPropertyChangedListener _cvsListener;
        private WeakEventListener<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs> _sourceErrorsChangedListener;
        private WeakEventListener<BindingExpression, INotifyDataErrorInfo, DataErrorsChangedEventArgs> _valueErrorsChangedListener;
        private INotifyDataErrorInfo _dataErrorSource;
        private INotifyDataErrorInfo _dataErrorValue;

        private readonly PropertyPathWalker _propertyPathWalker;

        private BindingExpression(Binding binding, BindingExpressionBase owner)
            : base(binding, owner)
        {
            _propertyPathWalker = new PropertyPathWalker(this);
        }

        // Create a new BindingExpression from the given Bind description
        internal static BindingExpression CreateBindingExpression(DependencyProperty dp, Binding binding, BindingExpressionBase parent)
        {
            if (dp.ReadOnly)
            {
                throw new ArgumentException($"'{dp.Name}' property cannot be data-bound.", nameof(dp));
            }

            // create the BindingExpression
            var bindExpr = new BindingExpression(binding, parent);

            // Two-way Binding with an empty path makes no sense
            if (bindExpr.IsReflective && (binding.Path.Path == string.Empty || binding.Path.Path == "."))
            {
                throw new InvalidOperationException("Two-way binding requires Path.");
            }

            return bindExpr;
        }

        private void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            BindingSource = args.NewValue;

            _propertyPathWalker.AttachDataItem(BindingSource, true);
        }

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
        public object DataItem => BindingSource;

        /// <summary>
        /// Sends the current binding target value to the binding source property in
        /// <see cref="BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BindingExpression"/> is detached from the binding target.
        /// </exception>
        public void UpdateSource()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException("The Binding has been detached from its target.");
            }

            NeedsUpdate = true;
            Update();
        }

        internal override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            object value;

            if (_propertyPathWalker.IsPathBroken)
            {
                //------------------------
                // BROKEN PATH
                //------------------------

                if (_dataContextListener != null && _propertyPathWalker.IsEmpty)
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

            if (value != DependencyProperty.UnsetValue)
            {
                if (ParentBinding.Converter != null)
                {
                    value = ParentBinding.Converter.Convert(value,
                        TargetProperty.PropertyType,
                        ParentBinding.ConverterParameter,
                        ParentBinding.ConverterCulture);
                }
            }

            if (value == DependencyProperty.UnsetValue)
            {
                value = ParentBinding.FallbackValue ?? DefaultValue;
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

            if (BindingSource is not null)
            {
                _propertyPathWalker.AttachDataItem(BindingSource, false);
            }
        }

        internal override void DetachOverride()
        {
            if (_dataContextListener != null)
            {
                _dataContextListener.Dispose();
                _dataContextListener = null;
            }

            if (ValidatesOnNotifyDataErrors)
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
            _propertyPathWalker.DetachDataItem();

            UpdateValidationError(null);
            UpdateNotifyDataErrorValidationErrors(null);

            DetachMentor();

            Target.InheritedContextChanged -= new EventHandler(OnTargetInheritedContextChanged);

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

            _propertyPathWalker.AttachDataItem(BindingSource, true);
        }

        private void UpdateNotifyDataErrors(object value)
        {
            if (!ValidatesOnNotifyDataErrors)
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
            if (!ValidatesOnNotifyDataErrors || !IsAttached)
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

            if (value != _dataErrorValue)
            {
                if (_valueErrorsChangedListener != null)
                {
                    _valueErrorsChangedListener.Detach();
                    _valueErrorsChangedListener = null;
                }

                _dataErrorValue = null;

                if (value != DependencyProperty.UnsetValue)
                {
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
                _propertyPathWalker.AttachDataItem(BindingSource, true);
            }
        }

        internal override void Update()
        {
            if (!NeedsUpdate || !IsReflective || IsInTransfer || _propertyPathWalker.IsPathBroken)
            {
                return;
            }

            object convertedValue = GetRawProposedValue();

            ValidationError vError = null;

            try
            {
                convertedValue = ConvertProposedValue(convertedValue);

                if (convertedValue == DependencyProperty.UnsetValue)
                {
                    return;
                }

                UpdateSource(convertedValue);
            }
            catch (Exception ex)
            {
                ex = CriticalExceptions.Unwrap(ex);
                if (CriticalExceptions.IsCriticalApplicationException(ex))
                {
                    throw;
                }

                if (ValidatesOnExceptions)
                {
                    vError = new ValidationError(this)
                    {
                        Exception = ex,
                        ErrorContent = ex.Message,
                    };
                }
            }

            vError ??= GetBaseValidationError();
            UpdateValidationError(vError);
        }

        private object GetRawProposedValue() => Target.GetValue(TargetProperty);

        private object ConvertProposedValue(object value)
        {
            object convertedValue = value;
            Type expectedType = _propertyPathWalker.FinalNode.Type;

            if (expectedType != null && ParentBinding.Converter != null)
            {
                convertedValue = ParentBinding.Converter.ConvertBack(convertedValue,
                    expectedType,
                    ParentBinding.ConverterParameter,
                    ParentBinding.ConverterCulture);
            }

            if (convertedValue != DependencyProperty.UnsetValue && !DependencyProperty.IsValidType(convertedValue, expectedType))
            {
                convertedValue = DynamicConverter.Convert(convertedValue,
                    expectedType,
                    null,
                    ParentBinding.ConverterCulture);
            }

            return convertedValue;
        }

        internal void UpdateSource(object convertedValue)
        {
            BeginSourceUpdate();
            try
            {
                _propertyPathWalker.FinalNode.SetValue(convertedValue);
            }
            finally
            {
                EndSourceUpdate();
            }
        }

        private ValidationError GetBaseValidationError()
        {
            if (ValidatesOnDataErrors && _propertyPathWalker.FinalNode.Source is IDataErrorInfo dataErrorInfo)
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
                convertedValue = DynamicConverter.Convert(value,
                    targetType,
                    ParentBinding.ConverterParameter,
                    ParentBinding.ConverterCulture);
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
                value = DefaultValue;
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
                value = DefaultValue;
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
    }
}
