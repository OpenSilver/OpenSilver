

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
    public partial class BindingBase : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the BindingBase class.
        /// </summary>
        public BindingBase() { }

        /// <exclude/>
        ///
#if BRIDGE && !CSHTML5BLAZOR
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
