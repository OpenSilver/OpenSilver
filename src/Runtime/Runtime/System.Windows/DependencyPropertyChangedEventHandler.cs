﻿
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents the method that will handle events raised when a <see cref="DependencyProperty"/>
    /// is changed on a particular <see cref="DependencyObject"/> implementation.
    /// </summary>
    /// <param name="sender">
    /// The source of the event (typically the object where the property changed).
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    public delegate void DependencyPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);
}