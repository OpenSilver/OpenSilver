
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
            DependencyProperty.Register("Value", typeof(object), typeof(Setter), new PropertyMetadata(null, Value_Changed));

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