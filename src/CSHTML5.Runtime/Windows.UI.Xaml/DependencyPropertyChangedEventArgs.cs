
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

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
    }
}
