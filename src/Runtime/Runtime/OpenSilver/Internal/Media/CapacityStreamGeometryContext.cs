
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

using System.Windows;
using System.Windows.Media;

namespace OpenSilver.Internal.Media;

internal abstract class CapacityStreamGeometryContext : StreamGeometryContext
{
    internal virtual void SetFigureCount(int figureCount) { }
    internal virtual void SetSegmentCount(int segmentCount) { }

    internal void AddRect(Rect rect) => AddRect(rect, Matrix.Identity);

    internal virtual void AddRect(Rect rect, Matrix transform)
    {
        Point topLeft = rect.TopLeft;
        Point topRight = rect.TopRight;
        Point bottomLeft = rect.BottomLeft;
        Point bottomRight = rect.BottomRight;

        if (!transform.IsIdentity)
        {
            topLeft *= transform;
            topRight *= transform;
            bottomLeft *= transform;
            bottomRight *= transform;
        }

        BeginFigure(topLeft, true, true);
        LineTo(topRight, true, false);
        LineTo(bottomRight, true, false);
        LineTo(bottomLeft, true, false);
    }
}