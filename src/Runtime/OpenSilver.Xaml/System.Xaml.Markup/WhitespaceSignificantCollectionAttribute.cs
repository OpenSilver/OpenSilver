using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xaml.Markup
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class WhitespaceSignificantCollectionAttribute : Attribute
	{
	}
}
