#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support access by a UI automation client to
	//     controls or elements that can be moved, resized, or rotated within a two-dimensional
	//     space.
	public partial interface ITransformProvider
	{
		//
		// Summary:
		//     Gets a value that indicates whether the element can be moved.
		//
		// Returns:
		//     true if the element can be moved; otherwise, false.
		bool CanMove
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that indicates whether the element can be resized.
		//
		// Returns:
		//     true if the element can be resized; otherwise, false.
		bool CanResize
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that indicates whether the element can be rotated.
		//
		// Returns:
		//     true if the element can be rotated; otherwise, false.
		bool CanRotate
		{
			get;
		}

		//
		// Summary:
		//     Moves the control.
		//
		// Parameters:
		//   x:
		//     The absolute screen coordinates of the left side of the control.
		//
		//   y:
		//     The absolute screen coordinates of the top of the control.
		void Move(double x, double y);
		//
		// Summary:
		//     Resizes the control.
		//
		// Parameters:
		//   width:
		//     The new width of the window, in pixels.
		//
		//   height:
		//     The new height of the window, in pixels.
		void Resize(double width, double height);
		//
		// Summary:
		//     Rotates the control.
		//
		// Parameters:
		//   degrees:
		//     The number of degrees to rotate the control. A positive number rotates the control
		//     clockwise. A negative number rotates the control counterclockwise.
		void Rotate(double degrees);
	}
}
