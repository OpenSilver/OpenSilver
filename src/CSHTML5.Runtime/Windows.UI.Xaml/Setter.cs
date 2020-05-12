

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
        private bool _throwOnNextValueChange; // used to validate setter value

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
            if (value == DependencyProperty.UnsetValue)
            {
                throw new ArgumentException("Cannot unset a Setter value.");
            }
            CheckValidProperty(property);

            Property = property;
            Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the property to apply the Value to. The default is null.
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
        /// Gets or sets the value to apply to the property that is specified by the
        /// Setter.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value", typeof(object), typeof(Setter), new PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Note: This callback only purpose is to validate the value change.
            // The value change is not acceptable if one the the two conditions
            // below is met :
            // 1 - the new value is DependencyProperty.UnsetValue.
            // 2 - the setter is sealed.
            // We reset the value to the old value and throw an exception if one
            // of these conditions is verified.
            Setter setter = (Setter)d;
            if (setter._throwOnNextValueChange)
            {
                setter._throwOnNextValueChange = false;
                setter.CheckSealed();
                throw new InvalidOperationException("Cannot unset a Setter value.");
            }
            if (setter.IsSealed || e.NewValue == DependencyProperty.UnsetValue)
            {
                setter._throwOnNextValueChange = true;
                setter.Value = e.OldValue;
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

            //todo: add the following when refactoring of DependencyProperty/PropertyMetadata is done.
            //if (!dp.IsValidValue(value))
            //{
            //    // The only markup extensions supported by styles is resources and bindings.
            //    if (value is MarkupExtension)
            //    {
            //        if (!(value is DynamicResourceExtension) && !(value is System.Windows.Data.BindingBase))
            //        {
            //            throw new ArgumentException(SR.Get(SRID.SetterValueOfMarkupExtensionNotSupported,
            //                                               value.GetType().Name));
            //        }
            //    }
            //}

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
                throw new InvalidOperationException(string.Format("'{0}' property cannot be set in the current element's Style.", FrameworkElement.NameProperty.Name));
            }
        }

        #endregion
    }
}