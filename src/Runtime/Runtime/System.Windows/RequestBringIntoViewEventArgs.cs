
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

namespace System.Windows;

/// <summary>
/// Represents the method that will handle the <see cref="FrameworkElement.RequestBringIntoView"/> routed event.
/// </summary>
/// <param name="sender">
/// The object where the event handler is attached.
/// </param>
/// <param name="e">
/// The event data.
/// </param>
public delegate void RequestBringIntoViewEventHandler(object sender, RequestBringIntoViewEventArgs e);

/// <summary>
/// Provides data for the <see cref="FrameworkElement.RequestBringIntoView"/> routed event.
/// </summary>
public class RequestBringIntoViewEventArgs : RoutedEventArgs
{
    internal RequestBringIntoViewEventArgs(DependencyObject target, Rect targetRect)
    {
        TargetObject = target;
        TargetRect = targetRect;
    }

    /// <summary>
    /// Gets the object that should be made visible in response to the event.
    /// </summary>
    /// <returns>
    /// The object that called <see cref="FrameworkElement.BringIntoView()"/>.
    /// </returns>
    public DependencyObject TargetObject { get; }

    /// <summary>
    /// Gets the rectangular region in the object's coordinate space which should be made visible.
    /// </summary>
    /// <returns>
    /// The requested rectangular space.
    /// </returns>
    public Rect TargetRect { get; }

    /// <inheritdoc />
    protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget) =>
        ((RequestBringIntoViewEventHandler)genericHandler)(genericTarget, this);
}
