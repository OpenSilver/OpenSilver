
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
