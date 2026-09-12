
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
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Displays a bitmap that can be written to and updated in place.
/// </summary>
/// <remarks>
/// Use <see cref="WriteableImage"/> when pixels change often, for example every frame. Modify the pixel 
/// buffer, then call <see cref="Invalidate"/> to present it.
/// </remarks>
/// <example>
/// <code lang="C#">
/// var image = new WriteableImage(256, 256);
/// image.Pixels[0] = unchecked((int)0xFF0000FF);
/// image.Invalidate();
/// </code>
/// </example>
public sealed class WriteableImage : FrameworkElement
{
    private int[] _pixels;
    private int _pixelWidth;
    private int _pixelHeight;
    private bool _isMeasureDirty;
    private HtmlElementReference _canvas;

    static WriteableImage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WriteableImage), new PropertyMetadata(typeof(WriteableImage)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteableImage"/> class.
    /// </summary>
    public WriteableImage()
    {
        _pixels = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteableImage"/> class using the provided dimensions.
    /// </summary>
    /// <param name="pixelWidth">
    /// The width of the image.
    /// </param>
    /// <param name="pixelHeight">
    /// The height of the image.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pixelWidth" /> or <paramref name="pixelHeight" /> is negative.
    /// </exception>
    public WriteableImage(int pixelWidth, int pixelHeight)
        : this()
    {
        Resize(pixelWidth, pixelHeight);
    }

    internal sealed override bool EnablePointerEventsCore => true;

    /// <summary>
    /// Gets a buffer representing the image pixels in the RGBA format.
    /// </summary>
    /// <returns>
    /// An array of <see cref="int"/> values, one per pixel, in row-major order.
    /// </returns>
    public int[] Pixels => _pixels;

    private static readonly DependencyPropertyKey PixelWidthPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(PixelWidth),
            typeof(int),
            typeof(WriteableImage),
            new PropertyMetadata(0, OnPixelWidthChanged));

    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PixelWidthProperty = PixelWidthPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the width of the image in pixels.
    /// </summary>
    /// <returns>
    /// The width of the image in pixels.
    /// </returns>
    public int PixelWidth => _pixelWidth;

    private static void OnPixelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WriteableImage)d)._pixelWidth = (int)e.NewValue;
    }

    private static readonly DependencyPropertyKey PixelHeightPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(PixelHeight),
            typeof(int),
            typeof(WriteableImage),
            new PropertyMetadata(0, OnPixelHeightChanged));

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PixelHeightProperty = PixelHeightPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the height of the image in pixels.
    /// </summary>
    /// <returns>
    /// The height of the image in pixels.
    /// </returns>
    public int PixelHeight => _pixelHeight;

    private static void OnPixelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WriteableImage)d)._pixelHeight = (int)e.NewValue;
    }

    /// <summary>
    /// Identifies the <see cref="Stretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchProperty = Viewbox.StretchProperty.AddOwner(typeof(WriteableImage));

    /// <summary>
    /// Gets or sets a value that describes how a <see cref="WriteableImage"/> should be stretched to 
    /// fill the destination rectangle.
    /// </summary>
    /// <returns>
    /// A value of the <see cref="Media.Stretch"/> enumeration that specifies how the source image is 
    /// applied if the <see cref="FrameworkElement.Height"/> and <see cref="FrameworkElement.Width"/> 
    /// of the <see cref="WriteableImage"/> are specified and are different than the source image's 
    /// height and width. The default value is <see cref="Stretch.Uniform"/>.
    /// </returns>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValueInternal(StretchProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StretchDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchDirectionProperty = Viewbox.StretchDirectionProperty.AddOwner(typeof(WriteableImage));

    /// <summary>
    /// Gets or sets a value that indicates how the image is scaled.
    /// </summary>
    /// <returns>
    /// One of the <see cref="Controls.StretchDirection"/> values. The default is <see cref="StretchDirection.Both"/>.
    /// </returns>
    public StretchDirection StretchDirection
    {
        get => (StretchDirection)GetValue(StretchDirectionProperty);
        set => SetValueInternal(StretchDirectionProperty, value);
    }

    /// <summary>
    /// Requests a draw or redraw of the entire image.
    /// </summary>
    public void Invalidate()
    {
        if (!_canvas.IsConnected)
        {
            return;
        }

        if (_isMeasureDirty)
        {
            _isMeasureDirty = false;
            InvalidateMeasure();
        }

        TransferBytes();
    }

    /// <summary>
    /// Replaces the pixel buffer with a new one of the specified size.
    /// </summary>
    /// <param name="pixelWidth">
    /// The width of the image.
    /// </param>
    /// <param name="pixelHeight">
    /// The height of the image.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="pixelWidth" /> or <paramref name="pixelHeight" /> is negative.
    /// </exception>
    /// <exception cref="OverflowException">
    /// The product of <paramref name="pixelWidth"/> and <paramref name="pixelHeight"/> exceeds the maximum 
    /// number of pixels that can be stored in <see cref="Pixels"/>.
    /// </exception>
    public void Resize(int pixelWidth, int pixelHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pixelWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(pixelHeight);

        if (_pixelWidth == pixelWidth && _pixelHeight == pixelHeight)
        {
            return;
        }

        int length = checked(pixelWidth * pixelHeight);

        _pixels = new int[length];
        SetValueInternal(PixelWidthPropertyKey, pixelWidth);
        SetValueInternal(PixelHeightPropertyKey, pixelHeight);

        _isMeasureDirty = true;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize) => MeasureArrangeHelper(availableSize);

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize) => MeasureArrangeHelper(finalSize);

    /// <inheritdoc />
    protected internal override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        (var outerDiv, _canvas) = INTERNAL_HtmlDomManager.CreateWriteableImageDomElementAndAppendIt(parent, this);
        Invalidate();
        return outerDiv;
    }

    private Size MeasureArrangeHelper(Size inputSize)
    {
        if (_pixelWidth == 0 && _pixelHeight == 0)
        {
            return new Size();
        }

        var naturalSize = new Size(_pixelWidth, _pixelHeight);

        // get computed scale factor
        Size scaleFactor = Viewbox.ComputeScaleFactor(inputSize,
            naturalSize,
            Stretch,
            StretchDirection);

        // Returns our minimum size & sets DesiredSize.
        return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
    }

    private void TransferBytes()
    {
        if (OpenSilver.Interop.IsRunningInTheSimulator)
        {
            TransferBytesSlow();
        }
        else
        {
            TransferBytesWasm();
        }
    }

    private void TransferBytesWasm()
    {
        OpenSilver.Interop.JavaScriptRuntime.Flush();
        OpenSilver.Interop.NativeMethods.WriteableImage_TransferBytes(
            _canvas.Uid,
            MemoryMarshal.AsBytes(_pixels.AsSpan()),
            _pixelWidth,
            _pixelHeight);
    }

    private void TransferBytesSlow()
    {
        string base64;

        int byteLength = _pixels.Length * 4;
        byte[] bytes = ArrayPool<byte>.Shared.Rent(byteLength);
        byte[] compressed = ArrayPool<byte>.Shared.Rent(GetDeflateBound(byteLength));

        try
        {
            MemoryMarshal.AsBytes(_pixels).CopyTo(bytes);

            using var destStream = new MemoryStream(compressed, 0, compressed.Length, writable: true, publiclyVisible: true);
            using (var deflate = new DeflateStream(destStream, CompressionLevel.Fastest, leaveOpen: true))
            {
                deflate.Write(bytes, 0, byteLength);
            }

            base64 = Convert.ToBase64String(compressed, 0, (int)destStream.Position);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
            ArrayPool<byte>.Shared.Return(compressed);
        }

        string width = _pixelWidth.ToInvariantString();
        string height = _pixelHeight.ToInvariantString();
        OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.writeableImage.transferBytesBase64('{_canvas.Uid}', '{base64}', {width}, {height})");
    }

    private static int GetDeflateBound(int uncompressedLength)
    {
        // Same conservative zlib/deflate bound as PngEncoder (raw DEFLATE, no zlib wrapper).
        long n = uncompressedLength;
        long fixedBound = n + ((n + 7) >> 3) + ((n + 63) >> 6) + 5;
        long storedBound = n + (n >> 5) + (n >> 7) + (n >> 11) + 7;
        long bound = Math.Max(fixedBound, storedBound);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(bound, int.MaxValue);

        return (int)bound;
    }
}
