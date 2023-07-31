
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
        private readonly Action<ControlToWatch> _callback;
        private Rect _bounds;

        internal UIElement Control => _control;
        internal Rect Bounds => _bounds;

        internal ControlToWatch(UIElement control, Action<ControlToWatch> callback)
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
            Rect bounds = new(position, size);
            if (_bounds != bounds)
            {
                _bounds = bounds;
                _callback(this);
            }
        }

        private void Initialize()
        {
            Point position = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(_control);
            Size size = _control switch
            {
                FrameworkElement fe => fe.RenderSize,
                _ => new Size(),
            };

            _bounds = new Rect(position, size);
        }
    }
}
