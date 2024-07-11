
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

using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using OpenSilver.Internal;

namespace System.Windows.Media.Imaging;

/// <summary>
/// Provides a source object for properties that use a bitmap.
/// </summary>
public abstract class BitmapSource : ImageSource
{
    private const string PngB64Prefix = "data:image/png;base64,";

    private MemoryStream _streamSource;
    private string _b64String;

    /// <summary>
    /// Sets the source of the <see cref="BitmapSource"/>.
    /// </summary>
    /// <param name="streamSource">
    /// The stream to set the source to.
    /// </param>
    public void SetSource(Stream streamSource)
    {
        // early exit if both values are null
        if (streamSource == _streamSource)
        {
            return;
        }

        MemoryStream stream = null;

        if (streamSource is not null)
        {
            if (streamSource.Length > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "The Stream set as the BitmapSource's Source is too big (more than int.MaxValue (2,147,483,647) bytes).");
            }

            stream = new MemoryStream();
            streamSource.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
        }

        SetSourceInternal(stream);
    }

    internal void SetSourceInternal(MemoryStream stream)
    {
        _streamSource?.Dispose();
        _b64String = null;
        _streamSource = stream;

        RaiseChanged();
    }

    internal override ValueTask<string> GetDataStringAsync(UIElement parent)
    {
        string data = string.Empty;
        if (_streamSource is not null)
        {
            data = AsBase64String();
        }
#pragma warning disable CS0618 // Type or member is obsolete
        else if (!string.IsNullOrEmpty(INTERNAL_DataURL))
        {
            data = INTERNAL_DataURL;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        return new(data);
    }

    private string AsBase64String()
    {
        if (_b64String is null)
        {
            _b64String = PngB64Prefix + Convert.ToBase64String(_streamSource.GetBuffer(), 0, (int)_streamSource.Length);
            _streamSource.Dispose();
        }

        return _b64String;
    }

    /// <summary>
    /// Gets the height of the bitmap in pixels.
    /// </summary>
    [OpenSilver.NotImplemented]
    public int PixelHeight => PixelHeightInternal;

    /// <summary>
    /// Identifies the <see cref="PixelHeight"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty PixelHeightProperty =
        DependencyProperty.Register(
            nameof(PixelHeight),
            typeof(int),
            typeof(BitmapSource),
            new PropertyMetadata(0));

    /// <summary>
    /// Gets the width of the bitmap in pixels.
    /// </summary>
    [OpenSilver.NotImplemented]
    public int PixelWidth => PixelWidthInternal;

    /// <summary>
    /// Identifies the <see cref="PixelWidth"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty PixelWidthProperty =
        DependencyProperty.Register(
            nameof(PixelWidth),
            typeof(int),
            typeof(BitmapSource),
            new PropertyMetadata(0));

    internal virtual int PixelHeightInternal => (int)GetValue(PixelHeightProperty);

    internal virtual int PixelWidthInternal => (int)GetValue(PixelWidthProperty);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use BitmapSource.SetSource(Stream) instead.")]
    public Stream INTERNAL_StreamSource
    {
        get => _streamSource;
        private set => SetSource(value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use the Base64Image class instead.")]
    public string INTERNAL_DataURL { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage)]
    public string INTERNAL_StreamAsBase64String
    {
        get
        {
            if (_streamSource is null)
            {
                return string.Empty;
            }

            return AsBase64String().Substring(PngB64Prefix.Length);
        }
    }

    /// <summary>
    /// Sets the source image for a BitmapSource by passing a "data URL".
    /// </summary>
    /// <param name="dataUrl">The image encoded in "data URL" format.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Helper.ObsoleteMemberMessage + " Use the Base64Image class instead.")]
    public void SetSource(string dataUrl) => INTERNAL_DataURL = dataUrl;
}
