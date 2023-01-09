using System.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using OpenSilver.Internal;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class ImageBrush : TileBrush
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

        [Obsolete("Unused.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<object> ConvertToCSSValues(DependencyObject parent)
        {
            var task = GetDataStringAsync(parent as UIElement);

            // For any source except WriteableBitmap, the url is retrieved
            // synchronously. In case we are dealing with a WriteableBitmap,
            // we can only return the source if the WriteableBitmap has already
            // been fully loaded.
            if (task.IsCompleted)
            {
                return new List<object>(1) { task.Result };
            }
            
            return new List<object>();
        }

        internal async override Task<string> GetDataStringAsync(UIElement parent)
        {
            ImageSource source = ImageSource;
            if (source != null)
            {
                string url = await source.GetDataStringAsync(parent);
                if (!string.IsNullOrEmpty(url))
                {
                    string opacity = (1.0 - Opacity).ToInvariantString();
                    string size = ConvertStretch(Stretch);
                    return $"linear-gradient(to right, rgba(255, 255, 255, {opacity}) 0 100%), url({url}) center center / {size} no-repeat";
                }
            }

            return string.Empty;
        }

        private static string ConvertStretch(Stretch stretch)
            => stretch switch
            {
                Stretch.None => "auto",
                Stretch.Uniform => "contain",
                Stretch.UniformToFill => "cover",
                _ => "100% 100%",
            };
    }
}
