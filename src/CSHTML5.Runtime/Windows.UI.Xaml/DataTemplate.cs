

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
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Describes the visual structure of a data object.
    /// </summary>
    public partial class DataTemplate : FrameworkTemplate
    {
        /// <summary>
        /// Initializes a new instance of the DataTemplate class.
        /// </summary>
        public DataTemplate() : base() { }

        /// <summary>
        /// Creates the System.Windows.UIElement objects in the System.Windows.DataTemplate.
        /// </summary>
        /// <returns>The root System.Windows.UIElement of the System.Windows.DataTemplate.</returns>
        public DependencyObject LoadContent()
        {
            return this.INTERNAL_InstantiateFrameworkTemplate();
        }
    }
}
