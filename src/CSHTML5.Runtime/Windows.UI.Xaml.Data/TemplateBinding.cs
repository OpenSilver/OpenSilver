
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
