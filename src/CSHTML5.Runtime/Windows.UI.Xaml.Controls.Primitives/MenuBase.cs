

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

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that defines choices for users to select.
    /// </summary>
    public abstract partial class MenuBase : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.Primitives.MenuBase class.
        /// </summary>
        protected MenuBase() { }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

    }
}
