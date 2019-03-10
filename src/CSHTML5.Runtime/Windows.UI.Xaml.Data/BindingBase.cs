
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

#if WORKINPROGRESS
        string _stringFormat;
        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays 
        /// the bound value as a string.
        /// </summary>
        public string StringFormat
        {
            get { return _stringFormat; }
            set { _stringFormat = value; }
        }
#endif

    }
}
