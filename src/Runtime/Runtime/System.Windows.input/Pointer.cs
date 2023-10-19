
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

namespace System.Windows.Input
{
    /// <summary>
    /// Represents a pointer device.
    /// </summary>
    public sealed class Pointer
    {
        internal static UIElement INTERNAL_captured;

        /// <summary>
        /// Gets a value that determines whether the pointer device was in contact with
        /// a sensor or digitizer at the time that the event was reported.
        /// </summary>
        public bool IsInContact { get; private set; }

        /// <summary>
        /// Gets a value that determines whether the pointer device was in detection
        /// range of a sensor or digitizer at the time that the event was reported.
        /// </summary>
        public bool IsInRange { get; private set; }
        
        /// <summary>
        /// Gets the PointerDeviceType for the pointer device.
        /// </summary>
        public PointerDeviceType PointerDeviceType { get; private set; }

        /// <summary>
        /// Gets the system-generated identifier for this pointer reference.
        /// </summary>
        public uint PointerId { get; private set; }
    }
}
