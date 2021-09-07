#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	public enum TouchAction
	{
		Down = 1,
		Move = 2,
		Up = 3
	}
}
