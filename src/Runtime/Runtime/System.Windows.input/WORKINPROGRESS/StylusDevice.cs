

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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
    public sealed partial class StylusDevice
    {
        [OpenSilver.NotImplemented]
        public TabletDeviceType DeviceType { get; private set; }

        /// <summary>
        /// Returns the stylus points collected since the last mouse event.
        /// </summary>
        /// <param name="relativeTo">Specifies the offset for the object (typically an System.Windows.Controls.InkPresenter)
        /// that should be applied to captured points.</param>
        /// <returns>A collection of the stylus points collected since the last mouse event.</returns>
        [OpenSilver.NotImplemented]
        public StylusPointCollection GetStylusPoints(UIElement relativeTo)
        {
            throw new NotImplementedException();
        }
    }
}
