
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
                       "ContextMenu",              // Name
                        typeof(ContextMenu),        // Type
                        typeof(ContextMenuService), // Owner
                        new PropertyMetadata(null));

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
