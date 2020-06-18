

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


using CSHTML5;
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    /// <summary>
    /// Provides the practical object source type for the Source and ImageSource properties.
    /// </summary>
    public partial class BitmapImage : BitmapSource
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

        public string INTERNAL_NameOfAssemblyThatSetTheSourceUri; // Useful to convert relative URI to absolute URI.


        // Summary:
        //     Gets or sets the BitmapCreateOptions for a BitmapImage.
        //
        // Returns:
        //     The BitmapCreateOptions used for this BitmapImage.
        //public BitmapCreateOptions CreateOptions { get; set; }
        //
        // Summary:
        //     Identifies the CreateOptions dependency property.
        //
        // Returns:
        //     The identifier for the CreateOptions dependency property.
        //public static DependencyProperty CreateOptionsProperty { get; }

        //// <summary>
        //// Gets or sets the height to use for image decoding operations.
        //// </summary>
        /*
        public int DecodePixelHeight
        {
            get { return (int)GetValue(DecodePixelHeightProperty); }
            set { SetValue(DecodePixelHeightProperty, value); }
        }
        public static readonly DependencyProperty DecodePixelHeightProperty =
            DependencyProperty.Register("DecodePixelHeight", typeof(int), typeof(BitmapImage), new PropertyMetadata(null, DecodePixelHeight_Changed));


        static void DecodePixelHeight_Changed(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)i;
            int newValue = (int)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(image))
            {
                if (newValue is BitmapImage)
                {
                    //todo.
                }
                INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(image).src = newValue; //translate the element to a html version.
                INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(image).alt = ""; //the text displayed when the image cannot be found. We set it as an empty string since there is nothing in Xaml
            }
        }
        //
        // Summary:
        //     Gets or sets the width to use for image decoding operations.
        //
        // Returns:
        //     The width (in pixels) to use for image decoding operations.
        public int DecodePixelWidth { get; set; }
        //
        // Summary:
        //     Identifies the DecodePixelWidth dependency property.
        //
        // Returns:
        //     The identifier for the DecodePixelWidth dependency property.
        public static DependencyProperty DecodePixelWidthProperty { get; }
         * */

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
#if !BRIDGE
                // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
                string callerAssemblyName = CSHTML5.Interop.IsRunningInTheSimulator ? Assembly.GetCallingAssembly().GetName().Name : INTERNAL_UriHelper.GetJavaScriptCallingAssembly();
#else
                // Get the assembly name of the calling method: //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method that is executed immediately after the one where the URI is defined! Be careful when moving the following line of code.
                string callerAssemblyName = INTERNAL_UriHelper.GetJavaScriptCallingAssembly();

#endif
                this.INTERNAL_NameOfAssemblyThatSetTheSourceUri = callerAssemblyName;
                SetValue(UriSourceProperty, value);
            }
        }
        /// <summary>
        /// Identifies the UriSource dependency property.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(BitmapImage), new PropertyMetadata(null, UriSource_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        static void UriSource_Changed(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            var bitmapImage = (BitmapImage)i;
            Uri newValue = (Uri)e.NewValue;
            bitmapImage.OnUriSourceChanged();

            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(bitmapImage))
            //{

            //    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(bitmapImage).src = newValue; //translate the element to a html version.
            //    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(bitmapImage).alt = ""; //the text displayed when the image cannot be found. We set it as an empty string since there is nothing in Xaml
            //}
        }



        // Summary:
        //     Occurs when a significant change has occurred in the download progress of
        //     the BitmapImage content.
        //public event DownloadProgressEventHandler DownloadProgress;

        /// <summary>
        /// Occurs when there is an error associated with image retrieval or format.
        /// </summary>
        public event ExceptionRoutedEventHandler ImageFailed;
        protected void OnImageFailed()
        {
            if (ImageFailed != null)
            {
                ImageFailed(this, new ExceptionRoutedEventArgs()
                {
                    OriginalSource = this
                });
            }
        }

        /// <summary>
        /// Occurs when the image source is downloaded and decoded with no failure. You can use this event to determine the size of an image before rendering it.
        /// </summary>
        public event RoutedEventHandler ImageOpened;
        protected void OnImageOpened()
        {
            if (ImageOpened != null)
            {
                ImageOpened(this, new RoutedEventArgs()
                {
                    OriginalSource = this
                });
            }
        }

        //internal override void INTERNAL_AttachToDomEvents()
        //{
        //    PrivateAttachToDomEvents("load", e =>
        //    {
        //        OnImageOpened();
        //    });

        //    PrivateAttachToDomEvents("error", e =>
        //    {
        //        OnImageFailed();
        //    });
        //}

        //[JSReplacement("$this.INTERNAL_OuterDomElement.addEventListener($name, $handler)")]
        //void PrivateAttachToDomEvents(string name, Action<object> handler)
        //{
        //    HtmlEventProxy.Create("on" + name, this.INTERNAL_OuterDomElement, (EventHandler)((s, e) => { handler(e); }));
        //}

        /// <summary>
        /// Occurs when the UriSource is changed
        /// </summary>
        public event EventHandler UriSourceChanged;
        protected void OnUriSourceChanged()
        {
            if (UriSourceChanged != null)
            {
                UriSourceChanged(this, new EventArgs());
            }
        }

#if WORKINPROGRESS
        public BitmapCreateOptions CreateOptions { get; set; }
#endif
    }
}
