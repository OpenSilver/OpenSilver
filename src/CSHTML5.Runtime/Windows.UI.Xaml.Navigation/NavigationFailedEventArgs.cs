
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