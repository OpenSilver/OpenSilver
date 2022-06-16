
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
using OpenSilver.Internal.Data;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides data for a PropertyChangedCallback implementation.
    /// </summary>
    public sealed partial class DependencyPropertyChangedEventArgs : IDependencyPropertyChangedEventArgs
    {
        /// <summary>
        /// Gets the value of the property before the change.
        /// </summary>
        public object OldValue { get; internal set; }
        /// <summary>
        /// Gets the value of the property after the change.
        /// </summary>
        public object NewValue { get; internal set; }
        /// <summary>
        /// Gets the identifier for the dependency property where the value change occurred.
        /// </summary>
        public DependencyProperty Property { get; internal set; }

        internal DependencyPropertyChangedEventArgs(object oldValue, object newValue, DependencyProperty property)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Property = property;
        }

        public DependencyPropertyChangedEventArgs()
        {

        }
    }
}
