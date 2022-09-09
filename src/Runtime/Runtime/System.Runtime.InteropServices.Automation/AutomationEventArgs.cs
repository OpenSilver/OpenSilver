
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
    /// Provides data for the <see cref="AutomationEvent.EventRaised"/> event.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class AutomationEventArgs : EventArgs
    {
        internal AutomationEventArgs(object[] args)
        {
            Arguments = args;
        }

        /// <summary>
        /// Gets the event arguments from the Automation event.
        /// </summary>
        /// <returns>
        /// The event arguments from the Automation event.
        /// </returns>
        public object[] Arguments { get; }
    }
}

#endif
