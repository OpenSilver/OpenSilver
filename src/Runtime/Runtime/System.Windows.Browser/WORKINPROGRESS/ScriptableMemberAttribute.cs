namespace System.Windows.Browser
{
	//
	// Summary:
	//     Indicates that a specific property, method, or event is accessible to JavaScript
	//     callers.
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
	[OpenSilver.NotImplemented]
	public sealed partial class ScriptableMemberAttribute : Attribute
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Browser.ScriptableMemberAttribute
		//     class.
		[OpenSilver.NotImplemented]
		public ScriptableMemberAttribute()
		{

		}

		//
		// Summary:
		//     Controls the generation of Silverlight plug-in helper methods that can be used
		//     to create wrappers around managed objects.
		//
		// Returns:
		//     true if the HTML bridge feature should automatically generate helper methods
		//     in the scope of the registered scriptable type; otherwise, false. The default
		//     is true.
		[OpenSilver.NotImplemented]
		public bool EnableCreateableTypes { get; set; }
		//
		// Summary:
		//     Gets or sets the name of a property, method, or event that is exposed to JavaScript
		//     code. By default, the script alias is the same as the name of the managed property,
		//     method, or event.
		//
		// Returns:
		//     The name of a property, method, or event.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The alias is set to an empty string.
		//
		//   T:System.ArgumentNullException:
		//     The alias is set to null.
		[OpenSilver.NotImplemented]
		public string ScriptAlias { get; set; }
	}
}
