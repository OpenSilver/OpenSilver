#if WORKINPROGRESS
using System.Windows.Media;
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
	public sealed partial class WriteableBitmap : BitmapSource
	{
		private Int32[] _pixels;
		public Int32[] Pixels
		{
			get
			{
				return _pixels;
			}
		}

		public WriteableBitmap(BitmapSource @source)
		{
			_pixels = new int[0];
		}

		public WriteableBitmap(int @pixelWidth, int @pixelHeight)
		{
			_pixels = new int[0];
		}

		public WriteableBitmap(UIElement @element, Transform @transform)
		{
			_pixels = new int[0];
		}

		public void Render(UIElement @element, Transform @transform)
		{
		}

		public void Invalidate()
		{
		}
	}
}
#endif