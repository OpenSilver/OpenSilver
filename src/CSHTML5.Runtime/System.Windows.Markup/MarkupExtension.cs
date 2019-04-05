
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

namespace System.Windows.Markup
{
    /// <summary>
    /// Provides a base class for XAML markup extension implementations that can
    /// be supported by .NET Framework XAML Services and other XAML readers and XAML
    /// writers.
    /// </summary> 
    public abstract class MarkupExtension
    {
        //// <summary>
        //// Initializes a new instance of a class derived from System.Windows.Markup.MarkupExtension.
        //// </summary>
        //protected MarkupExtension();

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as
        /// the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        /// 
#if BRIDGE
        public abstract object ProvideValue(ServiceProvider serviceProvider);
#else
        public abstract object ProvideValue(IServiceProvider serviceProvider);
#endif
    }
}
