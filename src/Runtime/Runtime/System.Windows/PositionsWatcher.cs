
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

#if MIGRATION
using System.Windows.Threading;
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal sealed class PositionsWatcher
    {
        private readonly List<ControlToWatch> _controlsToWatch = new List<ControlToWatch>();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
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

        internal ControlToWatch AddControlToWatch(UIElement elementToWatch, Action<Point, Size> callback)
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
