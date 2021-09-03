#if MIGRATION

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices.Automation
{
	//
	// Summary:
	//     Provides access to registered Automation servers.
	[OpenSilver.NotImplemented]
	public static class AutomationFactory
	{
		//
		// Summary:
		//     Gets a value that indicates whether the Automation feature in Silverlight is
		//     available to the application.
		//
		// Returns:
		//     true if the Automation feature in Silverlight is available to the application;
		//     otherwise, false.
		[OpenSilver.NotImplemented]
		public static bool IsAvailable { get; }

		//
		// Summary:
		//     Activates and returns a reference to the registered Automation server with the
		//     specified programmatic identifier (ProgID).
		//
		// Parameters:
		//   progID:
		//     The ProgID of the registered automation server to activate.
		//
		// Returns:
		//     A late-bound reference to the specified automation server.
		//
		// Exceptions:
		//   T:System.Exception:
		//     No object was found registered for the specified progID.
		[OpenSilver.NotImplemented]
		public static dynamic CreateObject(string progID)
		{
			return default(dynamic);
		}
		
		//
		// Summary:
		//     Not yet implemented.
		//
		// Type parameters:
		//   T:
		//     The type of object to create.
		//
		// Returns:
		//     null.
		[EditorBrowsable(EditorBrowsableState.Never)]
		[OpenSilver.NotImplemented]
		public static T CreateObject<T>()
		{
			return default(T);
		}
		
		//
		// Summary:
		//     Gets an object that represents the specified event of the specified Automation
		//     server.
		//
		// Parameters:
		//   automationObject:
		//     A reference to the Automation server to retrieve an event for.
		//
		//   eventName:
		//     The name of the event to retrieve.
		//
		// Returns:
		//     An object that represents the specified event.
		[OpenSilver.NotImplemented]
		public static AutomationEvent GetEvent(dynamic automationObject, string eventName)
		{
			return default(AutomationEvent);
		}
		
		//
		// Summary:
		//     Gets a reference to the previously activated and currently running Automation
		//     server with the specified programmatic identifier (ProgID).
		//
		// Parameters:
		//   progID:
		//     The ProgID of the registered Automation server to retrieve a reference to.
		//
		// Returns:
		//     A late-bound reference to the specified Automation server.
		//
		// Exceptions:
		//   T:System.Exception:
		//     No object was found registered for the specified progID.
		[OpenSilver.NotImplemented]
		public static dynamic GetObject(string progID)
		{
			return default(dynamic);
		}
	}
}

#endif
