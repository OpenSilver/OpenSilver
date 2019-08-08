
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

#if !MIGRATION
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Defines a generated instance of a FrameworkTemplate.
    /// </summary>
    public class TemplateInstance
    {
        /// <summary>
        /// The element that contains the FrameworkTemplate.
        /// </summary>
        public Control TemplateOwner;

        /// <summary>
        /// The visual subtree that has been generated for the FrameworkTemplate.
        /// Note: this should only be used inside the methods put as parameter in FrameworkTemplate.SetMethodToInstantiateFrameworkTemplate, to define the root of the Template's generated subtree.
        /// </summary>
        public FrameworkElement TemplateContent;
    }
}
