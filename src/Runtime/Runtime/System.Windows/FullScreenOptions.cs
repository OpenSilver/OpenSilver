using System;

namespace System.Windows.Interop
{
	/// <summary>
	/// Defines constants that indicate the behavior of full-screen mode. 
	/// </summary>
	[Flags]
	public enum FullScreenOptions
	{
		/// <summary>
		/// The application uses the default full-screen behavior.
		/// </summary>
		None = 0,
		/// <summary>
		/// The application does not exit full-screen mode when other applications gain focus. 
		/// </summary>
		StaysFullScreenWhenUnfocused = 1
	}
}
