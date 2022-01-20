

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
        private bool m_handled;

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

        public bool Handled
        {
            get { return m_handled; }
            set { m_handled = value; }
        }

        ///// <summary>
        ///// Invokes the specified handler in a type-specific way on the specified object.
        ///// </summary>
        ///// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
        ///// <param name="genericTarget">The object to invoke the handler on.</param>
        //protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget);
    }
}