using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml.ComponentModel
{
	/// <summary>
	/// This attribute defines method, which responcible for object serialization.
	/// The method have to returns bool type and hasn't arguments.
	/// </summary>
	[EnhancedXaml]
	internal class ShouldSerializeAttribute : Attribute
	{
		public ShouldSerializeAttribute(string methodName)
		{
			MethodName = methodName;
		}

		/// <summary>
		/// Method name which will responce for serialization
		/// </summary>
		public string MethodName { get; set; }
	}
}
