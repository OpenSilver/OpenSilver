

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal Style INTERNAL_ParentStyle;

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
        /// <param name="targetProperty">The dependency property identifier for the property that is being styled.</param>
        /// <param name="value">The value to assign to the value when the Setter applies.</param>
        public Setter(DependencyProperty targetProperty, object value)
        {
            Property = targetProperty;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the property to apply the Value to. The default is null.
        /// </summary>
        public DependencyProperty Property { get; set; }




        // Returns:
        //     The value to apply to the property that is specified by the Setter.
        /// <summary>
        /// Gets or sets the value to apply to the property that is specified by the
        /// Setter.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(Setter), new PropertyMetadata(null, Value_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Setter setter = (Setter)d;
            if (setter.INTERNAL_ParentStyle != null)
            {
                setter.INTERNAL_ParentStyle.NotifySetterValueChanged(setter);
            }
            else
            {
                //it could be nice to  know if the Setter is in a Style yet or not so we can throw an exception when needed... but we can't know.
                //throw new Exception("The Setter's Value could not be set through its style.");
            }
        }


    }
}