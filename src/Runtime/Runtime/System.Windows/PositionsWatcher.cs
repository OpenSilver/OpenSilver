
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
using System.Windows.Threading;

namespace System.Windows
{
    internal sealed class PositionsWatcher
    {
        private readonly HashSet<ControlToWatch> _controlsToWatch = new();
        private readonly DispatcherTimer _timer = new();
        private TimeSpan _interval = TimeSpan.FromMilliseconds(500);

        internal PositionsWatcher()
        {
            _timer.Interval = Interval;
            _timer.Tick += OnTick;
        }

        internal TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (_timer != null)
                {
                    _timer.Interval = value;
                }
            }
        }

        internal ControlToWatch AddControlToWatch(UIElement elementToWatch, Action<ControlToWatch> callback)
        {
            var ctw = new ControlToWatch(elementToWatch, callback);
            _controlsToWatch.Add(ctw);
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
            return ctw;
        }

        internal void RemoveControlToWatch(ControlToWatch control)
        {
            _controlsToWatch.Remove(control);
            if (_controlsToWatch.Count == 0)
            {
                _timer.Stop();
            }
        }

        private void OnTick(object sender, object e)
        {
            foreach (ControlToWatch ctw in _controlsToWatch.ToArray())
            {
                ctw.InvokeCallback();
            }
        }
    }
}
