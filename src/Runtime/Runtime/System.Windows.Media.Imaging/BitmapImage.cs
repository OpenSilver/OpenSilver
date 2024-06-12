
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

using System.Reflection;
using System.Threading.Tasks;
using CSHTML5.Internal;

namespace System.Windows.Media.Imaging
{
    /// <summary>
    /// Provides the practical object source type for the Source and ImageSource properties.
    /// </summary>
    public sealed class BitmapImage : BitmapSource
    {
        /// <summary>
        /// Initializes a new instance of the BitmapImage class.
        /// </summary>
        public BitmapImage() : base() { }
        /// <summary>
        /// Initializes a new instance of the BitmapImage class with the specified path to the source.
        /// </summary>
        /// <param name="uriSource">The path to the source of the image.</param>
        public BitmapImage(Uri uriSource)
            : base()
        {
            UriSource = uriSource;
        }

        internal override ValueTask<string> GetDataStringAsync(UIElement parent)
        {
            if (UriSource != null)
            {
                return new(INTERNAL_UriHelper.ConvertToHtml5Path(UriSource.OriginalString, parent));
            }

            return base.GetDataStringAsync(parent);
        }

        public string INTERNAL_NameOfAssemblyThatSetTheSourceUri; // Useful to convert relative URI to absolute URI.

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) of the graphics source
        /// file that generated this BitmapImage.
        /// 
        /// Returns an object describing the of the graphics source file that generated this BitmapImage.
        /// </summary>
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set
            {
                string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
                INTERNAL_NameOfAssemblyThatSetTheSourceUri = callerAssemblyName;
                SetValueInternal(UriSourceProperty, value);
            }
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

        private static void OnUriSourceChanged(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            var bitmapImage = (BitmapImage)i;
            bitmapImage.OnUriSourceChanged();
        }

        /// <summary>
        /// Occurs when there is an error associated with image retrieval or format.
        /// </summary>
        public event ExceptionRoutedEventHandler ImageFailed;
        
        internal void OnImageFailed()
        {
            ImageFailed?.Invoke(this, new ExceptionRoutedEventArgs { OriginalSource = this });
        }

        /// <summary>
        /// Occurs when the image source is downloaded and decoded with no failure. You can use this event to determine the size of an image before rendering it.
        /// </summary>
        public event RoutedEventHandler ImageOpened;
        
        internal void OnImageOpened()
        {
            ImageOpened?.Invoke(this, new RoutedEventArgs { OriginalSource = this });
        }

        /// <summary>
        /// Occurs when the UriSource is changed
        /// </summary>
        public event EventHandler UriSourceChanged;
        
        internal void OnUriSourceChanged()
        {
            UriSourceChanged?.Invoke(this, new EventArgs());
        }

        [OpenSilver.NotImplemented]
        public BitmapCreateOptions CreateOptions { get; set; }
    }
}
