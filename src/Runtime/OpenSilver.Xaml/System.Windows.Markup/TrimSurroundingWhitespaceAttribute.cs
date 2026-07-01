using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Markup
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TrimSurroundingWhitespaceAttribute : Attribute
	{
	}
}
