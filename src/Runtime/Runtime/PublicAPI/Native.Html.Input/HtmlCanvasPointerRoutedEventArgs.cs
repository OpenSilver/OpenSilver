

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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CSHTML5.Native.Html.Controls;

namespace CSHTML5.Native.Html.Input
{
    public class HtmlCanvasPointerRoutedEventArgs : MouseButtonEventArgs
    {
        public readonly HtmlCanvas HtmlCanvas;

        // Note: there are multiple constructor overloads.
        public HtmlCanvasPointerRoutedEventArgs(MouseEventArgs e, HtmlCanvas htmlCanvas)
            : base(e.MouseDevice, e.Timestamp, MouseButton.Left, MouseButtonState.Released, e.IsTouchEvent, e.KeyModifiers, e._pointerAbsoluteX, e._pointerAbsoluteY)
        {
            this.Source = e.OriginalSource;
            this.Handled = e.Handled;
            this.Pointer = e.Pointer;
            this.HtmlCanvas = htmlCanvas;
        }

        internal HtmlCanvasPointerRoutedEventArgs(MouseButtonEventArgs e, HtmlCanvas htmlCanvas)
            : base(e.MouseDevice, e.Timestamp, e.ChangedButton, e.ButtonState, e.IsTouchEvent, e.KeyModifiers, e._pointerAbsoluteX, e._pointerAbsoluteY)
        {
            this.Source = e.OriginalSource;
            this.Handled = e.Handled;
            this.Pointer = e.Pointer;
            this.HtmlCanvas = htmlCanvas;
        }

        // Note: there are multiple constructor overloads.
        public HtmlCanvasPointerRoutedEventArgs(RightTappedRoutedEventArgs e, HtmlCanvas htmlCanvas)
            : base(e.MouseDevice, e.Timestamp, MouseButton.Right, MouseButtonState.Released, e.IsTouchEvent, e.KeyModifiers, e._pointerAbsoluteX, e._pointerAbsoluteY)
        {
            this.Source = e.OriginalSource;
            this.Handled = e.Handled;
            this.HtmlCanvas = htmlCanvas;
        }


        public Point GetPosition(HtmlCanvasElement relativeTo)
        {
            if (relativeTo == null)
            {
                return base.GetPosition(null);
            }
            else
            {
                Point pointerPoint = base.GetPosition(this.HtmlCanvas);
                Stack<HtmlCanvasElement> elem = HtmlCanvas.SearchElement(this.HtmlCanvas, relativeTo);

                double x = pointerPoint.X;
                double y = pointerPoint.Y;
                if (elem == null)
                    return pointerPoint;

                while (elem.Count > 0)
                {
                    var e = elem.Pop();
                    x -= e.X;
                    y -= e.Y;
                }


                pointerPoint = new Point(x, y);
                return pointerPoint;
            }
        }
    }
}
