
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
