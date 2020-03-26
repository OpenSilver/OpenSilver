

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
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Provides data for the NavigationFailed event of the NavigationService class and the NavigationFailed event of the Frame class.</summary>
    public sealed partial class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the NavigationFailedEventArgs.
        /// </summary>
        public NavigationFailedEventArgs(Exception exception, bool handled, Uri uri)
        {
            _exception = exception;
            _handled = handled;
            _uri = uri;
        }

        private Exception _exception;
        private bool _handled;
        private Uri _uri;

        /// <summary>
        /// Gets the error from the failed navigation.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the failure event has been handled.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }


        /// <summary>
        /// Gets the uniform resource identifier (URI) for the content that could not be navigated to.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return _uri;
            }
        }
    }
}