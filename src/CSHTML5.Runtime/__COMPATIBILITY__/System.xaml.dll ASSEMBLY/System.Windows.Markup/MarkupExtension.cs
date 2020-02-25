
//-----------------------------------------------------------------------------
//  CONFIDENTIALITY NOTICE:
//  This code is the sole property of Userware and is strictly confidential.
//  Unless you have a written agreement in effect with Userware that states
//  otherwise, you are not authorized to view, copy, modify, or compile this
//  source code, and you should destroy all the copies that you possess.
//  Any redistribution in source code form is strictly prohibited.
//  Redistribution in binary form is allowed provided that you have obtained
//  prior written consent from Userware. You can contact Userware at:
//  contact@userware-solutions.com - Copyright (c) 2016 Userware
//-----------------------------------------------------------------------------


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
        /// <summary>
        /// Initializes a new instance of a class derived from System.Windows.Markup.MarkupExtension.
        /// </summary>
        //protected MarkupExtension();

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as
        /// the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public abstract object ProvideValue(IServiceProvider serviceProvider);
    }
}
