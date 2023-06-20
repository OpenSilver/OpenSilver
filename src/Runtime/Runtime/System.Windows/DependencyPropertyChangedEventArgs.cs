
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
    public sealed class DependencyPropertyChangedEventArgs : IDependencyPropertyChangedEventArgs
    {
        internal DependencyPropertyChangedEventArgs(
            object oldValue,
            object newValue,
            DependencyProperty property,
            PropertyMetadata metadata)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Property = property;
            Metadata = metadata;
        }

        public DependencyPropertyChangedEventArgs() { }

        /// <summary>
        /// Gets the value of the property before the change.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// Gets the value of the property after the change.
        /// </summary>
        public object NewValue { get; }
        
        /// <summary>
        /// Gets the identifier for the dependency property where the value change occurred.
        /// </summary>
        public DependencyProperty Property { get; }

        /// <summary>
        /// Metadata for the property
        /// </summary>
        internal PropertyMetadata Metadata { get; }
    }
}
