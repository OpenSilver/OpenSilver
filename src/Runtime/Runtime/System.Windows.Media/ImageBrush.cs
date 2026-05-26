
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
/// Paints an area with an image.
/// </summary>
public sealed class ImageBrush : TileBrush
{
    private WeakEventToken _weakEventToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageBrush"/> class.
    /// </summary>
    public ImageBrush() { }

    /// <summary>
    /// Identifies the <see cref="ImageSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageSourceProperty =
    DependencyProperty.Register(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ImageBrush),
        new PropertyMetadata(null, OnImageSourceChanged));

    /// <summary>
    /// Gets or sets the image displayed by this <see cref="ImageBrush"/>.
    /// </summary>
    /// <returns>
    /// The image displayed by this <see cref="ImageBrush"/>.
    /// </returns>
    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValueInternal(ImageSourceProperty, value);
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ImageBrush ib = (ImageBrush)d;

        if (ib._weakEventToken != null)
        {
            ib._weakEventToken.Dispose();
            ib._weakEventToken = null;
        }

        if (e.NewValue is ImageSource source)
        {
            ib._weakEventToken = WeakEvent.Subscribe<ImageBrush, ImageSource, EventArgs>(
                ib,
                source,
                static (instance, sender, args) => instance.OnSourceChanged(sender, args),
                static (handler, source) => source.Changed -= new EventHandler(handler),
                static (handler, source) => source.Changed += new EventHandler(handler));
        }

        ib.RaiseChanged();
    }

    private void OnSourceChanged(object sender, EventArgs e) => RaiseChanged();

    /// <summary>
    /// Occurs when there is an error associated with image retrieval or format.
    /// </summary>
    [NotImplemented]
    public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

    /// <summary>
    /// Occurs when the image source is downloaded and decoded with no failure. You can
    /// use this event to determine the size of an image before rendering it.
    /// </summary>
    [NotImplemented]
    public event EventHandler<RoutedEventArgs> ImageOpened;

    internal async override ValueTask<string> GetDataStringAsync(UIElement parent)
    {
        if (ImageSource is ImageSource source)
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

    internal override ISvgBrush GetSvgElement(Shape shape) => new SvgPattern(shape, this);

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

    private sealed class SvgPattern : ISvgBrush
    {
        private readonly ImageBrush _imageBrush;
        private readonly HtmlElementReference _pattern;
        private readonly HtmlElementReference _crop;
        private readonly HtmlElementReference _image;
        private readonly WeakEventToken _weakTransformChangedEventToken;
        private readonly WeakEventToken _weakSizeChangedEventToken;

        public SvgPattern(Shape shape, ImageBrush imageBrush)
        {
            _imageBrush = imageBrush;
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
                imageBrush,
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
            if (_imageBrush.ImageSource is ImageSource imageSource)
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

            if (_imageBrush.Transform is Transform t && !Transform.IsIdentityTransform(t))
            {
                _pattern.SetAttribute("patternTransform", MatrixTransform.MatrixToHtmlString(t.Matrix));
            }
            else
            {
                _pattern.RemoveAttribute("patternTransform");
            }

            _image.SetCssStyleProperty(CssPropertyNames.Opacity, Math.Round(_imageBrush.Opacity, 2).ToInvariantString());

            UpdateLayout(shape);
        }

        private void UpdateLayout(Shape shape)
        {
            Rect viewport = _imageBrush.Viewport;
            Rect viewbox = _imageBrush.Viewbox;

            string mode = ((int)_imageBrush.TileMode).ToInvariantString();
            string stretch = ((int)_imageBrush.Stretch).ToInvariantString();
            string aX = ((int)_imageBrush.AlignmentX).ToInvariantString();
            string aY = ((int)_imageBrush.AlignmentY).ToInvariantString();

            string vpU = ((int)_imageBrush.ViewportUnits).ToInvariantString();
            string vpX = viewport.X.ToInvariantString();
            string vpY = viewport.Y.ToInvariantString();
            string vpW = viewport.Width.ToInvariantString();
            string vpH = viewport.Height.ToInvariantString();

            string vbU = ((int)_imageBrush.ViewboxUnits).ToInvariantString();
            string vbX = viewbox.X.ToInvariantString();
            string vbY = viewbox.Y.ToInvariantString();
            string vbW = viewbox.Width.ToInvariantString();
            string vbH = viewbox.Height.ToInvariantString();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.drawSvgPattern('{_pattern.Uid}','{_crop.Uid}','{_image.Uid}','{shape.SvgElement.Uid}',{mode},{vpU},{vpX},{vpY},{vpW},{vpH},{vbU},{vbX},{vbY},{vbW},{vbH},{stretch},{aX},{aY})");
        }
    }
}
