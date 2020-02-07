#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    //
    // Summary:
    //     Specifies initialization options for a bitmap image.
    public enum BitmapCreateOptions
    {
        //
        // Summary:
        //     No initialization options are specified. This is the NOT the default value for
        //     the System.Windows.Media.Imaging.BitmapImage.CreateOptions property in Silverlight
        //     or Silverlight for Windows Phone (System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation
        //     is the default).
        None = 0,
        //
        // Summary:
        //     Causes a System.Windows.Media.Imaging.BitmapSource object to delay initialization
        //     until it is necessary. This is useful when dealing with collections of images.
        //     This is the default value of the System.Windows.Media.Imaging.BitmapImage.CreateOptions
        //     property in Silverlight and Silverlight for Windows Phone.
        DelayCreation = 2,
        //
        // Summary:
        //     Initializes images without using an existing image cache. Any existing entries
        //     in the image cache are replaced, even if they share the same URI. This option
        //     should only be selected when images in a cache need to be refreshed.
        IgnoreImageCache = 8,
        //
        // Summary:
        //     Causes a System.Windows.Media.Imaging.BitmapSource to be initialized as soon
        //     as it is declared. This option uses the image cache for previously used URIs.
        //     If an image is not in the image cache, the image will be downloaded and decoded
        //     on a separate background thread.
        BackgroundCreation = 16
    }
}

#endif