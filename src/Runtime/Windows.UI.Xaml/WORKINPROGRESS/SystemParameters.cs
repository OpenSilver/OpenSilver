#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public static partial class SystemParameters
	{
		private static int _wheelScrollLines;
		public static int WheelScrollLines
		{
			get
			{
				return _wheelScrollLines;
			}
		}
	}
}
#endif