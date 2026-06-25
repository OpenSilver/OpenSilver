
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
    /// Initializes a new instance of the <see cref="ImageBrush"/> class that paints an 
    /// area with the specified image.
    /// </summary>
    /// <param name="image">
    /// The image to display.
    /// </param>
    public ImageBrush(ImageSource image)
    {
        ImageSource = image;
    }

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
    [OpenSilver.NotImplemented]
    public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

    /// <summary>
    /// Occurs when the image source is downloaded and decoded with no failure. You can
    /// use this event to determine the size of an image before rendering it.
    /// </summary>
    [OpenSilver.NotImplemented]
    public event EventHandler<RoutedEventArgs> ImageOpened;

    internal async override ValueTask<string> GetDataStringAsync(UIElement parent)
    {
        ImageSource source = ImageSource;
        if (source != null)
        {
            string url = await source.GetDataStringAsync(parent);
            if (!string.IsNullOrEmpty(url))
            {
                string opacity = (1.0 - Opacity).ToInvariantString();
                string positionX = ConvertAlignmentX(AlignmentX);
                string positionY = ConvertAlignmentX(AlignmentY);
                string stretch = ConvertStretch(Stretch);
                return $"linear-gradient(to right, rgba(255, 255, 255, {opacity}) 0 100%), url({url}) {positionX} {positionY} / {stretch} no-repeat";
            }
        }

        return string.Empty;
    }

    internal override ISvgBrush GetSvgElement(Shape shape) => new SvgPattern(shape, this);

    private static string ConvertAlignmentX(AlignmentX alignmentX)
        => alignmentX switch
        {
            AlignmentX.Left => "left",
            AlignmentX.Right => "right",
            _ => "center",
        };

    private static string ConvertAlignmentX(AlignmentY alignmentY)
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
        private readonly HtmlElementReference _image;
        private readonly WeakEventToken _weakTransformChangedEventToken;
        private readonly WeakEventToken _weakSizeChangedEventToken;

        public SvgPattern(Shape shape, ImageBrush imageBrush)
        {
            _imageBrush = imageBrush;
            _pattern = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(shape.DefsElement, "pattern");
            _image = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(_pattern, "image");
            _pattern.SetAttribute("x", "0");
            _pattern.SetAttribute("y", "0");
            _pattern.SetAttribute("width", "100%");
            _pattern.SetAttribute("height", "100%");

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
            if (_imageBrush.Stretch == Stretch.None)
            {
                SetPreserveAspectRatio((Shape)sender);
            }
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

            _image.SetCssStyleProperty(CssPropertyNames.Opacity, Math.Round(_imageBrush.Opacity, 2).ToInvariantString());

            SetPreserveAspectRatio(shape);
        }

        private void SetPreserveAspectRatio(Shape shape)
        {
            Stretch stretch = _imageBrush.Stretch;

            string alignX = _imageBrush.AlignmentX switch
            {
                AlignmentX.Left => "xMin",
                AlignmentX.Center => "xMid",
                AlignmentX.Right => "xMax",
                _ => string.Empty
            };

            string alignY = _imageBrush.AlignmentY switch
            {
                AlignmentY.Top => "YMin",
                AlignmentY.Center => "YMid",
                AlignmentY.Bottom => "YMax",
                _ => string.Empty
            };

            string preserveAspectRatio = stretch switch
            {
                Stretch.None => $"{alignX}{alignY}",
                Stretch.Fill => "none",
                Stretch.Uniform => $"{alignX}{alignY} meet",
                Stretch.UniformToFill => $"{alignX}{alignY} slice",
                _ => string.Empty
            };

            if (stretch == Stretch.UniformToFill)
            {
                _pattern.SetAttribute("preserveAspectRatio", preserveAspectRatio);
            }

            _image.SetAttribute("preserveAspectRatio", preserveAspectRatio);

            if (stretch == Stretch.None)
            {
                SetNaturalSize(shape);
            }
            else
            {
                _image.SetAttribute("width", "100%");
                _image.SetAttribute("height", "100%");
                _pattern.RemoveAttribute("viewBox");
            }
        }

        private void SetNaturalSize(Shape shape)
        {
            string shapeId = shape.OuterDiv.Uid;
            string patternId = _pattern.Uid;
            string imageId = _image.Uid;

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.setSvgPatternNaturalSize('{patternId}', '{imageId}', '{shapeId}', {(int)_imageBrush.AlignmentX}, {(int)_imageBrush.AlignmentY})");
        }
    }
}
