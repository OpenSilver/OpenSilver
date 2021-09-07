using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	[OpenSilver.NotImplemented]
	public static partial class SystemParameters
	{
		private static int _wheelScrollLines;
		[OpenSilver.NotImplemented]
		public static int WheelScrollLines
		{
			get
			{
				return _wheelScrollLines;
			}
		}
	}
}
