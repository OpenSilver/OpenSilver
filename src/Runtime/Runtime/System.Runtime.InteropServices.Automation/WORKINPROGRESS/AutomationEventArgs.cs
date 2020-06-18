#if WORKINPROGRESS
#if MIGRATION

namespace System.Runtime.InteropServices.Automation
{
	//
	// Summary:
	//     Provides data for the System.Runtime.InteropServices.Automation.AutomationEvent.EventRaised
	//     event.
	public sealed class AutomationEventArgs : EventArgs
	{
		//
		// Summary:
		//     Gets the event arguments from the Automation event.
		//
		// Returns:
		//     The event arguments from the Automation event.
		public object[] Arguments { get; }
	}
}

#endif
#endif