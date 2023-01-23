

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.Foundation;
#endif


#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal partial class ControlToWatch
    {
        internal ControlToWatch(UIElement controlToWatch, Action<Point, Size> OnPositionOrSizeChangedCallback)
        {
            Debug.Assert(controlToWatch != null);
            Debug.Assert(OnPositionOrSizeChangedCallback != null);
            ControltoWatch = controlToWatch;
            OnPositionOrSizeChanged = OnPositionOrSizeChangedCallback;
        }

        internal UIElement ControltoWatch;
        internal Size PreviousSize;
        internal Point PreviousPosition;
        internal Action<Point, Size> OnPositionOrSizeChanged;
    }
}
