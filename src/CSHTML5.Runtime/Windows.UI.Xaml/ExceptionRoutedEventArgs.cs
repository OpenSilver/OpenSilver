
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides event data for exceptions that are raised as events by asynchronous operations, such as MediaFailed or ImageFailed.
    /// </summary>
    public class ExceptionRoutedEventArgs : RoutedEventArgs
    {
        private string _errorMessage = "";

        /// <summary>
        /// Gets the message component of the exception, as a string.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        /// <summary>
        /// Initializes a new instance of Windows.UI.Xaml.ExceptionRoutedEventArgs with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ExceptionRoutedEventArgs(string errorMessage) :base()
        {
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of Windows.UI.Xaml.ExceptionRoutedEventArgs.
        /// </summary>
        public ExceptionRoutedEventArgs(): base() { }
    }
}
