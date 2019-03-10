
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
