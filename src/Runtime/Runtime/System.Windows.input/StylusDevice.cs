
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

#if !MIGRATION
using Windows.Foundation;
using MouseEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Reports stylus device information, such as the collection of stylus points associated
    /// with the input.
    /// </summary>
    public sealed class StylusDevice
    {
        private readonly double _x;
        private readonly double _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="StylusDevice"/> class.
        /// </summary>
        internal StylusDevice(MouseEventArgs args)
        {
            _x = args._pointerAbsoluteX;
            _y = args._pointerAbsoluteY;
        }

        /// <summary>
        /// Gets the type of the tablet device.
        /// </summary>
        /// <returns>
        /// The type of the tablet device.
        /// </returns>
        [OpenSilver.NotImplemented]
        public TabletDeviceType DeviceType => TabletDeviceType.Mouse;

        /// <summary>
        /// Gets or sets a value that indicates whether the stylus is inverted.
        /// </summary>
        /// <returns>
        /// true if the stylus is inverted; otherwise, false. The default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool Inverted => false;

        /// <summary>
        /// Returns the stylus points collected since the last mouse event.
        /// </summary>
        /// <param name="relativeTo">
        /// Specifies the offset for the object (typically an <see cref="Controls.InkPresenter"/>)
        /// that should be applied to captured points.
        /// </param>
        /// <returns>
        /// A collection of the stylus points collected since the last mouse event.
        /// </returns>
        public StylusPointCollection GetStylusPoints(UIElement relativeTo)
        {
            Point p = MouseEventArgs.GetPosition(new Point(_x, _y), relativeTo);
            return new StylusPointCollection
            {
                new StylusPoint(p.X, p.Y),
            };
        }
    }
}
