

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Input;
#else
using Windows.Devices.Input;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Represents a pointer device.
    /// </summary>
    public sealed partial class Pointer
    {

        internal static UIElement INTERNAL_captured;

        bool _isInContact = false;
        /// <summary>
        /// Gets a value that determines whether the pointer device was in contact with
        /// a sensor or digitizer at the time that the event was reported.
        /// </summary>
        public bool IsInContact
        {
            get { return _isInContact; }
            private set { _isInContact = value; }
        }

        bool _isInRange = false;
        /// <summary>
        /// Gets a value that determines whether the pointer device was in detection
        /// range of a sensor or digitizer at the time that the event was reported.
        /// </summary>
        public bool IsInRange
        {
            get { return _isInRange; }
            private set { _isInRange = value; }
        }



        PointerDeviceType _pointerDeviceType;
        /// <summary>
        /// Gets the PointerDeviceType for the pointer device.
        /// </summary>
        public PointerDeviceType PointerDeviceType
        {
            get { return _pointerDeviceType; }
            private set { _pointerDeviceType = value; }
        }

        uint _pointerId;
        /// <summary>
        /// Gets the system-generated identifier for this pointer reference.
        /// </summary>
        public uint PointerId
        {
            get { return _pointerId; }
            private set { _pointerId = value; }
        }
    }
}
