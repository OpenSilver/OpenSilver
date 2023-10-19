

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
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows
{
    /// <summary>
    /// Applies a value to a property in a Style.
    /// </summary>
    public sealed class Setter : SetterBase, ISupportInitialize
    {
#region Data

        private DependencyProperty _property;
        private object _value;

        private object _unresolvedValue = null;
        private ITypeDescriptorContext _serviceProvider = null;
        private CultureInfo _cultureInfoForTypeConverter = null;

#endregion

#region Constructors

        /// <summary>
        /// Initializes a new instance of the Setter class with no initial Property or
        /// Value.
        /// </summary>
        public Setter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Setter class with initial Property and
        /// Value information.
        /// </summary>
        /// <param name="property">The dependency property identifier for the property that is being styled.</param>
        /// <param name="value">The value to assign to the value when the Setter applies.</param>
        public Setter(DependencyProperty property, object value)
        {
            CheckValidProperty(property);

            _property = property;
            _value = value == DependencyProperty.UnsetValue ? null : value;
        }

#endregion

#region Public Properties

        /// <summary>
        /// Gets or sets the property to apply the <see cref="Setter.Value"/> to.
        /// The default is null.
        /// </summary>
        [Ambient]
        [TypeConverter(typeof(DependencyPropertyConverter))]
        public DependencyProperty Property
        {
            get
            {
                return _property;
            }
            set
            {
                CheckValidProperty(value);
                CheckSealed();
                _property = value;
            }
        }

        /// <summary>
        /// Gets or sets the value to apply to the property that is specified 
        /// by the <see cref="Setter"/>.
        /// </summary>
        [TypeConverter(typeof(SetterValueConverter))]
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                CheckSealed();

                if (value == DependencyProperty.UnsetValue)
                {
                    // Silverlight uses a DependencyProperty for Setter.Value,
                    // so in case of DependencyProperty.UnsetValue, we emulate
                    // a call to DependencyObject.ClearValue(...).
                    _value = null;
                    return;
                }

                if (value is BindingExpression)
                {
                    throw new ArgumentException("BindingExpression type is not a valid Style value.");
                }

                _value = value;
            }
        }

#endregion

#region Internal Methods

        /// <summary>
        ///     Seals this setter
        /// </summary>
        internal override void Seal()
        {
            // Do the validation that can't be done until we know all of the property
            // values.

            DependencyProperty dp = Property;
            object value = Value;

            if (dp == null)
            {
                throw new ArgumentException(string.Format("Must have non-null value for '{0}'.", "Setter.Property"));
            }

            if (dp.IsObjectType || !dp.IsValidValue(value))
            {
                switch (value)
                {
                    case Color color:
                        if (dp.PropertyType == typeof(Brush))
                            _value = new SolidColorBrush(color);
                        break;

                    case Binding binding:
                        binding._isInStyle = true;
                        break;

                    default:
                        if (!dp.IsObjectType)
                            throw new ArgumentException(
                                $"'{value}' is not a valid value for the '{dp.OwnerType}.{dp.Name}' property on a Setter.");
                        break;
                }
            }

            base.Seal();
        }

        private void CheckValidProperty(DependencyProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (property == FrameworkElement.NameProperty)
            {
                // Note: Silverlight allows this, but will crash as soon as
                // the style is used 2 times in the visual tree.
                throw new InvalidOperationException(
                    string.Format("'{0}' property cannot be set in the current element's Style.", 
                                  FrameworkElement.NameProperty.Name));
            }
        }

#endregion        

        internal void ReceiveTypeConverter(ITypeDescriptorContext serviceProvider, CultureInfo culture, object unresolvedValue)
        {
            _serviceProvider = serviceProvider;
            _cultureInfoForTypeConverter = culture;
            _unresolvedValue = unresolvedValue;
        }

        void ISupportInitialize.BeginInit() { }

        void ISupportInitialize.EndInit()
        {
            if (_unresolvedValue != null)
            {
                try
                {
                    Value = SetterValueConverter.ResolveValue(_serviceProvider,
                        Property, _cultureInfoForTypeConverter, _unresolvedValue);
                }
                finally
                {
                    _unresolvedValue = null;
                }
            }

            _serviceProvider = null;
            _cultureInfoForTypeConverter = null;
        }
    }
}