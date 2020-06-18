#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public sealed partial class VisualStateChangedEventArgs : EventArgs
	{
		public static readonly DependencyProperty NewStateProperty = DependencyProperty.Register("NewStateProperty", typeof(VisualState), typeof(VisualStateChangedEventArgs), new PropertyMetadata());
		public VisualState NewState
		{
			get
			{
				return null;
			}

			set
			{
				return;
			}
		}

		public VisualStateChangedEventArgs()
		{
		}
	}
}
#endif