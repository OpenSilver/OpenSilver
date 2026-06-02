
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

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Represents a converter that converts the dimensions of a <see cref="GroupBox"/> control into a <see cref="CombinedGeometry"/>.
/// </summary>
public class BorderGapClipConverter : IMultiValueConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BorderGapClipConverter"/> class.
    /// </summary>
    public BorderGapClipConverter() { }

    /// <summary>
    /// Creates a <see cref="CombinedGeometry"/> that draws the border for a <see cref="GroupBox"/> control.
    /// </summary>
    /// <param name="values">
    /// An array of three numbers that represent the <see cref="GroupBox"/> control parameters.
    /// </param>
    /// <param name="targetType">
    /// This parameter is not used.
    /// </param>
    /// <param name="parameter">
    /// The width of the visible line to the left of the <see cref="HeaderedContentControl.Header"/> in the <see cref="GroupBox"/>.
    /// </param>
    /// <param name="culture">
    /// This parameter is not used.
    /// </param>
    /// <returns>
    /// A <see cref="CombinedGeometry"/> that draws the border around a <see cref="GroupBox"/> control that includes a gap 
    /// for the <see cref="HeaderedContentControl.Header"/> content.
    /// </returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter == null ||
            values == null ||
            values.Length != 3 ||
            values[0] is not double headerWidth ||
            values[1] is not double borderWidth ||
            values[2] is not double borderHeight)
        {
            return DependencyProperty.UnsetValue;
        }

        if (parameter is not double && parameter is not string)
        {
            return DependencyProperty.UnsetValue;
        }

        if (borderWidth == 0 || borderHeight == 0)
        {
            return null;
        }

        double lineWidth = parameter switch
        {
            string s => double.Parse(s, NumberFormatInfo.InvariantInfo),
            _ => (double)parameter,
        };

        var bounds = new RectangleGeometry(
            new Rect(0, 0, borderWidth, borderHeight));

        var gap = new RectangleGeometry(
            new Rect(
                lineWidth,
                0,
                headerWidth,
                borderHeight / 2.0));

        var clip = new CombinedGeometry(
            GeometryCombineMode.Exclude,
            bounds,
            gap);

        return clip;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="value">
    /// This parameter is not used.
    /// </param>
    /// <param name="targetTypes">
    /// This parameter is not used.
    /// </param>
    /// <param name="parameter">
    /// This parameter is not used.
    /// </param>
    /// <param name="culture">
    /// This parameter is not used.
    /// </param>
    /// <returns>
    /// <see cref="DependencyProperty.UnsetValue"/> in all cases.
    /// </returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [DependencyProperty.UnsetValue];
    }
}
