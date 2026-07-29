
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
/// Provides special handling information to inform event listeners whether specific handlers 
/// should be invoked.
/// </summary>
public readonly struct RoutedEventHandlerInfo
{
    /// <summary>
    ///     Construtor for RoutedEventHandlerInfo
    /// </summary>
    /// <param name="handler">
    ///     Non-null handler
    /// </param>
    /// <param name="handledEventsToo">
    ///     Flag that indicates if or not the handler must 
    ///     be invoked for already handled events
    /// </param>
    internal RoutedEventHandlerInfo(Delegate handler, bool handledEventsToo)
    {
        Handler = handler;
        InvokeHandledEventsToo = handledEventsToo;
    }

    /// <summary>
    /// Gets the event handler.
    /// </summary>
    /// <returns>
    /// The event handler.
    /// </returns>
    public Delegate Handler { get; }

    /// <summary>
    /// Gets a value that indicates whether the event handler is invoked when the routed event
    /// is marked handled.
    /// </summary>
    /// <returns>
    /// true if the event handler is invoked when the routed event is marked handled; otherwise, 
    /// false.
    /// </returns>
    public bool InvokeHandledEventsToo { get; }

    // Invokes handler instance as per specified 
    // invocation preferences
    internal void InvokeHandler(object target, RoutedEventArgs routedEventArgs)
    {
        if (!routedEventArgs.Handled || InvokeHandledEventsToo)
        {
            if (Handler is RoutedEventHandler handler)
            {
                // Generic RoutedEventHandler is called directly here since
                //  we don't need the InvokeEventHandler override to cast to
                //  the proper type - we know what it is.
                handler(target, routedEventArgs);
            }
            else
            {
                // NOTE: Cannot call protected method InvokeEventHandler directly
                routedEventArgs.InvokeHandler(Handler, target);
            }
        }
    }

    /// <summary>
    /// Determines whether the specified object is equivalent to the current <see cref="RoutedEventHandlerInfo"/>.
    /// </summary>
    /// <param name="obj">
    /// The object to compare to the current <see cref="RoutedEventHandlerInfo"/>.
    /// </param>
    /// <returns>
    /// true if the specified object is equivalent to the current <see cref="RoutedEventHandlerInfo"/>; 
    /// otherwise, false.
    /// </returns>
    public override bool Equals(object obj) => obj is RoutedEventHandlerInfo info && Equals(info);

    /// <summary>
    /// Determines whether the specified <see cref="RoutedEventHandlerInfo"/> is equivalent to the current
    /// <see cref="RoutedEventHandlerInfo"/>.
    /// </summary>
    /// <param name="handlerInfo">
    /// The <see cref="RoutedEventHandlerInfo"/> to compare to the current <see cref="RoutedEventHandlerInfo"/>.
    /// </param>
    /// <returns>
    /// true if the specified <see cref="RoutedEventHandlerInfo"/> is equivalent to the current 
    /// <see cref="RoutedEventHandlerInfo"/>; otherwise, false.
    /// </returns>
    public bool Equals(RoutedEventHandlerInfo handlerInfo)
        => Handler == handlerInfo.Handler && InvokeHandledEventsToo == handlerInfo.InvokeHandledEventsToo;

    /// <summary>
    /// Returns a hash code for the current <see cref="RoutedEventHandlerInfo"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="RoutedEventHandlerInfo"/>.
    /// </returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Determines whether the specified objects are equivalent.
    /// </summary>
    /// <param name="handlerInfo1">
    /// The first object to compare.
    /// </param>
    /// <param name="handlerInfo2">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// true if the objects are equivalent; otherwise, false.
    /// </returns>
    public static bool operator ==(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        => handlerInfo1.Equals(handlerInfo2);

    /// <summary>
    /// Determines whether the specified objects are not equivalent.
    /// </summary>
    /// <param name="handlerInfo1">
    /// The first object to compare.
    /// </param>
    /// <param name="handlerInfo2">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// true if the objects are not equivalent; otherwise, false.
    /// </returns>
    public static bool operator !=(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        => !handlerInfo1.Equals(handlerInfo2);
}