
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
}
