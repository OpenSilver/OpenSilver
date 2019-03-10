
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

namespace System.Windows.Markup
{
    /// <summary>
    /// Provides a base class for XAML markup extension implementations that can
    /// be supported by .NET Framework XAML Services and other XAML readers and XAML
    /// writers.
    /// </summary> 
    public abstract class MarkupExtension
    {
        //// <summary>
        //// Initializes a new instance of a class derived from System.Windows.Markup.MarkupExtension.
        //// </summary>
        //protected MarkupExtension();

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as
        /// the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        /// 
#if BRIDGE
        public abstract object ProvideValue(ServiceProvider serviceProvider);
#else
        public abstract object ProvideValue(IServiceProvider serviceProvider);
#endif
    }
}
