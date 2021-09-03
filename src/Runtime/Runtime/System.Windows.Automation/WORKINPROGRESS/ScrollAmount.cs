#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
	public enum ScrollAmount
	{
		//
		// Summary:
		//     Specifies that scrolling is performed in large decrements, which is equivalent
		//     to pressing the PAGE UP key or to clicking a blank part of a scrollbar. If the
		//     distance represented by the PAGE UP key is not a relevant amount for the control,
		//     or if no scrollbar exists, the value represents an amount equal to the size of
		//     the currently visible window.
		LargeDecrement = 0,
		//
		// Summary:
		//     Specifies that scrolling is performed in small decrements, which is equivalent
		//     to pressing an arrow key or to clicking the arrow button on a scrollbar.
		SmallDecrement = 1,
		//
		// Summary:
		//     Specifies that scrolling should not be performed.
		NoAmount = 2,
		//
		// Summary:
		//     Specifies that scrolling is performed in large increments, which is equivalent
		//     to pressing the PAGE DOWN key or to clicking a blank part of a scrollbar. If
		//     the distance represented by the PAGE DOWN key is not a relevant amount for the
		//     control, or if no scrollbar exists, the value represents an amount equal to the
		//     size of the currently visible region.
		LargeIncrement = 3,
		//
		// Summary:
		//     Specifies that scrolling is performed in small increments, which equivalent to
		//     pressing an arrow key or to clicking the arrow button on a scrollbar.
		SmallIncrement = 4
	}
}
