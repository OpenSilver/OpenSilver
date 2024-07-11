
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Automation.Peers;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows.Controls
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
        private INTERNAL_HtmlDomElementReference _imageDiv;
        private Size _naturalSize;
        private WeakEventListener<Image, ImageSource, EventArgs> _sourceChangedListener;

        static Image()
        {
            IsHitTestableProperty.OverrideMetadata(typeof(Image), new PropertyMetadata(BooleanBoxes.TrueBox));
        }

        internal override bool EnablePointerEventsCore => true;

        /// <summary>
        /// Gets or sets the source for the image.
        /// </summary>
        /// <returns>
        /// A source object for the drawn image.
        /// </returns>
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValueInternal(SourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(ImageSource),
                typeof(Image),
                new PropertyMetadata(null, OnSourceChanged)
                {
                    MethodToUpdateDom2 = UpdateDomOnSourceChanged,
                });

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Image image = (Image)d;

            if (image._sourceChangedListener != null)
            {
                image._sourceChangedListener.Detach();
                image._sourceChangedListener = null;
            }

            if (e.NewValue is ImageSource source)
            {
                image._sourceChangedListener = new(image, source)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnSourceChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
                };
                source.Changed += image._sourceChangedListener.OnEvent;
            }
        }

        private static void UpdateDomOnSourceChanged(DependencyObject d, object oldValue, object newValue)
        {
            ((Image)d).RefreshSource();
        }

        /// <summary>
        /// Gets or sets a value that describes how an <see cref="Image"/> should
        /// be stretched to fill the destination rectangle.
        /// </summary>
        /// <returns>
        /// A value of the <see cref="Media.Stretch"/> enumeration that specifies how the
        /// source image is applied if the <see cref="FrameworkElement.Height"/> and 
        /// <see cref="FrameworkElement.Width"/> of the <see cref="Image"/> are specified 
        /// and are different than the source image's height and width. The default value 
        /// is <see cref="Stretch.Uniform"/>.
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValueInternal(StretchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(Image),
                new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure)
                {
                    MethodToUpdateDom2 = UpdateDomOnStretchChanged,
                });

        private static void UpdateDomOnStretchChanged(DependencyObject d, object oldValue, object newValue)
        {
            Image img = (Image)d;

            img._imageDiv.Style.objectFit = (Stretch)newValue switch
            {
                Stretch.None => "none",
                Stretch.Fill => "fill",
                Stretch.Uniform => "contain",
                Stretch.UniformToFill => "cover",
                _ => string.Empty,
            };

            img.SetObjectPosition();
        }

        /// <summary>
        /// Occurs when there is an error associated with image retrieval or format.
        /// </summary>
        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

        /// <summary>
        /// Occurs when the image source is downloaded and decoded with no failure.
        /// </summary>
        public event EventHandler<RoutedEventArgs> ImageOpened;

        internal INTERNAL_HtmlDomElementReference ImageDiv => _imageDiv;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            domElementWhereToPlaceChildren = null;
            (var outerDiv, _imageDiv) = INTERNAL_HtmlDomManager.CreateImageDomElementAndAppendIt((INTERNAL_HtmlDomElementReference)parentRef, this);
            return outerDiv;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new ImageAutomationPeer(this);

        protected override Size MeasureOverride(Size availableSize)
            => MeasureArrangeHelper(availableSize);

        protected override Size ArrangeOverride(Size finalSize)
            => MeasureArrangeHelper(finalSize);

        /// <summary>
        /// Contains the code common for MeasureOverride and ArrangeOverride.
        /// </summary>
        /// <param name="inputSize">
        /// input size is the parent-provided space that Image should use to "fit in", 
        /// according to other properties.
        /// </param>
        /// <returns>Image's desired size.</returns>
        private Size MeasureArrangeHelper(Size inputSize)
        {
            if (Source == null)
            {
                return new Size();
            }

            Size naturalSize = _naturalSize;

            //get computed scale factor
            Size scaleFactor = Viewbox.ComputeScaleFactor(inputSize,
                naturalSize,
                Stretch,
                StretchDirection.Both);

            // Returns our minimum size & sets DesiredSize.
            return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
        }

        private void SetObjectPosition()
        {
            if (_imageDiv == null) return;

            string hPos, vPos;

            switch (Stretch)
            {
                case Stretch.None:
                case Stretch.Uniform:
                    hPos = HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => "left",
                        HorizontalAlignment.Right => "right",
                        _ => "center",
                    };
                    vPos = VerticalAlignment switch
                    {
                        VerticalAlignment.Top => "top",
                        VerticalAlignment.Bottom => "bottom",
                        _ => "center",
                    };
                    break;

                case Stretch.Fill:
                case Stretch.UniformToFill:
                    hPos = "left";
                    vPos = "top";
                    break;

                default:
                    hPos = vPos = "center";
                    break;
            }

            _imageDiv.Style.objectPosition = $"{hPos} {vPos}";
        }

        private void OnSourceChanged(object sender, EventArgs e) => RefreshSource();

        private void RefreshSource()
        {
            _naturalSize = new Size();

            if (Source is ImageSource source)
            {
                var task = source.GetDataStringAsync(this);
                if (task.IsCompletedSuccessfully)
                {
                    INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", task.Result ?? string.Empty, true);
                }
            }
            else
            {
                INTERNAL_HtmlDomManager.RemoveAttribute(_imageDiv, "src");
                _imageDiv.Style.display = "none";

                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        internal void OnLoadNative()
        {
            _naturalSize = ImageManager.Instance.GetNaturalSize(this);

            InvalidateMeasure();
            InvalidateArrange();
            ImageOpened?.Invoke(this, new RoutedEventArgs { OriginalSource = this });
        }

        internal void OnErrorNative()
        {
            _naturalSize = new Size();

            InvalidateMeasure();
            InvalidateArrange();
            ImageFailed?.Invoke(this, new ExceptionRoutedEventArgs { OriginalSource = this });
        }
    }
}
