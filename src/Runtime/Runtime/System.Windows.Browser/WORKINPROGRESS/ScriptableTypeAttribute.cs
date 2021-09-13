using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Browser
{
	//
	// Summary:
	//     Indicates that all public properties, methods, and events on a managed type are
	//     available to JavaScript code when they are registered by using the System.Windows.Browser.HtmlPage.RegisterCreateableType(System.String,System.Type)
	//     method.
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	[OpenSilver.NotImplemented]
	public sealed partial class ScriptableTypeAttribute : Attribute
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Browser.ScriptableTypeAttribute
		//     class.
		[OpenSilver.NotImplemented]
		public ScriptableTypeAttribute()
		{

		}
	}
}
