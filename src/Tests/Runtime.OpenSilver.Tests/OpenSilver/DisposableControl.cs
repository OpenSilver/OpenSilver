
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
using System.Windows;
using System.Windows.Controls.Primitives;

namespace OpenSilver
{
    internal abstract class DisposableControlWrapper<T> : IDisposable
        where T : FrameworkElement
    {
        protected DisposableControlWrapper(T control)
        {
            Control = control;
        }

        public T Control { get; }

        public abstract void Dispose();
    }

    internal class FocusableControlWrapper<T> : DisposableControlWrapper<T>
        where T : FrameworkElement
    {
        private Popup _popup;
        private bool _isDisposed;

        public FocusableControlWrapper(T control)
            : base(control)
        {
            _popup = new Popup
            {
                Child = control,
                IsOpen = true,
            };
        }

        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _popup.IsOpen = false;
            _popup = null;
        }
    }
}
