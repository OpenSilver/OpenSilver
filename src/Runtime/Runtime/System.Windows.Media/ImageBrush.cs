
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

using System.Threading.Tasks;
using System.Windows.Shapes;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;

namespace System.Windows.Media;

/// <summary>
/// Paints an area with an image.
/// </summary>
public sealed class ImageBrush : TileBrush
{
    private WeakEventListener<ImageBrush, ImageSource, EventArgs> _sourceChangedListener;

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

        if (ib._sourceChangedListener != null)
        {
            ib._sourceChangedListener.Detach();
            ib._sourceChangedListener = null;
        }

        if (e.NewValue is ImageSource source)
        {
            ib._sourceChangedListener = new(ib, source)
            {
                OnEventAction = static (instance, sender, args) => instance.OnSourceChanged(sender, args),
                OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
            };
            source.Changed += ib._sourceChangedListener.OnEvent;
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
        private readonly INTERNAL_HtmlDomElementReference _pattern;
        private readonly INTERNAL_HtmlDomElementReference _image;
        private readonly WeakEventListener<SvgPattern, Brush, EventArgs> _transformChangedListener;
        private readonly WeakEventListener<SvgPattern, Shape, SizeChangedEventArgs> _sizeChangedListener;

        public SvgPattern(Shape shape, ImageBrush imageBrush)
        {
            _imageBrush = imageBrush;
            _pattern = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(shape.DefsElement, "pattern");
            _image = INTERNAL_HtmlDomManager.CreateSvgElementAndAppendIt(_pattern, "image");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "x", "0");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "y", "0");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "width", "100%");
            INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "height", "100%");

            DrawPattern(shape);

            _transformChangedListener = new(this, imageBrush)
            {
                OnEventAction = static (instance, sender, args) => instance.OnTransformChanged(sender, args),
                OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
            };
            imageBrush.TransformChanged += _transformChangedListener.OnEvent;

            _sizeChangedListener = new(this, shape)
            {
                OnEventAction = static (instance, sender, args) => instance.OnRenderSizeChanged(sender, args),
                OnDetachAction = static (listener, source) => source.SizeChanged -= listener.OnEvent,
            };
            shape.SizeChanged += _sizeChangedListener.OnEvent;
        }

        public string GetBrush(Shape shape) => $"url(#{_pattern.UniqueIdentifier})";

        public void DestroyBrush(Shape shape)
        {
            _transformChangedListener.Detach();
            _sizeChangedListener.Detach();
            INTERNAL_HtmlDomManager.RemoveNodeNative(_pattern);
        }

        public void RenderBrush(Shape shape) => DrawPattern(shape);

        private void OnTransformChanged(object sender, EventArgs e)
        {
            Transform transform = ((Brush)sender).Transform;

            if (transform is null || transform.IsIdentity)
            {
                INTERNAL_HtmlDomManager.RemoveAttribute(_pattern, "patternTransform");
            }
            else
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern,
                    "patternTransform",
                    MatrixTransform.MatrixToHtmlString(transform.Matrix));
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

                INTERNAL_HtmlDomManager.SetDomElementAttribute(_image, "href", vTask.Result);
            }
            else
            {
                INTERNAL_HtmlDomManager.RemoveAttribute(_image, "href");
            }

            if (_imageBrush.Transform is Transform t && !t.IsIdentity)
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "patternTransform", MatrixTransform.MatrixToHtmlString(t.Matrix));
            }

            _image.Style.opacity = Math.Round(_imageBrush.Opacity, 2).ToInvariantString();

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
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_pattern, "preserveAspectRatio", preserveAspectRatio);
            }

            INTERNAL_HtmlDomManager.SetDomElementAttribute(_image, "preserveAspectRatio", preserveAspectRatio);

            if (stretch == Stretch.None)
            {
                SetNaturalSize(shape);
            }
            else
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_image, "width", "100%");
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_image, "height", "100%");
                INTERNAL_HtmlDomManager.RemoveAttribute(_pattern, "viewBox");
            }
        }

        private void SetNaturalSize(Shape shape)
        {
            string shapeId = shape.OuterDiv.UniqueIdentifier;
            string patternId = _pattern.UniqueIdentifier;
            string imageId = _image.UniqueIdentifier;

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.setSvgPatternNaturalSize('{patternId}', '{imageId}', '{shapeId}', {(int)_imageBrush.AlignmentX}, {(int)_imageBrush.AlignmentY})");
        }
    }
}
