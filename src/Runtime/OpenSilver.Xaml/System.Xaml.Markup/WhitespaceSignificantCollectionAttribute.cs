using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml.Markup
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class WhitespaceSignificantCollectionAttribute : Attribute
	{
	}
}
