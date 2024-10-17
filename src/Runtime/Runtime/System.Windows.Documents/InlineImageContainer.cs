
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
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace System.Windows.Documents;

public sealed class InlineImageContainer : Inline
{
    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(ImageSource),
            typeof(InlineImageContainer),
            new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// Gets or sets the source for the image.
    /// </summary>
    /// <returns>
    /// A source object for the drawn image.
    /// </returns>
    [TypeConverter(typeof(ExtendedImageSourceConverter))]
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValueInternal(SourceProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Stretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(InlineImageContainer),
            new PropertyMetadata(Stretch.Uniform, OnPropertyChanged));

    /// <summary>
    /// Gets or sets a value that describes how an <see cref="InlineImageContainer"/> should be stretched 
    /// to fill the destination rectangle.
    /// </summary>
    /// <returns>
    /// A value of the <see cref="Media.Stretch"/> enumeration that specifies how the source image is 
    /// applied if the <see cref="Height"/> and <see cref="Width"/> of the <see cref="InlineImageContainer"/> 
    /// are specified and are different than the source image's height and width. The default value is 
    /// <see cref="Stretch.Uniform"/>.
    /// </returns>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValueInternal(StretchProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
            nameof(Width),
            typeof(double),
            typeof(InlineImageContainer),
            new PropertyMetadata(double.NaN, OnPropertyChanged),
            IsWidthHeightValid);

    /// <summary>
    /// Gets or sets the width of a <see cref="InlineImageContainer"/>.
    /// </summary>
    /// <returns>
    /// The width of the object, in pixels. The default is <see cref="double.NaN"/>. Except
    /// for the special <see cref="double.NaN"/> value, this value must be equal to or greater
    /// than 0.
    /// </returns>
    [TypeConverter(typeof(OpenSilver.Internal.LengthConverter))]
    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValueInternal(WidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height),
            typeof(double),
            typeof(InlineImageContainer),
            new PropertyMetadata(double.NaN, OnPropertyChanged),
            IsWidthHeightValid);

    /// <summary>
    /// Gets or sets the suggested height of a <see cref="InlineImageContainer"/>.
    /// </summary>
    /// <returns>
    /// The height, in pixels, of the object. The default is <see cref="double.NaN"/>. Except
    /// for the special <see cref="double.NaN"/> value, this value must be equal to or greater
    /// than 0.
    /// </returns>
    [TypeConverter(typeof(OpenSilver.Internal.LengthConverter))]
    public double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValueInternal(HeightProperty, value);
    }

    private static bool IsWidthHeightValid(object value)
    {
        double v = (double)value;
        return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((InlineImageContainer)d).OnPropertyChanged();
    }

    private void OnPropertyChanged()
    {
        if (IsModel)
        {
            TextContainer.OnTextContentChanged();
        }
    }

    internal string GetImageData()
    {
        if (Source is ImageSource source)
        {
            var task = source.GetDataStringAsync(this);
            if (task.IsCompleted)
            {
                return task.Result;
            }
        }

        return string.Empty;
    }

    internal string GetOriginalSource()
    {
        if (Source is BitmapImage bmi && bmi.UriSource is Uri uri)
        {
            return uri.ToString();
        }

        return null;
    }

    internal static string ConvertStretch(Stretch stretch) =>
        stretch switch
        {
            Stretch.None => "none",
            Stretch.Fill => "fill",
            Stretch.Uniform => "contain",
            Stretch.UniformToFill => "cover",
            _ => string.Empty,
        };

    internal static Stretch ParseStretch(string str) =>
        str switch
        {
            "none" => Stretch.None,
            "fill" => Stretch.Fill,
            "cover" => Stretch.UniformToFill,
            _ => Stretch.Uniform,
        };

    internal static ImageSource ParseSource(string str) => ExtendedImageSourceConverter.FromString(str);

    private sealed class ExtendedImageSourceConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(Uri) || sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value is string source)
            {
                return FromString(source);
            }
            else if (value is Uri uri)
            {
                return new BitmapImage(uri);
            }

            throw GetConvertFromException(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException($"'{typeof(ExtendedImageSourceConverter)}' does not implement '{nameof(ConvertTo)}'.");
        }

        internal static ImageSource FromString(string source)
        {
            if (source is null)
            {
                return null;
            }

            UriKind uriKind;
            if (source.Contains(":/"))
            {
                uriKind = UriKind.Absolute;
            }
            else
            {
                uriKind = UriKind.Relative;
            }

            if (Uri.TryCreate(source, uriKind, out Uri uri))
            {
                return new BitmapImage(uri);
            }

            // just assume it is a base64 data url
            return new Base64Image(source);
        }
    }
}
