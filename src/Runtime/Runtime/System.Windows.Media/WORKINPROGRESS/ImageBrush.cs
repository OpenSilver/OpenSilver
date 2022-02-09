using System.Windows;
using System;
using System.Collections.Generic;
using CSHTML5.Internal;

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
            List<object> returnValues = new List<object>();
            if (ImageSource != null)
            {
                if (ImageSource is BitmapImage)
                {

                    BitmapImage sourceAsBitmapImage = (BitmapImage)ImageSource;
                    if (sourceAsBitmapImage.UriSource != null)
                    {
                        Uri sourceUri = null;
                        sourceUri = ((BitmapImage)ImageSource).UriSource;
                        string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(sourceUri.OriginalString, parent as UIElement);
                        returnValues.Add("url('" + html5Path + "')");
                    }
                    else if (sourceAsBitmapImage.INTERNAL_StreamSource != null)
                    {
                        string dataUrl = "data:image/png;base64," + sourceAsBitmapImage.INTERNAL_StreamAsBase64String;
                        returnValues.Add("url(" + dataUrl + ")");
                    }
                    else if (!string.IsNullOrEmpty(sourceAsBitmapImage.INTERNAL_DataURL))
                    {
                        string dataUrl = sourceAsBitmapImage.INTERNAL_DataURL;
                        returnValues.Add("url(" + dataUrl + ")");
                    }
                }
            }
            return returnValues;
        }

	}
}
