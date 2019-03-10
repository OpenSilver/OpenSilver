
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public class TemplateBinding : BindingBase
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
