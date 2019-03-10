
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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a selectable item inside a Menu.
    /// </summary>
    public class MenuItem : Button //HeaderedItemsControl, ICommandSource //todo: support the other features of the MenuItem class, such as the ability to nest menu items inside one another.
    {
        /// <summary>
        /// Initializes a new instance of the MenuItem class.
        /// </summary>
        public MenuItem()
        {
            // Set default style:
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultMenuItemStyle.GetDefaultStyle());
        }


        /// <summary>
        /// Gets or sets the icon that appears in a MenuItem.
        /// </summary>
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.MenuItem.Icon dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(MenuItem), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the item that labels the control.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.HeaderedItemsControl.Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(MenuItem), new PropertyMetadata(null, Header_Changed));

        private static void Header_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // In the current implementation, we simply copy the "Header" property into the "Content" property:
            var menuItem = (MenuItem)d;
            var newHeader = e.NewValue;
            menuItem.Content = newHeader;
        }


        /*protected override void OnPointerPressed(Input.PointerRoutedEventArgs eventArgs)
        {
            // Transform the "PointerPressed" event into a Click event so that we can then cancel the "Bubbling Up" of the "Pressed event", so that we can distinguish between clicking inside the menu and outside the menu (cf. ContextMenu.IsOpen_Changed):
            this.OnClick(new RoutedEventArgs());
            eventArgs.Handled = true;
        }*/

        //protected override void OnPointerReleased(Input.PointerRoutedEventArgs eventArgs)
        //{
        //    base.OnPointerReleased(eventArgs);

        //    eventArgs.Handled = true;
        //}

    }
}
