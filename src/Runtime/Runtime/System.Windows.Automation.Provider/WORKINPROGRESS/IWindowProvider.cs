#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
	//
	// Summary:
	//     Exposes methods and properties to support access by a UI automation client to
	//     controls that provide fundamental window-based functionality within a traditional
	//     graphical user interface (GUI).
	public partial interface IWindowProvider
	{
		//
		// Summary:
		//     Gets the interaction state of the window.
		//
		// Returns:
		//     The interaction state of the control, as a value of the enumeration.
		WindowInteractionState InteractionState
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that specifies whether the window is modal.
		//
		// Returns:
		//     true if the window is modal; otherwise, false.
		bool IsModal
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that specifies whether the window is the topmost element in the
		//     z-order of layout.
		//
		// Returns:
		//     true if the window is topmost; otherwise, false.
		bool IsTopmost
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that specifies whether the window can be maximized.
		//
		// Returns:
		//     true if the window can be maximized; otherwise, false.
		bool Maximizable
		{
			get;
		}

		//
		// Summary:
		//     Gets a value that specifies whether the window can be minimized.
		//
		// Returns:
		//     true if the window can be minimized; otherwise, false.
		bool Minimizable
		{
			get;
		}

		//
		// Summary:
		//     Gets the visual state of the window.
		//
		// Returns:
		//     The visual state of the window, as a value of the enumeration.
		WindowVisualState VisualState
		{
			get;
		}

		//
		// Summary:
		//     Closes the window.
		void Close();
		//
		// Summary:
		//     Changes the visual state of the window (such as minimizing or maximizing it).
		//
		// Parameters:
		//   state:
		//     The visual state of the window to change to, as a value of the enumeration.
		void SetVisualState(WindowVisualState state);
		//
		// Summary:
		//     Blocks the calling code for the specified time or until the associated process
		//     enters an idle state, whichever completes first.
		//
		// Parameters:
		//   milliseconds:
		//     The amount of time, in milliseconds, to wait for the associated process to become
		//     idle.
		//
		// Returns:
		//     true if the window has entered the idle state; false if the timeout occurred.
		bool WaitForInputIdle(int milliseconds);
	}
}
