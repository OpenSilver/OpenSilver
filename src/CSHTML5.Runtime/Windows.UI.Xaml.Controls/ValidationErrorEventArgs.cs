
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
#else
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides information for the System.Windows.Controls.Validation.Error attached
    /// event.
    /// </summary>
    public partial class ValidationErrorEventArgs : RoutedEventArgs
    {
        private ValidationErrorEventAction _action;

        /// <summary>
        /// Gets a value that indicates whether the error is a new error or an existing
        /// error that has now been cleared.
        /// </summary>
        public ValidationErrorEventAction Action
        {
            get { return _action; }
            internal set { _action = value; }
        }


        private ValidationError _error;

        /// <summary>
        /// Gets the error that caused this System.Windows.Controls.Validation.Error
        /// event.
        /// </summary>
        public ValidationError Error
        {
            get { return _error; }
            internal set { _error = value; }
        }

        ///// <summary>
        ///// Invokes the specified handler in a type-specific way on the specified object.
        ///// </summary>
        ///// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
        ///// <param name="genericTarget">The object to invoke the handler on.</param>
        //protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget);
    }
}