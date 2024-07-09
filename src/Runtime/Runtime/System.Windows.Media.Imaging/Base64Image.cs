
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

namespace System.Windows.Media.Imaging;

public sealed class Base64Image : ImageSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Base64Image"/> class.
    /// </summary>
    public Base64Image() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Base64Image"/> class.
    /// </summary>
    /// <param name="dataUrl">
    /// The base64 image.
    /// </param>
    public Base64Image(string dataUrl)
    {
        DataURL = dataUrl;
    }

    /// <summary>
    /// Identifies the <see cref="DataURL"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataURLProperty =
        DependencyProperty.Register(
            nameof(DataURL),
            typeof(string),
            typeof(Base64Image),
            new PropertyMetadata(string.Empty, OnDataUrlChanged));

    /// <summary>
    /// Gets or sets the source of this image.
    /// </summary>
    public string DataURL
    {
        get => (string)GetValue(DataURLProperty);
        set => SetValue(DataURLProperty, value);
    }

    private static void OnDataUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Base64Image)d).RaiseChanged();
    }

    internal override ValueTask<string> GetDataStringAsync(UIElement parent) => new(DataURL ?? string.Empty);
}
