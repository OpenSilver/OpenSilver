
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
using OpenSilver.Internal.Media;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace System.Windows.Media;

/// <summary>
/// Base class that describes a way to paint a region.
/// </summary>
public abstract class TileBrush : Brush
{
    /// <summary>
    /// Provides initialization for base class values when called by the constructor
    /// of a derived class.
    /// </summary>
    protected TileBrush() { }

    /// <summary>
    /// Identifies the <see cref="AlignmentX"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentXProperty =
        DependencyProperty.Register(
            nameof(AlignmentX),
            typeof(AlignmentX),
            typeof(TileBrush),
            new PropertyMetadata(AlignmentX.Center, OnPropertyChanged),
            ValidateEnums.IsAlignmentXValid);

    /// <summary>
    /// Gets or sets the horizontal alignment of content in the <see cref="TileBrush"/>
    /// base file.
    /// </summary>
    /// <returns>
    /// A value that specifies the horizontal position of <see cref="TileBrush"/> content 
    /// in its base tile. The default value is <see cref="AlignmentX.Center"/>.
    /// </returns>
    public AlignmentX AlignmentX
    {
        get => (AlignmentX)GetValue(AlignmentXProperty);
        set => SetValueInternal(AlignmentXProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="AlignmentY"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentYProperty =
        DependencyProperty.Register(
            nameof(AlignmentY),
            typeof(AlignmentY),
            typeof(TileBrush),
            new PropertyMetadata(AlignmentY.Center, OnPropertyChanged),
            ValidateEnums.IsAlignmentYValid);

    /// <summary>
    /// Gets or sets the vertical alignment of content in the <see cref="TileBrush"/>
    /// base file.
    /// </summary>
    /// <returns>
    /// A value that specifies the vertical position of <see cref="TileBrush"/> content 
    /// in its base tile. The default value is <see cref="AlignmentY.Center"/>.
    /// </returns>
    public AlignmentY AlignmentY
    {
        get => (AlignmentY)GetValue(AlignmentYProperty);
        set => SetValueInternal(AlignmentYProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Stretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(TileBrush),
            new PropertyMetadata(Stretch.Fill, OnPropertyChanged),
            ValidateEnums.IsStretchValid);

    /// <summary>
    /// Gets or sets a value that specifies how the content of this <see cref="TileBrush"/>
    /// stretches to fit its tiles.
    /// </summary>
    /// <returns>
    /// A value that specifies how this <see cref="TileBrush"/> content is projected
    /// onto its base tile. The default value is <see cref="Stretch.Fill"/>.
    /// </returns>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValueInternal(StretchProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ViewboxUnits"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewboxUnitsProperty =
        DependencyProperty.Register(
            nameof(ViewboxUnits),
            typeof(BrushMappingMode),
            typeof(TileBrush),
            new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnPropertyChanged),
            ValidateEnums.IsBrushMappingModeValid);

    /// <summary>
    /// Gets or sets a value that specifies whether the <see cref="Viewbox"/> value is relative 
    /// to the bounding box of the <see cref="TileBrush"/> contents or whether the value is absolute.
    /// </summary>
    /// <returns>
    /// A value that indicates whether the <see cref="Viewbox"/> value is relative to the bounding 
    /// box of the <see cref="TileBrush"/> contents or whether it is an absolute value. The default 
    /// value is <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
    /// </returns>
    public BrushMappingMode ViewboxUnits
    {
        get => (BrushMappingMode)GetValue(ViewboxUnitsProperty);
        set => SetValueInternal(ViewboxUnitsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Viewbox"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewboxProperty =
        DependencyProperty.Register(
            nameof(Viewbox),
            typeof(Rect),
            typeof(TileBrush),
            new PropertyMetadata(new Rect(0, 0, 1, 1), OnPropertyChanged));

    /// <summary>
    /// Gets or sets the position and dimensions of the content in a <see cref="TileBrush"/> 
    /// tile.
    /// </summary>
    /// <returns>
    /// The position and dimensions of the <see cref="TileBrush"/> content. The default value 
    /// is a rectangle (<see cref="Rect"/>) that has a <see cref="Rect.TopLeft"/> of (0,0), and 
    /// a <see cref="Rect.Width"/> and <see cref="Rect.Height"/> of 1.
    /// </returns>
    public Rect Viewbox
    {
        get => (Rect)GetValue(ViewboxProperty);
        set => SetValueInternal(ViewboxProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ViewportUnits"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewportUnitsProperty =
        DependencyProperty.Register(
            nameof(ViewportUnits),
            typeof(BrushMappingMode),
            typeof(TileBrush),
            new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnPropertyChanged));

    /// <summary>
    /// Gets or sets a <see cref="BrushMappingMode"/> enumeration that specifies whether the 
    /// value of the <see cref="Viewport"/>, which indicates the size and position of the 
    /// <see cref="TileBrush"/> base tile, is relative to the size of the output area.
    /// </summary>
    /// <returns>
    /// Indicates whether the value of the <see cref="Viewport"/>, which describes the size and 
    /// position of the <see cref="TileBrush"/> tiles, is relative to the size of the whole output 
    /// area. The default value is <see cref="BrushMappingMode.RelativeToBoundingBox"/>.
    /// </returns>
    public BrushMappingMode ViewportUnits
    {
        get => (BrushMappingMode)GetValue(ViewportUnitsProperty);
        set => SetValueInternal(ViewportUnitsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Viewport"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewportProperty =
        DependencyProperty.Register(
            nameof(Viewport),
            typeof(Rect),
            typeof(TileBrush),
            new PropertyMetadata(new Rect(0, 0, 1, 1), OnPropertyChanged));

    /// <summary>
    /// Gets or sets the position and dimensions of the base tile for a <see cref="TileBrush"/>.
    /// </summary>
    /// <returns>
    /// The position and dimensions of the base tile for a <see cref="TileBrush"/>. The default 
    /// value is a rectangle (<see cref="Rect"/>) with a <see cref="Rect.TopLeft"/> of (0,0) and 
    /// a <see cref="Rect.Width"/> and <see cref="Rect.Height"/> of 1.
    /// </returns>
    public Rect Viewport
    {
        get => (Rect)GetValue(ViewportProperty);
        set => SetValueInternal(ViewportProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TileMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TileModeProperty =
        DependencyProperty.Register(
            nameof(TileMode),
            typeof(TileMode),
            typeof(TileBrush),
            new PropertyMetadata(TileMode.None, OnPropertyChanged),
            ValidateEnums.IsTileModeValid);

    /// <summary>
    /// Gets or sets a value that specifies how a <see cref="TileBrush"/> fills the area that you 
    /// are painting if the base tile is smaller than the output area.
    /// </summary>
    /// <returns>
    /// A value that specifies how the <see cref="TileBrush"/> tiles fill the output area when the 
    /// base tile, which is specified by the <see cref="Viewport"/> property, is smaller than the 
    /// output area. The default value is <see cref="TileMode.None"/>.
    /// </returns>
    public TileMode TileMode
    {
        get => (TileMode)GetValue(TileModeProperty);
        set => SetValueInternal(TileModeProperty, value);
    }

    internal virtual ImageSource GetImageSource() => null;

    internal async sealed override ValueTask<string> GetDataStringAsync(UIElement parent)
    {
        if (GetImageSource() is ImageSource source)
        {
            string url = await source.GetDataStringAsync(parent);
            if (!string.IsNullOrEmpty(url))
            {
                Rect viewport = Viewport;
                BrushMappingMode viewportUnits = ViewportUnits;

                string position;
                string size;
                string opacity = (1.0 - Opacity).ToInvariantString();
                string repeat = ConvertTileMode(TileMode);

                if (viewportUnits == BrushMappingMode.Absolute)
                {
                    position = $"{viewport.X.ToInvariantString()}px {viewport.Y.ToInvariantString()}px";
                    size = $"{viewport.Width.ToInvariantString()}px {viewport.Height.ToInvariantString()}px";
                }
                else if (IsDefaultViewport(viewport))
                {
                    position = $"{ConvertAlignmentX(AlignmentX)} {ConvertAlignmentY(AlignmentY)}";
                    size = ConvertStretch(Stretch);
                }
                else
                {
                    string x = viewport.Width == 1.0 ? "0%" : $"{(viewport.X / (1.0 - viewport.Width) * 100.0).ToInvariantString()}%";
                    string y = viewport.Height == 1.0 ? "0%" : $"{(viewport.Y / (1.0 - viewport.Height) * 100.0).ToInvariantString()}%";
                    position = $"{x} {y}";
                    size = $"{(viewport.Width * 100.0).ToInvariantString()}% {(viewport.Height * 100.0).ToInvariantString()}%";
                }

                return $"linear-gradient(to right, rgba(255, 255, 255, {opacity}) 0 100%), url({url}) {position} / {size} {repeat}";
            }
        }

        return string.Empty;
    }

    private static bool IsDefaultViewport(Rect vp) => vp.X == 0.0 && vp.Y == 0.0 && vp.Width == 1.0 && vp.Height == 1.0;

    private string ConvertTileMode(TileMode tileMode)
        => tileMode switch
        {
            TileMode.None => "no-repeat",
            _ => "repeat",
        };

    private static string ConvertAlignmentX(AlignmentX alignmentX)
        => alignmentX switch
        {
            AlignmentX.Left => "left",
            AlignmentX.Right => "right",
            _ => "center",
        };

    private static string ConvertAlignmentY(AlignmentY alignmentY)
        => alignmentY switch
        {
            AlignmentY.Bottom => "bottom",
            AlignmentY.Top => "top",
            _ => "center",
        };

    private static string ConvertStretch(Stretch stretch)
        => stretch switch
        {
            Stretch.None => "auto",
            Stretch.Uniform => "contain",
            Stretch.UniformToFill => "cover",
            _ => "100% 100%",
        };

    internal sealed override ISvgBrush GetSvgElement(Shape shape) => new SvgPattern(shape, this);

    private sealed class SvgPattern : ISvgBrush
    {
        private readonly TileBrush _tileBrush;
        private readonly HtmlElementReference _pattern;
        private readonly HtmlElementReference _crop;
        private readonly HtmlElementReference _image;
        private readonly WeakEventToken _weakTransformChangedEventToken;
        private readonly WeakEventToken _weakSizeChangedEventToken;

        public SvgPattern(Shape shape, TileBrush tileBrush)
        {
            _tileBrush = tileBrush;
            _pattern = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(shape.DefsElement, "pattern");
            _crop = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(_pattern, "svg");
            _image = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(_crop, "image");

            // <image> is always rendered at its natural pixel size; the crop <svg>
            // does all Viewbox/Viewport/Stretch/Alignment work via its viewBox + preserveAspectRatio,
            // and the JS helper writes the per-property layout.
            _image.SetAttribute("preserveAspectRatio", "none");

            DrawPattern(shape);

            _weakTransformChangedEventToken = WeakEvent.Subscribe<SvgPattern, Brush, EventArgs>(
                this,
                tileBrush,
                static (instance, sender, args) => instance.OnTransformChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));

            _weakSizeChangedEventToken = WeakEvent.Subscribe<SvgPattern, Shape, SizeChangedEventArgs>(
                this,
                shape,
                static (instance, sender, args) => instance.OnRenderSizeChanged(sender, args),
                static (handler, source) => source.SizeChanged -= new SizeChangedEventHandler(handler),
                static (handler, source) => source.SizeChanged += new SizeChangedEventHandler(handler));
        }

        public string GetBrush(Shape shape) => $"url(#{_pattern.Uid})";

        public void DestroyBrush(Shape shape)
        {
            _weakTransformChangedEventToken.Dispose();
            _weakSizeChangedEventToken.Dispose();
            INTERNAL_HtmlDomManager.RemoveNodeNative(_pattern);
        }

        public void RenderBrush(Shape shape) => DrawPattern(shape);

        private void OnTransformChanged(object sender, EventArgs e)
        {
            Transform transform = ((Brush)sender).Transform;

            if (transform is null || Transform.IsIdentityTransform(transform))
            {
                _pattern.RemoveAttribute("patternTransform");
            }
            else
            {
                _pattern.SetAttribute("patternTransform", MatrixTransform.MatrixToHtmlString(transform.Matrix));
            }
        }

        private void OnRenderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // The layout can depend on the shape's bounding box (for Stretch.None and
            // for ViewportUnits.Absolute), so always re-run the JS helper on size changes.
            UpdateLayout((Shape)sender);
        }

        private void DrawPattern(Shape shape)
        {
            if (_tileBrush.GetImageSource() is ImageSource imageSource)
            {
                ValueTask<string> vTask = imageSource.GetDataStringAsync(shape);
                if (!vTask.IsCompletedSuccessfully)
                {
                    return;
                }

                _image.SetAttribute("href", vTask.Result);
            }
            else
            {
                _image.RemoveAttribute("href");
            }

            if (_tileBrush.Transform is Transform t && !Transform.IsIdentityTransform(t))
            {
                _pattern.SetAttribute("patternTransform", MatrixTransform.MatrixToHtmlString(t.Matrix));
            }
            else
            {
                _pattern.RemoveAttribute("patternTransform");
            }

            _image.SetCssStyleProperty(CssPropertyNames.Opacity, Math.Round(_tileBrush.Opacity, 2).ToInvariantString());

            UpdateLayout(shape);
        }

        private void UpdateLayout(Shape shape)
        {
            Rect viewport = _tileBrush.Viewport;
            Rect viewbox = _tileBrush.Viewbox;

            string mode = ((int)_tileBrush.TileMode).ToInvariantString();
            string stretch = ((int)_tileBrush.Stretch).ToInvariantString();
            string aX = ((int)_tileBrush.AlignmentX).ToInvariantString();
            string aY = ((int)_tileBrush.AlignmentY).ToInvariantString();

            string vpU = ((int)_tileBrush.ViewportUnits).ToInvariantString();
            string vpX = viewport.X.ToInvariantString();
            string vpY = viewport.Y.ToInvariantString();
            string vpW = viewport.Width.ToInvariantString();
            string vpH = viewport.Height.ToInvariantString();

            string vbU = ((int)_tileBrush.ViewboxUnits).ToInvariantString();
            string vbX = viewbox.X.ToInvariantString();
            string vbY = viewbox.Y.ToInvariantString();
            string vbW = viewbox.Width.ToInvariantString();
            string vbH = viewbox.Height.ToInvariantString();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.drawSvgPattern('{_pattern.Uid}','{_crop.Uid}','{_image.Uid}','{shape.SvgElement.Uid}',{mode},{vpU},{vpX},{vpY},{vpW},{vpH},{vbU},{vbX},{vbY},{vbW},{vbH},{stretch},{aX},{aY})");
        }
    }
}
