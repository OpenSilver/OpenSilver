#if WORKINPROGRESS
using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public sealed partial class TouchDevice : DependencyObject
	{
		public static readonly DependencyProperty IdProperty = DependencyProperty.Register("IdProperty", typeof(int), typeof(TouchDevice), new PropertyMetadata());
		public static readonly DependencyProperty DirectlyOverProperty = DependencyProperty.Register("DirectlyOverProperty", typeof(UIElement), typeof(TouchDevice), new PropertyMetadata());
		public int Id
		{
			get
			{
				return (int)this.GetValue(TouchDevice.IdProperty);
			}
		}

		public UIElement DirectlyOver
		{
			get
			{
				return (UIElement)this.GetValue(TouchDevice.DirectlyOverProperty);
			}
		}

		public TouchDevice()
		{
		}
	}
}
#endif