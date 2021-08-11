
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
using System.Collections;
using OpenSilver.Internal.Controls;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Handles the layout of <see cref="TabItem" /> objects on 
    /// a <see cref="TabControl" />.
    /// </summary>
    public partial class TabPanel : WrapPanel
    {
        public TabPanel()
        {
            this.Orientation = Orientation.Horizontal;
        }

        protected sealed override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return base.CreateUIElementCollection(null);
        }

        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                // Note: Since children are displayed in a grid in our implementation,
                // this panel's children are not logical children. There are the logical
                // children of the grid they are displayed in.
                return EmptyEnumerator.Instance;
            }
        }
    }
}
