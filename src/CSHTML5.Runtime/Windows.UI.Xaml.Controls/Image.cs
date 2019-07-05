
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



//TODOBRIDGE: usefull using bridge?
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Media;
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that displays an image. The image is specified as an image file in several possible formats, see Remarks.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:ms-appx="aa" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <StackPanel x:Name="MyStackPanel">
    ///     <Image Source="ms-appx:/MyImage.png"/>
    /// </StackPanel>
    /// </code>
    /// <code lang="C#">
    /// Image image = new Image();
    /// image.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:/MyImage.png"));
    /// MyStackPanel.Children.Add(image);
    /// </code>
    /// </example>
    public sealed class Image : FrameworkElement
    {
        dynamic _imageDiv = null;
        double imgWidth = 0; //might be useless, might be useful.
        double imgHeight = 0;


        /*
        /// <summary>
        /// Gets or sets a value for a nine-grid metaphor that controls how the image can be resized.
        /// Returns a  Thickness value that sets the Left, Top, Right, Bottom measurements for the nine-grid resizing metaphor.
        /// </summary>
        public Thickness NineGrid { get; set; }

        /// <summary>
        /// Identifies the NineGrid dependency property.
        /// Returns the identifier for the NineGrid dependency property.
        /// </summary>
        public static DependencyProperty NineGridProperty { get; }

        //
        // Summary:
        //     Gets the information that is transmitted if the Image is used for a "PlayTo"
        //     scenario.
        //
        // Returns:
        //     A reference object that carries the "PlayTo" source information.
        public PlayToSource PlayToSource { get; }
        //
        // Summary:
        //     Identifies the PlayToSource dependency property.
        //
        // Returns:
        //     The identifier for the PlayToSource dependency property.
        public static DependencyProperty PlayToSourceProperty { get; }
         * */

        /// <summary>
        /// Gets or sets the source for the image.
        /// 
        /// Returns an object that represents the image source file for the drawn image. Typically
        /// you set this with a BitmapSource object, constructed with the that describes
        /// the path to a valid image source file.
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(Image), new PropertyMetadata(null, Source_Changed));


        static void Source_Changed(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)i;
            ImageSource newValue = (ImageSource)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(image))
            {
                if (newValue is BitmapImage)
                {
                    BitmapImage bmpImage = (BitmapImage)newValue;
                    //we relay the bitmap's events:
                    bmpImage.ImageFailed -= image.bmpImage_ImageFailed;
                    bmpImage.ImageFailed += image.bmpImage_ImageFailed;
                    bmpImage.ImageOpened -= image.bmpImage_ImageOpened;
                    bmpImage.ImageOpened += image.bmpImage_ImageOpened;
                    bmpImage.UriSourceChanged -= image.bmpImage_UriSourceChanged;
                    bmpImage.UriSourceChanged += image.bmpImage_UriSourceChanged;
                }
                image.RefreshSource();
            }
        }

        void bmpImage_UriSourceChanged(object sender, EventArgs e)
        {
            RefreshSource();
        }

        private void RefreshSource()
        {
            if (Source != null)
            {
                Loaded += Image_Loaded;
                if (Source is BitmapImage)
                {
                    BitmapImage sourceAsBitmapImage = (BitmapImage)Source;
                    if (sourceAsBitmapImage.UriSource != null)
                    {
                        Uri sourceUri = null;
                        sourceUri = ((BitmapImage)Source).UriSource;

                        string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(sourceUri.OriginalString, this);

                        INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", html5Path);
                    }
                    else if (sourceAsBitmapImage.INTERNAL_StreamSource != null)
                    {
                        string dataUrl = "data:image/png;base64," + sourceAsBitmapImage.INTERNAL_StreamAsBase64String;
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", dataUrl);
                    }
                    else if (!string.IsNullOrEmpty(sourceAsBitmapImage.INTERNAL_DataURL))
                    {
                        string dataUrl = sourceAsBitmapImage.INTERNAL_DataURL;
                        INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", dataUrl);
                    }
                }
            }
            else
            {
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", "");
            }
            INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "alt", " "); //the text displayed when the image cannot be found. We set it as an empty string since there is nothing in Xaml
        }

        void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Image_Loaded;
            //once the image is loaded, we get the size of the parent of this element and fit the size of the image to it.
            //This will allow us to PARTIALLY fix the problems that come with the fact that display:table is unable to limit the size of its content.

            //todo-perf: we might want to put Parent in a local variable but I doubt this would have a big impact since it only happens once when the image is loaded.
            //              If we move this to a method that will be called whenever there is a size change, we might actually want to do it (probably still minor impact on performance though).

            double parentWidth = ((FrameworkElement)Parent).ActualWidth;
            double parentHeight = ((FrameworkElement)Parent).ActualHeight;


            imgHeight = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.naturalHeight", _imageDiv));
            imgWidth = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.naturalWidth", _imageDiv));

            double currentWidth = ActualWidth;
            double currentHeight = ActualHeight;

            // I think it should be independent from the value of this.Stretch due to object-fit.
            // If one of the sizes is bigger than that of the container, reduce it?
            // -----------------------smaller----------------------------increase it?

            bool isParentLimitingHorizontalSize = !((Parent is StackPanel && ((StackPanel)Parent).Orientation == Orientation.Horizontal)
                                                    || (Parent is WrapPanel && ((WrapPanel)Parent).Orientation == Orientation.Horizontal)
                                                    || Parent is Canvas); //todo: fill the list.
            bool limitSize = isParentLimitingHorizontalSize
                                && (HorizontalAlignment == HorizontalAlignment.Stretch && (double.IsNaN(Width) && currentWidth != parentWidth)) //is stretch and different size than the parent
                                || ((double.IsNaN(Width) && currentWidth > parentWidth));  //this can only be true if not stretch, meaning that we only want to limit the size to that of the parent.

            if (isParentLimitingHorizontalSize)
            {
                if (double.IsNaN(Width) && currentWidth != parentWidth)
                {
                    imgWidth = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.style.width = $1", _imageDiv, parentWidth));
                    //_imageDiv.style.width = parentWidth;
                }
            }

            bool isParentLimitingVerticalSize = !((Parent is StackPanel && ((StackPanel)Parent).Orientation == Orientation.Vertical)
                                                    || (Parent is WrapPanel && ((WrapPanel)Parent).Orientation == Orientation.Vertical)
                                                    || Parent is Canvas); //todo: fill the list.
            if (isParentLimitingVerticalSize)
            {
                if (double.IsNaN(Height) && currentHeight != parentHeight)
                {
                    imgWidth = Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("$0.style.height = $1", _imageDiv, parentHeight));
                    //_imageDiv.style.height = parentHeight;
                }
            }
        }

        void bmpImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            OnImageOpened(e);
        }

        void bmpImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnImageFailed(e);
        }


        /// <summary>
        /// Gets or sets a value that describes how an Image should be stretched to fill the destination rectangle.
        /// 
        /// Returns a value of the Stretch enumeration that specifies how the source image is applied if the Height and Width of the Image are specified and are different
        /// than the source image's height and width. The default value is Uniform.
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        /// <summary>
        /// Identifies the Stretch dependency property
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Image), new PropertyMetadata(Stretch.Uniform, Stretch_Changed));


        static void Stretch_Changed(DependencyObject i, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image)i;
            Stretch newValue = (Stretch)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(image))
            {
                string objectFitvalue = "";
                string objectPosition = "center top"; //todo: see if the values are correct for this and if it should take into consideration the Vertical/HorizontalAlignment.
                switch (newValue)
                {
                    case Stretch.None:
                        //img.width = "auto";
                        //img.height = "auto";
                        objectFitvalue = "none";
                        objectPosition = "left top";
                        break;
                    case Stretch.Fill:
                        // Commented because it should be the same as the one set by the FrameworkElement Width/Height.
                        //img.width = "100%";
                        //img.height = "100%";
                        objectFitvalue = "100% 100%";
                        objectPosition = "center center";
                        break;
                    case Stretch.Uniform: //todo: make it work when the image needs to be made bigger to fill the container
                        //img.maxWidth = "100%";
                        //img.maxHeight = "100%";
                        objectFitvalue = "contain";
                        objectPosition = "center center";
                        break;
                    case Stretch.UniformToFill: //todo: add a negative margin top and left so that the image is centered 
                        //img.minWidth = "100%";
                        //img.minHeight = "100%";
                        objectFitvalue = "cover";
                        objectPosition = "left top";
                        break;
                    default:
                        break;
                }
                CSHTML5.Interop.ExecuteJavaScript(@"$0.style.objectFit = $1;
$0.style.objectPosition = $2", image._imageDiv, objectFitvalue, objectPosition);

            }
        }
        

        
        #region Image failed event

        INTERNAL_EventManager<ExceptionRoutedEventHandler, ExceptionRoutedEventArgs> _imageFailedEventManager = null;

        /// <summary>
        /// Occurs when there is an error associated with image retrieval or format.
        /// </summary>
        public event ExceptionRoutedEventHandler ImageFailed
        {
            add
            {
                if (_imageFailedEventManager == null)
                {
                    _imageFailedEventManager = new INTERNAL_EventManager<ExceptionRoutedEventHandler, ExceptionRoutedEventArgs>(() => _imageDiv, "error", ProcessOnImageFailed);
                    _imageFailedEventManager.Add(value);
                }
                else
                {
                    _imageFailedEventManager.Add(value);
                }
            }
            remove
            {
                if (_imageFailedEventManager != null)
                {
                    _imageFailedEventManager.Remove(value);
                }
                
            }
        }


        /// <summary>
        /// Raises the ImageFailed event
        /// </summary>
        void ProcessOnImageFailed(object jsEventArg)
        {
            var eventArgs = new ExceptionRoutedEventArgs() //todo: fill the rest
            {
                OriginalSource = this
            };
            OnImageFailed(eventArgs);
        }

        /// <summary>
        /// Raises the ImageFailed event
        /// </summary>
        void OnImageFailed(ExceptionRoutedEventArgs eventArgs)
        {
            foreach (ExceptionRoutedEventHandler handler in _imageFailedEventManager.Handlers.ToList<ExceptionRoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }


        #endregion


        #region Image opened event

        INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs> _imageOpenedEventManager = null;

        /// <summary>
        /// Occurs when the image source is downloaded and decoded with no failure.
        /// </summary>
        public event RoutedEventHandler ImageOpened
        {
            add
            {
                if (_imageOpenedEventManager == null)
                {
                    _imageOpenedEventManager = new INTERNAL_EventManager<RoutedEventHandler, RoutedEventArgs>(() => _imageDiv, "load", ProcessOnImageOpened);
                    _imageOpenedEventManager.Add(value);
                }
                else
                {
                    _imageOpenedEventManager.Add(value);
                }
            }
            remove
            {
                if (_imageOpenedEventManager != null)
                {
                    _imageOpenedEventManager.Remove(value);
                }
            }
        }

       
        /// <summary>
        /// Raises the ImageOpened event
        /// </summary>
        void ProcessOnImageOpened(object jsEventArg)
        {
            var eventArgs = new RoutedEventArgs() //todo: fill the rest
            {
                OriginalSource = this
            };
            OnImageOpened(eventArgs);
        }

        /// <summary>
        /// Raises the ImageOpened event
        /// </summary>
        void OnImageOpened(RoutedEventArgs eventArgs)
        {
            foreach (RoutedEventHandler handler in _imageOpenedEventManager.Handlers.ToList<RoutedEventHandler>())
            {
                handler(this, eventArgs);
            }
        }


        #endregion






        //todo: create a test case for those events.
        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();
            if (_imageOpenedEventManager != null)
            {
                _imageOpenedEventManager.AttachToDomEvents();
            }
            if (_imageFailedEventManager != null)
            {         
                _imageFailedEventManager.AttachToDomEvents();
            }
        }

        public override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();
            if (_imageOpenedEventManager != null)
            {
                _imageOpenedEventManager.DetachFromDomEvents();
            }
            if (_imageFailedEventManager != null)
            {
                _imageFailedEventManager.DetachFromDomEvents();
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            //<img style="width: 100%; height: 100%;" src="C:\Users\Sylvain\Documents\Adventure Maker v4.7\Projects\ASA_game\Icons\settings.ico" alt="settings" />
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);

            var img = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("img", div, this);
            INTERNAL_HtmlDomManager.SetDomElementAttribute(img, "alt", " "); //the text displayed when the image cannot be found. We set it as an empty string since there is nothing in Xaml

            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(img);
            style.display = "block"; //this is to avoid a random few pixels wide gap below the image.
            style.width = "100%";
            style.height = "100%";
            style.objectPosition = "center top";

            CSHTML5.Interop.ExecuteJavaScriptAsync(@"
$0.addEventListener('mousedown', function(e) {
e.preventDefault();
}, false);", img);

            _imageDiv = img;
            domElementWhereToPlaceChildren = null;
            return div;
        }

        public object INTERNAL_DomImageElement
        {
            get
            {
                return _imageDiv;
            }
        }
    }
}
