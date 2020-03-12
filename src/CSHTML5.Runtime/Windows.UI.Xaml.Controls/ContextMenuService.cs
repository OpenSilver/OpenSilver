

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
using System.Windows.Controls.Primitives;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public static class ContextMenuService
    {
        /// <summary>
        ///     The DependencyProperty for the ContextMenu property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
                DependencyProperty.Register(
                       "ContextMenu",
                        typeof(ContextMenu),
                        typeof(ContextMenuService),
                        new PropertyMetadata(null)
                        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        ///     Gets the value of the ContextMenu property on the specified object.
        /// </summary>
        /// <param name="element">The object on which to query the ContextMenu property.</param>
        /// <returns>The value of the ContextMenu property.</returns>
        public static ContextMenu GetContextMenu(DependencyObject element)
        {
            return ((FrameworkElement)element).ContextMenu;
        }

        /// <summary>
        ///     Sets the ContextMenu property on the specified object.
        /// </summary>
        /// <param name="element">The object on which to set the ContextMenu property.</param>
        /// <param name="value">
        ///     The value of the ContextMenu property. If the value is of type ContextMenu, then
        ///     that is the ContextMenu that will be used (without any modification). If the value
        ///     is of any other type, then that value will be used as the content for a ContextMenu
        ///     provided by this service, and the other attached properties of this service
        ///     will be used to configure the ContextMenu.
        /// </param>
        public static void SetContextMenu(DependencyObject element, ContextMenu value)
        {
            ((FrameworkElement)element).ContextMenu = value;
        }
    }
}
