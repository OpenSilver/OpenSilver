
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
    public abstract class BindingBase : MarkupExtension
    {
        private bool _isSealed;
        private object _fallbackValue;
        private object _targetNullValue;
        protected string _stringFormat;

        /// <summary>
        /// Initializes a new instance of the BindingBase class.
        /// </summary>
        protected BindingBase() { }

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

        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        public object FallbackValue
        {
            get { return _fallbackValue; }
            set { CheckSealed(); _fallbackValue = value; }
        }

        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source
        /// is null.
        /// </summary>
        public object TargetNullValue
        {
            get { return _targetNullValue; }
            set { CheckSealed(); _targetNullValue = value; }
        }

        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays 
        /// the bound value as a string.
        /// </summary>
        public string StringFormat
        {
            get { return _stringFormat; }
            set { CheckSealed(); _stringFormat = value; }
        }

        protected internal void CheckSealed()
        {
            if (_isSealed)
            {
                throw new InvalidOperationException("Binding cannot be changed after it has been used.");
            }
        }

        internal void Seal()
        {
            _isSealed = true;
        }
    }
}
