using System.Windows;
using System;
using System.Collections.Generic;
using CSHTML5.Internal;
using OpenSilver.Internal;

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
            return (List<object>)INTERNAL_ToHtmlString(parent);
        }

        internal List<object> INTERNAL_ToHtmlString(DependencyObject parent)
        {
            if (ImageSource is BitmapImage bitmapImage)
            {
                string url = null;
                if (bitmapImage.UriSource != null)
                {
                    Uri sourceUri = bitmapImage.UriSource;
                    url = INTERNAL_UriHelper.ConvertToHtml5Path(sourceUri.OriginalString, parent as UIElement);
                }
                else if (bitmapImage.INTERNAL_StreamSource != null)
                {
                    url = "data:image/png;base64," + bitmapImage.INTERNAL_StreamAsBase64String;
                }
                else if (!string.IsNullOrEmpty(bitmapImage.INTERNAL_DataURL))
                {
                    url = bitmapImage.INTERNAL_DataURL;
                }

                if (url != null)
                {
                    return new List<object>(1)
                    {
                        $"linear-gradient(to right,rgba(255,255,255,{(1.0 - Opacity).ToInvariantString()}) 0 100%),url({url})",
                    };
                }
            }

            return new List<object>();
        }

	}
}
