using System.Windows;
using System;
using System.Collections.Generic;
using CSHTML5.Internal;
using OpenSilver.Internal;
using System.Threading.Tasks;

#if MIGRATION
using System.Windows.Media.Imaging;
namespace System.Windows.Media
#else
using Windows.UI.Xaml.Media.Imaging;
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class ImageBrush : TileBrush, ICanConvertToCSSValues
	{
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImageBrush), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public ImageSource ImageSource
		{
			get
			{
				return (ImageSource)this.GetValue(ImageBrush.ImageSourceProperty);
			}

			set
			{
				this.SetValue(ImageBrush.ImageSourceProperty, value);
			}
		}

        [OpenSilver.NotImplemented]
		public ImageBrush()
		{
		}
        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            string url = INTERNAL_ToHtmlString(parent);
            if (url != null)
            {
                return new List<object>(1) { url };
            }

            return new List<object>();
        }

        internal string INTERNAL_ToHtmlString(DependencyObject parent)
        {
            string url = null;
            if (ImageSource is BitmapImage bitmapImage)
            {
                if (bitmapImage.UriSource != null)
                {
                    Uri sourceUri = bitmapImage.UriSource;
                    url = INTERNAL_UriHelper.ConvertToHtml5Path(sourceUri.OriginalString, parent as UIElement);
                }
                else
                {
                    url = GetImageUrl(bitmapImage);
                }
            }
            else if (ImageSource is BitmapSource image)
            {
                url = GetImageUrl(image);
            }

            if (url != null)
            {
                return $"linear-gradient(to right,rgba(255,255,255,{(1.0 - Opacity).ToInvariantString()}) 0 100%),url({url})";
            }

            return null;
        }

        private static string GetImageUrl(BitmapSource bitmapSource)
        {
            if (bitmapSource.INTERNAL_StreamSource != null)
            {
                return "data:image/png;base64," + bitmapSource.INTERNAL_StreamAsBase64String;
            }
            else if (!string.IsNullOrEmpty(bitmapSource.INTERNAL_DataURL))
            {
                return bitmapSource.INTERNAL_DataURL;
            }

            return null;
        }
	}
}
