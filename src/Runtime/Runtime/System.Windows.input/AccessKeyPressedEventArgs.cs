
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

namespace System.Windows.Input;

/// <summary>
/// Represents the method that will handle the <b>AccessKeyManager.AccessKeyPressed</b> attached event.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void AccessKeyPressedEventHandler(object sender, AccessKeyPressedEventArgs e);

/// <summary>
/// Provides data for the <see cref="AccessKeyManager"/> routed event.
/// </summary>
public class AccessKeyPressedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessKeyEventArgs"/> class.
    /// </summary>
    public AccessKeyPressedEventArgs()
    {
        RoutedEvent = AccessKeyManager.AccessKeyPressedEvent;
        Key = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessKeyPressedEventArgs"/> class with the specified access key.
    /// </summary>
    /// <param name="key">
    /// The access key.
    /// </param>
    public AccessKeyPressedEventArgs(string key)
        : this()
    {
        Key = key;
    }

    /// <summary>
    /// Gets the scope for the element that raised this event.
    /// </summary>
    /// <returns>
    /// The element's scope.
    /// </returns>
    public object Scope { get; set; }

    /// <summary>
    /// Gets or sets the target for the event.
    /// </summary>
    /// <returns>
    /// The element that raised this event.
    /// </returns>
    public UIElement Target { get; set; }

    /// <summary>
    /// Gets a string representation of the access key that was pressed.
    /// </summary>
    /// <returns>
    /// The access key.
    /// </returns>
    public string Key { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((AccessKeyPressedEventHandler)genericHandler)(genericTarget, this);
}
