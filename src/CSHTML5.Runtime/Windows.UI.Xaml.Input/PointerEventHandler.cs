
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
#if MIGRATION

    /// <summary>
    /// Represents the method that handles the System.Windows.UIElement.MouseLeftButtonDown
    /// and System.Windows.UIElement.MouseLeftButtonUp events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Represents the method that will handle mouse related routed events that do not
    /// specifically involve mouse buttons; for example, System.Windows.UIElement.MouseMove.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

#else

    /// <summary>
    /// Represents the method that will handle pointer message events such as PointerPressed.
    /// </summary>
    /// <param name="sender">The object that fired the event</param>
    /// <param name="e">The infos on the event</param>
    public delegate void PointerEventHandler(object sender, PointerRoutedEventArgs e);

#endif
}