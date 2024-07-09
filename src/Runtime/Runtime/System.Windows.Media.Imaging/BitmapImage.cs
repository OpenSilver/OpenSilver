
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
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows.Media.Imaging;

/// <summary>
/// Provides the practical object source type for the <see cref="Image.Source"/>
/// and <see cref="ImageBrush.ImageSource"/> properties.
/// </summary>
public sealed class BitmapImage : BitmapSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapImage"/> class.
    /// </summary>
    public BitmapImage() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapImage"/> class, using the supplied URI.
    /// </summary>
    /// <param name="uriSource">
    /// The URI that references the source graphics file for the image.
    /// </param>
    public BitmapImage(Uri uriSource)
    {
        UriSource = uriSource;
    }

    /// <summary>
    /// Identifies the <see cref="UriSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UriSourceProperty =
        DependencyProperty.Register(
            nameof(UriSource),
            typeof(Uri),
            typeof(BitmapImage),
            new PropertyMetadata(null, OnUriSourceChanged));

    /// <summary>
    /// Gets or sets the URI of the graphics source file that generated this <see cref="BitmapImage"/>.
    /// </summary>
    /// <returns>
    /// The URI of the graphics source file that generated this <see cref="BitmapImage"/>.
    /// </returns>
    public Uri UriSource
    {
        get => (Uri)GetValue(UriSourceProperty);
        set
        {
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
#pragma warning disable CS0618 // Type or member is obsolete
            INTERNAL_NameOfAssemblyThatSetTheSourceUri = callerAssemblyName;
#pragma warning restore CS0618 // Type or member is obsolete
            SetValueInternal(UriSourceProperty, value);
        }
    }

    private static void OnUriSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e)
    {
        var bitmapImage = (BitmapImage)i;
        bitmapImage.RaiseChanged();
        bitmapImage.OnUriSourceChanged();
    }

    /// <summary>
    /// Identifies the <see cref="CreateOptions"/> dependency property.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly DependencyProperty CreateOptionsProperty =
        DependencyProperty.Register(
            nameof(CreateOptions),
            typeof(BitmapCreateOptions),
            typeof(BitmapImage),
            new PropertyMetadata(BitmapCreateOptions.DelayCreation));

    /// <summary>
    /// Gets or sets the <see cref="BitmapCreateOptions"/> for a <see cref="BitmapImage"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="BitmapCreateOptions"/> used for this <see cref="BitmapImage"/>.
    /// The default is <see cref="BitmapCreateOptions.DelayCreation"/>.
    /// </returns>
    [OpenSilver.NotImplemented]
    public BitmapCreateOptions CreateOptions
    {
        get => (BitmapCreateOptions)GetValue(CreateOptionsProperty);
        set => SetValue(CreateOptionsProperty, value);
    }

    /// <summary>
    /// Occurs when there is an error associated with image retrieval or format.
    /// </summary>
    [OpenSilver.NotImplemented]
    public event ExceptionRoutedEventHandler ImageFailed;

    /// <summary>
    /// Occurs when the image source is downloaded and decoded with no failure. You can
    /// use this event to determine the size of an image before rendering it.
    /// </summary>
    [OpenSilver.NotImplemented]
    public event RoutedEventHandler ImageOpened;

    /// <summary>
    /// Occurs when the <see cref="UriSource"/> is changed.
    /// </summary>
    public event EventHandler UriSourceChanged;

    internal override ValueTask<string> GetDataStringAsync(UIElement parent)
    {
        if (UriSource != null)
        {
            return new(INTERNAL_UriHelper.ConvertToHtml5Path(UriSource.OriginalString, parent));
        }

        return base.GetDataStringAsync(parent);
    }

    private void OnUriSourceChanged() => UriSourceChanged?.Invoke(this, EventArgs.Empty);

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string INTERNAL_NameOfAssemblyThatSetTheSourceUri; // Useful to convert relative URI to absolute URI.
}
