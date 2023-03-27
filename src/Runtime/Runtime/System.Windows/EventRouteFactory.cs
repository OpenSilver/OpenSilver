
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
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

/// <summary>
///     Creates and recycles instance of EventRoute
/// </summary>
internal static class EventRouteFactory
{
    /// <summary>
    ///     Fetch a recycled object if available 
    ///     else create a new instance
    /// </summary>
    internal static EventRoute FetchObject(RoutedEvent routedEvent)
    {
        EventRoute eventRoute = Pop();

        if (eventRoute == null)
        {
            eventRoute = new EventRoute(routedEvent);
        }
        else
        {
            eventRoute.RoutedEvent = routedEvent;
        }

        return eventRoute;
    }

    /// <summary>
    ///     Recycle the given instance of EventRoute
    /// </summary>
    internal static void RecycleObject(EventRoute eventRoute)
    {
        // Cleanup all refernces held
        eventRoute.Clear();

        // Push instance on to the stack
        Push(eventRoute);
    }

    /// <summary>
    ///     Push the given instance of EventRoute on to the stack
    /// </summary>
    private static void Push(EventRoute eventRoute)
    {
        lock (_synchronized)
        {
            // In a normal scenario it is extremely rare to 
            // require more than 2 EventRoutes at the same time
            if (_eventRouteStack == null)
            {
                _eventRouteStack = new EventRoute[2];
                _stackTop = 0;
            }

            if (_stackTop < 2)
            {
                _eventRouteStack[_stackTop++] = eventRoute;
            }
        }
    }

    /// <summary>
    ///     Pop off the last instance of EventRoute in the stack
    /// </summary>
    private static EventRoute Pop()
    {
        lock (_synchronized)
        {
            if (_stackTop > 0)
            {
                EventRoute eventRoute = _eventRouteStack[--_stackTop];
                _eventRouteStack[_stackTop] = null;
                return eventRoute;
            }
        }

        return null;
    }

    private static EventRoute[] _eventRouteStack;
    private static int _stackTop;
    private static readonly object _synchronized = new object();
}