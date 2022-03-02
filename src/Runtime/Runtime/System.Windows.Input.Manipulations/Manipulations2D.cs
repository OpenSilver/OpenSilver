#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
using System;

namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	[Flags]
	public enum Manipulations2D
	{
		None = 0,
		TranslateX = 1,
		TranslateY = 2,
		Translate = 3,
		Scale = 4,
		Rotate = 8,
		All = 15
	}
}