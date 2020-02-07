
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

        public DependencyPropertyChangedEventArgs()
        {

        }
    }
}
