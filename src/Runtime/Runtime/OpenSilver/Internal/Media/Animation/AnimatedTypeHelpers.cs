
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

namespace OpenSilver.Internal.Media.Animation;

internal static class AnimatedTypeHelpers
{
    internal static Color InterpolateColor(Color from, Color to, double progress) =>
        Color.FromArgb(
            (byte)(from.A + (to.A - from.A) * progress),
            (byte)(from.R + (to.R - from.R) * progress),
            (byte)(from.G + (to.G - from.G) * progress),
            (byte)(from.B + (to.B - from.B) * progress));

    internal static double InterpolateDouble(double from, double to, double progress) =>
        from + ((to - from) * progress);

    internal static short InterpolateInt16(short from, short to, double progress)
    {
        if (progress == 0.0)
        {
            return from;
        }
        else if (progress == 1.0)
        {
            return to;
        }
        else
        {
            double addend = to - from;
            addend *= progress;
            addend += (addend > 0.0) ? 0.5 : -0.5;

            return (short)(from + (short)addend);
        }
    }

    internal static int InterpolateInt32(int from, int to, double progress)
    {
        if (progress == 0.0)
        {
            return from;
        }
        else if (progress == 1.0)
        {
            return to;
        }
        else
        {
            double addend = to - from;
            addend *= progress;
            addend += (addend > 0.0) ? 0.5 : -0.5;

            return from + (int)addend;
        }
    }

    internal static long InterpolateInt64(long from, long to, double progress)
    {
        if (progress == 0.0)
        {
            return from;
        }
        else if (progress == 1.0)
        {
            return to;
        }
        else
        {
            double addend = to - from;
            addend *= progress;
            addend += (addend > 0.0) ? 0.5 : -0.5;

            return from + (long)addend;
        }
    }

    internal static Point InterpolatePoint(Point from, Point to, double progress) =>
        new(from.X + (to.X - from.X) * progress, from.Y + (to.Y - from.Y) * progress);
}
