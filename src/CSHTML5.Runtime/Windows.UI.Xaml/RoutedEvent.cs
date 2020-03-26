

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class RoutedEvent
    {
        string _eventName;

        public RoutedEvent(string eventName)
        {
            _eventName = eventName;
        }

        /// <summary>
        /// Returns the string representation of the routed event.
        /// </summary>
        /// <returns>The name of the routed event.</returns>
        public override string ToString()
        {
            return _eventName;
        }
    }
}
