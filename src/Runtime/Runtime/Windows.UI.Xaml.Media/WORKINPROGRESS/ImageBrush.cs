#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class ImageBrush : TileBrush
	{
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImageBrush), new PropertyMetadata());
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

		public ImageBrush()
		{
		}
	}
}
#endif