
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
using System.Threading;
using System.Windows.Threading;

namespace OpenSilver.Internal
{
    internal sealed class DebounceDispatcher
    {
        private readonly DispatcherTimer _timer;
        private Action _action;

        public DebounceDispatcher()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(OnTimerTick);
        }

        public void Debounce(TimeSpan interval, Action action)
        {
            _timer.Stop();

            _action = action;
            _timer.Interval = interval;

            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (Interlocked.Exchange(ref _action, null) is Action action)
            {
                action();
            }
        }
    }
}
