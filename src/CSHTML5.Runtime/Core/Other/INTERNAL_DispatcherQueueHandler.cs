

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


using System;

namespace CSHTML5.Internal
{
    /// <summary>
    /// A class that allows to queue an Action so that it is executed in a Dispatcher.Invoke call,
    /// but only if another action is not already pending execution. This is useful for example to
    /// redraw the screen when a collection of item changes, doing so only once if the collection
    /// changes multiple times. Another example is redrawing an element when any of its properties
    /// change, while ensuring that if many properties are changed, the element is redrawn only once.
    /// </summary>
    public class INTERNAL_DispatcherQueueHandler
    {
        bool _queueIsEmpty = true;

        /// <summary>
        /// Call this method to perform the specified action in a Dispatcher.Invoke call, but only
        /// if another action is not already pending execution. This is useful for example to redraw
        /// the screen when a collection of item changes, doing so only once if the collection changes
        /// multiple times. Another example is redrawing an element when any of its properties change,
        /// while ensuring that if many properties are changed, the element is redrawn only once.
        /// </summary>
        /// <param name="action">The action to queue</param>
        public void QueueActionIfQueueIsEmpty(Action action)
        {
            if (_queueIsEmpty) // This ensures that the "QueueAction" method below is only called once, and it is not called again until its delegate has been executed.
            {
                _queueIsEmpty = false;
                INTERNAL_DispatcherHelpers.QueueAction(() =>
                {
                    _queueIsEmpty = true;

                    action();
                });
            }
        }
    }
}
