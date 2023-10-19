
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OpenSilver.Internal;

internal interface IRectangleAdapter
{
    double ActualWidth { get; }
    double ActualHeight { get; }
    Size DesiredSize { get; }
    Visibility Visibility { get; set; }
    Geometry Clip { get; set; }
    Brush Fill { get; set; }
}

internal static class RectangleAdapterProvider
{
    public static IRectangleAdapter From(object o)
        => o switch
        {
            Rectangle r => new RectangleToRectangleAdapter(r),
            Border b => new BorderToRectangleAdapter(b),
            _ => null,
        };

    private sealed class RectangleToRectangleAdapter : RectangleAdapterBase<Rectangle>
    {
        public RectangleToRectangleAdapter(Rectangle rectangle)
            : base(rectangle)
        {
        }

        public override Brush Fill
        {
            get => Element.Fill;
            set => Element.Fill = value;
        }
    }

    private sealed class BorderToRectangleAdapter : RectangleAdapterBase<Border>
    {
        public BorderToRectangleAdapter(Border border)
            : base(border)
        {
        }

        public override Brush Fill
        {
            get => Element.Background;
            set => Element.Background = value;
        }
    }

    private abstract class RectangleAdapterBase<T> : IRectangleAdapter
        where T : FrameworkElement
    {
        protected RectangleAdapterBase(T element)
        {
            Debug.Assert(element != null);
            Element = element;
        }

        public T Element { get; }

        public double ActualWidth => Element.ActualWidth;

        public double ActualHeight => Element.ActualHeight;

        public Size DesiredSize => Element.DesiredSize;

        public Visibility Visibility
        {
            get => Element.Visibility;
            set => Element.Visibility = value;
        }

        public Geometry Clip
        {
            get => Element.Clip;
            set => Element.Clip = value;
        }

        public abstract Brush Fill { get; set; }
    }
}
