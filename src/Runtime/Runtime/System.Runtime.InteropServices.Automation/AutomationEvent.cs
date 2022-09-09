
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

namespace System.Runtime.InteropServices.Automation
{
    /// <summary>
    /// Represents an Automation event.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class AutomationEvent
    {
        internal AutomationEvent() { }

        /// <summary>
        /// Occurs when the Automation event occurs.
        /// </summary>
        public event EventHandler<AutomationEventArgs> EventRaised;
        
        /// <summary>
        /// Attaches the specified delegate to the Automation event.
        /// </summary>
        /// <param name="handler">
        /// The delegate to attach to the event.
        /// </param>
        public void AddEventHandler(Delegate handler) { }

        /// <summary>
        /// Detaches the specified delegate from the Automation event.
        /// </summary>
        /// <param name="handler">
        /// The delegate to detach from the event.
        /// </param>
        public void RemoveEventHandler(Delegate handler) { }
    }
}

#endif
