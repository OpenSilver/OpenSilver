
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

using OpenSilver.Internal;

namespace System.Windows.Media;

/// <summary>
/// Describes how a shape is outlined.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class Pen : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pen"/> class.
    /// </summary>
    public Pen() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Pen"/> class with the specified <see cref="Media.Brush"/> 
    /// and thickness.
    /// </summary>
    /// <param name="brush">
    /// The Brush for this Pen.
    /// </param>
    /// <param name="thickness">
    /// The thickness of the Pen.
    /// </param>
    public Pen(Brush brush, double thickness)
    {
        Brush = brush;
        Thickness = thickness;
    }

    /// <summary>
    /// Identifies the <see cref="MiterLimit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MiterLimitProperty =
        DependencyProperty.Register(
            nameof(MiterLimit),
            typeof(double),
            typeof(Pen),
            new PropertyMetadata(10.0));

    /// <summary>
    /// Gets or sets the limit on the ratio of the miter length to half this pen's <see cref="Thickness"/>.
    /// </summary>
    /// <returns>
    /// The limit on the ratio of the miter length to half the pen's <see cref="Thickness"/>. This value 
    /// is always a positive number greater than or equal to 1. The default value is 10.0.
    /// </returns>
    public double MiterLimit
    {
        get => (double)GetValue(MiterLimitProperty);
        set => SetValueInternal(MiterLimitProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LineJoin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LineJoinProperty =
        DependencyProperty.Register(
            nameof(LineJoin),
            typeof(PenLineJoin),
            typeof(Pen),
            new PropertyMetadata(PenLineJoin.Miter),
            ValidateEnums.IsPenLineJoinValid);

    /// <summary>
    /// Gets or sets the type of joint used at the vertices of a shape's outline.
    /// </summary>
    /// <returns>
    /// The type of joint used at the vertices of a shape's outline. The default value is 
    /// <see cref="PenLineJoin.Miter"/>.
    /// </returns>
    public PenLineJoin LineJoin
    {
        get => (PenLineJoin)GetValue(LineJoinProperty);
        set => SetValueInternal(LineJoinProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StartLineCap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartLineCapProperty =
        DependencyProperty.Register(
            nameof(StartLineCap),
            typeof(PenLineCap),
            typeof(Pen),
            new PropertyMetadata(PenLineCap.Flat),
            ValidateEnums.IsPenLineCapValid);

    /// <summary>
    /// Gets or sets the type of shape to use at the beginning of a stroke.
    /// </summary>
    /// <returns>
    /// The type of shape that starts the stroke. The default value is <see cref="PenLineCap.Flat"/>.
    /// </returns>
    public PenLineCap StartLineCap
    {
        get => (PenLineCap)GetValue(StartLineCapProperty);
        set => SetValueInternal(StartLineCapProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="EndLineCap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EndLineCapProperty =
        DependencyProperty.Register(
            nameof(EndLineCap),
            typeof(PenLineCap),
            typeof(Pen),
            new PropertyMetadata(PenLineCap.Flat),
            ValidateEnums.IsPenLineCapValid);

    /// <summary>
    /// Gets or sets the type of shape to use at the end of a stroke.
    /// </summary>
    /// <returns>
    /// The type of shape that ends the stroke. The default value is <see cref="PenLineCap.Flat"/>.
    /// </returns>
    public PenLineCap EndLineCap
    {
        get => (PenLineCap)GetValue(EndLineCapProperty);
        set => SetValueInternal(EndLineCapProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="DashStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DashStyleProperty =
        DependencyProperty.Register(
            nameof(DashStyle),
            typeof(DashStyle),
            typeof(Pen),
            new PropertyMetadata(DashStyles.Solid));

    /// <summary>
    /// Gets or sets a value that describes the pattern of dashes generated by this <see cref="Pen"/>.
    /// </summary>
    /// <returns>
    /// A value that describes the pattern of dashes generated by this <see cref="Pen"/>. The default 
    /// is <see cref="DashStyles.Solid"/>, which indicates that there should be no dashes.
    /// </returns>
    public DashStyle DashStyle
    {
        get => (DashStyle)GetValue(DashStyleProperty);
        set => SetValueInternal(DashStyleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Thickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register(
            nameof(Thickness),
            typeof(double),
            typeof(Pen),
            new PropertyMetadata(1.0));

    /// <summary>
    /// Gets or sets the thickness of the stroke produced by this <see cref="Pen"/>.
    /// </summary>
    /// <returns>
    /// The thickness of the stroke produced by this <see cref="Pen"/>. Default is 1.
    /// </returns>
    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValueInternal(ThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Brush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BrushProperty =
        DependencyProperty.Register(
            nameof(Brush),
            typeof(Brush),
            typeof(Pen),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the fill the outline produced by this <see cref="Pen"/>.
    /// </summary>
    /// <returns>
    /// The fill of the outline produced by this <see cref="Pen"/>. The default value is null.
    /// </returns>
    public Brush Brush
    {
        get => (Brush)GetValue(BrushProperty);
        set => SetValueInternal(BrushProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="DashCap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DashCapProperty =
        DependencyProperty.Register(
            nameof(DashCap),
            typeof(PenLineCap),
            typeof(Pen),
            new PropertyMetadata(PenLineCap.Square),
            ValidateEnums.IsPenLineCapValid);

    /// <summary>
    /// Gets or sets a value that specifies how the ends of each dash are drawn.
    /// </summary>
    /// <returns>
    /// Specifies how the ends of each dash are drawn. This setting applies to both ends of each dash.
    /// The default value is <see cref="PenLineCap.Square"/>.
    /// </returns>
    public PenLineCap DashCap
    {
        get => (PenLineCap)GetValue(DashCapProperty);
        set => SetValueInternal(DashCapProperty, value);
    }
}
