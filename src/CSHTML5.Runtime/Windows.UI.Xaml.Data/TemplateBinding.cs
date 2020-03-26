

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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
using System.Globalization;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    //------------------------------------
    //
    // IMPORTANT:
    //
    // This class is here only for the XAML Code Editor not to complain.
    // In reality, the CSHTML5 compiler transforms "TemplateBinding" into a standard Binding with RelativeSource set to TemplatedParent.
    //
    //------------------------------------
    [ContentProperty("Property")]
    public partial class TemplateBinding : BindingBase
    {
        public TemplateBinding() { }

        public TemplateBinding(string property) { }

        DependencyProperty _property;
        public DependencyProperty Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        IValueConverter _converter;
        public IValueConverter Converter
        {
            get
            {
                return _converter;
            }
            set
            {
                _converter = value;
            }
        }

#if MIGRATION
        private CultureInfo _converterCulture;
        public CultureInfo ConverterCulture
        {
            get { return _converterCulture; }
            set { _converterCulture = value; }
        }
        
#else
        string _converterLanguage;
        public string ConverterLanguage
        {
            get
            {
                return _converterLanguage;
            }
            set
            {
                _converterLanguage = value;
            }
        }
#endif

        object _converterParameter = null;
        public object ConverterParameter
        {
            get
            {
                return _converterParameter;
            }
            set
            {
                _converterParameter = value;
            }
        }

    }
}
