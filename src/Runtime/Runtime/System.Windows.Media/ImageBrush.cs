
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
using OpenSilver.Internal;

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
}
