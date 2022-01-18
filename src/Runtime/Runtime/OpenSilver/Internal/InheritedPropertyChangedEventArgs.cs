
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

namespace OpenSilver.Internal
{
    // Event args for the (internal) InheritedPropertyChanged event
    internal class InheritedPropertyChangedEventArgs : EventArgs
    {
        internal InheritedPropertyChangedEventArgs(ref InheritablePropertyChangeInfo info)
        {
            _info = info;
        }

        internal InheritablePropertyChangeInfo Info
        {
            get { return _info; }
        }

        private InheritablePropertyChangeInfo _info;
    }

    // Handler delegate for the (internal) InheritedPropertyChanged event
    internal delegate void InheritedPropertyChangedEventHandler(object sender, InheritedPropertyChangedEventArgs e);
}
