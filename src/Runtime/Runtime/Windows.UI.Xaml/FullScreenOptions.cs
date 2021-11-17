using System;

#if MIGRATION
#if WORKINPROGRESS
namespace System.Windows.Interop
#else
namespace System.Windows // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
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
		StaysFullScreenWhenUnfocused = 1 << 0
	}
}
