

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    internal partial class PositionsWatcher
    {
        private List<ControlToWatch> _controlsToWatch = new List<ControlToWatch>();
        private DispatcherTimer _timer = new DispatcherTimer();

        internal PositionsWatcher()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, object e)
        {
            foreach (ControlToWatch controlToWatch in _controlsToWatch)
            {
                //get the element's new Position:
                Point elementCurrentPosition = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(controlToWatch.ControltoWatch);
                Size elementCurrentSize;
                if (controlToWatch.ControltoWatch is FrameworkElement)
                {
                    elementCurrentSize = ((FrameworkElement)controlToWatch.ControltoWatch).INTERNAL_GetActualWidthAndHeight();
                }
                else
                {
                    elementCurrentSize = new Size();
                }

                if (elementCurrentPosition != controlToWatch.PreviousPosition || !INTERNAL_SizeComparisonHelpers.AreSizesEqual(elementCurrentSize, controlToWatch.PreviousSize))
                {
                    controlToWatch.OnPositionOrSizeChanged(elementCurrentPosition, elementCurrentSize);
                    controlToWatch.PreviousPosition = elementCurrentPosition;
                    controlToWatch.PreviousSize = elementCurrentSize;
                }
            }
        }

        internal ControlToWatch AddControlToWatch(UIElement elementToWatch, Action<Point, Size> callback)
        {
            ControlToWatch returnvalue = new ControlToWatch(elementToWatch, callback);

            Size elementCurrentSize;
            if (elementToWatch is FrameworkElement)
            {
                elementCurrentSize = ((FrameworkElement)elementToWatch).INTERNAL_GetActualWidthAndHeight();
            }
            else
            {
                elementCurrentSize = new Size();
            }

            Point elementCurrentPosition = INTERNAL_PopupsManager.GetUIElementAbsolutePosition(elementToWatch);
            returnvalue.PreviousPosition = elementCurrentPosition;
            returnvalue.PreviousSize = elementCurrentSize;

            _controlsToWatch.Add(returnvalue);
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
            return returnvalue; //note: we need to return the value to be able to remove it afterwards.
        }

        internal void RemoveControlToWatch(ControlToWatch controlToStopWatching)
        {
            _controlsToWatch.Remove(controlToStopWatching);
            if (_controlsToWatch.Count == 0)
            {
                _timer.Stop();
            }
        }
    }
}
