
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

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Globalization;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Automation.Peers;
using Windows.Foundation;
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
        private object _imageDiv;
        private Size _naturalSize;
        private JavaScriptCallback _imgLoadCallback;
        private JavaScriptCallback _imgErrorCallack;
        private WeakEventListener<Image, BitmapImage, EventArgs> _sourceChangedListener;

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
            set => SetValue(SourceProperty, value);
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

            if (e.NewValue is BitmapImage bmi)
            {
                image._sourceChangedListener = new(image, bmi)
                {
                    OnEventAction = static (instance, sender, args) => instance.OnBitmapImageSourceChanged(sender, args),
                    OnDetachAction = static (listener, source) => source.UriSourceChanged -= listener.OnEvent,
                };
                bmi.UriSourceChanged += image._sourceChangedListener.OnEvent;
            }
        }

        private static void UpdateDomOnSourceChanged(DependencyObject d, object oldValue, object newValue)
        {
            _ = ((Image)d).RefreshSource();
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
            set => SetValue(StretchProperty, value);
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

            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(img._imageDiv)
                .objectFit = (Stretch)newValue switch
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

        public override void INTERNAL_AttachToDomEvents()
        {
            base.INTERNAL_AttachToDomEvents();

            DisposeJSCallbacks();

            _imgLoadCallback = JavaScriptCallback.Create(ProcessLoadEvent, true);
            _imgErrorCallack = JavaScriptCallback.Create(ProcessErrorEvent, true);

            string sImage = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_imageDiv);
            string sLoadCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_imgLoadCallback);
            string sErrorCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_imgErrorCallack);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                @$"{sImage}.addEventListener('load', function (e) {{ {sLoadCallback}(); }});
{sImage}.addEventListener('error', function (e) {{ {sErrorCallback}(); }});");
        }

        public override void INTERNAL_DetachFromDomEvents()
        {
            base.INTERNAL_DetachFromDomEvents();
            DisposeJSCallbacks();
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object INTERNAL_DomImageElement => _imageDiv;

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var style = INTERNAL_HtmlDomManager.CreateDomLayoutElementAppendItAndGetStyle("div", parentRef, this, out object div);
            style.lineHeight = "0px";
            _imageDiv = INTERNAL_HtmlDomManager.CreateImageDomElementAndAppendIt(div, this);
            domElementWhereToPlaceChildren = null;
            return div;
        }

        protected override void OnAfterApplyHorizontalAlignmentAndWidth() => SetObjectPosition();

        protected override void OnAfterApplyVerticalAlignmentAndWidth() => SetObjectPosition();

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

            INTERNAL_HtmlDomManager.GetDomElementStyleForModification(_imageDiv).objectPosition = $"{hPos} {vPos}";
        }

        private void DisposeJSCallbacks()
        {
            if (_imgLoadCallback != null)
            {
                _imgLoadCallback.Dispose();
                _imgLoadCallback = null;
            }

            if (_imgErrorCallack != null)
            {
                _imgErrorCallack.Dispose();
                _imgErrorCallack = null;
            }
        }

        private void OnBitmapImageSourceChanged(object sender, EventArgs e)
        {
            _ = RefreshSource();
        }

        private async Task RefreshSource()
        {
            _naturalSize = new Size();

            string sImageDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_imageDiv);
            if (Source != null)
            {
                var imageSrc = await Source.GetDataStringAsync(this);
                INTERNAL_HtmlDomManager.SetDomElementAttribute(_imageDiv, "src", imageSrc ?? string.Empty, true);
            }
            else
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"{sImageDiv}.removeAttribute('src'); {sImageDiv}.style.display = 'none';");

                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        private void ProcessLoadEvent()
        {
            _naturalSize = GetNaturalSize();

            InvalidateMeasure();
            InvalidateArrange();
            ImageOpened?.Invoke(this, new RoutedEventArgs { OriginalSource = this });
        }

        private void ProcessErrorEvent()
        {
            _naturalSize = new Size();

            InvalidateMeasure();
            InvalidateArrange();
            ImageFailed?.Invoke(this, new ExceptionRoutedEventArgs { OriginalSource = this });
        }

        private Size GetNaturalSize()
        {
            string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_imageDiv);
            var size = OpenSilver.Interop.ExecuteJavaScriptString(
                $"(function(img) {{ return img.naturalWidth + '|' + img.naturalHeight; }})({sDiv});");

            int sepIndex = size.IndexOf('|');
            if (sepIndex > -1)
            {
                if (double.TryParse(size.Substring(0, sepIndex), NumberStyles.Any, CultureInfo.InvariantCulture, out double naturalWidth)
                    && double.TryParse(size.Substring(sepIndex + 1), NumberStyles.Any, CultureInfo.InvariantCulture, out double naturalHeight))
                {
                    return new Size(naturalWidth, naturalHeight);
                }
            }

            return new Size(0, 0);
        }
    }
}
