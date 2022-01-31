

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
    public class RoutedEventArgs
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
