using System.Windows.Media;
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class WriteableBitmap : BitmapSource
	{
		private Int32[] _pixels;
        [OpenSilver.NotImplemented]
		public Int32[] Pixels
		{
			get
			{
				return _pixels;
			}
		}

        [OpenSilver.NotImplemented]
		public WriteableBitmap(BitmapSource @source)
		{
			_pixels = new int[0];
		}

        [OpenSilver.NotImplemented]
		public WriteableBitmap(int @pixelWidth, int @pixelHeight)
		{
			_pixels = new int[0];
		}

        [OpenSilver.NotImplemented]
		public WriteableBitmap(UIElement @element, Transform @transform)
		{
			_pixels = new int[0];
		}

        [OpenSilver.NotImplemented]
		public void Render(UIElement @element, Transform @transform)
		{
		}

        [OpenSilver.NotImplemented]
		public void Invalidate()
		{
		}
	}
}
