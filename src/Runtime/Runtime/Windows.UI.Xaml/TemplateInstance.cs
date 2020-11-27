

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
    public partial class TemplateInstance
    {
        /// <summary>
        /// The element that contains the FrameworkTemplate.
        /// </summary>
        public FrameworkElement TemplateOwner;

        /// <summary>
        /// The visual subtree that has been generated for the FrameworkTemplate.
        /// Note: this should only be used inside the methods put as parameter in FrameworkTemplate.SetMethodToInstantiateFrameworkTemplate, to define the root of the Template's generated subtree.
        /// </summary>
        public FrameworkElement TemplateContent;
    }
}
