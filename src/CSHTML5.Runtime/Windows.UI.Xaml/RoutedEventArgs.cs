
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



using CSHTML5;
using CSHTML5.Internal;
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
    /// Contains state information and event data associated with a routed event.
    /// </summary>
    public partial class RoutedEventArgs
#if MIGRATION
        : EventArgs
#endif
    {
        /// <summary>
        /// Initializes a new instance of the RoutedEventArgs class.
        /// </summary>
        public RoutedEventArgs() { }


        private DependencyObject _originalSource;
        /// <summary>
        /// Gets a reference to the object that raised the event.
        /// </summary>
        public object OriginalSource
        {
            get
            {
                if (_originalSource != null)
                {
                    return _originalSource;
                }
                else if (_originalJSEventArg != null)
                {
                    object jsTarget = CSHTML5.Interop.ExecuteJavaScript(@"$0.target || $0.srcElement", _originalJSEventArg);
                    UIElement correspondingUiElementIfFound = INTERNAL_HtmlDomManager.GetUIElementFromDomElement(jsTarget); //Note: already handles the possibility that "jsTarget" is null or undefined.
                    return correspondingUiElementIfFound;
                }
                else
                    return null;
            }
            set
            {
                if (value == null || !(value is DependencyObject))
                {
                    throw new ArgumentException();
                }
                _originalSource = (DependencyObject)value;
            }
        }

        object _originalJSEventArg;
        /// <summary>
        /// (Optional) Gets the original javascript event arg.
        /// </summary>
        public object INTERNAL_OriginalJSEventArg
        {
            get
            {
                return _originalJSEventArg;
            }
            set
            {
                _originalJSEventArg = value;
            }
        }

    }
}
