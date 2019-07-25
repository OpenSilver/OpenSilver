
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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Provides an abstract base class for the Binding class.
    /// </summary>
    public class BindingBase : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the BindingBase class.
        /// </summary>
        public BindingBase() { }

        /// <exclude/>
        ///
#if BRIDGE
        public override object ProvideValue(ServiceProvider serviceProvider)
#else
        public override object ProvideValue(IServiceProvider serviceProvider)
#endif
        {
            //do nothing ?
            return this;
        }

        object _fallbackValue;
        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        public object FallbackValue
        {
            get { return _fallbackValue; }
            set { _fallbackValue = value; }
        }

        object _targetNullValue;
        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source
        /// is null.
        /// </summary>
        public object TargetNullValue
        {
            get { return _targetNullValue; }
            set { _targetNullValue = value; }
        }

        protected string _stringFormat;
        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays 
        /// the bound value as a string.
        /// </summary>
        public string StringFormat
        {
            get { return _stringFormat; }
            set { _stringFormat = value; }
        }

    }
}
