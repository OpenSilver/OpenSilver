

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

#if MIGRATION
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Applies a value to a property in a Style.
    /// </summary>
    public sealed partial class Setter : SetterBase
    {
#region Data

        private DependencyProperty _property;
        private object _value;

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

            bool isObjectType = dp.PropertyType == typeof(object);

            if (isObjectType || !DependencyProperty.IsValueTypeValid(value, dp.PropertyType))
            {
                Binding binding = value as Binding;
                if (binding == null)
                {
                    // Special case :
                    // In xaml, setting a color via StaticResource to the value of a
                    // style setter converts the color to a SolidColorBrush as shown in
                    // the following code.
                    //   <Color x:Key="color">#FFFF0000</Color>
                    //   <Style x:Key="style" TargetType="Control">
                    //      <Setter Property="Foreground" Value="{StaticResource color}" />
                    //   </Style>
                    // Note: this behavior is not reproduced when a style is constructed
                    // directly in C# (Silverlight), but it will work anyway with this
                    // workaround.
                    if (dp.PropertyType == typeof(Brush) && value is Color color)
                    {
                        // do not use Value to avoid unecessary checks.
                        _value = new SolidColorBrush(color);
                    }
                    else if (!isObjectType)
                    {
                        throw new ArgumentException(
                            string.Format("'{0}' is not a valid value for the '{1}.{2}' property on a Setter.",
                                          value, dp.OwnerType, dp.Name));
                    }
                }
                else
                {
                    binding._isInStyle = true;
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
    }
}