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
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty NewStateProperty = DependencyProperty.Register("NewStateProperty", typeof(VisualState), typeof(VisualStateChangedEventArgs), new PropertyMetadata());
		[OpenSilver.NotImplemented]
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

		[OpenSilver.NotImplemented]
		public VisualStateChangedEventArgs()
		{
		}
	}
}
#endif