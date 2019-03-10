
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
    public class XamlParseException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class.
        /// </summary>
        public XamlParseException()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class, using the specified exception message string.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public XamlParseException(string message)
            : base(message)
        {
        }
       
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class, using the specified exception message string and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The initial exception that occurred.</param>
        public XamlParseException(string message, Exception innerException)
            : base (message, innerException)
        {
        }
    }
}
