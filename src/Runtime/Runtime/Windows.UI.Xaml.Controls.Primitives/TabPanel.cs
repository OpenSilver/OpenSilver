

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
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Handles the layout of <see cref="T:System.Windows.Controls.TabItem" />
    /// objects on a <see cref="T:System.Windows.Controls.TabControl" />.
    /// </summary>
    public partial class TabPanel : WrapPanel
    {
        public TabPanel()
        {
            this.Orientation = Orientation.Horizontal;
        }
    }
}
