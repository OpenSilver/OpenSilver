
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

    internal static float InterpolateSingle(float from, float to, double progress) =>
        from + (float)((to - from) * progress);

    internal static decimal InterpolateDecimal(decimal from, decimal to, double progress) =>
        from + ((to - from) * (decimal)progress);

    internal static byte InterpolateByte(byte from, byte to, double progress) =>
        (byte)(from + (int)((to - from + (double)0.5) * progress));

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

    internal static Size InterpolateSize(Size from, Size to, double progress) =>
        (Size)InterpolateVector((Vector)from, (Vector)to, progress);

    internal static Vector InterpolateVector(Vector from, Vector to, double progress) =>
        from + ((to - from) * progress);

    internal static Thickness InterpolateThickness(Thickness from, Thickness to, double progress) =>
        new Thickness(
            InterpolateDouble(from.Left, to.Left, progress),
            InterpolateDouble(from.Top, to.Top, progress),
            InterpolateDouble(from.Right, to.Right, progress),
            InterpolateDouble(from.Bottom, to.Bottom, progress));

    internal static Rect InterpolateRect(Rect from, Rect to, double progress) =>
        new Rect(
            InterpolateDouble(from.X, to.X, progress),
            InterpolateDouble(from.Y, to.Y, progress),
            InterpolateDouble(from.Width, to.Width, progress),
            InterpolateDouble(from.Height, to.Height, progress));

    internal static float ScaleSingle(float value, double factor) => (float)((double)value * factor);

    internal static byte AddByte(byte value1, byte value2) => (byte)(value1 + value2);

    internal static byte SubtractByte(byte value1, byte value2) => (byte)(value1 - value2);

    internal static byte ScaleByte(byte value, double factor) => (byte)(value * factor);

    internal static short AddInt16(short value1, short value2) => (short)(value1 + value2);

    internal static short SubtractInt16(short value1, short value2) => (short)(value1 - value2);

    internal static short ScaleInt16(short value, double factor) => (short)(value * factor);

    internal static int ScaleInt32(int value, double factor) => (int)(value * factor);

    internal static long ScaleInt64(long value, double factor) => (long)(value * factor);

    internal static Size AddSize(Size value1, Size value2) =>
        new Size(value1.Width + value2.Width, value1.Height + value2.Height);

    internal static Size SubtractSize(Size value1, Size value2) =>
        new Size(value1.Width - value2.Width, value1.Height - value2.Height);

    internal static Size ScaleSize(Size value, double factor) =>
        new Size(value.Width * factor, value.Height * factor);

    internal static Point AddPoint(Point value1, Point value2) =>
        new Point(value1.X + value2.X, value1.Y + value2.Y);

    internal static Point SubtractPoint(Point value1, Point value2) =>
        new Point(value1.X - value2.X, value1.Y - value2.Y);

    internal static Point ScalePoint(Point value, double factor) =>
        new Point(value.X * factor, value.Y * factor);

    internal static Thickness AddThickness(Thickness value1, Thickness value2) =>
        new Thickness(
            value1.Left + value2.Left,
            value1.Top + value2.Top,
            value1.Right + value2.Right,
            value1.Bottom + value2.Bottom);

    internal static Thickness SubtractThickness(Thickness value1, Thickness value2) =>
        new Thickness(
            value1.Left - value2.Left,
            value1.Top - value2.Top,
            value1.Right - value2.Right,
            value1.Bottom - value2.Bottom);

    internal static Thickness ScaleThickness(Thickness value, double factor) =>
        new Thickness(
            factor * value.Left,
            factor * value.Top,
            factor * value.Right,
            factor * value.Bottom);

    internal static Rect AddRect(Rect value1, Rect value2) =>
        new Rect(AddPoint(value1.Location, value2.Location), AddSize(value1.Size, value2.Size));

    internal static Rect SubtractRect(Rect value1, Rect value2) =>
        new Rect(SubtractPoint(value1.Location, value2.Location), SubtractSize(value1.Size, value2.Size));

    internal static Rect ScaleRect(Rect value, double factor) =>
        new Rect(ScalePoint(value.Location, factor), ScaleSize(value.Size, factor));

    internal static bool IsValidAnimationValueDouble(double value) => !IsInvalidDouble(value);

    internal static bool IsValidAnimationValueSingle(float value) => !IsInvalidDouble(value);

    internal static bool IsValidAnimationValuePoint(Point value) =>
        !IsInvalidDouble(value.X) && !IsInvalidDouble(value.Y);

    internal static bool IsValidAnimationValueVector(Vector value) =>
        !IsInvalidDouble(value.X) && !IsInvalidDouble(value.Y);

    internal static bool IsValidAnimationValueSize(Size value) =>
        !IsInvalidDouble(value.Width) && !IsInvalidDouble(value.Height);

    internal static bool IsValidAnimationValueThickness(Thickness value) =>
        !IsInvalidDouble(value.Left) &&
        !IsInvalidDouble(value.Top) &&
        !IsInvalidDouble(value.Right) &&
        !IsInvalidDouble(value.Bottom);

    internal static bool IsValidAnimationValueRect(Rect value) =>
        IsValidAnimationValuePoint(value.Location) &&
        IsValidAnimationValueSize(value.Size) &&
        !value.IsEmpty;

    internal static bool IsValidAnimationValueDecimal(decimal value) => true;

    internal static bool IsValidAnimationValueByte(byte value) => true;

    internal static bool IsValidAnimationValueInt16(int value) => true;

    internal static bool IsValidAnimationValueInt32(int value) => true;

    internal static bool IsValidAnimationValueInt64(long value) => true;

    internal static bool IsValidAnimationValueColor(Color value) => true;

    private static bool IsInvalidDouble(double value) => double.IsInfinity(value) || double.IsNaN(value);
}
