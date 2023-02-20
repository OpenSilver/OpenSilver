
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
using System.Diagnostics;
using DotNetForHtml5.Core;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal sealed class ControlToWatch
    {
        private readonly UIElement _control;
        private readonly Action<Point, Size> _callback;
        private Size _previousSize;
        private Point _previousPosition;

        internal ControlToWatch(UIElement control, Action<Point, Size> callback)
        {
            Debug.Assert(control != null);
            Debug.Assert(callback != null);

            _control = control;
            _callback = callback;

            Initialize();
        }

        internal void InvokeCallback()
        {
            Point position = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(_control);
            Size size = _control.GetBoundingClientSize();
            if (position != _previousPosition || size != _previousSize)
            {
                _callback(position, size);
                _previousPosition = position;
                _previousSize = size;
            }
        }

        private void Initialize()
        {
            _previousPosition = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(_control);
            _previousSize = _control switch
            {
                FrameworkElement fe => fe.INTERNAL_GetActualWidthAndHeight(),
                _ => new Size(),
            };
        }
    }
}
