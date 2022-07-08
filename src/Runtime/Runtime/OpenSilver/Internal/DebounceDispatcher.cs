
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

#if MIGRATION
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
    internal class DebounceDispatcher
    {
        private DispatcherTimer _timer;

        public void Debounce(TimeSpan interval, Action action)
        {
            _timer?.Stop();
            _timer = null;

            // timer is recreated for each event and effectively resets the timeout. 
            // Action only fires after timeout has fully elapsed without other events firing in between
            _timer = new DispatcherTimer
            {
                Interval = interval
            };

            _timer.Tick += (s, e) =>
            {
                if (_timer == null)
                    return;
                _timer?.Stop();
                _timer = null;
                action.Invoke();
            };

            _timer.Start();
        }
    }
}
