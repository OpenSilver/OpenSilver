
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
