
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

namespace System.Windows.Controls;

/// <summary>
/// Represents the method that will handle the <see cref="TextBox.TextChanged"/> routed event.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

/// <summary>
/// Provides data for the <see cref="TextBox.TextChanged"/> event.
/// </summary>
public class TextChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextChangedEventArgs"/> class.
    /// </summary>
    public TextChangedEventArgs() { }

    /// <summary>
    /// Performs the proper type casting to call the type-safe <see cref="TextChangedEventHandler"/> 
    /// delegate for the <see cref="TextBox.TextChanged"/> event.
    /// </summary>
    /// <param name="genericHandler">
    /// The handler to invoke.
    /// </param>
    /// <param name="genericTarget">
    /// The current object along the event's route.
    /// </param>
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        => ((TextChangedEventHandler)genericHandler)(genericTarget, this);
}
