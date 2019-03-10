
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
