
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
                    object jsTarget = Interop.ExecuteJavaScript(@"$0.target || $0.srcElement", _originalJSEventArg);
                    UIElement correspondingUiElementIfFound = INTERNAL_HtmlDomManager.GetUIElementFromDomElement(jsTarget); //Note: already handles the possibility that "jsTarget" is null or undefined.
                    return correspondingUiElementIfFound;
                }
                else
                    return null;
            }
            internal set
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
