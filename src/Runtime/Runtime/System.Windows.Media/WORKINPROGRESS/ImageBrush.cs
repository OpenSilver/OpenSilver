using System.Windows;
using System;

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
	}
}
