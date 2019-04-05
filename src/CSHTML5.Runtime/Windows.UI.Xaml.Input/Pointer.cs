
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
    public sealed class Pointer
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
