
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    class StandardPropertyPathNode : PropertyPathNode
    {
        //private string _STypeName;
        private string _propertyName;
        internal bool BindsDirectlyToSource = false;
        //PropertyInfo PropertyInfo;
        private IPropertyChangedListener _dependencyPropertyListener;

        internal StandardPropertyPathNode(string typeName, string propertyName) {
            //_STypeName = typeName;
            _propertyName = propertyName;
        }

        /// <summary>
        /// This constructor is called only when there is no path, which means that the binding's source is directly the value we are looking for.
        /// </summary>
        internal StandardPropertyPathNode() {
            BindsDirectlyToSource = true;
        }

        internal override void SetValue(object value) {
            if (DependencyProperty != null)
                ((DependencyObject)Source).SetValue(DependencyProperty, value);
            else if (PropertyInfo != null)
                this.PropertyInfo.SetValue(this.Source, value);
            else if (FieldInfo != null)
                this.FieldInfo.SetValue(this.Source, value);
        }

        internal override void UpdateValue() {
            if (DependencyProperty != null)
            {
                //this.ValueType = this.DependencyProperty.GetTargetType();

                UpdateValueAndIsBroken(((DependencyObject)Source).GetValue(DependencyProperty), CheckIsBroken());
            }
            else if (PropertyInfo != null)
            {
                //TODO: this.ValueType = PropertyInfo.PropertyType;
                //this.ValueType = null; //todo: don't know what this is for
                try
                {
#if BRIDGE
                    //Bridge throws an exception when trying to call GetValue through PropertyInfo.GetValue on a Static Property while putting an instance as a parameter (which should not be the case in my opinion).
                    //Because of that, we need to check whether the property is Static and then accordingly call GetValue with either null or the instance as a parameter.
                    object propertyValue = null;
                    MethodInfo methodInfo = PropertyInfo.GetMethod;
                    if (INTERNAL_BridgeWorkarounds.MethodInfoIsStatic_SimulatorCompatible(methodInfo))
                    {
                        propertyValue = PropertyInfo.GetValue(null);
                    }
                    else
                    {
                        propertyValue = PropertyInfo.GetValue(this.Source);
                    }
                    UpdateValueAndIsBroken(propertyValue, CheckIsBroken());
#else
                    UpdateValueAndIsBroken(PropertyInfo.GetValue(this.Source), CheckIsBroken());
#endif
                }
                catch
                {
                    UpdateValueAndIsBroken(null, CheckIsBroken());
                }
            }
            else if (FieldInfo != null)
            {
                try
                {
                    UpdateValueAndIsBroken(FieldInfo.GetValue(this.Source), CheckIsBroken());
                }
                catch
                {
                    UpdateValueAndIsBroken(null, CheckIsBroken());
                }
            }
            else if (BindsDirectlyToSource)
            {
                this.UpdateValueAndIsBroken(Source, CheckIsBroken(BindsDirectlyToSource));
            }
            else
            {
                //this.ValueType = null;
                this.UpdateValueAndIsBroken(null, CheckIsBroken());
            }
        }

        internal override void OnSourceChanged(object oldvalue, object newValue)
        {
            DependencyObject oldSource = null;
            DependencyObject newSource = null;
            if (oldvalue is DependencyObject)
            {
                oldSource = (DependencyObject)oldvalue;
            }
            if (newValue is DependencyObject)
            {
                newSource = (DependencyObject)newValue;
            }

            var listener = _dependencyPropertyListener;
            if (listener != null)
            {
                listener.Detach();
                _dependencyPropertyListener = listener = null;
            }

            DependencyProperty = null;
            PropertyInfo = null;
            FieldInfo = null;
            if (Source == null)
                return;

            if (newSource != null)
            {
                if (!BindsDirectlyToSource)
                {
                    DependencyProperty dependencyProperty;
                    if (_propertyName == "DataContext") // Note: we handle the special case of the DataContext because the "DataContext" property is defined in the "UIElement" class, and some classes such as "RotateTransform" inherit the DataContext property even though they are not UIElements, so if we looked for the "DataContext" property in the type (and its base types) we wouldn't find it.
                    {
                        dependencyProperty = FrameworkElement.DataContextProperty;
                    }
                    else
                    {
                        Type type = Source.GetType();
                        dependencyProperty = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(type, _propertyName);
                    }

                    if (dependencyProperty != null)
                    {
                        this.DependencyProperty = dependencyProperty;
                        this._dependencyPropertyListener = listener = INTERNAL_PropertyStore.ListenToChanged(newSource, dependencyProperty, this.OnPropertyChanged);
                    }
                } //else (if there is no path), we don't need a listener because changing the source will directly call this method.
            }

            //todo: support attached DependencyProperties
            if (DependencyProperty == null)// || !this.DependencyProperty.IsAttached)
            {
                if (!BindsDirectlyToSource)
                {
                    Type sourceType = Source.GetType();
                    this.PropertyInfo = sourceType.GetProperty(_propertyName);
                    if (this.PropertyInfo == null)
                    {
                        // Try in case it is a simple field instead of a property:
                        this.FieldInfo = sourceType.GetField(_propertyName);
                    }
                }
            }
        }

        internal void OnPropertyChanged(object sender, IDependencyPropertyChangedEventArgs args)
        {
            try
            {
                this.UpdateValue();
                if (Next != null)
                {
                    Next.SetSource(this.Value);
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

        internal override void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName && (PropertyInfo != null || FieldInfo != null))
            {
                this.UpdateValue();
                var next = this.Next;
                if (next != null)
                    next.SetSource(this.Value);
            }
        }
    }
}
