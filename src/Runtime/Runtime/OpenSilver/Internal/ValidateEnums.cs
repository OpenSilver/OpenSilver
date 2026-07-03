
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

using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilver.Internal;

internal static class ValidateEnums
{
    public static bool IsFillRuleValid(object o)
    {
        var value = (FillRule)o;
        return value == FillRule.EvenOdd ||
               value == FillRule.Nonzero;
    }

    public static bool IsPenLineCapValid(object o)
    {
        var value = (PenLineCap)o;
        return value == PenLineCap.Flat ||
               value == PenLineCap.Square ||
               value == PenLineCap.Round ||
               value == PenLineCap.Triangle;
    }

    public static bool IsPenLineJoinValid(object o)
    {
        var value = (PenLineJoin)o;
        return value == PenLineJoin.Miter ||
               value == PenLineJoin.Bevel ||
               value == PenLineJoin.Round;
    }

    public static bool IsGeometryCombineModeValid(object o)
    {
        var value = (GeometryCombineMode)o;
        return value == GeometryCombineMode.Union ||
               value == GeometryCombineMode.Intersect ||
               value == GeometryCombineMode.Xor ||
               value == GeometryCombineMode.Exclude;
    }

    public static bool IsBrushMappingModeValid(object o)
    {
        var value = (BrushMappingMode)o;
        return value == BrushMappingMode.Absolute ||
               value == BrushMappingMode.RelativeToBoundingBox;
    }

    public static bool IsGradientSpreadMethodValid(object o)
    {
        var value = (GradientSpreadMethod)o;
        return value == GradientSpreadMethod.Pad ||
               value == GradientSpreadMethod.Reflect ||
               value == GradientSpreadMethod.Repeat;
    }

    public static bool IsColorInterpolationModeValid(object o)
    {
        var value = (ColorInterpolationMode)o;
        return value == ColorInterpolationMode.ScRgbLinearInterpolation ||
               value == ColorInterpolationMode.SRgbLinearInterpolation;
    }

    public static bool IsTileModeValid(object o)
    {
        var value = (TileMode)o;
        return value == TileMode.None ||
               value == TileMode.Tile ||
               value == TileMode.FlipX ||
               value == TileMode.FlipY ||
               value == TileMode.FlipXY;
    }

    public static bool IsAlignmentXValid(object o)
    {
        var value = (AlignmentX)o;
        return value == AlignmentX.Left ||
               value == AlignmentX.Center ||
               value == AlignmentX.Right;
    }

    public static bool IsAlignmentYValid(object o)
    {
        var value = (AlignmentY)o;
        return value == AlignmentY.Top ||
               value == AlignmentY.Center ||
               value == AlignmentY.Bottom;
    }

    public static bool IsStretchValid(object o)
    {
        var value = (Stretch)o;
        return value == Stretch.None ||
               value == Stretch.Fill ||
               value == Stretch.Uniform ||
               value == Stretch.UniformToFill;
    }

    public static bool IsStretchDirectionValid(object o)
    {
        var value = (StretchDirection)o;
        return value == StretchDirection.UpOnly ||
               value == StretchDirection.DownOnly ||
               value == StretchDirection.Both;
    }
}
