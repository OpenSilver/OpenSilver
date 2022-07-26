
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
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Internal.Data
{
    internal class StandardPropertyPathNode : PropertyPathNode
    {
        private readonly Type _resolvedType;
        private readonly string _propertyName;
        private readonly BindingExpression _expression;

        private IPropertyChangedListener _dpListener;
        private DependencyProperty _dp;
        private PropertyInfo _prop;
        private FieldInfo _field;        

        internal StandardPropertyPathNode(BindingExpression expression, PropertyPathWalker listener, string typeName, string propertyName)
            : base(listener)
        {
            _expression = expression;
            _resolvedType = typeName != null ? Type.GetType(typeName) : null;
            _propertyName = propertyName;
        }

        public override Type Type
        {
            get
            {
                if (_dp != null)
                {
                    return _dp.PropertyType;
                }

                if (_prop != null)
                {
                    return _prop.PropertyType;
                }

                if (_field != null)
                {
                    return _field.FieldType;
                }

                return null;
            }
        }

        public override bool IsBound => _dp != null || _prop != null;

        internal override void SetValue(object value)
        {
            if (_dp != null)
            {
                ((DependencyObject)Source).SetValue(_dp, value);
            }
            else if (_prop != null)
            {
                _prop.SetValue(Source, value);
            }
            else if (_field != null)
            {
                _field.SetValue(Source, value);
            }
        }

        internal override void UpdateValue()
        {
            if (_dp != null)
            {
                UpdateValueAndIsBroken(((DependencyObject)Source).GetValue(_dp), CheckIsBroken());
            }
            else if (_prop != null)
            {
                //TODO: this.ValueType = PropertyInfo.PropertyType;
                //this.ValueType = null; //todo: don't know what this is for
                try
                {
#if BRIDGE
                    //Bridge throws an exception when trying to call GetValue through PropertyInfo.GetValue on a Static Property while putting an instance as a parameter (which should not be the case in my opinion).
                    //Because of that, we need to check whether the property is Static and then accordingly call GetValue with either null or the instance as a parameter.
                    object propertyValue = null;
                    MethodInfo methodInfo = _prop.GetMethod;
                    if (INTERNAL_BridgeWorkarounds.MethodInfoIsStatic_SimulatorCompatible(methodInfo))
                    {
                        propertyValue = _prop.GetValue(null);
                    }
                    else
                    {
                        propertyValue = _prop.GetValue(this.Source);
                    }
                    UpdateValueAndIsBroken(propertyValue, CheckIsBroken());
#else
                    UpdateValueAndIsBroken(_prop.GetValue(this.Source), CheckIsBroken());
#endif
                }
                catch
                {
                    UpdateValueAndIsBroken(null, CheckIsBroken());
                }
            }
            else if (_field != null)
            {
                try
                {
                    UpdateValueAndIsBroken(_field.GetValue(Source), CheckIsBroken());
                }
                catch
                {
                    UpdateValueAndIsBroken(null, CheckIsBroken());
                }
            }
            else
            {
                UpdateValueAndIsBroken(null, CheckIsBroken());
            }
        }

        internal bool EnableNotifyDataErrorChanges { get; set; } = false;

        internal override void OnSourceChanged(object oldValue, object newValue)
        {
            if (Listener.ListenForChanges && oldValue is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= new PropertyChangedEventHandler(OnSourcePropertyChanged);
            }

            IPropertyChangedListener listener = _dpListener;
            if (listener != null)
            {
                _dpListener = null;
                listener.Detach();
            }

            _dp = null;
            _prop = null;
            _field = null;

            UpdateBindingForNotifyDataErrorInfo(oldValue, false);

            if (Source == null)
                return;

            if (Listener.ListenForChanges)
            {
                inpc = newValue as INotifyPropertyChanged;
                if (inpc != null)
                {
                    inpc.PropertyChanged += new PropertyChangedEventHandler(OnSourcePropertyChanged);
                }
            }

            if (newValue is DependencyObject sourceDO)
            {
                Type type = _resolvedType ?? Source.GetType();

                DependencyProperty dependencyProperty = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(type, _propertyName);

                if (dependencyProperty != null)
                {
                    _dp = dependencyProperty;
                    if (Listener.ListenForChanges)
                    {
                        _dpListener = INTERNAL_PropertyStore.ListenToChanged(sourceDO, dependencyProperty, OnPropertyChanged);
                    }
                }
            }

            if (_dp == null)
            {
                Type sourceType = Source.GetType();
                for (Type t = sourceType; t != null; t = t.BaseType)
                {
                    _prop = t.GetProperty(
                        _propertyName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
                    );

                    if (_prop != null)
                    {
                        break;
                    }
                }

                if (_prop == null)
                {
                    // Try in case it is a simple field instead of a property:
                    _field = sourceType.GetField(_propertyName);
                }
            }

            UpdateBindingForNotifyDataErrorInfo(newValue, true);
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == _propertyName || string.IsNullOrEmpty(e.PropertyName)) && (_prop != null || _field != null))
            {
                UpdateValue();

                IPropertyPathNode next = Next;
                if (next != null)
                {
                    next.Source = Value;
                }
            }
        }

        private void OnPropertyChanged(object sender, IDependencyPropertyChangedEventArgs args)
        {
            try
            {
                UpdateValue();
                if (Next != null)
                {
                    Next.Source = Value;
                }
            }
            catch (XamlParseException ex)
            {
                if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                    throw ex;
                else
                    MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                //Ignore
                Debug.WriteLine("Binding exception: " + ex.ToString());
            }
        }

        private bool CheckIsBroken()
        {
            return Source == null || (_prop == null && _field == null && _dp == null);
        }

        private void UpdateBindingForNotifyDataErrorInfo(object source, bool attach)
        {
            if (!_expression.ParentBinding.NotifyOnValidationError || !EnableNotifyDataErrorChanges) return;

            var ndei = source as INotifyDataErrorInfo;
            if (ndei != null)
            {
                if (attach)
                {
                    ndei.ErrorsChanged += NotifyDataErrorInfo_ErrorsChanged;
                }
                else
                {
                    ndei.ErrorsChanged -= NotifyDataErrorInfo_ErrorsChanged;
                }
            }
        }

        private void NotifyDataErrorInfo_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            var notifyDataErrorInfo = sender as INotifyDataErrorInfo;
            if (notifyDataErrorInfo != null)
            {
                if (e.PropertyName == _propertyName)
                {
                    if (notifyDataErrorInfo.HasErrors)
                    {
                        var errors = notifyDataErrorInfo.GetErrors(_propertyName);
                        if (errors != null)
                        {
                            foreach (var error in errors)
                            {
                                if (error != null)
                                {
                                    Validation.MarkInvalid(_expression, new ValidationError(_expression) { ErrorContent = error.ToString() });
                                }
                            }
                        }
                    }
                    else
                    {
                        Validation.ClearInvalid(_expression);
                    }
                }
            }
        }
    }
}
