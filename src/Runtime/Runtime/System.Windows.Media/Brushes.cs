
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

using System.Threading;

namespace System.Windows.Media;

/// <summary>
/// Implements a set of predefined <see cref="SolidColorBrush"/> objects.
/// </summary>
public static class Brushes
{
    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF0F8FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush AliceBlue => GetKnownSolidColorBrush(ref _aliceBlue, Colors.KnownColor.AliceBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFAEBD7</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush AntiqueWhite => GetKnownSolidColorBrush(ref _antiqueWhite, Colors.KnownColor.AntiqueWhite);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00FFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Aqua => GetKnownSolidColorBrush(ref _aqua, Colors.KnownColor.Aqua);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF7FFFD4</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Aquamarine => GetKnownSolidColorBrush(ref _aquamarine, Colors.KnownColor.Aquamarine);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF0FFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Azure => GetKnownSolidColorBrush(ref _azure, Colors.KnownColor.Azure);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF5F5DC</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Beige => GetKnownSolidColorBrush(ref _beige, Colors.KnownColor.Beige);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFE4C4</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Bisque => GetKnownSolidColorBrush(ref _bisque, Colors.KnownColor.Bisque);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF000000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Black => GetKnownSolidColorBrush(ref _black, Colors.KnownColor.Black);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFEBCD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush BlanchedAlmond => GetKnownSolidColorBrush(ref _blanchedAlmond, Colors.KnownColor.BlanchedAlmond);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF0000FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Blue => GetKnownSolidColorBrush(ref _blue, Colors.KnownColor.Blue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF8A2BE2</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush BlueViolet => GetKnownSolidColorBrush(ref _blueViolet, Colors.KnownColor.BlueViolet);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFA52A2A</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Brown => GetKnownSolidColorBrush(ref _brown, Colors.KnownColor.Brown);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDEB887</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush BurlyWood => GetKnownSolidColorBrush(ref _burlyWood, Colors.KnownColor.BurlyWood);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF5F9EA0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush CadetBlue => GetKnownSolidColorBrush(ref _cadetBlue, Colors.KnownColor.CadetBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF7FFF00</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Chartreuse => GetKnownSolidColorBrush(ref _chartreuse, Colors.KnownColor.Chartreuse);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFD2691E</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Chocolate => GetKnownSolidColorBrush(ref _chocolate, Colors.KnownColor.Chocolate);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF7F50</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Coral => GetKnownSolidColorBrush(ref _coral, Colors.KnownColor.Coral);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF6495ED</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush CornflowerBlue => GetKnownSolidColorBrush(ref _cornflowerBlue, Colors.KnownColor.CornflowerBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFF8DC</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Cornsilk => GetKnownSolidColorBrush(ref _cornsilk, Colors.KnownColor.Cornsilk);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDC143C</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Crimson => GetKnownSolidColorBrush(ref _crimson, Colors.KnownColor.Crimson);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00FFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Cyan => GetKnownSolidColorBrush(ref _cyan, Colors.KnownColor.Cyan);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00008B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkBlue => GetKnownSolidColorBrush(ref _darkBlue, Colors.KnownColor.DarkBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF008B8B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkCyan => GetKnownSolidColorBrush(ref _darkCyan, Colors.KnownColor.DarkCyan);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFB8860B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkGoldenrod => GetKnownSolidColorBrush(ref _darkGoldenrod, Colors.KnownColor.DarkGoldenrod);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFA9A9A9</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkGray => GetKnownSolidColorBrush(ref _darkGray, Colors.KnownColor.DarkGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF006400</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkGreen => GetKnownSolidColorBrush(ref _darkGreen, Colors.KnownColor.DarkGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFBDB76B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkKhaki => GetKnownSolidColorBrush(ref _darkKhaki, Colors.KnownColor.DarkKhaki);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF8B008B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkMagenta => GetKnownSolidColorBrush(ref _darkMagenta, Colors.KnownColor.DarkMagenta);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF556B2F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkOliveGreen => GetKnownSolidColorBrush(ref _darkOliveGreen, Colors.KnownColor.DarkOliveGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF8C00</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkOrange => GetKnownSolidColorBrush(ref _darkOrange, Colors.KnownColor.DarkOrange);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF9932CC</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkOrchid => GetKnownSolidColorBrush(ref _darkOrchid, Colors.KnownColor.DarkOrchid);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF8B0000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkRed => GetKnownSolidColorBrush(ref _darkRed, Colors.KnownColor.DarkRed);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFE9967A</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkSalmon => GetKnownSolidColorBrush(ref _darkSalmon, Colors.KnownColor.DarkSalmon);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF8FBC8F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkSeaGreen => GetKnownSolidColorBrush(ref _darkSeaGreen, Colors.KnownColor.DarkSeaGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF483D8B</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkSlateBlue => GetKnownSolidColorBrush(ref _darkSlateBlue, Colors.KnownColor.DarkSlateBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF2F4F4F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkSlateGray => GetKnownSolidColorBrush(ref _darkSlateGray, Colors.KnownColor.DarkSlateGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00CED1</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkTurquoise => GetKnownSolidColorBrush(ref _darkTurquoise, Colors.KnownColor.DarkTurquoise);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF9400D3</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DarkViolet => GetKnownSolidColorBrush(ref _darkViolet, Colors.KnownColor.DarkViolet);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF1493</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DeepPink => GetKnownSolidColorBrush(ref _deepPink, Colors.KnownColor.DeepPink);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00BFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DeepSkyBlue => GetKnownSolidColorBrush(ref _deepSkyBlue, Colors.KnownColor.DeepSkyBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF696969</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DimGray => GetKnownSolidColorBrush(ref _dimGray, Colors.KnownColor.DimGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF1E90FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush DodgerBlue => GetKnownSolidColorBrush(ref _dodgerBlue, Colors.KnownColor.DodgerBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFB22222</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Firebrick => GetKnownSolidColorBrush(ref _firebrick, Colors.KnownColor.Firebrick);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFAF0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush FloralWhite => GetKnownSolidColorBrush(ref _floralWhite, Colors.KnownColor.FloralWhite);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF228B22</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush ForestGreen => GetKnownSolidColorBrush(ref _forestGreen, Colors.KnownColor.ForestGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF00FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Fuchsia => GetKnownSolidColorBrush(ref _fuchsia, Colors.KnownColor.Fuchsia);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDCDCDC</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Gainsboro => GetKnownSolidColorBrush(ref _gainsboro, Colors.KnownColor.Gainsboro);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF8F8FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush GhostWhite => GetKnownSolidColorBrush(ref _ghostWhite, Colors.KnownColor.GhostWhite);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFD700</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Gold => GetKnownSolidColorBrush(ref _gold, Colors.KnownColor.Gold);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDAA520</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Goldenrod => GetKnownSolidColorBrush(ref _goldenrod, Colors.KnownColor.Goldenrod);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF808080</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Gray => GetKnownSolidColorBrush(ref _gray, Colors.KnownColor.Gray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF008000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Green => GetKnownSolidColorBrush(ref _green, Colors.KnownColor.Green);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFADFF2F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush GreenYellow => GetKnownSolidColorBrush(ref _greenYellow, Colors.KnownColor.GreenYellow);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF0FFF0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Honeydew => GetKnownSolidColorBrush(ref _honeydew, Colors.KnownColor.Honeydew);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF69B4</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush HotPink => GetKnownSolidColorBrush(ref _hotPink, Colors.KnownColor.HotPink);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFCD5C5C</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush IndianRed => GetKnownSolidColorBrush(ref _indianRed, Colors.KnownColor.IndianRed);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF4B0082</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Indigo => GetKnownSolidColorBrush(ref _indigo, Colors.KnownColor.Indigo);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFFF0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Ivory => GetKnownSolidColorBrush(ref _ivory, Colors.KnownColor.Ivory);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF0E68C</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Khaki => GetKnownSolidColorBrush(ref _khaki, Colors.KnownColor.Khaki);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFE6E6FA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Lavender => GetKnownSolidColorBrush(ref _lavender, Colors.KnownColor.Lavender);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFF0F5</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LavenderBlush => GetKnownSolidColorBrush(ref _lavenderBlush, Colors.KnownColor.LavenderBlush);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF7CFC00</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LawnGreen => GetKnownSolidColorBrush(ref _lawnGreen, Colors.KnownColor.LawnGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFACD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LemonChiffon => GetKnownSolidColorBrush(ref _lemonChiffon, Colors.KnownColor.LemonChiffon);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFADD8E6</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightBlue => GetKnownSolidColorBrush(ref _lightBlue, Colors.KnownColor.LightBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF08080</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightCoral => GetKnownSolidColorBrush(ref _lightCoral, Colors.KnownColor.LightCoral);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFE0FFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightCyan => GetKnownSolidColorBrush(ref _lightCyan, Colors.KnownColor.LightCyan);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFAFAD2</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightGoldenrodYellow => GetKnownSolidColorBrush(ref _lightGoldenrodYellow, Colors.KnownColor.LightGoldenrodYellow);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF90EE90</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightGreen => GetKnownSolidColorBrush(ref _lightGreen, Colors.KnownColor.LightGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFD3D3D3</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightGray => GetKnownSolidColorBrush(ref _lightGray, Colors.KnownColor.LightGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFB6C1</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightPink => GetKnownSolidColorBrush(ref _lightPink, Colors.KnownColor.LightPink);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFA07A</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightSalmon => GetKnownSolidColorBrush(ref _lightSalmon, Colors.KnownColor.LightSalmon);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF20B2AA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightSeaGreen => GetKnownSolidColorBrush(ref _lightSeaGreen, Colors.KnownColor.LightSeaGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF87CEFA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightSkyBlue => GetKnownSolidColorBrush(ref _lightSkyBlue, Colors.KnownColor.LightSkyBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF778899</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightSlateGray => GetKnownSolidColorBrush(ref _lightSlateGray, Colors.KnownColor.LightSlateGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFB0C4DE</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightSteelBlue => GetKnownSolidColorBrush(ref _lightSteelBlue, Colors.KnownColor.LightSteelBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFFE0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LightYellow => GetKnownSolidColorBrush(ref _lightYellow, Colors.KnownColor.LightYellow);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00FF00</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Lime => GetKnownSolidColorBrush(ref _lime, Colors.KnownColor.Lime);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF32CD32</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush LimeGreen => GetKnownSolidColorBrush(ref _limeGreen, Colors.KnownColor.LimeGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFAF0E6</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Linen => GetKnownSolidColorBrush(ref _linen, Colors.KnownColor.Linen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF00FF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Magenta => GetKnownSolidColorBrush(ref _magenta, Colors.KnownColor.Magenta);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF800000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Maroon => GetKnownSolidColorBrush(ref _maroon, Colors.KnownColor.Maroon);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF66CDAA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumAquamarine => GetKnownSolidColorBrush(ref _mediumAquamarine, Colors.KnownColor.MediumAquamarine);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF0000CD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumBlue => GetKnownSolidColorBrush(ref _mediumBlue, Colors.KnownColor.MediumBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFBA55D3</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumOrchid => GetKnownSolidColorBrush(ref _mediumOrchid, Colors.KnownColor.MediumOrchid);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF9370DB</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumPurple => GetKnownSolidColorBrush(ref _mediumPurple, Colors.KnownColor.MediumPurple);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF3CB371</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumSeaGreen => GetKnownSolidColorBrush(ref _mediumSeaGreen, Colors.KnownColor.MediumSeaGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF7B68EE</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumSlateBlue => GetKnownSolidColorBrush(ref _mediumSlateBlue, Colors.KnownColor.MediumSlateBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00FA9A</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumSpringGreen => GetKnownSolidColorBrush(ref _mediumSpringGreen, Colors.KnownColor.MediumSpringGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF48D1CC</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumTurquoise => GetKnownSolidColorBrush(ref _mediumTurquoise, Colors.KnownColor.MediumTurquoise);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFC71585</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MediumVioletRed => GetKnownSolidColorBrush(ref _mediumVioletRed, Colors.KnownColor.MediumVioletRed);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF191970</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MidnightBlue => GetKnownSolidColorBrush(ref _midnightBlue, Colors.KnownColor.MidnightBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF5FFFA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MintCream => GetKnownSolidColorBrush(ref _mintCream, Colors.KnownColor.MintCream);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFE4E1</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush MistyRose => GetKnownSolidColorBrush(ref _mistyRose, Colors.KnownColor.MistyRose);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFE4B5</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Moccasin => GetKnownSolidColorBrush(ref _moccasin, Colors.KnownColor.Moccasin);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFDEAD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush NavajoWhite => GetKnownSolidColorBrush(ref _navajoWhite, Colors.KnownColor.NavajoWhite);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF000080</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Navy => GetKnownSolidColorBrush(ref _navy, Colors.KnownColor.Navy);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFDF5E6</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush OldLace => GetKnownSolidColorBrush(ref _oldLace, Colors.KnownColor.OldLace);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF808000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Olive => GetKnownSolidColorBrush(ref _olive, Colors.KnownColor.Olive);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF6B8E23</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush OliveDrab => GetKnownSolidColorBrush(ref _oliveDrab, Colors.KnownColor.OliveDrab);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFA500</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Orange => GetKnownSolidColorBrush(ref _orange, Colors.KnownColor.Orange);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF4500</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush OrangeRed => GetKnownSolidColorBrush(ref _orangeRed, Colors.KnownColor.OrangeRed);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDA70D6</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Orchid => GetKnownSolidColorBrush(ref _orchid, Colors.KnownColor.Orchid);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFEEE8AA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PaleGoldenrod => GetKnownSolidColorBrush(ref _paleGoldenrod, Colors.KnownColor.PaleGoldenrod);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF98FB98</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PaleGreen => GetKnownSolidColorBrush(ref _paleGreen, Colors.KnownColor.PaleGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFAFEEEE</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PaleTurquoise => GetKnownSolidColorBrush(ref _paleTurquoise, Colors.KnownColor.PaleTurquoise);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDB7093</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PaleVioletRed => GetKnownSolidColorBrush(ref _paleVioletRed, Colors.KnownColor.PaleVioletRed);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFEFD5</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PapayaWhip => GetKnownSolidColorBrush(ref _papayaWhip, Colors.KnownColor.PapayaWhip);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFDAB9</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PeachPuff => GetKnownSolidColorBrush(ref _peachPuff, Colors.KnownColor.PeachPuff);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFCD853F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Peru => GetKnownSolidColorBrush(ref _peru, Colors.KnownColor.Peru);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFC0CB</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Pink => GetKnownSolidColorBrush(ref _pink, Colors.KnownColor.Pink);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFDDA0DD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Plum => GetKnownSolidColorBrush(ref _plum, Colors.KnownColor.Plum);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFB0E0E6</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush PowderBlue => GetKnownSolidColorBrush(ref _powderBlue, Colors.KnownColor.PowderBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF800080</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Purple => GetKnownSolidColorBrush(ref _purple, Colors.KnownColor.Purple);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF0000</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Red => GetKnownSolidColorBrush(ref _red, Colors.KnownColor.Red);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFBC8F8F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush RosyBrown => GetKnownSolidColorBrush(ref _rosyBrown, Colors.KnownColor.RosyBrown);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF4169E1</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush RoyalBlue => GetKnownSolidColorBrush(ref _royalBlue, Colors.KnownColor.RoyalBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF8B4513</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SaddleBrown => GetKnownSolidColorBrush(ref _saddleBrown, Colors.KnownColor.SaddleBrown);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFA8072</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Salmon => GetKnownSolidColorBrush(ref _salmon, Colors.KnownColor.Salmon);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF4A460</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SandyBrown => GetKnownSolidColorBrush(ref _sandyBrown, Colors.KnownColor.SandyBrown);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF2E8B57</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SeaGreen => GetKnownSolidColorBrush(ref _seaGreen, Colors.KnownColor.SeaGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFF5EE</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SeaShell => GetKnownSolidColorBrush(ref _seaShell, Colors.KnownColor.SeaShell);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFA0522D</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Sienna => GetKnownSolidColorBrush(ref _sienna, Colors.KnownColor.Sienna);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFC0C0C0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Silver => GetKnownSolidColorBrush(ref _silver, Colors.KnownColor.Silver);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF87CEEB</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SkyBlue => GetKnownSolidColorBrush(ref _skyBlue, Colors.KnownColor.SkyBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF6A5ACD</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SlateBlue => GetKnownSolidColorBrush(ref _slateBlue, Colors.KnownColor.SlateBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF708090</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SlateGray => GetKnownSolidColorBrush(ref _slateGray, Colors.KnownColor.SlateGray);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFAFA</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Snow => GetKnownSolidColorBrush(ref _snow, Colors.KnownColor.Snow);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF00FF7F</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SpringGreen => GetKnownSolidColorBrush(ref _springGreen, Colors.KnownColor.SpringGreen);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF4682B4</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush SteelBlue => GetKnownSolidColorBrush(ref _steelBlue, Colors.KnownColor.SteelBlue);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFD2B48C</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Tan => GetKnownSolidColorBrush(ref _tan, Colors.KnownColor.Tan);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF008080</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Teal => GetKnownSolidColorBrush(ref _teal, Colors.KnownColor.Teal);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFD8BFD8</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Thistle => GetKnownSolidColorBrush(ref _thistle, Colors.KnownColor.Thistle);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFF6347</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Tomato => GetKnownSolidColorBrush(ref _tomato, Colors.KnownColor.Tomato);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#00FFFFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Transparent => GetKnownSolidColorBrush(ref _transparent, Colors.KnownColor.Transparent);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF40E0D0</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Turquoise => GetKnownSolidColorBrush(ref _turquoise, Colors.KnownColor.Turquoise);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFEE82EE</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Violet => GetKnownSolidColorBrush(ref _violet, Colors.KnownColor.Violet);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF5DEB3</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Wheat => GetKnownSolidColorBrush(ref _wheat, Colors.KnownColor.Wheat);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFFFF</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush White => GetKnownSolidColorBrush(ref _white, Colors.KnownColor.White);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFF5F5F5</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush WhiteSmoke => GetKnownSolidColorBrush(ref _whiteSmoke, Colors.KnownColor.WhiteSmoke);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FFFFFF00</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush Yellow => GetKnownSolidColorBrush(ref _yellow, Colors.KnownColor.Yellow);

    /// <summary>
    /// Gets the solid fill color that has a hexadecimal value of <c>#FF9ACD32</c>.
    /// </summary>
    /// <returns>
    /// A solid fill color.
    /// </returns>
    public static SolidColorBrush YellowGreen => GetKnownSolidColorBrush(ref _yellowGreen, Colors.KnownColor.YellowGreen);

    private static SolidColorBrush GetKnownSolidColorBrush(ref SolidColorBrush brush, Colors.KnownColor knownColor)
    {
        if (brush is null)
        {
            var newBrush = new SolidColorBrush(Color.FromUInt32((uint)knownColor));
            newBrush.Seal();
            Interlocked.CompareExchange(ref brush, newBrush, null);
        }
        return brush;
    }

    private static SolidColorBrush _aliceBlue;
    private static SolidColorBrush _antiqueWhite;
    private static SolidColorBrush _aqua;
    private static SolidColorBrush _aquamarine;
    private static SolidColorBrush _azure;
    private static SolidColorBrush _beige;
    private static SolidColorBrush _bisque;
    private static SolidColorBrush _black;
    private static SolidColorBrush _blanchedAlmond;
    private static SolidColorBrush _blue;
    private static SolidColorBrush _blueViolet;
    private static SolidColorBrush _brown;
    private static SolidColorBrush _burlyWood;
    private static SolidColorBrush _cadetBlue;
    private static SolidColorBrush _chartreuse;
    private static SolidColorBrush _chocolate;
    private static SolidColorBrush _coral;
    private static SolidColorBrush _cornflowerBlue;
    private static SolidColorBrush _cornsilk;
    private static SolidColorBrush _crimson;
    private static SolidColorBrush _cyan;
    private static SolidColorBrush _darkBlue;
    private static SolidColorBrush _darkCyan;
    private static SolidColorBrush _darkGoldenrod;
    private static SolidColorBrush _darkGray;
    private static SolidColorBrush _darkGreen;
    private static SolidColorBrush _darkKhaki;
    private static SolidColorBrush _darkMagenta;
    private static SolidColorBrush _darkOliveGreen;
    private static SolidColorBrush _darkOrange;
    private static SolidColorBrush _darkOrchid;
    private static SolidColorBrush _darkRed;
    private static SolidColorBrush _darkSalmon;
    private static SolidColorBrush _darkSeaGreen;
    private static SolidColorBrush _darkSlateBlue;
    private static SolidColorBrush _darkSlateGray;
    private static SolidColorBrush _darkTurquoise;
    private static SolidColorBrush _darkViolet;
    private static SolidColorBrush _deepPink;
    private static SolidColorBrush _deepSkyBlue;
    private static SolidColorBrush _dimGray;
    private static SolidColorBrush _dodgerBlue;
    private static SolidColorBrush _firebrick;
    private static SolidColorBrush _floralWhite;
    private static SolidColorBrush _forestGreen;
    private static SolidColorBrush _fuchsia;
    private static SolidColorBrush _gainsboro;
    private static SolidColorBrush _ghostWhite;
    private static SolidColorBrush _gold;
    private static SolidColorBrush _goldenrod;
    private static SolidColorBrush _gray;
    private static SolidColorBrush _green;
    private static SolidColorBrush _greenYellow;
    private static SolidColorBrush _honeydew;
    private static SolidColorBrush _hotPink;
    private static SolidColorBrush _indianRed;
    private static SolidColorBrush _indigo;
    private static SolidColorBrush _ivory;
    private static SolidColorBrush _khaki;
    private static SolidColorBrush _lavender;
    private static SolidColorBrush _lavenderBlush;
    private static SolidColorBrush _lawnGreen;
    private static SolidColorBrush _lemonChiffon;
    private static SolidColorBrush _lightBlue;
    private static SolidColorBrush _lightCoral;
    private static SolidColorBrush _lightCyan;
    private static SolidColorBrush _lightGoldenrodYellow;
    private static SolidColorBrush _lightGreen;
    private static SolidColorBrush _lightGray;
    private static SolidColorBrush _lightPink;
    private static SolidColorBrush _lightSalmon;
    private static SolidColorBrush _lightSeaGreen;
    private static SolidColorBrush _lightSkyBlue;
    private static SolidColorBrush _lightSlateGray;
    private static SolidColorBrush _lightSteelBlue;
    private static SolidColorBrush _lightYellow;
    private static SolidColorBrush _lime;
    private static SolidColorBrush _limeGreen;
    private static SolidColorBrush _linen;
    private static SolidColorBrush _magenta;
    private static SolidColorBrush _maroon;
    private static SolidColorBrush _mediumAquamarine;
    private static SolidColorBrush _mediumBlue;
    private static SolidColorBrush _mediumOrchid;
    private static SolidColorBrush _mediumPurple;
    private static SolidColorBrush _mediumSeaGreen;
    private static SolidColorBrush _mediumSlateBlue;
    private static SolidColorBrush _mediumSpringGreen;
    private static SolidColorBrush _mediumTurquoise;
    private static SolidColorBrush _mediumVioletRed;
    private static SolidColorBrush _midnightBlue;
    private static SolidColorBrush _mintCream;
    private static SolidColorBrush _mistyRose;
    private static SolidColorBrush _moccasin;
    private static SolidColorBrush _navajoWhite;
    private static SolidColorBrush _navy;
    private static SolidColorBrush _oldLace;
    private static SolidColorBrush _olive;
    private static SolidColorBrush _oliveDrab;
    private static SolidColorBrush _orange;
    private static SolidColorBrush _orangeRed;
    private static SolidColorBrush _orchid;
    private static SolidColorBrush _paleGoldenrod;
    private static SolidColorBrush _paleGreen;
    private static SolidColorBrush _paleTurquoise;
    private static SolidColorBrush _paleVioletRed;
    private static SolidColorBrush _papayaWhip;
    private static SolidColorBrush _peachPuff;
    private static SolidColorBrush _peru;
    private static SolidColorBrush _pink;
    private static SolidColorBrush _plum;
    private static SolidColorBrush _powderBlue;
    private static SolidColorBrush _purple;
    private static SolidColorBrush _red;
    private static SolidColorBrush _rosyBrown;
    private static SolidColorBrush _royalBlue;
    private static SolidColorBrush _saddleBrown;
    private static SolidColorBrush _salmon;
    private static SolidColorBrush _sandyBrown;
    private static SolidColorBrush _seaGreen;
    private static SolidColorBrush _seaShell;
    private static SolidColorBrush _sienna;
    private static SolidColorBrush _silver;
    private static SolidColorBrush _skyBlue;
    private static SolidColorBrush _slateBlue;
    private static SolidColorBrush _slateGray;
    private static SolidColorBrush _snow;
    private static SolidColorBrush _springGreen;
    private static SolidColorBrush _steelBlue;
    private static SolidColorBrush _tan;
    private static SolidColorBrush _teal;
    private static SolidColorBrush _thistle;
    private static SolidColorBrush _tomato;
    private static SolidColorBrush _transparent;
    private static SolidColorBrush _turquoise;
    private static SolidColorBrush _violet;
    private static SolidColorBrush _wheat;
    private static SolidColorBrush _white;
    private static SolidColorBrush _whiteSmoke;
    private static SolidColorBrush _yellow;
    private static SolidColorBrush _yellowGreen;
}