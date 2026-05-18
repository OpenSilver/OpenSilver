
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

using CSHTML5.Internal;
using OpenSilver;
using OpenSilver.Internal;
using System.Windows.Markup;

namespace System.Windows.Documents;

/// <summary>
/// A block-level flow content element that provides facilities for presenting content in an ordered 
/// or unordered list.
/// </summary>
[ContentProperty(nameof(ListItems))]
public class List : Block
{
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="List"/> class.
    /// </summary>
    public List()
    {
        ListItems = new ListItemCollection(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="List"/> class, taking a specified <see cref="ListItem"/> 
    /// object as the initial contents of the new <see cref="List"/>.
    /// </summary>
    /// <param name="listItem">
    /// A <see cref="ListItem"/> object specifying the initial contents of the new <see cref="List"/>.
    /// </param>
    public List(ListItem listItem)
        : this()
    {
        ArgumentNullException.ThrowIfNull(listItem);
        ListItems.Add(listItem);
    }

    /// <summary>
    /// Gets a <see cref="ListItemCollection"/> containing the <see cref="ListItem"/> elements that 
    /// comprise the contents of the <see cref="List"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ListItemCollection"/> containing the <see cref="ListItem"/> elements that comprise 
    /// the contents of the <see cref="List"/>. This property has no default value.
    /// </returns>
    public ListItemCollection ListItems { get; }

    /// <summary>
    /// Identifies the <see cref="MarkerOffset"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MarkerOffsetProperty =
        DependencyProperty.Register(
            nameof(MarkerOffset),
            typeof(double),
            typeof(List),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((List)d).OuterDiv.SetCssStyleProperty(
                    "--marker-offset",
                    (double)newValue switch
                    {
                        double.NaN => string.Empty,
                        double offset => $"{offset.ToInvariantString()}px",
                    }),
            },
            IsValidMarkerOffset);

    /// <summary>
    /// Gets or sets the desired distance between the contents of each <see cref="ListItem"/> element, and 
    /// the near edge of the list marker.
    /// </summary>
    /// <returns>
    /// A double value specifying the desired distance between list content and the near edge of list markers,
    /// in device independent pixels. A value of <see cref="double.NaN"/> (equivalent to an attribute value of 
    /// "Auto") causes the marker offset to be determined automatically. The default value is <see cref="double.NaN"/>.
    /// </returns>
    public double MarkerOffset
    {
        get => (double)GetValue(MarkerOffsetProperty);
        set => SetValueInternal(MarkerOffsetProperty, value);
    }

    private static bool IsValidMarkerOffset(object o)
    {
        const double maxOffset = 1000000;

        double value = (double)o;

        if (double.IsNaN(value))
        {
            return true;
        }

        return value >= -maxOffset && value <= maxOffset;
    }

    /// <summary>
    /// Identifies the <see cref="MarkerStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MarkerStyleProperty =
        DependencyProperty.Register(
            nameof(MarkerStyle),
            typeof(TextMarkerStyle),
            typeof(List),
            new FrameworkPropertyMetadata(TextMarkerStyle.Disc, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((List)d).OuterDiv.SetCssStyleProperty(
                    CssPropertyNames.ListStyleType,
                    (TextMarkerStyle)newValue switch
                    {
                        TextMarkerStyle.Disc => "disc",
                        TextMarkerStyle.Circle => "circle",
                        TextMarkerStyle.Square => "square",
                        TextMarkerStyle.Box => "square",
                        TextMarkerStyle.LowerRoman => "lower-roman",
                        TextMarkerStyle.UpperRoman => "upper-roman",
                        TextMarkerStyle.LowerLatin => "lower-latin",
                        TextMarkerStyle.UpperLatin => "upper-latin",
                        TextMarkerStyle.Decimal => "decimal",
                        _ => "none",
                    }),
            },
            IsValidMarkerStyle);

    /// <summary>
    /// Gets or sets the marker style for the <see cref="List"/>.
    /// </summary>
    /// <returns>
    /// A member of the <see cref="TextMarkerStyle"/> enumeration specifying the marker style to use. The default 
    /// value is <see cref="TextMarkerStyle.Disc"/>.
    /// </returns>
    public TextMarkerStyle MarkerStyle
    {
        get => (TextMarkerStyle)GetValue(MarkerStyleProperty);
        set => SetValueInternal(MarkerStyleProperty, value);
    }

    private static bool IsValidMarkerStyle(object o)
    {
        var value = (TextMarkerStyle)o;
        return value == TextMarkerStyle.None
            || value == TextMarkerStyle.Disc
            || value == TextMarkerStyle.Circle
            || value == TextMarkerStyle.Square
            || value == TextMarkerStyle.Box
            || value == TextMarkerStyle.LowerRoman
            || value == TextMarkerStyle.UpperRoman
            || value == TextMarkerStyle.LowerLatin
            || value == TextMarkerStyle.UpperLatin
            || value == TextMarkerStyle.Decimal;
    }

    /// <summary>
    /// Identifies the <see cref="StartIndex"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartIndexProperty =
        DependencyProperty.Register(
            nameof(StartIndex),
            typeof(int),
            typeof(List),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((List)d).OuterDiv.SetAttribute("start", (int)newValue),
            },
            IsValidStartIndex);

    /// <summary>
    /// Gets or sets the starting index for labeling the items in an ordered list.
    /// </summary>
    /// <returns>
    /// The starting index for labeling items in an ordered list. The default value is 1.
    /// </returns>
    public int StartIndex
    {
        get => (int)GetValue(StartIndexProperty);
        set => SetValueInternal(StartIndexProperty, value);
    }

    private static bool IsValidStartIndex(object o) => (int)o > 0;

    /// <inheritdoc />
    protected sealed override int VisualChildrenCount => ListItems.InternalCount;

    /// <inheritdoc />
    protected sealed override UIElement GetVisualChild(int index)
    {
        if (index >= VisualChildrenCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return ListItems.InternalItems[index];
    }

    internal override bool IsModel
    {
        get => ListItems.IsModel;
        set => ListItems.IsModel = value;
    }

    internal override string TagName => "ol";

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return INTERNAL_HtmlDomManager.CreateListDomElementAndAppendIt(parent, this);
    }

    protected internal sealed override void AttachVisualChildrenInternal()
    {
        foreach (var listItem in ListItems.InternalItems)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(listItem, this);
        }
    }
}
